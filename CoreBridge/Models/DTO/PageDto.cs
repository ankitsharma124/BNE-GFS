using Newtonsoft.Json;

namespace CodeBridge.Models.DTO
{
    /***
     * Pagenation用Dto
     * 
     */
    public class PageDto
    {
        public PageDto()
        {

        }

        public PageDto(int startPage,int pageSize)
        {
            StartPage = startPage;
            PageSize = pageSize;
        }


        [JsonProperty("start_page")]
        public int StartPage { get; set; }

        [JsonProperty("page_size")]
        public int PageSize { get; set; } = 10;
    }
}
