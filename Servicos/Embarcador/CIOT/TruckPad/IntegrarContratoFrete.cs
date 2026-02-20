using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.CIOT.TruckPad;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Utilidades.Extensions;

namespace Servicos.Embarcador.CIOT.TruckPad
{
    public partial class IntegracaoTruckPad
    {
        #region Métodos Globais

        #endregion

        #region Métodos Privados

        private bool IntegrarContratoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Int64? pefCardNumber, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento repConfiguracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork);

            envCIOTPadrao envioWS = this.ObterContratoFrete(cargaCIOT, modalidadeTerceiro, pefCardNumber, unitOfWork);

            var retornoWS = this.TransmitirTruckPad(enumTipoWS.POST, envioWS, "ciot/default?locale=pt_BR", this.tokenAutenticacao);

            bool sucesso = false;

            if (retornoWS.erro)
            {
                mensagemErro = string.Concat("Ocorreu erro ao consumir o webservice: ", retornoWS.mensagem);
                sucesso = false;
            }
            else
            {
                retCIOTPadrao retorno = null;

                try
                {
                    retorno = retornoWS.jsonRetorno.FromJson<retCIOTPadrao>();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar JSON de retorno do webservice TruckPad - IntegrarContratoFrete: {ex.ToString()}", "CatchNoAction");
                }

                if (retorno == null)
                {
                    mensagemErro = string.Format("Message: Ocorreu um erro ao efetuar o envio da viagem; RetornoWS {0}.", retornoWS.jsonRetorno);
                    sucesso = false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(retorno.code))
                    {
                        cargaCIOT.CIOT.Numero = retorno.code;
                        cargaCIOT.CIOT.CodigoVerificador = retorno.verifier_code;
                        cargaCIOT.CIOT.DataAbertura = DateTime.Now;
                        cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                        cargaCIOT.CIOT.Mensagem = "CIOT processado com sucesso.";

                        repContratoFrete.Atualizar(cargaCIOT.ContratoFrete);
                        repCIOT.Atualizar(cargaCIOT.CIOT);
                        sucesso = true;
                    }
                    else
                    {
                        string mensagemRetorno = "Falha ao enviar o contrato de frete:";
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

            return sucesso;
        }

        private envCIOTPadrao ObterContratoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Int64? pefCardNumber, Repositorio.UnitOfWork unitOfWork)
        {
            envCIOTPadrao retorno = new envCIOTPadrao();
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaCIOT.Carga.Pedidos.FirstOrDefault();

            DateTime dataPartida = cargaPedido.Pedido.DataPrevisaoSaida ?? DateTime.Now.AddHours(1);
            DateTime dataTermino = cargaPedido.Pedido.PrevisaoEntrega ?? dataPartida.AddDays(1);

            if (_configuracaoIntegracaoTruckPad.ConfiguracaoCIOT.UtilizarDataAtualComoInicioTerminoCIOT)
            {
                dataPartida = DateTime.Now;
                dataTermino = dataPartida.AddDays(_configuracaoIntegracaoTruckPad.ConfiguracaoCIOT.DiasTerminoCIOT ?? 1);
            }

            retorno.start_date = dataPartida.ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T"); ;
            retorno.end_date = dataTermino.ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T"); ;
            retorno.client_id = cargaCIOT.CIOT.Codigo.ToString();
            retorno.office_id = _configuracaoIntegracaoTruckPad.OfficeID;
            retorno.hireds = this.ObterContratoFreteHireds(cargaCIOT, modalidadeTerceiro);
            retorno.vehicles = this.ObterContratoFreteVehicles(cargaCIOT);
            retorno.taxes = this.ObterContratoFreteTaxes(cargaCIOT);
            retorno.events = this.ObterContratoFreteEvents(cargaCIOT);
            retorno.payment = this.ObterContratoFretePayment(cargaCIOT, modalidadeTerceiro, unitOfWork);
            retorno.fuel_value_money = 0;
            retorno.gross_value_money = Convert.ToInt32(Math.Round(cargaCIOT.ContratoFrete.ValorBruto * 100));
            retorno.note = null;
            retorno.integrated_by = null;
            retorno.payment_receiver = modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Transportador ? "owner" : "driver";
            retorno.origin_address = this.ObterContratoFreteOrigin_Address(cargaPedido);
            retorno.destination_address = this.ObterContratoFreteDestination_Address(cargaPedido);
            retorno.interested_persons = this.ObterContratoFreteInterested_Persons(cargaPedido);
            retorno.cargoes = this.ObterContratoFreteCargoes(cargaCIOT, unitOfWork);

            return retorno;
        }

