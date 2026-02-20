using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Servicos
{
    public class ManifestacaoDestinatario
    {
        public static Retorno.TRetEnvEvento EnviarManifestacaoDestinatario(string cnpj, Envio.TAmb tpAmb, Envio.TEventoInfEventoTpEvento tpEvento, string chaveNFe, DateTime dataEmissao, long idLote, string justificativa, string caminhoCertificado, string senhaCertificado)
        {
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(caminhoCertificado, senhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);

            string id = "ID" + tpEvento.ToString("d") + chaveNFe + "01";

            Envio.TEventoInfEventoDetEventoDescEvento descEvento;

            switch (tpEvento)
            {
                case Envio.TEventoInfEventoTpEvento.Item210200:
                    descEvento = Envio.TEventoInfEventoDetEventoDescEvento.ConfirmacaodaOperacao;
                    break;
                case Envio.TEventoInfEventoTpEvento.Item210210:
                    descEvento = Envio.TEventoInfEventoDetEventoDescEvento.CienciadaOperacao;
                    break;
                case Envio.TEventoInfEventoTpEvento.Item210220:
                    descEvento = Envio.TEventoInfEventoDetEventoDescEvento.DesconhecimentodaOperacao;
                    break;
                case Envio.TEventoInfEventoTpEvento.Item210240:
                    descEvento = Envio.TEventoInfEventoDetEventoDescEvento.OperacaonaoRealizada;
                    break;
                default:
                    descEvento = Envio.TEventoInfEventoDetEventoDescEvento.CienciadaOperacao;
                    break;
            }

            Envio.TEnvEvento dadosEnvio = new Envio.TEnvEvento();
            dadosEnvio.idLote = idLote.ToString();
            dadosEnvio.versao = "1.00";
            dadosEnvio.evento = new Envio.TEvento[]
            {
                 new Envio.TEvento()
                 {
                      infEvento = new Envio.TEventoInfEvento()
                      {
                           chNFe = chaveNFe,
                           cOrgao = Envio.TCOrgaoIBGE.Item91,
                           detEvento = new Envio.TEventoInfEventoDetEvento()
                           {
                                descEvento = descEvento,
                                versao = Envio.TEventoInfEventoDetEventoVersao.Item100,
                                xJust = tpEvento == Envio.TEventoInfEventoTpEvento.Item210240 ? justificativa : null
                           },
                           dhEvento = dataEmissao.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                           Id = id,
                           Item = cnpj,
                           ItemElementName = Envio.ItemChoiceType.CNPJ,
                           nSeqEvento = "1",
                           tpAmb = tpAmb,
                           tpEvento = tpEvento,
                           verEvento = "1.00",
                      },
                      versao = "1.00"
                 }
            };

            ServicoRecepcaoEvento.nfeCabecMsg dadosCabecalho = new ServicoRecepcaoEvento.nfeCabecMsg()
            {
                cUF = "91",
                versaoDados = "1.00"
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Envio.TEnvEvento));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "http://www.portalfiscal.inf.br/nfe");

            XmlDocument doc = new XmlDocument();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, dadosEnvio, namespaces);

                memoryStream.Position = 0;

                doc.Load(memoryStream);
            }

            if (doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
            {
                XmlDeclaration xmlDeclaration = (XmlDeclaration)doc.FirstChild;
                xmlDeclaration.Encoding = "UTF-8";
            }

            XmlElement xmlElement = NFe.Servicos.Assinatura.AssinarXML(doc.GetElementsByTagName("evento")[0], doc.GetElementsByTagName("infEvento")[0], certificado);

            using (ServicoRecepcaoEvento.RecepcaoEventoSoapClient svcRecepcaoEvento = new ServicoRecepcaoEvento.RecepcaoEventoSoapClient())
            {
                svcRecepcaoEvento.ClientCredentials.ClientCertificate.Certificate = certificado;

                XmlNode dadosRetorno = svcRecepcaoEvento.nfeRecepcaoEvento(dadosCabecalho, doc.DocumentElement);

                XmlSerializer ser = new XmlSerializer(typeof(Retorno.TRetEnvEvento));

                using (TextReader reader = new StringReader(dadosRetorno.OuterXml))
                {
                    Retorno.TRetEnvEvento result = (Retorno.TRetEnvEvento)ser.Deserialize(reader);

                    return result;
                }
            }
        }

        public static Retorno.TRetEnvEvento EnviarManifestacaoDestinatario4(string cnpj, Envio.TAmb tpAmb, Envio.TEventoInfEventoTpEvento tpEvento, string chaveNFe, DateTime dataEmissao, long idLote, string justificativa, string caminhoCertificado, string senhaCertificado)
        {
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(caminhoCertificado, senhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);

            string id = "ID" + tpEvento.ToString("d") + chaveNFe + "01";

            Envio.TEventoInfEventoDetEventoDescEvento descEvento;

            switch (tpEvento)
            {
                case Envio.TEventoInfEventoTpEvento.Item210200:
                    descEvento = Envio.TEventoInfEventoDetEventoDescEvento.ConfirmacaodaOperacao;
                    break;
                case Envio.TEventoInfEventoTpEvento.Item210210:
                    descEvento = Envio.TEventoInfEventoDetEventoDescEvento.CienciadaOperacao;
                    break;
                case Envio.TEventoInfEventoTpEvento.Item210220:
                    descEvento = Envio.TEventoInfEventoDetEventoDescEvento.DesconhecimentodaOperacao;
                    break;
                case Envio.TEventoInfEventoTpEvento.Item210240:
                    descEvento = Envio.TEventoInfEventoDetEventoDescEvento.OperacaonaoRealizada;
                    break;
                default:
                    descEvento = Envio.TEventoInfEventoDetEventoDescEvento.CienciadaOperacao;
                    break;
            }

            Envio.TEnvEvento dadosEnvio = new Envio.TEnvEvento();
            dadosEnvio.idLote = idLote.ToString();
            dadosEnvio.versao = "1.00";
            dadosEnvio.evento = new Envio.TEvento[]
            {
                 new Envio.TEvento()
                 {
                      infEvento = new Envio.TEventoInfEvento()
                      {
                           chNFe = chaveNFe,
                           cOrgao = Envio.TCOrgaoIBGE.Item91,
                           detEvento = new Envio.TEventoInfEventoDetEvento()
                           {
                                descEvento = descEvento,
                                versao = Envio.TEventoInfEventoDetEventoVersao.Item100,
                                xJust = tpEvento == Envio.TEventoInfEventoTpEvento.Item210240 ? justificativa : null
                           },
                           dhEvento = dataEmissao.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                           Id = id,
                           Item = cnpj,
                           ItemElementName = Envio.ItemChoiceType.CNPJ,
                           nSeqEvento = "1",
                           tpAmb = tpAmb,
                           tpEvento = tpEvento,
                           verEvento = "1.00",
                      },
                      versao = "1.00"
                 }
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Envio.TEnvEvento));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "http://www.portalfiscal.inf.br/nfe");

            XmlDocument doc = new XmlDocument();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, dadosEnvio, namespaces);

                memoryStream.Position = 0;

                doc.Load(memoryStream);
            }

            if (doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
            {
                XmlDeclaration xmlDeclaration = (XmlDeclaration)doc.FirstChild;
                xmlDeclaration.Encoding = "UTF-8";
            }

            NFe.Servicos.Assinatura.AssinarXML(doc.GetElementsByTagName("evento")[0], doc.GetElementsByTagName("infEvento")[0], certificado);
                             
            ServicoRecepcaoEvento4.NFeRecepcaoEvento4SoapClient svcRecepcaoEvento = ObterClient();

            svcRecepcaoEvento.ClientCredentials.ClientCertificate.Certificate = certificado;

            XmlNode dadosRetorno = svcRecepcaoEvento.nfeRecepcaoEventoNF(doc.DocumentElement);

            using (TextReader reader = new StringReader(dadosRetorno.OuterXml))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Retorno.TRetEnvEvento));

                Retorno.TRetEnvEvento result = (Retorno.TRetEnvEvento)ser.Deserialize(reader);

                return result;
            }
        }

        private static ServicoRecepcaoEvento4.NFeRecepcaoEvento4SoapClient ObterClient()
        {
            string url = $"https://www.nfe.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx";

            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);
            binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Certificate;

            return new ServicoRecepcaoEvento4.NFeRecepcaoEvento4SoapClient(binding, endpointAddress);
        }
    }
}
