using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Servicos.Embarcador.Integracao.Nestle
{
    public class IntegracaoNestle
    {
        public static bool IntegrarCarga(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork, out string msgRetorno)
        {
            msgRetorno = "";
            try
            {
                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                string xmlCTe = svcCTe.ObterStringXMLAutorizacao(cte, unitOfWork);
                if (string.IsNullOrEmpty(xmlCTe))
                {
                    Servicos.Log.TratarErro("CT-e Selecionado sem XML de autorização", "IntegracaoNestle");
                    msgRetorno = "CT-e Selecionado sem XML de autorização";
                    return false;
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string endPoint = "https://e2126-iflmap.hcisbt.eu1.hana.ondemand.com/http/testing_idoc";                
                string clientResponseContent = "";

                HttpWebRequest client = (HttpWebRequest)WebRequest.Create(endPoint);
                client.Method = System.Net.WebRequestMethods.Http.Post;
                client.ProtocolVersion = HttpVersion.Version10;
                client.ContentType = "text/xml";
                client.KeepAlive = false;
                
                StreamWriter dataWriter = null;
               
                try
                {
                    dataWriter = new StreamWriter(client.GetRequestStream());
                    dataWriter.Write(xmlCTe);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e, "IntegracaoNestle");
                    Servicos.Log.TratarErro("Erro ao preparar dados de requisição", "IntegracaoNestle");
                    msgRetorno = "Problemas ao preparar os dados para a requisição.";
                    return false;
                }
                finally
                {
                    dataWriter.Close();
                }


                var certificado = new X509Certificate2(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosEmpresas + @"\13969629000196\Certificado\13969629000196.pfx", "multi");
                client.ClientCertificates.Add(certificado);
                client.PreAuthenticate = true;

                HttpWebResponse objResponse = (HttpWebResponse)client.GetResponse();
                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                {
                    clientResponseContent = sr.ReadToEnd();
                    sr.Close();
                }

                //Verificar com volta o erro
                if (!string.IsNullOrWhiteSpace(clientResponseContent))
                {
                    msgRetorno = "Inegração não realizada.";
                    Servicos.Log.TratarErro("Falha ao integrar", "IntegracaoNestle");
                    return false;
                }
                else
                    return false;

            }
            catch (Exception e)
            {
                msgRetorno = "Falha ao realizar a integração com a Nestle.";
                Servicos.Log.TratarErro(e, "IntegracaoNestle");
                return false;
            }
        }

    }
}