        public envCIOTPadraoHireds ObterContratoFreteHireds(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro)
        {
            envCIOTPadraoHireds retorno = new envCIOTPadraoHireds();
            retorno.owner = ObterContratoFreteHiredsOwner(cargaCIOT, modalidadeTerceiro);
            retorno.subcontractor = ObterContratoFreteHiredsSubContractor(cargaCIOT);
            retorno.drivers = ObterContratoFreteHiredsDrivers(cargaCIOT);
            return retorno;
        }

        public envCIOTPadraoHiredsOwner ObterContratoFreteHiredsOwner(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro)
        {
            envCIOTPadraoHiredsOwner retorno = new envCIOTPadraoHiredsOwner();
            retorno.name = cargaCIOT.ContratoFrete.TransportadorTerceiro.Nome;
            retorno.document = cargaCIOT.ContratoFrete.TransportadorTerceiro.CPF_CNPJ_SemFormato;
            retorno.rntrc = string.IsNullOrEmpty(modalidadeTerceiro.RNTRC) ? null : modalidadeTerceiro.RNTRC.PadLeft(9, '0');
            retorno.telephone_number = cargaCIOT.ContratoFrete.TransportadorTerceiro.Celular;
            return retorno;
        }

        public envCIOTPadraoHiredsSubcontractor ObterContratoFreteHiredsSubContractor(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            envCIOTPadraoHiredsSubcontractor retorno = null;
            return retorno;
        }

        public List<envCIOTPadraoHiredsDrivers> ObterContratoFreteHiredsDrivers(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            List<envCIOTPadraoHiredsDrivers> retorno = new List<envCIOTPadraoHiredsDrivers>();
            var motorista = new envCIOTPadraoHiredsDrivers();
            motorista.name = cargaCIOT.CIOT.Motorista.Nome;
            motorista.document = cargaCIOT.CIOT.Motorista.CPF;
            motorista.rntrc = null;
            motorista.telephone_number = cargaCIOT.CIOT.Motorista.Celular;
            retorno.Add(motorista);
            return retorno;
        }

        public List<envCIOTPadraoVehicles> ObterContratoFreteVehicles(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            List<envCIOTPadraoVehicles> retorno = null;

            if (!string.IsNullOrEmpty(cargaCIOT.CIOT.Veiculo.Placa))
            {
                if (retorno == null)
                    retorno = new List<envCIOTPadraoVehicles>();

                var veic = new envCIOTPadraoVehicles();

                veic.plate = cargaCIOT.CIOT.Veiculo.Placa;
                veic.category = cargaCIOT.CIOT.Veiculo.TipoDoVeiculo?.Descricao;
                veic.rntrc = cargaCIOT.CIOT.Veiculo.RNTRC.ToString().PadLeft(9, '0');

                retorno.Add(veic);
            }

            for (int i = 0; i < cargaCIOT.Carga?.VeiculosVinculados.Count; i++)
            {
                Dominio.Entidades.Veiculo reboque = cargaCIOT.Carga.VeiculosVinculados.ToList()[i];

                if (retorno == null)
                    retorno = new List<envCIOTPadraoVehicles>();

                var veic = new envCIOTPadraoVehicles();

                veic.plate = reboque.Placa;
                veic.category = reboque.TipoDoVeiculo?.Descricao;
                veic.rntrc = string.Format("{0:00000000}", reboque.RNTRC);

                retorno.Add(veic);
            }

            return retorno;
        }

