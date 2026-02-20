using System;
using System.Net;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Servicos.Ravex.IntegraCTE;

namespace Servicos.Embarcador.Integracao.Ravex
{
    public class Carga
    {
        public static void IntegrarCTe(ref Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Configuracoes.IntegracaoRavex repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoRavex(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponenteFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRavex configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracao || string.IsNullOrWhiteSpace(configuracaoIntegracao.UrlIntegracao))
            {
                cargaCTeIntegracao.ProblemaIntegracao = "Configuração para integração com o Ravex inválida.";
                cargaCTeIntegracao.DataIntegracao = DateTime.Now;
                cargaCTeIntegracao.NumeroTentativas++;
                cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                return;
            }
            string mensagemErro = string.Empty;
            try
            {
                if (cargaCTeIntegracao == null || cargaCTeIntegracao.CargaCTe == null || cargaCTeIntegracao.CargaCTe.CTe == null)
                {
                    cargaCTeIntegracao.ProblemaIntegracao = "Não foi localizado nenhum CT-e pendente de integração nesta carga.";
                    cargaCTeIntegracao.DataIntegracao = DateTime.Now;
                    cargaCTeIntegracao.NumeroTentativas++;
                    cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    return;
                }

                string urlWebService = configuracaoIntegracao.UrlIntegracao;

                Servicos.Ravex.IntegraCTE.WebServiceCteSoapClient svcRavex = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<Servicos.Ravex.IntegraCTE.WebServiceCteSoapClient, Servicos.Ravex.IntegraCTE.WebServiceCteSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Ravex_WebServiceCte, out Servicos.Models.Integracao.InspectorBehavior inspector);
                
                svcRavex.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlWebService);
                
                svcRavex.ClientCredentials.UserName.UserName = configuracaoIntegracao.Usuario;
                svcRavex.ClientCredentials.UserName.Password = configuracaoIntegracao.Senha;
                int qtdNotas = cargaCTeIntegracao.CargaCTe.CTe.XMLNotaFiscais != null && cargaCTeIntegracao.CargaCTe.CTe.XMLNotaFiscais.Count > 0 ? cargaCTeIntegracao.CargaCTe.CTe.XMLNotaFiscais.Count : 0;

                var cteIntegrar = new Cte()
                {
                    ChaveAcesso = cargaCTeIntegracao.CargaCTe.CTe.ChaveAcesso,
                    CnpjTransportadora = cargaCTeIntegracao.CargaCTe.CTe.Empresa?.CNPJ_SemFormato ?? "",
                    CpfMotorista = cargaCTeIntegracao.CargaCTe.Carga.CPFPrimeiroMotorista,
                    DataEmissao = cargaCTeIntegracao.CargaCTe.CTe.DataEmissao.HasValue ? cargaCTeIntegracao.CargaCTe.CTe.DataEmissao.Value : DateTime.Now,
                    IdentificadorViagem = cargaCTeIntegracao.CargaCTe.Carga.CodigoCargaEmbarcador,
                    Motorista = cargaCTeIntegracao.CargaCTe.Carga.NomePrimeiroMotorista,                    
                    Placa = cargaCTeIntegracao.CargaCTe.Carga.Veiculo?.Placa ?? "",
                    StatusCte = "Aprovado",
                    Transportadora = cargaCTeIntegracao.CargaCTe.CTe.Empresa?.RazaoSocial ?? "",
                    ValorCte = cargaCTeIntegracao.CargaCTe.CTe.ValorAReceber,
                    NotasFiscais = new Servicos.Ravex.IntegraCTE.NotaFiscal[qtdNotas]
                };
                int i = 0;
                foreach (var notaFiscal in cargaCTeIntegracao.CargaCTe.CTe.XMLNotaFiscais)
                {
                    Servicos.Ravex.IntegraCTE.NotaFiscal nf = new Servicos.Ravex.IntegraCTE.NotaFiscal()
                    {
                        ChaveAcessoNFe = notaFiscal.Chave,
                        Cliente = notaFiscal.Destinatario?.Nome ?? "",
                        CnpjCliente = notaFiscal.Destinatario?.CPF_CNPJ_SemFormato,
                        Numero = notaFiscal.Numero.ToString(),
                        Valor = notaFiscal.Valor
                    };
                    cteIntegrar.NotasFiscais[i] = nf;
                    i++;
                }

                try
                {
                    Servicos.Ravex.IntegraCTE.Retorno retorno = svcRavex.ImportarCte(configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha, cteIntegrar);

                    if (retorno.Id.HasValue && retorno.Id.Value < 0)
                    {
                        cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        mensagemErro = "Retorno " + (retorno.Id.HasValue ? retorno.Id.Value.ToString() : "") + " - " + retorno.Mensagem;
                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        arquivoIntegracao.Data = cargaCTeIntegracao.DataIntegracao;
                        arquivoIntegracao.Mensagem = "Retorno " + (retorno.Id.HasValue ? retorno.Id.Value.ToString() : "") + " - " + retorno.Mensagem;
                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unidadeDeTrabalho);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unidadeDeTrabalho);
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        cargaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                    }
                    else
                    {
                        cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        mensagemErro = "Retorno " + (retorno.Id.HasValue ? retorno.Id.Value.ToString() : "") + " - " + retorno.Mensagem;
                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        arquivoIntegracao.Data = cargaCTeIntegracao.DataIntegracao;
                        arquivoIntegracao.Mensagem = "Retorno " + (retorno.Id.HasValue ? retorno.Id.Value.ToString() : "") + " - " + retorno.Mensagem;
                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unidadeDeTrabalho);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unidadeDeTrabalho);
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        cargaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                    }
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao, "IntegrarCTeRavax");
                    mensagemErro = "Ocorreu uma falha ao comunicar com o Web Service da Revax.";
                    cargaCTeIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da Revax.";
                    cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCTeIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unidadeDeTrabalho);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unidadeDeTrabalho);
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemErro = "Falha ao enviar escrituração o CT-e " + cargaCTeIntegracao.CargaCTe.CTe.Numero + " para Ravex";
                cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            cargaCTeIntegracao.ProblemaIntegracao = mensagemErro;
            cargaCTeIntegracao.DataIntegracao = DateTime.Now;
            cargaCTeIntegracao.NumeroTentativas++;
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
                LastRequestXML = request.ToString();
                return request;
            }
        }
    }
}
