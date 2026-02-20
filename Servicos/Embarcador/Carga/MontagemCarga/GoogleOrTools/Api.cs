using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
//using System.Net.Http;
//using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools
{
    public class Api
    {

        #region Propriedades privadas

        private string ApiUrl = "https://ppc.multihomo.com.br/roteirizacao";
        private string ApiKey = "348969859b3838c3aef99b607688a849";
        private string ServerOsrm = "http://191.232.193.82/";

        #endregion

        #region Métodos privados

        private string GetUrlServiceApi(string action)
        {
            return string.Format("{0}/route/{1}/{2}", ApiUrl, action, ApiKey);
        }

        private string PostRequest(string action, string json)
        {
            string url = this.GetUrlServiceApi(action);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Api));
            client.Timeout = TimeSpan.FromSeconds(300);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = client.PostAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                    return response.Content.ReadAsStringAsync().Result;
                else
                    throw new HttpRequestException("Erro: " + response.StatusCode);
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                Servicos.Log.TratarErro(ex);
                Servicos.Log.TratarErro($"POST {url} {Environment.NewLine}{json}");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                Servicos.Log.TratarErro($"POST {url} {Environment.NewLine}{json}");
            }
            return string.Empty;
        }

        #endregion

        #region Propriedades publicas

        public bool DesconsiderarTempoDeslocamentoDeposito { get; set; }

        public List<Veiculo> Veiculos { get; set; }

        public EnumCargaTpRota TipoRota { get; set; }

        public EnumFirstSolutionStrategy Strategy { get; set; }

        public List<Local> Locais { get; set; }

        public int QtdeMaximaEntregas { get; set; }

        public int QtdeMaximaPedidosCanalEntrega { get; set; }

        #endregion

        public Api(string apiURL = null)
        {
            if (!string.IsNullOrWhiteSpace(apiURL))
                ApiUrl = apiURL;
        }

        public Api(string apiURL, string serverOsrm)
        {
            if (!string.IsNullOrWhiteSpace(apiURL))
                ApiUrl = apiURL;
            if (!string.IsNullOrWhiteSpace(serverOsrm))
                ServerOsrm = serverOsrm;
        }

        #region Métodos publicos

        public ApiResultadoTsp TSP()
        {
            if (this.Locais?.Count < 2)
                throw new Exception("1 - TSP - são necessários pelo menos 2 locais para otimizar o percurso.");

            if (!this.ServerOsrm.ToLower().Contains("http://"))
                this.ServerOsrm = "http://" + this.ServerOsrm;

            var problema = new
            {
                this.ServerOsrm,
                this.TipoRota,
                this.Strategy,
                this.Locais
            };

            string json = JsonConvert.SerializeObject(problema);

            string strget = PostRequest("tsp", json);
            ApiResultadoTsp result = JsonConvert.DeserializeObject<ApiResultadoTsp>(strget);
            return result;
        }

        public ApiResultado CVRP(bool gerarCarregamentosAlemDispVeiculos, int timeLimitMs = 10000)
        {
            if (this.Locais?.Count < 2)
                throw new Exception("1 - CVRP - São necessários pelo menos 2 locais para otimizar o percurso.");

            if (this.Veiculos?.Count < 1)
                throw new Exception("2 - CVRP - Informe os veículos disponíveis para gerar os carregamentos.");

            if (!this.ServerOsrm.ToLower().Contains("http://"))
                this.ServerOsrm = "http://" + this.ServerOsrm;

            var problema = new
            {
                this.ServerOsrm,
                GerarCarregamentosExtras = gerarCarregamentosAlemDispVeiculos,
                this.TipoRota,
                this.Strategy,
                TimeLimitMs = timeLimitMs,
                this.Locais,
                this.Veiculos
            };

            string json = JsonConvert.SerializeObject(problema);

            string strget = PostRequest("cvrp", json);
            ApiResultado result = JsonConvert.DeserializeObject<ApiResultado>(strget);
            return result;
        }

        public ApiResultado CVRPTW(bool gerarCarregamentosAlemDispVeiculos, int tempoMaxRota, int timeLimitMs = 60000)
        {
            if (this.Locais?.Count < 2)
                throw new Exception("1 - CVRPTW - São necessários pelo menos 2 locais para otimizar o percurso.");

            if (this.Veiculos?.Count < 1)
                throw new Exception("2 - CVRPTW - Informe os veículos disponíveis para gerar os carregamentos.");

            if (!this.ServerOsrm.ToLower().Contains("http://"))
                this.ServerOsrm = "http://" + this.ServerOsrm;

            var problema = new
            {
                this.ServerOsrm,
                GerarCarregamentosExtras = gerarCarregamentosAlemDispVeiculos,
                this.TipoRota,
                this.Strategy,
                TempoMaxRota = tempoMaxRota,
                TimeLimitMs = timeLimitMs,
                this.Locais,
                this.Veiculos,
                this.QtdeMaximaEntregas,
                this.DesconsiderarTempoDeslocamentoDeposito
            };

            string json = JsonConvert.SerializeObject(problema);

            string strget = PostRequest("cvrptw", json);
            ApiResultado result = JsonConvert.DeserializeObject<ApiResultado>(strget);
            return result;
        }

        #endregion
    }
}
