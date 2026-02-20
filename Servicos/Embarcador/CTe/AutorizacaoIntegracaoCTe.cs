using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.CTe
{
    public sealed class AutorizacaoIntegracaoCTe : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Cargas.Carga,
        Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe,
        Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe
    >
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;

        #endregion

        #region Construtores

        public AutorizacaoIntegracaoCTe(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public AutorizacaoIntegracaoCTe(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador): base(unitOfWork)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe repositorio = new Repositorio.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        var aprovacao = new Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe()
                        {
                            OrigemAprovacao = carga,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = carga.DataCriacaoCarga,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(carga, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    var aprovacao = new Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe()
                    {
                        OrigemAprovacao = carga,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = System.DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = carga.DataCriacaoCarga,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            if (existeRegraSemAprovacao)
            {
                carga.SituacaoAutorizacaoIntegracaoCTe = SituacaoAutorizacaoIntegracaoCTe.AguardandoAprovacao;

                NotificarSituacaoAprovacaoAoOperadorCarga(carga, tipoServicoMultisoftware);
            }
            else
                carga.SituacaoAutorizacaoIntegracaoCTe = SituacaoAutorizacaoIntegracaoCTe.Aprovada;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                _configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            }

            return _configuracaoEmbarcador;
        }

        private List<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe>(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            List<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe>();
            bool freteCalculadoPorFilialEmissora = (carga.EmpresaFilialEmissora != null && (configuracaoEmbarcador.CalcularFreteFilialEmissoraPorTabelaDeFrete || (carga.TipoOperacao?.CalculaFretePorTabelaFreteFilialEmissora ?? false)));
            decimal valorFrete = freteCalculadoPorFilialEmissora ? carga.ValorFreteAPagarFilialEmissora : carga.ValorFreteAPagar;

            foreach (Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe regra in listaRegras)
            {
                if (regra.RegraPorTipoOperacao && !ValidarAlcadas<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(regra.AlcadasTipoOperacao, carga.TipoOperacao?.Codigo))
                    continue;

                if (regra.RegraPorValorFrete && !ValidarAlcadas<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AlcadaValorFrete, decimal>(regra.AlcadasValorFrete, valorFrete))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: carga.Codigo,
                URLPagina: "Cargas/Carga",
                titulo: Localization.Resources.CTes.CTe.EscrituracaoPagamentoCarga,
                nota: string.Format(Localization.Resources.CTes.CTe.CriadaSolicitacaoAprovacaoIntegracaoCTesCarga, carga.CodigoCargaEmbarcador),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            RemoverAprovacao(carga);

            List<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe> regras = ObterRegrasAutorizacao(carga);

            if (regras.Count > 0)
                CriarRegrasAprovacao(carga, regras, tipoServicoMultisoftware);
            else
                carga.SituacaoAutorizacaoIntegracaoCTe = SituacaoAutorizacaoIntegracaoCTe.NaoInformada;
        }

        public void NotificarSituacaoAprovacaoAoOperadorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador)
                return;

            string acao = string.Empty;

            if (carga.SituacaoAutorizacaoIntegracaoCTe == SituacaoAutorizacaoIntegracaoCTe.AguardandoAprovacao)
                acao = Localization.Resources.Gerais.Geral.Criada;
            else if (carga.SituacaoAutorizacaoIntegracaoCTe == SituacaoAutorizacaoIntegracaoCTe.Aprovada)
                acao = Localization.Resources.Gerais.Geral.Aprovada;
            else
                acao = Localization.Resources.Gerais.Geral.Rejeitada;

            new Carga.Carga(_unitOfWork).NotificarAlteracaoAoOperador(carga, string.Format(Localization.Resources.Cargas.Carga.AcaoSolicitacaoAutorizacaoIntegracaoCTesCarga, acao, carga.CodigoCargaEmbarcador), _unitOfWork, tipoServicoMultisoftware);
        }

        #endregion
    }
}
