using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{

    public class IntegracaoGVSAT : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoGVSAT Instance;
        private static readonly string nameConfigSection = "GVSAT";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a Ravex

        private string token;
        private DateTime tokenExpiration;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_APPID = "appid";
        private const string URI_HANDSHAKE = "seguranca/logon";
        private const string URI_ULTIMA_POSICAO = "posicoes/ultimaPosicao";
        #endregion

        #region Construtor privado

        private IntegracaoGVSAT(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GVSAT, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoGVSAT GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoGVSAT(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        /**
         * Complementa configurações gerais
         */
        override protected void ComplementarConfiguracoes()
        {

        }

        /**
         * Confirmação de parâmetros corretos, executada apenas uma vez
         */
        override protected void Validar()
        {

        }

        /**
         * Preparação para iniciar a execução, executada apenas uma vez
         */
        override protected void Preparar()
        {

        }

        /**
         * Execução principal de cada iteração da thread, repetida infinitamente
         */
        override protected void Executar(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao)
        {
            this.conta = contaIntegracao;
            ObtemOuRevalidaToken();
            IntegrarPosicao();
        }

        #endregion

        #region Métodos privados

        /**
         * Executa a integração das posições, consultando no WebService e registrando na tabela T_POSICAO
         */
        private void IntegrarPosicao()
        {
            Log($"Consultando posicoes", 2);

            // Busca as posições do WebService
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();

            // Registra as posições recebidas dos veículos
            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);
        }

        /**
         * Busca as posições de todos os veículos
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            if (TokenEstaValido())
            {
                try
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT.Veiculo> veiculos = ObterUltimasPosicoes();
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
                                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.GVSat
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

        /**
         * Requisição ao servico "posicoes/ultimaPosicao"
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT.Veiculo> ObterUltimasPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT.Veiculo> veiculos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT.Veiculo>();
            string bodyResponseUltimaPosicao = Request(URI_ULTIMA_POSICAO);

            Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT.ResponseVeiculo responseUltimaPosicao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT.ResponseVeiculo>(bodyResponseUltimaPosicao);
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

        /**
         * Autentica e obtém o token para as demais requisições
         */
        private void ObterToken()
        {
            Log("Obtendo Token", 2);

            Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT.Handshake handshake = new Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT.Handshake();
            handshake.username = this.conta.Usuario;
            handshake.password = this.conta.Senha;
            handshake.appid = ObterAppId();

            string bodyRequestHandshake = JsonConvert.SerializeObject(handshake, Formatting.Indented);

            string bodyResponseHandshake = Request(URI_HANDSHAKE, bodyRequestHandshake);
            Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT.ResponseHandshake responseHandshake = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT.ResponseHandshake>(bodyResponseHandshake);
            if (responseHandshake.status == "OK")
            {
                this.token = responseHandshake.Handshake.token;
                this.tokenExpiration = Util.ObterDataPelosMilisegundos(responseHandshake.Handshake.expiration);
            }
            else
            {
                this.token = null;
                this.tokenExpiration = DateTime.MinValue;
                Log($"Falha na autenticação {responseHandshake.status} = {responseHandshake.responseMessage}", 3);
            }

        }

        /**
         * Confirma que o token ainda está valido
         */
        private void ObtemOuRevalidaToken()
        {
            if (!TokenEstaValido()) ObterToken();
        }

        /**
         * Verifica a validadedo token
         */
        private bool TokenEstaValido()
        {
            return (!string.IsNullOrWhiteSpace(this.token) && this.tokenExpiration > DateTime.Now);
        }

        /**
         * Executa a requisição ao WebService
         */
        private string Request(string uri, string body = null)
        {
            string response = "";

            string url = ObterUrl(uri);

            // Requisição
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            // Headers das requisição
            request.Headers["token"] = this.token;
            request.Headers["Cache-Control"] = "no-cache";
            request.Accept = "application/json";
            request.ContentType = "application/json";

            // Execução da reequisição
            DateTime inicio = DateTime.UtcNow;

            // Escrita da requisição
            if (!string.IsNullOrWhiteSpace(body))
            {
                byte[] sendData = Encoding.UTF8.GetBytes(body);
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(sendData, 0, sendData.Length);
                requestStream.Flush();
                requestStream.Dispose();
            }

            // Leitura da resposta
            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                using (var responseStream = resp.GetResponseStream())
                using (var responseStreamReader = new StreamReader(responseStream))
                {
                    response = responseStreamReader.ReadToEnd();
                }
                Log($"Requisicao {uri}", inicio, 3);

                // Verificação do StatusCode
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

        /**
         * Cria a URL para a requisição
         */
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

        /**
         * Parâmetros da requisição
         */
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

        private int ObterAppId()
        {
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_APPID, this.conta.ListaParametrosAdicionais);
            int appid = 0;
            try
            {
                appid = int.Parse(value);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao converter appid GVSAT para inteiro: {ex.ToString()}", "CatchNoAction");
            }
            return appid;
        }

        private int ExtrairComponenteInt(List<Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT.Componente> componentes, string nomeComponente)
        {
            string valorString = ExtrairComponente(componentes, nomeComponente);
            int valor = 0;
            try
            {
                valor = int.Parse(valorString);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao converter componente GVSAT para inteiro: {ex.ToString()}", "CatchNoAction");
            }
            return valor;
        }

        private string ExtrairComponente(List<Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT.Componente> componentes, string nomeComponente)
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

