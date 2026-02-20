using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DocumentosEmissao
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class ConsultaDocumentosReceitaController : BaseController
    {
		#region Construtores

		public ConsultaDocumentosReceitaController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ConsultarReceita()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(unitOfWork);
                string chave = Request.Params("Chave").Replace(" ", "").Replace(".", "");

                if (serDocumento.ValidarChave(chave))
                {
                    string tipoDocumento = chave.Substring(20, 2);
                    if (tipoDocumento == "55")//nfe
                    {
                        Dominio.Entidades.WebServicesConsultaNFe webService = Servicos.Embarcador.Documentos.ConsultaReceita.ObterWebService(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceConsultaReceita.NFe, unitOfWork);

                        ConsultaNFe.ConsultaNFeClient consultaNFe = new ConsultaNFe.ConsultaNFeClient();

                        if (webService != null)
                            consultaNFe.Endpoint.Address = new EndpointAddress(webService.WebService);

                        OperationContextScope scope = new OperationContextScope(consultaNFe.InnerChannel);
                        MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "4ed60154d2f04201ab8b57ed4198da32");
                        OperationContext.Current.OutgoingMessageHeaders.Add(header);

                        ConsultaNFe.RetornoOfRequisicaoSefazgj5B5PAD requisicaoSefaz = consultaNFe.SolicitarRequisicaoSefaz();

                        Servicos.Embarcador.Documentos.ConsultaReceita.AjustarConsulta(webService, requisicaoSefaz.Status, unitOfWork);

                        if (requisicaoSefaz.Status)
                        {
                            var retorno = new
                            {
                                NotaAdicionada = false, //todo: quando incluir o quebra captcha já executar toda a consulta aqui (ver regra quebra de captcha)
                                TipoConsultaPortalFazenda = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaPortalFazenda.NFe,
                                DadosConsultar = new
                                {
                                    VIEWSTATE = requisicaoSefaz.Objeto.VIEWSTATE,
                                    EVENTVALIDATION = requisicaoSefaz.Objeto.EVENTVALIDATION,
                                    imgCaptcha = requisicaoSefaz.Objeto.Captcha,
                                    token = requisicaoSefaz.Objeto.TokenCaptcha,
                                    SessionID = requisicaoSefaz.Objeto.SessionID
                                }
                            };
                            
                            return new JsonpResult(retorno);
                        }
                        else
                        {
                            return new JsonpResult(false, true, requisicaoSefaz.Mensagem);
                        }
                    }
                    else
                    {
                        if (tipoDocumento == "57")//cte
                        {
                            Dominio.Entidades.WebServicesConsultaNFe webService = Servicos.Embarcador.Documentos.ConsultaReceita.ObterWebService(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceConsultaReceita.CTe, unitOfWork);

                            ConsultaCTe.CosultaCTeClient consultaCTe = new ConsultaCTe.CosultaCTeClient();

                            if (webService != null)
                                consultaCTe.Endpoint.Address = new EndpointAddress(webService.WebService);

                            OperationContextScope scope = new OperationContextScope(consultaCTe.InnerChannel);
                            MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "4ed60154d2f04201ab8b57ed4198da32");
                            OperationContext.Current.OutgoingMessageHeaders.Add(header);

                            ConsultaCTe.RetornoOfRequisicaoSefazpIzbOyUQ requisicaoSefaz = consultaCTe.SolicitarRequisicaoSefaz();

                            Servicos.Embarcador.Documentos.ConsultaReceita.AjustarConsulta(webService, requisicaoSefaz.Status, unitOfWork);

                            if (requisicaoSefaz.Status)
                            {
                                var retorno = new
                                {
                                    NotaAdicionada = false, //todo: quando incluir o quebra captcha já executar toda a consulta aqui (ver regra quebra de captcha)
                                    TipoConsultaPortalFazenda = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaPortalFazenda.CTe,
                                    DadosConsultar = new
                                    {
                                        VIEWSTATE = requisicaoSefaz.Objeto.VIEWSTATE,
                                        EVENTVALIDATION = requisicaoSefaz.Objeto.EVENTVALIDATION,
                                        imgCaptcha = requisicaoSefaz.Objeto.Captcha,
                                        token = requisicaoSefaz.Objeto.TokenCaptcha,
                                        SessionID = requisicaoSefaz.Objeto.SessionID
                                    }
                                };

                                return new JsonpResult(retorno);
                            }
                            else
                            {
                                return new JsonpResult(false, true, requisicaoSefaz.Mensagem);
                            }
                        }
                        else
                        {
                            return new JsonpResult(false, true, "A chave informada para o documento não é valida, por favor, verifique.");
                        }
                    }
                }
                else
                {
                    return new JsonpResult(false, true, "A chave informada é inválida, por favor, verifique e tente novamente.");
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar a NF-e.");
            }
        }

        public async Task<IActionResult> InformarCaptchaReceita()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(int.Parse(Request.Params("CargaPedido")));

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacao = cargaPedido.TipoContratacaoCarga;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaPortalFazenda tipoConsultaPortalFazenda = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaPortalFazenda)int.Parse(Request.Params("TipoConsultaPortalFazenda"));

                string VIEWSTATE = Request.Params("VIEWSTATE");
                string EVENTVALIDATION = Request.Params("EVENTVALIDATION");
                string TokenCaptcha = Request.Params("token");
                string SessionID = Request.Params("SessionID");
                string chave = Request.Params("Chave").Replace(" ", "").Replace(".", "");
                string captcha = Request.Params("Captcha");

                if (tipoConsultaPortalFazenda == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaPortalFazenda.NFe)
                {
                    Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz requisicaoSefaz = new Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz();
                    requisicaoSefaz.VIEWSTATE = VIEWSTATE;
                    requisicaoSefaz.EVENTVALIDATION = EVENTVALIDATION;
                    requisicaoSefaz.TokenCaptcha = TokenCaptcha;
                    requisicaoSefaz.SessionID = SessionID;

                    string retorno = BuscarDadosPortaNFe(requisicaoSefaz, cargaPedido, chave, captcha, unitOfWork);
                    if (string.IsNullOrEmpty(retorno))
                    {
                        if (tipoContratacao != cargaPedido.TipoContratacaoCarga)
                            return new JsonpResult(serCarga.ObterDetalhesDaCarga(cargaPedido.Carga, TipoServicoMultisoftware, unitOfWork));
                        else
                            return new JsonpResult(true);
                    }
                    else
                        return new JsonpResult(false, true, retorno);
                }
                else
                {
                    Dominio.ObjetosDeValor.WebService.CTe.RequisicaoSefaz requisicaoSefaz = new Dominio.ObjetosDeValor.WebService.CTe.RequisicaoSefaz();
                    requisicaoSefaz.VIEWSTATE = VIEWSTATE;
                    requisicaoSefaz.EVENTVALIDATION = EVENTVALIDATION;
                    requisicaoSefaz.TokenCaptcha = TokenCaptcha;
                    requisicaoSefaz.SessionID = SessionID;

                    string retorno = BuscarDadosPortalCTe(requisicaoSefaz, cargaPedido, chave, captcha, unitOfWork);
                    if (string.IsNullOrEmpty(retorno))
                    {
                        if (tipoContratacao != cargaPedido.TipoContratacaoCarga)
                            return new JsonpResult(serCarga.ObterDetalhesDaCarga(cargaPedido.Carga, TipoServicoMultisoftware, unitOfWork));
                        else
                            return new JsonpResult(true);
                    }
                    else
                        return new JsonpResult(false, true, retorno);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar no Sefaz.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string BuscarDadosPortalCTe(Dominio.ObjetosDeValor.WebService.CTe.RequisicaoSefaz requisicaoSefaz, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, string chave, string captcha, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.WebServicesConsultaNFe webService = Servicos.Embarcador.Documentos.ConsultaReceita.ObterWebService(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceConsultaReceita.CTe, unitOfWork);

            ConsultaCTe.CosultaCTeClient consultaCTe = new ConsultaCTe.CosultaCTeClient();

            if (webService != null)
                consultaCTe.Endpoint.Address = new EndpointAddress(webService.WebService);

            OperationContextScope scope = new OperationContextScope(consultaCTe.InnerChannel);
            MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "4ed60154d2f04201ab8b57ed4198da32");
            OperationContext.Current.OutgoingMessageHeaders.Add(header);

            ConsultaCTe.RetornoOfConsultaSefazpIzbOyUQ consultaSefaz = consultaCTe.ConsultarSefaz(requisicaoSefaz, chave, captcha);
            string retorno = "";
            if (consultaSefaz.Status)
            {
                if (consultaSefaz.Objeto.ConsultaValida)
                {
                    Servicos.Embarcador.Carga.CTeSubContratacao serCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
                    unitOfWork.Start();

                    if (consultaSefaz.Objeto.CTe.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario && !serCTeSubContratacao.SetarConfiguracaoCTeRedespachoIntermediario(out string erro, cargaPedido, consultaSefaz.Objeto.CTe, unitOfWork))
                        return erro;

                    serCTeSubContratacao.SetarConfiguracaoModalAereo(consultaSefaz.Objeto.CTe);
                    Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
                    retorno = serCTeSubContratacao.InformarDadosCTeNaCarga(unitOfWork, consultaSefaz.Objeto.CTe, cargaPedido, TipoServicoMultisoftware, ref pedidoCTeParaSubContratacao);

                    if (string.IsNullOrEmpty(retorno))
                    {
                        unitOfWork.CommitChanges();
                    }
                    else
                    {
                        unitOfWork.Rollback();
                    }
                }
                else
                {
                    retorno = consultaSefaz.Objeto.MensagemSefaz;
                }
            }
            else
            {
                retorno = consultaSefaz.Mensagem;
            }
            return retorno;
        }


        private string BuscarDadosPortaNFe(Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz requisicaoSefaz, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, string chave, string captcha, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.WebServicesConsultaNFe webService = Servicos.Embarcador.Documentos.ConsultaReceita.ObterWebService(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceConsultaReceita.NFe, unitOfWork);

            ConsultaNFe.ConsultaNFeClient consultaNFe = new ConsultaNFe.ConsultaNFeClient();

            if (webService != null)
                consultaNFe.Endpoint.Address = new EndpointAddress(webService.WebService);
                        
            OperationContextScope scope = new OperationContextScope(consultaNFe.InnerChannel);
            MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "4ed60154d2f04201ab8b57ed4198da32");
            OperationContext.Current.OutgoingMessageHeaders.Add(header);

            ConsultaNFe.RetornoOfConsultaSefazgj5B5PAD consultaSefaz = consultaNFe.ConsultarSefaz(requisicaoSefaz, chave, captcha);
            string retorno = "";
            if (consultaSefaz.Status)
            {
                if (consultaSefaz.Objeto.ConsultaValida)
                {
                    Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
                    unitOfWork.Start();

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && consultaSefaz.Objeto.NotaFiscal.ValorFrete > 0 || consultaSefaz.Objeto.NotaFiscal.ValorFreteLiquido > 0) //se for tms não seta e deixa o operador informar manualmente
                    {
                        consultaSefaz.Objeto.NotaFiscal.ValorFrete = 0;
                        consultaSefaz.Objeto.NotaFiscal.ValorFreteLiquido = 0;
                    }
                    
                    retorno = serCargaNotaFiscal.InformarDadosNotaCarga(consultaSefaz.Objeto.NotaFiscal, cargaPedido, TipoServicoMultisoftware, ConfiguracaoEmbarcador, Auditado, out bool alteradoTipoDeCarga);

                    if (string.IsNullOrEmpty(retorno))
                    {
                        unitOfWork.CommitChanges();
                    }
                    else
                    {
                        unitOfWork.Rollback();
                    }
                }
                else
                {
                    retorno = consultaSefaz.Objeto.MensagemSefaz;
                }
            }
            else
            {
                retorno = consultaSefaz.Mensagem;
            }
            return retorno;
        }


    }
}
