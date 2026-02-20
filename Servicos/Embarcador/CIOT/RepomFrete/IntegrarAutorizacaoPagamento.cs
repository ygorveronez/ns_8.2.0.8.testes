using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete;
using Newtonsoft.Json.Linq;
using Utilidades.Extensions;

namespace Servicos.Embarcador.CIOT.RepomFrete
{
    public partial class IntegracaoRepomFrete
    {
        #region Métodos Globais

        public bool IntegrarAutorizacaoPagamento(out string mensagemErro, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela tipoAutorizacaoPagamentoCIOTParcela = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela.Saldo)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOTIntegracaoArquivo repCargaCIOTIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCIOTIntegracaoArquivo(unitOfWork);

            this.configuracaoIntegracaoRepomFrete = this.ObterConfiguracaoRepomFrete(ciot.ConfiguracaoCIOT, unitOfWork);

            string protocoloAutorizacao = string.Empty;
            if (ciot.CIOTPorPeriodo)
            {
                if (cargaCIOT == null)
                {
                    mensagemErro = "Processo Abortado! CIOT por período e carga não definida.";
                    return false;
                }

                protocoloAutorizacao = cargaCIOT.CargaAberturaCIOT ? ciot.ProtocoloAutorizacao : cargaCIOT.ProtocoloAutorizacao;
            }
            else
            {
                cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);
                protocoloAutorizacao = ciot.ProtocoloAutorizacao;
            }

            if (string.IsNullOrEmpty(protocoloAutorizacao))
            {
                mensagemErro = "Processo Abortado! ID da viagem não definido.";
                return false;
            }

            if (tipoAutorizacaoPagamentoCIOTParcela == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela.Adiantamento)
            {
                //Valor do Adiantamento a ser creditado no Cartão/Conta do Contratado/Motorista.
                //Este valor será debitado imediatamente do Saldo de sua Conta Repom e Creditado para o Contratado/Motorista.
                mensagemErro = "Pagamento do Adiantamento é debitado do saldo da conta repom imediamente após emissão do contrato e creditado para o transportador.";
                return true;
            }

            #region Buscar Branch Code (Código da Filial Repom)
            string branchCode = cargaCIOT.Carga.Pedidos?.FirstOrDefault()?.Pedido?.CentroDeCustoViagem?.CodigoFilialRepom;

            if (string.IsNullOrEmpty(branchCode))
                branchCode = cargaCIOT.Carga.Empresa.Configuracao.CodigoFilialRepom;
            #endregion

            bool sucesso = false;

