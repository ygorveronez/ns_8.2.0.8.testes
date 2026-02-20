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

        public SituacaoRetornoCIOT IntegrarAdicionarViagemContratoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            bool sucesso = false;
            mensagemErro = string.Empty;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT retIntContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;

            try
            {
                this.configuracaoIntegracaoRepomFrete = this.ObterConfiguracaoRepomFrete(cargaCIOT.CIOT.ConfiguracaoCIOT, unitOfWork);

                var Motorista = cargaCIOT.Carga.Motoristas.FirstOrDefault();
                var Contratante = cargaCIOT.Carga.Empresa;
                var Veiculo = cargaCIOT.Carga.Veiculo;
                var VeiculosVinculados = cargaCIOT.Carga.VeiculosVinculados.ToList();

                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(cargaCIOT.CIOT.Transportador, unitOfWork);
                Dominio.Entidades.Veiculo carreta = VeiculosVinculados.FirstOrDefault();
                Dominio.Entidades.Cliente proprietarioCarreta = null;
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiroCarreta = null;

                if (carreta != null && carreta.Tipo == "T" && carreta.Proprietario != null && carreta.Proprietario.CPF_CNPJ != cargaCIOT.CIOT.Transportador.CPF_CNPJ)
                {
                    proprietarioCarreta = carreta.Proprietario;
                    modalidadeTerceiroCarreta = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(carreta.Proprietario, unitOfWork);
                }

                enumAcaoContratoFrete acaoContratoFrete = this.ObterAcaoContratoFrete(cargaCIOT);

                // Efetua o login na administradora e gera o token
                if (this.ObterToken(out mensagemErro))
                {
                    if (acaoContratoFrete == enumAcaoContratoFrete.IncluirContratoFrete)
                    {
                        string numeroCartao = modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? cargaCIOT.CIOT.Motorista.NumeroCartao : modalidadeTerceiro?.NumeroCartao;

                        Int64? pefCardNumber = null;
                        Int64 nCardNumber;
                        if (Int64.TryParse(numeroCartao, out nCardNumber))
                            pefCardNumber = nCardNumber;

                        #region Buscar Branch Code (Código da Filial Repom)
                        string branchCode = cargaCIOT.Carga.Pedidos?.FirstOrDefault()?.Pedido?.CentroDeCustoViagem?.CodigoFilialRepom;

                        if (string.IsNullOrEmpty(branchCode))
                            branchCode = cargaCIOT.Carga.Empresa.Configuracao.CodigoFilialRepom;
                        #endregion
                        string jsonEnvio = string.Empty;
                        string jsonRetorno = string.Empty;

                        if (IntegrarProprietario(cargaCIOT.CIOT.Transportador, modalidadeTerceiro, unitOfWork, out mensagemErro, out jsonEnvio, out jsonRetorno) &&
                            IntegrarProprietario(proprietarioCarreta, modalidadeTerceiroCarreta, unitOfWork, out mensagemErro, out jsonEnvio, out jsonRetorno) &&
                            IntegrarMotorista(cargaCIOT.CIOT.Transportador, Motorista, unitOfWork, out mensagemErro, out jsonEnvio, out jsonRetorno) &&
                            IntegrarVeiculo(Veiculo, false, unitOfWork, out mensagemErro, out jsonEnvio, out jsonRetorno) &&
                            IntegrarVeiculosVinculados(VeiculosVinculados.ToList(), true, unitOfWork, out mensagemErro, out jsonEnvio, out jsonRetorno) &&
                            ConsultarRoteiro(cargaCIOT.Carga, cargaCIOT.Carga.Rota, unitOfWork, out mensagemErro, out string codigoRoteiro, out string codigoPercurso, branchCode, out jsonEnvio, out jsonRetorno) &&
                            validarProprietarioContratoFrete(cargaCIOT.CIOT.Transportador, modalidadeTerceiro, unitOfWork, out mensagemErro) &&
                            validarCartaoProprietarioContratoFrete(cargaCIOT.CIOT.Transportador, modalidadeTerceiro, pefCardNumber, unitOfWork, out mensagemErro) &&
                            validarVeiculoContratoFrete(Veiculo, false, unitOfWork, out mensagemErro) &&
                            validarVeiculoContratoFrete(carreta, true, unitOfWork, out mensagemErro))
                        {
                            retIntContratoFrete = IntegrarAdicionarViagem(acaoContratoFrete, cargaCIOT, modalidadeTerceiro, codigoRoteiro, codigoPercurso, pefCardNumber, branchCode, tipoServicoMultisoftware, unitOfWork, out mensagemErro);

                            if (retIntContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao)
                                sucesso = true;
                        }
                    }
                    else
                    {
                        retIntContratoFrete = IntegrarAdicionarViagem(acaoContratoFrete, cargaCIOT, modalidadeTerceiro, null, null, null, null, tipoServicoMultisoftware, unitOfWork, out mensagemErro);

                        if (retIntContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao || retIntContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
                            sucesso = true;
                    }
                }
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);
                mensagemErro = $"Falha ao realizar a integração da Repom Frete: {excecao.Message}";
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                mensagemErro = "Ocorreu uma falha ao realizar a integração da Repom Frete.";
            }

            if (!sucesso)
            {
                cargaCIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                cargaCIOT.Mensagem = mensagemErro;
            }

            if (cargaCIOT.Codigo > 0)
                repCargaCIOT.Atualizar(cargaCIOT);
            else
                repCargaCIOT.Inserir(cargaCIOT);

            if (sucesso && retIntContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
                return SituacaoRetornoCIOT.Autorizado;
            else if (sucesso)
                return SituacaoRetornoCIOT.EmProcessamento;
            else
                return SituacaoRetornoCIOT.ProblemaIntegracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT IntegrarAdicionarViagem(enumAcaoContratoFrete acaoContratoFrete, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, string codigoRoteiro, string codigoPercurso, Int64? pefCardNumber, string branchCode, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCIOTIntegracaoArquivo(unitOfWork);
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

                if (string.IsNullOrEmpty(cargaCIOT.CIOT.CodigoContratoAgregado))
                {
                    mensagemErro = "Processo abortado! Não foi identificar o código de contrato agregado.";
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                }

                #endregion

                var envioWS = this.ObterContratoFrete(cargaCIOT, modalidadeTerceiro, codigoRoteiro, codigoPercurso, pefCardNumber, branchCode, emitirValePedagio, unitOfWork, tipoPagamentoCIOT, cargaCIOT.CIOT.CodigoContratoAgregado);

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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta do envio de viagem RepomFrete: {ex.ToString()}", "CatchNoAction");
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
                    Dominio.Entidades.Embarcador.Cargas.CargaCIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCIOTIntegracaoArquivo();

                    ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork);
                    ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork);
                    ciotIntegracaoArquivo.Data = DateTime.Now;
                    ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                    if (sucesso)
                        ciotIntegracaoArquivo.Mensagem = "Envio realizado com sucesso.";
                    else
                        ciotIntegracaoArquivo.Mensagem = "Falha no envio.";

                    repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

                    cargaCIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                    repCargaCIOT.Atualizar(cargaCIOT);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                #endregion

                if (sucesso && !string.IsNullOrEmpty(numeroProtocolo))
                {
                    cargaCIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao;
                    cargaCIOT.ProtocoloAbertura = numeroProtocolo;
                    cargaCIOT.Mensagem = "Viagem Aguardando Processamento.";
                    repCargaCIOT.Atualizar(cargaCIOT);

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
                string cEnvio = cargaCIOT.ProtocoloAbertura;
                var retornoWS = this.TransmitirRepom(enumTipoWS.GET, cEnvio, "Shipping/StatusProcessing/ByOperationKey", this.tokenAutenticacao);

                retShippingStatusProcessing retorno = null;

                try
                {
                    retorno = retornoWS.jsonRetorno.FromJson<retShippingStatusProcessing>();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar status de processamento da viagem RepomFrete: {ex.ToString()}", "CatchNoAction");
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
                    Dominio.Entidades.Embarcador.Cargas.CargaCIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCIOTIntegracaoArquivo();

                    ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork);
                    ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork);
                    ciotIntegracaoArquivo.Data = DateTime.Now;
                    ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
                    ciotIntegracaoArquivo.Mensagem = "Consultando status do envio.";

                    repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

                    cargaCIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                    repCargaCIOT.Atualizar(cargaCIOT);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                #endregion

                if (sucesso && !string.IsNullOrEmpty(IdViagemAdministradora))
                {
                    cargaCIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao;
                    cargaCIOT.ProtocoloAutorizacao = IdViagemAdministradora;
                    cargaCIOT.Mensagem = "CIOT Aguardando Processamento.";
                    repCargaCIOT.Atualizar(cargaCIOT);

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
                string cEnvio = cargaCIOT.ProtocoloAutorizacao;
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
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar consulta de status do contrato de frete RepomFrete: {ex.ToString()}", "CatchNoAction");
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
                    Dominio.Entidades.Embarcador.Cargas.CargaCIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCIOTIntegracaoArquivo();

                    ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork);
                    ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork);
                    ciotIntegracaoArquivo.Data = DateTime.Now;
                    ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
                    ciotIntegracaoArquivo.Mensagem = "Consultando status da viagem.";

                    repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

                    cargaCIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                    repCargaCIOT.Atualizar(cargaCIOT);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                #endregion

                if (sucesso && !string.IsNullOrEmpty(numeroCIOT))
                {
                    cargaCIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                    cargaCIOT.Mensagem = "CIOT processado com sucesso.";

                    repCargaCIOT.Atualizar(cargaCIOT);

                    AtualizarDadosValePedagio(valorValePedagio, numeroComprovanteValePedagio, fornecedorValePedagio, cargaCIOT.ProtocoloAutorizacao, cargaCIOT.Carga, tipoServicoMultisoftware, unitOfWork);

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

            return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao;
        }

        #endregion
    }
}
