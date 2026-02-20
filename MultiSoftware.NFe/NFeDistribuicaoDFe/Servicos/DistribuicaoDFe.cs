using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MultiSoftware.NFe.NFeDistribuicaoDFe.Servicos
{
    public class DistribuicaoDFe
    {

        public static DFe.retDistDFeInt ConsultarDocumentoPorChave(string chave, string cnpj, DFe.TCodUfIBGE cUFAutor, string caminhoCertificado, string senhaCertificado, int tipoAmbiente = 1)
        {            
            try
            {
                System.Security.Cryptography.X509Certificates.X509Certificate2 certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(caminhoCertificado, senhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);

                if (certificado.NotAfter < DateTime.Now || certificado.NotBefore > DateTime.Now)
                    throw new Exception("Certificado expirado.");

                dynamic item1 = null;
                item1 = new DFe.distDFeIntConsChNFe() { chNFe = chave };

                DFe.distDFeInt infoDFe = new DFe.distDFeInt()
                {
                    cUFAutor = cUFAutor,
                    Item = cnpj,
                    ItemElementName = cnpj.Length >= 14 ? DFe.ItemChoiceType.CNPJ : DFe.ItemChoiceType.CPF,
                    tpAmb = tipoAmbiente == 1 ? DFe.TAmb.Item1 : DFe.TAmb.Item2,
                    versao = DFe.TVerDistDFe.Item101,          
                    Item1 = item1
                };

                using (ServicoNFeDistribuicaoDFe.NFeDistribuicaoDFeSoapClient svcDistribuicaoDFe = new ServicoNFeDistribuicaoDFe.NFeDistribuicaoDFeSoapClient())
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (TextWriter streamWriter = new StreamWriter(memoryStream))
                        {
                            XmlSerializer xmlSerializerEnvio = new XmlSerializer(typeof(DFe.distDFeInt));
                            xmlSerializerEnvio.Serialize(streamWriter, infoDFe);

                            XElement dadosEnvio = XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));

                            svcDistribuicaoDFe.ClientCredentials.ClientCertificate.Certificate = certificado;

                            XElement dadosRetorno = svcDistribuicaoDFe.nfeDistDFeInteresse(dadosEnvio);

                            XmlSerializer xmlSerializerRetorno = new XmlSerializer(typeof(DFe.retDistDFeInt));
                            return (DFe.retDistDFeInt)xmlSerializerRetorno.Deserialize(dadosRetorno.CreateReader());
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao consultar status do documento destinado à empresa " + cnpj + ".", ex);
            }
        }

        /// <summary>
        /// Realiza consulta de documentos fiscais emitidos contra o CNPJ informado.
        /// </summary>
        /// <param name="cnpj">CNPJ do destinatário do documento fiscal.</param>
        /// <param name="ultNSU">Último número sequencial único consultado. Vai retornar a pesquisa a partir desse número.</param>
        /// <param name="cUFAutor">Código IBGE da UF do CNPJ informado.</param>
        /// <param name="caminhoCertificado">Caminho do certificado digital.</param>
        /// <param name="senhaCertificado">Senha do certificado digital.</param>
        /// <returns></returns>
        public static DFe.retDistDFeInt ConsultarDocumentosFiscais(string cnpj, long ultNSU, DFe.TCodUfIBGE cUFAutor, string caminhoCertificado, string senhaCertificado, bool consultaUnicoNSU = false, int tipoAmbiente = 1)
        {
            try
            {
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
                    cUFAutor = cUFAutor,
                    Item = cnpj,
                    ItemElementName = cnpj.Length >= 14 ? DFe.ItemChoiceType.CNPJ : DFe.ItemChoiceType.CPF,
                    tpAmb = tipoAmbiente == 1 ? DFe.TAmb.Item1 : DFe.TAmb.Item2,
                    versao = DFe.TVerDistDFe.Item100,
                    Item1 = item1
                };

                using (ServicoNFeDistribuicaoDFe.NFeDistribuicaoDFeSoapClient svcDistribuicaoDFe = new ServicoNFeDistribuicaoDFe.NFeDistribuicaoDFeSoapClient())
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (TextWriter streamWriter = new StreamWriter(memoryStream))
                        {
                            XmlSerializer xmlSerializerEnvio = new XmlSerializer(typeof(DFe.distDFeInt));
                            xmlSerializerEnvio.Serialize(streamWriter, infoDFe);

                            XElement dadosEnvio = XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));

                            svcDistribuicaoDFe.ClientCredentials.ClientCertificate.Certificate = certificado;

                            XElement dadosRetorno = svcDistribuicaoDFe.nfeDistDFeInteresse(dadosEnvio);

                            XmlSerializer xmlSerializerRetorno = new XmlSerializer(typeof(DFe.retDistDFeInt));
                            return (DFe.retDistDFeInt)xmlSerializerRetorno.Deserialize(dadosRetorno.CreateReader());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao consultar notas fiscais destinadas à empresa " + cnpj + ".", ex);
            }
        }
    }
}
