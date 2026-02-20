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

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoBrasilRiskPosicoes : Abstract.AbstractIntegracaoSOAP
    {
        #region Atributos privados

        private static IntegracaoBrasilRiskPosicoes Instance;
        private static readonly string nameConfigSection = "BrasilRiskPosicoes";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;
        private string token;
        private DateTime validadeToken;

        #endregion

        #region Construtor privado

        private IntegracaoBrasilRiskPosicoes(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskPosicoes, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */

        public static IntegracaoBrasilRiskPosicoes GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoBrasilRiskPosicoes(cliente);
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

            posicoes = ObterPosicoesSM();

            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoesSM()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            try
            {
                Log($"Consultando posicoes SM", 3);
                List<PosicaoBRK> listaPosicoesBRK = ObterPosicoesVeiculos();
                if (listaPosicoesBRK?.Count > 0)
                {
                    foreach (PosicaoBRK posicaoBRK in listaPosicoesBRK)
                    {
                        try
                        {
                            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao = new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao()
                            {
                                Data = posicaoBRK.DataEnvio ?? DateTime.Now,
                                DataVeiculo = posicaoBRK.DataLocalizacao ?? DateTime.Now,
                                DataCadastro = DateTime.Now,
                                Placa = posicaoBRK.PlacaCavalo,
                                IDEquipamento = posicaoBRK.PlacaCavalo,
                                Latitude = double.Parse(posicaoBRK.LatitudeLocalizacao, System.Globalization.CultureInfo.InvariantCulture),
                                Longitude = double.Parse(posicaoBRK.LongitudeLocalizacao, System.Globalization.CultureInfo.InvariantCulture),
                                Descricao = $"SM: {posicaoBRK.NumeroSM}",
                                Rastreador = this.ObterEnumRastreadorDescricao(posicaoBRK.NomeTecnologia),
                                Gerenciadora = EnumTecnologiaGerenciadora.BRK,
                                Temperatura = posicaoBRK.Temperatura,
                                Velocidade = posicaoBRK.VelocidadeVeiculo ?? 0,
                                SensorTemperatura = posicaoBRK.Temperatura.HasValue
                            };
                            posicoes.Add(posicao);
                        }
                        catch (Exception ex) // Enterra e segue para próxima posição.
                        {
                            Log("Erro PosiçãoBRK: " + ex.Message, 2);
                        }
                    }
                    Log($"Retornando {posicoes.Count} posicoes", 3);
                }
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
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBrasilRiskPosicoes));

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
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBrasilRiskPosicoes));

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

        private List<PosicaoBRK> ObterPosicoesVeiculos()
        {
            try
            {
                string url = $"{conta.Protocolo}://{conta.Servidor}/api/Veiculo/BuscaPosicoes";

                HttpClient requisicao = CriarRequisicao(url);

                HttpResponseMessage retornoRequisicao = requisicao.GetAsync(url).Result;
                string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                string stringsalvarLog = ObterValorMonitorar("SalvarLogResponse");
                bool salvarLog = (!string.IsNullOrWhiteSpace(stringsalvarLog)) && bool.Parse(stringsalvarLog);
                if (salvarLog)
                    LogNomeArquivo(jsonRetorno, DateTime.Now, "ResponsePosicoesBRKPosicoes", 0, true);

                if (retornoRequisicao.IsSuccessStatusCode)
                {
                    List<PosicaoBRK> retorno = JsonConvert.DeserializeObject<List<PosicaoBRK>>(jsonRetorno);
                    return retorno;
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.Unauthorized)
                    throw new ServicoException("Integração não autorizada, verifique o usuário e senha!");
                else if (!string.IsNullOrWhiteSpace(jsonRetorno))
                    throw new ServicoException($"Não retornou sucesso: '{jsonRetorno}'");
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
            public int? NumeroSM { get; set; }
            public string NumeroTerminalEquipamento { get; set; }
            public string NomeTecnologia { get; set; }
            public string PlacaCavalo { get; set; }
            public DateTime? DataLocalizacao { get; set; }
            public string LatitudeLocalizacao { get; set; }
            public string LongitudeLocalizacao { get; set; }
            public int? VelocidadeVeiculo { get; set; }
            public int? Temperatura { get; set; }
            public DateTime? DataEnvio { get; set; }
        }
        #endregion
    }
}
