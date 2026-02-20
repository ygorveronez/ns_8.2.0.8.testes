using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoRastreamentoLink : Abstract.AbstractIntegracaoSOAP
    {
        #region Atributos privados

        private static IntegracaoRastreamentoLink Instance;
        private static readonly string nameConfigSection = "Link";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;


        private Servicos.IntegracaoLink.wsUltimasPortTypeClient wsLink;
        #endregion

        #region Construtor privado

        private IntegracaoRastreamentoLink(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Link, nameConfigSection, cliente) { }

        #endregion


        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoRastreamentoLink GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoRastreamentoLink(cliente);
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
            InicializandoWS();
            IntegrarPosicao();

        }

        #endregion


        #region Metodos Privados

        private void InicializandoWS()
        {
            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);

            wsLink = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<IntegracaoLink.wsUltimasPortTypeClient, IntegracaoLink.wsUltimasPortType>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Link_wsUltimas, url);

            wsLink.ClientCredentials.UserName.UserName = this.conta.Usuario;
            wsLink.ClientCredentials.UserName.Password = this.conta.Senha;
        }

        private void IntegrarPosicao()
        {

            Log($"Consultando posicoes", 2);

            // Busca as posições do WebService
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();

            // Registra as posições recebidas dos veículos
            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);

        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            // Busca os eventos normais, que contém apenas as posições dos veículos
            try
            {
                // Servicos.IntegracaoLink.retorno retorno = ObterPosicoesIntegracao();

                posicoes = ObterPosicoesIntegracao();

                //if (!string.IsNullOrEmpty(retorno.erro))
                //{
                //    Log($"Recebidos {retorno.resposta.Length} registros de posições", 3);
                //    posicoes = ConverterParaPosicao(retorno.resposta);
                //}
                //else
                //    Log("Erro ObterPosições " + retorno.erro, 3);

            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes " + ex.Message, 3);
            }

            return posicoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ConverterParaPosicao(IntegracaoLink.posicao[] posicoes)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesretorno = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            int total = posicoes.Length;
            for (int i = 0; i < total; i++)
            {
                // Conversão das coordenadas em GMS para decimal
                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(posicoes[i].latitude, posicoes[i].longitude);
                posicoesretorno.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                {
                    Data = DateTime.Parse(posicoes[i].data_hora),
                    DataVeiculo = DateTime.Parse(posicoes[i].data_hora),
                    IDEquipamento = posicoes[i].numero_serie,
                    Latitude = wayPoint.Latitude,
                    Longitude = wayPoint.Longitude,
                    Velocidade = posicoes[i].velocidade,
                    Ignicao = posicoes[i].ignicao,
                    SensorTemperatura = false,
                    Descricao = posicoes[i].logradouro,
                    Placa = posicoes[i].placa,
                });

            }
            return posicoesretorno;
        }
        /**
        * Requisição ao serviço ObtemEventosNormais que contém apenas posições dos veículos
        */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoesIntegracao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesRetornar = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            string url = "https://seguro.linkmonitoramento.com.br/monitoramento/servicos/api/ultimas/index.php";

            //SOAP envelope string
            string xmlSOAP =
               $@"<?xml version=""1.0"" encoding=""ISO-8859-1""?>
               <soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:wsPosicoes"">
                <soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:wsPosicoes"">
                   <soapenv:Header/>
                     <soapenv:Body>
                          <urn:recebeUltimas soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
                            <usuario_mapas xsi:type=""xsd:string"">{this.conta.Usuario}</usuario_mapas>
                                  <senha_mapas xsi:type=""xsd:string"">{this.conta.Senha}</senha_mapas>
                                 </urn:recebeUltimas>
                               </soapenv:Body>
                             </soapenv:Envelope>
                       <soapenv:Header/>
                     <soapenv:Body>
                      <urn:recebeUltimas soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
                         <usuario_mapas xsi:type=""xsd:string"">{this.conta.Usuario}</usuario_mapas>
                         <senha_mapas xsi:type=""xsd:string"">{this.conta.Senha}</senha_mapas>
                      </urn:recebeUltimas>
                   </soapenv:Body>
                </soapenv:Envelope>";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoRastreamentoLink));
            HttpContent httpContent = new StringContent(xmlSOAP);
            // HttpResponseMessage response;

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Add("SOAPAction", "recebeUltimas");
            req.Method = HttpMethod.Post;
            req.Content = httpContent;
            req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/xml");

            HttpResponseMessage retornoRequisicao = client.SendAsync(req).Result;
            var result = retornoRequisicao.Content.ReadAsStringAsync().Result;
            var ObjetoJson = XmlToJson(result);

            dynamic objetoDinamico = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(ObjetoJson);

            dynamic erro = objetoDinamico["SOAP-ENV:Envelope"]["SOAP-ENV:Body"]["ns1:recebeUltimasResponse"]["return"]["erro"];

            if (erro["#text"].Value != "Ok!")
                Log("Erro ObterPosicoes " + erro["#text"].Value, 3);
            else
            {
                dynamic Itens = objetoDinamico["SOAP-ENV:Envelope"]["SOAP-ENV:Body"]["ns1:recebeUltimasResponse"]["return"]["resposta"]["item"];

                for (int i = 0; i < Itens?.Count; i++)
                    posicoesRetornar.Add(ObterPosicao(Itens[i]));

            }


            return posicoesRetornar;

            //DateTime inicio = DateTime.UtcNow;
            //Servicos.IntegracaoLink.retorno response;

            //Log("Requisicao recebeUltimas ", inicio, 3);
            //response = wsLink.recebeUltimas(this.conta.Usuario, this.conta.Senha);

            //return response;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao ObterPosicao(dynamic item)
        {

            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(item["latitude"]["#text"].Value, item["longitude"]["#text"].Value);
            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao = new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao();

            DateTime dataPosicao = DateTime.Parse(item["data_hora"]?["#text"].Value);

            posicao.Data = dataPosicao.AddHours(-3);
            posicao.DataVeiculo = dataPosicao.AddHours(-3);
            posicao.IDEquipamento = item["numero_serie"]?["#text"]?.Value ?? "";
            posicao.Latitude = wayPoint.Latitude;
            posicao.Longitude = wayPoint.Longitude;
            posicao.Velocidade = string.IsNullOrEmpty(item["velocidade"]?["#text"]?.Value ?? "") ? 0 : int.Parse(item["velocidade"]?["#text"].Value);
            posicao.Ignicao = string.IsNullOrEmpty(item["ignicao"]?["#text"].Value) ? 0 : int.Parse(item["ignicao"]?["#text"].Value);
            posicao.SensorTemperatura = false;
            posicao.Descricao = item["logradouro"]?["#text"]?.Value ?? "";
            posicao.Placa = item["placa"]?["#text"].Value;
            posicao.Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Link;
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
}
