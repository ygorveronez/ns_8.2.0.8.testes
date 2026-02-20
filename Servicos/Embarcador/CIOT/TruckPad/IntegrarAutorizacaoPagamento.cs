using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.CIOT.TruckPad;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json.Linq;
using Utilidades.Extensions;

namespace Servicos.Embarcador.CIOT.TruckPad
{
    public partial class IntegracaoTruckPad
    {
        #region Métodos Globais

        public bool IntegrarAutorizacaoPagamento(out string mensagemErro, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela tipoAutorizacaoPagamentoCIOTParcela = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela.Saldo)
        {
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repModalidadeTransportadoraPessoasTipoPagamentoCIOT = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unitOfWork);

            this._configuracaoIntegracaoTruckPad = this.ObterConfiguracaoTruckPad(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            if (string.IsNullOrEmpty(ciot.ProtocoloAutorizacao))
            {
                mensagemErro = "Processo Abortado! ID da viagem não definido.";
                return false;
            }

            if (tipoAutorizacaoPagamentoCIOTParcela == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela.Adiantamento)
            {
                mensagemErro = "Pagamento do Adiantamento é debitado do saldo da imediamente após emissão do contrato e creditado para o transportador.";
                return true;
            }

            bool sucesso = false;

            // Efetua o login na administradora e gera o token
            if (this.ObterToken(out mensagemErro))
            {
                retornoWebService retornoWS = null;
                string codigoParcela = null;
                if (tipoAutorizacaoPagamentoCIOTParcela == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela.Adiantamento)
                    codigoParcela = $"AD{cargaCIOT.CIOT.Codigo}";
                else
                    codigoParcela = $"SD{cargaCIOT.CIOT.Codigo}";

                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT = repModalidadeTransportadoraPessoasTipoPagamentoCIOT.BuscarTipoPagamentoPorOperadora(modalidadeTerceiro.Codigo, OperadoraCIOT.TruckPad);


                envAutorizacaoPagamento envioWS = ObterAutorizacaoPagamento(cargaCIOT, modalidadeTerceiro, tipoPagamentoCIOT, unitOfWork);

                //Transmite o arquivo
                retornoWS = this.TransmitirTruckPad(enumTipoWS.PATCH, envioWS, $"ciot/{cargaCIOT.CIOT.Numero}/installment/{codigoParcela}?locale=pt_BR", this.tokenAutenticacao);

                #region Salvar JSON
                Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork),
                    Data = DateTime.Now,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                    Mensagem = mensagemErro ?? string.Empty
                };

                repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

                ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                repCIOT.Atualizar(ciot);
                #endregion

                if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    ciot.Mensagem = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);

                    mensagemErro = "Falha na integração da autorização de pagamento.";
                    sucesso = false;
                }
                else
                {
                    retAutorizacaoPagamento retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retAutorizacaoPagamento>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao deserializar retorno JSON autorização pagamento TruckPad: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        ciot.Mensagem = string.Format("Message: Ocorreu uma falha ao efetuar a autorização de pagamento; RetornoWS {0}.", retornoWS.jsonRetorno);

                        mensagemErro = "Falha na integração da autorização de pagamento.";
                        sucesso = false;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(retorno.id))
                        {
                            ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.PagamentoAutorizado;
                            ciot.Mensagem = "Pagamento autorizado com sucesso.";
                            ciot.DataAutorizacaoPagamento = DateTime.Now;

                            mensagemErro = "Autorizacao de pagamento integrada com sucesso.";
                            sucesso = true;
                        }
                        else
                        {
                            string mensagemRetorno = "Rejeitado:";
                            mensagemRetorno += " Ocorreu um erro ao efetuar autorização de pagamento.";

                            ciot.Mensagem = mensagemRetorno;
                            mensagemErro = $"Falha na integração da autorização de pagamento: {mensagemRetorno}";
                            sucesso = false;
                        }
                    }
                }
            }
            else
            {
                ciot.Mensagem = mensagemErro;
            }

            repCIOT.Atualizar(ciot);

            return sucesso;
        }

        #endregion

        #region Métodos Privados

        private envAutorizacaoPagamento ObterAutorizacaoPagamento(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamento, Repositorio.UnitOfWork unitOfWork)
        {
            envAutorizacaoPagamento retorno = new envAutorizacaoPagamento();
            retorno.has_discount = false;
            retorno.flexible_payment = ObterAutorizacaoPagamentoInstallmentsFlexible_payment(cargaCIOT, modalidadeTerceiro, tipoPagamento);
            return retorno;
        }

        public envAutorizacaoPagamentoFlexiblePayment ObterAutorizacaoPagamentoInstallmentsFlexible_payment(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamento)
        {
            envAutorizacaoPagamentoFlexiblePayment retorno = null;

            if (tipoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.PIX)
            {
                retorno = new envAutorizacaoPagamentoFlexiblePayment();
                retorno.type = "pix";
                retorno.receiver = modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Transportador ? "owner" : "driver";
                retorno.key = modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Transportador ? (cargaCIOT.CIOT.Transportador?.ChavePix ?? "") : (cargaCIOT.CIOT.Motorista?.DadosBancarios?.FirstOrDefault()?.ChavePix ?? "");
            }

            return retorno;
        }
        #endregion
    }
}
