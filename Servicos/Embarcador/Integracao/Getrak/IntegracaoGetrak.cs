using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace Servicos.Embarcador.Integracao.Getrak
{

    /**
     * http://apidoc.getrak.com/oauth-client_password/#!/obtencao-token/post_newkoauth_oauth_token
     * http://apidoc.getrak.com/services-PlatformClient/
     */

    public class IntegracaoGetrak
    {
        #region Atributos privados

        private static IntegracaoGetrak Instance;

        private string url;
        private string uriToken;
        private string uriLocalizacoes;
        private string chave;
        private string usuario;
        private string senha;

        #endregion

        #region Construtor privado

        private IntegracaoGetrak() {}

        #endregion

        #region Métodos públicos

        public static IntegracaoGetrak GetInstance()
        {
            if (Instance == null) Instance = new IntegracaoGetrak();
            return Instance;
        }

        public void SetURL(string url)
        {
            this.url = url;
        }

        public void SetURIToken(string uri)
        {
            this.uriToken = uri;
        }

        public void SetURILocalizacoes(string uri)
        {
            this.uriLocalizacoes = uri;
        }

        public void SetChave(string chave)
        {
            this.chave = chave;
        }

        public void SetUsuario(string usuario)
        {
            this.usuario = usuario;
        }

        public void SetSenha(string senha)
        {
            this.senha = senha;
        }

        public bool VerificarValidadeToken(Dominio.ObjetosDeValor.Embarcador.Integracao.Getrak.Token token)
        {
            return (token != null && !string.IsNullOrWhiteSpace(token.access_token));
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Getrak.Token ObterToken()
        {
            VerificarConfiguracoes();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Getrak.Token token = new Dominio.ObjetosDeValor.Embarcador.Integracao.Getrak.Token();

            // Requisição
           System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = true;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.url + this.uriToken);
            request.Method = "POST";

            /**
             * Content-Type: application/x-www-form-urlencoded
             * Content-Type: application/x-www-form-urlencoded
             * Accept: application/json
             * Accept: application/json
             * Authorization: Basic cHJpdmlhc29sdXRpb25zOjVhOGI4OWM5ODE3YQ==
             * Authorization: Basic cHJpdmlhc29sdXRpb25zOjVhOGI4OWM5ODE3YQ==
             * grant_type=password&username=multisoftware@priviasolutions&password=multisoftware
             * grant_type=password&username=multisoftware@priviasolutions&password=multisoftware&
             * 
             * https://api.getrak.com/newkoauth/oauth/token
             * https://api.getrak.com/newkoauth/oauth/token
            */

            // Headers das requisição
            request.Accept = "application/json";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers["Cache-Control"] = "no-cache, no-store, max-age=0, must-revalidate, no-store";
            request.Headers["Authorization"] = "Basic " + this.chave;

            NameValueCollection requestParams = new NameValueCollection();
            requestParams.Add("grant_type", "password");
            requestParams.Add("username", this.usuario);
            requestParams.Add("password", this.senha);
            string requestParamsEncoded = EncodeRequestParams(requestParams);

            byte[] sendData = Encoding.UTF8.GetBytes(requestParamsEncoded);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(sendData, 0, sendData.Length);
            requestStream.Flush();
            requestStream.Dispose();

            // Leitura da resposta
            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                using (var responseStream = resp.GetResponseStream())
                using (var responseStreamReader = new StreamReader(responseStream))
                {
                    string response = responseStreamReader.ReadToEnd();
                    token = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Getrak.Token>(response, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }

                // Verificação do StatusCode
                switch (resp.StatusCode)
                {
                    case HttpStatusCode.OK: break;
                    case HttpStatusCode.BadRequest:
                        throw new ServicoException("Não suportado.");
                    case HttpStatusCode.Unauthorized:
                        throw new ServicoException("Requer autenticação.");
                    case HttpStatusCode.Forbidden:
                        throw new ServicoException("Header inválido.");
                    default:
                        throw new ServicoException("Erro na requisicao: HTTP Status " + resp.StatusCode);
                }
            }

            return token;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.Getrak.Veiculo> BuscarLocalizacoes(Dominio.ObjetosDeValor.Embarcador.Integracao.Getrak.Token token)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Getrak.Veiculo> veiculos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Getrak.Veiculo>();

            if (VerificarValidadeToken(token))
            {

                // Requisição
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.url + this.uriLocalizacoes);
                request.Method = "GET";

                // Headers
                request.Accept = "application/json";
                request.Headers["Cache-Control"] = "no-cache, no-store, max-age=0, must-revalidate, no-store";
                request.Headers["Authorization"] = $"Bearer {token.access_token}";

                // Leitura da resposta
                using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                {
                    using (var responseStream = resp.GetResponseStream())
                    using (var responseStreamReader = new StreamReader(responseStream))
                    {
                        string response = responseStreamReader.ReadToEnd();
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Getrak.LocalizacoesResponse localizacoesResponse = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Getrak.LocalizacoesResponse>(response, new JsonSerializerSettings{ NullValueHandling = NullValueHandling.Ignore });
                        if (localizacoesResponse != null)
                        {
                            veiculos = localizacoesResponse.veiculos;
                        }
                    }

                    // Verificação do StatusCode
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.OK: break;
                        case HttpStatusCode.BadRequest:
                            throw new ServicoException("Não suportado.");
                        case HttpStatusCode.Unauthorized:
                            throw new ServicoException("Requer autenticação.");
                        case HttpStatusCode.Forbidden:
                            throw new ServicoException("Header inválido.");
                        default:
                            throw new ServicoException("Erro na requisicao: HTTP Status " + resp.StatusCode);
                    }
                }
            }
            else
            {
                throw new ServicoException("Token não encontrado.");
            }

            return veiculos;
        }

        #endregion

        #region Métodos privados

        private void VerificarConfiguracoes()
        {
            if (string.IsNullOrWhiteSpace(this.url)) throw new ServicoException("URL Getrak não definida");
            if (string.IsNullOrWhiteSpace(this.chave)) throw new ServicoException("Chave Getrak não definido");
            if (string.IsNullOrWhiteSpace(this.usuario)) throw new ServicoException("Usuário Getrak não definido");
            if (string.IsNullOrWhiteSpace(this.senha)) throw new ServicoException("Senha Getrak não definida");
        }

        private string EncodeRequestParams(NameValueCollection queryParams)
        {
            string url = string.Empty;
            if (queryParams != null && queryParams.Count > 0)
            {
                foreach (string key in queryParams)
                {
                    url += $"{key}={Uri.EscapeUriString(queryParams[key])}&";
                }
            }
            return url;
        }

        #endregion

    }

}
