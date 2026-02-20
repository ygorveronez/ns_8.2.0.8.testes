using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.ICMS
{
    public sealed class RegraICMSAprovacao : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.ICMS.RegraICMS,
        Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS,
        Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.AprovacaoAlcadaAlteracaoRegraICMS
    >
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;

        #endregion

        #region Construtores

        public RegraICMSAprovacao(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public RegraICMSAprovacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : base(unitOfWork)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS, List<Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.AprovacaoAlcadaAlteracaoRegraICMS repositorio = new Repositorio.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.AprovacaoAlcadaAlteracaoRegraICMS(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.AprovacaoAlcadaAlteracaoRegraICMS aprovacao = new Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.AprovacaoAlcadaAlteracaoRegraICMS()
                        {
                            OrigemAprovacao = regraICMS,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = regraICMS.DataAlteracao,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(regraICMS, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.AprovacaoAlcadaAlteracaoRegraICMS aprovacao = new Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.AprovacaoAlcadaAlteracaoRegraICMS()
                    {
                        OrigemAprovacao = regraICMS,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = System.DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = regraICMS.DataAlteracao,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            regraICMS.SituacaoAlteracao = existeRegraSemAprovacao ? SituacaoAlteracaoRegraICMS.AguardandoAprovacao : SituacaoAlteracaoRegraICMS.Aprovada;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private List<Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS> ObterRegrasAutorizacao()
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS>(_unitOfWork);
            List<Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();

            return listaRegras;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS, Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.AprovacaoAlcadaAlteracaoRegraICMS aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: regraICMS.Codigo,
                URLPagina: "ICMS/RegraICMS",
                titulo: Localization.Resources.ICMS.RegraICMS.ConfiguracaoRegraICMS,
                nota: string.Format(Localization.Resources.ICMS.RegraICMS.CriadaSolicitacaoAlteracaoConfiguracaoRegraICMS, regraICMS.Descricao),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void CriarAprovacao(Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            RemoverAprovacao(regraICMS);

            List<Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS> regras = ObterRegrasAutorizacao();

            if (regras.Count > 0)
                CriarRegrasAprovacao(regraICMS, regras, tipoServicoMultisoftware);
            else
                regraICMS.SituacaoAlteracao = SituacaoAlteracaoRegraICMS.SemRegraAprovacao;

            if (regraICMS.SituacaoAlteracao == SituacaoAlteracaoRegraICMS.Aprovada)
                new Carga.ICMS().AplicarAlteracoes(regraICMS, _unitOfWork);
        }

        public bool UtilizarAlcadaAprovacaoAlteracaoRegraICMS()
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            return configuracaoEmbarcador?.UtilizarAlcadaAprovacaoAlteracaoRegraICMS ?? false;
        }

        #endregion
    }
}
