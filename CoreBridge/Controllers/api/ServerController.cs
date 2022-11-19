using Microsoft.AspNetCore.Mvc;

namespace CoreBridge.Controllers.api
{
    public class ServerController : ControllerBase
    {
        /** @var string ユーザー情報のキャッシュキー */
        private const string USERINFO_SERVER_KEY_FORMAT = "server.%s_%d";

        /*
        protected override void ProcessParams()
        {
            var titleCode = GetTitleCodeByUrl();


            // 一時的にURLのタイトルコードでキャッシュを効かせる 41
            //$this->load->library('QueryCache', ['title_cd' => $title_cd]);


        }*/
    }
}
