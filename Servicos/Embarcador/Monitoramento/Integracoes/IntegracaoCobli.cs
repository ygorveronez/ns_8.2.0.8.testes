using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoCobli : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoCobli Instance;
        private static readonly string nameConfigSection = "Cobli";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_API_KEY = "cobli-api-key";

        #endregion

        #region Construtor privado

        private IntegracaoCobli(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cobli, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        public static IntegracaoCobli GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoCobli(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        protected override void ComplementarConfiguracoes()
        {

        }

        protected override void Validar()
        {

        }

        protected override void Preparar()
        {

        }

        protected override void Executar(ContaIntegracao contaIntegracao)
        {
            this.conta = contaIntegracao;
            IntegrarPosicao();
        }

        #endregion

        #region Métodos privados

        private void IntegrarPosicao()
        {
            DateTime dataInicioProcesso = DateTime.Now;
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes;
            Log($"Consultando posicoes", 2);
            posicoes = ObterPosicoes(dataInicioProcesso);
            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes(DateTime dataInicioProcesso)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new();

            try
            {
                List<VeiculoCobli> veiculosCobli = BuscarVeiculosDaCobli();
                if (veiculosCobli == null || veiculosCobli.Count == 0)
                    return posicoes;

                List<VeiculoCobli> veiculosMonitorados = FiltrarVeiculosMonitorados(veiculosCobli);
                if (veiculosMonitorados == null || veiculosMonitorados.Count == 0)
                    return posicoes;

                string salvarLogResponse = ObterValorMonitorar("SalvarLogResponse");

                Parallel.ForEach(veiculosMonitorados, new ParallelOptions() { MaxDegreeOfParallelism = 5 }, veiculo =>
                {
                    try
                    {
                        TelemetriaCobli telemetria = ObterTelemetriaVeiculo(veiculo.device_id);
                        if (telemetria?.data == null) return;

                        if (!string.IsNullOrWhiteSpace(salvarLogResponse) && bool.Parse(salvarLogResponse))
                        {
                            var jsonResponse = JsonConvert.SerializeObject(telemetria, Formatting.None);
                            LogNomeArquivo($"TELEMETRIA - Placa: {veiculo.license_plate} - Device: {veiculo.device_id} - {jsonResponse}", DateTime.Now, "ResponseTelemetriaCobli", 0, true);
                        }

                            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao = new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                            {
                                DataCadastro = DateTime.Now,
                                Data = dataInicioProcesso,
                                DataVeiculo = telemetria.data.updated_at.ToLocalTime(),
                                IDEquipamento = veiculo.device_id,
                                Placa = veiculo.license_plate,
                                Latitude = telemetria.data.latitude,
                                Longitude = telemetria.data.longitude,
                                Velocidade = (int)telemetria.data.speed,
                                Ignicao = telemetria.data.ignition_on.HasValue && telemetria.data.ignition_on.Value ? 1 : 0,
                                Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Cobli,
                            };

                        lock (posicoes) posicoes.Add(posicao);
                    }
                    catch (Exception ex)
                    {
                        Log($"ERRO ao processar TELEMETRIA do veículo {veiculo.license_plate} (Device: {veiculo.device_id}): {ex.Message}", 2);
                    }
                });

                Log($"Foram processadas {posicoes.Count} posições", 2);
            }
            catch (Exception ex)
            {
                Log($"ERRO GERAL em ObterPosicoes: {ex.Message}", 2);
            }

            return posicoes;
        }

        private List<VeiculoCobli> BuscarVeiculosDaCobli()
        {
            try
            {
                string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);
                url += "/vehicles?limit=2000&page=1";

                using (var client = CriarHttpClient())
                {
                    var response = client.GetAsync(url).Result;
                    var jsonRetorno = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string salvarLogResponse = ObterValorMonitorar("SalvarLogResponse");
                        if (!string.IsNullOrWhiteSpace(salvarLogResponse) && bool.Parse(salvarLogResponse))
                        {
                            LogNomeArquivo($"VEHICLES - {jsonRetorno}", DateTime.Now, "ResponseVehiclesCobli", 0, true);
                        }

                        RespostaVeiculosCobli resposta = JsonConvert.DeserializeObject<RespostaVeiculosCobli>(jsonRetorno);
                        List<VeiculoCobli> veiculos = resposta?.data ?? new List<VeiculoCobli>();
                        Log($"Foram retornados {veiculos.Count} veículos na API Cobli", 2);
                        return veiculos;
                    }
                    else
                    {
                        Log($"ERRO ENDPOINT VEHICLES - Status: {response.StatusCode} - Response: {jsonRetorno}", 2);
                        throw new ServicoException($"Erro na API Cobli /vehicles: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"ERRO ENDPOINT VEHICLES - Exception: {ex.Message}", 2);
                return new List<VeiculoCobli>();
            }
        }

        private List<VeiculoCobli> FiltrarVeiculosMonitorados(List<VeiculoCobli> veiculosCobli)
        {
            List<VeiculoCobli> veiculosMonitorados = new List<VeiculoCobli>();
            HashSet<string> placasLocais = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento veiculo in ListaVeiculos.Where(v => !string.IsNullOrWhiteSpace(v.Placa)))
            {
                string placaNormalizada = veiculo.Placa.ToUpperInvariant().Replace(" ", "").Replace("-", "");
                placasLocais.Add(placaNormalizada);
            }

            Log($"Placas locais para comparação: {placasLocais.Count}", 2);

            foreach (VeiculoCobli veiculoCobli in veiculosCobli.Where(v => !string.IsNullOrWhiteSpace(v.license_plate) && !string.IsNullOrWhiteSpace(v.device_id)))
            {
                string placaCobliNormalizada = veiculoCobli.license_plate.ToUpperInvariant().Replace(" ", "").Replace("-", "");
                if (placasLocais.Contains(placaCobliNormalizada))
                {
                    Log($"MATCH: {veiculoCobli.license_plate} -> {veiculoCobli.device_id}", 2);
                    veiculosMonitorados.Add(veiculoCobli);
                }
            }

            Log($"Foram encontrados {veiculosMonitorados.Count} veículos monitorados na Cobli", 2);
            return veiculosMonitorados;
        }

        private TelemetriaCobli ObterTelemetriaVeiculo(string deviceId)
        {
            try
            {
                string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);
                url += $"/devices/{deviceId}/telemetry";

                using (HttpClient client = CriarHttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    string jsonRetorno = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        Log($"Sucesso ao obter TELEMETRIA do Device {deviceId}", 2);
                        return JsonConvert.DeserializeObject<TelemetriaCobli>(jsonRetorno);
                    }
                    else
                    {
                        Log($"ERRO ENDPOINT TELEMETRY - Device {deviceId} - Status: {response.StatusCode} - Response: {jsonRetorno}", 2);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"ERRO ENDPOINT TELEMETRY - Device {deviceId} - Exception: {ex.Message}", 2);
                return null;
            }
        }

        private HttpClient CriarHttpClient()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string apiKey = ObterApiKey();
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                client.DefaultRequestHeaders.Add("cobli-api-key", apiKey);
            }

            return client;
        }

        private string ObterApiKey()
        {
            return Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_API_KEY, this.conta.ListaParametrosAdicionais);
        }

        #endregion

        #region Classes de Resposta da API Cobli

        public class RespostaVeiculosCobli
        {
            public List<VeiculoCobli> data { get; set; }
        }

        public class VeiculoCobli
        {
            public string id { get; set; }
            public string device_id { get; set; }
            public string license_plate { get; set; }
            public string brand { get; set; }
            public string model { get; set; }
            public int year { get; set; }
        }

        public class TelemetriaCobli
        {
            public DataTelemetria data { get; set; }
        }

        public class DataTelemetria
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
            public double? speed { get; set; }
            public bool? ignition_on { get; set; }
            public DateTime updated_at { get; set; }
            public string gps_status { get; set; }
        }

        #endregion
    }
}