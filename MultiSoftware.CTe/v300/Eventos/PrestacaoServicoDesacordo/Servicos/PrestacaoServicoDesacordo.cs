using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text.RegularExpressions;
using System.Net;

namespace MultiSoftware.CTe.v300.Eventos.PrestacaoServicoDesacordo.Servicos
{
    public class PrestacaoServicoDesacordo
    {
        public static Retorno.TRetEvento EnviarDesacordoPrestacaoServico(string cnpj, Envio.TCOrgaoIBGE uf, Envio.TAmb tpAmb, string chaveCTe, DateTime dataEmissao, long idLote, string justificativa, string caminhoCertificado, string senhaCertificado, string urlSefaz, ref string xmlEvento)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

            var certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(caminhoCertificado, senhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);

            string id = "ID610110" + chaveCTe + "01";

            Envio.TEvento eventoCTe = new Envio.TEvento()
            {
                versao = "3.00",
                infEvento = new Envio.TEventoInfEvento()
                {
                    Id = id,
                    chCTe = chaveCTe,
                    CNPJ = cnpj,
                    cOrgao = uf,
                    detEvento = new Envio.TEventoInfEventoDetEvento()
                    {
                        versaoEvento = "3.00",
                        evPrestDesacordo = new Envio.evPrestDesacordo()
                        {
                            descEvento = Envio.evPrestDesacordoDescEvento.PrestaçãodoServiçoemDesacordo,
                            xObs = justificativa,
                            indDesacordoOper = Envio.evPrestDesacordoIndDesacordoOper.Item1
                        }
                    },
                    dhEvento = dataEmissao.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    nSeqEvento = "1",
                    tpAmb = tpAmb,
                    tpEvento = "610110"
                }
            };

            ServicoRecepcaoEvento.cteCabecMsg dadosCabecalho = new ServicoRecepcaoEvento.cteCabecMsg()
            {
                cUF = uf.ToString("d"),
                versaoDados = "3.00"
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Envio.TEvento));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "http://www.portalfiscal.inf.br/cte");

            XmlDocument doc = new XmlDocument();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, eventoCTe, namespaces);

                memoryStream.Position = 0;

                doc.Load(memoryStream);
            }

            if (doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
            {
                XmlDeclaration xmlDeclaration = (XmlDeclaration)doc.FirstChild;
                xmlDeclaration.Encoding = "UTF-8";
            }

            XmlElement xmlElement = CTe.Servicos.Assinatura.AssinarXML(doc.GetElementsByTagName("eventoCTe")[0], doc.GetElementsByTagName("infEvento")[0], certificado);

            using (ServicoRecepcaoEvento.CteRecepcaoEventoSoap12Client svcRecepcaoEvento = new ServicoRecepcaoEvento.CteRecepcaoEventoSoap12Client())
            {
                svcRecepcaoEvento.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlSefaz);

                svcRecepcaoEvento.ClientCredentials.ClientCertificate.Certificate = certificado;

                InspectorBehavior inspector = new InspectorBehavior();
                svcRecepcaoEvento.Endpoint.EndpointBehaviors.Add(inspector);

                XmlNode dadosRetorno = svcRecepcaoEvento.cteRecepcaoEvento(ref dadosCabecalho, doc.DocumentElement);

                string xmlEnvio = inspector.LastRequestXML;
                string xmlRetorno = inspector.LastResponseXML;

                XmlSerializer ser = new XmlSerializer(typeof(Retorno.TRetEvento));

                using (TextReader reader = new StringReader(dadosRetorno.OuterXml))
                {
                    Retorno.TRetEvento result = (Retorno.TRetEvento)ser.Deserialize(reader);

                    if (result.infEvento.cStat == "135" || result.infEvento.cStat == "136" || result.infEvento.cStat == "631")
                    {
                        if (result.infEvento.cStat == "631")
                        {
                            string dataEvento = string.Empty;
                            string protocoloEvento = string.Empty;

                            string pattern = @"(?:\[nProt:([0-9]+)\])(?:\[dhRegEvento:(.{19})\])";  //(?:\[dhRegEvento:(.{19}))

                            MatchCollection matches = Regex.Matches(result.infEvento.xMotivo, pattern);

                            if (matches.Count > 0)
                            {
                                protocoloEvento = matches[0].Groups[1].Value;
                                dataEvento = matches[0].Groups[2].Value;
                            }

                            result.infEvento.cStat = "135";
                            result.infEvento.xMotivo = "Evento registrado e vinculado a CT-e";
                            result.infEvento.chCTe = chaveCTe;
                            result.infEvento.tpEvento = "610110";
                            result.infEvento.xEvento = "Prestação Serviço em Desacordo";
                            result.infEvento.nSeqEvento = "1";
                            result.infEvento.nProt = protocoloEvento;
                            result.infEvento.dhRegEvento = dataEvento;
                        }


                        //TextReader readerEvento = new StringReader(doc.OuterXml);                        

                        Envio.procEventoCTe procEventoCTe = new Envio.procEventoCTe()
                        {
                            eventoCTe = eventoCTe, //(Envio.TRetEvento)ser.Deserialize(readerEvento),// 
                            retEventoCTe = result,
                            versao = "3.00"
                        };

                        XmlSerializer xmlSerializerRetorno = new XmlSerializer(typeof(Envio.procEventoCTe));
                        XmlDocument docRetorno = new XmlDocument();
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            xmlSerializerRetorno.Serialize(memoryStream, procEventoCTe, namespaces);

                            memoryStream.Position = 0;

                            docRetorno.Load(memoryStream);
                        }
                        xmlEvento = docRetorno.OuterXml;
                    }

                    return result;
                }
            }
        }


        public class InspectorBehavior : IEndpointBehavior
        {
            public string LastRequestXML
            {
                get
                {
                    return myMessageInspector.LastRequestXML;
                }
            }

            public string LastResponseXML
            {
                get
                {
                    return myMessageInspector.LastResponseXML;
                }
            }


            private MyMessageInspector myMessageInspector = new MyMessageInspector();
            public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
            {

            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {

            }

            public void Validate(ServiceEndpoint endpoint)
            {

            }


            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
                clientRuntime.ClientMessageInspectors.Add(myMessageInspector);
            }
        }

        public class MyMessageInspector : IClientMessageInspector
        {
            public string LastRequestXML { get; private set; }
            public string LastResponseXML { get; private set; }
            public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
            {
                LastResponseXML = reply.ToString();
            }

            public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
            {
                LastRequestXML = request?.ToString() ?? "";
                return request;
            }
        }
    }
}
