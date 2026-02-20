using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class Faturamento : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga, IFluxoGestaoPatioEtapaLiberarAutomaticamente
    {
        #region Construtores

        public Faturamento(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public Faturamento(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.Faturamento, cliente) { }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            if (fluxoGestaoPatioEtapaAdicionar.EtapaLiberada && (fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga != null))
            {
                fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga.EtapaFaturamentoLiberado = true;

                new Repositorio.Embarcador.Cargas.Carga(_unitOfWork).Atualizar(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga);
            }
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            if (etapaLiberada && (carga != null))
            {
                carga.EtapaFaturamentoLiberado = true;

                new Repositorio.Embarcador.Cargas.Carga(_unitOfWork).Atualizar(carga);
            }
        }

        public void LiberarProximaEtapaAutomaticamente(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if ((fluxoGestaoPatio.Carga == null) || fluxoGestaoPatio.Carga.SituacaoCarga.IsSituacaoCargaNaoFaturada())
                return;

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();

            if (configuracaoGestaoPatio.FaturamentoPermiteAvancarAutomaticamenteAposGerarDocumentos)
                LiberarProximaEtapa(fluxoGestaoPatio);
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            if (fluxoGestaoPatio.Carga.EtapaFaturamentoLiberado && !cargaNova.EtapaFaturamentoLiberado)
            {
                cargaNova.EtapaFaturamentoLiberado = true;

                new Repositorio.Embarcador.Cargas.Carga(_unitOfWork).Atualizar(cargaNova);
            }
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataFaturamentoPrevista.HasValue)
                fluxoGestaoPatio.DataFaturamentoPrevista = preSetTempoEtapa.DataFaturamentoPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            LiberarProximaEtapa(fluxoGestaoPatio);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataFaturamentoPrevista = preSetTempoEtapa.DataFaturamentoPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataFaturamento.HasValue)
                return false;

            fluxoGestaoPatio.TempoAgFaturamento = tempoEtapaAnterior;
            fluxoGestaoPatio.DataFaturamento = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if (fluxoGestaoPatio.CargaBase.IsCarga())
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = (Dominio.Entidades.Embarcador.Cargas.Carga)fluxoGestaoPatio.CargaBase;

                carga.EtapaFaturamentoLiberado = true;

                new Repositorio.Embarcador.Cargas.Carga(_unitOfWork).Atualizar(carga);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataFaturamento;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataFaturamentoPrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if (fluxoGestaoPatio.CargaBase.IsCarga())
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = (Dominio.Entidades.Embarcador.Cargas.Carga)fluxoGestaoPatio.CargaBase;

                carga.EtapaFaturamentoLiberado = false;

                new Repositorio.Embarcador.Cargas.Carga(_unitOfWork).Atualizar(carga);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAgFaturamento = 0;
            fluxoGestaoPatio.DataFaturamento = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataFaturamentoPrevista.HasValue)
                fluxoGestaoPatio.DataFaturamentoReprogramada = fluxoGestaoPatio.DataFaturamentoPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
