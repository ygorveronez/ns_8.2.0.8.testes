using Infrastructure.Services.Cache;
using Newtonsoft.Json.Linq;
using Servicos.Cache;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Servicos
{
    public class TokenSettings
    {
        public Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte config_;
        private string tokenUri;

        public TokenSettings(Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte config, TokenSettingsService tokenService)
        {
            this.ClientId = config.ClientId;
            this.TenantId = config.TenantId;
            this.clientSecret = config.ClientSecret;
            this.RedirectUri = config.RedirectUri;
            this.Email = config.Email;
            this.Password = config.Senha;
            this.PopServer = config.Pop3;
            this.PopPort = config.PortaSmtp;
            this.config_ = config;
            this.msgErro = "";

            //TODO (cache): Individualizar tokens no cache (tirar lista)
            var tokens = tokenService.Obter();

            var token = tokens.FirstOrDefault(x => x.Email == config.Email);
            if (token == null)
            {
                tokens.Add(this);
                this.TokenExpires = DateTime.MinValue;
                this.AccessToken = "";
                token = this;
            }
            else
            {
                token.msgErro = "";
            }
            this.GetRemoteAccessToken(token);
        }

        private bool Update(TokenSettings token)
        {
            bool change = false;
            if (token.clientSecret != this.clientSecret || token.Password != this.Password || token.Email != this.Email)
            {
                token.clientSecret = this.clientSecret;
                token.Password = this.Password;
                token.AccessToken = "";
                this.AccessToken = "";
                change = true;
            }
            token.ClientId = this.ClientId;
            token.TenantId = this.TenantId;
            token.ObjectId = this.ObjectId;
            token.clientSecret = this.clientSecret;
            token.RedirectUri = this.RedirectUri;
            token.Email = this.Email;
            token.Password = this.Password;
            token.PopServer = this.PopServer;
            token.PopPort = this.PopPort;
            token.scope = this.scope;
            return change;
        }

        public string ClientId { get; set; }
        public string TenantId { get; set; }
        public string ObjectId { get; set; }
        public string clientSecret { get; set; }
        public string RedirectUri { get; set; }
        private string AccessToken { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PopServer { get; set; }
        public int PopPort { get; set; }
        public string scope { get; set; }
        public string code { get; set; }
        private DateTime TokenExpires { get; set; }
        public string msgErro { get; set; }

        public void SetTokenExpires(int tokenExpiresInSeconds)
        {
            this.TokenExpires = DateTime.Now.AddSeconds(tokenExpiresInSeconds);
        }
        public bool TokenExpired()
        {
            if (DateTime.Now > this.TokenExpires)
                return true;
            else
                return false;
        }

        public string GetAccessToken()
        {
            return this.AccessToken;
        }
        internal void GetRemoteAccessToken(TokenSettings token)
        {
            if (this.Update(token) || token.TokenExpired() || string.IsNullOrEmpty(token.AccessToken))
            {
                if (string.IsNullOrEmpty(token.scope))
                    token.scope = "https://outlook.office.com/IMAP.AccessAsUser.All%20offline_access%20email%20openid";

                tokenUri = "https://login.microsoftonline.com/" + this.TenantId + "/oauth2/v2.0/token";
                string tokenRequestBody = string.Format("scope={0}&redirect_uri={1}&client_id={2}&client_secret={3}&username={4}&password={5}&grant_type=password",
                token.scope, Uri.EscapeDataString(this.RedirectUri), this.ClientId, this.clientSecret, this.Email, this.Password);

                HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(this.tokenUri);
                tokenRequest.Method = "POST";
                tokenRequest.ContentType = "application/x-www-form-urlencoded";
                tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

                byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
                tokenRequest.ContentLength = _byteVersion.Length;

                Stream stream = tokenRequest.GetRequestStream();
                stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
                stream.Close();
                try
                {
                    // gets the response
                    WebResponse tokenResponse = tokenRequest.GetResponse();
                    using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                    {
                        JObject json = JObject.Parse(reader.ReadToEnd());
                        this.AccessToken = token.AccessToken = (string)json["access_token"];
                        this.SetTokenExpires((int)json["expires_in"]);
                    }

                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        var response = ex.Response as HttpWebResponse;
                        if (response != null)
                        {
                            this.msgErro = "HTTP: " + response.StatusCode;
                            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                            {
                                this.msgErro += reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            else
            {
                this.AccessToken = token.AccessToken;
            }
        }

    }

}