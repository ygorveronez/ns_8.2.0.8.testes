using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Avon
{
    public class IntegracaoFaturaAvon
    {
        public static void EnviarFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracaoFatura, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo repFaturaIntegracaoArquivo = new Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaIntegracaoCTeRemover repFaturaIntegracaoCTeRemover = new Repositorio.Embarcador.Fatura.FaturaIntegracaoCTeRemover(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.IntegracaoAvon repConfiguracaoIntegracaoAvon = new Repositorio.Embarcador.Configuracoes.IntegracaoAvon(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon configuracaoIntegracaoAvon = repConfiguracaoIntegracaoAvon.BuscarPorEmpresa(integracaoFatura.Fatura.Empresa.Codigo);

            if (configuracaoIntegracaoAvon == null || string.IsNullOrWhiteSpace(configuracaoIntegracaoAvon.EnterpriseID) || string.IsNullOrWhiteSpace(configuracaoIntegracaoAvon.TokenProducao))
            {
                integracaoFatura.MensagemRetorno = $"A configuração de integração da empresa {integracaoFatura.Fatura.Empresa.Descricao} para a Avon é inválida.";
                integracaoFatura.Tentativas += 1;
                integracaoFatura.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repFaturaIntegracao.Atualizar(integracaoFatura);

                return;
            }

            string endpoint, token;

            ServicoAvon.RequestSoapClient svcAvon = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<ServicoAvon.RequestSoapClient, ServicoAvon.RequestSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Avon_Request, out Servicos.Models.Integracao.InspectorBehavior inspector);

            Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao;

            if (!integracaoFatura.Fatura.NovoModelo)
                tipoAmbiente = repFaturaCargaDocumento.BuscarTipoAmbienteCTeFatura(integracaoFatura.Fatura.Codigo);
            else
                tipoAmbiente = repFaturaDocumento.BuscarTipoAmbienteFatura(integracaoFatura.Fatura.Codigo);

            if (tipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
            {
                token = configuracaoIntegracaoAvon.TokenProducao;
                endpoint = "http://portalnfe.avon.com/Infinitri.WebService.ExchangeMessage/ExchangeMessage.asmx";
            }
            else
            {
                token = configuracaoIntegracaoAvon.TokenHomologacao;
                endpoint = "http://portalnfeqa.avon.com/Infinitri.WebService.ExchangeMessage/ExchangeMessage.asmx";
            }

            svcAvon.Endpoint.Address = new System.ServiceModel.EndpointAddress(endpoint);

            List<string> chaveCTesAutorizados = null;

            if (integracaoFatura.Fatura.NovoModelo)
            {
                chaveCTesAutorizados = repFaturaDocumento.BuscarChaveCTesAutorizadosPorCarga(integracaoFatura.Fatura.Codigo);
                chaveCTesAutorizados.AddRange(repFaturaDocumento.BuscarChaveCTesAutorizadosPorCTe(integracaoFatura.Fatura.Codigo));
            }
            else
            {
                chaveCTesAutorizados = repFaturaCargaDocumento.BuscarChavesCTesAutorizadosFatura(integracaoFatura.Fatura.Codigo);
            }

            List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCTeRemover> ctesRemover = repFaturaIntegracaoCTeRemover.BuscarPorFatura(integracaoFatura.Fatura.Codigo);

            foreach (Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCTeRemover cteRemover in ctesRemover)
                chaveCTesAutorizados.Remove(cteRemover.CTe.Chave);

            Dominio.ObjetosDeValor.CrossTalk.Fatura arquivoFatura = new Dominio.ObjetosDeValor.CrossTalk.Fatura();

            arquivoFatura.CNPJAvon = integracaoFatura.Fatura.ClienteTomadorFatura.CPF_CNPJ_SemFormato; //se não funcionar usar esse -> repFaturaCargaDocumento.BuscarCNPJRemetenteCTeFatura(integracaoFatura.Fatura.Codigo);
            arquivoFatura.CNPJTransportador = integracaoFatura.Fatura.Empresa.CNPJ;
            arquivoFatura.DataEmissao = integracaoFatura.Fatura.DataFatura;
            arquivoFatura.DataVencimento = integracaoFatura.Fatura.Parcelas.OrderBy(o => o.DataVencimento).Select(o => o.DataVencimento).FirstOrDefault();
            arquivoFatura.NumeroFatura = integracaoFatura.Fatura.Numero.ToString();
            arquivoFatura.SerieFatura = "";
            arquivoFatura.ValorTotal = integracaoFatura.Fatura.Total + integracaoFatura.Fatura.Acrescimos - integracaoFatura.Fatura.Descontos;
            arquivoFatura.CTes = chaveCTesAutorizados;

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
            {
                CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                {
                    ProcessCode = "10006",
                    MessageType = "100",
                    ExchangePattern = "7",
                    EnterpriseId = configuracaoIntegracaoAvon.EnterpriseID,
                    Token = token
                }
            };

            string request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

            string response = svcAvon.Send(request, arquivoFatura.ObterArquivo());

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message retorno = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);

            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo();

            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unidadeTrabalho);
            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unidadeTrabalho);
            arquivoIntegracao.Data = DateTime.Now;
            arquivoIntegracao.Mensagem = retorno.CrossTalk_Header.ResponseCode + " - " + retorno.CrossTalk_Header.ResponseCodeMessage;
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            repFaturaIntegracaoArquivo.Inserir(arquivoIntegracao);

            integracaoFatura.MensagemRetorno = retorno.CrossTalk_Header.ResponseCode + " - " + retorno.CrossTalk_Header.ResponseCodeMessage;
            integracaoFatura.ArquivosIntegracao.Add(arquivoIntegracao);
            integracaoFatura.Tentativas += 1;
            integracaoFatura.DataEnvio = DateTime.Now;

            if (retorno.CrossTalk_Header != null && retorno.CrossTalk_Header.ResponseCode == "202")
            {
                integracaoFatura.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                integracaoFatura.CodigoIntegracaoIntegradora = retorno.CrossTalk_Header.GUID;
            }
            else
            {
                integracaoFatura.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            repFaturaIntegracao.Atualizar(integracaoFatura);
        }

        public static void ConsultarRetornoFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracaoFatura, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo repFaturaIntegracaoArquivo = new Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.IntegracaoAvon repConfiguracaoIntegracaoAvon = new Repositorio.Embarcador.Configuracoes.IntegracaoAvon(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon configuracaoIntegracaoAvon = repConfiguracaoIntegracaoAvon.BuscarPorEmpresa(integracaoFatura.Fatura.Empresa.Codigo);

            if (configuracaoIntegracaoAvon == null || string.IsNullOrWhiteSpace(configuracaoIntegracaoAvon.EnterpriseID) || string.IsNullOrWhiteSpace(configuracaoIntegracaoAvon.TokenProducao))
            {
                integracaoFatura.MensagemRetorno = $"A configuração de integração da empresa {integracaoFatura.Empresa.Descricao} para a Avon é inválida.";
                integracaoFatura.Tentativas += 1;
                integracaoFatura.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repFaturaIntegracao.Atualizar(integracaoFatura);

                return;
            }

            string endpoint, token;

            ServicoAvon.RequestSoapClient svcAvon = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<ServicoAvon.RequestSoapClient, ServicoAvon.RequestSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Avon_Request, out Servicos.Models.Integracao.InspectorBehavior inspector);

            Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao;

            if (!integracaoFatura.Fatura.NovoModelo)
                tipoAmbiente = repFaturaCargaDocumento.BuscarTipoAmbienteCTeFatura(integracaoFatura.Fatura.Codigo);
            else
                tipoAmbiente = repFaturaDocumento.BuscarTipoAmbienteFatura(integracaoFatura.Fatura.Codigo);

            if (tipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
            {
                token = configuracaoIntegracaoAvon.TokenProducao;
                endpoint = "http://portalnfe.avon.com/Infinitri.WebService.ExchangeMessage/ExchangeMessage.asmx";
            }
            else
            {
                token = configuracaoIntegracaoAvon.TokenHomologacao;
                endpoint = "http://portalnfeqa.avon.com/Infinitri.WebService.ExchangeMessage/ExchangeMessage.asmx";
            }

            svcAvon.Endpoint.Address = new System.ServiceModel.EndpointAddress(endpoint);

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
            {
                CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                {
                    ProcessCode = "10006",
                    MessageType = "100",
                    ExchangePattern = "8",
                    EnterpriseId = configuracaoIntegracaoAvon.EnterpriseID,
                    Token = token,
                    GUID = integracaoFatura.CodigoIntegracaoIntegradora
                }
            };

            string request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

            string response = svcAvon.Send(request, "");

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message retorno = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            string mensagemRetorno = string.Empty;

            if (retorno != null && retorno.CrossTalk_Header != null && retorno.CrossTalk_Header.ResponseCode == "200")
            {
                if (retorno.CrossTalk_Body != null && retorno.CrossTalk_Body.Root != null && retorno.CrossTalk_Body.Root.Document != null && retorno.CrossTalk_Body.Root.Document.Response != null)
                {
                    mensagemRetorno = retorno.CrossTalk_Body.Root.Document.Response.InnerCode + " - " + retorno.CrossTalk_Body.Root.Document.Response.Description;

                    if (retorno.CrossTalk_Body.Root.Document.Response.ErrorMessages != null)
                        foreach (string error in retorno.CrossTalk_Body.Root.Document.Response.ErrorMessages)
                            mensagemRetorno += " - " + error;

                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
                else if (retorno.CrossTalk_Body != null && retorno.CrossTalk_Body.Document != null)
                {
                    mensagemRetorno = retorno.CrossTalk_Body.Document[0].Response.InnerCode + " - " + retorno.CrossTalk_Body.Document[0].Response.Description;
                }
                else
                {
                    mensagemRetorno = retorno.CrossTalk_Header.ResponseCode + " - " + retorno.CrossTalk_Header.ResponseCodeMessage;
                }
            }
            else if (retorno != null && retorno.CrossTalk_Header != null && retorno.CrossTalk_Header.ResponseCode == "202")
            {
                situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                mensagemRetorno = retorno.CrossTalk_Header.ResponseCode + " - " + retorno.CrossTalk_Header.ResponseCodeMessage;
            }
            else if (retorno != null && retorno.CrossTalk_Header != null)
            {
                situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = retorno.CrossTalk_Header.ResponseCode + " - " + retorno.CrossTalk_Header.ResponseCodeMessage;
            }

            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unidadeTrabalho),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unidadeTrabalho),
                Data = DateTime.Now,
                Mensagem = mensagemRetorno,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento
            };

            repFaturaIntegracaoArquivo.Inserir(arquivoIntegracao);

            integracaoFatura.SituacaoIntegracao = situacao;
            integracaoFatura.MensagemRetorno = mensagemRetorno;
            integracaoFatura.ArquivosIntegracao.Add(arquivoIntegracao);

            repFaturaIntegracao.Atualizar(integracaoFatura);
        }

        public Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message EnviarFatura(string enterpriseId, string token, Dominio.Entidades.FaturaAvon fatura, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
#if DEBUG
            enterpriseId = "82809088000666";
            token = "A2ED7133603A40A4BC6C15E909837604";
#endif

            Repositorio.DocumentoManifestoAvon repDocumento = new Repositorio.DocumentoManifestoAvon(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.CrossTalk.Fatura arquivoFatura = new Dominio.ObjetosDeValor.CrossTalk.Fatura
            {
                CNPJAvon = repDocumento.ObterCNPJRemetente(fatura.Manifestos.First().Codigo),
                CNPJTransportador = fatura.Empresa.CNPJ,
                DataEmissao = fatura.DataEmissao,
                DataVencimento = fatura.DataVencimento,
                NumeroFatura = fatura.Numero.ToString(),
                SerieFatura = fatura.Serie.ToString(),
                ValorTotal = fatura.ValorTotal,
                CTes = repDocumento.ObterChaveDosCTes(fatura.Manifestos.Select(o => o.Codigo).ToArray())
            };

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
            {
                CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                {
                    ProcessCode = "10006",
                    MessageType = "100",
                    ExchangePattern = "7",
                    EnterpriseId = enterpriseId,
                    Token = token
                }
            };

            string request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

            //Servicos.Log.TratarErro("Request 1 - fatura " + fatura.Numero.ToString() + ": " + request);

            ServicoAvon.RequestSoapClient svcAvon = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoAvon.RequestSoapClient, ServicoAvon.RequestSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Avon_Request);

            string response = svcAvon.Send(request, arquivoFatura.ObterArquivo());

            //Servicos.Log.TratarErro("Response 1 - fatura " + fatura.Numero.ToString() + ": " + response + " Arquivo Fatura: " + arquivoFatura.ObterArquivo());

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message retorno = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);

            if (retorno.CrossTalk_Header != null && retorno.CrossTalk_Header.ResponseCode == "202")
            {
                int countConsultas = 0;

                while (retorno.CrossTalk_Header.ResponseCode != "200" && countConsultas < 20)
                {
                    System.Threading.Thread.Sleep(2000);

                    countConsultas++;

                    mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
                    {
                        CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                        {
                            ProcessCode = "10006",
                            MessageType = "100",
                            ExchangePattern = "8",
                            EnterpriseId = enterpriseId,
                            Token = token,
                            GUID = retorno.CrossTalk_Header.GUID
                        }
                    };

                    request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

                    response = svcAvon.Send(request, "");

                    //Servicos.Log.TratarErro("Response 2 (pode haver vários) - fatura " + fatura.Numero.ToString() + ": " + response);

                    retorno = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);
                }
            }

            return retorno;
        }
    }
}
