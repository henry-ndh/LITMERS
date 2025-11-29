from dotenv import load_dotenv
import os
from urllib.parse import quote_plus
load_dotenv()

DAILY_LIMIT = int(os.getenv('DAILY_LIMIT', 100))
PER_MINUTE_LIMIT = int(os.getenv('PER_MINUTE_LIMIT', 10))

VECTOR_DB_USER = os.getenv("VECTOR_DB_USER", "postgres")
VECTOR_DB_PASSWORD = os.getenv("VECTOR_DB_PASSWORD", "")
VECTOR_DB_HOST = os.getenv("VECTOR_DB_HOST", "localhost")
VECTOR_DB_PORT = os.getenv("VECTOR_DB_PORT", "5432")

VECTOR_DB_NAME = os.getenv("VECTOR_DB_NAME") or os.getenv("POSTGRES_DB", "postgres")
VECTOR_DB_CONNECTION = (
    "postgresql+psycopg://"
    f"{quote_plus(VECTOR_DB_USER)}:"
    f"{quote_plus(VECTOR_DB_PASSWORD)}@"
    f"{VECTOR_DB_HOST}:{VECTOR_DB_PORT}/{VECTOR_DB_NAME}"
)