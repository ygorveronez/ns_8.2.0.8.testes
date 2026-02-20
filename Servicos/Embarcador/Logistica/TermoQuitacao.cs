using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public sealed class TermoQuitacao : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao,
        Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao,
        Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao
    >
    {
        #region Construtores

        public TermoQuitacao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao termoQuitacao, List<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao repositorio = new Repositorio.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;
            DateTime dataCriacao = DateTime.Now;

            foreach (Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao aprovacao = new Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao()
                        {
                            OrigemAprovacao = termoQuitacao,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = dataCriacao,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(termoQuitacao, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao aprovacao = new Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao()
                    {
                        OrigemAprovacao = termoQuitacao,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = dataCriacao,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            termoQuitacao.Situacao = existeRegraSemAprovacao ? SituacaoTermoQuitacao.AguardandoAprovacao : SituacaoTermoQuitacao.Finalizado;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao termoQuitacao)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao>(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao> listaRegras = repositorioRegra.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao>();

            foreach (Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao regra in listaRegras)
            {
                if (regra.RegraPorTransportador && !ValidarAlcadas<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AlcadaTransportador, Dominio.Entidades.Empresa>(regra.AlcadasTransportador, termoQuitacao.Transportador?.Codigo ?? 0))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao termoQuitacao, Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: termoQuitacao.Codigo,
                URLPagina: "Logistica/TermoQuitacao",
                titulo: Localization.Resources.Logistica.TermoQuitacao.TituloTermoQuitacao,
                nota: string.Format(Localization.Resources.Logistica.TermoQuitacao.TransportadorAceitouTermoQuitacao, termoQuitacao.Transportador.Descricao, termoQuitacao.Numero),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao termoQuitacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            RemoverAprovacao(termoQuitacao);

            List<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao> regras = ObterRegrasAutorizacao(termoQuitacao);

            if (regras.Count > 0)
                CriarRegrasAprovacao(termoQuitacao, regras, tipoServicoMultisoftware);
            else
                termoQuitacao.Situacao = SituacaoTermoQuitacao.SemRegraAprovacao;
        }

        #endregion
    }
}
