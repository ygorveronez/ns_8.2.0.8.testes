using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoAngelLira : Abstract.AbstractIntegracaoSOAP
    {
        #region Atributos privados

        private static IntegracaoAngelLira Instance;
        private static readonly string nameConfigSection = "AngelLira";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        private long ultimoIDSequencialParaIniciar;
        private long ultimoSequencial;

        private const string KEY_CONF_ULTIMO_ID_SEQUENCIAL_PARA_INICIAR = "UltimoIDSequencialParaIniciar";
        private const string KEY_ULTIMO_SEQUENCIAL = "UltimoSequencial";

        #endregion

        #region Construtor privado

        private IntegracaoAngelLira(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoAngelLira GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoAngelLira(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        protected override void ComplementarConfiguracoes()
        {
            long.TryParse(ObterValorOpcao(KEY_CONF_ULTIMO_ID_SEQUENCIAL_PARA_INICIAR), out this.ultimoIDSequencialParaIniciar);
        }

        protected override void Validar()
        {
            base.ValidarConfiguracaoDiretorioEArquivoControle(base.contasIntegracao);
        }

        protected override void Preparar() { }

        protected override void Executar(ContaIntegracao contaIntegracao)
        {
            this.conta = contaIntegracao;
            ObterUltimosSequenciaisDoArquivo();
            IntegrarPosicoes();
            SalvarUltimosSequenciaisNoArquivo();
        }

        #endregion

        #region Métodos privados 

        private void IntegrarPosicoes()
        {
            Log($"Consultando posicoes", 2);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();

            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            try
            {
                posicoes = BuscaPosicoesVeiculos();
                Log($"Recebidas posiçoes de {posicoes.GroupBy(x => x.Placa).ToList().Count} veículos", 3);
                Log($"{posicoes.Count} posicoes", 3);
            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes " + ex.Message, 3);
            }

            return posicoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> BuscaPosicoesVeiculos()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesRetornar = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);
            string homologacao = "false";

            //SOAP envelope string
            string xmlSOAP =
            $@"  <soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:ang=""http://www.angellira.com.br/"">
                    <soap:Header>
                        <ang:ValidationSoapHeader>
                            <ang:userCod>{this.conta.Usuario}</ang:userCod>
                            <ang:userPwd>{this.conta.Senha}</ang:userPwd>
                            <ang:homologacao>{homologacao}</ang:homologacao>
                            <ang:api_interna>false</ang:api_interna>
                        </ang:ValidationSoapHeader>
                    </soap:Header>
                    <soap:Body>
                        <ang:GetPosicaoVeiculo>
                            <ang:id>{this.ultimoSequencial}</ang:id>
                        </ang:GetPosicaoVeiculo>
                    </soap:Body>
                </soap:Envelope>";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoAngelLira));
            HttpContent httpContent = new StringContent(xmlSOAP);

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Add("SOAPAction", "http://www.angellira.com.br/GetPosicaoVeiculo");
            req.Method = HttpMethod.Post;
            req.Content = httpContent;
            req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/xml;charset=UTF-8");

            HttpResponseMessage retornoRequisicao = client.SendAsync(req).Result;
            var result = retornoRequisicao.Content.ReadAsStringAsync().Result.ToString().Replace(@" xsi:nil=""true""", "");
            var ObjetoJson = XmlToJson(result);

            dynamic objetoDinamico = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(ObjetoJson);

            if (retornoRequisicao.StatusCode == System.Net.HttpStatusCode.OK)
            {
                dynamic response = objetoDinamico["soap:Envelope"]["soap:Body"]["GetPosicaoVeiculoResponse"]["GetPosicaoVeiculoResult"]["Posicao"];
                List<PosicaoIntegracaoAngelLira> posicoes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PosicaoIntegracaoAngelLira>>(response.ToString());
                foreach (PosicaoIntegracaoAngelLira posicao in posicoes)
                    posicoesRetornar.Add(ObterPosicao(posicao));
            }
            else
            {
                string mensagemErro = objetoDinamico["soap:Envelope"]["soap:Body"]["soap:Fault"]["soap:Reason"]["soap:Text"]["#text"];
                Log("Erro BuscaPosicoesVeiculo " + mensagemErro, 4);
            }

            return posicoesRetornar;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao ObterPosicao(PosicaoIntegracaoAngelLira placaPosicao)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(placaPosicao.LATITUDE ?? 0, placaPosicao.LONGITUDE ?? 0);
            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao = new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao();

            DateTime dataVeiculo = DateTime.Now;
            try
            {
                dataVeiculo = Convert.ToDateTime(placaPosicao.DATAHORA);
            }
            catch (Exception e) 
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao converter data de posição Angel Lira - continuando processamento: {e.ToString()}", "CatchNoAction");
            }; //Se não conseguiu converter a data, segue o baile.
            
            posicao.Data = DateTime.Now;
            posicao.DataVeiculo = dataVeiculo;
            posicao.IDEquipamento = placaPosicao.IDRASTREADOR;
            posicao.Latitude = wayPoint.Latitude;
            posicao.Longitude = wayPoint.Longitude;
            posicao.Velocidade = placaPosicao.VELOCIDADE ?? 0;
            posicao.Ignicao = placaPosicao.IGNICAO ?? 0;
            posicao.Temperatura = placaPosicao.TEMPERATURA ?? 0;
            posicao.Descricao = placaPosicao.DESCRICAO;
            posicao.Placa = placaPosicao.PLACA.Replace("-", "");
            posicao.Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.AngelLira;

            if (placaPosicao.IDMENSAGEM > this.ultimoSequencial)
                this.ultimoSequencial = (long)placaPosicao.IDMENSAGEM;

            return posicao;
        }

        public string XmlToJson(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            string jsonText = JsonConvert.SerializeXmlNode(doc);
            return jsonText;
        }

        /**
         * Busca os últimos sequenciais dos eventos das posições já lidas
         */
        private void ObterUltimosSequenciaisDoArquivo()
        {
            string value;
            List<KeyValuePair<string, string>> dadosControle = base.CarregarDadosDeControleDoArquivo(this.conta);

            value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_ULTIMO_SEQUENCIAL, dadosControle);
            this.ultimoSequencial = long.Parse((String.IsNullOrWhiteSpace(value)) ? ObterValorOpcao(KEY_CONF_ULTIMO_ID_SEQUENCIAL_PARA_INICIAR) : value);

            Log($"Lido ultimo sequencial normal {ultimoSequencial}", 2);

        }

        /**
         * Registra no arquivo os últimos números sequenciais dos eventos
         */
        private void SalvarUltimosSequenciaisNoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = new List<KeyValuePair<string, string>>();
            dadosControle.Add(new KeyValuePair<string, string>(KEY_ULTIMO_SEQUENCIAL, ultimoSequencial.ToString()));
            base.SalvarDadosDeControleNoArquivo(this.conta, dadosControle);
            Log($"Salvo ultimo sequencial normal {ultimoSequencial}", 2);
        }
        #endregion
    }

    public class PosicaoIntegracaoAngelLira
    {
        public string DATAHORA { get; set; }
        public string DESCRICAO { get; set; }
        public long? IDMENSAGEM { get; set; }
        public string IDRASTREADOR { get; set; }
        public double? LATITUDE { get; set; }
        public double? LONGITUDE { get; set; }
        public string PLACA { get; set; }
        public int? IGNICAO { get; set; }
        public int? VELOCIDADE { get; set; }
        public string DATATEMPERATURA { get; set; }
        public decimal? TEMPERATURA { get; set; }
        public string CIDADE { get; set; }
        public string UF { get; set; }
        public int? HODOMETRO { get; set; }
    }
}

