using System;
using System.Collections.Generic;
using System.Linq;
using Utilidades.Extensions;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.CIOT.RepomFrete
{
    public partial class IntegracaoRepomFrete
    {
        #region Métodos Globais

        public void EmitirPagamentoMotorista(int codigoPagamento, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno repPagamentoMotoristaIntegracaoRetorno = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento = repPagamentoMotorista.BuscarPorCodigo(codigoPagamento);
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio = repPagamentoMotoristaIntegracaoEnvio.BuscarPorPagamento(codigoPagamento);

            if (pagamento.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) > 0)
            {
                try
                {
                    pagamentoEnvio.Data = DateTime.Now;
                    this.configuracaoIntegracaoRepomFrete = this.ObterConfiguracaoRepomFrete(null, unidadeDeTrabalho);
                    string mensagemErro = string.Empty;

                    // Efetua o login na administradora e gera o token
                    if (!this.ObterToken(out mensagemErro))
                        throw new Exception($"Não foi possível obter o token: {mensagemErro}");

                    var acaoContratoFrete = string.IsNullOrEmpty(pagamentoEnvio.ProtocoloAbertura) ? enumAcaoContratoFrete.IncluirContratoFrete : enumAcaoContratoFrete.ConsultarStatusIncluirContratoFrete;

                    var retTransmissao = IntegrarPagamentoMotorista(acaoContratoFrete, unidadeDeTrabalho, pagamento, pagamentoEnvio, configuracaoTMS, tipoServicoMultisoftware, auditado, out mensagemErro, out string protocoloAbertura, out string idViagemOperadora, out string jsonEnvio, out string jsonRetorno);
                                        
                    if (retTransmissao == SituacaoCIOT.Pendencia)
                    {
                        pagamento.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.FalhaIntegracao;
                        pagamentoEnvio.Retorno = mensagemErro;                        
                        pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else if (retTransmissao == SituacaoCIOT.AgIntegracao)
                    {
                        pagamento.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AgIntegracao;
                        pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                        pagamentoEnvio.Retorno = "Enviado com sucesso";
                        if (string.IsNullOrEmpty(pagamentoEnvio.ProtocoloAbertura) && !string.IsNullOrEmpty(protocoloAbertura))
                            pagamentoEnvio.ProtocoloAbertura = protocoloAbertura;
                    }
                    else if (retTransmissao == SituacaoCIOT.Aberto)
                    {
                        pagamentoEnvio.Retorno = "Sucesso";  
                        pagamento.CodigoViagem = Convert.ToInt32(idViagemOperadora);
                        pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    }

                    if(!string.IsNullOrEmpty(jsonEnvio) && !string.IsNullOrEmpty(jsonRetorno))
                    {
                        Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetorno = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno();
                        pagamentoRetorno.Data = pagamentoEnvio.Data;
                        pagamentoRetorno.PagamentoMotoristaTMS = pagamento;
                        pagamentoRetorno.PagamentoMotoristaIntegracaoEnvio = pagamentoEnvio;
                        pagamentoRetorno.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonEnvio, "json", unidadeDeTrabalho);
                        pagamentoRetorno.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unidadeDeTrabalho);
                        pagamentoRetorno.ArquivoRetorno = jsonRetorno;
                        pagamentoRetorno.DescricaoRetorno = pagamentoEnvio.Retorno;
                        repPagamentoMotoristaIntegracaoRetorno.Inserir(pagamentoRetorno);
                    }                    
                }
                catch (ServicoException ex)
                {
                    pagamento.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.FalhaIntegracao;
                    pagamentoEnvio.Data = DateTime.Now;
                    pagamentoEnvio.Retorno = ex.Message;
                    pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);

                    pagamento.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.FalhaIntegracao;
                    pagamentoEnvio.Data = DateTime.Now;
                    pagamentoEnvio.Retorno = "Ocorreu uma falha ao transmitir o pagamento de motorista para Repom Frete.";
                    pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            else
            {
                SalvarPagamentoSemValor(ref pagamentoEnvio, ref pagamento);
            }
            repPagamentoMotoristaIntegracaoEnvio.Atualizar(pagamentoEnvio);
            repPagamentoMotorista.Atualizar(pagamento);            
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT IntegrarPagamentoMotorista(enumAcaoContratoFrete acaoContratoFrete, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, out string mensagemErro, out string protocoloAbertura, out string idViagemOperadora, out string jsonEnvio, out string jsonRetorno)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT retornoPagamento = SituacaoCIOT.Pendencia;
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            mensagemErro = null;
            protocoloAbertura = null;
            idViagemOperadora = null;
            jsonEnvio = string.Empty;
            jsonRetorno = string.Empty;

            #region Incluir Contrato
            if (acaoContratoFrete == enumAcaoContratoFrete.IncluirContratoFrete)
            {
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(pagamento.Carga.Codigo);

                if (contratoFrete == null)
                {
                    retornoPagamento = SituacaoCIOT.Pendencia;
                    mensagemErro = "Não foi encontrado contrato de frete vinculado a carga, necessário para efetuar está operação.";
                }
                else
                {
                    Dominio.Entidades.Cliente transportador = contratoFrete.TransportadorTerceiro;
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(transportador, unitOfWork);
                    Dominio.Entidades.Veiculo carreta = pagamento.Carga.VeiculosVinculados.FirstOrDefault();
                    Dominio.Entidades.Cliente proprietarioCarreta = null;
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiroCarreta = null;

                    #region Buscar Branch Code (Código da Filial Repom)
                    string branchCode = pagamento.Carga.Pedidos?.FirstOrDefault()?.Pedido?.CentroDeCustoViagem?.CodigoFilialRepom;

                    if (string.IsNullOrEmpty(branchCode))
                        branchCode = pagamento.Carga.Empresa.Configuracao.CodigoFilialRepom;
                    #endregion

                    if (carreta != null && carreta.Tipo == "T" && carreta.Proprietario != null && carreta.Proprietario.CPF_CNPJ != transportador.CPF_CNPJ)
                    {
                        proprietarioCarreta = carreta.Proprietario;
                        modalidadeTerceiroCarreta = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(carreta.Proprietario, unitOfWork);
                    }

                    if (IntegrarProprietario(contratoFrete.TransportadorTerceiro, modalidadeTerceiro, unitOfWork, out mensagemErro, out jsonEnvio, out jsonRetorno) &&
                        IntegrarProprietario(proprietarioCarreta, modalidadeTerceiroCarreta, unitOfWork, out mensagemErro, out jsonEnvio, out jsonRetorno) &&
                        IntegrarMotorista(transportador, pagamento.Motorista, unitOfWork, out mensagemErro, out jsonEnvio, out jsonRetorno) &&
                        IntegrarVeiculo(pagamento.Carga.Veiculo, false, unitOfWork, out mensagemErro, out jsonEnvio, out jsonRetorno) &&
                        IntegrarVeiculo(carreta, true, unitOfWork, out mensagemErro, out jsonEnvio, out jsonRetorno) &&
                        ConsultarRoteiro(pagamento.Carga, pagamento.Carga.Rota, unitOfWork, out mensagemErro, out string codigoRoteiro, out string codigoPercurso, branchCode, out jsonEnvio, out jsonRetorno))
                    {

                        List<envShipping> envioWS = this.ObterPagamentoMotorista(pagamento, pagamentoEnvio, transportador, modalidadeTerceiro, codigoRoteiro, codigoPercurso, branchCode, unitOfWork);

                        var retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "Shipping", this.tokenAutenticacao);

                        jsonEnvio = retornoWS.jsonEnvio;
                        jsonRetorno = retornoWS.jsonRetorno;

                        bool sucesso = false;
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
                                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta de autorização de pagamento motorista RepomFrete: {ex.ToString()}", "CatchNoAction");
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
                                    protocoloAbertura = retorno.Response.Message.Replace("OperationKey: ", "").Replace("OperationKey:", "");
                                    sucesso = true;
                                }
                                else
                                {
                                    string mensagemRetorno = "Falha ao enviar o contrato de frete:";

                                    if (retorno.Errors != null && retorno.Errors.Count() > 0)
                                        retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                                    else
                                        mensagemRetorno += " Ocorreu um erro ao efetuar o envio do contrato de frete.";

                                    mensagemErro = mensagemRetorno;
                                    sucesso = false;
                                }
                            }
                        }

                        if (sucesso && !string.IsNullOrEmpty(protocoloAbertura))
                        {
                            retornoPagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao;
                        }
                        else
                        {
                            retornoPagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                        }
                    }
                    else
                    {
                        retornoPagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                    }
                }
            }
            #endregion

            #region Atualizar Contrato ID Viagem
            else if (acaoContratoFrete == enumAcaoContratoFrete.ConsultarStatusIncluirContratoFrete)
            {
                string cEnvio = pagamentoEnvio.ProtocoloAbertura;
                var retornoWS = this.TransmitirRepom(enumTipoWS.GET, cEnvio, "Shipping/StatusProcessing/ByOperationKey", this.tokenAutenticacao);

                jsonEnvio = retornoWS.jsonEnvio;
                jsonRetorno = retornoWS.jsonRetorno;

                retShippingStatusProcessing retorno = null;               

                try
                {
                    retorno = retornoWS.jsonRetorno.FromJson<retShippingStatusProcessing>();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar status de processamento da viagem motorista RepomFrete: {ex.ToString()}", "CatchNoAction");
                }

                bool sucesso = true;
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
                                idViagemOperadora = nShippingID.ToString();

                            if (!string.IsNullOrEmpty(idViagemOperadora) || cStatus != "Error")
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

                if (sucesso && !string.IsNullOrEmpty(idViagemOperadora))
                {
                    retornoPagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                }
                else if (sucesso)
                {
                    retornoPagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao;
                }
                else
                {
                    retornoPagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                }
            }
            #endregion

            return retornoPagamento;
        }

        private List<envShipping> ObterPagamentoMotorista(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio, Dominio.Entidades.Cliente transportador, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, string codigoRoteiro, string codigoPercurso, string branchCode, Repositorio.UnitOfWork unitOfWork)
        {
            List<envShipping> retorno = new List<envShipping>();

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = pagamento.Carga.Pedidos.FirstOrDefault();

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
            Dominio.Entidades.Veiculo carreta = pagamento.Carga.Veiculo;
            Dominio.Entidades.Usuario motorista = pagamento.Motorista;

            string rntrcCarreta = string.Empty;

            if (carreta != null && carreta.Proprietario != null && carreta.Tipo == "T" && carreta.Proprietario?.CPF_CNPJ != transportador.CPF_CNPJ && carreta.RNTRC != 0)
                rntrcCarreta = carreta.RNTRC.ToString().PadLeft(9, '0');

            DateTime dataSaida = DateTime.Now.AddHours(1);

            envShipping pef = new envShipping();

            pef.Identifier = "Pag"+pagamento.Numero.ToString();
            pef.BranchCode = branchCode;
            pef.Country = "Brazil";
            pef.HiredCountry = "Brazil";
            pef.HiredNationalId = transportador.CPF_CNPJ_SemFormato;
            pef.OperationIdentifier = "3"; //TODO: Fixo futuramente criar parâmetro

            //string numeroCartao = motorista.NumeroCartao;
            string numeroCartao = modalidadeTerceiro?.NumeroCartao;

            Int64 nCardNumber;
            if (Int64.TryParse(numeroCartao, out nCardNumber))
                pef.CardNumber = nCardNumber;

            pef.VPRCardNumber = null;

            Int64 nRouteCode;
            if (Int64.TryParse(codigoRoteiro, out nRouteCode))
                pef.BrazilianRouteRouteCode = nRouteCode;

            int nTraceCode;
            if (int.TryParse(codigoPercurso, out nTraceCode))
                pef.BrazilianRouteTraceCode = nTraceCode;

            pef.IssueDate = dataSaida != null ? ((DateTime)dataSaida).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;

            pef.TotalFreightValue = pagamento.Valor;//- pagamento.ValorIRRF - pagamento.ValorSEST - pagamento.ValorSENAT - pagamento.ValorINSS;
            pef.TotalLoadWeight = repPedidoXMLNotaFiscal.BuscarPesoPorCarga(pagamento.Carga.Codigo);
            pef.TotalLoadValue = repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(pagamento.Carga.Codigo);

            //Valor do Adiantamento a ser creditado no Cartão/Conta do Contratado/Motorista.
            //Este valor será debitado imediatamente do Saldo de sua Conta Repom e Creditado para o Contratado/Motorista.
            pef.AdvanceMoneyValue = pagamento.Valor - pagamento.ValorIRRF - pagamento.ValorSEST - pagamento.ValorSENAT - pagamento.ValorINSS; //pef.TotalFreightValue;

            pef.LoadBrazilianNCM = !string.IsNullOrWhiteSpace(pagamento.Carga.TipoDeCarga?.NCM) && pagamento.Carga.TipoDeCarga.NCM.Length >= 4 ? pagamento.Carga.TipoDeCarga.NCM.Substring(0, 4) : "";
            pef.LoadBrazilianANTTCodeType = 1;

            if (cargaPedido.Origem.CodigoIBGE != 0)
                pef.BrazilianIBGECodeSource = cargaPedido.Origem.CodigoIBGE.ToString();

            if (!string.IsNullOrEmpty(remetente.CEP))
                pef.BrazilianCEPSource = Utilidades.String.OnlyNumbers(remetente.CEP);

            if (cargaPedido.Destino.CodigoIBGE != 0)
                pef.BrazilianIBGECodeDestination = cargaPedido.Destino.CodigoIBGE.ToString();

            if (!string.IsNullOrEmpty(destinatario.CEP))
                pef.BrazilianCEPDestination = Utilidades.String.OnlyNumbers(destinatario.CEP);

            pef.TravelledDistance = (int)pagamento.Carga.DadosSumarizados.Distancia;

            pef.VPR = this.PagamentoMotoristaGerarShippingVPR(carreta, unitOfWork);
            pef.Drivers = this.PagamentoMotoristaGerarShippingDrivers(motorista);
            pef.Vehicles = this.PagamentoMotoristaGerarShippingVehicles(pagamento);
            pef.Documents = this.PagamentoMotoristaGerarShippingDocuments(pagamento, branchCode);
            pef.ShippingPayment = this.PagamentoMotoristaGerarShippingShippingPayment(pagamento, pagamentoEnvio.Data);
            pef.AccountingAdjustments = this.PagamentoMotoristaGerarShippingAccountingAdjustments(pagamento);
            pef.ShippingReceiver = this.GerarShippingReceiver(destinatario);
            pef.Taxes = this.PagamentoMotoristaGerarShippingTaxes(pagamento);

            retorno.Add(pef);

            return retorno;
        }

        private VPR PagamentoMotoristaGerarShippingVPR(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            VPR retorno = new VPR();
            retorno.Issue = false;
            retorno.VPROneWay = false;
            retorno.VPRSuspendedAxleNumber = 0;
            retorno.VPRReturnSuspendedAxleNumber = 0;

            return retorno;
        }

        private List<Drivers> PagamentoMotoristaGerarShippingDrivers(Dominio.Entidades.Usuario motorista)
        {
            List<Drivers> retorno = new List<Drivers>();

            var retMotorista = new Drivers();
            retMotorista.Country = "Brazil";
            retMotorista.NationalId = motorista.CPF;
            retMotorista.Main = true;
            retorno.Add(retMotorista);

            return retorno;
        }

        private ShippingPayment PagamentoMotoristaGerarShippingShippingPayment(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento, DateTime dataPrevisaoPagamento)
        {
            ShippingPayment retorno = new ShippingPayment();

            // Data previsão Pagamento Saldo
            retorno.ExpectedDeliveryDate = dataPrevisaoPagamento.ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T");
            retorno.ExpectedDeliveryLocationType = "Branch";

            return retorno;
        }

        private List<AccountingAdjustments> PagamentoMotoristaGerarShippingAccountingAdjustments(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento)
        {
            List<AccountingAdjustments> retorno = null;

            return retorno;
        }

        private List<Vehicles> PagamentoMotoristaGerarShippingVehicles(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento)
        {
            List<Vehicles> retorno = null;

            if (!string.IsNullOrEmpty(pagamento.Carga.Veiculo.Placa))
            {
                if (retorno == null)
                    retorno = new List<Vehicles>();

                var veic = new Vehicles();

                veic.Country = "Brazil";
                veic.LicensePlate = pagamento.Carga.Veiculo.Placa;

                retorno.Add(veic);
            }


            for (int i = 0; i < pagamento.Carga?.VeiculosVinculados.Count; i++)
            {
                Dominio.Entidades.Veiculo reboque = pagamento.Carga.VeiculosVinculados.ToList()[i];

                if (retorno == null)
                    retorno = new List<Vehicles>();

                var veic = new Vehicles();

                veic.Country = "Brazil";
                veic.LicensePlate = reboque.Placa;

                retorno.Add(veic);
            }

            return retorno;
        }

        private List<Documents> PagamentoMotoristaGerarShippingDocuments(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento, string branchCode)
        {
            List<Documents> retorno = new List<Documents>(); ;

            var doc = new Documents();
            doc.BranchCode = branchCode;

            //['CTE', 'NFe', 'NFSe', 'CRT', 'OCC', 'Romaneio', 'MDFe'],
            doc.DocumentType = "Romaneio";

            doc.Series = "U";
            doc.Number = pagamento.Numero.ToString();
            doc.EletronicKey = null;

            retorno.Add(doc);

            return retorno;
        }

        private List<Taxes> PagamentoMotoristaGerarShippingTaxes(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento)
        {
            List<Taxes> retorno = new List<Taxes>();

            var irrf = new Taxes();
            irrf.Type = "IRRF";
            irrf.Value = pagamento.ValorIRRF;
            retorno.Add(irrf);

            var inss = new Taxes();
            inss.Type = "INSS";
            inss.Value = pagamento.ValorINSS;
            retorno.Add(inss);

            var sest = new Taxes();
            sest.Type = "SEST";
            sest.Value = pagamento.ValorSEST;
            retorno.Add(sest);

            var senat = new Taxes();
            senat.Type = "SENAT";
            senat.Value = pagamento.ValorSENAT;
            retorno.Add(senat);

            return retorno;
        }

        private void SalvarPagamentoSemValor(ref Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio, ref Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento)
        {
            pagamentoEnvio.ArquivoEnvio = "";
            pagamentoEnvio.Data = DateTime.Now;
            pagamentoEnvio.TipoIntegracaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista.RepomFrete;

            pagamentoEnvio.Retorno = "Não foi enviado para Repom Frete devido saldo do motorista";
            pagamentoEnvio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
        }

        #endregion
    }
}
