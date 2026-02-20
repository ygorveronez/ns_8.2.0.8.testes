using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pallets
{
    public sealed class EstoquePallet
    {
        #region Atributos Privados Somente Leitura

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public EstoquePallet(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null)
        {
        }

        public EstoquePallet(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private void InserirMovimentacaoEstoqueAvaria(Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque, bool isPorTransportador)
        {
            var avaria = ObterAvaria(dadosMovimentacaoEstoque.CodigoAvaria);
            var tipoMovimentacao = ObterTipoMovimentacaoAvaria(dadosMovimentacaoEstoque);

            var listaMovimentacaoEstoqueQuantidade = (
                from avariaQuantidade in avaria.QuantidadesAvariadas
                select new Dominio.ObjetosDeValor.Embarcador.Pallets.MovimentacaoEstoqueQuantidade() {
                    Quantidade = avariaQuantidade.Quantidade,
                    SituacaoDevolucao = avariaQuantidade.SituacaoDevolucaoPallet
                }
            ).ToList();

            InserirMovimentacaoEstoqueAvariaOuReforma(dadosMovimentacaoEstoque, listaMovimentacaoEstoqueQuantidade, tipoMovimentacao, isAvaria: true, isPorTransportador: isPorTransportador);
        }

        private void InserirMovimentacaoEstoqueAvariaDevolucao(Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque)
        {
            if (dadosMovimentacaoEstoque.DevolucaoPallet?.Situacoes?.Count > 0)
            {
                var listaMovimentacaoEstoqueQuantidade = (
                    from avariaQuantidade in dadosMovimentacaoEstoque.DevolucaoPallet?.Situacoes
                    where avariaQuantidade.Situacao.SituacaoPalletAvariado && (avariaQuantidade.Quantidade > 0)
                    select new Dominio.ObjetosDeValor.Embarcador.Pallets.MovimentacaoEstoqueQuantidade()
                    {
                        Quantidade = avariaQuantidade.Quantidade,
                        SituacaoDevolucao = avariaQuantidade.Situacao
                    }
                ).ToList();

                if (listaMovimentacaoEstoqueQuantidade.Count > 0)
                {
                    var dadosMovimentacaoAvariaDevolucao = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
                    {
                        CpfCnpjCliente = dadosMovimentacaoEstoque.CpfCnpjCliente,
                        CodigoFilial = dadosMovimentacaoEstoque.CodigoFilial,
                        CodigoSetor = dadosMovimentacaoEstoque.CodigoSetor,
                        CodigoTransportador = dadosMovimentacaoEstoque.CodigoTransportador,
                        Quantidade = listaMovimentacaoEstoqueQuantidade.Sum(o => o.Quantidade),
                        TipoLancamento = dadosMovimentacaoEstoque.TipoLancamento,
                        TipoOperacaoMovimentacao = TipoOperacaoMovimentacaoEstoquePallet.FilialSaida,
                        CodigoGrupoPessoas = dadosMovimentacaoEstoque.CodigoGrupoPessoas
                    };

                    InserirMovimentacaoEstoqueFilial(dadosMovimentacaoAvariaDevolucao);

                    InserirMovimentacaoEstoqueAvariaOuReforma(dadosMovimentacaoAvariaDevolucao, listaMovimentacaoEstoqueQuantidade, TipoMovimentacaoEstoquePallet.Entrada, isAvaria: false, isPorTransportador: false);
                }
            }
        }

        private void InserirMovimentacaoEstoqueAvariaOuReforma(Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque, List<Dominio.ObjetosDeValor.Embarcador.Pallets.MovimentacaoEstoqueQuantidade> listaMovimentacaoEstoqueQuantidade, TipoMovimentacaoEstoquePallet tipoMovimentacao, bool isAvaria, bool isPorTransportador)
        {
            if (listaMovimentacaoEstoqueQuantidade.Count > 0)
            {
                var repositorioEstoquePallet = new Repositorio.Embarcador.Pallets.EstoquePallet(_unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
                Dominio.Entidades.Empresa transportador = null;

                if (isPorTransportador)
                    transportador = ObterTransportador(dadosMovimentacaoEstoque.CodigoTransportador);
                else
                    filial = ObterFilial(dadosMovimentacaoEstoque.CodigoFilial);

                var observacao = ObterObservacao(dadosMovimentacaoEstoque);
                var setor = ObterSetor(dadosMovimentacaoEstoque.CodigoSetor);

                foreach (var movimentacaoEstoqueQuantidade in listaMovimentacaoEstoqueQuantidade)
                {
                    var saldoAtual = isAvaria ? repositorioEstoquePallet.ObterSaldoAvariaPorFilial(movimentacaoEstoqueQuantidade.SituacaoDevolucao.Codigo, dadosMovimentacaoEstoque.CodigoFilial) : repositorioEstoquePallet.ObterSaldoReformaPorFilial(movimentacaoEstoqueQuantidade.SituacaoDevolucao.Codigo, dadosMovimentacaoEstoque.CodigoFilial);

                    var estoquePallet = new Dominio.Entidades.Embarcador.Pallets.EstoquePallet()
                    {
                        Data = DateTime.Now,
                        Devolucao = dadosMovimentacaoEstoque.DevolucaoPallet,
                        NaturezaMovimentacao = isAvaria ? NaturezaMovimentacaoEstoquePallet.Avaria : NaturezaMovimentacaoEstoquePallet.Reforma,
                        Filial = filial,
                        Observacao = observacao,
                        Quantidade = movimentacaoEstoqueQuantidade.Quantidade,
                        SaldoTotal = ObterSaldoTotal(saldoAtual, movimentacaoEstoqueQuantidade.Quantidade, tipoMovimentacao),
                        Setor = setor,
                        SituacaoDevolucaoPallet = movimentacaoEstoqueQuantidade.SituacaoDevolucao,
                        TipoLancamento = dadosMovimentacaoEstoque.TipoLancamento,
                        TipoMovimentacao = tipoMovimentacao,
                        Transportador = transportador,
                        Usuario = _auditado?.Usuario,
                        GrupoPessoas = dadosMovimentacaoEstoque.CodigoGrupoPessoas > 0 ? ObterGrupoPessoas(dadosMovimentacaoEstoque.CodigoGrupoPessoas) : null
                    };

                    repositorioEstoquePallet.Inserir(estoquePallet, _auditado);
                }
            }
        }

        private void InserirMovimentacaoEstoqueCliente(Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque)
        {
            var repositorioEstoquePallet = new Repositorio.Embarcador.Pallets.EstoquePallet(_unitOfWork);
            var tipoMovimentacao = ObterTipoMovimentacaoCliente(dadosMovimentacaoEstoque);
            var saldoAtual = repositorioEstoquePallet.ObterSaldoCliente(dadosMovimentacaoEstoque.CpfCnpjCliente);

            var estoquePallet = new Dominio.Entidades.Embarcador.Pallets.EstoquePallet()
            {
                Cliente = ObterCliente(dadosMovimentacaoEstoque.CpfCnpjCliente),
                Data = DateTime.Now,
                Devolucao = dadosMovimentacaoEstoque.DevolucaoPallet,
                NaturezaMovimentacao = NaturezaMovimentacaoEstoquePallet.Cliente,
                Observacao = ObterObservacao(dadosMovimentacaoEstoque),
                Quantidade = dadosMovimentacaoEstoque.Quantidade,
                QuantidadeDescartada = dadosMovimentacaoEstoque.QuantidadeDescartada,
                SaldoTotal = ObterSaldoTotal(saldoAtual, dadosMovimentacaoEstoque.Quantidade, tipoMovimentacao),
                TipoLancamento = dadosMovimentacaoEstoque.TipoLancamento,
                TipoMovimentacao = tipoMovimentacao,
                Usuario = _auditado?.Usuario,
                GrupoPessoas = dadosMovimentacaoEstoque.CodigoGrupoPessoas > 0 ? ObterGrupoPessoas(dadosMovimentacaoEstoque.CodigoGrupoPessoas) : null
            };

            repositorioEstoquePallet.Inserir(estoquePallet, _auditado);
        }

        private void InserirMovimentacaoEstoqueFilial(Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque)
        {
            var repositorioEstoquePallet = new Repositorio.Embarcador.Pallets.EstoquePallet(_unitOfWork);
            var tipoMovimentacao = ObterTipoMovimentacaoFilial(dadosMovimentacaoEstoque);
            var saldoAtual = repositorioEstoquePallet.ObterSaldoFilial(dadosMovimentacaoEstoque.CodigoFilial);

            var estoquePallet = new Dominio.Entidades.Embarcador.Pallets.EstoquePallet()
            {
                Data = DateTime.Now,
                Devolucao = dadosMovimentacaoEstoque.DevolucaoPallet,
                CancelamentoBaixaPallets = dadosMovimentacaoEstoque.CancelamentoBaixaPallets,
                NaturezaMovimentacao = NaturezaMovimentacaoEstoquePallet.Filial,
                Filial = ObterFilial(dadosMovimentacaoEstoque.CodigoFilial),
                Observacao = ObterObservacao(dadosMovimentacaoEstoque),
                Quantidade = dadosMovimentacaoEstoque.Quantidade,
                QuantidadeDescartada = dadosMovimentacaoEstoque.QuantidadeDescartada,
                SaldoTotal = ObterSaldoTotal(saldoAtual, dadosMovimentacaoEstoque.Quantidade, tipoMovimentacao),
                Setor = ObterSetor(dadosMovimentacaoEstoque.CodigoSetor),
                TipoLancamento = dadosMovimentacaoEstoque.TipoLancamento,
                TipoMovimentacao = tipoMovimentacao,
                Usuario = _auditado?.Usuario,
                GrupoPessoas = dadosMovimentacaoEstoque.CodigoGrupoPessoas > 0 ? ObterGrupoPessoas(dadosMovimentacaoEstoque.CodigoGrupoPessoas) : null
            };

            repositorioEstoquePallet.Inserir(estoquePallet, _auditado);
        }

        private void InserirMovimentacaoEstoqueReforma(Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque, bool isPorTransportador)
        {
            var reforma = ObterReforma(dadosMovimentacaoEstoque.CodigoReforma);
            var tipoMovimentacao = ObterTipoMovimentacaoReforma(dadosMovimentacaoEstoque);

            var listaMovimentacaoEstoqueQuantidade = (
                from reformaQuantidade in reforma.Envio.QuantidadesReforma
                select new Dominio.ObjetosDeValor.Embarcador.Pallets.MovimentacaoEstoqueQuantidade()
                {
                    Quantidade = reformaQuantidade.Quantidade,
                    SituacaoDevolucao = reformaQuantidade.SituacaoDevolucaoPallet
                }
            ).ToList();

            InserirMovimentacaoEstoqueAvariaOuReforma(dadosMovimentacaoEstoque, listaMovimentacaoEstoqueQuantidade, tipoMovimentacao, isAvaria: false, isPorTransportador: isPorTransportador);
        }

        private void InserirMovimentacaoEstoqueReformaMovimentandoAvaria(Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque, bool isPorTransportador)
        {
            var reforma = ObterReforma(dadosMovimentacaoEstoque.CodigoReforma);
            var tipoMovimentacao = ObterTipoMovimentacaoReforma(dadosMovimentacaoEstoque);

            var listaMovimentacaoEstoqueQuantidade = (
                from reformaQuantidade in reforma.Envio.QuantidadesReforma
                select new Dominio.ObjetosDeValor.Embarcador.Pallets.MovimentacaoEstoqueQuantidade()
                {
                    Quantidade = reformaQuantidade.Quantidade,
                    SituacaoDevolucao = reformaQuantidade.SituacaoDevolucaoPallet
                }
            ).ToList();

            var isMovimentacaoSaidaAvaria = (tipoMovimentacao == TipoMovimentacaoEstoquePallet.Entrada);

            InserirMovimentacaoEstoqueAvariaOuReforma(dadosMovimentacaoEstoque, listaMovimentacaoEstoqueQuantidade, TipoMovimentacaoEstoquePallet.Saida, isAvaria: isMovimentacaoSaidaAvaria, isPorTransportador: isPorTransportador);
            InserirMovimentacaoEstoqueAvariaOuReforma(dadosMovimentacaoEstoque, listaMovimentacaoEstoqueQuantidade, TipoMovimentacaoEstoquePallet.Entrada, isAvaria: !isMovimentacaoSaidaAvaria, isPorTransportador: isPorTransportador);
        }

        private void InserirMovimentacaoEstoqueTransportador(Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque)
        {
            var repositorioEstoquePallet = new Repositorio.Embarcador.Pallets.EstoquePallet(_unitOfWork);
            var tipoMovimentacao = ObterTipoMovimentacaoTransportador(dadosMovimentacaoEstoque);
            var saldoAtual = repositorioEstoquePallet.ObterSaldoTransportador(dadosMovimentacaoEstoque.CodigoTransportador);
            var quantidade = dadosMovimentacaoEstoque.Quantidade;
            var quantidadeDescartada = dadosMovimentacaoEstoque.QuantidadeDescartada;

            if (dadosMovimentacaoEstoque.TipoServicoMultisoftware.HasValue && (dadosMovimentacaoEstoque.TipoServicoMultisoftware.Value == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
            {
                quantidade += quantidadeDescartada;
                quantidadeDescartada = 0;
            }

            var estoquePallet = new Dominio.Entidades.Embarcador.Pallets.EstoquePallet()
            {
                Data = dadosMovimentacaoEstoque.DataMovimento.HasValue ? dadosMovimentacaoEstoque.DataMovimento.Value : DateTime.Now,
                Devolucao = dadosMovimentacaoEstoque.DevolucaoPallet,
                CancelamentoBaixaPallets = dadosMovimentacaoEstoque.CancelamentoBaixaPallets,
                NaturezaMovimentacao = NaturezaMovimentacaoEstoquePallet.Transportador,
                Observacao = ObterObservacao(dadosMovimentacaoEstoque),
                Quantidade = quantidade,
                QuantidadeDescartada = quantidadeDescartada,
                SaldoTotal = ObterSaldoTotal(saldoAtual, dadosMovimentacaoEstoque.Quantidade, tipoMovimentacao),
                TipoLancamento = dadosMovimentacaoEstoque.TipoLancamento,
                TipoMovimentacao = tipoMovimentacao,
                Transportador = ObterTransportador(dadosMovimentacaoEstoque.CodigoTransportador),
                Usuario = _auditado?.Usuario,
                GrupoPessoas = dadosMovimentacaoEstoque.CodigoGrupoPessoas > 0 ? ObterGrupoPessoas(dadosMovimentacaoEstoque.CodigoGrupoPessoas) : null
            };

            repositorioEstoquePallet.Inserir(estoquePallet, _auditado);
        }

        private bool IsTipoOperacaoMovimentacaoUtilizaAvaria(TipoOperacaoMovimentacaoEstoquePallet tipoOperacaoMovimentacao)
        {
            return (
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.FilialAvaria) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.TransportadorAvaria)
            );
        }

        private bool IsTipoOperacaoMovimentacaoUtilizaCliente(TipoOperacaoMovimentacaoEstoquePallet tipoOperacaoMovimentacao)
        {
            return (
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ClienteEntrada) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ClienteSaida) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ClienteFilial) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ClienteTransportador) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.FilialCliente) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.TransportadorCliente)
            );
        }

        private bool IsTipoOperacaoMovimentacaoUtilizaFilial(TipoOperacaoMovimentacaoEstoquePallet tipoOperacaoMovimentacao)
        {
            return (
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.AvariaReforma) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ClienteFilial) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.FilialAvaria) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.FilialCliente) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.FilialEntrada) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.FilialSaida) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.FilialTransportador) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ReformaFilial) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.TransportadorFilial)
            );
        }

        private bool IsTipoOperacaoMovimentacaoUtilizaQuantidade(TipoOperacaoMovimentacaoEstoquePallet tipoOperacaoMovimentacao)
        {
            return (
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ClienteEntrada) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ClienteFilial) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ClienteSaida) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ClienteTransportador) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.FilialCliente) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.FilialEntrada) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.FilialSaida) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.FilialTransportador) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.TransportadorCliente) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.TransportadorEntrada) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.TransportadorFilial) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.TransportadorSaida)
            );
        }

        private bool IsTipoOperacaoMovimentacaoUtilizaReforma(TipoOperacaoMovimentacaoEstoquePallet tipoOperacaoMovimentacao)
        {
            return (
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ReformaFilial) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ReformaTransportador)
            );
        }

        private bool IsTipoOperacaoMovimentacaoUtilizaTransportador(TipoOperacaoMovimentacaoEstoquePallet tipoOperacaoMovimentacao)
        {
            return (
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ClienteTransportador) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.FilialTransportador) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.TransportadorCliente) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.TransportadorEntrada) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.TransportadorSaida) ||
                (tipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.TransportadorFilial)
            );
        }

        private Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet ObterAvaria(int codigo)
        {
            var repositorioAvaria = new Repositorio.Embarcador.Pallets.AvariaPallet(_unitOfWork);
            var avaria = repositorioAvaria.BuscarPorCodigo(codigo);

            if (avaria == null)
                throw new ServicoException("Avaria não encontrada");

            return avaria;
        }

        private Dominio.Entidades.Cliente ObterCliente(double cpfCnpj)
        {
            var repositorio = new Repositorio.Cliente(_unitOfWork);
            var cliente = repositorio.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente == null)
                throw new ServicoException("Cliente não encontrado");

            return cliente;
        }

        private Dominio.Entidades.Embarcador.Filiais.Filial ObterFilial(int codigo)
        {
            var repositorio = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            var filial = repositorio.BuscarPorCodigo(codigo);

            if (filial == null)
                throw new ServicoException("Filial não encontrada");

            return filial;
        }

        private string ObterObservacao(Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque)
        {
            if ((dadosMovimentacaoEstoque.TipoLancamento == TipoLancamento.Manual) && string.IsNullOrWhiteSpace(dadosMovimentacaoEstoque.Observacao))
                throw new ServicoException("Observação não informada");

            if (!string.IsNullOrWhiteSpace(dadosMovimentacaoEstoque.Observacao))
                return dadosMovimentacaoEstoque.Observacao.Trim();

            System.Text.StringBuilder observacao = new System.Text.StringBuilder(dadosMovimentacaoEstoque.TipoOperacaoMovimentacao.ObterObservacao());
            Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet devolucaoPallet = dadosMovimentacaoEstoque.CancelamentoBaixaPallets?.Devolucao ?? dadosMovimentacaoEstoque.DevolucaoPallet;

            if (devolucaoPallet != null)
            {
                observacao.Append(" referente a ");

                if (devolucaoPallet.CargaPedido != null)
                    observacao.Append($"Carga {devolucaoPallet.CargaPedido.Carga.CodigoCargaEmbarcador} e ");

                if (devolucaoPallet.XMLNotaFiscal != null)
                    observacao.Append($"NF {devolucaoPallet.XMLNotaFiscal.Numero.ToString()}");
            }

            return observacao.ToString();
        }

        private Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet ObterReforma(int codigo)
        {
            var repositorioReforma = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(_unitOfWork);
            var reforma = repositorioReforma.BuscarPorCodigo(codigo);

            if (reforma == null)
                throw new ServicoException("Reforma não encontrada");

            return reforma;
        }

        private int ObterSaldoTotal(int saldoAtual, int quantidade, TipoMovimentacaoEstoquePallet tipoMovimentacao)
        {
            if (tipoMovimentacao == TipoMovimentacaoEstoquePallet.Entrada)
                return saldoAtual + quantidade;

            return saldoAtual - quantidade;
        }

        private Dominio.Entidades.Setor ObterSetor(int codigo)
        {
            if (codigo == 0)
                return null;

            var repositorioSetor = new Repositorio.Setor(_unitOfWork);
            var setor = repositorioSetor.BuscarPorCodigo(codigo);

            if (setor == null)
                throw new ServicoException("Setor não encontrado");

            return setor;
        }

        private TipoMovimentacaoEstoquePallet ObterTipoMovimentacaoAvaria(Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque)
        {
            if (
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.AvariaReforma) ||
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.AvariaReformaPorTransportador)
            )
                return TipoMovimentacaoEstoquePallet.Saida;

            return TipoMovimentacaoEstoquePallet.Entrada;
        }

        private TipoMovimentacaoEstoquePallet ObterTipoMovimentacaoCliente(Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque)
        {
            if (
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ClienteFilial) ||
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ClienteSaida) ||
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ClienteTransportador)
            )
                return TipoMovimentacaoEstoquePallet.Saida;

            return TipoMovimentacaoEstoquePallet.Entrada;
        }

        private TipoMovimentacaoEstoquePallet ObterTipoMovimentacaoFilial(Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque)
        {
            if (
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.FilialAvaria) ||
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.FilialCliente) ||
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.FilialTransportador) ||
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.FilialSaida)
            )
                return TipoMovimentacaoEstoquePallet.Saida;

            return TipoMovimentacaoEstoquePallet.Entrada;
        }

        private TipoMovimentacaoEstoquePallet ObterTipoMovimentacaoReforma(Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque)
        {
            if (
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ReformaAvaria) ||
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ReformaAvariaPorTransportador) ||
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ReformaFilial) ||
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.ReformaTransportador)
            )
                return TipoMovimentacaoEstoquePallet.Saida;

            return TipoMovimentacaoEstoquePallet.Entrada;
        }

        private TipoMovimentacaoEstoquePallet ObterTipoMovimentacaoTransportador(Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque)
        {
            if (
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.TransportadorAvaria) ||
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.TransportadorFilial) ||
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.TransportadorSaida) ||
                (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao == TipoOperacaoMovimentacaoEstoquePallet.TransportadorCliente)
            )
                return TipoMovimentacaoEstoquePallet.Saida;

            return TipoMovimentacaoEstoquePallet.Entrada;
        }

        private Dominio.Entidades.Empresa ObterTransportador(int codigo)
        {
            var repositorio = new Repositorio.Empresa(_unitOfWork);
            var transportador = repositorio.BuscarPorCodigo(codigo);

            if (transportador == null)
                throw new ServicoException("Transportador não encontrado");

            return transportador;
        }

        private Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas ObterGrupoPessoas (int codigo)
        {
            var repositorio = new Repositorio.Embarcador.Pessoas.GrupoPessoas (_unitOfWork);
            var grupoPessoas = repositorio.BuscarPorCodigo(codigo);

            if (grupoPessoas == null)
                throw new ServicoException("Grupo de Pessoas não encontrado");

            return grupoPessoas;
        }

        private void ValidarDadosMovimentacao(Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque)
        {
            if (IsTipoOperacaoMovimentacaoUtilizaAvaria(dadosMovimentacaoEstoque.TipoOperacaoMovimentacao) && (dadosMovimentacaoEstoque.CodigoAvaria == 0))
                throw new ServicoException("Avaria não informada");

            if (IsTipoOperacaoMovimentacaoUtilizaCliente(dadosMovimentacaoEstoque.TipoOperacaoMovimentacao) && (dadosMovimentacaoEstoque.CpfCnpjCliente == 0d))
                throw new ServicoException("Cliente não informado");

            if (IsTipoOperacaoMovimentacaoUtilizaFilial(dadosMovimentacaoEstoque.TipoOperacaoMovimentacao) && (dadosMovimentacaoEstoque.CodigoFilial == 0))
                throw new ServicoException("Filial não informada");

            if (IsTipoOperacaoMovimentacaoUtilizaReforma(dadosMovimentacaoEstoque.TipoOperacaoMovimentacao) && (dadosMovimentacaoEstoque.CodigoReforma == 0))
                throw new ServicoException("Reforma não informada");

            if (IsTipoOperacaoMovimentacaoUtilizaTransportador(dadosMovimentacaoEstoque.TipoOperacaoMovimentacao) && (dadosMovimentacaoEstoque.CodigoTransportador == 0))
                throw new ServicoException("Transportador não informado");

            if (IsTipoOperacaoMovimentacaoUtilizaQuantidade(dadosMovimentacaoEstoque.TipoOperacaoMovimentacao) && (dadosMovimentacaoEstoque.Quantidade == 0)) 
                throw new ServicoException("Quantidade deve ser superior a zero");
        }

        #endregion

        #region Métodos Públicos

        public void InserirMovimentacao(Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque)
        {
            ValidarDadosMovimentacao(dadosMovimentacaoEstoque);

            switch (dadosMovimentacaoEstoque.TipoOperacaoMovimentacao)
            {
                case TipoOperacaoMovimentacaoEstoquePallet.AvariaReforma:
                    InserirMovimentacaoEstoqueReformaMovimentandoAvaria(dadosMovimentacaoEstoque, isPorTransportador: false);
                    break;

                case TipoOperacaoMovimentacaoEstoquePallet.AvariaReformaPorTransportador:
                    InserirMovimentacaoEstoqueReformaMovimentandoAvaria(dadosMovimentacaoEstoque, isPorTransportador: true);
                    break;

                case TipoOperacaoMovimentacaoEstoquePallet.ClienteEntrada:
                case TipoOperacaoMovimentacaoEstoquePallet.ClienteSaida:
                    InserirMovimentacaoEstoqueCliente(dadosMovimentacaoEstoque);
                    break;

                case TipoOperacaoMovimentacaoEstoquePallet.ClienteFilial:
                    InserirMovimentacaoEstoqueCliente(dadosMovimentacaoEstoque);
                    InserirMovimentacaoEstoqueFilial(dadosMovimentacaoEstoque);
                    break;

                case TipoOperacaoMovimentacaoEstoquePallet.FilialEntrada:
                case TipoOperacaoMovimentacaoEstoquePallet.FilialSaida:
                    InserirMovimentacaoEstoqueFilial(dadosMovimentacaoEstoque);
                    break;

                case TipoOperacaoMovimentacaoEstoquePallet.ClienteTransportador:
                    InserirMovimentacaoEstoqueCliente(dadosMovimentacaoEstoque);
                    InserirMovimentacaoEstoqueTransportador(dadosMovimentacaoEstoque);
                    break;

                case TipoOperacaoMovimentacaoEstoquePallet.FilialAvaria:
                    InserirMovimentacaoEstoqueFilial(dadosMovimentacaoEstoque);
                    InserirMovimentacaoEstoqueAvaria(dadosMovimentacaoEstoque, isPorTransportador: false);
                    break;

                case TipoOperacaoMovimentacaoEstoquePallet.FilialCliente:
                    InserirMovimentacaoEstoqueFilial(dadosMovimentacaoEstoque);
                    InserirMovimentacaoEstoqueCliente(dadosMovimentacaoEstoque);
                    break;

                case TipoOperacaoMovimentacaoEstoquePallet.FilialTransportador:
                    InserirMovimentacaoEstoqueFilial(dadosMovimentacaoEstoque);
                    InserirMovimentacaoEstoqueTransportador(dadosMovimentacaoEstoque);
                    break;

                case TipoOperacaoMovimentacaoEstoquePallet.ReformaAvaria:
                    InserirMovimentacaoEstoqueReformaMovimentandoAvaria(dadosMovimentacaoEstoque, isPorTransportador: false);
                    break;

                case TipoOperacaoMovimentacaoEstoquePallet.ReformaAvariaPorTransportador:
                    InserirMovimentacaoEstoqueReformaMovimentandoAvaria(dadosMovimentacaoEstoque, isPorTransportador: true);
                    break;

                case TipoOperacaoMovimentacaoEstoquePallet.ReformaFilial:
                    InserirMovimentacaoEstoqueReforma(dadosMovimentacaoEstoque, isPorTransportador: false);
                    InserirMovimentacaoEstoqueFilial(dadosMovimentacaoEstoque);
                    break;

                case TipoOperacaoMovimentacaoEstoquePallet.ReformaTransportador:
                    InserirMovimentacaoEstoqueReforma(dadosMovimentacaoEstoque, isPorTransportador: true);
                    InserirMovimentacaoEstoqueTransportador(dadosMovimentacaoEstoque);
                    break;

                case TipoOperacaoMovimentacaoEstoquePallet.TransportadorAvaria:
                    InserirMovimentacaoEstoqueTransportador(dadosMovimentacaoEstoque);
                    InserirMovimentacaoEstoqueAvaria(dadosMovimentacaoEstoque, isPorTransportador: true);
                    break;

                case TipoOperacaoMovimentacaoEstoquePallet.TransportadorCliente:
                    InserirMovimentacaoEstoqueTransportador(dadosMovimentacaoEstoque);
                    InserirMovimentacaoEstoqueCliente(dadosMovimentacaoEstoque);
                    break;

                case TipoOperacaoMovimentacaoEstoquePallet.TransportadorEntrada:
                case TipoOperacaoMovimentacaoEstoquePallet.TransportadorSaida:
                    InserirMovimentacaoEstoqueTransportador(dadosMovimentacaoEstoque);
                    break;

                case TipoOperacaoMovimentacaoEstoquePallet.TransportadorFilial:
                    InserirMovimentacaoEstoqueTransportador(dadosMovimentacaoEstoque);
                    InserirMovimentacaoEstoqueFilial(dadosMovimentacaoEstoque);
                    InserirMovimentacaoEstoqueAvariaDevolucao(dadosMovimentacaoEstoque);
                    break;
            }
        }

        #endregion
    }
}
