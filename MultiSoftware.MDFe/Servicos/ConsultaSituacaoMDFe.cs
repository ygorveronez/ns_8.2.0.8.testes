using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;

namespace MultiSoftware.MDFe.Servicos
{
    public class ConsultaSituacaoMDFe
    {
        public static bool Consultar(out string xmlRetorno, out v300.ConsultaSituacaoMDFe.TRetConsSitMDFe retConsSitMDFe, out string erro, string chave, v300.TAmb tipoAmbiente, v300.TCodUfIBGE cUFEmissor, string caminhoCertificado, string senhaCertificado)
        {
            retConsSitMDFe = null;
            xmlRetorno = null;
            erro = null;

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            System.Security.Cryptography.X509Certificates.X509Certificate2 certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(caminhoCertificado, senhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);

            if (certificado.NotAfter < DateTime.Now || certificado.NotBefore > DateTime.Now)
            {
                erro = "Certificado expirado.";
                return false;
            }

            v300.ConsultaSituacaoMDFe.TConsSitMDFe consSitMDFe = new v300.ConsultaSituacaoMDFe.TConsSitMDFe()
            {
                chMDFe = chave,
                tpAmb = tipoAmbiente,
                versao = "3.00"
            };

            ServicoMDFeConsultaMDF.mdfeCabecMsg dadosCabecalho = new ServicoMDFeConsultaMDF.mdfeCabecMsg()
            {
                cUF = cUFEmissor.ToString("D"), //"91",
                versaoDados = "3.00"
            };

            using (ServicoMDFeConsultaMDF.MDFeConsultaSoap12Client svcMDFeConsultaMDF = new ServicoMDFeConsultaMDF.MDFeConsultaSoap12Client())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    XmlSerializer xmlSerializerEnvio = new XmlSerializer(typeof(v300.ConsultaSituacaoMDFe.TConsSitMDFe));

                    xmlSerializerEnvio.Serialize(memoryStream, consSitMDFe);

                    memoryStream.Position = 0;

                    XmlDocument doc = new XmlDocument();
                    doc.Load(memoryStream);

                    svcMDFeConsultaMDF.ClientCredentials.ClientCertificate.Certificate = certificado;

                    XmlNode dadosRetorno = svcMDFeConsultaMDF.mdfeConsultaMDF(ref dadosCabecalho, doc.DocumentElement);

                    using (TextReader reader = new StringReader(dadosRetorno.OuterXml))
                    {
                        XmlSerializer xmlSerializerRetorno = new XmlSerializer(typeof(v300.ConsultaSituacaoMDFe.TRetConsSitMDFe));

                        xmlRetorno = dadosRetorno.OuterXml;
                        retConsSitMDFe = (v300.ConsultaSituacaoMDFe.TRetConsSitMDFe)xmlSerializerRetorno.Deserialize(reader);

                        return true;
                    }
                }
            }
        }
    }
}
