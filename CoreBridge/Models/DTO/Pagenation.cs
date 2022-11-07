using Google.Api;
using Newtonsoft.Json;

namespace CodeBridge.Models.DTO
{
    public class Pagenation
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        public PageDto? PageDto { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }

        public Pagenation(PageDto page, int count)
        {
            Count = count;
            PageDto = page;
        }
        public int pageCount 
        { 
            get {
                int ret = 0;
                if (PageDto != null)
                    ret = (int)Math.Ceiling((float)Count / PageDto.PageSize);
                
                return ret;
            } 
        }
        public int currentPage
        {
            get
            {
                int ret = 0;
                if (PageDto != null)
                    ret = PageDto.StartPage;

                return ret;
            }
        }
        public int leftMostPage
        {
            get
            {
                int ret = 5;
                if (PageDto != null && PageDto.StartPage > ret)
                {
                    ret = PageDto.StartPage-ret;
                    
                }
                else
                {
                    ret = 0;
                }

                return ret;
            }
        }

        public int pageRange 
        {
            get
            {
                int ret = 5;

                if (PageDto != null && pageCount > PageDto.StartPage)
                {
                    ret = PageDto.StartPage + ret;

                }
                else
                {
                    ret = pageCount;
                }

                return ret;
            }
        }
    }
}
