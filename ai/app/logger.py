import logging
import sys
from typing import Optional
from logging.handlers import RotatingFileHandler


def get_logger(
        name: Optional[str] = None,
        level: int = logging.INFO,
        fmt: str = '%(asctime)s - %(name)s - %(levelname)s - %(message)s',
        datefmt: str = '%Y-%m-%d %H:%M:%S',
        logfile: Optional[str] = None,
        max_bytes: int = 10 * 1024 * 1024,
        backup_count: int = 5
) -> logging.Logger:
    """
    """
    logger = logging.getLogger(name)
    logger.setLevel(level)

    formatter = logging.Formatter(fmt=fmt, datefmt=datefmt)

    if not logger.handlers:
        console_handler = logging.StreamHandler(sys.stdout)
        console_handler.setLevel(level)
        console_handler.setFormatter(formatter)
        logger.addHandler(console_handler)

        if logfile:
            file_handler = RotatingFileHandler(
                logfile,
                maxBytes=max_bytes,
                backupCount=backup_count,
                encoding='utf-8'
            )
            file_handler.setLevel(level)
            file_handler.setFormatter(formatter)
            logger.addHandler(file_handler)

    return logger


logger = get_logger(__name__, logfile='app.log')
