using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoTotalTrac : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoTotalTrac Instance;
        private static readonly string nameConfigSection = "TotalTrac";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;
        private DateTime ultimaDataConsultada;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_ULTIMA_DATA = "UltimaData";
        private const string KEY_ID_USUARIO = "IdUsuario";

        #endregion

        #region Construtor privado

        public IntegracaoTotalTrac(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TotalTrac, nameConfigSection, cliente)
        {
        }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         **/

        public static IntegracaoTotalTrac GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoTotalTrac(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        protected override void ComplementarConfiguracoes()
        {

        }

        protected override void Validar()
        {
            base.ValidarConfiguracaoArquivoControle(base.contasIntegracao);
        }

        protected override void Preparar()
        {

        }

        protected override void Executar(ContaIntegracao contaIntegracao)
        {
            this.conta = contaIntegracao;
            ObterUltimaDataDoArquivo();
            IntegrarPosicao();
            SalvarUltimaDataNoArquivo();
        }

        #endregion

        #region Métodos privados

        private void IntegrarPosicao()
        {
            Log($"Consultando posicoes", 2);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes;

            posicoes = ObterPosicoes();

            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            try
            {
                Log($"Consultando posicoes TotalTrac", 3);
                List<PosicaoTotalTrac> listaPosicoesTotalTrac = ObterPosicoesVeiculos();
                if (listaPosicoesTotalTrac?.Count > 0)
                {
                    foreach (PosicaoTotalTrac posicaoTotalTrac in listaPosicoesTotalTrac)
                    {
                        try
                        {
                            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao = new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao()
                            {
                                Data = posicaoTotalTrac.dataServidor ?? DateTime.Now,
                                DataVeiculo = posicaoTotalTrac.dataGPS ?? DateTime.Now,
                                DataCadastro = DateTime.Now,
                                Placa = posicaoTotalTrac.placa,
                                IDEquipamento = posicaoTotalTrac.idVeiculo,
                                Latitude = double.Parse(posicaoTotalTrac.latitude, System.Globalization.CultureInfo.InvariantCulture),
                                Longitude = double.Parse(posicaoTotalTrac.longitude, System.Globalization.CultureInfo.InvariantCulture),
                                Descricao = posicaoTotalTrac.localizacao,
                                Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.TotalTrac,
                                Temperatura = posicaoTotalTrac.temperaturaS1,
                                Velocidade = posicaoTotalTrac.velocidade ?? 0,
                                SensorTemperatura = posicaoTotalTrac.temperaturaS1.HasValue,
                            };
                            posicoes.Add(posicao);
                        }
                        catch (Exception ex)
                        {
                            Log($"Erro posicaoTotalTrac: {ex.Message}", 2);
                        }
                    }
                    Log($"Retornando {posicoes.Count} posicoes", 3);
                }
                AtualizarUltimaDataConsultada(posicoes);
            }
            catch (Exception ex)
            {
                Log($"Erro ObterPosicoes " + ex.Message, 2);
            }
            return posicoes;
        }

        private List<PosicaoTotalTrac> ObterPosicoesVeiculos()
        {
            try
            {
                string url = $"{conta.Protocolo}://{conta.Servidor}:{conta.Porta}/{conta.URI}";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (var requisicao = new HttpClient())
                {
                    requisicao.DefaultRequestHeaders.Accept.Clear();
                    requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var body = new
                    {
                        login = conta.Usuario,
                        senha = conta.Senha,
                        idUsuario = int.TryParse(ObterIdUsuario(), out var id) ? id : 0,
                        dataInicial = this.ultimaDataConsultada.ToString("yyyy-MM-dd HH:mm:ss"),
                        dataFinal = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8, "application/json");

                    var response = requisicao.PostAsync(url, content).Result;
                    var jsonRetorno = response.Content.ReadAsStringAsync().Result;

                    string stringsalvarLog = ObterValorMonitorar("SalvarLogResponse");
                    bool salvarLog = (!string.IsNullOrWhiteSpace(stringsalvarLog)) && bool.Parse(stringsalvarLog);
                    if (salvarLog)
                        LogNomeArquivo(jsonRetorno, DateTime.Now, "ResponsePosicoesTotalTrac", 0, true);

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<List<PosicaoTotalTrac>>(jsonRetorno);
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        throw new ServicoException("Integração não autorizada, verifique o usuário e senha!");
                    else if (!string.IsNullOrWhiteSpace(jsonRetorno))
                        throw new ServicoException($"Não retornou sucesso: '{jsonRetorno}'");
                    else
                        throw new ServicoException($"Problema ao efetuar a integração");
                }
            }
            catch (ServicoException excecao)
            {
                Log($"TotalTrac -> ServicoException: '{excecao.Message}'", 4);
            }
            catch (Exception excecao)
            {
                Log($"TotalTrac -> Ocorreu um erro: '{excecao.Message}'", 4);
            }
            return new List<PosicaoTotalTrac>();
        }

        private void ObterUltimaDataDoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = base.CarregarDadosDeControleDoArquivo(this.conta);
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_ULTIMA_DATA, dadosControle);
            try
            {
                this.ultimaDataConsultada = DateTime.Parse(value);
            }
            catch
            {
                this.ultimaDataConsultada = DateTime.Now;
            }

            Log($"Ultima data consulta {ultimaDataConsultada}", 2);
        }
        private void AtualizarUltimaDataConsultada(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes)
        {
            int cont = posicoes.Count;
            if (cont > 0)
            {
                this.ultimaDataConsultada = posicoes[0].Data;
                for (int i = 1; i < cont; i++)
                {
                    if (posicoes[i].Data > this.ultimaDataConsultada)
                    {
                        this.ultimaDataConsultada = posicoes[i].Data.AddSeconds(1);
                    }
                }
            }
        }
        private void SalvarUltimaDataNoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = new List<KeyValuePair<string, string>>();
            dadosControle.Add(new KeyValuePair<string, string>(KEY_ULTIMA_DATA, this.ultimaDataConsultada.ToString()));
            base.SalvarDadosDeControleNoArquivo(this.conta, dadosControle);
            Log($"Atualizando ultimo Timestamp {this.ultimaDataConsultada}", 2);
        }

        private string ObterIdUsuario()
        {
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_ID_USUARIO, this.conta.ListaParametrosAdicionais);
            return value;
        }

        #endregion

        #region Classe Retorno Posição TotalTrac
        public class PosicaoTotalTrac
        {
            public int? idUnico { get; set; }
            public string idVeiculo { get; set; }
            public string localizacao { get; set; }
            public string placa { get; set; }
            public DateTime? dataGPS { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public int? velocidade { get; set; }
            public int? temperaturaS1 { get; set; }
            public DateTime? dataServidor { get; set; }
        }
        #endregion
    }
}
