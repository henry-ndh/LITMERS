from app.utils.db_connection import MySQLConnection
from app.core.config import settings

def test_connection():
    db_config = {
        "host": settings.MYSQL_HOST,
        "user": settings.MYSQL_USER,
        "password": settings.MYSQL_PASSWORD,
        "database": settings.MYSQL_DATABASE,
        "port": settings.MYSQL_PORT,
    }
    mysql_conn = MySQLConnection(**db_config)
    try:
        mysql_conn.connect()
        assert mysql_conn.connection is not None
        print("Kết nối thành công!")
    finally:
        mysql_conn.close()


test_connection()
