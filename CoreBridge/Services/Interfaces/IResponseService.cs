namespace CoreBridge.Services.Interfaces
{
    public interface IResponseService
    {
        bool GetUseJson();
        Task ReturnBNErrorAsync(HttpResponse response, int statusCode);

        Task ReturnBNResponseAsync(HttpResponse response, object details,
           Action<List<object>> fxCustomizeHeader = null, Action<object> fxCustomizeContent = null,
           Func<int, int> fxGetApiStatus = null,
           int result = -1, int status = -1);

        int ResultOK { get; }
        int ResultNG { get; }
    }
}
