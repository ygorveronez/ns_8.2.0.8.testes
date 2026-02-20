using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{

    public class IntegracaoRavex : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoRavex Instance;
        private static readonly string nameConfigSection = "Ravex";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a Ravex

        private long ultimoIdPosicao;
        private string token;
        private DateTime validadeToken;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string URI_TOKEN = "/Token";
        private const string URI_OBTER_POSICOES = "ObterPacotePosicoesV2";
        private const string KEY_ULTIMO_ID_POSICAO = "UltimoIDPosicao";

        #endregion

        #region Construtor privado

        private IntegracaoRavex(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ravex, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoRavex GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoRavex(cliente);
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
            base.ValidarConfiguracaoDiretorioEArquivoControle(base.contasIntegracao);
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
            ObterUltimoIdPosicaoDoArquivo();
            IntegrarPosicao();
            SalvarUltimoIdPosicaoNoArquivo();
        }

        #endregion

        #region Métodos privados

        /**
         * Autentica e obtém o token para as demais requisições
         */
        private void ObterToken()
        {
            Log("Obtendo Token", 2);

            NameValueCollection requestParams = new NameValueCollection();
            requestParams.Add("username", this.conta.Usuario);
            requestParams.Add("password", this.conta.Senha);
            requestParams.Add("grant_type", "password");

            string responseJson = Request("POST", URI_TOKEN, requestParams);
            var response = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Ravex.Token>(responseJson);
            if (!string.IsNullOrWhiteSpace(response.access_token))
            {
                this.token = response.access_token;
                this.validadeToken = DateTime.Now.AddSeconds(response.expires_in);
            }
            else
            {
                this.token = null;
                this.validadeToken = DateTime.MinValue;
                Log($"Falha na autenticação {response.error_description}", 3);
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
            return (!string.IsNullOrWhiteSpace(this.token) && this.validadeToken > DateTime.Now);
        }

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

                    // Busca os eventos normais, que contém as posições dos veículos
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.Ravex.Posicao> posicoesRavex = ObterPacotePosicoes();
                    int total = posicoesRavex.Count;
                    Log($"Recebidas {total} posicoes", 3);

                    for (int i = 0; i < total; i++)
                    {
                        posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                        {
                            ID = posicoesRavex[i].IdPosicao,
                            Data = posicoesRavex[i].Evento_Datahora.ToLocalTime(),
                            DataVeiculo = posicoesRavex[i].Evento_Datahora.ToLocalTime(),
                            IDEquipamento = posicoesRavex[i].IdRastreador.ToString(),
                            Placa = posicoesRavex[i].Placa,
                            Latitude = posicoesRavex[i].GPS_Latitude,
                            Longitude = posicoesRavex[i].GPS_Longitude,
                            Velocidade = posicoesRavex[i].GPS_Velocidade,
                            Ignicao = (posicoesRavex[i].Ignicao) ? 1 : 0,
                            Temperatura = posicoesRavex[i].Temperatura1,
                            SensorTemperatura = false,
                            Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Ravex
                        });
                    }
                    Log($"{posicoes.Count} posicoes", 3);

                    // Extrai o maior ID entre as posições recebidas para iniciar por ela na próxima requisição
                    AtualizarUltimoIdPosicao(posicoesRavex);

                }
                catch (Exception ex)
                {
                    Log("Erro ObterPacotePosicoes " + ex.Message, 3);
                }
            }
            else
            {
                Log("Token invalido", 3);
            }
            return posicoes;
        }

        /**
         * Requisição ao servico "ObterPacotePosicoes"
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Ravex.Posicao> ObterPacotePosicoes()
        {
            // Parâmetros query string
            NameValueCollection queryParams = new NameValueCollection();
            queryParams.Add("pIdInicial", ultimoIdPosicao.ToString());

            string responseJson = Request("GET", this.conta.URI + URI_OBTER_POSICOES, queryParams);
            var response = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Ravex.PosicaoResponse>(responseJson);
            return response.ListaPosicoes;
        }

        /**
         * Executa a requisição ao WebService
         */
        private string Request(string method, string uri, NameValueCollection requestParams = null)
        {
            string response = "";

            string url = BuildURL(uri);
            string requestParamsEncoded = EncodeRequestParams(requestParams);
            if (method.ToUpper().Equals("GET"))
            {
                url += "?" + requestParamsEncoded;
            }

            // Requisição
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;

            // Headers das requisição
            request.Headers["Cache-Control"] = "no-cache";
            request.Headers["Authorization"] = $"Bearer {this.token}";
            request.Accept = "application/json";

            // Execução da reequisição
            DateTime inicio = DateTime.UtcNow;

            // Escrita da requisição
            if (method.ToUpper().Equals("POST"))
            {
                byte[] sendData = Encoding.UTF8.GetBytes(requestParamsEncoded);
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
        private string BuildURL(string uri)
        {
            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURLBase(this.conta);
            url += uri;
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

        /**
         * Extrai o maior ID entre as posições rescebidas
         */
        private void AtualizarUltimoIdPosicao(List<Dominio.ObjetosDeValor.Embarcador.Integracao.Ravex.Posicao> posicoes)
        {
            int cont = posicoes.Count;
            for (int i = 0; i < cont; i++)
            {
                if (posicoes[i].IdPosicao > this.ultimoIdPosicao)
                {
                    this.ultimoIdPosicao = posicoes[i].IdPosicao;
                }
            }
        }

        /**
         * Busca o último ID de posições já recebido
         */
        private void ObterUltimoIdPosicaoDoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = base.CarregarDadosDeControleDoArquivo(this.conta);
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_ULTIMO_ID_POSICAO, dadosControle);
            this.ultimoIdPosicao = (String.IsNullOrWhiteSpace(value)) ? 1 : Int64.Parse(value);
            Log($"Ultimo ID Posicao {this.ultimoIdPosicao}", 2);
        }

        /**
         * Registra no arquivo o último ID das posições
         */
        private void SalvarUltimoIdPosicaoNoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = new List<KeyValuePair<string, string>>();
            dadosControle.Add(new KeyValuePair<string, string>(KEY_ULTIMO_ID_POSICAO, this.ultimoIdPosicao.ToString()));
            base.SalvarDadosDeControleNoArquivo(this.conta, dadosControle);
            Log($"Atualizando ultimo ID de posicao {this.ultimoIdPosicao}", 2);
        }

        #endregion

    }

}