        public List<envCIOTPadraoTaxes> ObterContratoFreteTaxes(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            List<envCIOTPadraoTaxes> retorno = new List<envCIOTPadraoTaxes>();

            var irrf = new envCIOTPadraoTaxes();
            irrf.type = "ir";
            if (cargaCIOT.ContratoFrete.ValorIRRF <= 10)
                irrf.value_money = 0;
            else
                irrf.value_money = Convert.ToInt32(Math.Round(cargaCIOT.ContratoFrete.ValorIRRF * 100));
            retorno.Add(irrf);

            var inss = new envCIOTPadraoTaxes();
            inss.type = "inss";
            inss.value_money = Convert.ToInt32(Math.Round(cargaCIOT.ContratoFrete.ValorINSS * 100));
            retorno.Add(inss);

            var sest = new envCIOTPadraoTaxes();
            sest.type = "sest_senat";
            sest.value_money = Convert.ToInt32(Math.Round(cargaCIOT.ContratoFrete.ValorSENAT * 100));
            retorno.Add(sest);

            return retorno;
        }

        public List<envCIOTPadraoEvents> ObterContratoFreteEvents(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            List<envCIOTPadraoEvents> retorno = null;
            return retorno;
        }

        public envCIOTPadraoPayment ObterContratoFretePayment(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Repositorio.UnitOfWork unitOfWork)
        {
            envCIOTPadraoPayment retorno = new envCIOTPadraoPayment();
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repModalidadeTransportadoraPessoasTipoPagamentoCIOT = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT = repModalidadeTransportadoraPessoasTipoPagamentoCIOT.BuscarTipoPagamentoPorOperadora(modalidadeTerceiro.Codigo, OperadoraCIOT.TruckPad);

            if (tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.BBC)
                retorno.type = "bbc";
            else if (tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.PIX)
                retorno.type = "pix";
            else
                retorno.type = "deposit";

            retorno.installments = ObterContratoFretePaymentInstallments(cargaCIOT, modalidadeTerceiro, tipoPagamentoCIOT);
            retorno.bank_detail = ObterContratoFretePaymentBank_detail(retorno.type == "deposit", cargaCIOT, modalidadeTerceiro);
            return retorno;
        }

        public List<envCIOTPadraoPaymentInstallments> ObterContratoFretePaymentInstallments(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamento)
        {
            List<envCIOTPadraoPaymentInstallments> retorno = new List<envCIOTPadraoPaymentInstallments>();

            decimal valorAdiantamento = cargaCIOT.ContratoFrete.ValorAdiantamento;
            decimal valorFrete = cargaCIOT.ContratoFrete.SaldoAReceber;

            if (valorAdiantamento > 0m)
            {
                envCIOTPadraoPaymentInstallments adiantamento = new envCIOTPadraoPaymentInstallments();
                adiantamento.type = "advance";
                adiantamento.value_money = Convert.ToInt32(Math.Round(valorAdiantamento * 100));
                adiantamento.effectiveness = "automatic";
                adiantamento.status = "released";
                adiantamento.identification = null;
                adiantamento.origin_address = null;
                adiantamento.destination_address = null;
                adiantamento.external_client_id = $"AD{cargaCIOT.CIOT.Codigo}";
                adiantamento.flexible_payment = ObterContratoFreteInstallmentsFlexible_payment(cargaCIOT, modalidadeTerceiro, tipoPagamento); 
                
                retorno.Add(adiantamento);
            }

            envCIOTPadraoPaymentInstallments saldo = new envCIOTPadraoPaymentInstallments();
            saldo.type = "final_balance";
            saldo.value_money = Convert.ToInt32(Math.Round(valorFrete * 100));
            saldo.effectiveness = "manual";
            saldo.status = "pending";
            saldo.identification = null;
            saldo.origin_address = null;
            saldo.destination_address = null;
            saldo.external_client_id = $"SD{cargaCIOT.CIOT.Codigo}";
            saldo.flexible_payment = ObterContratoFreteInstallmentsFlexible_payment(cargaCIOT, modalidadeTerceiro, tipoPagamento);

            retorno.Add(saldo);

            return retorno;
        }
        public envCIOTPadraoPaymentInstallmentsFlexible_payment ObterContratoFreteInstallmentsFlexible_payment(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamento)
        {
            envCIOTPadraoPaymentInstallmentsFlexible_payment retorno = null;

            if (tipoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.PIX)
            {
                retorno = new envCIOTPadraoPaymentInstallmentsFlexible_payment();
                retorno.type = "pix";
                retorno.receiver = modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Transportador ? "owner" : "driver";
                retorno.key = modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Transportador ? (cargaCIOT.CIOT.Transportador?.ChavePix ?? "") : (cargaCIOT.CIOT.Motorista?.DadosBancarios?.FirstOrDefault()?.ChavePix ?? "");
            }

            return retorno;
        }
        public envCIOTPadraoPaymentBank_detail ObterContratoFretePaymentBank_detail(bool pagamentoEmDepositoEmContra, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro)
        {
            envCIOTPadraoPaymentBank_detail retorno = null;

            if (pagamentoEmDepositoEmContra)
            {
                if (modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Transportador)
                {
                    if (cargaCIOT.CIOT.Transportador.Banco != null && !string.IsNullOrWhiteSpace(cargaCIOT.CIOT.Transportador.NumeroConta) && !string.IsNullOrWhiteSpace(cargaCIOT.CIOT.Transportador.Agencia))
                    {
                        retorno = new envCIOTPadraoPaymentBank_detail();
                        retorno.bank = string.Format("{0:0000}", cargaCIOT.CIOT.Transportador.Banco.Numero);
                        retorno.agency = cargaCIOT.CIOT.Transportador.Agencia;
                        retorno.account = cargaCIOT.CIOT.Transportador.NumeroConta;
                        retorno.account_type = cargaCIOT.CIOT.Transportador.TipoContaBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Poupança ? "saving" : "current";
                    }
                }
                else if (modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista)
                {
                    if (cargaCIOT.CIOT.Motorista.Banco != null && !string.IsNullOrWhiteSpace(cargaCIOT.CIOT.Motorista.NumeroConta) && !string.IsNullOrWhiteSpace(cargaCIOT.CIOT.Motorista.Agencia))
                    {
                        retorno = new envCIOTPadraoPaymentBank_detail();
                        retorno.bank = string.Format("{0:0000}", cargaCIOT.CIOT.Motorista.Banco.Numero);
                        retorno.agency = cargaCIOT.CIOT.Motorista.Agencia;
                        retorno.account = cargaCIOT.CIOT.Motorista.NumeroConta;
                        retorno.account_type = cargaCIOT.CIOT.Motorista.TipoContaBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Poupança ? "saving" : "current";
                    }
                }
            }

            return retorno;
        }

