namespace CoreBridge.Services.Interfaces
{
    public interface IRequestService
    {
        Task CopyOriginalBodyToHeader(HttpRequest req);
        Task RemoveHash(HttpRequest req);
#if DEBUG
        Task Debug_CopyBodyToHeader(HttpRequest req);
        byte[] GetDebugBodyCopyInBytesFromHeader(HttpRequest req);
        string GetDebugMsgpackBodyCopyInJsonStringFromHeader(HttpRequest req);
        string GetDebugBodyCopyInJsonStringFromHeader(HttpRequest req);

#endif
        byte[] GetOriginalBodyInBytesFromHeader(HttpRequest req);

        byte[] GetBodyHashInBytesFromHeader(HttpRequest req);

        string GetOriginalInJsonStringFromHeader(HttpRequest req);
    }
}
