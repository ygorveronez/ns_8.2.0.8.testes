using iTextSharp.text.pdf.qrcode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Integracao.Migrate
{
    public partial class IntegracaoMigrate
    {
        #region Atributos Globais

        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMigrate _configuracaoIntegracao;
        private readonly string _chaveParceiro_PK = "o41sx0KIbbrouQLyCYmjKA==";
        private readonly string _urlProducao = "https://nfse.invoicy.com.br/arecepcao.aspx";
        private readonly string _urlHomologacao = "https://homolog.invoicy.com.br/arecepcao.aspx";

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoMigrate(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoMigrate repMigrate = new Repositorio.Embarcador.Configuracoes.IntegracaoMigrate(unitOfWork);
            _configuracaoIntegracao = repMigrate.Buscar();
        }

        #endregion Construtores

        #region Métodos Públicos

        #endregion

        #region Métodos Privados

        private static ServicosMigrate.recepcaoSoapPortClient ObterClientIntegrarNFSe(string url)
        {
            ServicosMigrate.recepcaoSoapPortClient cliente = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.None;
                cliente = new ServicosMigrate.recepcaoSoapPortClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                cliente = new ServicosMigrate.recepcaoSoapPortClient(binding, endpointAddress);
            }

            return cliente;
        }

        //Função de geração de hashMD5 para CK.
        private String GeraHashMD5(string texto, string chaveComunicacao_CK)
        {
            using (System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(chaveComunicacao_CK + texto.Trim()));

                System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }

        private string ObterUrlWebService(Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            if (!string.IsNullOrEmpty(_configuracaoIntegracao?.URL))
                return _configuracaoIntegracao?.URL;

            if (tipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                return _urlProducao;
            else
                return _urlHomologacao;
        }

        public static T DeSerialize<T>(string pXml)
        {
            StringReader reader = new StringReader(pXml);
            XmlSerializer desserializador = new XmlSerializer(typeof(T));
            return (T)desserializador.Deserialize(reader);
        }

        public static string Serialize(object pObject)
        {
            StringBuilder writer = new StringBuilder();
            XmlSerializer serializer = new XmlSerializer(pObject.GetType());

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(String.Empty, string.Empty);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Indent = false;
            settings.Encoding = Encoding.UTF8;

            using (XmlWriter xmlWriter = XmlWriter.Create(writer, settings))
            {
                serializer.Serialize(xmlWriter, pObject, ns);
                return writer.ToString();
            }
        }

        public void SalvarArquivoXML(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, bool armazenarEmArquivo, string xmlBase64, string xmlConteudo, Dominio.Enumeradores.TipoXMLCTe tipoXML, string status, Repositorio.UnitOfWork unitOfWork)
        {
            if (cte != null)
            {
                byte[] decodedData = null;

                if (!string.IsNullOrWhiteSpace(xmlBase64))
                    decodedData = Encoding.Default.GetPreamble().Concat(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Default, System.Convert.FromBase64String(xmlBase64))).ToArray();
                else if (!string.IsNullOrWhiteSpace(xmlConteudo))
                    decodedData = Encoding.UTF8.GetBytes(xmlConteudo);

                if (decodedData != null)
                {
                    Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);

                    Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);

                    Dominio.Entidades.XMLCTe xmlCTe = repXMLCTe.BuscarPorCTe(cte.Codigo, tipoXML);

                    if (xmlCTe == null)
                        xmlCTe = new Dominio.Entidades.XMLCTe();

                    if (armazenarEmArquivo)
                    {
                        string arquivo = servicoCTe.CriarERetornarCaminhoXMLCTe(cte, status, unitOfWork);
                        Utilidades.IO.FileStorageService.Storage.WriteAllBytes(arquivo, decodedData);
                        xmlCTe.XMLArmazenadoEmArquivo = true;
                        xmlCTe.XML = "";
                    }
                    else
                    {
                        xmlCTe.XMLArmazenadoEmArquivo = false;
                        xmlCTe.XML = Encoding.UTF8.GetString(decodedData);
                    }

                    xmlCTe.CTe = cte;
                    xmlCTe.Tipo = tipoXML;


                    if (xmlCTe.Codigo > 0)
                        repXMLCTe.Atualizar(xmlCTe);
                    else
                        repXMLCTe.Inserir(xmlCTe);
                }
            }
        }

        private void SalvarArquivoPDF(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string pdfBase64, Repositorio.UnitOfWork unitOfWork)
        {
            string diretorio = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;
            if (!string.IsNullOrWhiteSpace(diretorio))
            {
                if (!string.IsNullOrWhiteSpace(pdfBase64))
                {
                    string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios, "NFSe", cte.Empresa.CNPJ, cte.Codigo.ToString() + "_" + cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString()) + ".pdf";

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                    {
                        //byte[] decodedData = System.Text.Encoding.Default.GetPreamble().Concat(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Default, System.Convert.FromBase64String(pdfBase64))).ToArray();
                        byte[] pdf = Convert.FromBase64String(pdfBase64);
                        Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoPDF, pdf);
                    }
                }
            }
        }

        #endregion Métodos Privados
    }
}