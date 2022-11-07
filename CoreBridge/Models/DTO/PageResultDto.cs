using Newtonsoft.Json;

namespace CodeBridge.Models.DTO
{
    public class PageResultDto<T>
    {
        [JsonProperty("result")]
        public List<T> Result { get; set; } = new();

        [JsonProperty("count")]
        public int Count { get; set; }
        public Pagenation? Pagenation { get; set; }
    }
}
