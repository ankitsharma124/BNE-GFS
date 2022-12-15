namespace CoreBridge.Models
{
    //システムで使う共通の定数をまとめたクラス
    public class Com_define
    {
    }

    /// <summary>
    /// ユーザー権限
    /// AdminUser:スーパーユーザー
    /// BneManager:BNE管理ユーザー
    /// Manager:ゲーム制作者側管理ユーザー
    /// Reference:参照のみ
    /// EditReference:編集権限＋参照
    /// </summary>
    public enum AdminUserRoleEnum
    {
        AdminUser,
        BneManager,
        Manager,
        Reference,
        EditReference
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
