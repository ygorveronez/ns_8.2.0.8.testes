using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.Logistica
{
    public sealed class PrazoSituacaoCarga
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public PrazoSituacaoCarga(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public PrazoSituacaoCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoPadrao()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador =  new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private SituacaoCargaJanelaCarregamento? ObterProximaSituacao(SituacaoCargaJanelaCarregamento situacaoAtual)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoPadrao();

            switch (situacaoAtual)
            {
                case SituacaoCargaJanelaCarregamento.AgAprovacaoComercial: return SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores;
                case SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores: return SituacaoCargaJanelaCarregamento.SemValorFrete;
                case SituacaoCargaJanelaCarregamento.SemValorFrete: return SituacaoCargaJanelaCarregamento.SemTransportador;
                case SituacaoCargaJanelaCarregamento.SemTransportador: return SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador;
                case SituacaoCargaJanelaCarregamento.AgAceiteTransportador: return SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador;
                case SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador: return (configuracaoEmbarcador?.UtilizarFilaCarregamento ?? false) ? SituacaoCargaJanelaCarregamento.AgEncosta : SituacaoCargaJanelaCarregamento.ProntaParaCarregamento;
                case SituacaoCargaJanelaCarregamento.AgEncosta: return SituacaoCargaJanelaCarregamento.ProntaParaCarregamento;
                default: return null;
            }
        }

        #endregion

        #region Métodos Públicos

        public DateTime ObterDataProximaSituacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.LiberaradaParaFaturamentePeloTransportador)
                return cargaJanelaCarregamento.DataProximaSituacao;

            SituacaoCargaJanelaCarregamento? proximaSituacao = ObterProximaSituacao(cargaJanelaCarregamento.Situacao);

            if (!proximaSituacao.HasValue)
                return cargaJanelaCarregamento.InicioCarregamento;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoPadrao();

            if (configuracaoEmbarcador.UtilizarFilaCarregamento && cargaJanelaCarregamento.CentroCarregamento != null)
            {
                if (proximaSituacao.Value == SituacaoCargaJanelaCarregamento.AgEncosta)
                {
                    if (cargaJanelaCarregamento.CentroCarregamento.TempoAguardarConfirmacaoTransportador > 0)
                        return DateTime.Now.AddMinutes(cargaJanelaCarregamento.CentroCarregamento.TempoAguardarConfirmacaoTransportador);
                }
                else if (proximaSituacao.Value == SituacaoCargaJanelaCarregamento.ProntaParaCarregamento)
                {
                    if (cargaJanelaCarregamento.CentroCarregamento.TempoEncostaDoca > 0)
                        return DateTime.Now.AddMinutes(cargaJanelaCarregamento.CentroCarregamento.TempoEncostaDoca);
                }
            }

            int tempoSituacao = ObterTempo(proximaSituacao.Value, tempoSituacaoPadrao: 0);

            return cargaJanelaCarregamento.InicioCarregamento.AddMinutes(-tempoSituacao);
        }

        public Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga ObterPrazoSituacaoCarga(SituacaoCargaJanelaCarregamento situacao)
        {
            Repositorio.Embarcador.Logistica.PrazoSituacaoCarga repositorioPrazoSituacaoCarga = new Repositorio.Embarcador.Logistica.PrazoSituacaoCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga prazoSituacao = repositorioPrazoSituacaoCarga.BuscarPorSituacao(situacao);

            return prazoSituacao;
        }

        public int ObterTempo(SituacaoCargaJanelaCarregamento situacao, int tempoSituacaoPadrao)
        {
            Repositorio.Embarcador.Logistica.PrazoSituacaoCarga repositorioPrazoSituacaoCarga = new Repositorio.Embarcador.Logistica.PrazoSituacaoCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga prazoSituacao = repositorioPrazoSituacaoCarga.BuscarPorSituacao(situacao);

            return prazoSituacao?.Tempo ?? tempoSituacaoPadrao;
        }

        #endregion
    }
}
