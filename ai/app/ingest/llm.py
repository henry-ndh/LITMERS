from typing import List, Optional, Union, Dict, Any
from langchain_openai import ChatOpenAI

from app.constants import CHAT_MODEL


class DynamicChatModel:
    """
    Dùng:
        model = DynamicChatModel(
            model_name="gpt-4o-mini",
            api_key="YOUR_OPENAI_API_KEY",
            # base_url="https://api.openai.com/v1",  # nếu cần override
        )

        resp = model.invoke("Hello")
        print(resp.content)
    """

    def __init__(
        self,
        model_name: str = CHAT_MODEL,
        temperature: float = 0.0,
        max_tokens: Optional[int] = None,
        timeout: Optional[float] = None,
        max_retries: int = 2,
        **kwargs: Any,
    ):
        """
        :param model_name: tên model OpenAI, ví dụ:
            - "gpt-4o-mini"
            - "gpt-4.1-mini"
            - "gpt-4.1"
        :param temperature: độ sáng tạo
        :param max_tokens: giới hạn token output
        :param timeout: timeout request
        :param max_retries: số lần retry
        :param kwargs: các tham số khác cho ChatOpenAI (api_key, base_url, tổ chức, ...)
        """
        self.model_name = model_name or "gpt-4o-mini"
        self.temperature = temperature
        self.max_tokens = max_tokens
        self.timeout = timeout
        self.max_retries = max_retries
        self.extra_kwargs = kwargs

        self._model = self._init_model()
        print(f"Using OpenAI model '{self.model_name}'")

    def _init_model(self) -> ChatOpenAI:
        """
        Khởi tạo instance ChatOpenAI.
        """
        return ChatOpenAI(
            model=self.model_name,
            temperature=self.temperature,
            max_tokens=self.max_tokens,
            timeout=self.timeout,
            max_retries=self.max_retries,
            **self.extra_kwargs,
        )

    def invoke(
        self,
        messages: Union[
            str,
            List[Union[tuple[str, str], Dict[str, Any]]],
        ],
        **invoke_kwargs: Any,
    ) -> Any:
        """
        Gọi model sync.

        messages có thể là:
        - string: "Hello"
        - list[("human", "Hi"), ("system", "You are ...")]
        - list[{"role": "...", "content": "..."}]
        """
        return self._model.invoke(messages, **invoke_kwargs)

    async def ainvoke(
        self,
        messages: Union[
            str,
            List[Union[tuple[str, str], Dict[str, Any]]],
        ],
        **invoke_kwargs: Any,
    ) -> Any:
        """
        Gọi model async.
        """
        if not hasattr(self._model, "ainvoke"):
            raise AttributeError("ChatOpenAI không hỗ trợ ainvoke() (phiên bản bạn dùng).")
        return await self._model.ainvoke(messages, **invoke_kwargs)

    def stream(
        self,
        messages: Union[
            str,
            List[Union[tuple[str, str], Dict[str, Any]]],
        ],
        **stream_kwargs: Any,
    ):
        """
        Stream token (sync).
        """
        if not hasattr(self._model, "stream"):
            raise AttributeError("ChatOpenAI không hỗ trợ stream().")
        return self._model.stream(messages, **stream_kwargs)

    async def astream(
        self,
        messages: Union[
            str,
            List[Union[tuple[str, str], Dict[str, Any]]],
        ],
        **stream_kwargs: Any,
    ):
        """
        Stream token (async).
        """
        if not hasattr(self._model, "astream"):
            raise AttributeError("ChatOpenAI không hỗ trợ astream().")
        return self._model.astream(messages, **stream_kwargs)

    def bind_tools(self, tools: list[Any], strict: bool = False, **bind_kwargs: Any) -> None:
        """
        Bind tools (function calling) vào model.
        """
        if not hasattr(self._model, "bind_tools"):
            raise AttributeError("ChatOpenAI không hỗ trợ bind_tools().")
        self._model = self._model.bind_tools(tools, strict=strict, **bind_kwargs)

    def with_structured_output(self, schema_model: Any) -> None:
        """
        Structured output với Pydantic / schema.
        """
        if not hasattr(self._model, "with_structured_output"):
            raise AttributeError("ChatOpenAI không hỗ trợ with_structured_output().")
        self._model = self._model.with_structured_output(schema_model)

    def get_underlying_model(self) -> ChatOpenAI:
        """
        Lấy instance ChatOpenAI gốc nếu cần xài trực tiếp.
        """
        return self._model
