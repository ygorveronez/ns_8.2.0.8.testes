using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{

    public class IntegracaoSmartTracking : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoSmartTracking Instance;
        private static readonly string nameConfigSection = "SmartTracking";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a Ravex

        private DateTime ultimaDataConsultada;
        private DateTime dataAtual;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_ULTIMA_DATA = "UltimaData";
        private const string KEY_TOKEN = "Token";
        private const string KEY_MAXIMO_MINUTOS_INTERVALO = "MaximoMinutosIntervalo";

        #endregion

        #region Construtor privado

        private IntegracaoSmartTracking(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SmartTracking, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoSmartTracking GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoSmartTracking(cliente);
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
            this.dataAtual = DateTime.Now;
            ObterUltimaDataDoArquivo();
            IntegrarPosicao();
            SalvarUltimoTimestampNoArquivo();
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
            try
            {

                // Intervalo máximo de consulta permitido
                int maximoMinutosIntervalo = int.Parse(ObterValorOpcao(KEY_MAXIMO_MINUTOS_INTERVALO));

                // Período de consulta
                DateTime dataBase = this.ultimaDataConsultada.ToUniversalTime();
                DateTime dataInicial = dataBase.AddMilliseconds(1);
                DateTime dataFinal = dataBase.AddMinutes(maximoMinutosIntervalo);

                // Busca os eventos normais, que contém as posições dos veículos
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.SmartTracking.TrackerData> trackerData = ObterTrackerData(dataInicial, dataFinal);
                int total = trackerData.Count;
                Log($"Recebidas {total} posicoes", 3);

                if (total > 0)
                {
                    char separator = ' ';
                    for (int i = 0; i < total; i++)
                    {

                        // Data da posição
                        DateTime data = Util.ObterDataPelosMilisegundos(trackerData[i].timestamp);

                        // Placa e equipamento. Ex "EWS8364 (T0123)"
                        string placa = "";
                        string idEquipamento = "";

                        string[] partes = trackerData[i].trackerId.Split(separator);
                        if (partes.Length > 0)
                        {
                            placa = partes[0].Trim().Replace("-", "").ToUpper();
                        }
                        if (partes.Length > 1)
                        {
                            idEquipamento = partes[1].Trim().Replace("(", "").Replace(")", "").Replace("ID:", "");
                        }
                        posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                        {
                            Data = data,
                            DataVeiculo = data,
                            IDEquipamento = idEquipamento,
                            Placa = placa,
                            Latitude = trackerData[i].lat,
                            Longitude = trackerData[i].lon,
                            Velocidade = (int)trackerData[i].gpsSpeed,
                            Ignicao = 0,
                            SensorTemperatura = false,
                            Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.SmartTracking
                        });
                    }
                    Log($"{posicoes.Count} posicoes", 3);

                    // Extrai a maior data para iniciar por ela na próxima requisição
                    AtualizarUltimaDataConsultada(posicoes);

                }
                else
                {

                    // Avança a data para não ficar parado no tempo
                    if (dataFinal < this.dataAtual)
                    {
                        this.ultimaDataConsultada = dataFinal;
                    }
                }

            }
            catch (Exception ex)
            {
                Log("Erro ObterTrackerData " + ex.Message, 3);
            }
            return posicoes;
        }

        /**
         * Requisição ao servico "ObterPacotePosicoes"
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.SmartTracking.TrackerData> ObterTrackerData(DateTime dataInicial, DateTime dataFinal)
        {

            // Período de consulta via timestamp no formato milissegundos desde o epoch
            long from = Util.ObterMiliseconds(dataInicial);
            long to = Util.ObterMiliseconds(dataFinal);

            // Parâmetros query string
            NameValueCollection queryParams = new NameValueCollection();
            queryParams.Add("from", from.ToString());
            queryParams.Add("to", to.ToString());

            string responseJson = Request(queryParams);
            var response = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.SmartTracking.TrackerData>>(responseJson);
            return response;
        }

        /**
         * Executa a requisição ao WebService
         */
        private string Request(NameValueCollection requestParams)
        {
            string response = "";
            string url = ObterUrl(requestParams);

            // Requisição
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            // Headers das requisição
            request.Headers["Cache-Control"] = "no-cache";
            request.Headers["Authorization"] = $"Bearer {ObterTokenConta()}";
            request.Accept = "application/json";

            // Execução da reequisição
            DateTime inicio = DateTime.UtcNow;

            // Leitura da resposta
            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                using (var responseStream = resp.GetResponseStream())
                using (var responseStreamReader = new StreamReader(responseStream))
                {
                    response = responseStreamReader.ReadToEnd();
                }
                Log("Requisicao", inicio, 3);

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

        private string ObterUrl(NameValueCollection requestParams)
        {
            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);
            string requestParamsEncoded = EncodeRequestParams(requestParams);
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

        /**
         * Extrai a maior data entre os dados recebidos
         */
        private void AtualizarUltimaDataConsultada(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes)
        {
            int cont = posicoes.Count;
            if (cont > 0)
            {
                this.ultimaDataConsultada = posicoes[0].DataVeiculo;
                for (int i = 1; i < cont; i++)
                {
                    if (posicoes[i].DataVeiculo > this.ultimaDataConsultada)
                    {
                        this.ultimaDataConsultada = posicoes[i].DataVeiculo;
                    }
                }
            }
        }

        /**
         * Busca a última data consultada
         */
        private void ObterUltimaDataDoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = base.CarregarDadosDeControleDoArquivo(this.conta);
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_ULTIMA_DATA, dadosControle);
            this.ultimaDataConsultada = (String.IsNullOrWhiteSpace(value)) ? DateTime.Now : DateTime.Parse(value);
            Log($"Ultimo Timestamp {this.ultimaDataConsultada}", 2);
        }

        /**
         * Registra a última data consultada
         */
        private void SalvarUltimoTimestampNoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = new List<KeyValuePair<string, string>>();
            dadosControle.Add(new KeyValuePair<string, string>(KEY_ULTIMA_DATA, this.ultimaDataConsultada.ToString()));
            base.SalvarDadosDeControleNoArquivo(this.conta, dadosControle);
            Log($"Atualizando ultimo Timestamp {this.ultimaDataConsultada}", 2);
        }

        private string ObterTokenConta()
        {
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_TOKEN, this.conta.ListaParametrosAdicionais);
            return value;
        }

        #endregion

    }

}
