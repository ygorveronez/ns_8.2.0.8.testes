using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Escrituracao
{
    public sealed class LiberacaoEscrituracaoPagamento : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Cargas.Carga,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga
    >
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;

        #endregion

        #region Construtores

        public LiberacaoEscrituracaoPagamento(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public LiberacaoEscrituracaoPagamento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador): base(unitOfWork)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga repositorio = new Repositorio.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        var aprovacao = new Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga()
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
                    var aprovacao = new Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga()
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
                carga.SituacaoLiberacaoEscrituracaoPagamentoCarga = SituacaoLiberacaoEscrituracaoPagamentoCarga.AguardandoAprovacao;

                NotificarSituacaoAprovacaoAoOperadorCarga(carga, tipoServicoMultisoftware);
            }
            else
                carga.SituacaoLiberacaoEscrituracaoPagamentoCarga = SituacaoLiberacaoEscrituracaoPagamentoCarga.Aprovada;
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

        private List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga>(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga>();
            bool freteCalculadoPorFilialEmissora = (carga.EmpresaFilialEmissora != null && (configuracaoEmbarcador.CalcularFreteFilialEmissoraPorTabelaDeFrete || (carga.TipoOperacao?.CalculaFretePorTabelaFreteFilialEmissora ?? false)));
            decimal valorFrete = freteCalculadoPorFilialEmissora ? carga.ValorFreteAPagarFilialEmissora : carga.ValorFreteAPagar;

            foreach (Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga regra in listaRegras)
            {
                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, carga.Filial?.Codigo))
                    continue;

                if (regra.RegraPorModeloVeicularCarga && !ValidarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AlcadaModeloVeicularCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(regra.AlcadasModeloVeicularCarga, carga.ModeloVeicularCarga?.Codigo))
                    continue;

                if (regra.RegraPorTipoCarga && !ValidarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(regra.AlcadasTipoCarga, carga.TipoDeCarga?.Codigo))
                    continue;

                if (regra.RegraPorTipoOperacao && !ValidarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(regra.AlcadasTipoOperacao, carga.TipoOperacao?.Codigo))
                    continue;

                if (regra.RegraPorValorFrete && !ValidarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AlcadaValorFrete, decimal>(regra.AlcadasValorFrete, valorFrete))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }
      
        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: carga.Codigo,
                URLPagina: "Escrituracao/AutorizacaoLiberacaoEscrituracaoPagamentoCarga",
                titulo: Localization.Resources.Escrituracao.LiberacaoEscrituracaoPagamento.EscrituracaoPagamentoCarga,
                nota: string.Format(Localization.Resources.Escrituracao.LiberacaoEscrituracaoPagamento.CriadaSolicitacaoLiberacaoEscrituracaoPagamentoCarga, carga.CodigoCargaEmbarcador),
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

            if (!IsUtilizarAlcadaAprovacaoLiberacaoEscrituracaoPagamentoCarga())
                return;

            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga> regras = ObterRegrasAutorizacao(carga);

            if (regras.Count > 0)
                CriarRegrasAprovacao(carga, regras, tipoServicoMultisoftware);
            else
                carga.SituacaoLiberacaoEscrituracaoPagamentoCarga = SituacaoLiberacaoEscrituracaoPagamentoCarga.SemRegraAprovacao;
        }

        public bool IsUtilizarAlcadaAprovacaoLiberacaoEscrituracaoPagamentoCarga()
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            return configuracaoEmbarcador?.UtilizarAlcadaAprovacaoLiberacaoEscrituracaoPagamentoCarga ?? false;
        }

        public void NotificarSituacaoAprovacaoAoOperadorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador)
                return;

            string acao = string.Empty;

            if (carga.SituacaoLiberacaoEscrituracaoPagamentoCarga == SituacaoLiberacaoEscrituracaoPagamentoCarga.AguardandoAprovacao)
                acao = Localization.Resources.Gerais.Geral.Criada;
            else if (carga.SituacaoLiberacaoEscrituracaoPagamentoCarga == SituacaoLiberacaoEscrituracaoPagamentoCarga.Aprovada)
                acao = Localization.Resources.Gerais.Geral.Aprovada;
            else
                acao = Localization.Resources.Gerais.Geral.Rejeitada;

            new Carga.Carga(_unitOfWork).NotificarAlteracaoAoOperador(carga, string.Format(Localization.Resources.Cargas.Carga.AcaoSolicitacaoLiberacaoEscrituracaoPagamentoCarga, acao, carga.CodigoCargaEmbarcador), _unitOfWork, tipoServicoMultisoftware);
        }

        #endregion
    }
}
