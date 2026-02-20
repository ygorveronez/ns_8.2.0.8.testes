using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Bidding
{
    public class BiddingConviteAprovacao : RegraAutorizacao.AprovacaoAlcada
        <
        Dominio.Entidades.Embarcador.Bidding.BiddingConvite,
        Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding,
        Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite
        >
    {
        #region Construtores

        public BiddingConviteAprovacao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public void EtapaAprovacao(Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding> regras = ObterRegrasAutorizacao(biddingConvite);

            if (regras.Count > 0)
                CriarRegrasAprovacao(biddingConvite, regras, tipoServicoMultisoftware);
            else
                biddingConvite.Status = StatusBiddingConvite.SemRegra;
        }

        public static object ObterDetalhesAprovacao(Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite repositorioAprovacao = new Repositorio.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite(unitOfWork);
            int aprovacoes = repositorioAprovacao.ContarAprovacoes(biddingConvite.Codigo);
            int aprovacoesNecessarias = repositorioAprovacao.ContarAprovacoesNecessarias(biddingConvite.Codigo);
            int reprovacoes = repositorioAprovacao.ContarReprovacoes(biddingConvite.Codigo);

            return new
            {
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                Reprovacoes = reprovacoes,
                biddingConvite.Descricao,
                Status = biddingConvite.Status,
                biddingConvite.Codigo
            };
        }

        #endregion Métodos Públicos

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite, Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: biddingConvite.Codigo,
                URLPagina: "Bidding/AutorizacaoBiddingConvite",
                titulo: Localization.Resources.Frotas.OrdemServico.TituloOrdemServico,
                nota: string.Format("Criada Solicitacao Aprovacao Bidding Convite: ", biddingConvite.Codigo),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding>(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding>();

            foreach (Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding regra in listaRegras)
            {
                if (regra.RegraPorTipoBidding && !ValidarAlcadas<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.AlcadaTipoBidding, Dominio.Entidades.Embarcador.Bidding.TipoBidding>(regra.AlcadasTipoBidding, biddingConvite.TipoBidding.Codigo))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite, List<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite repositorio = new Repositorio.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        var aprovacao = new Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite()
                        {
                            OrigemAprovacao = biddingConvite,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = biddingConvite.DataInicio,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(biddingConvite, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    var aprovacao = new Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite()
                    {
                        OrigemAprovacao = biddingConvite,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = biddingConvite.DataInicio
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            biddingConvite.Status = existeRegraSemAprovacao ? StatusBiddingConvite.AguardandoAprovacao : StatusBiddingConvite.Aguardando;
        }

        #endregion Métodos Privados

    }
}
