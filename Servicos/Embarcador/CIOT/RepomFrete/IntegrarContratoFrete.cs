using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Utilidades.Extensions;

namespace Servicos.Embarcador.CIOT.RepomFrete
{
    public partial class IntegracaoRepomFrete
    {
        #region Métodos Globais

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT IntegrarContratoFrete(enumAcaoContratoFrete acaoContratoFrete, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, string codigoRoteiro, string codigoPercurso, Int64? pefCardNumber, string branchCode, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Filiais.ValePedagioTransportadorFilial repositorioValePedagioTransportador = new Repositorio.Embarcador.Filiais.ValePedagioTransportadorFilial(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento repConfiguracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork);

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repModalidadeTransportadoraPessoasTipoPagamentoCIOT = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT = repModalidadeTransportadoraPessoasTipoPagamentoCIOT.BuscarTipoPagamentoPorOperadora(modalidadeTerceiro.Codigo, OperadoraCIOT.RepomFrete);

            #region Incluir Contrato
            if (acaoContratoFrete == enumAcaoContratoFrete.IncluirContratoFrete)
            {
                #region Verificar Gerar Vale Pedágio

                bool emitirValePedagio = false;

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = repConfiguracaoCargaEmissaoDocumento.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial valePedagioTransportador = cargaCIOT.Carga.Filial != null ? repositorioValePedagioTransportador.BuscarPorFilialETransportador(cargaCIOT.Carga.Filial.Codigo, cargaCIOT.Carga.Empresa.Codigo) : null;
                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoValePedagio = cargaCIOT.Carga.Veiculo?.TiposIntegracaoValePedagio?.Count > 0 ? cargaCIOT.Carga.Veiculo.TiposIntegracaoValePedagio?.ToList() : valePedagioTransportador != null ? new List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>() { valePedagioTransportador.TipoIntegracaoValePedagio } : cargaCIOT.Carga.Empresa.TiposIntegracaoValePedagio.ToList();

                if (!cargaCIOT.Carga?.TipoOperacao.NaoComprarValePedagio ?? false && !(configuracaoCargaEmissaoDocumento?.NaoComprarValePedagio ?? false))
                {
                    if ((valePedagioTransportador?.ComprarValePedagio ?? cargaCIOT.Carga.Empresa.CompraValePedagio) || tiposIntegracaoValePedagio.Count > 0 && tiposIntegracaoValePedagio.Any(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.RepomFrete))
                        emitirValePedagio = true;
                }

                #endregion

                var envioWS = this.ObterContratoFrete(cargaCIOT, modalidadeTerceiro, codigoRoteiro, codigoPercurso, pefCardNumber, branchCode, emitirValePedagio, unitOfWork, tipoPagamentoCIOT);

                var retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "Shipping", this.tokenAutenticacao);

                bool sucesso = false;
                string numeroProtocolo = null;
                if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    mensagemErro = string.Concat("Ocorreu erro ao consumir o webservice: ", retornoWS.mensagem);
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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar JSON de retorno do webservice RepomFrete - IntegrarContratoFrete: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu um erro ao efetuar o envio da viagem; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                        {
                            numeroProtocolo = retorno.Response.Message.Replace("OperationKey: ", "").Replace("OperationKey:", "");
                            sucesso = true;
                        }
                        else
                        {
                            string mensagemRetorno = "Falha ao enviar o contrato de frete:";

                            if (retorno.Errors != null && retorno.Errors.Count() > 0)
                                retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                            else if (!string.IsNullOrEmpty(retorno.Response?.Message))
                                mensagemRetorno += retorno.Response?.Message;
                            else if (!string.IsNullOrEmpty(retorno.Message))
                                mensagemRetorno += retorno.Message;
                            else
                                mensagemRetorno += " Ocorreu um erro ao efetuar o envio do contrato de frete.";

                            mensagemErro = mensagemRetorno;
                            sucesso = false;
                        }
                    }
                }

