from fastapi import FastAPI
from contextlib import asynccontextmanager
from fastapi.middleware.cors import CORSMiddleware

from app.connections import get_sqlalchemy_engine
from app.utils.db_connection import mysql_pool, initialize_mysql_pool
from app.api.v1.ai import router as ai_router


@asynccontextmanager
async def lifespan(app: FastAPI):
    """
    Lifespan handler cho FastAPI.
    """
    print("Ứng dụng khởi động.")

    print("Khởi tạo MySQL Pool...")
    await initialize_mysql_pool()

    get_sqlalchemy_engine()
    # Chờ ứng dụng xử lý yêu cầu
    yield

    print("Ứng dụng dừng.")
    print("Đóng MySQL Pool...")
    await mysql_pool.close_all_connections()
    print("Đã đóng pool MySQL")


# Khởi tạo ứng dụng với lifespan
app = FastAPI(lifespan=lifespan)

# Cấu hình danh sách origin được phép
origins = [
    "http://localhost:3000",
]

# Thêm middleware CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=origins,
    allow_credentials=True,
    allow_methods=["*"],  # Cho phép tất cả các phương thức HTTP (GET, POST, ...)
    allow_headers=["*"],  # Cho phép tất cả các headers
)

# Đăng ký router cho API
app.include_router(ai_router, prefix="/api/v1/ai", tags=["ai"])


# Lớp kiểm tra trạng thái (health check)
@app.get("/", tags=["Health Check"])
def read_root():
    return {"status": "ok"}

@app.get("/healthz", include_in_schema=False, tags=["Health"])
def healthz():
    return {"status": "ok"}