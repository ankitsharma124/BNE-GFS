namespace CoreBridge.Services.Interfaces
{
    public interface IResponseService
    {
        bool GetUseJson();
        Task ReturnBNErrorAsync(int apiCode, HttpResponse response, int statusCode);

        Task ReturnBNResponseAsync(int apiCode, HttpResponse response, object details,
           Action<List<object>> fxCustomizeHeader = null, Func<object, object> fxCustomizeContent = null,
           int result = -1, int status = -1);

        int ResultOK { get; }
        int ResultNG { get; }

        Task<string> ReadResponseBody(HttpResponse response, bool isServerApiCall);
    }
}