            // Efetua o login na administradora e gera o token
            if (this.ObterToken(out mensagemErro))
            {
                retornoWebService retornoWS = null;

                if (ciot.CIOTPorPeriodo)
                {
                    envShippingPaymen envioWS = ObterAutorizaContrato(protocoloAutorizacao, cargaCIOT, branchCode, unitOfWork);
                    retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "ShippingPayment", this.tokenAutenticacao);
                }
                else
                {
                    if (this.configuracaoIntegracaoRepomFrete?.RealizarEncerramentoAutorizacaoPagamentoSeparado ?? false)
                    {
                        List<envPaymentAuthorization> envioWS = ObterAutorizacaoPagamento(protocoloAutorizacao, cargaCIOT, branchCode, tipoAutorizacaoPagamentoCIOTParcela, unitOfWork);
                        retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "PaymentAuthorization", this.tokenAutenticacao);
                    }
                    else
                    {
                        envShippingPaymen envioWS = ObterAutorizaContrato(protocoloAutorizacao, cargaCIOT, branchCode, unitOfWork);
                        retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "ShippingPayment", this.tokenAutenticacao);
                    }
                }

                #region Salvar JSON

                if (!ciot.CIOTPorPeriodo || (ciot.CIOTPorPeriodo && cargaCIOT.CargaAberturaCIOT))
                {
                    Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
                    {
                        ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork),
                        ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork),
                        Data = DateTime.Now,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                        Mensagem = "Autorização de pagamento"
                    };

                    repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);
                    ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);
                    repCIOT.Atualizar(ciot);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCIOTIntegracaoArquivo cargaCIOTIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCIOTIntegracaoArquivo
                    {
                        ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork),
                        ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork),
                        Data = DateTime.Now,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                        Mensagem = "Autorização de pagamento"
                    };

                    repCargaCIOTIntegracaoArquivo.Inserir(cargaCIOTIntegracaoArquivo);
                    cargaCIOT.ArquivosTransacao.Add(cargaCIOTIntegracaoArquivo);
                    repCargaCIOT.Atualizar(cargaCIOT);
                }

                #endregion

                if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    if (ciot.CIOTPorPeriodo)
                        cargaCIOT.Mensagem = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    else
                        ciot.Mensagem = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);

                    mensagemErro = "Falha na integração da autorização de pagamento.";
                    sucesso = false;
                }
                else
                {
                    retPadrao retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retPadrao>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar JSON de retorno do webservice RepomFrete - IntegrarAutorizacaoPagamento: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        if (ciot.CIOTPorPeriodo)
                            cargaCIOT.Mensagem = string.Format("Message: Ocorreu uma falha ao efetuar a autorização de pagamento; RetornoWS {0}.", retornoWS.jsonRetorno);
                        else
                            ciot.Mensagem = string.Format("Message: Ocorreu uma falha ao efetuar a autorização de pagamento; RetornoWS {0}.", retornoWS.jsonRetorno);


                        mensagemErro = "Falha na integração da autorização de pagamento.";
                        sucesso = false;
                    }
                    else
                    {
                        if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                        {
                            if (ciot.CIOTPorPeriodo)
                            {
                                cargaCIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado;
                                cargaCIOT.Mensagem = "Quitação realizada com sucesso.";
                                cargaCIOT.DataAutorizacaoPagamento = DateTime.Now;
                            }
                            else
                            {
                                //Repom Frete não possui método de encerramento para CIOT padrão, o mesmo é encerrado automaticamente ao efetuar o pagamento do Saldo.
                                if (this.configuracaoIntegracaoRepomFrete?.RealizarEncerramentoAutorizacaoPagamentoSeparado ?? false)
                                {
                                    ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.PagamentoAutorizado;
                                    ciot.Mensagem = "Pagamento autorizado com sucesso.";
                                }
                                else
                                {
                                    ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado;
                                    ciot.Mensagem = "Quitação realizada com sucesso.";
                                    ciot.DataEncerramento = DateTime.Now;
                                }

                                ciot.DataAutorizacaoPagamento = DateTime.Now;
                            }

                            mensagemErro = "Autorizacao de pagamento integrada com sucesso.";
                            sucesso = true;
                        }
                        else
                        {
                            string mensagemRetorno = "Rejeitado:";

                            if (retorno.Errors != null && retorno.Errors.Count() > 0)
                                retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                            else if (!string.IsNullOrEmpty(retornoWS.jsonRetorno))
                            {
                                string mensagem = " ";
                                try
                                { 
                                    JObject jo = JObject.Parse(retornoWS.jsonRetorno);
                                    mensagem = jo.Property("Message").ToString();
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao analisar JSON de erro na autorização de pagamento RepomFrete: {ex.ToString()}", "CatchNoAction");
                                }
                                if (!string.IsNullOrEmpty(mensagem))
                                    mensagemRetorno += " Ocorreu um erro ao efetuar autorização de pagamento.";
                                else
                                    mensagemRetorno += mensagem;
                            }
                            else
                                mensagemRetorno += " Ocorreu um erro ao efetuar autorização de pagamento.";

                            if (ciot.CIOTPorPeriodo)
                                cargaCIOT.Mensagem = mensagemRetorno;
                            else
                                ciot.Mensagem = mensagemRetorno;

                            mensagemErro = $"Falha na integração da autorização de pagamento: {mensagemRetorno}";
                            sucesso = false;
                        }
                    }
                }
            }
            else
            {
                if (ciot.CIOTPorPeriodo)
                    cargaCIOT.Mensagem = mensagemErro;
                else
                    ciot.Mensagem = mensagemErro;
            }

            if (ciot.CIOTPorPeriodo)
                repCargaCIOT.Atualizar(cargaCIOT);
            else
                repCIOT.Atualizar(ciot);

            return sucesso;
        }

        #endregion

        #region Métodos Privados

        private envShippingPaymen ObterAutorizaContrato(string protocoloAutorizacao, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, string branchCode, Repositorio.UnitOfWork unitOfWork)
        {
            envShippingPaymen retorno = new envShippingPaymen();

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            retorno.BranchCode = branchCode;

            Int64 nShippingID;
            if (Int64.TryParse(protocoloAutorizacao, out nShippingID))
                retorno.ShippingID = nShippingID;

            retorno.TotalUnloadWeight = repPedidoXMLNotaFiscal.BuscarPesoPorCarga(cargaCIOT.Carga.Codigo);
            retorno.Documents = this.GerarShippingPaymenDocuments(cargaCIOT, branchCode);

            return retorno;
        }

        private List<envShippingPaymenDocuments> GerarShippingPaymenDocuments(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, string branchCode)
        {
            List<envShippingPaymenDocuments> retorno = null;

            foreach (Dominio.Entidades.Embarcador.Documentos.CIOTCTe ciotCTe in cargaCIOT.CIOT.CTes)
            {
                if (retorno == null)
                    retorno = new List<envShippingPaymenDocuments>();

                var doc = new envShippingPaymenDocuments();
                doc.BranchCode = branchCode;

                //['CTE', 'NFe', 'NFSe', 'CRT', 'OCC', 'Romaneio', 'MDFe'],
                if (ciotCTe.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || ciotCTe.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                    doc.DocumentType = "NFSe";
                else if (ciotCTe.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros)
                    doc.DocumentType = "OCC";
                else
                    doc.DocumentType = "CTE";

                doc.Series = ciotCTe.CargaCTe.CTe.Serie?.Numero.ToString() ?? string.Empty;
                doc.Number = ciotCTe.CargaCTe.CTe.Numero.ToString();

                //DocumentStatus: ['Dismissed', 'Lost', 'Delivered', 'Reshipping']
                doc.DocumentStatus = "Delivered";

                retorno.Add(doc);
            }

            return retorno;
        }

        private List<envPaymentAuthorization> ObterAutorizacaoPagamento(string protocoloAutorizacao, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, string branchCode, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela tipoAutorizacaoPagamentoCIOTParcela, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.CIOT.CIOTRepomFrete repCIOTRepomFrete = new Repositorio.Embarcador.CIOT.CIOTRepomFrete(unitOfWork);

            List<envPaymentAuthorization> retorno = new List<envPaymentAuthorization>();
            envPaymentAuthorization autorizacaoPagamento = new envPaymentAuthorization();

            Int64 nShippingID;
            if (Int64.TryParse(protocoloAutorizacao, out nShippingID))
                autorizacaoPagamento.ShippingId = nShippingID;

            autorizacaoPagamento.Identifier = cargaCIOT.CIOT.Codigo.ToString();
            autorizacaoPagamento.Branch = null;
            autorizacaoPagamento.BranchCode = branchCode;

            DateTime PaymentDate = System.DateTime.Now;           

            if (tipoAutorizacaoPagamentoCIOTParcela == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela.Saldo && cargaCIOT != null && 
                cargaCIOT.CIOT != null && cargaCIOT.CIOT.ConfiguracaoCIOT != null) 
            {
                var ciotRepomFrete = repCIOTRepomFrete.BuscarPorConfiguracaoCIOT(cargaCIOT.CIOT.ConfiguracaoCIOT.Codigo);
                if (cargaCIOT.CIOT.Transportador != null && ciotRepomFrete !=null && ciotRepomFrete.UsarDataPagamentoTransportadorTerceiro)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(cargaCIOT.CIOT.Transportador, unitOfWork);

                    if (modalidadeTerceiro != null && modalidadeTerceiro.HabilitarDataFixaVencimento)
                        PaymentDate = Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro.ObterVencimentoSaldoPessoa(cargaCIOT.CIOT.Transportador, PaymentDate);
                    else
                        PaymentDate = PaymentDate.AddDays(modalidadeTerceiro?.DiasVencimentoSaldoContratoFrete ?? 0);
                }
            }

            if (PaymentDate <= System.DateTime.Now) 
                PaymentDate = PaymentDate.AddHours(1);

            autorizacaoPagamento.PaymentDate = PaymentDate.ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T"); ;
            retorno.Add(autorizacaoPagamento);

            return retorno;
        }

        #endregion
    }
}
