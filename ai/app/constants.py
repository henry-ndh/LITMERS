from dotenv import load_dotenv
import os
load_dotenv()

MINIMUM_LENGTH_DES_ISSUE = int(os.getenv("MINIMUM_LENGTH_DES_ISSUE", 10))
CHAT_MODEL = os.getenv("CHAT_MODEL")
OPENAI_MODEL_NAME_EMBEDDING = os.getenv("OPENAI_MODEL_NAME_EMBEDDING", 'text-embedding-3-small')