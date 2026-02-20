using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Documentos
{
    public sealed class GestaoDocumentoAprovacao : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Documentos.GestaoDocumento,
        Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento,
        Dominio.Entidades.Embarcador.Documentos.Alcadas.AprovacaoAlcadaGestaoDocumento
    >
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;

        #endregion

        #region Construtores

        public GestaoDocumentoAprovacao(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public GestaoDocumentoAprovacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : base(unitOfWork)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento, List<Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Documentos.Alcadas.AprovacaoAlcadaGestaoDocumento repositorio = new Repositorio.Embarcador.Documentos.Alcadas.AprovacaoAlcadaGestaoDocumento(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;
            DateTime dataBase = DateTime.Now;

            foreach (Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Documentos.Alcadas.AprovacaoAlcadaGestaoDocumento aprovacao = new Dominio.Entidades.Embarcador.Documentos.Alcadas.AprovacaoAlcadaGestaoDocumento()
                        {
                            OrigemAprovacao = gestaoDocumento,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = dataBase,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(gestaoDocumento, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Documentos.Alcadas.AprovacaoAlcadaGestaoDocumento aprovacao = new Dominio.Entidades.Embarcador.Documentos.Alcadas.AprovacaoAlcadaGestaoDocumento()
                    {
                        OrigemAprovacao = gestaoDocumento,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = dataBase,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = dataBase,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            gestaoDocumento.SituacaoGestaoDocumento = existeRegraSemAprovacao ? SituacaoGestaoDocumento.AguardandoAprovacao : SituacaoGestaoDocumento.Aprovado;
        }

        private List<int> ObterCodigosFiliais(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento)
        {
            int codigoFilial = gestaoDocumento.CargaCTe?.Carga?.Filial?.Codigo ?? 0;
            int codigoFilialOcorrencia = gestaoDocumento.CargaCTeComplementoInfo?.CargaOcorrencia?.Carga?.Filial?.Codigo ?? 0;
            List<int> codigosFiliais = new List<int>();

            if (codigoFilial > 0)
                codigosFiliais.Add(codigoFilial);

            if (codigoFilialOcorrencia > 0)
                codigosFiliais.Add(codigoFilialOcorrencia);

            return codigosFiliais;
        }

        private List<int> ObterCodigosTiposOperacao(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento)
        {
            int codigoTipoOperacao = gestaoDocumento.CargaCTe?.Carga?.TipoOperacao?.Codigo ?? 0;
            int codigoTipoOperacaoOcorrencia = gestaoDocumento.CargaCTeComplementoInfo?.CargaOcorrencia?.Carga?.TipoOperacao?.Codigo ?? 0;
            List<int> codigosTiposOperacao = new List<int>();

            if (codigoTipoOperacao > 0)
                codigosTiposOperacao.Add(codigoTipoOperacao);

            if (codigoTipoOperacaoOcorrencia > 0)
                codigosTiposOperacao.Add(codigoTipoOperacaoOcorrencia);

            return codigosTiposOperacao;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private List<Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento>(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento>();
            List<int> codigosFiliais = ObterCodigosFiliais(gestaoDocumento);
            List<int> codigosTiposOperacao = ObterCodigosTiposOperacao(gestaoDocumento);

            foreach (Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento regra in listaRegras)
            {
                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, codigosFiliais))
                    continue;

                if (regra.RegraPorMotivoRejeicao && !ValidarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaMotivoRejeicao, MotivoInconsistenciaGestaoDocumento>(regra.AlcadasMotivoRejeicao, gestaoDocumento.MotivoInconsistenciaGestaoDocumento))
                    continue;

                if (regra.RegraPorTipoOperacao && !ValidarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(regra.AlcadasTipoOperacao, codigosTiposOperacao))
                    continue;

                if (regra.RegraPorTomador && !ValidarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaTomador, Dominio.Entidades.Cliente>(regra.AlcadasTomador, gestaoDocumento.CTe.TomadorPagador?.Cliente?.Codigo))
                    continue;

                if (regra.RegraPorTransportador && !ValidarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaTransportador, Dominio.Entidades.Empresa>(regra.AlcadasTransportador, gestaoDocumento.CTe.Empresa?.Codigo))
                    continue;

                if (regra.RegraPorValorPagamento && !ValidarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaValorPagamento, decimal>(regra.AlcadasValorPagamento, gestaoDocumento.CTe.ValorAReceber))
                    continue;

                if (regra.RegraPorCanalEntrega && !ValidarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaCanalEntrega, Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>(regra.AlcadasCanalEntrega, gestaoDocumento.PreCTe?.CodigoCanalEntrega))
                    continue;

                if (regra.RegraPorPeso && !ValidarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaPeso, decimal>(regra.AlcadasPeso, gestaoDocumento.CTe.Peso))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        private bool UtilizarAlcadaAprovacaoGestaoDocumentos()
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            return configuracaoEmbarcador?.UsarAlcadaAprovacaoGestaoDocumentos ?? false;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento, Dominio.Entidades.Embarcador.Documentos.Alcadas.AprovacaoAlcadaGestaoDocumento aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: gestaoDocumento.Codigo,
                URLPagina: "Documentos/GestaoDocumento",
                titulo: Localization.Resources.Documentos.GestaoDocumentoAprovacao.GestaoDocumento,
                nota: string.Format(Localization.Resources.Documentos.GestaoDocumentoAprovacao.CriadaSolicitacaoAprovacaoGestaoDocumento, gestaoDocumento.Descricao),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            RemoverAprovacao(gestaoDocumento);

            if (!UtilizarAlcadaAprovacaoGestaoDocumentos())
                return;

            List<Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento> regras = ObterRegrasAutorizacao(gestaoDocumento);

            if (regras.Count > 0)
                CriarRegrasAprovacao(gestaoDocumento, regras, tipoServicoMultisoftware);
            else
                gestaoDocumento.SituacaoGestaoDocumento = SituacaoGestaoDocumento.SemRegraAprovacao;
        }

        #endregion
    }
}
