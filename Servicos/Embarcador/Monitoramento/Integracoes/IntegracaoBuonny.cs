using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Infrastructure.Services.HttpClientFactory;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoBuonny : Abstract.AbstractIntegracaoSOAP
    {
        #region Atributos privados


        private static IntegracaoBuonny Instance;
        private static readonly string nameConfigSection = "Buonny";
        private Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        private Servicos.Embarcador.Integracao.Buonny.IntegracaoBuonny ServicoIntegracaoBuonny;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_TOKEN = "Token";
        private const string KEY_CNPJ_EMPRESA = "CNPJEmpresa";
        private const string KEY_NAMESPACE_WS = "Namespace";

        #endregion

        #region Construtor privado

        private IntegracaoBuonny(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny, nameConfigSection, cliente) { }

        #endregion


        #region Métodos públicos

        public static IntegracaoBuonny GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoBuonny(cliente);
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
            InicializarWS();
            IntegrarPosicao();
        }

        #endregion

        #region Métodos privados

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

            try
            {
                string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);

                var token = this.conta.ListaParametrosAdicionais
               .FirstOrDefault(p => p.Key.Equals(KEY_TOKEN, StringComparison.OrdinalIgnoreCase))
               .Value;

                var cnpj = this.conta.ListaParametrosAdicionais
                    .FirstOrDefault(p => p.Key.Equals(KEY_CNPJ_EMPRESA, StringComparison.OrdinalIgnoreCase))
                    .Value;

                var namespaceWs = this.conta.ListaParametrosAdicionais
                    .FirstOrDefault(p => p.Key.Equals(KEY_NAMESPACE_WS, StringComparison.OrdinalIgnoreCase))
                    .Value;

                if (string.IsNullOrWhiteSpace(cnpj) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(namespaceWs))
                    throw new Exception("Parâmetros de autenticação não configurados para Buonny.");

                this.ServicoIntegracaoBuonny.DefinirConfiguracoes(url, this.conta.Usuario, this.conta.Senha);

                (string resultadoXml, int statusCode) = CallWebServiceAsync(token, cnpj, url, namespaceWs);

                if (string.IsNullOrEmpty(resultadoXml))
                    throw new Exception("Erro ao buscar as posições dos veículos.");

                List<PosicaoViagemItensResponse> ultimasPosicoes = DeserializeXmlToPosicaoEmViagemResponse(resultadoXml, statusCode);


                int total = ultimasPosicoes?.Count ?? 0;
                for (int i = 0; i < total; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao novaPosicao = new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                    {
                        Data = ultimasPosicoes[i].dataHora,
                        DataVeiculo = ultimasPosicoes[i].dataHora,
                        Placa = ultimasPosicoes[i].placa.Trim().Replace("-", ""),
                        IDEquipamento = ultimasPosicoes[i].idTerminal,
                        Latitude = ultimasPosicoes[i].latitude,
                        Longitude = ultimasPosicoes[i].longitude,
                        Descricao = ultimasPosicoes[i].logradouro,
                        Velocidade = 0,
                        Temperatura = 0,
                        Ignicao = 0,
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Buonny
                    };

                    posicoes.Add(novaPosicao);
                }
            }
            catch (Exception e)
            {
                Log($"Erro ObterPosicoes Buonny " + e.Message, 3);
            }
            return posicoes;
        }

        private void InicializarWS()
        {
            Log("Inicializando WS", 2);
            this.ServicoIntegracaoBuonny = Servicos.Embarcador.Integracao.Buonny.IntegracaoBuonny.GetInstance();
        }

        public static (string json, int statusCode) CallWebServiceAsync(string token, string cnpj, string url, string namespaceWs)
        {
            string soapRequest = $@"
<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/'
                  xmlns:buon='{namespaceWs}'>
    <soapenv:Header/>
    <soapenv:Body>
        <buon:autenticador>
            <buon:autenticacao>
                <buon:token>{token}</buon:token>
            </buon:autenticacao>
            <buon:cnpj_cliente>{cnpj}</buon:cnpj_cliente>
        </buon:autenticador>
    </soapenv:Body>
</soapenv:Envelope>";


            var client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBuonny));
            var content = new StringContent(soapRequest, Encoding.UTF8, "text/xml");

            var response = client.PostAsync(url, content).Result;

            string responseContent = response.Content.ReadAsStringAsync().Result;
            return (responseContent, (int)response.StatusCode);


        }

        public static List<PosicaoViagemItensResponse> DeserializeXmlToPosicaoEmViagemResponse(string xmlResponse, int statusCode)
        {

            XDocument doc = XDocument.Parse(xmlResponse);

            List<PosicaoViagemItensResponse> posicoes = new List<PosicaoViagemItensResponse>();

            if (statusCode != 200)
            {
                var mensagemErro = doc.Descendants()
                    .Where(p => p.Name.LocalName == "faultstring")
                    .Select(p => p.Value).FirstOrDefault();

                throw new Exception(mensagemErro);
            }

            posicoes = doc.Descendants()
                          .Where(p => p.Name.LocalName == "posicao_em_viagem")
                          .Select(p => new PosicaoViagemItensResponse
                          {
                              idPosicao = p.Element("idPosicao")?.Value,
                              dataHora = DateTime.Parse(p.Element("dataHora")?.Value),
                              idTerminal = p.Element("idTerminal")?.Value,
                              placa = p.Element("placa")?.Value,
                              latitude = double.TryParse(p.Element("latitude")?.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double result)
                              ? result
                              : 0,
                              longitude = double.TryParse(p.Element("longitude")?.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double resultLong)
                              ? resultLong
                              : 0,
                              logradouro = p.Element("logradouro")?.Value
                          }).ToList();


            return posicoes;


        }
        #endregion
    }



    public class PosicaoViagemResponse
    {
        [XmlArray("posicoes")]
        [XmlArrayItem("posicao_em_viagem")]
        public PosicaoViagemItensResponse[] posicoes { get; set; }

        public PosicaoViagemResponse() { }

        public PosicaoViagemResponse(PosicaoViagemItensResponse[] posicoes)
        {
            this.posicoes = posicoes;
        }
    }


    public class PosicaoViagemItensResponse
    {
        public string idPosicao { get; set; }

        public DateTime dataHora { get; set; }

        public string idTerminal { get; set; }

        public string placa { get; set; }

        public double latitude { get; set; }

        public double longitude { get; set; }

        public string logradouro { get; set; }
    }
}
