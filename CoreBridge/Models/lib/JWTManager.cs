using LitJWT;
using LitJWT.Algorithms;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using System.Text;

namespace CoreBridge.Models.lib
{
    //JWTを使用するためのクラス
    //参考: https://github.com/Cysharp/LitJWT
    public class JWTManager
    {
        const string secret = "secret_goes_here";

        // Get recommended-size random key.
        private byte[] key = HS256Algorithm.GenerateRandomRecommendedKey();
        private JwtEncoder encoder;
        private string token = string.Empty;

        public JWTManager()
        {
            //set encoder type:コンストラクタで暗号化タイプを設定出来たらいいと思う.
            encoder = new JwtEncoder(new HS256Algorithm(key));
        }

        //以下サンプルコード
        /// <summary>
        /// Tokenの作成
        /// </summary>
        /// <param name="paysample"></param>
        public void CreateToken(PayloadSample paysample)
        {
            //Encode
            token = encoder.Encode(new PayloadSample { foo = paysample.foo, bar = paysample.bar }, TimeSpan.FromMinutes(30),
                (x, writer) => writer.Write(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(x))));
        }

        /// <summary>
        /// 作成されたTokenの取得
        /// </summary>
        /// <returns></returns>
        public Task<string> GetToken()
        {
            return Task.FromResult(token);
        }

        /// <summary>
        /// Tokenを文字列へ戻す
        /// </summary>
        /// <returns></returns>
        public Task<string> DecToken()
        {
            string retDecStr = string.Empty;

            // Create decoder, JwtDecoder is also thread-safe so recommend to store static/singleton.
            var decoder = new JwtDecoder(encoder.SignAlgorithm);

            // Decode and verify, you can check the result.
            var result = decoder.TryDecode(token,
                x => JsonConvert.DeserializeObject<PayloadSample>(Encoding.UTF8.GetString(x)), out var payload);
            if (result == DecodeResult.Success)
            {
                retDecStr = payload.foo + "." + payload.bar;
            }

            return Task.FromResult(retDecStr);
        }
        //サンプルココまで！！


        //JWTが必要な場合の処理は下記に追加していくイメージ

    }

    //Sample Class
    public class PayloadSample
    {
        public string foo;
        public string bar;
    }

    //下記にTokenに必要な要素を追加して行くイメージ

}
