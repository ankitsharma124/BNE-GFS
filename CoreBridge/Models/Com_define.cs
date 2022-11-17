namespace CoreBridge.Models
{
    //システムで使う共通の定数をまとめたクラス
    public class Com_define
    {
    }

    /// <summary>
    /// プラットフォーム
    /// </summary>
    public enum PType
    {
        PLATFORM_PSN,       //PSN
        PLATFORM_XBOX,      //XBox
        PLATFORM_STEAM,     //Steam
        PLATFORM_SWITCH,    //Switch
        PLATFORM_IOS,       //iOS
        PLATFORM_ANDROID,   //Android
        PLATFORM_PS5,       //PS5
        PLATFORM_XBOXSX,    //XboxSX
        PLATFORM_GAMEPASS,  //Gamepass for PC
    }

    /// <summary>
    /// チェックワードの判定種別
    /// </summary>
    public enum WType
    {
        WORDTYPE_WORD,
        WORDTYPE_NAME
    }

    /// <summary>
    /// KPIの同意
    /// </summary>
    public enum KPIType
    {
        KPI_COLLECTION_DISAGREEMENT,    //非同意
        KPI_COLLECTION_AGREE,           //同意
        KPI_COLLECTION_DEFAULT = 999    //ユーザー作成時の初期値
    }
}
