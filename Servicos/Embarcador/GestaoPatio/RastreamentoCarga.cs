using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class RastreamentoCarga : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga
    {
        #region Construtores

        public RastreamentoCarga(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null, cliente: null) { }

        public RastreamentoCarga(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public RastreamentoCarga(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.Posicao, cliente) { }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            Repositorio.Embarcador.GestaoPatio.RastreamentoCarga repositorioRastreamentoCarga = new Repositorio.Embarcador.GestaoPatio.RastreamentoCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga rastreamentoCarga = repositorioRastreamentoCarga.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (rastreamentoCarga != null)
                return;
            
            rastreamentoCarga = new Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga()
            {
                FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                EtapaRastreamentoLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada,
                PrevisaoInicio = fluxoGestaoPatioEtapaAdicionar.DataPrevisaoInicio
            };

            repositorioRastreamentoCarga.Inserir(rastreamentoCarga);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.GestaoPatio.RastreamentoCarga repositorioRastreamentoCarga = new Repositorio.Embarcador.GestaoPatio.RastreamentoCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga rastreamentoCarga = repositorioRastreamentoCarga.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (rastreamentoCarga != null)
            {
                rastreamentoCarga.Carga = carga;
                repositorioRastreamentoCarga.Atualizar(rastreamentoCarga);
            }
        }

        public void ProcessarMacroRecebida(Dominio.Entidades.Embarcador.Veiculos.MacroVeiculo macroVeiculo)
        {
            Repositorio.Embarcador.GestaoPatio.RastreamentoCarga repositorioRastreamentoCarga = new Repositorio.Embarcador.GestaoPatio.RastreamentoCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga rastreamentoCarga = repositorioRastreamentoCarga.BuscarPorVeiculoDaCarga(macroVeiculo.Veiculo.Codigo);

            if (rastreamentoCarga == null)
                return;

            rastreamentoCarga.UltimaAtualizacao = macroVeiculo.DataMacro;
            rastreamentoCarga.SetLatitude(macroVeiculo.Latitude);
            rastreamentoCarga.SetLongitude(macroVeiculo.Longitude);

            repositorioRastreamentoCarga.Atualizar(rastreamentoCarga);
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.RastreamentoCarga repositorioRastreamentoCarga = new Repositorio.Embarcador.GestaoPatio.RastreamentoCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga rastreamentoCarga = repositorioRastreamentoCarga.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (rastreamentoCarga != null)
            {
                rastreamentoCarga.Carga = cargaNova;
                repositorioRastreamentoCarga.Atualizar(rastreamentoCarga);
            }
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataPosicaoPrevista.HasValue)
                fluxoGestaoPatio.DataPosicaoPrevista = preSetTempoEtapa.DataPosicaoPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            LiberarProximaEtapa(fluxoGestaoPatio);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataPosicaoPrevista = preSetTempoEtapa.DataPosicaoPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataPosicao.HasValue)
                return false;

            fluxoGestaoPatio.TempoAgPosicao = tempoEtapaAnterior;
            fluxoGestaoPatio.DataPosicao = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.RastreamentoCarga repositorioRastreamentoCarga = new Repositorio.Embarcador.GestaoPatio.RastreamentoCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga rastreamentoCarga = repositorioRastreamentoCarga.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (rastreamentoCarga != null)
            {
                rastreamentoCarga.EtapaRastreamentoLiberada = true;
                repositorioRastreamentoCarga.Atualizar(rastreamentoCarga);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataPosicao;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataPosicaoPrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.RastreamentoCarga repositorioRastreamentoCarga = new Repositorio.Embarcador.GestaoPatio.RastreamentoCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga rastreamentoCarga = repositorioRastreamentoCarga.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (rastreamentoCarga != null)
            {
                rastreamentoCarga.EtapaRastreamentoLiberada = false;
                repositorioRastreamentoCarga.Atualizar(rastreamentoCarga);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAgPosicao = 0;
            fluxoGestaoPatio.DataPosicao = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataPosicaoPrevista.HasValue)
                fluxoGestaoPatio.DataPosicaoReprogramada = fluxoGestaoPatio.DataPosicaoPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
