using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoABFSat : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoABFSat Instance;
        private static readonly string nameConfigSection = "ABFSat";
        private DateTime lastTokenExpiration;
        private string lastToken;
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Construtor privado

        private IntegracaoABFSat(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ABFSat, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoABFSat GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoABFSat(cliente);
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

        protected override void Executar(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao)
        {
            this.conta = contaIntegracao;
            IntegrarPosicao();
        }

        #endregion

        #region Métodos privados 
        private void IntegrarPosicao()
        {
            Log($"Consultando posicoes", 2);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes().GetAwaiter().GetResult();

            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);
        }

        private string Login()
        {
            if (!string.IsNullOrWhiteSpace(lastToken) && lastTokenExpiration > DateTime.UtcNow.AddMinutes(1))
                return lastToken;

            var parametros = conta.ListaParametrosAdicionais;

            Dominio.ObjetosDeValor.Embarcador.Integracao.ABFSat.RequisicaoLogin payload = new()
            {
                Username = conta.Usuario,
                Password = conta.Senha,
                HashAuth = ExtrairParametroAdicional("HashAuth")
            };

            var result = PostFormData<Dominio.ObjetosDeValor.Embarcador.Integracao.ABFSat.RequisicaoLogin, Dominio.ObjetosDeValor.Embarcador.Integracao.ABFSat.RespostaLogin>("Login", payload, 5);

            lastTokenExpiration = new DateTime(result.ExpiresIn);
            lastToken = result.AccessToken;

            return result.AccessToken;
        }

        private string MontarUrl(string metodo)
        {
            return MontarUrl(metodo, null);
        }

        private string MontarUrl(string metodo, string versao)
        {
            string url = $"{Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProtocoloHelper.ObterValor(conta.Protocolo)}://{conta.Servidor}";


            if (!string.IsNullOrWhiteSpace(versao))
                url += $"/{versao}";

            url += $"/{metodo}";

            return url;
        }

        private string ExtrairParametroAdicional(string chave)
        {
            KeyValuePair<string, string> param = conta.ListaParametrosAdicionais.Find(p => p.Key == chave);

            return param.Key != null ? param.Value : string.Empty;
        }


        private async Task<TResponse> PostJson<TRequest, TResponse>(string metodo, TRequest payload, string token, int timeoutEmMinutos)
        {
            var cliente = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoABFSat));
            
            if (timeoutEmMinutos > 0)
                cliente.Timeout = TimeSpan.FromMinutes(timeoutEmMinutos);

            var url = MontarUrl(metodo, ExtrairParametroAdicional("Versao"));

            if (token != null)
            {
                cliente.DefaultRequestHeaders.Add("Authorization", token);
            }

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            HttpResponseMessage result = await cliente.PostAsync(url, content);

            if (!result.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.ABFSat.RespostaErroABFSat errorResponse = null;
                string error = await result.Content.ReadAsStringAsync();

                try
                {
                    errorResponse = JsonSerializer.Deserialize<Dominio.ObjetosDeValor.Embarcador.Integracao.ABFSat.RespostaErroABFSat>(
                        error, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                catch (Exception)
                {
                    throw new ServicoException($"Erro ao chamar o método {metodo}: {result.StatusCode}");
                }

                throw new ServicoException($"Erro ao chamar o método {metodo}: {result.StatusCode}: {errorResponse.Message}");
            }

            string responseContent = await result.Content.ReadAsStringAsync();

            if (typeof(TResponse) == typeof(string))
                return (TResponse)(object)responseContent;

            return JsonSerializer.Deserialize<TResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private TResponse PostFormData<TRequest, TResponse>(string metodo, TRequest payload, int timeoutEmMinutos)
        {
            using var cliente = new HttpClient();

            if (timeoutEmMinutos > 0)
                cliente.Timeout = TimeSpan.FromMinutes(timeoutEmMinutos);

            var url = MontarUrl(metodo);

            var content = new MultipartFormDataContent();
            foreach (var prop in typeof(TRequest).GetProperties())
            {
                var value = prop.GetValue(payload);
                content.Add(new StringContent(value?.ToString() ?? ""), prop.Name);
            }

            var response = cliente.PostAsync(url, content).Result;

            if (!response.IsSuccessStatusCode)
                throw new ServicoException($"Erro ao chamar o método {metodo}: {response.StatusCode}");

            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (typeof(TResponse) == typeof(string))
                return (TResponse)(object)responseContent;

            return JsonSerializer.Deserialize<TResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private async Task<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>> ObterPosicoes()
        {
            var posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            try
            {
                string token = Login();

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.ABFSat.RequisicaoHistoricoPosicoes> payload = new()
                {
                    new Dominio.ObjetosDeValor.Embarcador.Integracao.ABFSat.RequisicaoHistoricoPosicoes().CarregarRequisicaoAPartirDasOpcoesDaConfiguracaoDaTecnologia(
                        ObterOpcoesConfiguracaoTecnologiaMonitoramento()
                    )
                };

                var placasResult = await PostJson<List<Dominio.ObjetosDeValor.Embarcador.Integracao.ABFSat.RequisicaoHistoricoPosicoes>, List<Dominio.ObjetosDeValor.Embarcador.Integracao.ABFSat.RespostaPosicaoHistorico>>(
                    conta.URI,
                    payload,
                    token,
                    12
                );

                foreach (var resultado in placasResult)
                {
                    DateTime dataPosicao = DateTime.MinValue;

                    if (!DateTime.TryParse(resultado.EventDate, out dataPosicao))
                    {
                        Log($"Data de posição inválida para a placa {resultado.Plate}: {resultado.EventDate}", 3);
                        continue;
                    }

                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                    {
                        Data = dataPosicao,
                        DataCadastro = DateTime.Now,
                        DataVeiculo = dataPosicao,
                        IDEquipamento = resultado.IdTrackedUnit.ToString(),
                        Placa = resultado.Plate?.Replace("-", "").Replace(" ", ""),
                        Latitude = resultado.Latitude,
                        Longitude = resultado.Longitude,
                        Velocidade = 0,
                        Temperatura = 0,
                        SensorTemperatura = false,
                        Descricao = resultado.Address,
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.ABFSat,
                        Ignicao = resultado.Ignition ? 1 : 0
                    });

                }

                Log($"Total de posições integradas: {posicoes.Count}", 2);
            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes: " + ex.Message, 3);
            }

            return posicoes;
        }

        private List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao> ObterOpcoesConfiguracaoTecnologiaMonitoramento()
        {
            Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento repConfiguracaoIntegracaoTecnologiaMonitoramento = new(unitOfWork);
            Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao = new(unitOfWork);

            Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento config = repConfiguracaoIntegracaoTecnologiaMonitoramento.BuscarPorTipo(
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ABFSat
            );

            if (config == null || config.Codigo == 0)
                throw new ServicoException("Configuração de integração com ABFSat não encontrada.");

            return repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao.BuscarPorConfiguracao(config);
        }

        #endregion
    }
}
