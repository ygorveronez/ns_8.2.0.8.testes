using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public abstract class FluxoGestaoPatioEtapa
    {
        #region Atributos Privados

        private readonly EtapaFluxoGestaoPatio _etapaFluxo;

        #endregion Atributos Privados

        #region Atributos Protegidos

        protected readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        protected readonly Repositorio.UnitOfWork _unitOfWork;
        protected readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _cliente;

        #endregion Atributos Protegidos

        #region Construtores

        public FluxoGestaoPatioEtapa(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, EtapaFluxoGestaoPatio etapaFluxo, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            _auditado = auditado;
            _etapaFluxo = etapaFluxo;
            _unitOfWork = unitOfWork;
            _cliente = cliente;
        }

        #endregion Construtores

        #region Métodos Protegidos

        protected void LiberarProximaEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual != _etapaFluxo && (!ObterConfiguracaoGestaoPatio().IsPermiteAnteciparEtapa(_etapaFluxo) || fluxoGestaoPatio.GetEtapas().FindIndex(o => o.EtapaFluxoGestaoPatio == _etapaFluxo) < fluxoGestaoPatio.EtapaAtual))
                return;

            FluxoGestaoPatio servicoFluxoGestaoPatio = new FluxoGestaoPatio(_unitOfWork, _auditado, _cliente);

            servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, _etapaFluxo);
        }

        protected Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio ObterConfiguracaoGestaoPatio()
        {
            FluxoGestaoPatioConfiguracao servicoFluxoGestaoPatioConfiguracao = new FluxoGestaoPatioConfiguracao(_unitOfWork);

            return servicoFluxoGestaoPatioConfiguracao.ObterConfiguracao();
        }

        #endregion Métodos Protegidos

        #region Métodos Públicos

        public int? ObterTempoExcedido(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime? dataEtapaAnterior)
        {
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();

            DateTime? dataEtapaLiberada = configuracaoGestaoPatio.UtilizarDataPrevistaEtapaAtualAtivarAlerta
                ? ObterDataPrevista(fluxoGestaoPatio)
                : dataEtapaAnterior ?? ObterDataPrevista(fluxoGestaoPatio);

            if (!dataEtapaLiberada.HasValue)
                return null;

            return (int)(DateTime.Now - dataEtapaLiberada.Value).TotalMinutes;
        }

        #endregion Métodos Públicos

        #region Métodos Públicos Abstratos

        public abstract void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa);

        public abstract void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio);

        public abstract void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa);

        public abstract bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataFluxo, decimal tempoEtapaAnterior);

        public abstract bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio);

        public abstract DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio);

        public abstract DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio);

        public abstract void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio);

        public abstract void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio);

        public abstract void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar);

        #endregion Métodos Públicos Abstratos
    }
}
