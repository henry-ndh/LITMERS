import json
from decimal import Decimal


class DecimalEncoder(json.JSONEncoder):
    """
    Custom JSON encoder để chuyển đổi Decimal thành float.
    """
    def default(self, obj):
        if isinstance(obj, Decimal):
            return float(obj)  # Chuyển Decimal thành float
        return super().default(obj)
