using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Serialization;

namespace MultiSoftware.NFe.NFeDownloadNF.Servicos
{
    public class DownloadNFe
    {
        public static DownloadNF.TRetDownloadNFe RealizarDownload(string cnpj, DownloadNF.TAmb tpAmb, string chaveNFe, string caminhoCertificado, string senhaCertificado)
        {
            X509Certificate2 certificado = new X509Certificate2(caminhoCertificado, senhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);

            DownloadNF.TDownloadNFe dadosEnvio = new DownloadNF.TDownloadNFe()
            {
                chNFe = new[] { chaveNFe },
                CNPJ = cnpj,
                tpAmb = tpAmb,
                versao = DownloadNF.TVerDownloadNFe.Item100,
                xServ = DownloadNF.TDownloadNFeXServ.DOWNLOADNFE
            };

            ServicoNFeDownloadNF.nfeCabecMsg dadosCabecalho = new ServicoNFeDownloadNF.nfeCabecMsg()
            {
                cUF = "91",
                versaoDados = "1.00"
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(DownloadNF.TDownloadNFe));
            XmlDocument doc = new XmlDocument();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, dadosEnvio);

                memoryStream.Position = 0;

                doc.Load(memoryStream);
            }
            
            using (MultiSoftware.NFe.ServicoNFeDownloadNF.NfeDownloadNFSoapClient svcDownloadNFe = new MultiSoftware.NFe.ServicoNFeDownloadNF.NfeDownloadNFSoapClient())
            {
                svcDownloadNFe.ClientCredentials.ClientCertificate.Certificate = certificado;

                XmlNode dadosRetorno = svcDownloadNFe.nfeDownloadNF(dadosCabecalho, doc.DocumentElement);

                XmlSerializer ser = new XmlSerializer(typeof(DownloadNF.TRetDownloadNFe));

                using (TextReader reader = new StringReader(dadosRetorno.OuterXml))
                {
                    DownloadNF.TRetDownloadNFe result = (DownloadNF.TRetDownloadNFe)ser.Deserialize(reader);

                    return result;
                }
            }
        }
    }
}
