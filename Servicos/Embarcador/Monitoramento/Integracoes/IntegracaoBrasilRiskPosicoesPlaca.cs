using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoBrasilRiskPosicoesPlaca : Abstract.AbstractIntegracaoSOAP
    {
        #region Atributos privados

        private static IntegracaoBrasilRiskPosicoesPlaca Instance;
        private static readonly string nameConfigSection = "BrasilRiskPosicoesPlaca";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;
        private string token;
        private DateTime validadeToken;

        #endregion

        #region Construtor privado

        private IntegracaoBrasilRiskPosicoesPlaca(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskPosicoesPlaca, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoBrasilRiskPosicoesPlaca GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoBrasilRiskPosicoesPlaca(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        protected override void ComplementarConfiguracoes()
        {

        }
        protected override void Validar()
        {
            base.ValidarConfiguracaoDiretorioEArquivoControle(base.contasIntegracao);
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
            Log($"Consultando posicoes", 2);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes;

            posicoes = ObterPosicoesPorPlaca();

            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoesPorPlaca()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            try
            {
                if (ListaVeiculosMonitorados == null || ListaVeiculosMonitorados.Count == 0)
                {
                    Log("Nenhum veículo para consultar", 4);
                    return posicoes;
                }

                Log($"Consultando {ListaVeiculosMonitorados.Count} posicoes", 3);
                string salvarLogResponse = ObterValorMonitorar("SalvarLogResponse");

                Parallel.ForEach(ListaVeiculosMonitorados, new ParallelOptions() { MaxDegreeOfParallelism = 15 }, veiculoConsultar =>
                {
                    if (veiculoConsultar == null || string.IsNullOrWhiteSpace(veiculoConsultar.Placa))
                    {
                        Log("Veículo nulo ou placa inválida", 4);
                        return;
                    }

                    Log($"Consultando placa '{veiculoConsultar.Placa}'", 5);
                    List<PosicaoBRK> posicoesBRK = ObterPosicoesVeiculosPorPlaca(veiculoConsultar.Placa);

                    if (posicoesBRK == null || posicoesBRK.Count == 0)
                        return;

                    var jsonResponse = JsonConvert.SerializeObject(posicoesBRK, Formatting.None);

                    bool salvarLog = (!string.IsNullOrWhiteSpace(salvarLogResponse)) && bool.Parse(salvarLogResponse);
                    if (salvarLog)
                        LogNomeArquivo("Placa: " + veiculoConsultar.Placa + " - " + jsonResponse, DateTime.Now, "ResponsePosicoesBRKPlaca", 0, true);

                    Log($"Tratando placa '{veiculoConsultar.Placa}'", 5);

                    foreach (PosicaoBRK posicaoBRK in posicoesBRK)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao = new();
                        try
                        {
                            posicao.DataCadastro = DateTime.Now;
                            posicao.Data = posicaoBRK.dataCriacaoRegistro;
                            posicao.DataVeiculo = posicaoBRK.dataPosicao;
                            posicao.Placa = posicaoBRK.placa;
                            posicao.Descricao = posicaoBRK.localizacao;
                            posicao.Rastreador = this.ObterEnumRastreadorDescricao(posicaoBRK.tecnologia);
                            posicao.Ignicao = posicaoBRK.ignicao.ToLower() == "ligada" ? 1 : 0;
                            posicao.Gerenciadora = EnumTecnologiaGerenciadora.BRK;
                            posicao.Temperatura = posicaoBRK.temperatura;
                            posicao.SensorTemperatura = posicaoBRK.temperatura.HasValue;
                            posicao.Latitude = posicaoBRK.latitude;
                            posicao.Longitude = posicaoBRK.longitude;
                            posicao.IDEquipamento = "99";
                        }
                        catch (Exception ex)
                        {
                            Log($"Erro no retorno dos dados '{ex.ToString()}'", 4);
                            Log($"Erro no retorno dos dados '{posicaoBRK}'", 4);
                            continue;
                        }

                        lock (posicoes) posicoes.Add(posicao);
                    }
                });

                Log($"Retornando {posicoes.Count} posicoes", 3);
            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes " + ex.Message, 2);
            }

            return posicoes;
        }

        private HttpClient CriarRequisicao(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBrasilRiskPosicoesPlaca));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("Authorization", "Bearer " + ObterToken());

            return requisicao;
        }

        private string ObterToken()
        {
            string format = "yyyy-MM-dd HH:mm:ss";

            if (TokenEstaValido())
                return this.token;

            //string url = $"{conta.Protocolo}://{conta.Servidor}/{conta.URI}";
            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBrasilRiskPosicoesPlaca));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(conta.Usuario, conta.Senha);

            HttpResponseMessage retornoRequisicao = requisicao.GetAsync(url).Result;
            string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

            dynamic retorno = JsonConvert.DeserializeObject(jsonRetorno);

            if (retornoRequisicao.IsSuccessStatusCode && !string.IsNullOrWhiteSpace((string)retorno?.accessToken))
            {
                DateTime.TryParseExact((string)retorno.expiration, format, null, System.Globalization.DateTimeStyles.None, out DateTime dataExpiracao);
                validadeToken = dataExpiracao;
                token = (string)retorno.accessToken;

                return token;
            }

            Log($"Problemas ao efetuar o login (BRK Posicoes) -> Retorno: '{retorno?.message}'", 4);
            throw new ServicoException((string)retorno?.accessToken ?? "Não foi possível autenticar");
        }

        private bool TokenEstaValido()
        {
            return (!string.IsNullOrWhiteSpace(this.token) && this.validadeToken > DateTime.Now);
        }

        private List<PosicaoBRK> ObterPosicoesVeiculosPorPlaca(string placa)
        {
            try
            {
                string url = $"{conta.Protocolo}://{conta.Servidor}/api/Veiculo/ListarPosicoesTecnologiaPorPlaca?Placa={placa}";

                HttpClient requisicao = CriarRequisicao(url);

                HttpResponseMessage retornoRequisicao = requisicao.GetAsync(url).Result;
                string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;


                if (!string.IsNullOrWhiteSpace(jsonRetorno) && jsonRetorno.ToLower().Contains("não cadastrado na brk"))
                {
                    if (!ListaPlacaVeiculosRemovidosMonitorados.Exists(x => x == placa))
                        ListaPlacaVeiculosRemovidosMonitorados.Add(placa);

                    throw new ServicoException($"{jsonRetorno}");
                }
                else if (!string.IsNullOrWhiteSpace(jsonRetorno) && jsonRetorno.ToLower().Contains("tecnologia embarcada"))
                {
                    if (!ListaPlacaVeiculosRemovidosMonitorados.Exists(x => x == placa))
                        ListaPlacaVeiculosRemovidosMonitorados.Add(placa);

                    throw new ServicoException($"{jsonRetorno}");
                }
                else if (retornoRequisicao.IsSuccessStatusCode)
                {
                    List<PosicaoBRK> retorno = JsonConvert.DeserializeObject<List<PosicaoBRK>>(jsonRetorno);
                    return retorno;
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.Unauthorized)
                    throw new ServicoException("Integração não autorizada, verifique o usuário e senha!");
                else if (!string.IsNullOrWhiteSpace(jsonRetorno))
                {
                    throw new ServicoException($"Não retornou sucesso: '{jsonRetorno}'");
                }
                else
                    throw new ServicoException($"Problema ao efetuar a integração");

            }
            catch (ServicoException excecao)
            {
                Log($"BrasilRisk -> ServicoException: '{excecao.Message}'", 4);
            }
            catch (Exception excecao)
            {
                Log($"BrasilRisk -> Ocorreu um erro: '{excecao.Message}'", 4);
            }

            return null;
        }
        #endregion

        #region Classe Retorno Posição BRK

        public class PosicaoBRK
        {
            public string tecnologia { get; set; }
            public string placa { get; set; }
            public DateTime dataPosicao { get; set; }
            public DateTime dataCriacaoRegistro { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public string localizacao { get; set; }
            public string ignicao { get; set; }
            public string cidade { get; set; }
            public string uf { get; set; }
            public int? temperatura { get; set; }
        }

        #endregion
    }
}