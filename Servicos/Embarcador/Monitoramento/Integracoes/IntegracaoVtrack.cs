using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoVtrack : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoVtrack Instance;
        private static readonly string nameConfigSection = "Vtrack";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a Vtrack

        private long ultimoIdPosicao;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_ULTIMO_ID_POSICAO = "UltimoIDPosicao";
        private const string KEY_TOKEN = "token";
        private const string KEY_ACAO = "acao";


        #endregion

        #region Construtor privado

        private IntegracaoVtrack(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Vtrack, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoVtrack GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoVtrack(cliente);
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
            ObterUltimoIdPosicaoDoArquivo();
            IntegrarPosicao();
            SalvarUltimoIdPosicaoNoArquivo();
            ExecutarCheckoutUltimoID();
        }

        private void ExecutarCheckoutUltimoID()
        {
            Log($"Checkout ultimo ID posicoes", 2);
            NameValueCollection queryParams = new NameValueCollection();
            queryParams.Add("token", ObterTokenConta());
            queryParams.Add("acao", "checkout");
            if (ultimoIdPosicao.ToString() != "1")
                queryParams.Add("retorno", ultimoIdPosicao.ToString());

            string responseJson = Request(null, null, queryParams);

            Log($"Fim Checkout ID posicoes", 2);

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

            string stringApenasTemperatura = ObterValorMonitorar("ProcessarApenasComTemperatura");

            // filtrar as posições da Vtrack de 5 em 5 minutos; priorizando posições com temperatura.
            bool processarApenasTemperatura = (!string.IsNullOrWhiteSpace(stringApenasTemperatura)) && bool.Parse(stringApenasTemperatura);
            if (processarApenasTemperatura)
                posicoes = FiltrarPosicoesTempoTemperatura(posicoes);

            // Registra as posições recebidas dos veículos
            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);

        }


        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> FiltrarPosicoesTempoTemperatura(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesRecebidas)
        {
            TimeSpan intervalo = new TimeSpan(0, 0, 50);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesRetorno = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            try
            {
                //List<string> placas = ExtrairPlacasVeiculos(posicoesRecebidas);

                //foreach (var p in placas)
                //{
                var listaOrdenada = posicoesRecebidas.Where(x => x.SensorTemperatura == true).ToList();
                posicoesRetorno.AddRange(listaOrdenada);

                var listaOrdenadaSemTemperatura = posicoesRecebidas.Where(x => x.SensorTemperatura == false).ToList();
                string posicoesSemTempString = JsonConvert.SerializeObject(listaOrdenadaSemTemperatura, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });

                Servicos.Embarcador.Monitoramento.MonitoramentoUtils.GravarLogTracking(posicoesSemTempString, this.GetType().Name + "_SEMTemperatura");

                //Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoAtual = null;

                //foreach (var t in listaOrdenada)
                //{
                //    if (posicaoAtual == null)
                //    {
                //        posicaoAtual = t;
                //        posicoesRetorno.Add(posicaoAtual);
                //        continue;
                //    }

                //    if (posicaoAtual != null && (t.DataVeiculo - posicaoAtual.DataVeiculo) > intervalo)
                //    {
                //        posicaoAtual = t;
                //        posicoesRetorno.Add(t);
                //    }
                //}
                //}

            }
            catch (Exception ex)
            {
                Log("Erro FiltrarPosicoesTempoTemperatura " + ex.Message, 3);
            }

            return posicoesRetorno;
        }

        //private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> FiltrarPosicoesTempoTemperatura(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesRecebidas, int tempoMinutos)
        //{
        //    TimeSpan intervalo = new TimeSpan(0, tempoMinutos, 0);

        //    List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesRetorno = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

        //    try
        //    {
        //        List<string> placas = ExtrairPlacasVeiculos(posicoesRecebidas);

        //        foreach (var p in placas)
        //        {
        //            var listaOrdenada = posicoesRecebidas.Where(x => x.Placa == p).OrderBy(x => x.DataVeiculo).ToList();

        //            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoAtual = null;

        //            foreach (var t in listaOrdenada)
        //            {
        //                if (posicaoAtual == null)
        //                {
        //                    posicaoAtual = t;
        //                    posicoesRetorno.Add(posicaoAtual);
        //                    continue;
        //                }

        //                if (posicaoAtual != null && (t.DataVeiculo - posicaoAtual.DataVeiculo) >= intervalo)
        //                {
        //                    if (t.SensorTemperatura == true)
        //                    {
        //                        posicaoAtual = t;
        //                        posicoesRetorno.Add(t);
        //                    }
        //                    else if ((t.DataVeiculo - posicaoAtual.DataVeiculo) >= intervalo) //se a ultima posicao tem um intervalo maior e nao tem temperatura, vamos salvar pra nao perder
        //                        posicoesRetorno.Add(t);
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Log("Erro FiltrarPosicoesTempoTemperatura " + ex.Message, 3);
        //    }

        //    return posicoesRetorno;
        //}

        private List<string> ExtrairPlacasVeiculos(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes)
        {
            List<string> placasDistintas = new List<string>();

            int total = posicoes.Count;
            for (int i = 0; i < total; i++)
            {
                if (posicoes[i].Placa != "")
                {
                    bool existe = false;
                    int totalPlacasDistintas = placasDistintas.Count;
                    for (int j = 0; j < totalPlacasDistintas; j++)
                    {
                        if (posicoes[i].Placa == placasDistintas[j])
                        {
                            existe = true;
                            break;
                        }
                    }

                    if (!existe)
                    {
                        placasDistintas.Add(posicoes[i].Placa);
                    }
                }
            }

            return placasDistintas;
        }

        /**
         * Busca as posições de todos os veículos
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            try
            {

                // Busca os eventos normais, que contém as posições dos veículos
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Vtrack.Posicao> posicoesVtrack = BuscaUltimaPosicaoVeiculo();
                Log($"Recebidas {posicoesVtrack.Count} posicoes", 3);

                // Converte os tele eventos para posição
                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Vtrack.Posicao posicaovtrack in posicoesVtrack)
                {

                    // ID do equipamento possui dois números separados por um "pipe", Considera apenas a primera parte
                    // string equipamento = posicaovtrack.NumeroTerminal.Trim().Split('|').First();

                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                    {
                        ID = posicaovtrack.id.ToLong(),
                        Data = posicaovtrack.data.ToDateTime("yyyy-MM-dd HH:mm:ss"),
                        DataVeiculo = posicaovtrack.data.ToDateTime("yyyy-MM-dd HH:mm:ss"),
                        IDEquipamento = "",//equipamento,
                        Placa = posicaovtrack.placa.Trim().Replace("-", "").ToUpper(),
                        Latitude = posicaovtrack.latitude.ToDouble(),
                        Longitude = posicaovtrack.longitude.ToDouble(),
                        Velocidade = posicaovtrack.velocidade.ToInt(),
                        Ignicao = posicaovtrack.ligado == "S" ? 1 : 0,
                        Temperatura = retornaTemperatura(posicaovtrack),
                        SensorTemperatura = posicaovtrack.adicionais == "N" ? false : true,
                        Descricao = "",
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Vtrack
                    });
                }
                Log($"{posicoes.Count} posicoes", 3);

                // Extrai o maior ID entre as posições recebidas para iniciar por ela na próxima requisição
                AtualizarUltimoIdPosicao(posicoesVtrack);

            }
            catch (Exception ex)
            {
                Log("Erro BuscaUltimaPosicaoVeiculo " + ex.Message, 3);
            }
            return posicoes;
        }

        private decimal? retornaTemperatura(Dominio.ObjetosDeValor.Embarcador.Integracao.Vtrack.Posicao posicaovtrack)
        {
            if (posicaovtrack.adicionais == "N")
                return null;

            if (!string.IsNullOrEmpty(posicaovtrack.sensor_temp1))
                return posicaovtrack.sensor_temp1.ToDecimal();
            else if (!string.IsNullOrEmpty(posicaovtrack.sensor_temp2))
                return posicaovtrack.sensor_temp2.ToDecimal();
            else if (!string.IsNullOrEmpty(posicaovtrack.sensor_temp3))
                return posicaovtrack.sensor_temp3.ToDecimal();
            else
                return null;
        }

        /**
         * Requisição ao servico "BuscaUltimaPosicaoVeiculo"
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Vtrack.Posicao> BuscaUltimaPosicaoVeiculo()
        {
            // Parâmetros query string 
            //token=jdSqJsv4Tm5f
            //acao=gprs

            NameValueCollection queryParams = new NameValueCollection();
            queryParams.Add("token", ObterTokenConta());
            queryParams.Add("acao", ObterAcaoConta());
            if (ultimoIdPosicao.ToString() != "1")
                queryParams.Add("retorno", ultimoIdPosicao.ToString());

            string responseJson = Request(null, null, queryParams);
            var response = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Vtrack.PosicaoVtrack>(responseJson);

            string stringsalvarLog = ObterValorMonitorar("SalvarLogResponse");
            bool salvarLog = (!string.IsNullOrWhiteSpace(stringsalvarLog)) && bool.Parse(stringsalvarLog);
            if (salvarLog)
                LogNomeArquivo(responseJson, DateTime.Now, "ResponsePosicoes", 0, true);


            return response.Posicoes;
        }

        /**
         * Executa a requisição ao WebService
         */
        private string Request(string operacao = null, NameValueCollection pathParams = null, NameValueCollection queryParams = null)
        {
            string response = "";
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Monta a URL para a requisição
            string url = BuildURL(operacao, pathParams, queryParams);

            // Requisição
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 600 * 1000;

            // Execução da reequisição
            DateTime inicio = DateTime.UtcNow;
            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {

                // Laitura da resposta
                using (var responseStream = resp.GetResponseStream())
                using (var responseStreamReader = new StreamReader(responseStream))
                {
                    response = responseStreamReader.ReadToEnd();
                }
                Log($"Requisicao {operacao}", inicio, 3);

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
        private string BuildURL(string operacao = null, NameValueCollection pathParams = null, NameValueCollection queryParams = null)
        {
            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);
            if (operacao != null)
                url += $"/{operacao}";

            // Concatena os parâmetros do path da URL
            if (pathParams != null && pathParams.Count > 0)
            {
                foreach (string key in pathParams)
                {
                    url += $"/{key}/{pathParams[key]}";
                }
            }

            // Concatena os parâmetros da queryString
            if (queryParams != null && queryParams.Count > 0)
            {
                url += "?";
                foreach (string key in queryParams)
                {
                    url += $"{key}={Uri.EscapeUriString(queryParams[key])}&";
                }
                url = url.Remove(url.Length - 1).ToString();
            }
            return url;
        }

        /**
         * Extrai o maior ID entre as posições rescebidas
         */
        private void AtualizarUltimoIdPosicao(List<Dominio.ObjetosDeValor.Embarcador.Integracao.Vtrack.Posicao> posicoesVtrack)
        {
            int cont = posicoesVtrack.Count;
            for (int i = 0; i < cont; i++)
            {
                if (posicoesVtrack[i].id.ToLong() > this.ultimoIdPosicao)
                {
                    this.ultimoIdPosicao = posicoesVtrack[i].id.ToLong();
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

        private string ObterTokenConta()
        {
            string Token = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_TOKEN, this.conta.ListaParametrosAdicionais);
            return Token;
        }

        private string ObterAcaoConta()
        {
            string acao = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_ACAO, this.conta.ListaParametrosAdicionais);
            return acao;
        }





        #endregion

    }

}