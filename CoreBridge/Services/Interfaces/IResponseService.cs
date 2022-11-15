namespace CoreBridge.Services.Interfaces
{
    public interface IResponseService
    {
        bool GetUseJson();
        Task ReturnBNErrorAsync(HttpResponse response, int statusCode);

        Task ReturnBNResponseAsync(HttpResponse response, object details,
           Action<object[]> fxCustomizeHeader = null, Action<object> fxCustomizeContent = null, int result = -1, int status = -1);

        int ResultOK { get; }
        int ResultNG { get; }
    }
}