        public envCIOTPadrao_Address ObterContratoFreteOrigin_Address(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            envCIOTPadrao_Address retorno = new envCIOTPadrao_Address();

            retorno.street = cargaPedido.Pedido.Remetente?.Endereco ?? "";
            retorno.number = cargaPedido.Pedido.Remetente?.Numero.ToString() ?? "";
            retorno.complement = cargaPedido.Pedido.Remetente?.Complemento ?? "";
            retorno.neighborhood = "";
            retorno.city = cargaPedido.Pedido.Remetente?.Localidade?.Descricao ?? "";
            retorno.state = cargaPedido.Pedido.Remetente?.Localidade?.Estado?.Sigla ?? "";
            retorno.country = cargaPedido.Pedido.Remetente?.Localidade?.Pais?.Descricao ?? "";
            retorno.zip_code = cargaPedido.Pedido.Remetente?.CEP ?? "";
            retorno.city_code = cargaPedido.Pedido.Remetente?.Localidade?.CodigoIBGE.ToString() ?? "";
            
            return retorno;
        }

        public envCIOTPadrao_Address ObterContratoFreteDestination_Address(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            envCIOTPadrao_Address retorno = new envCIOTPadrao_Address();

            if(cargaPedido.Pedido.Destinatario != null)
            {
                retorno.street = cargaPedido.Pedido.Destinatario.Endereco ?? "";
                retorno.number = cargaPedido.Pedido.Destinatario.Numero.ToString() ?? "";
                retorno.complement = cargaPedido.Pedido.Destinatario.Complemento ?? "";
                retorno.neighborhood = "";
                retorno.city = cargaPedido.Pedido.Destinatario.Localidade?.Descricao ?? "";
                retorno.state = cargaPedido.Pedido.Destinatario.Localidade?.Estado?.Sigla ?? "";
                retorno.country = cargaPedido.Pedido.Destinatario.Localidade?.Pais?.Descricao ?? "";
                retorno.zip_code = cargaPedido.Pedido.Destinatario.CEP ?? "";
                retorno.city_code = cargaPedido.Pedido.Destinatario.Localidade?.CodigoIBGE.ToString() ?? "";
            }

            return retorno;
        }

