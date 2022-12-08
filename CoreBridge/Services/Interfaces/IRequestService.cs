namespace CoreBridge.Services.Interfaces
{
    public interface IRequestService
    {
        void CheckApi(HttpRequest req);
        void CheckUri(HttpRequest req);
#if DEBUG

#endif
    }
}
