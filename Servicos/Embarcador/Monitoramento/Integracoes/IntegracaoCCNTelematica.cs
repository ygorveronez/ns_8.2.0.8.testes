using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoCCNTelematica : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoCCNTelematica Instance;
        private static readonly string nameConfigSection = "CCNTelematica";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Construtor privado

        private IntegracaoCCNTelematica(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CCNTelematica, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoCCNTelematica GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoCCNTelematica(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        protected override void ComplementarConfiguracoes() { }

        protected override void Validar() { }

        protected override void Preparar() { }

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

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();

            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            try
            {
                posicoes = BuscaPosicoesVeiculo();
                Log($"Recebidas posiçoes de {posicoes.GroupBy(x => x.Placa).ToList().Count} veículos", 3);
                Log($"{posicoes.Count} posicoes", 3);
            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes " + ex.Message, 3);
            }

            return posicoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> BuscaPosicoesVeiculo()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesRetornar = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);

            //SOAP envelope string
            string xmlSOAP =
               $@"  <soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:trmsuiteWS"">
                       <soapenv:Header/>
                       <soapenv:Body>
                          <urn:trmsuiteWSGetUnitsByUser soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
                             <username xsi:type=""xsd:string"">{this.conta.Usuario}</username>
                             <password xsi:type=""xsd:string"">{this.conta.Senha}</password>
                          </urn:trmsuiteWSGetUnitsByUser>
                       </soapenv:Body>
                    </soapenv:Envelope>";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCCNTelematica));
            HttpContent httpContent = new StringContent(xmlSOAP);

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Add("SOAPAction", "trmsuiteWSGetUnitsByUser");
            req.Method = HttpMethod.Post;
            req.Content = httpContent;
            req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/xml;charset=UTF-8");

            HttpResponseMessage retornoRequisicao = client.SendAsync(req).Result;
            var result = retornoRequisicao.Content.ReadAsStringAsync().Result;
            var ObjetoJson = XmlToJson(result);

            dynamic objetoDinamico = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(ObjetoJson);

            dynamic erro = objetoDinamico["SOAP-ENV:Envelope"]["SOAP-ENV:Body"]["ns1:trmsuiteWSGetUnitsByUserResponse"]["code"]["#text"];
            dynamic response = objetoDinamico["SOAP-ENV:Envelope"]["SOAP-ENV:Body"]["ns1:trmsuiteWSGetUnitsByUserResponse"]["response"]["#text"];

            if (erro.Value == "1")
            {
                List<PosicaoIntegracaoCCNTelematica> posicoes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PosicaoIntegracaoCCNTelematica>>(response.Value);
                foreach(PosicaoIntegracaoCCNTelematica posicao in posicoes)
                {
                    if (posicao.Placa.HasValue() && posicao.Num_Economico.HasValue() && posicao.Latitud.HasValue() && posicao.Longitud.HasValue())
                        posicoesRetornar.Add( ObterPosicao(posicao));
                }
            }
            else
                Log("Erro BuscaPosicoesVeiculo " + response.Value, 4);

            return posicoesRetornar;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao ObterPosicao(PosicaoIntegracaoCCNTelematica placaPosicao)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(placaPosicao.Latitud, placaPosicao.Longitud);
            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao = new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao();

            DateTime dataPosicao = DateTime.Now;

            posicao.Data = dataPosicao;
            posicao.DataVeiculo = dataPosicao;
            posicao.IDEquipamento = placaPosicao.Num_Economico;
            posicao.Latitude = wayPoint.Latitude;
            posicao.Longitude = wayPoint.Longitude;
            posicao.Velocidade = 0;
            posicao.Ignicao = 0;
            posicao.SensorTemperatura = false;
            posicao.Descricao = "";
            posicao.Placa = placaPosicao.Placa;
            posicao.Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.CCNTelematica;
            return posicao;
        }

        public string XmlToJson(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            string jsonText = JsonConvert.SerializeXmlNode(doc);
            return jsonText;
        }
        #endregion
    }

    public class PosicaoIntegracaoCCNTelematica
    {
        public string Placa { get; set; }
        public string Num_Economico { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
    }
}
