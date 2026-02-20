using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos.ServicoAX.ContratoFrete;
using Servicos.ServicoPamCard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Servicos.Embarcador.CIOT
{
    public class Pamcard
    {
        #region Métodos Globais 

        public SituacaoRetornoCIOT AbrirCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            if (ciot.Codigo == 0)
                repCIOT.Inserir(ciot);

            Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao = ObterConfiguracaoPamcard(ciot.ConfiguracaoCIOT, unitOfWork);

            string mensagemErro;

            if (ciot.Contratante == null)
                ciot.Contratante = configuracao.Matriz;

            ciot.Operadora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pamcard;

            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, unitOfWork);

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repModalidadeTransportadoraPessoasTipoPagamentoCIOT = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT = repModalidadeTransportadoraPessoasTipoPagamentoCIOT.BuscarTipoPagamentoPorOperadora(modalidade.Codigo, OperadoraCIOT.Pamcard);

            bool sucesso = false;
            if (IntegrarCartaoPortadorFrete(ciot, modalidade, configuracao, unitOfWork, out mensagemErro, tipoPagamentoCIOT))
            {
                if (ciot.CIOTPorPeriodo)
                    sucesso = IntegrarAberturaContratoFrete(ciot, modalidade, configuracao, unitOfWork, out mensagemErro, tipoPagamentoCIOT);
                else
                    sucesso = IntegrarContratoFrete(ciot, modalidade, configuracao, unitOfWork, out mensagemErro, tipoPagamentoCIOT);
            }

            if (sucesso)
            {
                ciot.Situacao = SituacaoCIOT.Aberto;

                repCIOT.Atualizar(ciot);

                return SituacaoRetornoCIOT.Autorizado;
            }
            else
            {
                ciot.Situacao = SituacaoCIOT.Pendencia;
                ciot.Mensagem = mensagemErro;

                repCIOT.Atualizar(ciot);

                return SituacaoRetornoCIOT.ProblemaIntegracao;
            }
        }

        public bool EncerrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao = ObterConfiguracaoPamcard(ciot.ConfiguracaoCIOT, unitOfWork);

            if (IntegrarEncerramentoContratoFrete(ciot, configuracao, unitOfWork, out mensagemErro))
            {
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado;
                ciot.DataEncerramento = DateTime.Now;
                ciot.Mensagem = mensagemErro;

                repCIOT.Atualizar(ciot);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CancelarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao = ObterConfiguracaoPamcard(ciot.ConfiguracaoCIOT, unitOfWork);

            if (IntegrarCancelamentoContratoFrete(ciot, configuracao, unitOfWork, out mensagemErro))
            {
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado;
                ciot.DataCancelamento = DateTime.Now;
                ciot.Mensagem = mensagemErro;

                repCIOT.Atualizar(ciot);

                return true;
            }
            else
            {
                return false;
            }
        }

        public SituacaoRetornoCIOT AdicionarViagem(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Dominio.Entidades.Embarcador.Documentos.CIOT ciot = cargaCIOT.CIOT;

            Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao = ObterConfiguracaoPamcard(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, unitOfWork);

            SituacaoRetornoCIOT retorno = IntegrarAtualizacaoContratoFrete(ciot, modalidade, configuracao, unitOfWork, out mensagemErro, false) ? SituacaoRetornoCIOT.Autorizado : SituacaoRetornoCIOT.ProblemaIntegracao;

            if (retorno == SituacaoRetornoCIOT.Autorizado)
            {
                cargaCIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                cargaCIOT.Mensagem = "CIOT processado com sucesso.";
            }
            else
            {
                cargaCIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                cargaCIOT.Mensagem = mensagemErro;
            }

            repCargaCIOT.Atualizar(cargaCIOT);

            return retorno;
        }

        public bool AutorizarPagamentoCIOT(out string mensagemErro, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao = ObterConfiguracaoPamcard(ciot.ConfiguracaoCIOT, unitOfWork);

            bool sucesso = IntegrarAutorizacaoPagamentoCIOT(ciot, configuracao, unitOfWork, out mensagemErro);

            if (sucesso)
            {
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.PagamentoAutorizado;
                ciot.Mensagem = "Pagamento autorizado com sucesso.";
                ciot.DataAutorizacaoPagamento = DateTime.Now;
            }
            else
            {
                ciot.Mensagem = mensagemErro;
            }

            repCIOT.Atualizar(ciot);

            return sucesso;
        }

        public bool AutorizarPagamentoParcelaCIOT(out string mensagemErro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela tipoAutorizacaoPagamentoCIOTParcela, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao = ObterConfiguracaoPamcard(ciot.ConfiguracaoCIOT, unitOfWork);

            bool sucesso = IntegrarAutorizacaoPagamentoCIOT(ciot, configuracao, unitOfWork, out mensagemErro, tipoAutorizacaoPagamentoCIOTParcela);

            if (sucesso)
            {
                switch (tipoAutorizacaoPagamentoCIOTParcela)
                {
                    case TipoAutorizacaoPagamentoCIOTParcela.Adiantamento:
                        ciot.DataAutorizacaoPagamentoAdiantamento = DateTime.Now;
                        ciot.AdiantamentoPago = true;
                        break;
                    case TipoAutorizacaoPagamentoCIOTParcela.Abastecimento:
                        ciot.DataAutorizacaoPagamentoAbastecimento = DateTime.Now;
                        ciot.AbastecimentoPago = true;
                        break;
                    case TipoAutorizacaoPagamentoCIOTParcela.Saldo:
                        ciot.DataAutorizacaoPagamentoSaldo = DateTime.Now;
                        ciot.SaldoPago = true;
                        break;
                }
            }
            else
            {
                ciot.Mensagem = mensagemErro;
            }

            repCIOT.Atualizar(ciot);

            return sucesso;
        }

        public bool AutorizarPagamentoAcrescimoDesconto(out string mensagemErro, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao = ObterConfiguracaoPamcard(ciot.ConfiguracaoCIOT, unitOfWork);

            return IntegrarAutorizacaoPagamentoAcrescimoDesconto(ciot, configuracao, unitOfWork, out mensagemErro);
        }

        public static void ConfigurarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.TipoOperacao == null ||
                carga.TipoOperacao.ConfiguracaoCIOT == null ||
                carga.TipoOperacao.ConfiguracaoCIOT.OperadoraCIOT != Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pamcard)
                return;

            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacaoConfiguracaoCIOTPamcard repTipoOperacaoConfiguracaoCIOTPamcard = new Repositorio.Embarcador.Pedidos.TipoOperacaoConfiguracaoCIOTPamcard(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOTDadosPamcard repCIOTDadosPamcard = new Repositorio.Embarcador.Documentos.CIOTDadosPamcard(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoCIOTPamcard tipoOperacaoConfiguracaoCIOTPamcard = repTipoOperacaoConfiguracaoCIOTPamcard.BuscarPorTipoOperacao(carga.TipoOperacao);

            if (tipoOperacaoConfiguracaoCIOTPamcard == null ||
                !tipoOperacaoConfiguracaoCIOTPamcard.UtilizarConfiguracaoPersonalizadaParcelas)
                return;

            Dominio.Entidades.Embarcador.Documentos.CIOTDadosPamcard ciotDadosPamcard = repCIOTDadosPamcard.BuscarPorCIOT(ciot);

            if (ciotDadosPamcard != null)
                return;

            ciotDadosPamcard = new Dominio.Entidades.Embarcador.Documentos.CIOTDadosPamcard()
            {
                EfetivacaoAbastecimento = tipoOperacaoConfiguracaoCIOTPamcard.EfetivacaoAbastecimento,
                EfetivacaoAdiantamento = tipoOperacaoConfiguracaoCIOTPamcard.EfetivacaoAdiantamento,
                EfetivacaoSaldo = tipoOperacaoConfiguracaoCIOTPamcard.EfetivacaoSaldo,
                StatusAbastecimento = tipoOperacaoConfiguracaoCIOTPamcard.StatusAbastecimento,
                StatusAdiantamento = tipoOperacaoConfiguracaoCIOTPamcard.StatusAdiantamento,
                StatusSaldo = tipoOperacaoConfiguracaoCIOTPamcard.StatusSaldo,
                CIOT = ciot
            };

            ciot.EfetivacaoAbastecimento = tipoOperacaoConfiguracaoCIOTPamcard.EfetivacaoAbastecimento;
            ciot.EfetivacaoAdiantamento = tipoOperacaoConfiguracaoCIOTPamcard.EfetivacaoAdiantamento;
            ciot.EfetivacaoSaldo = tipoOperacaoConfiguracaoCIOTPamcard.EfetivacaoSaldo;

            repCIOT.Atualizar(ciot);
            repCIOTDadosPamcard.Inserir(ciotDadosPamcard);
        }

        public bool IntegrarMovimentoFinanceiro(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valorMovimento, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao = ObterConfiguracaoPamcard(cargaCIOT.CIOT.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(cargaCIOT.CIOT.Transportador, unitOfWork);

            return IntegrarAtualizacaoContratoFrete(cargaCIOT.CIOT, modalidade, configuracao, unitOfWork, out mensagemErro, false, justificativa, valorMovimento);
        }

        #endregion

        #region Métodos Privados

        private bool IntegrarAutorizacaoPagamentoCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela? tipoAutorizacaoPagamentoCIOTParcela = null)
        {
            mensagemErro = null;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            ServicoPamCard.execute execute = new ServicoPamCard.execute()
            {
                arg0 = new ServicoPamCard.requestTO()
                {
                    context = "PayParcel",
                    fields = ObterCamposAutorizacaoPagamentoContratoFrete(ciot, configuracao, unitOfWork, tipoAutorizacaoPagamentoCIOTParcela)
                }
            };

            ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(configuracao.Matriz, unitOfWork, out Servicos.Models.Integracao.InspectorBehavior inspector);

            ServicoPamCard.executeResponse retorno = null;

            try
            {
                retorno = svcPamCard.execute(execute);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao enviar para a Pamcard: " + ex.Message);
            }

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento
            };

            string codigoRetorno = string.Empty, mensagemRetorno = string.Empty;

            if (retorno != null)
            {
                codigoRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.codigo") select obj.value).FirstOrDefault();
                mensagemRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.descricao") select obj.value).FirstOrDefault();
            }
            else
            {
                mensagemRetorno = "Ocorreu uma falha ao enviar a autorização de pagamento para a Pamcard.";
            }

            ciotIntegracaoArquivo.Mensagem = $"{codigoRetorno} - {mensagemRetorno}";

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(ciot);

            if (codigoRetorno == "0")
            {
                return true;
            }
            else
            {
                mensagemErro = $"{codigoRetorno} - {mensagemRetorno}";

                return false;
            }
        }

        private bool IntegrarAutorizacaoPagamentoAcrescimoDesconto(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            ServicoPamCard.execute execute = new ServicoPamCard.execute()
            {
                arg0 = new ServicoPamCard.requestTO()
                {
                    context = "PayParcel",
                    fields = ObterCamposAutorizacaoPagamentoContratoFreteAcrescimoDesconto(ciot, configuracao)
                }
            };

            ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(configuracao.Matriz, unitOfWork, out Servicos.Models.Integracao.InspectorBehavior inspector);

            ServicoPamCard.executeResponse retorno = null;

            try
            {
                retorno = svcPamCard.execute(execute);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao enviar para a Pamcard: " + ex.Message);
            }

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento
            };

            string codigoRetorno = string.Empty, mensagemRetorno = string.Empty;

            if (retorno != null)
            {
                codigoRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.codigo") select obj.value).FirstOrDefault();
                mensagemRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.descricao") select obj.value).FirstOrDefault();
            }
            else
                mensagemRetorno = "Ocorreu uma falha ao enviar a autorização de pagamento do acréscimo/desconto para a Pamcard.";

            ciotIntegracaoArquivo.Mensagem = $"{codigoRetorno} - {mensagemRetorno}";

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(ciot);

            if (codigoRetorno == "0")
                return true;
            else
            {
                mensagemErro = $"{codigoRetorno} - {mensagemRetorno}";
                return false;
            }
        }

        private bool IntegrarAberturaContratoFrete(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            mensagemErro = null;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            ServicoPamCard.execute execute = new ServicoPamCard.execute()
            {
                arg0 = new ServicoPamCard.requestTO()
                {
                    context = "InsertFreightContract",
                    fields = ObterCamposAberturaContratoFrete(ciot, modalidade, configuracao, unitOfWork, tipoPagamentoCIOT)
                }
            };

            Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();
            ServicoPamCard.WSTransacionalClient svcPamCard = null;
            ServicoPamCard.executeResponse retorno = null;

            try
            {
                svcPamCard = ObterClientPamCard(configuracao.Matriz, unitOfWork, out inspector);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao carregar o endpoint da Pamcard: " + ex.Message);
            }

            try
            {
                retorno = svcPamCard.execute(execute);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao enviar para a Pamcard: " + ex.Message);
            }

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

            ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
            ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
            ciotIntegracaoArquivo.Data = DateTime.Now;
            ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

            string codigoRetorno = string.Empty, mensagemRetorno = string.Empty, numeroCIOT = string.Empty, codigoViagem = string.Empty, codigoVerificador = string.Empty, digito = string.Empty;

            if (retorno != null)
            {
                codigoRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.codigo") select obj.value).FirstOrDefault();
                mensagemRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.descricao") select obj.value).FirstOrDefault();
                numeroCIOT = (from obj in retorno.@return where obj.key.Equals("viagem.antt.ciot.numero") select obj.value).FirstOrDefault();
                codigoViagem = (from obj in retorno.@return where obj.key.Equals("viagem.id") select obj.value).FirstOrDefault();
                codigoVerificador = (from obj in retorno.@return where obj.key.Equals("viagem.antt.ciot.protocolo") select obj.value).FirstOrDefault();
                digito = (from obj in retorno.@return where obj.key.Equals("viagem.digito") select obj.value).FirstOrDefault();
            }
            else
            {
                mensagemRetorno = "Ocorreu uma falha ao enviar o CIOT para a Pamcard.";
            }

            ciotIntegracaoArquivo.Mensagem = $"{codigoRetorno} - {mensagemRetorno}";

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            if (ciot.ArquivosTransacao == null)
                ciot.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo>();

            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            if (ciot.Codigo > 0)
                repCIOT.Atualizar(ciot);
            else
                repCIOT.Inserir(ciot);

            if (codigoRetorno == "0")
            {
                ciot.Numero = numeroCIOT;
                ciot.ProtocoloAutorizacao = codigoViagem;
                ciot.Mensagem = "CIOT processado com sucesso.";
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                ciot.CodigoVerificador = codigoVerificador;
                ciot.Digito = digito;

                repCIOT.Atualizar(ciot);

                return true;
            }
            else
            {
                mensagemErro = $"{codigoRetorno} - {mensagemRetorno}";

                return false;
            }
        }

        private bool IntegrarContratoFrete(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            mensagemErro = null;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            ServicoPamCard.execute execute = new ServicoPamCard.execute()
            {
                arg0 = new ServicoPamCard.requestTO()
                {
                    context = "InsertFreightContract",
                    fields = ObterCamposContratoFrete(ciot, modalidade, configuracao, unitOfWork, tipoPagamentoCIOT)
                }
            };

            Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();
            ServicoPamCard.WSTransacionalClient svcPamCard = null;
            ServicoPamCard.executeResponse retorno = null;

            try
            {
                svcPamCard = this.ObterClientPamCard(configuracao.Matriz, unitOfWork, out inspector);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao carregar o endpoint da Pamcard: " + ex.Message);
            }

            try
            {
                retorno = svcPamCard.execute(execute);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao enviar para a Pamcard: " + ex.Message);
            }

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

            ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
            ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
            ciotIntegracaoArquivo.Data = DateTime.Now;
            ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

            string codigoRetorno = string.Empty, mensagemRetorno = string.Empty, numeroCIOT = string.Empty, codigoViagem = string.Empty, codigoVerificador = string.Empty, digito = string.Empty;

            if (retorno != null)
            {
                codigoRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.codigo") select obj.value).FirstOrDefault();
                mensagemRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.descricao") select obj.value).FirstOrDefault();
                numeroCIOT = (from obj in retorno.@return where obj.key.Equals("viagem.antt.ciot.numero") select obj.value).FirstOrDefault();
                codigoViagem = (from obj in retorno.@return where obj.key.Equals("viagem.id") select obj.value).FirstOrDefault();
                codigoVerificador = (from obj in retorno.@return where obj.key.Equals("viagem.antt.ciot.protocolo") select obj.value).FirstOrDefault();
                digito = (from obj in retorno.@return where obj.key.Equals("viagem.digito") select obj.value).FirstOrDefault();
            }
            else
            {
                mensagemRetorno = "Ocorreu uma falha ao enviar o CIOT para a Pamcard.";
            }

            ciotIntegracaoArquivo.Mensagem = $"{codigoRetorno} - {mensagemRetorno}";

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(ciot);

            if (codigoRetorno == "0")
            {
                ciot.Numero = numeroCIOT;
                ciot.ProtocoloAutorizacao = codigoViagem;
                ciot.Mensagem = "CIOT processado com sucesso.";
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                ciot.CodigoVerificador = codigoVerificador;
                ciot.Digito = digito;

                repCIOT.Atualizar(ciot);

                return true;
            }
            else
            {
                mensagemErro = $"{codigoRetorno} - {mensagemRetorno}";

                return false;
            }
        }

        private bool IntegrarEncerramentoContratoFrete(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            if (configuracao.AjustarSaldoVencimentoDataEncerramento)
            {
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, unitOfWork);

                if (!IntegrarAtualizacaoContratoFrete(ciot, modalidade, configuracao, unitOfWork, out mensagemErro, true))
                    return false;
            }

            mensagemErro = null;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            ServicoPamCard.execute execute = new ServicoPamCard.execute()
            {
                arg0 = new ServicoPamCard.requestTO()
                {
                    context = "CloseFreightContract",
                    fields = ObterCamposEncerramentoContratoFrete(ciot, configuracao)
                }
            };

            ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(configuracao.Matriz, unitOfWork, out Servicos.Models.Integracao.InspectorBehavior inspector);

            ServicoPamCard.executeResponse retorno = null;

            try
            {
                retorno = svcPamCard.execute(execute);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao enviar para a Pamcard: " + ex.Message);
            }

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

            ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
            ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
            ciotIntegracaoArquivo.Data = DateTime.Now;
            ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

            string codigoRetorno = string.Empty, mensagemRetorno = string.Empty, numeroCIOT = string.Empty, codigoViagem = string.Empty;

            if (retorno != null)
            {
                codigoRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.codigo") select obj.value).FirstOrDefault();
                mensagemRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.descricao") select obj.value).FirstOrDefault();
            }
            else
            {
                mensagemRetorno = "Ocorreu uma falha ao enviar o encerramento do CIOT para a Pamcard.";
            }

            ciotIntegracaoArquivo.Mensagem = $"{codigoRetorno} - {mensagemRetorno}";

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(ciot);

            if (codigoRetorno == "0")
            {
                return true;
            }
            else
            {
                mensagemErro = $"{codigoRetorno} - {mensagemRetorno}";

                return false;
            }
        }

        private bool IntegrarCancelamentoContratoFrete(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            ServicoPamCard.execute execute = new ServicoPamCard.execute()
            {
                arg0 = new ServicoPamCard.requestTO()
                {
                    context = "CancelTrip",
                    fields = ObterCamposCancelamentoContratoFrete(ciot, configuracao)
                }
            };

            ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(configuracao.Matriz, unitOfWork, out Servicos.Models.Integracao.InspectorBehavior inspector);

            ServicoPamCard.executeResponse retorno = null;

            try
            {
                retorno = svcPamCard.execute(execute);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao enviar para a Pamcard: " + ex.Message);
            }

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

            ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
            ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
            ciotIntegracaoArquivo.Data = DateTime.Now;
            ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

            string codigoRetorno = string.Empty, mensagemRetorno = string.Empty;

            if (retorno != null)
            {
                codigoRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.codigo") select obj.value).FirstOrDefault();
                mensagemRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.descricao") select obj.value).FirstOrDefault();
            }
            else
            {
                mensagemRetorno = "Ocorreu uma falha ao enviar o cancelamento do CIOT para a Pamcard.";
            }

            ciotIntegracaoArquivo.Mensagem = $"{codigoRetorno} - {mensagemRetorno}";

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);
            ciot.MotivoCancelamento = "CANCELAMENTO GERADO PELO OPERADOR";

            repCIOT.Atualizar(ciot);

            if (codigoRetorno == "0")
            {
                return true;
            }
            else
            {
                mensagemErro = $"{codigoRetorno} - {mensagemRetorno}";

                return false;
            }
        }

        private bool IntegrarAtualizacaoContratoFrete(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, bool encerramentoCIOT, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = null, decimal valorMovimento = 0)
        {
            mensagemErro = null;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            ServicoPamCard.execute execute = new ServicoPamCard.execute()
            {
                arg0 = new ServicoPamCard.requestTO()
                {
                    context = "UpdateValuesFreightContract",
                    fields = ObterCamposAtualizacaoContratoFrete(ciot, modalidade, configuracao, unitOfWork, encerramentoCIOT, justificativa, valorMovimento)
                }
            };

            Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();
            ServicoPamCard.executeResponse retorno = null;

            try
            {
                ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(configuracao.Matriz, unitOfWork, out inspector);

                retorno = svcPamCard.execute(execute);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao enviar para a Pamcard: " + ex.Message);
            }

            string codigoRetorno = string.Empty, mensagemRetorno = string.Empty, numeroCIOT = string.Empty, codigoViagem = string.Empty;

            if (retorno != null)
            {
                codigoRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.codigo") select obj.value).FirstOrDefault();
                mensagemRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.descricao") select obj.value).FirstOrDefault();
            }
            else
            {
                mensagemRetorno = "Ocorreu uma falha ao enviar a atualização do CIOT para a Pamcard.";
            }

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                Mensagem = $"{codigoRetorno} - {mensagemRetorno}"
            };

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(ciot);

            if (codigoRetorno == "0")
            {
                return true;
            }
            else
            {
                mensagemErro = $"{codigoRetorno} - {mensagemRetorno}";

                return false;
            }
        }

        private bool IntegrarCartaoPortadorFrete(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            mensagemErro = null;

            if (!configuracao.AssociarCartaoMotoristaTransportador)
                return true;

            if (!tipoPagamentoCIOT.HasValue || tipoPagamentoCIOT.Value != TipoPagamentoCIOT.Cartao)
                return true;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            ServicoPamCard.execute execute = new ServicoPamCard.execute()
            {
                arg0 = new ServicoPamCard.requestTO()
                {
                    context = "InsertCardFreight",
                    fields = ObterCamposCartaoPortadorFrete(ciot, modalidade, configuracao, unitOfWork)
                }
            };

            ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(configuracao.Matriz, unitOfWork, out Servicos.Models.Integracao.InspectorBehavior inspector);

            ServicoPamCard.executeResponse retorno = null;

            try
            {
                retorno = svcPamCard.execute(execute);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao enviar para a Pamcard: " + ex.Message);
            }

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento
            };

            string codigoRetorno = string.Empty, mensagemRetorno = string.Empty;

            if (retorno != null)
            {
                codigoRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.codigo") select obj.value).FirstOrDefault();
                mensagemRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.descricao") select obj.value).FirstOrDefault();
            }
            else
                mensagemRetorno = "Ocorreu uma falha ao enviar a inclusão de cartão para a Pamcard.";

            ciotIntegracaoArquivo.Mensagem = $"{codigoRetorno} - {mensagemRetorno}";

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(ciot);

            if (codigoRetorno == "0")
                return true;
            else
            {
                mensagemErro = $"{codigoRetorno} - {mensagemRetorno}";

                return false;
            }
        }

        private Dominio.Entidades.Embarcador.CIOT.CIOTPamcard ObterConfiguracaoPamcard(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.CIOT.CIOTPamcard repCIOTPamcard = new Repositorio.Embarcador.CIOT.CIOTPamcard(unidadeTrabalho);

            Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao = repCIOTPamcard.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);

            return configuracao;
        }

        private ServicoPamCard.WSTransacionalClient ObterClientPamCard(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, out Servicos.Models.Integracao.InspectorBehavior inspectorBehavior)
        {
            ServicoPamCard.WSTransacionalClient svcPamCard = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoPamCard.WSTransacionalClient, ServicoPamCard.WSTransacional>(TipoWebServiceIntegracao.Pamcard_WSTransacional, out inspectorBehavior);

            svcPamCard.ClientCredentials.ClientCertificate.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(empresa.NomeCertificado, empresa.SenhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);

            return svcPamCard;
        }

        private (string DDD, string Numero) ObterTelefone(string telefoneCompleto)
        {
            telefoneCompleto = Utilidades.String.OnlyNumbers(telefoneCompleto);

            if (string.IsNullOrWhiteSpace(telefoneCompleto) || telefoneCompleto.Length <= 5)
                return ValueTuple.Create(string.Empty, string.Empty);

            string ddd;
            string telefone;

            if (telefoneCompleto.StartsWith("0"))
            {
                ddd = telefoneCompleto.Substring(1, 2);
                telefone = telefoneCompleto.Substring(3, telefoneCompleto.Length - 3);
            }
            else
            {
                ddd = telefoneCompleto.Substring(0, 2);
                telefone = telefoneCompleto.Substring(2, telefoneCompleto.Length - 2);
            }

            return ValueTuple.Create(ddd, telefone);
        }

        #endregion

        #region Métodos Privados - Campos

        private ServicoPamCard.fieldTO[] ObterCamposAberturaContratoFrete(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();
            //decimal valorPedagio = 10m;
            decimal pesoBruto = 10m;
            decimal tarifaSaque = configuracao.ConfiguracaoCIOT.TarifaSaque;
            decimal tarifaTransferencia = configuracao.ConfiguracaoCIOT.TarifaTransferencia;
            decimal valorBruto = tarifaSaque + tarifaTransferencia;

            if (valorBruto <= 0m)
                valorBruto = 1m;

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = configuracao.Matriz?.CNPJ_SemFormato ?? ciot.Contratante.CNPJ });

            if (ciot.Contratante != null && configuracao.Matriz != null && ciot.Contratante.Codigo != configuracao.Matriz.Codigo)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = ciot.Contratante.CNPJ });
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contrato.numero", value = ciot.Codigo.ToString() });

            campos.AddRange(ObterCamposVeiculos(ciot, unitOfWork));
            campos.AddRange(ObterCamposFavorecidos(ciot, modalidade, tipoPagamentoCIOT));

            DateTime dataPartida = ciot.DataAbertura ?? DateTime.Now;
            DateTime dataTermino = ciot.DataFinalViagem;

            if (configuracao.ConfiguracaoCIOT.UtilizarDataAtualComoInicioTerminoCIOT)
            {
                dataPartida = DateTime.Now;
                dataTermino = dataPartida.AddDays(configuracao.ConfiguracaoCIOT.DiasTerminoCIOT ?? 1);
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.data.partida", value = dataPartida.ToString("dd/MM/yyyy") });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.data.termino", value = dataTermino.ToString("dd/MM/yyyy") });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.origem.cidade.ibge", value = string.Format("{0:0000000}", ciot.Transportador.Localidade.CodigoIBGE) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.destino.cidade.ibge", value = string.Format("{0:0000000}", ciot.Transportador.Localidade.CodigoIBGE) });

            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.valor", value = valorPedagio.ToString("0.00", cultura) });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.carga.natureza", value = "0207" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.carga.peso", value = pesoBruto.ToString("0.00", cultura) });

            campos.AddRange(ObterCamposDocumentos(ciot, cultura, configuracao));

            int quantidadeParcelas = 1;
            string prefixo = "viagem.parcela";

            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}{quantidadeParcelas}.efetivacao.tipo", value = "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}{quantidadeParcelas}.valor", value = "1.00" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}{quantidadeParcelas}.subtipo", value = "3" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}{quantidadeParcelas}.data", value = ciot.DataFinalViagem.ToString("dd/MM/yyyy") });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}{quantidadeParcelas}.favorecido.tipo.id", value = modalidade.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? "3" : "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}{quantidadeParcelas}.status.id", value = "2" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}{quantidadeParcelas}.numero.cliente", value = "1" });

            if (tarifaSaque > 0m || tarifaTransferencia > 0m)
            {
                quantidadeParcelas++;

                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}{quantidadeParcelas}.efetivacao.tipo", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}{quantidadeParcelas}.valor", value = (tarifaSaque + tarifaTransferencia).ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}{quantidadeParcelas}.subtipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}{quantidadeParcelas}.data", value = ciot.DataFinalViagem.ToString("dd/MM/yyyy") });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}{quantidadeParcelas}.favorecido.tipo.id", value = modalidade.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? "3" : "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}{quantidadeParcelas}.status.id", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}{quantidadeParcelas}.numero.cliente", value = "2" });
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela.qtde", value = quantidadeParcelas.ToString() });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.valor.bruto", value = valorBruto.ToString("0.00", cultura) });

            int quantidadeFrete = 0;

            if (tarifaSaque > 0m)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "315" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = tarifaSaque.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tarifa.quantidade", value = "4" });
            }

            if (tarifaTransferencia > 0m)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "316" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = tarifaTransferencia.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tarifa.quantidade", value = "4" });
            }

            if (quantidadeFrete > 0)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item.qtde", value = quantidadeFrete.ToString() });

            return campos.ToArray();
        }

        private ServicoPamCard.fieldTO[] ObterCamposContratoFrete(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = ciot.CargaCIOT.FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaCIOT.Carga.Pedidos.FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoValePedagio = cargaCIOT.ContratoFrete.TipoIntegracaoValePedagio;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOTDadosPamcard repCIOTDadosPamcard = new Repositorio.Embarcador.Documentos.CIOTDadosPamcard(unitOfWork);

            Dominio.Entidades.Embarcador.Documentos.CIOTDadosPamcard ciotDadosPamcard = repCIOTDadosPamcard.BuscarPorCIOT(ciot);

            int quantidadeParcelas = 0, quantidadeFrete = 0;
            decimal valorAdiantamento = cargaCIOT.ContratoFrete.ValorAdiantamento;
            decimal valorAbastecimento = cargaCIOT.ContratoFrete.ValorAbastecimento;
            decimal valorFrete = cargaCIOT.ContratoFrete.SaldoAReceber;
            decimal valorIRRF = cargaCIOT.ContratoFrete.ValorIRRF;
            decimal valorINSS = cargaCIOT.ContratoFrete.ValorINSS;
            decimal valorSESTSENAT = cargaCIOT.ContratoFrete.ValorSEST + cargaCIOT.ContratoFrete.ValorSENAT;
            decimal valorPedagio = cargaCIOT.ContratoFrete.ValorPedagio;
            decimal valorBruto = cargaCIOT.ContratoFrete.ValorBruto;
            decimal tarifaSaque = cargaCIOT.ContratoFrete.TarifaSaque;
            decimal tarifaTransferencia = cargaCIOT.ContratoFrete.TarifaTransferencia;

            if (cargaCIOT.ContratoFrete.ReterImpostosContratoFrete && cargaCIOT.ContratoFrete.TransportadorTerceiro?.Tipo == "F")
            {
                valorIRRF = 0m;
                valorINSS = 0m;
                valorSESTSENAT = 0m;
            }

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = configuracao.Matriz?.CNPJ ?? ciot.Contratante.CNPJ });

            if (ciot.Contratante != null && configuracao.Matriz != null && ciot.Contratante.Codigo != configuracao.Matriz.Codigo)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = ciot.Contratante.CNPJ });
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contrato.numero", value = ciot.Codigo.ToString() });

            campos.AddRange(ObterCamposVeiculos(ciot, unitOfWork));
            campos.AddRange(ObterCamposFavorecidos(ciot, modalidade, tipoPagamentoCIOT));

            DateTime dataPartida = cargaPedido.Pedido.DataPrevisaoSaida ?? DateTime.Now.AddHours(1);
            DateTime dataTermino = cargaPedido.Pedido.PrevisaoEntrega ?? dataPartida.AddDays(1);

            if (configuracao.ConfiguracaoCIOT.UtilizarDataAtualComoInicioTerminoCIOT)
            {
                dataPartida = DateTime.Now;
                dataTermino = dataPartida.AddDays(configuracao.ConfiguracaoCIOT.DiasTerminoCIOT ?? 1);
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.data.partida", value = dataPartida.ToString("dd/MM/yyyy") });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.data.termino", value = dataTermino.ToString("dd/MM/yyyy") ?? string.Empty });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.origem.cidade.ibge", value = string.Format("{0:0000000}", cargaPedido.Origem.CodigoIBGE) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.destino.cidade.ibge", value = string.Format("{0:0000000}", cargaPedido.Destino.CodigoIBGE) });

            if (valorPedagio > 0m)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.valor", value = valorPedagio.ToString("0.00", cultura) });

                if (tipoIntegracaoValePedagio != null)
                {
                    if (tipoIntegracaoValePedagio.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar)
                        campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.solucao.id", value = "4" });
                }
            }

            string ncm = string.Empty;

            if (!string.IsNullOrWhiteSpace(cargaCIOT.Carga.TipoDeCarga?.NCM) && cargaCIOT.Carga.TipoDeCarga.NCM.Length > 3)
                ncm = cargaCIOT.Carga.TipoDeCarga?.NCM?.Substring(0, 4);

            decimal peso = repPedidoXMLNotaFiscal.BuscarPesoPorCarga(cargaCIOT.Carga.Codigo);

            if (configuracao.EnviarQuantidadesMaioresQueZero)
            {
                if (peso < 1m)
                    peso = 1;
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.carga.natureza", value = ncm });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.carga.peso", value = peso.ToString("0.00", cultura) });

            campos.AddRange(ObterCamposDocumentos(ciot, cultura, configuracao));

            DateTime dataVencimentoAdiantamento = DateTime.Now.AddDays(cargaCIOT.ContratoFrete.DiasVencimentoAdiantamento);
            DateTime dataVencimentoSaldo = configuracao.UtilizarDataAtualParaDefinirVencimentoSaldo ? DateTime.Now : ciot.DataFinalViagem;

            if (!ciot.DataVencimentoAdiantamento.HasValue)
                ciot.DataVencimentoAdiantamento = dataVencimentoAdiantamento;
            else
                dataVencimentoAdiantamento = ciot.DataVencimentoAdiantamento.Value;

            if (!ciot.DataVencimentoSaldo.HasValue || configuracao.UtilizarDataAtualParaDefinirVencimentoSaldo)
                ciot.DataVencimentoSaldo = dataVencimentoSaldo;
            else
                dataVencimentoSaldo = ciot.DataVencimentoSaldo.Value;

            string prefixoParcela;

            if (valorAdiantamento > 0m)
            {
                quantidadeParcelas += 1;

                prefixoParcela = $"viagem.parcela{quantidadeParcelas}";

                string tipoEfetivacaoAdiantamento = "2";

                if (ciotDadosPamcard?.EfetivacaoAdiantamento != null)
                    tipoEfetivacaoAdiantamento = ciotDadosPamcard.EfetivacaoAdiantamento.Value.ToString("D");

                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.efetivacao.tipo", value = tipoEfetivacaoAdiantamento });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.valor", value = valorAdiantamento.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.subtipo", value = "1" });

                if (ciotDadosPamcard?.StatusAdiantamento != null)
                    campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.status.id", value = ciotDadosPamcard.StatusAdiantamento.Value.ToString("D") });

                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.data", value = dataVencimentoAdiantamento.ToString("dd/MM/yyyy") });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.favorecido.tipo.id", value = modalidade.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? "3" : "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.numero.cliente", value = "3" });
            }

            quantidadeParcelas += 1;

            prefixoParcela = $"viagem.parcela{quantidadeParcelas}";

            string tipoEfetivacaoSaldo = "2";

            if (ciotDadosPamcard?.EfetivacaoSaldo != null)
                tipoEfetivacaoSaldo = ciotDadosPamcard.EfetivacaoSaldo.Value.ToString("D");

            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.efetivacao.tipo", value = tipoEfetivacaoSaldo });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.valor", value = valorFrete.ToString("0.00", cultura) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.subtipo", value = "3" });

            if (ciotDadosPamcard?.StatusSaldo != null)
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.status.id", value = ciotDadosPamcard.StatusSaldo.Value.ToString("D") });

            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.data", value = dataVencimentoSaldo.ToString("dd/MM/yyyy") });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.favorecido.tipo.id", value = modalidade.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? "3" : "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.numero.cliente", value = "2" });

            if (valorAbastecimento > 0m)
            {
                quantidadeParcelas += 1;

                prefixoParcela = $"viagem.parcela{quantidadeParcelas}";

                string tipoEfetivacaoAbastecimento = "2";
                string statusAbastecimento = "2";

                if (ciotDadosPamcard?.EfetivacaoAbastecimento != null)
                    tipoEfetivacaoAbastecimento = ciotDadosPamcard.EfetivacaoAbastecimento.Value.ToString("D");

                if (ciotDadosPamcard?.StatusAbastecimento != null)
                    statusAbastecimento = ciotDadosPamcard.StatusAbastecimento.Value.ToString("D");

                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.efetivacao.tipo", value = tipoEfetivacaoAbastecimento });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.valor", value = valorAbastecimento.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.subtipo", value = "5" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.status.id", value = statusAbastecimento });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.data", value = dataVencimentoAdiantamento.ToString("dd/MM/yyyy") });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.favorecido.tipo.id", value = modalidade.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? "3" : "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixoParcela}.numero.cliente", value = "4" });
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela.qtde", value = quantidadeParcelas.ToString() });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.valor.bruto", value = valorBruto.ToString("0.00", cultura) });

            if (valorIRRF > 0m)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = valorIRRF.ToString("0.00", cultura) });
            }

            if (valorINSS > 0m)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = valorINSS.ToString("0.00", cultura) });
            }

            if (valorSESTSENAT > 0m)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "3" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = valorSESTSENAT.ToString("0.00", cultura) });
            }

            if (tarifaSaque > 0m)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "315" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = tarifaSaque.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tarifa.quantidade", value = "4" });
            }

            if (tarifaTransferencia > 0m)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "316" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = tarifaTransferencia.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tarifa.quantidade", value = "4" });
            }

            if (quantidadeFrete > 0)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item.qtde", value = quantidadeFrete.ToString() });

            return campos.ToArray();
        }

        private ServicoPamCard.fieldTO[] ObterCamposEncerramentoContratoFrete(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            int quantidadeFrete = 0;
            decimal valorBruto = ciot.CargaCIOT.Sum(o => o.ContratoFrete.ValorBrutoComAcrescimoDescontoSaldo);
            decimal valorSESTSENAT = ciot.CargaCIOT.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorSEST + o.ContratoFrete.ValorSENAT);
            decimal valorINSS = ciot.CargaCIOT.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorINSS);
            decimal valorIRRF = ciot.CargaCIOT.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorIRRF);

            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = configuracao.Matriz?.CNPJ ?? ciot.Contratante.CNPJ });

            //if (configuracao.Matriz != null)
            //{
            //    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });
            //    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = ciot.Contratante.CNPJ });
            //}

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id", value = ciot.ProtocoloAutorizacao });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.antt.ciot.numero", value = ciot.Numero });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.valor.bruto", value = valorBruto.ToString("0.00", cultura) });

            if (valorIRRF > 0m)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = valorIRRF.ToString("0.00", cultura) });
            }

            if (valorINSS > 0m)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = valorINSS.ToString("0.00", cultura) });
            }

            if (valorSESTSENAT > 0m)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "3" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = valorSESTSENAT.ToString("0.00", cultura) });
            }

            if (quantidadeFrete > 0)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item.qtde", value = quantidadeFrete.ToString() });

            return campos.ToArray();
        }

        private ServicoPamCard.fieldTO[] ObterCamposCancelamentoContratoFrete(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id", value = ciot.ProtocoloAutorizacao });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = configuracao.Matriz?.CNPJ ?? ciot.Contratante.CNPJ });

            //if (configuracao.Matriz != null)
            //{
            //    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });
            //    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = ciot.Contratante.CNPJ });
            //}

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.antt.cancelamento.motivo", value = "CANCELAMENTO GERADO PELO OPERADOR" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.antt.ciot.numero", value = ciot.Numero });

            return campos.ToArray();
        }

        
        private ServicoPamCard.fieldTO[] ObterCamposAtualizacaoContratoFrete(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao, Repositorio.UnitOfWork unitOfWork, bool encerramentoCIOT, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valorMovimento)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            Repositorio.Embarcador.Documentos.CIOTDadosPamcard repCIOTDadosPamcard = new Repositorio.Embarcador.Documentos.CIOTDadosPamcard(unitOfWork);

            Dominio.Entidades.Embarcador.Documentos.CIOTDadosPamcard ciotDadosPamcard = repCIOTDadosPamcard.BuscarPorCIOT(ciot);

            int quantidadeParcelas = 0, quantidadeFrete = 0;
            decimal valorAbastecimento = ciot.CargaCIOT.Sum(o => o.ContratoFrete.ValorAbastecimento);
            decimal valorAdiantamento = ciot.CargaCIOT.Sum(o => o.ContratoFrete.ValorAdiantamento);
            decimal valorFrete = ciot.CargaCIOT.Sum(o => o.ContratoFrete.SaldoAReceber);
            decimal valorBruto = ciot.CargaCIOT.Sum(o => o.ContratoFrete.ValorBruto);
            decimal tarifaSaque = ciot.CargaCIOT.Sum(o => o.ContratoFrete.TarifaSaque);
            decimal tarifaTransferencia = ciot.CargaCIOT.Sum(o => o.ContratoFrete.TarifaTransferencia);
            decimal valorIRRF = ciot.CargaCIOT.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorIRRF);
            decimal valorINSS = ciot.CargaCIOT.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorINSS);
            decimal valorSESTSENAT = ciot.CargaCIOT.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorSEST + o.ContratoFrete.ValorSENAT);

            int diasVencimentoAdiantamento = ciot.CargaCIOT.Select(o => o.ContratoFrete.DiasVencimentoAdiantamento).FirstOrDefault();
            // int diasVencimentoSaldo = ciot.CargaCIOT.Select(o => o.ContratoFrete.DiasVencimentoSaldo).FirstOrDefault();
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato = ciot.CargaCIOT.Select(o => o.ContratoFrete).FirstOrDefault();

            DateTime dataVencimentoSaldo = configuracao.UtilizarDataAtualParaDefinirVencimentoSaldo ? DateTime.Now : Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro.ObterVencimentoSaldoContrato(contrato, ciot.DataFinalViagem);

            if (!ciot.DataVencimentoSaldo.HasValue || configuracao.UtilizarDataAtualParaDefinirVencimentoSaldo)
                ciot.DataVencimentoSaldo = dataVencimentoSaldo;
            else
                dataVencimentoSaldo = ciot.DataVencimentoSaldo.Value;

            string prefixo = null;

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id", value = ciot.ProtocoloAutorizacao });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = configuracao.Matriz?.CNPJ ?? ciot.Contratante.CNPJ });

            if (ciot.Contratante != null && configuracao.Matriz != null && ciot.Contratante.Codigo != configuracao.Matriz.Codigo)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = ciot.Contratante.CNPJ });
            }

            if (ciot.CargaCIOT.Count() == 1 && !encerramentoCIOT && justificativa == null) //adiciona esta parcela com valor zerado para remover o saldo adicionado na abertura do CIOT (apenas na primeira integração de atualização)
            {
                quantidadeParcelas += 1;

                prefixo = "viagem.parcela" + quantidadeParcelas.ToString();

                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.efetivacao.tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.valor", value = "0" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.subtipo", value = "3" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.data", value = ciot.DataFinalViagem.ToString("dd/MM/yyyy") });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.favorecido.tipo.id", value = modalidade.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? "3" : "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.status.id", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.numero.cliente", value = "1" });
            }

            if (valorAdiantamento > 0m)
            {
                if (!ciot.DataVencimentoAdiantamento.HasValue || configuracao.UtilizarDataAtualParaDefinirVencimentoAdiantamento)
                    ciot.DataVencimentoAdiantamento = configuracao.UtilizarDataAtualParaDefinirVencimentoAdiantamento ? DateTime.Now : ciot.DataAbertura?.AddDays(diasVencimentoAdiantamento);

                quantidadeParcelas += 1;

                prefixo = "viagem.parcela" + quantidadeParcelas.ToString();

                string tipoEfetivacaoAdiantamento = "2";

                if (ciotDadosPamcard?.EfetivacaoAdiantamento != null)
                    tipoEfetivacaoAdiantamento = ciotDadosPamcard?.EfetivacaoAdiantamento.Value.ToString("D");

                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.efetivacao.tipo", value = tipoEfetivacaoAdiantamento });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.valor", value = valorAdiantamento.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.subtipo", value = "1" });

                if (ciotDadosPamcard?.StatusAdiantamento != null)
                    campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.status.id", value = ciotDadosPamcard?.StatusAdiantamento.Value.ToString("D") });

                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.data", value = ciot.DataVencimentoAdiantamento?.ToString("dd/MM/yyyy") });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.favorecido.tipo.id", value = modalidade.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? "3" : "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.numero.cliente", value = "3" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.status.id", value = "2" });
            }

            quantidadeParcelas += 1;

            if (configuracao.AjustarSaldoVencimentoDataEncerramento && encerramentoCIOT)
                dataVencimentoSaldo = Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro.ObterVencimentoSaldoContrato(contrato);

            prefixo = "viagem.parcela" + quantidadeParcelas.ToString();

            string tipoEfetivacaoSaldo = "2";

            if (ciotDadosPamcard?.EfetivacaoSaldo != null)
                tipoEfetivacaoSaldo = ciotDadosPamcard?.EfetivacaoSaldo.Value.ToString("D");

            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.efetivacao.tipo", value = tipoEfetivacaoSaldo });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.valor", value = valorFrete.ToString("0.00", cultura) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.subtipo", value = "3" });

            if (ciotDadosPamcard?.StatusSaldo != null)
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.status.id", value = ciotDadosPamcard?.StatusSaldo.Value.ToString("D") });

            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.data", value = dataVencimentoSaldo.ToString("dd/MM/yyyy") });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.favorecido.tipo.id", value = modalidade.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? "3" : "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.numero.cliente", value = "2" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.status.id", value = "2" });

            if (valorAbastecimento > 0m)
            {
                if (!ciot.DataVencimentoAbastecimento.HasValue)
                    ciot.DataVencimentoAbastecimento = ciot.DataAbertura?.AddDays(diasVencimentoAdiantamento);

                quantidadeParcelas += 1;

                prefixo = "viagem.parcela" + quantidadeParcelas.ToString();

                string tipoEfetivacaoAbastecimento = "2";
                string statusAbastecimento = "2";

                if (ciotDadosPamcard?.EfetivacaoAbastecimento != null)
                    tipoEfetivacaoAbastecimento = ciotDadosPamcard?.EfetivacaoAbastecimento.Value.ToString("D");

                if (ciotDadosPamcard?.StatusAbastecimento != null)
                    statusAbastecimento = ciotDadosPamcard?.StatusAbastecimento.Value.ToString("D");

                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.efetivacao.tipo", value = tipoEfetivacaoAbastecimento });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.valor", value = valorAbastecimento.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.subtipo", value = "5" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.status.id", value = statusAbastecimento });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.data", value = ciot.DataVencimentoAbastecimento?.ToString("dd/MM/yyyy") });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.favorecido.tipo.id", value = modalidade.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? "3" : "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.numero.cliente", value = "4" });
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela.qtde", value = quantidadeParcelas.ToString() });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.valor.bruto", value = valorBruto.ToString("0.00", cultura) });

            if (valorIRRF > 0m)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.frete.item{quantidadeFrete}.tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.frete.item{quantidadeFrete}.valor", value = valorIRRF.ToString("0.00", cultura) });
            }

            if (valorINSS > 0m)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.frete.item{quantidadeFrete}.tipo", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.frete.item{quantidadeFrete}.valor", value = valorINSS.ToString("0.00", cultura) });
            }

            if (valorSESTSENAT > 0m)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.frete.item{quantidadeFrete}.tipo", value = "3" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.frete.item{quantidadeFrete}.valor", value = valorSESTSENAT.ToString("0.00", cultura) });
            }

            if (tarifaSaque > 0m)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "315" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = tarifaSaque.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tarifa.quantidade", value = "4" });
            }

            if (tarifaTransferencia > 0m)
            {
                quantidadeFrete += 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tipo", value = "316" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".valor", value = tarifaTransferencia.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item" + quantidadeFrete.ToString() + ".tarifa.quantidade", value = "4" });
            }

            if (quantidadeFrete > 0m)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.frete.item.qtde", value = quantidadeFrete.ToString() });

            return campos.ToArray();
        }

        private ServicoPamCard.fieldTO[] ObterCamposAutorizacaoPagamentoContratoFrete(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela? tipoAutorizacaoPagamentoCIOTParcela = null)
        {
            Repositorio.Embarcador.Documentos.CIOTDadosPamcard repCIOTDadosPamcard = new Repositorio.Embarcador.Documentos.CIOTDadosPamcard(unitOfWork);

            Dominio.Entidades.Embarcador.Documentos.CIOTDadosPamcard ciotDadosPamcard = repCIOTDadosPamcard.BuscarPorCIOT(ciot);

            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            int quantidadeParcelas = 0;
            decimal valorAbastecimento = ciot.CargaCIOT.Sum(o => o.ContratoFrete.ValorAbastecimento);
            decimal valorAdiantamento = ciot.CargaCIOT.Sum(o => o.ContratoFrete.ValorAdiantamento);

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id", value = ciot.ProtocoloAutorizacao });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = configuracao.Matriz?.CNPJ ?? ciot.Contratante.CNPJ });

            if (ciot.Contratante != null && configuracao.Matriz != null && ciot.Contratante.Codigo != configuracao.Matriz.Codigo)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = ciot.Contratante.CNPJ });
            }

            string prefixo = string.Empty;

            if (valorAdiantamento > 0m && (!tipoAutorizacaoPagamentoCIOTParcela.HasValue || tipoAutorizacaoPagamentoCIOTParcela == TipoAutorizacaoPagamentoCIOTParcela.Adiantamento))
            {
                if (ciotDadosPamcard == null || !ciotDadosPamcard.EfetivacaoAdiantamento.HasValue || ciotDadosPamcard.EfetivacaoAdiantamento.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao.Manual)
                {
                    quantidadeParcelas += 1;

                    prefixo = "viagem.parcela" + quantidadeParcelas.ToString();

                    campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.numero.cliente", value = "3" });
                }
            }

            if (!tipoAutorizacaoPagamentoCIOTParcela.HasValue || tipoAutorizacaoPagamentoCIOTParcela == TipoAutorizacaoPagamentoCIOTParcela.Saldo)
            {
                if (ciotDadosPamcard == null || !ciotDadosPamcard.EfetivacaoSaldo.HasValue || ciotDadosPamcard.EfetivacaoSaldo.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao.Manual)
                {
                    quantidadeParcelas += 1;

                    prefixo = "viagem.parcela" + quantidadeParcelas.ToString();

                    campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.numero.cliente", value = "2" });
                }
            }

            if (valorAbastecimento > 0m && (!tipoAutorizacaoPagamentoCIOTParcela.HasValue || tipoAutorizacaoPagamentoCIOTParcela == TipoAutorizacaoPagamentoCIOTParcela.Abastecimento))
            {
                if (ciotDadosPamcard == null || !ciotDadosPamcard.EfetivacaoAbastecimento.HasValue || ciotDadosPamcard.EfetivacaoAbastecimento.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao.Manual)
                {
                    quantidadeParcelas += 1;

                    prefixo = "viagem.parcela" + quantidadeParcelas.ToString();

                    campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.numero.cliente", value = "4" });
                }
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela.qtde", value = quantidadeParcelas.ToString() });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.antt.ciot.numero", value = ciot.Numero });

            return campos.ToArray();
        }

        private ServicoPamCard.fieldTO[] ObterCamposAutorizacaoPagamentoContratoFreteAcrescimoDesconto(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id", value = ciot.ProtocoloAutorizacao });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = configuracao.Matriz?.CNPJ ?? ciot.Contratante.CNPJ });

            if (ciot.Contratante != null && configuracao.Matriz != null && ciot.Contratante.Codigo != configuracao.Matriz.Codigo)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = ciot.Contratante.CNPJ });
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela1.numero.cliente", value = "3" });//Definição da parcela de adiantamento
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela.qtde", value = "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.antt.ciot.numero", value = ciot.Numero });

            return campos.ToArray();
        }

        private ServicoPamCard.fieldTO[] ObterCamposCartaoPortadorFrete(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = configuracao.Matriz?.CNPJ ?? ciot.Contratante.CNPJ });

            if (ciot.Contratante != null && configuracao.Matriz != null && ciot.Contratante.Codigo != configuracao.Matriz.Codigo)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = ciot.Contratante.CNPJ });
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.numero", value = modalidade.TipoFavorecidoCIOT == TipoFavorecidoCIOT.Motorista ? ciot.Motorista.NumeroCartao : modalidade.NumeroCartao });

            campos.AddRange(ObterCamposCartaoPortador(ciot.Motorista));

            if (modalidade.TipoFavorecidoCIOT == TipoFavorecidoCIOT.Transportador)
                campos.AddRange(ObterCamposCartaoEmpresa(ciot.Transportador, modalidade));

            return campos.ToArray();
        }

        private List<ServicoPamCard.fieldTO> ObterCamposCartaoPortador(Dominio.Entidades.Usuario motorista)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.documento.tipo", value = "2" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.documento.numero", value = motorista.CPF });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.rg", value = motorista.RG });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.uf.rg", value = motorista.EstadoRG?.Sigla ?? string.Empty });

            if (motorista.OrgaoEmissorRG.HasValue && motorista.OrgaoEmissorRG.Value != Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG.Nenhum)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.rg.emissor.id", value = motorista.OrgaoEmissorRG.Value.ToString("d") });
            if (motorista.DataEmissaoRG.HasValue)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.rg.emissao.data", value = motorista.DataEmissaoRG.Value.ToString("dd/MM/yyyy") });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.nome", value = Utilidades.String.Left(motorista.Nome, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.data.nascimento", value = motorista.DataNascimento?.ToString("dd/MM/yyyy") ?? string.Empty });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.nacionalidade.id", value = "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.naturalidade.ibge", value = string.Format("{0:0000000}", motorista.Localidade?.CodigoIBGE ?? 0) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.sexo", value = motorista.Sexo == Dominio.ObjetosDeValor.Enumerador.Sexo.Feminino ? "F" : "M" });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.endereco.logradouro", value = Utilidades.String.Left(motorista.Endereco, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.endereco.numero", value = "0" });
            if (!string.IsNullOrWhiteSpace(motorista.Complemento))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.endereco.complemento", value = Utilidades.String.Left(motorista.Complemento, 15) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.endereco.bairro", value = Utilidades.String.Left(motorista.Bairro, 30) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.endereco.cidade", value = Utilidades.String.Left(motorista.Localidade?.Descricao ?? string.Empty, 30) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.endereco.uf", value = motorista.Localidade?.Estado.Sigla ?? string.Empty });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.endereco.pais", value = "BRASIL" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.endereco.cep", value = Utilidades.String.OnlyNumbers(motorista.CEP) });

            if (motorista.TipoResidencia.HasValue && motorista.TipoResidencia.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoResidencia.Nenhum)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.endereco.propriedade.tipo.id", value = ((int)motorista.TipoResidencia).ToString() });

            (string DDD, string Numero) telefone = ObterTelefone(motorista.Telefone);
            if (!string.IsNullOrWhiteSpace(telefone.Numero))
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.telefone.ddd", value = "0" + telefone.DDD });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.telefone.numero", value = telefone.Numero });
            }

            if (!string.IsNullOrWhiteSpace(motorista.Email))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.portador.email", value = motorista.Email });

            return campos;
        }

        private List<ServicoPamCard.fieldTO> ObterCamposCartaoEmpresa(Dominio.Entidades.Cliente transportador, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.empresa.nome", value = Utilidades.String.Left(transportador.Nome, 50) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.empresa.cnpj", value = transportador.CPF_CNPJ_SemFormato });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.cartao.empresa.rntrc", value = modalidade.RNTRC });

            return campos;
        }

        private List<ServicoPamCard.fieldTO> ObterCamposFavorecidos(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            int quantidade = 1;

            if (!modalidade.TipoFavorecidoCIOT.HasValue || modalidade.TipoFavorecidoCIOT.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista)
                quantidade = 2;

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido.qtde", value = quantidade.ToString() });

            if (!modalidade.TipoFavorecidoCIOT.HasValue || modalidade.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista)
                campos.AddRange(ObterCamposMotorista(ciot, modalidade, tipoPagamentoCIOT));

            campos.AddRange(ObterCamposTransportador(ciot, modalidade, tipoPagamentoCIOT));

            return campos;
        }

        private List<ServicoPamCard.fieldTO> ObterCamposMotorista(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.tipo", value = "3" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento.qtde", value = "2" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento1.tipo", value = "2" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento1.numero", value = Utilidades.String.OnlyNumbers(ciot.Motorista.CPF) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento2.tipo", value = "3" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento2.numero", value = Utilidades.String.OnlyNumbers(ciot.Motorista.RG) });

            if (ciot.Motorista.EstadoRG != null)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento2.uf", value = ciot.Motorista.EstadoRG.Sigla });

            if (ciot.Motorista.OrgaoEmissorRG != null)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento2.emissor.id", value = ciot.Motorista.OrgaoEmissorRG.Value.ToString("d") });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.nome", value = Utilidades.String.Left(ciot.Motorista.Nome, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.nacionalidade.id", value = "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.naturalidade.ibge", value = string.Format("{0:0000000}", ciot.Motorista.Localidade?.CodigoIBGE ?? 0) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.sexo", value = "M" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.logradouro", value = Utilidades.String.Left(ciot.Motorista.Endereco, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.numero", value = "0" });

            if (!string.IsNullOrWhiteSpace(ciot.Motorista.Complemento))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.complemento", value = Utilidades.String.Left(ciot.Motorista.Complemento, 15) });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.bairro", value = Utilidades.String.Left(ciot.Motorista.Bairro, 30) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.cidade.ibge", value = string.Format("{0:0000000}", ciot.Motorista.Localidade?.CodigoIBGE ?? 0) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.cep", value = Utilidades.String.OnlyNumbers(ciot.Motorista.CEP) });

            if (ciot.Motorista.TipoResidencia.HasValue && ciot.Motorista.TipoResidencia.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoResidencia.Nenhum)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.propriedade.tipo.id", value = ((int)ciot.Motorista.TipoResidencia).ToString() });

            string telefoneCompleto = Utilidades.String.OnlyNumbers(ciot.Motorista.Telefone);

            if (!string.IsNullOrWhiteSpace(telefoneCompleto) && telefoneCompleto.Length > 5)
            {
                string ddd;
                string telefone;

                if (telefoneCompleto.StartsWith("0"))
                {
                    ddd = telefoneCompleto.Substring(1, 2);
                    telefone = telefoneCompleto.Substring(3, telefoneCompleto.Length - 3);
                }
                else
                {
                    ddd = telefoneCompleto.Substring(0, 2);
                    telefone = telefoneCompleto.Substring(2, telefoneCompleto.Length - 2);
                }

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.telefone.ddd", value = "0" + ddd });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.telefone.numero", value = telefone });
            }

            if (!string.IsNullOrWhiteSpace(ciot.Motorista.Email))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.email", value = ciot.Motorista.Email });

            if (tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.meio.pagamento", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.cartao.numero", value = ciot.Motorista.NumeroCartao });
            }
            else if (modalidade.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Transportador)
            {
                if (ciot.Transportador.Banco != null && !string.IsNullOrWhiteSpace(ciot.Transportador.NumeroConta) && !string.IsNullOrWhiteSpace(ciot.Transportador.Agencia))
                {
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.meio.pagamento", value = "2" });
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.conta.banco", value = string.Format("{0:0000}", ciot.Transportador.Banco.Numero) });
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.conta.agencia", value = ciot.Transportador.Agencia });
                    if (!string.IsNullOrWhiteSpace(ciot.Transportador.DigitoAgencia))
                        campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.conta.agencia.digito", value = ciot.Transportador.DigitoAgencia });
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.conta.numero", value = ciot.Transportador.NumeroConta });
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.conta.tipo", value = ciot.Transportador.TipoContaBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Poupança ? "2" : "1" });
                }
            }

            if (ciot.Motorista.DataNascimento.HasValue)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.data.nascimento", value = ciot.Motorista.DataNascimento?.ToString("dd/MM/yyyy") ?? string.Empty });

            return campos;
        }

        private List<ServicoPamCard.fieldTO> ObterCamposTransportador(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            string prefixo = "viagem.favorecido1";

            if (!modalidade.TipoFavorecidoCIOT.HasValue || modalidade.TipoFavorecidoCIOT.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista)
                prefixo = "viagem.favorecido2";

            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.tipo", value = "1" });

            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.documento1.tipo", value = ciot.Transportador.Tipo == "F" ? "2" : "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.documento1.numero", value = Utilidades.String.OnlyNumbers(ciot.Transportador.CPF_CNPJ_SemFormato) });

            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.documento2.tipo", value = "6" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.documento2.numero", value = modalidade.RNTRC });

            int quantidadeDocumentos = 2;

            if (!string.IsNullOrWhiteSpace(ciot.Transportador.RG_Passaporte) && ciot.Transportador.EstadoRG != null)
            {
                quantidadeDocumentos++;

                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.documento3.tipo", value = "3" });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.documento3.numero", value = ciot.Transportador.RG_Passaporte });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.documento3.uf", value = ciot.Transportador.EstadoRG.Sigla });

                if (ciot.Transportador.OrgaoEmissorRG != null)
                    campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.documento3.emissor.id", value = ciot.Transportador.OrgaoEmissorRG.Value.ToString("d") });
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.documento.qtde", value = quantidadeDocumentos.ToString() });

            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.nome", value = Utilidades.String.Left(ciot.Transportador.Nome, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.nacionalidade.id", value = "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.naturalidade.ibge", value = string.Format("{0:0000000}", ciot.Transportador.Localidade?.CodigoIBGE ?? 0) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.sexo", value = ciot.Transportador.Sexo == Dominio.ObjetosDeValor.Enumerador.Sexo.Feminino ? "F" : "M" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.logradouro", value = Utilidades.String.Left(ciot.Transportador.Endereco, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.numero", value = "0" });

            if (!string.IsNullOrWhiteSpace(ciot.Transportador.Complemento))
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.complemento", value = Utilidades.String.Left(ciot.Transportador.Complemento, 15) });

            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.bairro", value = Utilidades.String.Left(ciot.Transportador.Bairro, 30) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.cidade.ibge", value = string.Format("{0:0000000}", ciot.Transportador.Localidade?.CodigoIBGE ?? 0) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.cep", value = Utilidades.String.OnlyNumbers(ciot.Transportador.CEP) });

            if (!string.IsNullOrWhiteSpace(ciot.Transportador.Telefone1))
            {
                string telefoneCompleto = Utilidades.String.OnlyNumbers(ciot.Transportador.Telefone1);
                string ddd = string.Empty;
                string telefone = string.Empty;

                if (telefoneCompleto.StartsWith("0"))
                {
                    ddd = telefoneCompleto.Substring(1, 2);
                    telefone = telefoneCompleto.Substring(3, telefoneCompleto.Length - 3);
                }
                else
                {
                    ddd = telefoneCompleto.Substring(0, 2);
                    telefone = telefoneCompleto.Substring(2, telefoneCompleto.Length - 2);
                }

                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.telefone.ddd", value = "0" + ddd });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.telefone.numero", value = telefone });
            }

            if (!string.IsNullOrWhiteSpace(ciot.Transportador.Email))
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.email", value = ciot.Transportador.Email.Split(';').FirstOrDefault() });

            if (tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Deposito)
            {
                if (ciot.Transportador.Banco != null && !string.IsNullOrWhiteSpace(ciot.Transportador.NumeroConta) && !string.IsNullOrWhiteSpace(ciot.Transportador.Agencia))
                {
                    campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.meio.pagamento", value = "2" });
                    campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.conta.banco", value = string.Format("{0:0000}", ciot.Transportador.Banco.Numero) });
                    campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.conta.agencia", value = ciot.Transportador.Agencia });
                    if (!string.IsNullOrWhiteSpace(ciot.Transportador.DigitoAgencia))
                        campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.conta.agencia.digito", value = ciot.Transportador.DigitoAgencia });
                    campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.conta.numero", value = ciot.Transportador.NumeroConta });
                    campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.conta.tipo", value = ciot.Transportador.TipoContaBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Poupança ? "2" : "1" });
                }
            }
            else
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.meio.pagamento", value = "1" });

                if (!string.IsNullOrWhiteSpace(modalidade.NumeroCartao))
                    campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.cartao.numero", value = modalidade.NumeroCartao });
                else if (ciot.Transportador.CPF_CNPJ_SemFormato == ciot.Motorista.CPF)
                    campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.cartao.numero", value = ciot.Motorista.NumeroCartao });
            }

            if (ciot.Transportador.DataNascimento.HasValue)
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.data.nascimento", value = ciot.Transportador.DataNascimento?.ToString("dd/MM/yyyy") ?? string.Empty });

            return campos;
        }

        private List<ServicoPamCard.fieldTO> ObterCamposVeiculos(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = ciot.CargaCIOT?.FirstOrDefault();

            if (ciot.Veiculo == null && cargaCIOT != null)
            {
                ciot.Veiculo = cargaCIOT.Carga.Veiculo;
                ciot.VeiculosVinculados = cargaCIOT.Carga.VeiculosVinculados.ToList();
            }

            Dominio.Entidades.Veiculo tracao = ciot.Veiculo;
            List<Dominio.Entidades.Veiculo> reboques = ciot.VeiculosVinculados.Where(o => o.Tipo == "T").ToList();

            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            if (tracao != null)
            {
                int quantidadeVeiculos = reboques?.Count ?? 0;
                string tipoVeiculo = tracao?.ModeloVeicularCarga?.TipoVeiculoPamcard;

                if (string.IsNullOrWhiteSpace(tipoVeiculo))
                    tipoVeiculo = "4";

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo.qtde", value = (1 + quantidadeVeiculos).ToString() });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo1.placa", value = tracao.Placa });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo1.rntrc", value = string.Format("{0:00000000}", tracao.RNTRC) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo.categoria", value = tipoVeiculo });

                for (var i = 0; i < quantidadeVeiculos; i++)
                {
                    Dominio.Entidades.Veiculo reboque = reboques[i];
                    string posicao = (2 + i).ToString();

                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo" + posicao + ".placa", value = reboque.Placa });
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo" + posicao + ".rntrc", value = string.Format("{0:00000000}", reboque.RNTRC) });
                }
            }

            return campos;
        }

        private List<ServicoPamCard.fieldTO> ObterCamposDocumentos(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, System.Globalization.CultureInfo cultura, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = ciot.CargaCIOT?.Select(o => o.Carga.CargaCTes).SelectMany(o => o).ToList();

            int totalCTes = cargaCTes?.Count ?? 0;

            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            if (totalCTes > 0)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento.qtde", value = totalCTes.ToString() });

                for (var i = 0; i < totalCTes; i++)
                {
                    string prefixo = "viagem.documento" + (i + 1).ToString() + ".";

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = cargaCTes[i];

                    decimal volumes = cargaCTe.CTe.Volumes;
                    decimal peso = cargaCTe.CTe.Peso;
                    decimal valorMercadoria = cargaCTe.CTe.ValorTotalMercadoria;

                    if (configuracao.EnviarQuantidadesMaioresQueZero)
                    {
                        if (volumes < 1m)
                            volumes = 1m;

                        if (peso < 1m)
                            peso = 1m;

                        if (valorMercadoria < 1m)
                            valorMercadoria = 1m;
                    }

                    campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "tipo", value = "5" });
                    campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "numero", value = cargaCTe.CTe.Numero.ToString() });
                    campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "serie", value = cargaCTe.CTe.Serie.Numero.ToString() });
                    campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "quantidade", value = volumes.ToString("0.00", cultura) });
                    campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "especie", value = "UN" });
                    campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "peso", value = peso.ToString("0.00", cultura) });
                    campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "mercadoria.valor", value = valorMercadoria.ToString("0.00", cultura) });
                    campos.Add(new ServicoPamCard.fieldTO() { key = prefixo + "pessoafiscal.qtde", value = "2" });

                    if (cargaCTe.CTe.Expedidor != null)
                        campos.AddRange(this.ObterCamposPessoaDocumento(i, 1, "1", cargaCTe.CTe.Expedidor));
                    else
                        campos.AddRange(this.ObterCamposPessoaDocumento(i, 1, "1", cargaCTe.CTe.Remetente));

                    if (cargaCTe.CTe.Recebedor != null)
                        campos.AddRange(this.ObterCamposPessoaDocumento(i, 2, "2", cargaCTe.CTe.Recebedor));
                    else
                        campos.AddRange(this.ObterCamposPessoaDocumento(i, 2, "2", cargaCTe.CTe.Destinatario));
                }
            }
            else
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento.qtde", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento1.tipo", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento1.numero", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento1.serie", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento1.quantidade", value = "1" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento1.especie", value = "UN" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento1.peso", value = "10.00" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento1.mercadoria.valor", value = "1.00" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento1.pessoafiscal.qtde", value = "2" });

                campos.AddRange(ObterCamposPessoaDocumento(0, 1, "1", ciot.Transportador));
                campos.AddRange(ObterCamposPessoaDocumento(0, 2, "2", ciot.Transportador));
            }

            return campos;
        }

        private List<ServicoPamCard.fieldTO> ObterCamposPessoaDocumento(int indiceDocumento, int indicePessoa, string tipo, Dominio.Entidades.ParticipanteCTe participante)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            string prefixo = "viagem.documento" + (indiceDocumento + 1).ToString() + ".pessoafiscal" + (indicePessoa > 0 ? indicePessoa.ToString() : "");

            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.tipo", value = tipo });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.documento.tipo", value = participante.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica ? "2" : "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.documento.numero", value = participante.CPF_CNPJ });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.nome", value = Utilidades.String.Left(participante.Nome, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.logradouro", value = Utilidades.String.Left(participante.Endereco, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.numero", value = Utilidades.String.Left(participante.Numero, 5) });

            if (!string.IsNullOrWhiteSpace(participante.Complemento))
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.complemento", value = Utilidades.String.Left(participante.Complemento, 15) });

            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.bairro", value = Utilidades.String.Left(participante.Bairro, 30) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.cidade.ibge", value = string.Format("{0:0000000}", (participante.Localidade?.CodigoIBGE ?? 9999999)) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.cep", value = !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(participante.CEP)) ? Utilidades.String.OnlyNumbers(participante.CEP) : Utilidades.String.OnlyNumbers(participante.Cliente?.CEP ?? "") });

            return campos;
        }

        private List<ServicoPamCard.fieldTO> ObterCamposPessoaDocumento(int indiceDocumento, int indicePessoa, string tipo, Dominio.Entidades.Cliente participante)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            string prefixo = "viagem.documento" + (indiceDocumento + 1).ToString() + ".pessoafiscal" + (indicePessoa > 0 ? indicePessoa.ToString() : "");

            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.tipo", value = tipo });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.documento.tipo", value = participante.Tipo == "F" ? "2" : "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.documento.numero", value = participante.CPF_CNPJ_SemFormato });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.nome", value = Utilidades.String.Left(participante.Nome, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.logradouro", value = Utilidades.String.Left(participante.Endereco, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.numero", value = Utilidades.String.Left(participante.Numero, 5) });

            if (!string.IsNullOrWhiteSpace(participante.Complemento))
                campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.complemento", value = Utilidades.String.Left(participante.Complemento, 15) });

            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.bairro", value = Utilidades.String.Left(participante.Bairro, 30) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.cidade.ibge", value = string.Format("{0:0000000}", (participante.Localidade?.CodigoIBGE ?? 9999999)) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"{prefixo}.endereco.cep", value = Utilidades.String.OnlyNumbers(participante.CEP) });

            return campos;
        }

        #endregion
    }
}