                #region Salvar JSON
                try
                {
                    Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

                    ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork);
                    ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork);
                    ciotIntegracaoArquivo.Data = DateTime.Now;
                    ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                    if (sucesso)
                        ciotIntegracaoArquivo.Mensagem = "Envio realizado com sucesso.";
                    else
                        ciotIntegracaoArquivo.Mensagem = "Falha no envio.";

                    repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

                    cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                    repCIOT.Atualizar(cargaCIOT.CIOT);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                #endregion

                if (sucesso && !string.IsNullOrEmpty(numeroProtocolo))
                {
                    cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao;
                    cargaCIOT.CIOT.ProtocoloAbertura = numeroProtocolo;
                    cargaCIOT.CIOT.Mensagem = "Viagem Aguardando Processamento.";

                    repContratoFrete.Atualizar(cargaCIOT.ContratoFrete);
                    repCIOT.Atualizar(cargaCIOT.CIOT);

                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao;
                }
                else
                {
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                }
            }
            #endregion

            #region Atualizar Contrato ID Viagem
            else if (acaoContratoFrete == enumAcaoContratoFrete.ConsultarStatusIncluirContratoFrete)
            {
                string cEnvio = cargaCIOT.CIOT.ProtocoloAbertura;
                var retornoWS = this.TransmitirRepom(enumTipoWS.GET, cEnvio, "Shipping/StatusProcessing/ByOperationKey", this.tokenAutenticacao);

                retShippingStatusProcessing retorno = null;

                try
                {
                    retorno = retornoWS.jsonRetorno.FromJson<retShippingStatusProcessing>();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar JSON de status do processamento RepomFrete: {ex.ToString()}", "CatchNoAction");
                }

                bool sucesso = true;
                string IdViagemAdministradora = null;

                if (retorno == null)
                {
                    mensagemErro = string.Format("Message: Ocorreu um erro ao efetuar a consulta do status da viagem; RetornoWS {0}.", retornoWS.jsonRetorno);
                    sucesso = true;
                }
                else
                {
                    if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                    {
                        if (retorno.Result != null && retorno.Result.Results != null && retorno.Result.Results.Count() > 0)
                        {
                            //Status: 'Pending', 'Processing', 'Error', 'Finished'
                            var resultado = retorno.Result.Results.FirstOrDefault();

                            string cStatus = resultado.Status;

                            var nShippingID = resultado.ShippingId;
                            if (nShippingID != 0)
                                IdViagemAdministradora = nShippingID.ToString();

                            if (!string.IsNullOrEmpty(IdViagemAdministradora) || cStatus != "Error")
                            {
                                if (!string.IsNullOrEmpty(cStatus))
                                {
                                    string mensagemRetorno = retorno.Response.Message;

                                    if (string.IsNullOrEmpty(mensagemRetorno))
                                        mensagemRetorno = "Status Processamento Viagem: " + cStatus;
                                    else
                                        mensagemRetorno += System.Environment.NewLine + "Status Processamento Viagem: " + cStatus;

                                    mensagemErro = mensagemRetorno;
                                }
                            }
                            else
                            {
                                string mensagemRetorno = "Viagem Rejeitada:";

                                if (resultado.Errors != null && resultado.Errors.Count() > 0)
                                    resultado.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                                else
                                    mensagemRetorno = "Viagem Rejeitada pela administradora.";

                                mensagemErro = mensagemRetorno;
                                sucesso = false;
                            }
                        }
                    }
                    else
                    {
                        string mensagemRetorno = "Rejeitado:";

                        if (retorno.Errors != null && retorno.Errors.Count() > 0)
                            retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                        else
                            mensagemRetorno += " Ocorreu um erro ao efetuar o envio do contrato de frete.";

                        mensagemErro = mensagemRetorno;
                        sucesso = false;
                    }
                }

                #region Salvar JSON
                try
                {
                    Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

                    ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork);
                    ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork);
                    ciotIntegracaoArquivo.Data = DateTime.Now;
                    ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
                    ciotIntegracaoArquivo.Mensagem = "Consultando status do envio.";

                    repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

                    cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                    repCIOT.Atualizar(cargaCIOT.CIOT);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                #endregion

                if (sucesso && !string.IsNullOrEmpty(IdViagemAdministradora))
                {
                    cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao;
                    cargaCIOT.CIOT.ProtocoloAutorizacao = IdViagemAdministradora;
                    cargaCIOT.CIOT.Mensagem = "CIOT Aguardando Processamento.";

                    repContratoFrete.Atualizar(cargaCIOT.ContratoFrete);
                    repCIOT.Atualizar(cargaCIOT.CIOT);

                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao;
                }
                else if (sucesso)
                {
                    //Aguardando Processamento da Viagem.
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao;
                }
                else
                {
                    //Não foi possível processar a viagem por determinado problema.
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                }
            }
            #endregion

            #region Atualizar Contrato Dados CIOT
            else if (acaoContratoFrete == enumAcaoContratoFrete.ConsultarStatusCIOTANTT)
            {
                string cEnvio = cargaCIOT.CIOT.ProtocoloAutorizacao;
                var retornoWS = this.TransmitirRepom(enumTipoWS.GET, cEnvio, "Shipping/ByShipping", this.tokenAutenticacao, "2.4");

                retShippingByShipping retorno = null;

                bool sucesso = true;
                string numeroCIOT = null;
                string contratoID = null;
                decimal valorValePedagio = 0;
                string numeroComprovanteValePedagio = string.Empty;
                string fornecedorValePedagio = string.Empty;

                try
                {
                    retorno = retornoWS.jsonRetorno.FromJson<retShippingByShipping>();

                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar JSON de retorno ShippingByShipping RepomFrete: {ex.ToString()}", "CatchNoAction");
                }

                if (retorno == null)
                {
                    mensagemErro = string.Format("Message: Ocorreu um erro ao efetuar a consulta do status do contrato de frete; RetornoWS {0}.", retornoWS.jsonRetorno);
                    sucesso = false;
                }
                else
                {
                    if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                    {
                        if (retorno.Result != null)
                        {
                            if (!string.IsNullOrEmpty(retorno.Result.CIOT))
                            {
                                numeroCIOT = retorno.Result.CIOT;
                                contratoID = retorno.Result.ContractId != null ? retorno.Result.ContractId.ToString() : null;
                                valorValePedagio = (decimal)(retorno.Result.VPRValue != null ? retorno.Result.VPRValue : 0);
                                numeroComprovanteValePedagio = retorno.Result?.VPRAntt?.FirstOrDefault();
                                fornecedorValePedagio = retorno.Result.SupplierDocument.ObterSomenteNumeros();
                                sucesso = true;
                            }
                            else if (retorno.Result.Occurences != null && retorno.Result.Occurences.Count() > 0)
                            {
                                //Type: ['ErrorInCIOTEmissionAttempt', 'ReleasedWithoutCIOTIssue', 'ExemptionTACRate', 'LockShipping', 'UnlockShipping', 'InsufficientBalanceForTravelRelease', 'VPRCalculatedWithZeroValue'],
                                if (retorno.Result.Occurences.Where(x => x.Type == "ErrorInCIOTEmissionAttempt").FirstOrDefault() != null)
                                {
                                    string mensagemRetorno = "Rejeitado:";
                                    retorno.Result.Occurences.Where(x => x.Type == "ErrorInCIOTEmissionAttempt").ToList()
                                        .ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Detail);

                                    mensagemErro = mensagemRetorno;
                                    sucesso = false;
                                }
                                else if (retorno.Result.Status == "Cancelled")
                                {
                                    string mensagemRetorno = "Rejeitado: Viagem Cancelada pela Repom Frete.";
                                    mensagemErro = mensagemRetorno;
                                    sucesso = false;
                                }
                            }
                            else
                            {
                                string mensagemRetorno = "Processo Abortado! Não foi possível obter o nro do CIOT.";
                                if (!string.IsNullOrEmpty(retorno.Result.Status))
                                    mensagemRetorno += Environment.NewLine + "Status Viagem: " + retorno.Result.Status;

                                mensagemErro = mensagemRetorno;
                            }
                        }
                    }
                    else
                    {
                        string mensagemRetorno = "Rejeitado:";

                        if (retorno.Errors != null && retorno.Errors.Count() > 0)
                            retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                        else
                            mensagemRetorno += " Ocorreu um erro ao efetuar o envio do contrato de frete.";

                        mensagemErro = mensagemRetorno;
                        sucesso = false;
                    }
                }

                #region Salvar JSON
                try
                {
                    Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

                    ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork);
                    ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork);
                    ciotIntegracaoArquivo.Data = DateTime.Now;
                    ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
                    ciotIntegracaoArquivo.Mensagem = "Consultando status da viagem.";

                    repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

                    cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                    repCIOT.Atualizar(cargaCIOT.CIOT);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                #endregion

                if (sucesso && !string.IsNullOrEmpty(numeroCIOT))
                {
                    cargaCIOT.CIOT.Numero = numeroCIOT.Substring(0, 12);
                    cargaCIOT.CIOT.CodigoVerificador = numeroCIOT.Substring(12, 4);
                    cargaCIOT.CIOT.DataAbertura = DateTime.Now;
                    cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                    cargaCIOT.CIOT.Mensagem = "CIOT processado com sucesso.";
                    cargaCIOT.CIOT.CodigoContratoAgregado = contratoID;

                    repContratoFrete.Atualizar(cargaCIOT.ContratoFrete);
                    repCIOT.Atualizar(cargaCIOT.CIOT);

                    AtualizarDadosValePedagio(valorValePedagio, numeroComprovanteValePedagio, fornecedorValePedagio, cargaCIOT.CIOT.ProtocoloAutorizacao, cargaCIOT.Carga, tipoServicoMultisoftware, unitOfWork);

                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                }
                else if (sucesso)
                {
                    //Aguardando Processamento do CIOT.
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao;
                }
                else
                {
                    //Não foi possível processar a viagem por determinado problema.
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                }
            }
            #endregion

            //return true;
            return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao;
        }

        private List<envShipping> ObterContratoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, string codigoRoteiro, string codigoPercurso, Int64? pefCardNumber, string branchCode, bool emitirValePedagio, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT, string ContractId = "0")
        {
            List<envShipping> retorno = new List<envShipping>();

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaCIOT.Carga.Pedidos.FirstOrDefault();

            Dominio.Entidades.Cliente destinatario = null, remetente = null;

            if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor) && cargaPedido.Recebedor != null)
                destinatario = cargaPedido.Recebedor;
            else
                destinatario = cargaPedido.Pedido.Destinatario;

            if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor) && cargaPedido.Expedidor != null)
                remetente = cargaPedido.Expedidor;
            else
                remetente = cargaPedido.Pedido.Remetente;

            //Dominio.Entidades.Veiculo carreta = cargaCIOT.CIOT.VeiculosVinculados.FirstOrDefault();
            Dominio.Entidades.Veiculo carreta = cargaCIOT.Carga.Veiculo;
            Dominio.Entidades.Usuario motorista = cargaCIOT.Motorista;

            string rntrcCarreta = string.Empty;

            if (carreta != null && carreta.Proprietario != null && carreta.Tipo == "T" && carreta.Proprietario?.CPF_CNPJ != cargaCIOT.CIOT.Transportador.CPF_CNPJ && carreta.RNTRC != 0)
                rntrcCarreta = carreta.RNTRC.ToString().PadLeft(9, '0');

            DateTime dataSaida = DateTime.Now.AddHours(1);

            envShipping pef = new envShipping();

            pef.Identifier = cargaCIOT.CIOT.Codigo.ToString();
            pef.BranchCode = branchCode;
            pef.CIOTShippingType = cargaCIOT.CIOT.CIOTPorPeriodo ? "3" : "1";
            pef.ExistingContractId = ContractId;
            pef.Country = "Brazil";
            pef.HiredCountry = "Brazil";
            pef.HiredNationalId = cargaCIOT.CIOT.Transportador.CPF_CNPJ_SemFormato;
            pef.OperationIdentifier = cargaCIOT.Carga.TipoOperacao?.CodigoIntegracaoRepom;
            pef.CardNumber = pefCardNumber;

            if (emitirValePedagio)
            {
                Int64 nVPRCardNumber;
                string cardNumber = !string.IsNullOrEmpty(motorista?.NumeroCartaoValePedagio) ?
                                     motorista?.NumeroCartaoValePedagio : !string.IsNullOrEmpty(carreta?.NumeroCartaoValePedagio) ?
                                     carreta?.NumeroCartaoValePedagio : string.Empty;

                if (!string.IsNullOrEmpty(cardNumber) && Int64.TryParse(cardNumber, out nVPRCardNumber))
                    pef.VPRCardNumber = nVPRCardNumber;

                if (pef.VPRCardNumber == null && modalidadeTerceiro.TipoFavorecidoCIOT != TipoFavorecidoCIOT.Transportador)
                    throw new ServicoException("Configurado para emissão de vale pedágio junto ao CIOT e não encontrado cartão.");

                if (pef.VPRCardNumber == null && modalidadeTerceiro.TipoFavorecidoCIOT == TipoFavorecidoCIOT.Transportador && tipoPagamentoCIOT != TipoPagamentoCIOT.Deposito)
                    throw new ServicoException("Configurado para emissão de vale pedágio junto ao CIOT e não encontrado cartão.");

                if (modalidadeTerceiro.TipoFavorecidoCIOT == TipoFavorecidoCIOT.Transportador && tipoPagamentoCIOT == TipoPagamentoCIOT.Deposito)
                    pef.VPRCardNumber = 0;
            }
            else
            {
                pef.VPRCardNumber = null;
            }

            Int64 nRouteCode;
            if (Int64.TryParse(codigoRoteiro, out nRouteCode))
                pef.BrazilianRouteRouteCode = nRouteCode;

            int nTraceCode;
            if (int.TryParse(codigoPercurso, out nTraceCode))
                pef.BrazilianRouteTraceCode = nTraceCode;

            pef.IssueDate = dataSaida != null ? ((DateTime)dataSaida).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;

            pef.TotalFreightValue = (cargaCIOT.ContratoFrete.ValorLiquidoSemAdiantamentoEImpostos - cargaCIOT.ContratoFrete.ValorTotalDescontoSaldo - cargaCIOT.ContratoFrete.ValorTotalDescontoAdiantamento + cargaCIOT.ContratoFrete.ValorTotalAcrescimoSaldo + cargaCIOT.ContratoFrete.ValorTotalAcrescimoAdiantamento);
            pef.TotalLoadWeight = repPedidoXMLNotaFiscal.BuscarPesoPorCarga(cargaCIOT.Carga.Codigo);
            pef.TotalLoadValue = repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(cargaCIOT.Carga.Codigo);

            if(configuracaoIntegracaoRepomFrete?.EnviarQuantidadesMaioresQueZero ?? false)
            {
                if (pef.TotalLoadWeight <= 0)
                    pef.TotalLoadWeight = 1;

                if (pef.TotalLoadValue <= 0)
                    pef.TotalLoadValue = 1;
            }

            //Valor do Adiantamento a ser creditado no Cartão/Conta do Contratado/Motorista.
            //Este valor será debitado imediatamente do Saldo de sua Conta Repom e Creditado para o Contratado/Motorista.
            pef.AdvanceMoneyValue = cargaCIOT.ContratoFrete.ValorAdiantamento;

            pef.LoadBrazilianNCM = !string.IsNullOrWhiteSpace(cargaCIOT.Carga.TipoDeCarga?.NCM) && cargaCIOT.Carga.TipoDeCarga.NCM.Length >= 4 ? cargaCIOT.Carga.TipoDeCarga.NCM.Substring(0, 4) : "";
            pef.LoadBrazilianANTTCodeType = 1;

            if (cargaPedido.Origem.CodigoIBGE != 0)
                pef.BrazilianIBGECodeSource = cargaPedido.Origem.CodigoIBGE.ToString();

            if (!string.IsNullOrEmpty(remetente.CEP))
                pef.BrazilianCEPSource = Utilidades.String.OnlyNumbers(remetente.CEP);

            if (cargaPedido.Destino.CodigoIBGE != 0)
                pef.BrazilianIBGECodeDestination = cargaPedido.Destino.CodigoIBGE.ToString();

            if (!string.IsNullOrEmpty(destinatario.CEP))
                pef.BrazilianCEPDestination = Utilidades.String.OnlyNumbers(destinatario.CEP);

            pef.TravelledDistance = (int)cargaCIOT.Carga.DadosSumarizados.Distancia;

            pef.VPR = this.GerarShippingVPR(cargaCIOT, carreta, emitirValePedagio, unitOfWork);
            pef.Drivers = this.GerarShippingDrivers(cargaCIOT);
            pef.Vehicles = this.GerarShippingVehicles(cargaCIOT);
            pef.Documents = this.GerarShippingDocuments(cargaCIOT, branchCode);
            pef.ShippingPayment = this.GerarShippingShippingPayment(cargaCIOT, cargaPedido);
            pef.AccountingAdjustments = this.GerarShippingAccountingAdjustments(cargaCIOT);
            pef.ShippingReceiver = this.GerarShippingReceiver(destinatario);
            pef.Taxes = this.GerarShippingTaxes(cargaCIOT);
            //retorno.ShippingFuel = this.GerarShippingFuel(pPef);
            //retorno.ShippingInternationalRoute = this.GerarShippingInternationalRoute(pPef);

            retorno.Add(pef);

            return retorno;
        }

        private VPR GerarShippingVPR(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Veiculo veiculo, bool emitirValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.CargaPracaPedagio repCargaPracaPedagio = new Repositorio.CargaPracaPedagio(unitOfWork);
            Dominio.Entidades.CargaPracaPedagio cargaPracasPedagio = repCargaPracaPedagio.BuscarPorCarga(cargaCIOT.Carga.Codigo).FirstOrDefault();
            int qtdEixosSuspensos = cargaCIOT.Carga.ModeloVeicularCarga?.NumeroEixosSuspensos ?? 0;

            VPR retorno = new VPR();

            if (emitirValePedagio)
            {
                if (this.configuracaoIntegracaoRepomFrete?.RealizarCompraValePedagioIntegracaoCIOT ?? false)
                    retorno.Issue = true;
                else
                    retorno.Issue = false;

                retorno.VPROneWay = !veiculo.NaoComprarValePedagioRetorno;   //idavoltavalepedagio
                retorno.VPRSuspendedAxleNumber = 0;
                retorno.VPRReturnSuspendedAxleNumber = 0;

                if (cargaPracasPedagio != null)
                {
                    if (cargaPracasPedagio.EixosSuspenso == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspenso.Ida)
                        retorno.VPRSuspendedAxleNumber = qtdEixosSuspensos;

                    if (cargaPracasPedagio.EixosSuspenso == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspenso.Volta)
                        retorno.VPRReturnSuspendedAxleNumber = qtdEixosSuspensos;
                }
            }
            else
            {
                retorno.Issue = false;
                retorno.VPROneWay = true;
                retorno.VPRSuspendedAxleNumber = 0;
                retorno.VPRReturnSuspendedAxleNumber = 0;
            }

            return retorno;
        }

        private ShippingPayment GerarShippingShippingPayment(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            ShippingPayment retorno = new ShippingPayment();

            DateTime? expectedDeliveryDate = null;

            // Data previsão Pagamento Saldo
            if (this.configuracaoIntegracaoRepomFrete.UtilizarDataPrevisaoEntregaPedidoParaExpectativaPagamentoSaldo)
                expectedDeliveryDate = cargaPedido?.Pedido?.PrevisaoEntrega; 
            else
                expectedDeliveryDate = Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro.ObterVencimentoSaldoContrato(cargaCIOT.ContratoFrete, cargaCIOT.ContratoFrete.DataEmissaoContrato);

            retorno.ExpectedDeliveryDate = expectedDeliveryDate?.ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") ?? null;
            retorno.ExpectedDeliveryLocationType = "Branch";

            return retorno;
        }

        private List<Drivers> GerarShippingDrivers(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            List<Drivers> retorno = new List<Drivers>();

            var motorista = new Drivers();
            motorista.Country = "Brazil";
            motorista.NationalId = cargaCIOT.CIOT.Motorista.CPF;
            motorista.Main = true;
            retorno.Add(motorista);

            return retorno;
        }

        private List<Documents> GerarShippingDocuments(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, string branchCode)
        {
            List<Documents> retorno = null;

            foreach (Dominio.Entidades.Embarcador.Documentos.CIOTCTe ciotCTe in cargaCIOT.CIOT.CTes)
            {
                if (retorno == null)
                    retorno = new List<Documents>();

                var doc = new Documents();
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

                if (!(ciotCTe.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe   ||
                      ciotCTe.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS
                      ))
                    doc.EletronicKey = ciotCTe.CargaCTe.CTe.ChaveAcesso;

                retorno.Add(doc);
            }

            return retorno;
        }

        private List<Vehicles> GerarShippingVehicles(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            List<Vehicles> retorno = null;

            if (!string.IsNullOrEmpty(cargaCIOT.CIOT.Veiculo.Placa))
            {
                if (retorno == null)
                    retorno = new List<Vehicles>();

                var veic = new Vehicles();

                veic.Country = "Brazil";
                veic.LicensePlate = cargaCIOT.CIOT.Veiculo.Placa;

                retorno.Add(veic);
            }


            for (int i = 0; i < cargaCIOT.Carga?.VeiculosVinculados.Count; i++)
            {
                Dominio.Entidades.Veiculo reboque = cargaCIOT.Carga.VeiculosVinculados.ToList()[i];

                if (retorno == null)
                    retorno = new List<Vehicles>();

                var veic = new Vehicles();

                veic.Country = "Brazil";
                veic.LicensePlate = reboque.Placa;

                retorno.Add(veic);
            }

            return retorno;
        }

        private List<AccountingAdjustments> GerarShippingAccountingAdjustments(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            List<AccountingAdjustments> retorno = null;

            return retorno;
        }

        private ShippingReceiver GerarShippingReceiver(Dominio.Entidades.Cliente destinatario)
        {
            ShippingReceiver retorno = null;

            if (destinatario != null && !string.IsNullOrEmpty(destinatario.CPF_CNPJ_SemFormato))
            {
                if (retorno == null)
                    retorno = new ShippingReceiver();

                retorno.Country = "Brazil";
                retorno.NationalId = destinatario.CPF_CNPJ_SemFormato;
                retorno.Name = destinatario.Nome;

                if (destinatario.Tipo == "J")
                    retorno.ReceiverType = "Company";
                else if (destinatario.Tipo == "F")
                    retorno.ReceiverType = "Person";
            }

            return retorno;
        }

        private List<Taxes> GerarShippingTaxes(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            List<Taxes> retorno = new List<Taxes>();

            var irrf = new Taxes();
            irrf.Type = "IRRF";
            if (cargaCIOT.ContratoFrete.ValorIRRF <= 10)
                irrf.Value = 0;
            else
                irrf.Value = cargaCIOT.ContratoFrete.ValorIRRF;
            retorno.Add(irrf);

            var inss = new Taxes();
            inss.Type = "INSS";
            inss.Value = cargaCIOT.ContratoFrete.ValorINSS;
            retorno.Add(inss);

            var sest = new Taxes();
            sest.Type = "SEST";
            sest.Value = cargaCIOT.ContratoFrete.ValorSEST;
            retorno.Add(sest);

            var senat = new Taxes();
            senat.Type = "SENAT";
            senat.Value = cargaCIOT.ContratoFrete.ValorSENAT;
            retorno.Add(senat);

            return retorno;
        }

        private ShippingFuel GerarShippingFuel(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            ShippingFuel retorno = null;

            return retorno;
        }

        private ShippingInternationalRoute GerarShippingInternationalRoute(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            ShippingInternationalRoute retorno = null;

            return retorno;
        }

        private enumAcaoContratoFrete ObterAcaoContratoFrete(Dominio.Entidades.Embarcador.Documentos.CIOT ciot)
        {
            enumAcaoContratoFrete retorno = enumAcaoContratoFrete.IncluirContratoFrete;

            if (string.IsNullOrEmpty(ciot.ProtocoloAbertura) && string.IsNullOrEmpty(ciot.ProtocoloAutorizacao))
                retorno = enumAcaoContratoFrete.IncluirContratoFrete;
            else if (!string.IsNullOrEmpty(ciot.ProtocoloAbertura) && string.IsNullOrEmpty(ciot.ProtocoloAutorizacao))
                retorno = enumAcaoContratoFrete.ConsultarStatusIncluirContratoFrete;
            else if (!string.IsNullOrEmpty(ciot.ProtocoloAutorizacao))
                retorno = enumAcaoContratoFrete.ConsultarStatusCIOTANTT;

            return retorno;
        }

        private enumAcaoContratoFrete ObterAcaoContratoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            enumAcaoContratoFrete retorno = enumAcaoContratoFrete.IncluirContratoFrete;

            if (string.IsNullOrEmpty(cargaCIOT.ProtocoloAbertura) && string.IsNullOrEmpty(cargaCIOT.ProtocoloAutorizacao))
                retorno = enumAcaoContratoFrete.IncluirContratoFrete;
            else if (!string.IsNullOrEmpty(cargaCIOT.ProtocoloAbertura) && string.IsNullOrEmpty(cargaCIOT.ProtocoloAutorizacao))
                retorno = enumAcaoContratoFrete.ConsultarStatusIncluirContratoFrete;
            else if (!string.IsNullOrEmpty(cargaCIOT.ProtocoloAutorizacao))
                retorno = enumAcaoContratoFrete.ConsultarStatusCIOTANTT;

            return retorno;
        }

        private bool ConsultarViagem(ref Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            this.configuracaoIntegracaoRepomFrete = this.ObterConfiguracaoRepomFrete(ciot.ConfiguracaoCIOT, unitOfWork);

            if (this.ObterToken(out mensagemErro))
            {
                string cEnvio = ciot.ProtocoloAbertura;
                var retornoWS = this.TransmitirRepom(enumTipoWS.GET, cEnvio, "Shipping/StatusProcessing/ByOperationKey", this.tokenAutenticacao);
                mensagemErro = null;
                retShippingStatusProcessing retorno = null;

                bool sucesso = true;
                retorno = retornoWS.jsonRetorno.FromJson<retShippingStatusProcessing>();

                string IdViagemAdministradora = null;

                if (retorno == null)
                {
                    mensagemErro = string.Format("Message: Ocorreu um erro ao efetuar a consulta do status da viagem; RetornoWS {0}.", retornoWS.jsonRetorno);
                    sucesso = false;
                }
                else
                {
                    if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                    {
                        if (retorno.Result != null && retorno.Result.Results != null && retorno.Result.Results.Count() > 0)
                        {
                            //Status: 'Pending', 'Processing', 'Error', 'Finished'
                            var resultado = retorno.Result.Results.FirstOrDefault();

                            string cStatus = resultado.Status;

                            var nShippingID = resultado.ShippingId;
                            if (nShippingID != 0)
                                IdViagemAdministradora = nShippingID.ToString();

                            if (!string.IsNullOrEmpty(IdViagemAdministradora) || cStatus != "Error")
                            {
                                if (!string.IsNullOrEmpty(cStatus))
                                {
                                    string mensagemRetorno = retorno.Response.Message;

                                    if (string.IsNullOrEmpty(mensagemRetorno))
                                        mensagemRetorno = "Status Processamento Viagem: " + cStatus;
                                    else
                                        mensagemRetorno += System.Environment.NewLine + "Status Processamento Viagem: " + cStatus;

                                    mensagemErro = mensagemRetorno;
                                }
                            }
                            else
                            {
                                string mensagemRetorno = "Viagem Rejeitada:";

                                if (resultado.Errors != null && resultado.Errors.Count() > 0)
                                    resultado.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                                else
                                    mensagemRetorno = "Viagem Rejeitada pela administradora.";

                                mensagemErro = mensagemRetorno;
                            }
                        }
                    }
                    else
                    {
                        string mensagemRetorno = "Rejeitado:";


                        if (retorno.Errors != null && retorno.Errors.Count() > 0)
                        {
                            retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);

                        }
                        else
                            mensagemRetorno += " Ocorreu um erro ao efetuar o envio do contrato de frete.";

                        mensagemErro = mensagemRetorno;
                        sucesso = false;
                    }
                }
                if (sucesso && !string.IsNullOrEmpty(IdViagemAdministradora))
                {
                    ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao;
                    ciot.ProtocoloAutorizacao = IdViagemAdministradora;
                    ciot.Mensagem = "CIOT Aguardando Processamento.";
                    repCIOT.Atualizar(ciot);
                }
                return sucesso;
            }

            return false;
        }

        private bool ConsultarViagem(ref Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            this.configuracaoIntegracaoRepomFrete = this.ObterConfiguracaoRepomFrete(cargaCIOT.CIOT.ConfiguracaoCIOT, unitOfWork);

            if (this.ObterToken(out mensagemErro))
            {
                string cEnvio = cargaCIOT.ProtocoloAbertura;
                var retornoWS = this.TransmitirRepom(enumTipoWS.GET, cEnvio, "Shipping/StatusProcessing/ByOperationKey", this.tokenAutenticacao);
                mensagemErro = null;
                retShippingStatusProcessing retorno = null;

                bool sucesso = true;
                retorno = retornoWS.jsonRetorno.FromJson<retShippingStatusProcessing>();

                string IdViagemAdministradora = null;

                if (retorno == null)
                {
                    mensagemErro = string.Format("Message: Ocorreu um erro ao efetuar a consulta do status da viagem; RetornoWS {0}.", retornoWS.jsonRetorno);
                    sucesso = false;
                }
                else
                {
                    if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                    {
                        if (retorno.Result != null && retorno.Result.Results != null && retorno.Result.Results.Count() > 0)
                        {
                            //Status: 'Pending', 'Processing', 'Error', 'Finished'
                            var resultado = retorno.Result.Results.FirstOrDefault();

                            string cStatus = resultado.Status;

                            var nShippingID = resultado.ShippingId;
                            if (nShippingID != 0)
                                IdViagemAdministradora = nShippingID.ToString();

                            if (!string.IsNullOrEmpty(IdViagemAdministradora) || cStatus != "Error")
                            {
                                if (!string.IsNullOrEmpty(cStatus))
                                {
                                    string mensagemRetorno = retorno.Response.Message;

                                    if (string.IsNullOrEmpty(mensagemRetorno))
                                        mensagemRetorno = "Status Processamento Viagem: " + cStatus;
                                    else
                                        mensagemRetorno += System.Environment.NewLine + "Status Processamento Viagem: " + cStatus;

                                    mensagemErro = mensagemRetorno;
                                }
                            }
                            else
                            {
                                string mensagemRetorno = "Viagem Rejeitada:";

                                if (resultado.Errors != null && resultado.Errors.Count() > 0)
                                    resultado.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                                else
                                    mensagemRetorno = "Viagem Rejeitada pela administradora.";

                                mensagemErro = mensagemRetorno;
                            }
                        }
                    }
                    else
                    {
                        string mensagemRetorno = "Rejeitado:";


                        if (retorno.Errors != null && retorno.Errors.Count() > 0)
                        {
                            retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);

                        }
                        else
                            mensagemRetorno += " Ocorreu um erro ao efetuar o envio do contrato de frete.";

                        mensagemErro = mensagemRetorno;
                        sucesso = false;
                    }
                }
                if (sucesso && !string.IsNullOrEmpty(IdViagemAdministradora))
                {
                    cargaCIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao;
                    cargaCIOT.ProtocoloAutorizacao = IdViagemAdministradora;
                    cargaCIOT.Mensagem = "CIOT Aguardando Processamento.";
                    repCargaCIOT.Atualizar(cargaCIOT);
                }
                return sucesso;
            }
            return false;

        }

        private void AtualizarDadosValePedagio(decimal valorValePedagio, string numeroComprovanteValePedagio, string fornecedorValePedagio, string protocoloAutorizacao, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (valorValePedagio == 0)
                return;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra repositorioValePedagioDadosCompra = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(unitOfWork);

            #region Tabela de Integração a principio não é necessário incluir, entidade preenchida somente para utilizar os métodos já existentes.
            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio();
            cargaValePedagio.Carga = carga;
            cargaValePedagio.ValorValePedagio = valorValePedagio;
            cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Confirmada;
            cargaValePedagio.TipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.RepomFrete);
            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            cargaValePedagio.NumeroValePedagio = !string.IsNullOrEmpty(numeroComprovanteValePedagio) ? numeroComprovanteValePedagio : protocoloAutorizacao;
            cargaValePedagio.IdCompraValePedagio = protocoloAutorizacao;
            cargaValePedagio.TipoCompra = Dominio.Enumeradores.TipoCompraValePedagio.Cartao;
            cargaValePedagio.ProblemaIntegracao = "Vale Pedágio Comprado com Sucesso";
            #endregion

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra dadosCompra = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra
            {
                Carga = carga,
                CodigoFilialCliente = "",
                CodigoProcessoCliente = carga.CodigoCargaEmbarcador,
                CodigoViagem = protocoloAutorizacao.ToInt(),
                DataEmissao = DateTime.Now,
                ValorTotalPedagios = valorValePedagio
            };
            repositorioValePedagioDadosCompra.Inserir(dadosCompra);

            if (!string.IsNullOrEmpty(fornecedorValePedagio))
            {
                Dominio.Entidades.Cliente clienteFornecedorValePedagio = repCliente.BuscarPorCPFCNPJ(Convert.ToDouble(fornecedorValePedagio));

                if (clienteFornecedorValePedagio != null)
                    servicoValePedagio.AdicionarCargaValePedagioParaMDFe(cargaValePedagio, clienteFornecedorValePedagio, unitOfWork, false);
            }

            Servicos.Embarcador.Terceiros.ContratoFrete.AdicionarValePedagioContratoFrete(cargaValePedagio, unitOfWork, tipoServicoMultisoftware, valorValePedagio);
        }

        #endregion
    }
}
