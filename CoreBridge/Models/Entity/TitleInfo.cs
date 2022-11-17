using CoreBridge.Models.Interfaces;

namespace CoreBridge.Models.Entity
{
    //タイトルコード管理テーブル（仮）
    public class TitleInfo : CoreBridgeEntity, IAggregateRoot
    {
        private TitleInfo() { }

        public TitleInfo(string title_name, string title_cd, string trial_title_cd, int p_type, 
            string switch_app_id, string xbox_title_id, string ps_client_id, string ps_client_secret, 
            string steam_app_id, string steam_publisher_key, string dev_url, string qa_url, string prod_url)
        {
            Title_name = title_name;
            Title_cd = title_cd;
            Trial_title_cd = trial_title_cd;
            P_type = p_type;
            Switch_app_id = switch_app_id;
            Xbox_title_id = xbox_title_id;
            Ps_client_id = ps_client_id;
            Ps_client_secoret = ps_client_secret;
            Steam_app_id = steam_app_id;
            Steam_publisher_key = steam_publisher_key;
            Dev_url = dev_url;
            Qa_url = qa_url;
            Prod_url = prod_url;
        }

        public string Title_name { get; set; }
        public string Title_cd{ get; set; }
        public string Trial_title_cd { get; set; }
        public int P_type { get; set; }
        public string? Switch_app_id { get; set; }
        public string? Xbox_title_id { get; set; }
        public string? Ps_client_id { get; set; }
        public string? Ps_client_secoret { get; set; }
        public string? Steam_app_id { get; set; }
        public string? Steam_publisher_key { get; set; }
        public string? Dev_url { get; set; }
        public string? Qa_url { get; set; }
        public string? Prod_url { get; set; }
    }


}
