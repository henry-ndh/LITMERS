import os
from functools import lru_cache
from typing import Optional

from sqlalchemy import create_engine, Engine
from sqlalchemy.orm import sessionmaker, scoped_session, Session

from app.configs import VECTOR_DB_CONNECTION as POSTGRES_URL
from app.logger import logger


# =========================
# Engine & Session Factory
# =========================

@lru_cache(maxsize=1)
def _get_engine() -> Engine:
    """
    Tạo 1 instance Engine duy nhất (lazy, thread-safe nhờ lru_cache).
    """
    if not POSTGRES_URL:
        raise ValueError(
            "POSTGRES_URL (VECTOR_DB_CONNECTION) is not set. "
            "Please check your .env or environment variables."
        )

    logger.info("Initializing SQLAlchemy engine (singleton) with connection pool...")
    engine = create_engine(
        POSTGRES_URL,
        pool_size=5,
        max_overflow=10,
        pool_timeout=30,
        pool_recycle=1800,
        pool_pre_ping=True,
    )
    return engine


@lru_cache(maxsize=1)
def _get_session_factory() -> sessionmaker:
    """
    Tạo sessionmaker 1 lần, dùng chung cho toàn app.
    """
    engine = _get_engine()
    return sessionmaker(bind=engine, autoflush=False, autocommit=False)


def get_sqlalchemy_engine() -> Engine:
    """
    Public API: trả về Engine singleton.
    """
    return _get_engine()


def get_db_session() -> scoped_session[Session]:
    """
    Public API: trả về scoped_session (mỗi thread/worker một Session riêng).
    """
    factory = _get_session_factory()
    return scoped_session(factory)


# =========================
# Shutdown / Cleanup
# =========================

async def close_db_connections() -> None:
    """
    Dispose connection pool và clear cache để engine/session có thể được tạo lại nếu cần.
    Gọi hàm này trong lifespan shutdown của FastAPI.
    """
    if _get_engine.cache_info().currsize > 0:
        engine = _get_engine()
        logger.info("Disposing SQLAlchemy engine connection pool...")
        engine.dispose()
        logger.info("SQLAlchemy engine disposed.")

    # Clear cache để lần sau có thể tạo engine/session mới nếu app khởi động lại trong cùng process
    _get_engine.cache_clear()
    _get_session_factory.cache_clear()
