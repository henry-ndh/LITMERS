from pydantic_settings import BaseSettings, SettingsConfigDict
import os


class Settings(BaseSettings):
    model_config = SettingsConfigDict(
        env_file=".env",
        extra="ignore",
    )

    # MySQL
    MYSQL_HOST: str = "localhost"
    MYSQL_USER: str = "root"
    MYSQL_PASSWORD: str = "password"
    MYSQL_DATABASE: str = "database"
    MYSQL_PORT: int = 3306

    # Redis #Todo add later
    REDIS_URL: str = "redis://localhost:6379/0"

    # JWT
    JWT_SECRET_KEY: str = "secret"
    JWT_ALGORITHM: str = "HS256"

settings = Settings()


# Kiểm tra giá trị
print("MYSQL_HOST:", settings.MYSQL_HOST)
print("MYSQL_USER:", settings.MYSQL_USER)
print("MYSQL_PASSWORD:", settings.MYSQL_PASSWORD)
print("MYSQL_DATABASE:", settings.MYSQL_DATABASE)
print("MYSQL_PORT:", settings.MYSQL_PORT)

