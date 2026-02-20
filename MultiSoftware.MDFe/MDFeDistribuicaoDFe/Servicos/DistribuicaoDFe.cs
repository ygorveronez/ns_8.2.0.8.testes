using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;

namespace MultiSoftware.MDFe.MDFeDistribuicaoDFe.Servicos
{
    public class DistribuicaoDFe
    {
        /// <summary>
        /// Realiza consulta de documentos fiscais emitidos contra o CNPJ informado.
        /// </summary>
        /// <param name="cnpj">CNPJ do destinatário do documento fiscal.</param>
        /// <param name="ultNSU">Último número sequencial único consultado. Vai retornar a pesquisa a partir desse número.</param>
        /// <param name="cUFAutor">Código IBGE da UF do CNPJ informado.</param>
        /// <param name="caminhoCertificado">Caminho do certificado digital.</param>
        /// <param name="senhaCertificado">Senha do certificado digital.</param>
        /// <returns></returns>
        public static DFe.retDistDFeInt ConsultarDocumentosFiscais(string cnpj, long ultNSU, DFe.TAmb tipoAmbiente, DFe.TCodUfIBGE cUFAutor, string caminhoCertificado, string senhaCertificado, string urlSefaz, bool consultaUnicoNSU = false)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                System.Security.Cryptography.X509Certificates.X509Certificate2 certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(caminhoCertificado, senhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);

                if (certificado.NotAfter < DateTime.Now || certificado.NotBefore > DateTime.Now)
                    throw new Exception("Certificado expirado.");

                dynamic item1 = null;

                if (consultaUnicoNSU)
                    item1 = new DFe.distDFeIntConsNSU() { NSU = string.Format("{0:000000000000000}", ultNSU) };
                else
                    item1 = new DFe.distDFeIntDistNSU() { ultNSU = string.Format("{0:000000000000000}", ultNSU) };

                DFe.distDFeInt infoDFe = new DFe.distDFeInt()
                {                    
                    Item = cnpj,
                    ItemElementName = DFe.ItemChoiceType.CNPJ,
                    tpAmb = tipoAmbiente,
                    versao = DFe.TVerDistDFe.Item100,
                    Item1 = item1                    
                };

                ServicoMDFeDistribuicaoDFe.mdfeCabecMsg mdfeCabecMsg = new ServicoMDFeDistribuicaoDFe.mdfeCabecMsg()
                {
                    cUF = cUFAutor.ToString("D"),
                    versaoDados = "1.00"
                };

                using (ServicoMDFeDistribuicaoDFe.MDFeDistribuicaoDFeSoap12Client svcDistribuicaoDFe = new ServicoMDFeDistribuicaoDFe.MDFeDistribuicaoDFeSoap12Client())
                {
                    svcDistribuicaoDFe.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlSefaz);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        XmlSerializer xmlSerializerEnvio = new XmlSerializer(typeof(DFe.distDFeInt));
                        
                        xmlSerializerEnvio.Serialize(memoryStream, infoDFe);

                        memoryStream.Position = 0;

                        XmlDocument doc = new XmlDocument();
                        doc.Load(memoryStream);

                        svcDistribuicaoDFe.ClientCredentials.ClientCertificate.Certificate = certificado;

                        XmlNode dadosRetorno = svcDistribuicaoDFe.mdfeDistDFeInteresse(ref mdfeCabecMsg, doc.DocumentElement);

                        XmlSerializer xmlSerializerRetorno = new XmlSerializer(typeof(DFe.retDistDFeInt));
                        using (TextReader reader = new StringReader(dadosRetorno.OuterXml))
                        {
                            DFe.retDistDFeInt result = (DFe.retDistDFeInt)xmlSerializerRetorno.Deserialize(reader);

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao consultar MDFe destinadas à empresa " + cnpj + ".", ex);
            }
        }
    }
}
