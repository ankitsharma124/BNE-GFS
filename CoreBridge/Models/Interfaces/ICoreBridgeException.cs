namespace CoreBridge.Models.Interfaces
{
    /// <summary>
    ///　Exception基底インターフェース
    ///　Excpetionクラスの基底クラスに実装
    /// </summary>
    public interface ICoreBridgeException
    {
        /// <summary>
        ///　Code：エラーコード
        /// </summary>
        int Code { get; set; }

        /// <summary>
        ///　Action：Actionコード（エラーを受け取った後のクライアント側の動作を規定）
        /// </summary>
        int Action { get; set; }
    }
}
