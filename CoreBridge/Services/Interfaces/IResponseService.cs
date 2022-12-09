namespace CoreBridge.Services.Interfaces
{
    public interface IResponseService
    {
        int ResultNG { get; }
        int ResultOK { get; }

        Task ReturnBNErrorAsync(HttpResponse response, int statusCode);
        Task ReturnBNResponseAsync(HttpResponse response, object details, int result = -1, int status = -1);
    }
}