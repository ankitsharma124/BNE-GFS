namespace CoreBridge.Services.Interfaces
{
    public interface IHashService
    {
        /// <summary>
        /// keyとbodyの組み合わせのhash値を返却
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        byte[] GetHashWithKey(string hashKey, byte[] body);
        /// <summary>
        /// keyとbodyの組み合わせのhash値を返却
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        byte[] GetHashWithKey(string hashKey, string body);
        /// <summary>
        /// 現状（sss）でHashkeyが設定されているか確認。
        /// ServerApiでのみ実行。
        /// </summary>
        /// <exception cref="BNException"></exception>
        void CheckTitleHasHashKey();
        /// <summary>
        /// sss上のhashを確認。
        /// 不整合ならException
        /// </summary>
        /// <exception cref="BNException"></exception>
        void CheckHash();
    }
}