        public envCIOTPadraoInterested_persons ObterContratoFreteInterested_Persons(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            envCIOTPadraoInterested_persons retorno = new envCIOTPadraoInterested_persons();
            retorno.recipient = this.ObterContratoFreteInterested_PersonsRecipient(cargaPedido);
            retorno.service_taker = this.ObterContratoFreteInterested_PersonsService_taker(cargaPedido);
            retorno.consignee = this.ObterContratoFreteInterested_PersonsConsignee(cargaPedido);
            retorno.sender = this.ObterContratoFreteInterested_PersonsSender(cargaPedido);
            return retorno;
        }

        public envCIOTPadraoInterested_personsRecipient ObterContratoFreteInterested_PersonsRecipient(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            envCIOTPadraoInterested_personsRecipient retorno = null;

            if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor) && cargaPedido.Recebedor != null)
            {
                retorno = new envCIOTPadraoInterested_personsRecipient();

                retorno.document = cargaPedido.Recebedor.CPF_CNPJ_SemFormato;
                retorno.name = cargaPedido.Recebedor.Nome ?? "";

                retorno.address = new envCIOTPadrao_Address();
                retorno.address.street = cargaPedido.Recebedor.Endereco ?? "";
                retorno.address.number = cargaPedido.Recebedor.Numero.ToString() ?? "";
                retorno.address.complement = cargaPedido.Recebedor.Complemento ?? "";
                retorno.address.neighborhood = "";
                retorno.address.city = cargaPedido.Recebedor.Localidade?.Descricao ?? "";
                retorno.address.state = cargaPedido.Recebedor.Localidade?.Estado?.Sigla ?? "";
                retorno.address.country = cargaPedido.Recebedor.Localidade?.Pais?.Descricao ?? "";
                retorno.address.zip_code = cargaPedido.Recebedor.CEP ?? "";
                retorno.address.city_code = cargaPedido.Recebedor.Localidade?.CodigoIBGE.ToString() ?? "";
            }
            else if (cargaPedido.Pedido.Destinatario != null)
            {
                retorno = new envCIOTPadraoInterested_personsRecipient();
                retorno.document = cargaPedido.Pedido.Destinatario.CPF_CNPJ_SemFormato;
                retorno.name = cargaPedido.Pedido.Destinatario.Nome ?? "";

                retorno.address = new envCIOTPadrao_Address();
                retorno.address.street = cargaPedido.Pedido.Destinatario.Endereco ?? "";
                retorno.address.number = cargaPedido.Pedido.Destinatario.Numero.ToString() ?? "";
                retorno.address.complement = cargaPedido.Pedido.Destinatario.Complemento ?? "";
                retorno.address.neighborhood = "";
                retorno.address.city = cargaPedido.Pedido.Destinatario.Localidade?.Descricao ?? "";
                retorno.address.state = cargaPedido.Pedido.Destinatario.Localidade?.Estado?.Sigla ?? "";
                retorno.address.country = cargaPedido.Pedido.Destinatario.Localidade?.Pais?.Descricao ?? "";
                retorno.address.zip_code = cargaPedido.Pedido.Destinatario.CEP ?? "";
                retorno.address.city_code = cargaPedido.Pedido.Destinatario.Localidade?.CodigoIBGE.ToString() ?? "";              
            }

            return retorno;
        }

        public envCIOTPadraoInterested_personsService_taker ObterContratoFreteInterested_PersonsService_taker(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            envCIOTPadraoInterested_personsService_taker retorno = null;
            if (cargaPedido.Pedido.Tomador != null)
            {
                retorno = new envCIOTPadraoInterested_personsService_taker();
                retorno.document = cargaPedido.Pedido.Tomador.CPF_CNPJ_SemFormato;
                retorno.name = cargaPedido.Pedido.Tomador.Nome ?? "";

                retorno.address = new envCIOTPadrao_Address();
                retorno.address.street = cargaPedido.Pedido.Tomador.Endereco ?? "";
                retorno.address.number = cargaPedido.Pedido.Tomador.Numero.ToString() ?? "";
                retorno.address.complement = cargaPedido.Pedido.Tomador.Complemento ?? "";
                retorno.address.neighborhood = "";
                retorno.address.city = cargaPedido.Pedido.Tomador.Localidade?.Descricao ?? "";
                retorno.address.state = cargaPedido.Pedido.Tomador.Localidade?.Estado?.Sigla ?? "";
                retorno.address.country = cargaPedido.Pedido.Tomador.Localidade?.Pais?.Descricao ?? "";
                retorno.address.zip_code = cargaPedido.Pedido.Tomador.CEP ?? "";
                retorno.address.city_code = cargaPedido.Pedido.Tomador.Localidade?.CodigoIBGE.ToString() ?? "";
            }
            return retorno;
        }

        public envCIOTPadraoInterested_personsConsignee ObterContratoFreteInterested_PersonsConsignee(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            envCIOTPadraoInterested_personsConsignee retorno = null;

            if (cargaPedido.Pedido.Destinatario != null)
            {
                retorno = new envCIOTPadraoInterested_personsConsignee();
                retorno.document = cargaPedido.Pedido.Destinatario.CPF_CNPJ_SemFormato;
                retorno.name = cargaPedido.Pedido.Destinatario.Nome ?? "";

                retorno.address = new envCIOTPadrao_Address();
                retorno.address.street = cargaPedido.Pedido.Destinatario.Endereco ?? "";
                retorno.address.number = cargaPedido.Pedido.Destinatario.Numero.ToString() ?? "";
                retorno.address.complement = cargaPedido.Pedido.Destinatario.Complemento ?? "";
                retorno.address.neighborhood = "";
                retorno.address.city = cargaPedido.Pedido.Destinatario.Localidade?.Descricao ?? "";
                retorno.address.state = cargaPedido.Pedido.Destinatario.Localidade?.Estado?.Sigla ?? "";
                retorno.address.country = cargaPedido.Pedido.Destinatario.Localidade?.Pais?.Descricao ?? "";
                retorno.address.zip_code = cargaPedido.Pedido.Destinatario.CEP ?? "";
                retorno.address.city_code = cargaPedido.Pedido.Destinatario.Localidade?.CodigoIBGE.ToString() ?? "";
            }
            return retorno;
        }

        public envCIOTPadraoInterested_personsSender ObterContratoFreteInterested_PersonsSender(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            envCIOTPadraoInterested_personsSender retorno = null;
            if (cargaPedido.Pedido.Remetente != null)
            {
                retorno = new envCIOTPadraoInterested_personsSender();
                retorno.document = cargaPedido.Pedido.Remetente.CPF_CNPJ_SemFormato;
                retorno.name = cargaPedido.Pedido.Remetente.Nome ?? "";

                retorno.address = new envCIOTPadrao_Address();
                retorno.address.street = cargaPedido.Pedido.Remetente.Endereco ?? "";
                retorno.address.number = cargaPedido.Pedido.Remetente.Numero.ToString() ?? "";
                retorno.address.complement = cargaPedido.Pedido.Remetente.Complemento ?? "";
                retorno.address.neighborhood = "";
                retorno.address.city = cargaPedido.Pedido.Remetente.Localidade?.Descricao ?? "";
                retorno.address.state = cargaPedido.Pedido.Remetente.Localidade?.Estado?.Sigla ?? "";
                retorno.address.country = cargaPedido.Pedido.Remetente.Localidade?.Pais?.Descricao ?? "";
                retorno.address.zip_code = cargaPedido.Pedido.Remetente.CEP ?? "";
                retorno.address.city_code = cargaPedido.Pedido.Remetente.Localidade?.CodigoIBGE.ToString() ?? "";
            }
            return retorno;
        }

        public List<envCIOTPadraoCargoes> ObterContratoFreteCargoes(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Repositorio.UnitOfWork unitOfWork)
        {
            List<envCIOTPadraoCargoes> retorno = new List<envCIOTPadraoCargoes>();

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            envCIOTPadraoCargoes cargoes = new envCIOTPadraoCargoes();
            cargoes.load_weight = repPedidoXMLNotaFiscal.BuscarPesoPorCarga(cargaCIOT.Carga.Codigo); ;
            cargoes.nature_code = !string.IsNullOrWhiteSpace(cargaCIOT.Carga.TipoDeCarga?.NCM) && cargaCIOT.Carga.TipoDeCarga.NCM.Length >= 4 ? cargaCIOT.Carga.TipoDeCarga.NCM.Substring(0, 4) : ""; ;
            retorno.Add(cargoes);

            return retorno;
        }

        #endregion
    }
}
