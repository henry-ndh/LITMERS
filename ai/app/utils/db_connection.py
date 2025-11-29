import aiomysql
from asyncio import Queue
from app.core.config import settings


class MySQLConnectionPool:
    def __init__(self, host, user, password, database, port=3306, pool_size=10, max_overflow=5):
        """
        Khởi tạo pool kết nối MySQL bất đồng bộ với khả năng mở rộng kết nối vượt quá pool_size bằng max_overflow.
        """
        self.host = host
        self.user = user
        self.password = password
        self.database = database
        self.port = port
        self.pool_size = pool_size
        self.max_overflow = max_overflow
        self.pool = Queue(maxsize=pool_size)
        self.connections_created = 0
        self.connections = []  # Danh sách quản lý kết nối được tạo ra

    async def initialize_pool(self):
        """
        Khởi tạo kết nối ban đầu trong pool.
        """
        for _ in range(self.pool_size):
            connection = await self._create_new_connection()
            await self.pool.put(connection)

    async def _create_new_connection(self):
        """
        Tạo một kết nối MySQL mới.
        """
        try:
            connection = await aiomysql.connect(
                host=self.host,
                user=self.user,
                password=self.password,
                db=self.database,
                port=self.port,
                cursorclass=aiomysql.DictCursor
            )
            self.connections_created += 1
            self.connections.append(connection)
            print(f"Tạo kết nối mới tới MySQL ({self.connections_created}/{self.pool_size + self.max_overflow}).")
            return connection
        except aiomysql.MySQLError as e:
            print(f"Lỗi khi tạo kết nối tới MySQL: {e}")
            raise

    async def get_connection(self):
        """
        Lấy một kết nối từ pool hoặc tạo mới nếu cần.
        Kiểm tra trạng thái của kết nối trước khi sử dụng.
        """
        try:
            if self.pool.empty() and self.connections_created < self.pool_size + self.max_overflow:
                # Nếu pool rỗng và vẫn còn khả năng tạo thêm kết nối
                connection = await self._create_new_connection()
                return connection

            # Lấy một kết nối từ pool
            connection = await self.pool.get()

            # Kiểm tra trạng thái của kết nối
            try:
                await connection.ping()
            except aiomysql.MySQLError as e:
                print(f"Kết nối từ pool bị lỗi: {e}. Đang tạo một kết nối mới.")
                connection = await self._create_new_connection()

            return connection

        except Exception as e:
            print(f"Lỗi khi lấy kết nối từ pool: {e}")
            raise Exception("Không thể lấy kết nối từ pool.") from e

    async def return_connection(self, connection):
        """
        Trả kết nối về pool sau khi sử dụng.
        """
        if connection:
            await self.pool.put(connection)

    async def close_all_connections(self):
        """
        Đóng tất cả kết nối trong pool một cách an toàn.
        """
        print("Đang đóng tất cả kết nối MySQL...")
        while self.connections:
            connection = self.connections.pop()
            try:
                connection.close()
                print("Đã đóng một kết nối MySQL.")
            except Exception as e:
                print(f"Lỗi khi đóng kết nối: {e}")

    async def check_pool_status(self):
        """
        Kiểm tra trạng thái của các kết nối trong pool.
        Nếu có kết nối bị lỗi, loại bỏ kết nối đó.
        """
        print("Kiểm tra trạng thái của pool MySQL...")
        for connection in self.connections:
            try:
                await connection.ping()
            except Exception as e:
                print(f"Kết nối lỗi, đóng kết nối: {e}")
                self.connections.remove(connection)
                connection.close()
                self.connections_created -= 1

    async def restart_pool_if_needed(self):
        """
        Kiểm tra và khởi động lại pool MySQL nếu cần.
        """
        try:
            # Kiểm tra trạng thái pool
            await self.check_pool_status()

            # Nếu không đủ kết nối trong pool, khởi tạo thêm
            if self.connections_created < self.pool_size:
                print("Đang khởi tạo lại kết nối bị thiếu...")
                await self.initialize_pool()
        except Exception as e:
            print(f"Lỗi khi kiểm tra/kích hoạt lại pool MySQL: {e}")
            print("Đang cố gắng khởi tạo lại toàn bộ pool...")
            await self.close_all_connections()
            await self.initialize_pool()
            print("Pool MySQL đã được khởi tạo lại.")


### **Lớp MySQLConnection**

class MySQLConnection:
    def __init__(self, connection_pool: MySQLConnectionPool):
        """
        Sử dụng pool để quản lý kết nối MySQL.
        """
        self.connection_pool = connection_pool

    async def execute_query(self, query, params=None):
        """
        Thực thi một truy vấn SQL.
        """
        connection = await self.connection_pool.get_connection()
        try:
            async with connection.cursor() as cursor:
                await cursor.execute(query, params)
                if query.strip().lower().startswith("select"):
                    result = await cursor.fetchall()
                    return result
                else:
                    await connection.commit()
                    print("Thực hiện truy vấn thành công!")
        except aiomysql.MySQLError as e:
            print(f"Lỗi khi thực thi truy vấn: {e}")
            print(query)
            print(params)
            await connection.rollback()
            raise
        finally:
            # Trả kết nối về pool
            await self.connection_pool.return_connection(connection)


# Khởi tạo pool MySQL
db_config = {
    "host": settings.MYSQL_HOST,
    "user": settings.MYSQL_USER,
    "password": settings.MYSQL_PASSWORD,
    "database": settings.MYSQL_DATABASE,
    "port": settings.MYSQL_PORT,
}

mysql_pool = MySQLConnectionPool(**db_config, pool_size=10, max_overflow=10)


async def ensure_mysql_pool_ready():
    """
    Đảm bảo pool MySQL sẵn sàng trước khi ứng dụng chạy.
    """
    try:
        print("Đảm bảo pool MySQL sẵn sàng...")
        await mysql_pool.restart_pool_if_needed()
        print("Pool MySQL đã sẵn sàng.")
    except Exception as e:
        print(f"Lỗi khi đảm bảo pool MySQL: {e}")
        raise Exception("Không thể đảm bảo trạng thái của pool MySQL.")


async def initialize_mysql_pool():
    """
    Khởi tạo pool MySQL bất đồng bộ.
    """
    try:
        # Đảm bảo pool sẵn sàng
        await ensure_mysql_pool_ready()
    except Exception as e:
        print(f"Lỗi khi đảm bảo pool MySQL sẵn sàng: {e}")
        raise
    print("Đã khởi tạo lại pool MySQL.")

async def get_db_conn() -> aiomysql.Connection:
    conn = await mysql_pool.get_connection()
    try:
        yield conn
    finally:
        await mysql_pool.return_connection(conn)
