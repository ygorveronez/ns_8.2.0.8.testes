using System.Linq;

namespace Servicos.Embarcador.Integracao.Avon
{
    public class IntegracaoMinutaAvon
    {
        public static Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon ConsultarNFes(long numeroMinuta, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho, bool ambienteHomologacao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon informacoes = new Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon(numeroMinuta);

            Repositorio.Embarcador.Configuracoes.IntegracaoAvon repConfiguracaoIntegracaoAvon = new Repositorio.Embarcador.Configuracoes.IntegracaoAvon(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon configuracaoIntegracaoAvon = repConfiguracaoIntegracaoAvon.BuscarPorEmpresa(empresa.Codigo);

            if (configuracaoIntegracaoAvon == null || string.IsNullOrWhiteSpace(configuracaoIntegracaoAvon.EnterpriseID) || string.IsNullOrWhiteSpace(configuracaoIntegracaoAvon.TokenProducao))
            {
                informacoes.Mensagem = $"A configuração de integração da empresa {empresa.Descricao} para a Avon é inválida.";

                return informacoes;
            }

            ServicoAvon.RequestSoapClient svcAvon = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoAvon.RequestSoapClient, ServicoAvon.RequestSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Avon_Request, out Servicos.Models.Integracao.InspectorBehavior inspector);

            //if (ambienteHomologacao)
            //    svcAvon.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://portalnfeqa.avon.com/Infinitri.WebService.ExchangeMessage/ExchangeMessage.asmx");
            //else
            svcAvon.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://portalnfe.avon.com/Infinitri.WebService.ExchangeMessage/ExchangeMessage.asmx");

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
            {
                CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                {
                    ProcessCode = "10004",
                    MessageType = "103",
                    ExchangePattern = "1",
                    EnterpriseId = configuracaoIntegracaoAvon.EnterpriseID,
                    Token = /*ambienteHomologacao ? integracao.TokenAvonHomologacao :*/ configuracaoIntegracaoAvon.TokenProducao,
                },
                CrossTalk_Body = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Body()
                {
                    GroupId = "ManifestSearchDocument",
                    Identification = new Dominio.ObjetosDeValor.CrossTalk.Field[]
                    {
                        new Dominio.ObjetosDeValor.CrossTalk.Field() { Name="docNumber", Value = numeroMinuta.ToString() }
                    }
                }
            };

            string request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

            string response = svcAvon.Send(request, "");

            var retorno = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);

            if (retorno.CrossTalk_Header != null && retorno.CrossTalk_Header.ResponseCode == "200" && retorno.CrossTalk_Body != null && retorno.CrossTalk_Body.Documents != null)
            {
                informacoes.Mensagem = retorno.CrossTalk_Header.ResponseCodeMessage;
                informacoes.CodigoMensagem = retorno.CrossTalk_Header.ResponseCode;
                informacoes.GUID = retorno.CrossTalk_Header.GUID;

                informacoes.Documentos = (from obj in retorno.CrossTalk_Body.Documents select new Dominio.ObjetosDeValor.CrossTalk.NotaFiscalEletronica(obj)).ToList();
            }
            else if (retorno.CrossTalk_Body != null && (retorno.CrossTalk_Body.Response != null || !string.IsNullOrWhiteSpace(retorno.CrossTalk_Body.Message)))
            {
                informacoes.Mensagem = retorno.CrossTalk_Body.Response?.Message ?? retorno.CrossTalk_Body.Message;
                informacoes.CodigoMensagem = retorno.CrossTalk_Header.ResponseCode;
                informacoes.GUID = retorno.CrossTalk_Header.GUID;
            }
            else if (retorno.CrossTalk_Header != null)
            {
                informacoes.Mensagem = retorno.CrossTalk_Header.ResponseCodeMessage;
                informacoes.CodigoMensagem = retorno.CrossTalk_Header.ResponseCode;
                informacoes.GUID = retorno.CrossTalk_Header.GUID;
            }
            else
            {
                informacoes.Mensagem = "Não foram retornadas informações sobre erros pela Avon.";
            };

            informacoes.Requisicao = inspector.LastRequestXML;
            informacoes.Resposta = inspector.LastResponseXML;

            return informacoes;
        }
    }
}
