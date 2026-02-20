using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.GestaoPallet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.GestaoPallet
{
    public sealed class MovimentacaoPallet
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet _repositorioMovimentacaoPallet;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes _configuracaoPallet;

        #endregion Atributos Privados Somente Leitura

        #region Construtores

        public MovimentacaoPallet(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
            _repositorioMovimentacaoPallet = new Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet(_unitOfWork);
        }

        #endregion Construtores

        #region Configurações

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes ObterConfiguracaoPallet()
        {
            _configuracaoPallet ??= new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(_unitOfWork).BuscarConfiguracaoPadrao();
            return _configuracaoPallet;
        }

        #endregion

        #region Métodos Private

        private Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet AdicionarMovimentacao(AdicionarMovimentacaoPallet adicionarMovimentacaoPallet, Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet)
        {
            ControleEstoquePallet servicoControleEstoquePallet = new Servicos.Embarcador.GestaoPallet.ControleEstoquePallet(_unitOfWork);

            if (adicionarMovimentacaoPallet.TipoMovimentacao == TipoEntradaSaida.Saida)
                servicoControleEstoquePallet.ValidarSaldoSuficiente(controleEstoquePallet, adicionarMovimentacaoPallet.QuantidadePallets);

            Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet = new Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet()
            {
                XMLNotaFiscal = adicionarMovimentacaoPallet.XMLNotaFiscal,
                Cliente = adicionarMovimentacaoPallet.Cliente,
                Transportador = adicionarMovimentacaoPallet.Transportador,
                Filial = adicionarMovimentacaoPallet.Filial,
                FilialDestino = adicionarMovimentacaoPallet.FilialDestino,
                Carga = adicionarMovimentacaoPallet.Carga,
                CargaPedido = adicionarMovimentacaoPallet.CargaPedido,
                QuantidadePallets = adicionarMovimentacaoPallet.QuantidadePallets,
                ResponsavelMovimentacaoPallet = adicionarMovimentacaoPallet.ResponsavelPallet,
                RegraPallet = adicionarMovimentacaoPallet.RegraPallet,
                Situacao = adicionarMovimentacaoPallet.Situacao,
                DataRecebimentoNF = adicionarMovimentacaoPallet.XMLNotaFiscal != null ? DateTime.Now : null,
                ControleEstoquePallet = controleEstoquePallet,
                Observacao = adicionarMovimentacaoPallet.Observacao,
                TipoLancamento = adicionarMovimentacaoPallet.TipoLancamento,
                TipoMovimentacao = adicionarMovimentacaoPallet.TipoMovimentacao
            };

            _repositorioMovimentacaoPallet.Inserir(movimentacaoPallet, _auditado);

            if (movimentacaoPallet.Situacao != SituacaoGestaoPallet.AguardandoAvaliacao)
            {
                servicoControleEstoquePallet.AtualizarSaldo(controleEstoquePallet, adicionarMovimentacaoPallet.QuantidadePallets, adicionarMovimentacaoPallet.TipoMovimentacao);
                servicoControleEstoquePallet.VerficarEstoqueBaixo(controleEstoquePallet);
            }

            return movimentacaoPallet;
        }

        private void CancelarMovimentacao(Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet)
        {
            ControleEstoquePallet servicoControleEstoquePallet = new ControleEstoquePallet(_unitOfWork);

            AlterarSituacao(movimentacaoPallet, SituacaoGestaoPallet.Cancelada);

            servicoControleEstoquePallet.AtualizarSaldo(movimentacaoPallet.ControleEstoquePallet, movimentacaoPallet.QuantidadePallets, movimentacaoPallet.TipoMovimentacao.ObterTipoInverso());
        }

        private void DefinirAcaoPorTipoQuebraRegra(Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet, TipoQuebraRegra tipoQuebraRegra)
        {
            if (tipoQuebraRegra == TipoQuebraRegra.ValePallet)
            {
                movimentacaoPallet.RegraPallet = RegraPallet.ValePallet;

                return;
            }

            movimentacaoPallet.ResponsavelMovimentacaoPallet = ObterResponsavelQuebraRegra(movimentacaoPallet.RegraPallet);

        }

        private bool EfetivarReservaEstoque(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, ref RegraPallet regraPallet)
        {
            if (regraPallet != RegraPallet.Estoque)
                return false;

            Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacao = _repositorioMovimentacaoPallet.BuscarReservaPorCarga(cargaPedido.Carga.Codigo);

            if (movimentacao == null)
            {
                if (!IsCFOPRemessaValida(xMLNotaFiscal))
                    throw new ServicoException("Operação da Nota diverge da regra de pallet do Cliente (Emprestimo).");

                regraPallet = RegraPallet.Emprestimo;

                return false;
            }

            if (!IsCFOPDevolucaoValida(xMLNotaFiscal))
                throw new ServicoException("Operação da Nota diverge da regra de pallet do Cliente (Estoque).");

            Repositorio.Embarcador.GestaoPallet.AgendamentoColetaPallet repositorioAgendamentoColetaPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoColetaPallet(_unitOfWork);

            Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoColetaPallet agendamentoColetaPallet = repositorioAgendamentoColetaPallet.BuscarPorCarga(cargaPedido.Carga.Codigo);

            agendamentoColetaPallet.Situacao = SituacaoAgendamentoColetaPallet.Finalizado;

            movimentacao.XMLNotaFiscal = xMLNotaFiscal;
            movimentacao.DataRecebimentoNF = DateTime.Now;
            movimentacao.CargaPedido = cargaPedido;

            AlterarSituacao(movimentacao, SituacaoGestaoPallet.Concluido);
            repositorioAgendamentoColetaPallet.Atualizar(agendamentoColetaPallet);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, movimentacao, $"Movimentação efetivada ao informar a NF na carga.", _unitOfWork);

            return true;
        }

        private int ObterQuantidadePallets(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repositorioXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(_unitOfWork);

            decimal? quantidadePallets = repositorioXMLNotaFiscalProduto.BuscarQuantidadePorNotaFiscal(xMLNotaFiscal.Codigo);

            if (quantidadePallets == null)
                throw new ServicoException("Não é possível adicionar movimentação de pallet com quantiade 0, verifique as configurações!");

            return (int)quantidadePallets;
        }

        private (RegraPallet, Dominio.Entidades.Embarcador.Filiais.Filial) ObterRegraPallet(Dominio.Entidades.Cliente cliente, DateTime dataEmissaoNotaFiscal)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCNPJ(cliente.CPF_CNPJ.ToString());

            if (filial != null)
                return (RegraPallet.Transferencia, filial);

            RegraPalletHistorico servicoRegraPalletHistorico = new RegraPalletHistorico(_unitOfWork);

            RegraPallet regraPallet = servicoRegraPalletHistorico.ObterRegraPeriodo(cliente, dataEmissaoNotaFiscal);

            if (!regraPallet.IsRegraClienteValida())
                throw new ServicoException($"A regra de pallet do Cliente {cliente.Descricao} não é valida!");

            return (regraPallet, filial);
        }

        private ResponsavelPallet ObterResponsavelQuebraRegra(RegraPallet regraPallet)
        {
            return regraPallet switch
            {
                RegraPallet.ValePallet => ResponsavelPallet.Transportador,
                RegraPallet.Devolucao => ResponsavelPallet.Cliente,
                RegraPallet.CanhotoAssinado => ResponsavelPallet.Transportador,
                _ => ResponsavelPallet.Filial,
            };
        }

        private ResponsavelPallet ObterResponsavelMovimentacaoPallet(AdicionarMovimentacaoPallet adicionarMovimentacaoPallet)
        {
            return adicionarMovimentacaoPallet.RegraPallet switch
            {
                RegraPallet.ValePallet => ResponsavelPallet.Cliente,
                RegraPallet.Devolucao => ResponsavelPallet.Transportador,
                RegraPallet.CanhotoAssinado => ResponsavelPallet.Cliente,
                RegraPallet.Estoque => ObterResponsavelPulmao(adicionarMovimentacaoPallet),
                _ => throw new ServicoException("Cliente não possui regra de pallet cadastrada!"),
            };
        }

        private ResponsavelPallet ObterResponsavelPulmao(AdicionarMovimentacaoPallet adicionarMovimentacaoPallet)
        {
            Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacao = _repositorioMovimentacaoPallet.BuscarReservaPorCarga(adicionarMovimentacaoPallet.Carga.Codigo);

            if (movimentacao != null)
            {
                if (!IsCFOPDevolucaoValida(adicionarMovimentacaoPallet.XMLNotaFiscal))
                    throw new ServicoException("Operação da Nota diverge da regra de pallet do Cliente.");

                return adicionarMovimentacaoPallet.ResponsavelPallet;
            }

            if (IsCFOPRemessaValida(adicionarMovimentacaoPallet.XMLNotaFiscal))
                throw new ServicoException("Operação da Nota diverge da regra de pallet do Cliente.");

            adicionarMovimentacaoPallet.RegraPallet = RegraPallet.Emprestimo;

            return ResponsavelPallet.Cliente;
        }

        private bool IsCFOPDevolucaoValida(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal)
        {
            return xMLNotaFiscal.CFOP == "5921" || xMLNotaFiscal.CFOP == "6921";
        }

        private bool IsCFOPRemessaValida(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal)
        {
            return xMLNotaFiscal.CFOP == "5920" || xMLNotaFiscal.CFOP == "6920";
        }

        private bool IsUtilizarControlePallets()
        {
            return ObterConfiguracaoPallet().UtilizarControlePaletesPorCliente;
        }

        private bool IsCargaOrigemDevolucao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            bool isCargaDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(_unitOfWork).ExisteDevolucaoPorCarga(carga.Codigo);

            return isCargaDevolucao;
        }

        private void RealizarMudancaResponsavel(Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet, string origemAuditoria)
        {
            if (movimentacaoPallet.RegraPallet == RegraPallet.Transferencia)
            {
                movimentacaoPallet.Initialize();

                movimentacaoPallet.ResponsavelMovimentacaoPallet = ResponsavelPallet.Filial;
                AlterarSituacao(movimentacaoPallet, SituacaoGestaoPallet.Concluido);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, movimentacaoPallet, movimentacaoPallet.GetChanges(), $"Transferência concluída ao confirmar {origemAuditoria}.", _unitOfWork);

                AdicionarMovimentacaoAguardandoValidacao(movimentacaoPallet);

                return;
            }

            if (!movimentacaoPallet.RegraPallet.IsRegraCIF())
                return;

            movimentacaoPallet.Initialize();

            AdicionarMovimentacaoPallet adicionarMovimentacaoPallet = new AdicionarMovimentacaoPallet()
            {
                RegraPallet = movimentacaoPallet.RegraPallet,
            };

            movimentacaoPallet.ResponsavelMovimentacaoPallet = ObterResponsavelMovimentacaoPallet(adicionarMovimentacaoPallet);

            _repositorioMovimentacaoPallet.Atualizar(movimentacaoPallet);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, movimentacaoPallet, movimentacaoPallet.GetChanges(), $"Alterado responsável ao confirmar {origemAuditoria}.", _unitOfWork);
        }

        #endregion Métodos Private

        #region Métodos Públicos

        public void AdicionarMovimentacaoPalletAutomatico(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, int? quantidadePallets = null)
        {
            if (!IsUtilizarControlePallets() || (xMLNotaFiscal?.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet) || IsCargaOrigemDevolucao(cargaPedido.Carga))
                return;

            (RegraPallet regraPallet, Dominio.Entidades.Embarcador.Filiais.Filial filialDestino) = ObterRegraPallet(xMLNotaFiscal.Destinatario, xMLNotaFiscal.DataEmissao);

            if (EfetivarReservaEstoque(xMLNotaFiscal, cargaPedido, ref regraPallet))
                return;

            ControleEstoquePallet servicoControleEstoquePallet = new ControleEstoquePallet(_unitOfWork);

            quantidadePallets ??= ObterQuantidadePallets(xMLNotaFiscal);

            AdicionarMovimentacaoPallet adicionarMovimentacaoPallet = new AdicionarMovimentacaoPallet()
            {
                QuantidadePallets = (int)quantidadePallets,
                Cliente = xMLNotaFiscal.Destinatario,
                Carga = cargaPedido.Carga,
                Transportador = cargaPedido.Carga.Empresa,
                Filial = cargaPedido.Carga.Filial,
                FilialDestino = filialDestino,
                CargaPedido = cargaPedido,
                XMLNotaFiscal = xMLNotaFiscal,
                RegraPallet = regraPallet,
                Situacao = SituacaoGestaoPallet.Pendente,
                TipoMovimentacao = TipoEntradaSaida.Saida,
                TipoLancamento = TipoLancamento.Automatico,
                Observacao = "",
                ResponsavelPallet = ResponsavelPallet.Transportador
            };

            Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet = servicoControleEstoquePallet.ObterEstoqueParaMovimentacao(adicionarMovimentacaoPallet);

            AdicionarMovimentacao(adicionarMovimentacaoPallet, controleEstoquePallet);
        }

        public Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet AdicionarMovimentacaoPalletManual(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet, int quantidadePallets, string observacao, TipoEntradaSaida tipoEntradaSaida)
        {
            if (!IsUtilizarControlePallets() || (xMLNotaFiscal != null && xMLNotaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet))
                throw new ServicoException("A Nota fiscal não é do tipo Remessa Pallet");

            AdicionarMovimentacaoPallet adicionarMovimentacaoPallet = new AdicionarMovimentacaoPallet()
            {
                QuantidadePallets = quantidadePallets,
                Carga = carga,
                XMLNotaFiscal = xMLNotaFiscal,
                TipoMovimentacao = tipoEntradaSaida,
                TipoLancamento = TipoLancamento.Manual,
                Situacao = SituacaoGestaoPallet.Concluido,
                ResponsavelPallet = controleEstoquePallet.ResponsavelPallet,
                Observacao = observacao,
            };

            if (controleEstoquePallet.ResponsavelPallet == ResponsavelPallet.Transportador)
                adicionarMovimentacaoPallet.Transportador = controleEstoquePallet.Transportador;
            else if (controleEstoquePallet.ResponsavelPallet == ResponsavelPallet.Cliente)
                adicionarMovimentacaoPallet.Cliente = controleEstoquePallet.Cliente;
            else
                adicionarMovimentacaoPallet.Filial = controleEstoquePallet.Filial;

            return AdicionarMovimentacao(adicionarMovimentacaoPallet, controleEstoquePallet);
        }

        public void AdicionarMovimentacaoReservaPallet(Dominio.Entidades.Embarcador.Cargas.Carga carga, int quantidadePallets, ResponsavelPallet responsavelMovimentacaoPallet, Dominio.Entidades.Cliente cliente)
        {
            ControleEstoquePallet servicoControleEstoquePallet = new ControleEstoquePallet(_unitOfWork);

            AdicionarMovimentacaoPallet adicionarMovimentacaoPallet = new AdicionarMovimentacaoPallet()
            {
                QuantidadePallets = quantidadePallets,
                Carga = carga,
                Transportador = carga.Empresa,
                Cliente = cliente,
                Situacao = SituacaoGestaoPallet.Reserva,
                TipoMovimentacao = TipoEntradaSaida.Saida,
                TipoLancamento = TipoLancamento.Automatico,
                ResponsavelPallet = responsavelMovimentacaoPallet,
                RegraPallet = RegraPallet.Estoque
            };

            Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet = servicoControleEstoquePallet.ObterEstoqueParaMovimentacao(adicionarMovimentacaoPallet);

            Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet = AdicionarMovimentacao(adicionarMovimentacaoPallet, controleEstoquePallet);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, movimentacaoPallet, $"Quantidade Reservada ao realizar agendamento.", _unitOfWork);
        }

        public void AdicionarMovimentacaoAgendamentoPallet(Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet)
        {
            ControleEstoquePallet servicoControleEstoquePallet = new ControleEstoquePallet(_unitOfWork);

            AdicionarMovimentacaoPallet adicionarMovimentacaoPallet = new AdicionarMovimentacaoPallet()
            {
                QuantidadePallets = agendamentoPallet.QuantidadePallets,
                Carga = agendamentoPallet.Carga,
                Filial = agendamentoPallet.Carga.Filial,
                Transportador = agendamentoPallet.Transportador,
                Cliente = agendamentoPallet.Remetente,
                TipoMovimentacao = TipoEntradaSaida.Entrada,
                TipoLancamento = TipoLancamento.Automatico,
                ResponsavelPallet = agendamentoPallet.ResponsavelPallet,
                Situacao = SituacaoGestaoPallet.Concluido
            };

            DadosControlePallet dadosControlePallet = new DadosControlePallet(agendamentoPallet.Remetente, agendamentoPallet.Transportador, agendamentoPallet.Carga.Filial)
            {
                ResponsavelPallet = agendamentoPallet.ResponsavelPallet,
                TipoEstoquePallet = TipoEstoquePallet.Movimentacao
            };

            Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet = servicoControleEstoquePallet.AdicionarEstoque(dadosControlePallet);

            Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet = AdicionarMovimentacao(adicionarMovimentacaoPallet, controleEstoquePallet);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, movimentacaoPallet, $"Quantidade Adicionada ao finalizar descarga.", _unitOfWork);
        }

        public void AdicionarMovimentacaoAguardandoValidacao(Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoOriginal)
        {
            ControleEstoquePallet servicoControleEstoquePallet = new ControleEstoquePallet(_unitOfWork);

            AdicionarMovimentacaoPallet adicionarMovimentacaoPallet = new AdicionarMovimentacaoPallet()
            {
                QuantidadePallets = movimentacaoOriginal.QuantidadePallets,
                Cliente = movimentacaoOriginal.Cliente,
                Carga = movimentacaoOriginal.Carga,
                Transportador = movimentacaoOriginal.Transportador,
                Filial = movimentacaoOriginal.FilialDestino,
                CargaPedido = movimentacaoOriginal.CargaPedido,
                XMLNotaFiscal = movimentacaoOriginal.XMLNotaFiscal,
                RegraPallet = RegraPallet.Transferencia,
                Situacao = SituacaoGestaoPallet.AguardandoAvaliacao,
                TipoMovimentacao = TipoEntradaSaida.Entrada,
                TipoLancamento = TipoLancamento.Automatico,
                Observacao = "",
                ResponsavelPallet = ResponsavelPallet.Filial
            };

            DadosControlePallet dadosControlePallet = new DadosControlePallet(adicionarMovimentacaoPallet.Cliente, adicionarMovimentacaoPallet.Transportador, adicionarMovimentacaoPallet.Filial)
            {
                ResponsavelPallet = ResponsavelPallet.Filial,
                TipoEstoquePallet = TipoEstoquePallet.Movimentacao
            };

            Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet = servicoControleEstoquePallet.AdicionarEstoque(dadosControlePallet);

            Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet novaMovimentacao = AdicionarMovimentacao(adicionarMovimentacaoPallet, controleEstoquePallet);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, novaMovimentacao, null, $"Movimentação de entrada enviada pra avaliação.", _unitOfWork);
        }

        public void AdicionarMovimentacaoRetornoManutencao(Dominio.Entidades.Embarcador.GestaoPallet.ManutencaoPallet manutencaoPallet)
        {
            ControleEstoquePallet servicoControleEstoquePallet = new ControleEstoquePallet(_unitOfWork);

            AdicionarMovimentacaoPallet adicionarMovimentacaoPallet = new AdicionarMovimentacaoPallet()
            {
                QuantidadePallets = manutencaoPallet.QuantidadePallets,
                Filial = manutencaoPallet.Filial,
                Situacao = SituacaoGestaoPallet.Concluido,
                TipoMovimentacao = TipoEntradaSaida.Entrada,
                TipoLancamento = TipoLancamento.Automatico,
                Observacao = "Adicionado ao confirmar manutenção.",
                ResponsavelPallet = ResponsavelPallet.Filial
            };

            DadosControlePallet dadosControlePallet = new DadosControlePallet(adicionarMovimentacaoPallet.Filial.Codigo)
            {
                ResponsavelPallet = ResponsavelPallet.Filial,
                TipoEstoquePallet = TipoEstoquePallet.Movimentacao
            };

            Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet = servicoControleEstoquePallet.AdicionarEstoque(dadosControlePallet);

            Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet novaMovimentacao = AdicionarMovimentacao(adicionarMovimentacaoPallet, controleEstoquePallet);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, novaMovimentacao, null, $"Adicionado ao confirmar manutenção.", _unitOfWork);
        }

        public void AlterarSituacao(Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet, SituacaoGestaoPallet situacao)
        {
            if (movimentacaoPallet.Situacao == situacao)
                return;

            movimentacaoPallet.Situacao = situacao;

            _repositorioMovimentacaoPallet.Atualizar(movimentacaoPallet);
        }

        public void CancelarMovimentacaoPallet(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            if (!IsUtilizarControlePallets())
                return;

            Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet = _repositorioMovimentacaoPallet.BuscarPorNotaECargaPedido(xMLNotaFiscal.Codigo, cargaPedido.Codigo);

            if (movimentacaoPallet == null)
                return;

            CancelarMovimentacao(movimentacaoPallet);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, movimentacaoPallet, $"Movimentação cancelada.", _unitOfWork);
        }

        public void CancelarMovimentacaoPalletPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!IsUtilizarControlePallets())
                return;

            List<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> movimentacoesPallet = _repositorioMovimentacaoPallet.BuscarMovimentosCarga(carga.Codigo);

            if (movimentacoesPallet.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimento in movimentacoesPallet)
            {
                CancelarMovimentacao(movimento);
                Servicos.Auditoria.Auditoria.Auditar(_auditado, movimento, $"Movimentação cancelada.", _unitOfWork);
            }
        }

        public void CancelarMovimentacaoReservaPallet(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet = _repositorioMovimentacaoPallet.BuscarAutomaticoPorCarga(carga.Codigo);

            if (movimentacaoPallet == null)
                return;

            CancelarMovimentacao(movimentacaoPallet);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, movimentacaoPallet, $"Movimentação reserva cancelada.", _unitOfWork);
        }

        public void FinalizarMovimentacaoPallet(DadosFinalizarMovimentacaoPallet dadosFinalizarMovimentacaoPallet)
        {
            ControleEstoquePallet servicoControleEstoquePallet = new ControleEstoquePallet(_unitOfWork);
            ManutencaoPallet servicoManutencaoPallet = new ManutencaoPallet(_unitOfWork, _auditado);

            DadosControlePallet dadosControlePalletMovimentacao = new DadosControlePallet(dadosFinalizarMovimentacaoPallet.Filial.Codigo)
            {
                ResponsavelPallet = ResponsavelPallet.Filial
            };

            Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePalletMovimentacao = servicoControleEstoquePallet.ObterEstoquePallet(dadosControlePalletMovimentacao, TipoEstoquePallet.Movimentacao);

            new ControleEstoquePallet(_unitOfWork).AtualizarSaldo(controleEstoquePalletMovimentacao, dadosFinalizarMovimentacaoPallet.QuantidadeDevolvida, TipoEntradaSaida.Entrada);

            Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePalletManutencao = null;
            if (dadosFinalizarMovimentacaoPallet.QuantidadeManutencao > 0)
            {
                controleEstoquePalletManutencao = servicoControleEstoquePallet.ObterEstoquePallet(dadosControlePalletMovimentacao, TipoEstoquePallet.Manutencao);

                AdicionarManutencaoPallet adicionarManutencaoPallet = new AdicionarManutencaoPallet()
                {
                    QuantidadePallet = dadosFinalizarMovimentacaoPallet.QuantidadeManutencao,
                    Filial = dadosFinalizarMovimentacaoPallet.Filial,
                    Carga = dadosFinalizarMovimentacaoPallet.Carga,
                    XMLNotaFiscal = dadosFinalizarMovimentacaoPallet.XMLNotaFiscal,
                    TipoMovimentacao = TipoEntradaSaida.Entrada,
                    Observacao = dadosFinalizarMovimentacaoPallet.Observacao
                };

                servicoManutencaoPallet.AdicionarManutencaoPallet(adicionarManutencaoPallet, controleEstoquePalletManutencao);
            }

            if (dadosFinalizarMovimentacaoPallet.QuantidadeDescarte > 0)
            {
                controleEstoquePalletManutencao ??= servicoControleEstoquePallet.ObterEstoquePallet(dadosControlePalletMovimentacao, TipoEstoquePallet.Manutencao);

                AdicionarManutencaoPallet adicionarDescartePallet = new AdicionarManutencaoPallet()
                {
                    QuantidadePallet = dadosFinalizarMovimentacaoPallet.QuantidadeDescarte,
                    Filial = dadosFinalizarMovimentacaoPallet.Filial,
                    TipoMovimentacao = TipoEntradaSaida.Saida,
                    TipoManutencaoPallet = TipoManutencaoPallet.Descarte,
                    Observacao = dadosFinalizarMovimentacaoPallet.Observacao
                };

                servicoManutencaoPallet.AdicionarManutencaoPallet(adicionarDescartePallet, controleEstoquePalletManutencao, atualizarSaldo: false);
            }
        }

        public void ReverterMovimentacaoPallet(Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet)
        {
            CancelarMovimentacao(movimentacaoPallet);
        }

        public void InformarDevolucaoPallet(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao)
        {
            List<int> codigosXmlNotasFiscais = gestaoDevolucao.NotasFiscaisDeOrigem.Where(o => o.XMLNotaFiscal.TipoNotaFiscalIntegrada.HasValue && o.XMLNotaFiscal.TipoNotaFiscalIntegrada.Value == TipoNotaFiscalIntegrada.RemessaPallet).Select(o => o.XMLNotaFiscal.Codigo).ToList();

            if (codigosXmlNotasFiscais.Count <= 0)
                return;

            List<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> movimentacoesPallet = _repositorioMovimentacaoPallet.BuscarPorNotasESituacao(codigosXmlNotasFiscais, SituacaoGestaoPallet.Pendente);

            foreach (Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet in movimentacoesPallet)
            {
                AlterarSituacao(movimentacaoPallet, SituacaoGestaoPallet.Concluido);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, movimentacaoPallet, $"Movimentação concluída.", _unitOfWork);
            }

            if (gestaoDevolucao.Tipo == TipoGestaoDevolucao.PermutaPallet)
                return;

            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repositorioXmlNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(_unitOfWork);
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto repositorioGestaoDevolucaoLaudoProduto = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto(_unitOfWork);

            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador prudutoNotaFiscal = repositorioXmlNotaFiscalProduto.BuscarPrimeiroProdutoPorNotaFiscal(codigosXmlNotasFiscais.FirstOrDefault());
            QuantidadeLaudoPallet quantidadeLaudoPallet = repositorioGestaoDevolucaoLaudoProduto.BuscarQuantidadesPorProdutoELaudo(gestaoDevolucao.Laudo?.Codigo ?? 0, prudutoNotaFiscal?.Codigo ?? 0);

            if (quantidadeLaudoPallet == null)
                throw new ServicoException("Quantidades do laudo não encontradas");

            DadosFinalizarMovimentacaoPallet dadosFinalizarMovimentacaoPallet = new DadosFinalizarMovimentacaoPallet()
            {
                Filial = gestaoDevolucao.CargaDevolucao?.Filial ?? gestaoDevolucao.Filial,
                QuantidadeDevolvida = (int)quantidadeLaudoPallet.QuantidadeDisponivel,
                QuantidadeManutencao = (int)quantidadeLaudoPallet.QuantidadeManutencao,
                QuantidadeDescarte = (int)quantidadeLaudoPallet.QuantidadeDescarte,
                Observacao = "Adcionado ao concluir a Devolução"
            };

            FinalizarMovimentacaoPallet(dadosFinalizarMovimentacaoPallet);
        }

        public void InformarMudancaResponsavel(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            List<int> codigosXMLNotasFiscais = repositorioXMLNotaFiscal.BuscarCodigosPorCargaEntrega(cargaEntrega.Codigo);

            List<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> movimentacoesPallet = _repositorioMovimentacaoPallet.BuscarPorNotasEntregaESituacao(codigosXMLNotasFiscais, SituacaoGestaoPallet.Pendente);

            foreach (Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet in movimentacoesPallet)
                RealizarMudancaResponsavel(movimentacaoPallet, "entrega");
        }

        public void InformarMudancaResponsavel(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto)
        {
            if (!IsUtilizarControlePallets() || canhoto.XMLNotaFiscal == null)
                return;

            Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet = _repositorioMovimentacaoPallet.BuscarPorNotaCanhotoESituacao(canhoto.XMLNotaFiscal.Codigo, SituacaoGestaoPallet.Pendente);

            if (movimentacaoPallet == null)
                return;

            RealizarMudancaResponsavel(movimentacaoPallet, "digitalização canhoto");
        }

        public void InformarMudancaResponsavel(Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet)
        {
            RealizarMudancaResponsavel(movimentacaoPallet, "vale pallet");
        }

        public void InformarMudancaResponsavelManual(Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPallet responsavelMovimentacaoPallet, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Cliente cliente)
        {
            if (!movimentacaoPallet.RegraPallet.IsRegraCIF())
                return;

            movimentacaoPallet.Initialize();

            if (responsavelMovimentacaoPallet == ResponsavelPallet.Filial)
                movimentacaoPallet.Filial = filial;

            else if (responsavelMovimentacaoPallet == ResponsavelPallet.Cliente)
                movimentacaoPallet.Cliente = cliente;

            movimentacaoPallet.ResponsavelMovimentacaoPallet = responsavelMovimentacaoPallet;

            _repositorioMovimentacaoPallet.Atualizar(movimentacaoPallet);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, movimentacaoPallet, movimentacaoPallet.GetChanges(), $"Alterado responsável manualmente.", _unitOfWork);
        }

        public void InformarQuebraRegra(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, TipoQuebraRegra tipoQuebraRegra)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xMLNotasFiscais;

            if (chamado.XMLNotasFiscais.Any(o => o.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet))
                xMLNotasFiscais = chamado.XMLNotasFiscais.ToList();
            else if (chamado.CargaEntrega != null)
                xMLNotasFiscais = repositorioXMLNotaFiscal.BuscarPorCargaEntrega(chamado.CargaEntrega.Codigo);
            else
            {
                if (chamado.Carga == null || chamado.Pedido == null)
                    return;

                xMLNotasFiscais = repositorioXMLNotaFiscal.BuscarPorCargaEPedido(chamado.Carga.Codigo, chamado.Pedido.Codigo);
            }

            List<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> movimentacoesPallet = _repositorioMovimentacaoPallet.BuscarPorNotasESituacao(xMLNotasFiscais.Select(o => o.Codigo).ToList(), SituacaoGestaoPallet.Pendente);

            foreach (Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet in movimentacoesPallet)
            {
                if (!movimentacaoPallet.RegraPallet.IsRegraCIF() || movimentacaoPallet.QuebraRegraInformada)
                    continue;

                movimentacaoPallet.Initialize();

                DefinirAcaoPorTipoQuebraRegra(movimentacaoPallet, tipoQuebraRegra);

                movimentacaoPallet.QuebraRegraInformada = true;

                _repositorioMovimentacaoPallet.Atualizar(movimentacaoPallet);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, movimentacaoPallet, movimentacaoPallet.GetChanges(), $"Quebra de regra {tipoQuebraRegra.ObterDescricao()} informada", _unitOfWork);
            }
        }

        public void InformarTrocaTransportador(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!IsUtilizarControlePallets())
                return;

            List<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet> movimentacoesPallet = _repositorioMovimentacaoPallet.BuscarPorCargaDiffTransportador(carga.Codigo, carga.Empresa.Codigo);

            foreach (Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet in movimentacoesPallet)
            {
                movimentacaoPallet.Transportador = carga.Empresa;

                _repositorioMovimentacaoPallet.Atualizar(movimentacaoPallet);
            }
        }

        public void NotificarPalletsPendentes()
        {
            if (!ObterConfiguracaoPallet().NotificarPaletesPendentes || ObterConfiguracaoPallet().DiaSemanaNotificarPaletesPendentes != DiaSemanaHelper.ObterDiaSemana(DateTime.Now))
                return;

            IList<MovimentosPalletPendentes> movimentosPendentes = _repositorioMovimentacaoPallet.BuscarMovimentosPendentes();

            IEnumerable<(string Responsavel, int TipoResponsavel)> movimentosBuscar = movimentosPendentes.Select(o => (o.Responsavel, o.TipoResponsavel)).Distinct();

            foreach ((string Responsavel, int TipoResponsavel) in movimentosBuscar)
            {
                System.Text.StringBuilder mensagemEmail = new System.Text.StringBuilder();

                List<MovimentosPalletPendentes> movimentos = movimentosPendentes.Where(o => o.Responsavel == Responsavel && o.TipoResponsavel == TipoResponsavel).ToList();

                string email = movimentos[0].EmailResponsavel;

                string assuntoEmail = "Pallet pendente de devolução - Ypê";

                mensagemEmail.AppendLine($"Olá, caro Cliente/Transportador segue abaixo a lista de notas de pallets pendentes de devolução para Ypê, regularize sua pendencia.");

                mensagemEmail.Append("<style>table { width: 100 %; border - collapse: collapse; margin: 20px 0; } th, td { border: 1px solid #dddddd; text - align: left; padding: 8px; } th { background - color: #f2f2f2; } tr: nth - child(even) { background - color: #f9f9f9; }</style>");
                mensagemEmail.AppendLine();
                mensagemEmail.Append("<table><thead><tr><th>Nota</th><th>Carga</th><th>Data Emissão Nota</th><th>Situação</th></tr></thead><tbody>");


                foreach (MovimentosPalletPendentes movimento in movimentos)
                    mensagemEmail.Append($"<tr><td>{movimento.NumeroNota}</td><td>{movimento.Carga}</td><td>{movimento.DataEmissaoNota.ToString("d")}</td><td>Pendente</td></tr>");

                mensagemEmail.Append("</tbody></table>");

                string corpoEmail = mensagemEmail.ToString();

                new Servicos.Email(_unitOfWork).EnviarEmail(string.Empty, string.Empty, string.Empty, email, null, null, assuntoEmail, corpoEmail, string.Empty, null, string.Empty, false, string.Empty, 0, _unitOfWork);
            }
        }

        #endregion Métodos Públicos
    }
}
