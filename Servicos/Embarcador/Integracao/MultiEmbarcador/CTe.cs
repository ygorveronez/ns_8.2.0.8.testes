using System;
using System.IO;
using System.Linq;

namespace Servicos.Embarcador.Integracao.MultiEmbarcador
{
    public class CTe
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;
        
        public CTe(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void IntegrarCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao)
        {
            if (cargaCTeIntegracao.CargaCTe.CTe == null)
                return;

            Dominio.Entidades.XMLCTe xmlCTe = cargaCTeIntegracao.CargaCTe.CTe.XMLs?.Where(obj => obj.Tipo == Dominio.Enumeradores.TipoXMLCTe.Autorizacao)?.FirstOrDefault();

            if (xmlCTe == null)
                return;

            string nomeArquivo = Embarcador.CTe.CTe.ObterNomeArquivoDownloadCTe(cargaCTeIntegracao.CargaCTe.CTe, "xml");
            string mensagemErro = string.Empty;
            
            try
            {
                MemoryStream stream = Utilidades.String.ToStream(xmlCTe.XML);
                InspectorBehavior inspector = new InspectorBehavior();
                
                ServicoSGT.CTe.CTeClient servicoCTeCliente = ObterClientCTe(cargaCTeIntegracao.CargaCTe.CTe.TomadorPagador.GrupoPessoas.URLIntegracaoMultiEmbarcador, cargaCTeIntegracao.CargaCTe.CTe.TomadorPagador.GrupoPessoas.TokenIntegracaoMultiEmbarcador);
                servicoCTeCliente.Endpoint.EndpointBehaviors.Add(inspector);

                ServicoSGT.CTe.RetornoOfstring retorno = servicoCTeCliente.EnviarArquivoXMLCTe(stream);

                if (!retorno.Status)
                {
                    cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = retorno.Mensagem;
                }
                else
                {
                    cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaCTeIntegracao.ProblemaIntegracao = "Integrado com sucesso.";
                }
            }
            catch(Exception excecao)
            {
                Log.TratarErro(excecao);
                
                mensagemErro = "Falha genérica ao integrar o CT-e";

                cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                cargaCTeIntegracao.ProblemaIntegracao = mensagemErro;
                cargaCTeIntegracao.DataIntegracao = DateTime.Now;
                cargaCTeIntegracao.NumeroTentativas++;
            }
        }

        public static ServicoSGT.CTe.CTeClient ObterClientCTe(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "CTe.svc";

            ServicoSGT.CTe.CTeClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.CTe.CTeClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.CTe.CTeClient(binding, endpointAddress);
            }
            
            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }
    }
}
