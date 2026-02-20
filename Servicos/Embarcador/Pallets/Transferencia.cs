using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pallets
{
    public sealed class Transferencia : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet,
        Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.RegraAutorizacaoTransferenciaPallet,
        Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AprovacaoAlcadaTransferenciaPallet
    >
    {
        #region Construtores

        public Transferencia(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork)  { }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet transferencia, List<Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.RegraAutorizacaoTransferenciaPallet> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var existeRegraSemAprovacao = false;
            var repositorio = new Repositorio.Embarcador.Pallets.AlcadasTransferenciaPallets.AprovacaoAlcadaTransferenciaPallet(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (var regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AprovacaoAlcadaTransferenciaPallet aprovacao = new Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AprovacaoAlcadaTransferenciaPallet()
                        {
                            OrigemAprovacao = transferencia,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente,
                            DataCriacao = transferencia.Solicitacao.Data,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(transferencia, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    var aprovacao = new Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AprovacaoAlcadaTransferenciaPallet()
                    {
                        OrigemAprovacao = transferencia,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada,
                        Data = System.DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = transferencia.Solicitacao.Data,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            transferencia.Situacao = existeRegraSemAprovacao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransferenciaPallet.AguardandoAprovacao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransferenciaPallet.AguardandoRecebimento;
        }

        private List<Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.RegraAutorizacaoTransferenciaPallet> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet transferencia)
        {
            var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.RegraAutorizacaoTransferenciaPallet>(_unitOfWork);
            var listaRegras = repositorioRegra.BuscarPorAtiva();
            var listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.RegraAutorizacaoTransferenciaPallet>();

            foreach (var regra in listaRegras)
            {
                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, transferencia.Solicitacao.Filial.Codigo))
                    continue;

                if (regra.RegraPorQuantidade && !ValidarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AlcadaQuantidade, int>(regra.AlcadasQuantidade, transferencia.Solicitacao.Quantidade))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet transferencia, Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AprovacaoAlcadaTransferenciaPallet aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: transferencia.Codigo,
                URLPagina: "Pallets/TransferenciaPallet",
                titulo: Localization.Resources.Pallets.Transferencia.TransferenciaPallets,
                nota: string.Format(Localization.Resources.Pallets.Transferencia.SolicitanteCriouSolicitacaoTransferenciaPallets, transferencia.Solicitacao.Solicitante, transferencia.Codigo),
                icone: Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra,
                tipoNotificacao: Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void EtapaAprovacao(Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet transferencia, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var regras = ObterRegrasAutorizacao(transferencia);

            if (regras.Count > 0)
            {
                CriarRegrasAprovacao(transferencia, regras, tipoServicoMultisoftware);
                InserirMovimentacaoEstoqueEnvio(transferencia);
            }
            else
                transferencia.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransferenciaPallet.SemRegraAprovacao;
        }

        public void InserirMovimentacaoEstoqueEnvio(Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet transferencia)
        {
            if (transferencia.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransferenciaPallet.AguardandoRecebimento)
            {
                var movimentacaoSaida = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
                {
                    CodigoFilial = transferencia.Solicitacao.Filial.Codigo,
                    CodigoSetor = transferencia.Solicitacao.Setor.Codigo,
                    Quantidade = transferencia.Envio.Quantidade,
                    TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                    TipoOperacaoMovimentacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.FilialSaida
                };

                new EstoquePallet(_unitOfWork).InserirMovimentacao(movimentacaoSaida);
            }
        }

        #endregion
    }
}
