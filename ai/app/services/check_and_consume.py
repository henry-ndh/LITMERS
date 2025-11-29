# FR-042 – AI Rate Limiting
from dataclasses import dataclass
from datetime import datetime, timedelta, date
from app.configs import DAILY_LIMIT, PER_MINUTE_LIMIT
import aiomysql


@dataclass
class RateLimitInfo:
    remaining_daily: int
    remaining_minutely: int
    reset_day: datetime
    reset_minute: datetime


class RateLimitExceeded(Exception):
    """Ném ra khi user vượt quota AI."""
    def __init__(self, info: RateLimitInfo) -> None:
        self.info = info
        super().__init__("AI rate limit exceeded")


def _truncate_to_minute(dt: datetime) -> datetime:
    """Cắt datetime về đầu phút (bỏ giây / microsecond)."""
    return dt.replace(second=0, microsecond=0)


async def check_and_consume(
    conn: aiomysql.Connection,
    user_id: int,
    cost: int = 1,
) -> RateLimitInfo:
    """
    Service check + consume AI quota cho 1 user.

    - `conn`: kết nối MySQL (từ pool aiomysql của bạn).
    - `user_id`: id user trong bảng `users` (cột `Id` BIGINT).
    - `cost`: số “điểm” muốn trừ (thường là 1 cho mỗi call).
        + Nếu cost = 0: chỉ check, KHÔNG trừ quota (dùng khi trả cache).

    Lưu ý:
    - Hàm KHÔNG tự commit/rollback.
      Caller (endpoint) phải tự làm:
        try: await checkAndConsume(...); await conn.commit()
        except: await conn.rollback()
    """
    now = datetime.utcnow()
    today: date = now.date()
    minute_bucket = _truncate_to_minute(now)

    # Thời điểm reset:
    # - reset_day: 00:00 ngày hôm sau
    # - reset_minute: phút tiếp theo
    reset_day = datetime.combine(today + timedelta(days=1), datetime.min.time())
    reset_minute = minute_bucket + timedelta(minutes=1)

    async with conn.cursor() as cur:
        # 1) DAILY USAGE --------------------------------------
        await cur.execute(
            """
            SELECT id, `count`
            FROM ai_daily_usage
            WHERE user_id = %s AND `date` = %s
            FOR UPDATE
            """,
            (user_id, today),
        )
        daily_row = await cur.fetchone()

        if daily_row is None:
            # chưa có record -> insert mới với count = 0
            await cur.execute(
                """
                INSERT INTO ai_daily_usage (user_id, `date`, `count`, created_at, updated_at)
                VALUES (%s, %s, %s, UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))
                """,
                (user_id, today, 0),
            )
            daily_id = cur.lastrowid
            daily_count = 0
        else:
            daily_id = daily_row["id"]
            daily_count = daily_row["count"]

        # 2) MINUTE USAGE -------------------------------------
        await cur.execute(
            """
            SELECT id, `count`
            FROM ai_minute_usage
            WHERE user_id = %s AND minute_bucket = %s
            FOR UPDATE
            """,
            (user_id, minute_bucket),
        )
        minute_row = await cur.fetchone()

        if minute_row is None:
            await cur.execute(
                """
                INSERT INTO ai_minute_usage (user_id, minute_bucket, `count`, created_at, updated_at)
                VALUES (%s, %s, %s, UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))
                """,
                (user_id, minute_bucket, 0),
            )
            minute_id = cur.lastrowid
            minute_count = 0
        else:
            minute_id = minute_row["id"]
            minute_count = minute_row["count"]

        # 3) CHECK LIMITS -------------------------------------
        if daily_count + cost > DAILY_LIMIT or minute_count + cost > PER_MINUTE_LIMIT:
            info = RateLimitInfo(
                remaining_daily=max(0, DAILY_LIMIT - daily_count),
                remaining_minutely=max(0, PER_MINUTE_LIMIT - minute_count),
                reset_day=reset_day,
                reset_minute=reset_minute,
            )
            # KHÔNG update count, chỉ ném exception
            raise RateLimitExceeded(info)

        # 4) CONSUME (nếu cost > 0) ---------------------------
        new_daily = daily_count + cost
        new_minute = minute_count + cost

        # Nếu cost = 0 → giữ nguyên
        if cost > 0:
            await cur.execute(
                """
                UPDATE ai_daily_usage
                SET `count` = %s,
                    updated_at = UTC_TIMESTAMP(6)
                WHERE id = %s
                """,
                (new_daily, daily_id),
            )
            await cur.execute(
                """
                UPDATE ai_minute_usage
                SET `count` = %s,
                    updated_at = UTC_TIMESTAMP(6)
                WHERE id = %s
                """,
                (new_minute, minute_id),
            )
        else:
            new_daily = daily_count
            new_minute = minute_count

    return RateLimitInfo(
        remaining_daily=DAILY_LIMIT - new_daily,
        remaining_minutely=PER_MINUTE_LIMIT - new_minute,
        reset_day=reset_day,
        reset_minute=reset_minute,
    )
