using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoMultiPortal : Abstract.AbstractIntegracaoREST
    {

        #region Atributos privados

        private static IntegracaoMultiPortal Instance;
        private static readonly string nameConfigSection = "MultiPortal";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a MultiPortal

        private string token;
        private DateTime tokenExpiration;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_APPID = "Appid";
        private const string URI_HANDSHAKE = "seguranca/logon";
        private const string URI_ULTIMA_POSICAO = "posicoes/ultimaPosicao";
        #endregion

        #region Construtor privado

        private IntegracaoMultiPortal(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiPortal, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        public static IntegracaoMultiPortal GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoMultiPortal(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        override protected void ComplementarConfiguracoes()
        {

        }

        override protected void Validar()
        {

        }

        override protected void Preparar()
        {

        }

        override protected void Executar(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao)
        {
            this.conta = contaIntegracao;
            ObtemOuRevalidaToken();
            IntegrarPosicao();
        }

        #endregion

        #region Métodos privados

        private void IntegrarPosicao()
        {
            Log($"Consultando posicoes", 2);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();

            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            if (TokenEstaValido())
            {
                try
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal.Veiculo> veiculos = ObterUltimasPosicoes();
                    int total = veiculos.Count;
                    Log($"Recebidas {total} ultimas posicoes de veiculos", 3);

                    if (total > 0)
                    {
                        for (int i = 0; i < total; i++)
                        {
                            int totalDispositivos = veiculos[i].dispositivos.Count;
                            for (int j = 0; j < totalDispositivos; j++)
                            {
                                int totalPosicoes = veiculos[i].dispositivos[j].posicoes.Count;
                                for (int k = 0; k < totalPosicoes; k++)
                                {
                                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                                    {
                                        Data = Util.ObterDataPelosMilisegundos(veiculos[i].dispositivos[j].posicoes[k].dataProcessamento),
                                        DataVeiculo = Util.ObterDataPelosMilisegundos(veiculos[i].dispositivos[j].posicoes[k].dataEquipamento),
                                        IDEquipamento = veiculos[i].id.ToString(),
                                        Placa = veiculos[i].placa,
                                        Descricao = veiculos[i].dispositivos[j].posicoes[k].endereco,
                                        Latitude = veiculos[i].dispositivos[j].posicoes[k].latitude,
                                        Longitude = veiculos[i].dispositivos[j].posicoes[k].longitude,
                                        Velocidade = veiculos[i].dispositivos[j].posicoes[k].velocidade,
                                        Ignicao = ExtrairComponenteInt(veiculos[i].dispositivos[j].posicoes[k].componentes, "Ignicao"),
                                        SensorTemperatura = false,
                                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.MultiPortal
                                    });
                                }
                            }
                        }
                        Log($"{posicoes.Count} posicoes", 3);
                    }
                }
                catch (Exception ex)
                {
                    Log("Erro ObterUltimasPosicoes " + ex.Message, 3);
                }
            }
            else
            {
                Log("Token invalido ", 3);
            }
            return posicoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal.Veiculo> ObterUltimasPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal.Veiculo> veiculos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal.Veiculo>();
            string bodyResponseUltimaPosicao = Request(URI_ULTIMA_POSICAO);

            Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal.ResponseVeiculo responseUltimaPosicao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal.ResponseVeiculo>(bodyResponseUltimaPosicao);
            if (responseUltimaPosicao.status == "OK")
            {
                veiculos = responseUltimaPosicao.Veiculos;
            }
            else
            {
                Log($"Falha ao buscar as ultimas posicoes {responseUltimaPosicao.status} = {responseUltimaPosicao.responseMessage}", 3);
            }
            return veiculos;
        }

        private void ObterToken()
        {
            Log("Obtendo Token", 2);

            Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal.Handshake handshake = new Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal.Handshake();
            handshake.username = this.conta.Usuario;
            handshake.password = this.conta.Senha;
            handshake.appid = ObterAppId();
            
            string bodyRequestHandshake = JsonConvert.SerializeObject(handshake, Formatting.Indented);

            string bodyResponseHandshake = Request(URI_HANDSHAKE, bodyRequestHandshake);
            Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal.ResponseHandshake responseHandshake = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal.ResponseHandshake>(bodyResponseHandshake);
            if (responseHandshake.status == "OK")
            {
                this.token = responseHandshake.Handshake.token;
                this.tokenExpiration = Util.ObterDataPelosMilisegundos((long)responseHandshake.Handshake.expiration);
            }
            else
            {
                this.token = null;
                this.tokenExpiration = DateTime.MinValue;
                Log($"Falha na autenticação {responseHandshake.status} = {responseHandshake.responseMessage}", 3);
            }

        }

        private void ObtemOuRevalidaToken()
        {
            if (!TokenEstaValido()) ObterToken();
        }

        private bool TokenEstaValido()
        {
            return (!string.IsNullOrWhiteSpace(this.token) && this.tokenExpiration > DateTime.Now);
        }

        private string Request(string uri, string body = null)
        {
            string response = "";

            string url = ObterUrl(uri);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            request.Headers["token"] = this.token;
            request.Headers["Cache-Control"] = "no-cache";
            request.Accept = "application/json";
            request.ContentType = "application/json";

            DateTime inicio = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(body))
            {
                byte[] sendData = Encoding.UTF8.GetBytes(body);
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(sendData, 0, sendData.Length);
                requestStream.Flush();
                requestStream.Dispose();
            }

            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                using (var responseStream = resp.GetResponseStream())
                using (var responseStreamReader = new StreamReader(responseStream))
                {
                    response = responseStreamReader.ReadToEnd();
                }
                Log($"Requisicao {uri}", inicio, 3);

                switch (resp.StatusCode)
                {
                    case HttpStatusCode.OK: break;
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Requer autenticação.");
                    case HttpStatusCode.Forbidden:
                        throw new Exception("Acesso a requisição negada.");
                    default:
                        throw new Exception("Erro na requisicao: HTTP Status " + resp.StatusCode);
                }
            }

            return response;
        }

        private string ObterUrl(string uri, NameValueCollection requestParams = null)
        {
            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);

            if (!string.IsNullOrWhiteSpace(uri))
                url += uri;

            string requestParamsEncoded = EncodeRequestParams(requestParams);
            if (!string.IsNullOrWhiteSpace(requestParamsEncoded))
                url += "?" + requestParamsEncoded;

            return url;
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

        private string ObterAppId()
        {
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_APPID, this.conta.ListaParametrosAdicionais);       

            return value;
        }

        private int ExtrairComponenteInt(List<Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal.Componente> componentes, string nomeComponente)
        {
            string valorString = ExtrairComponente(componentes, nomeComponente);
            int valor = 0;
            try
            {
                valor = int.Parse(valorString);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao converter componente MultiPortal para inteiro: {ex.ToString()}", "CatchNoAction");
            }
            return valor;
        }

        private string ExtrairComponente(List<Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal.Componente> componentes, string nomeComponente)
        {
            int total = componentes.Count;
            for (int i = 0; i < total; i++)
            {
                if (componentes[i].nome.Equals(nomeComponente))
                {
                    return componentes[i].valor;
                }
            }
            return "";
        }

        #endregion
    }
}


