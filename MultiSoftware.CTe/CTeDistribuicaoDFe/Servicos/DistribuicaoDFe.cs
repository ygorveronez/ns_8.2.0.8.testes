using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MultiSoftware.CTe.CTeDistribuicaoDFe.Servicos
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
		public static DFe.retDistDFeInt ConsultarDocumentosFiscais(string cnpj, long ultNSU, DFe.TAmb tipoAmbiente, DFe.TCodUfIBGE cUFAutor, string caminhoCertificado, string senhaCertificado, string urlSefaz, ServicoCTeDistribuicaoDFe.CTeDistribuicaoDFeSoapClient svcDistribuicaoDFe, bool consultaUnicoNSU = false)
		{
			try
			{
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

				System.Security.Cryptography.X509Certificates.X509Certificate2 certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(caminhoCertificado, senhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);

				if (certificado == null || certificado.NotAfter < DateTime.Now || certificado.NotBefore > DateTime.Now)
					throw new Exception("Certificado expirado ou não configurado.");

				dynamic item1 = null;

				if (consultaUnicoNSU)
					item1 = new DFe.distDFeIntConsNSU() { NSU = string.Format("{0:000000000000000}", ultNSU) };
				else
					item1 = new DFe.distDFeIntDistNSU() { ultNSU = string.Format("{0:000000000000000}", ultNSU) };

				DFe.distDFeInt infoDFe = new DFe.distDFeInt()
				{
					cUFAutor = cUFAutor,
					Item = cnpj,
					ItemElementName = DFe.ItemChoiceType.CNPJ,
					tpAmb = tipoAmbiente,
					versao = DFe.TVerDistDFe.Item100,
					Item1 = item1
				};

				svcDistribuicaoDFe.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlSefaz);

				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (TextWriter streamWriter = new StreamWriter(memoryStream))
					{
						XmlSerializer xmlSerializerEnvio = new XmlSerializer(typeof(DFe.distDFeInt));
						xmlSerializerEnvio.Serialize(streamWriter, infoDFe);

						XElement dadosEnvio = XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));

						svcDistribuicaoDFe.ClientCredentials.ClientCertificate.Certificate = certificado;

						XElement dadosRetorno = svcDistribuicaoDFe.cteDistDFeInteresse(dadosEnvio);

						XmlSerializer xmlSerializerRetorno = new XmlSerializer(typeof(DFe.retDistDFeInt));
						return (DFe.retDistDFeInt)xmlSerializerRetorno.Deserialize(dadosRetorno.CreateReader());
					}
				}

			}
			catch (Exception ex)
			{
				throw new Exception("Falha ao consultar na sefaz os Ctes destinadas à empresa " + cnpj + ". ", ex);
			}
		}
	}
}
