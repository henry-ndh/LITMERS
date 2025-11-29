import uuid
import asyncio
from typing import List, Optional, Any, Dict, Union, DefaultDict
from collections import defaultdict

from langchain_core.documents import Document
from app.logger import logger


class VectorStore:
    """
    """

    def __init__(self, collection: str = "litmers"):
        self.collection = collection
        # Lưu docs theo metadata["id"] -> list[Document]
        self._docs_by_meta_id: DefaultDict[str, List[Document]] = defaultdict(list)

        # Giữ thuộc tính giả cho compatibility (nếu code chỗ khác có truy cập)
        self.store = None
        self.record_manager = None
        logger.info("Initialized pseudo VectorStore for collection '%s'", collection)

    def embed_query(self, text: str) -> List[float]:
        """
        Pseudo embed: trả về vector rỗng.
        Nếu sau này cần, bạn có thể nối vào OpenAIEmbeddings thật ở đây.
        """
        if not isinstance(text, str):
            text = str(text)
        return []

    def search(
        self,
        query: str,
        k: int = 5,
        filters: Optional[dict] = None,
        return_score: bool = False,
    ) -> List[Any]:
        """

        """
        if not query:
            query = ""

        # Gom toàn bộ docs
        all_docs: List[Document] = []
        for meta_id, docs in self._docs_by_meta_id.items():
            all_docs.extend(docs)

        # Filter rất đơn giản
        def _match_filters(doc: Document) -> bool:
            if not filters:
                return True
            # Ví dụ chỉ xử lý filter {"id": {"$eq": "something"}}
            if "id" in filters:
                cond = filters["id"]
                if isinstance(cond, dict) and "$eq" in cond:
                    if doc.metadata.get("id") != cond["$eq"]:
                        return False
            return True

        matched: List[Document] = []
        for d in all_docs:
            if not _match_filters(d):
                continue
            if query.lower() in (d.page_content or "").lower():
                matched.append(d)

        # Cắt top k
        matched = matched[:k]

        if return_score:
            # Pseudo score: cho tất cả score = 0.0
            return [(d, 0.0) for d in matched]
        return matched

    def search_by_vector(
        self,
        query_or_embedding: Union[str, List[float]],
        k: int = 5,
        filters: Optional[dict] = None,
        return_score: bool = False,
    ) -> List[Any]:
        """
        Pseudo vector search:
        - Nếu input là string -> fallback sang search(text).
        - Nếu là list vector -> không dùng, chỉ trả về [] cho đơn giản.
        """
        if isinstance(query_or_embedding, str):
            return self.search(
                query=query_or_embedding,
                k=k,
                filters=filters,
                return_score=return_score,
            )

        if return_score:
            return []
        return []

    # ------------------------------------------------------------------
    # Indexing (add / update)
    # ------------------------------------------------------------------

    def add_documents(self, docs: List[Document], is_reset: bool = False) -> None:
        """
        Pseudo add:
        - Nếu is_reset=True -> clear toàn bộ collection trước.
        - Mỗi doc đảm bảo có metadata["id"], nếu không thì gán UUID4.
        - Lưu vào dict in-memory.
        """
        if not docs:
            logger.debug(
                "No documents provided to add to collection '%s'.",
                self.collection,
            )
            return

        if is_reset:
            self._docs_by_meta_id.clear()
            logger.debug(
                "Reset in-memory docs for collection '%s' before add.",
                self.collection,
            )

        for doc in docs:
            # Đảm bảo metadata["id"] tồn tại
            mid = doc.metadata.get("id")
            if not mid:
                mid = str(uuid.uuid4())
                doc.metadata["id"] = mid

            # Chuẩn hoá uuid.UUID -> str
            if isinstance(mid, uuid.UUID):
                mid = str(mid)
                doc.metadata["id"] = mid

            self._docs_by_meta_id[mid].append(doc)

        logger.debug(
            "Pseudo-indexed %d documents into '%s' (total groups: %d).",
            len(docs),
            self.collection,
            len(self._docs_by_meta_id),
        )

    async def aadd_documents(self, docs: List[Document], is_reset: bool = False) -> None:
        """
        Async wrapper cho add_documents.
        Dùng asyncio.to_thread để code gọi 'await' không bị lỗi.
        """
        if not docs:
            logger.debug(
                "No documents provided to add to collection '%s' (async).",
                self.collection,
            )
            return

        await asyncio.to_thread(self.add_documents, docs, is_reset)

    # ------------------------------------------------------------------
    # Delete theo metadata["id"]
    # ------------------------------------------------------------------

    def delete(self, ids: List[str]) -> None:
        """
        Xoá theo danh sách metadata['id'].
        """
        if not ids:
            logger.debug("No IDs provided for deletion.")
            return

        for mid in ids:
            self.delete_by_metadata_id(mid)

    def delete_by_metadata_id(self, metadata_id: str) -> None:
        """
        Xoá toàn bộ docs với metadata['id'] == metadata_id.
        """
        if metadata_id in self._docs_by_meta_id:
            count = len(self._docs_by_meta_id[metadata_id])
            del self._docs_by_meta_id[metadata_id]
            logger.debug(
                "Deleted %d in-memory docs for metadata.id = %s.",
                count,
                metadata_id,
            )
        else:
            logger.debug(
                "No in-memory docs found for metadata.id = %s.",
                metadata_id,
            )

    # ------------------------------------------------------------------
    # Stats
    # ------------------------------------------------------------------

    def count_documents(self) -> int:
        """
        Trả về số lượng documents đang giữ trong memory.
        """
        return sum(len(docs) for docs in self._docs_by_meta_id.values())
