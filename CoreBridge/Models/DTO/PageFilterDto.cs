using Newtonsoft.Json;

namespace CodeBridge.Models.DTO
{
    /**
     * Pagenation＋絞り込みする際に使用
     * 
     */
    public class PageFilterDto<T>
    {
        [JsonProperty("filter")]
        public T Filter { get; set; }

        [JsonProperty("page")]
        public PageDto PageDto { get; set; }
    }
}
