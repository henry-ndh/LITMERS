from fastapi import HTTPException

class NotFoundException(HTTPException):
    def __init__(self, name: str):
        super().__init__(status_code=404, detail=f"{name} not found")

class UnauthorizedException(HTTPException):
    def __init__(self):
        super().__init__(status_code=401, detail="Unauthorized access")
