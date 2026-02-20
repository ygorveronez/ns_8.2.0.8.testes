using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class ChegadaVeiculo : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga, IFluxoGestaoPatioEtapaLiberarAutomaticamente, IFluxoGestaoPatioEtapaRetornada
    {
        #region Atributos Privados

        private readonly GuaritaBase _guaritaBase;

        #endregion

        #region Construtores

        public ChegadaVeiculo(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public ChegadaVeiculo(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.ChegadaVeiculo, cliente)
        {
            _guaritaBase = new GuaritaBase(unitOfWork, auditado);
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            _guaritaBase.Adicionar(fluxoGestaoPatioEtapaAdicionar, EtapaFluxoGestaoPatio.ChegadaVeiculo);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            _guaritaBase.DefinirCarga(fluxoGestaoPatio, carga, etapaLiberada);
        }

        public void EtapaRetornada(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (guarita != null)
            {
                guarita.Situacao = SituacaoCargaGuarita.AgChegadaVeiculo;
                guarita.DataChegadaVeiculo = null;

                repositorioGuarita.Atualizar(guarita);
            }
        }

        public void LiberarProximaEtapaAutomaticamente(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (guarita == null)
                return;

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();

            if (configuracaoGestaoPatio.ChegadaVeiculoPermiteInformarComEtapaBloqueada && guarita.DataChegadaVeiculo.HasValue)
                LiberarProximaEtapa(fluxoGestaoPatio);
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            _guaritaBase.TrocarCarga(fluxoGestaoPatio, cargaNova);
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataChegadaVeiculoPrevista.HasValue)
                fluxoGestaoPatio.DataChegadaVeiculoPrevista = preSetTempoEtapa.DataChegadaVeiculoPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (guarita == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            guarita.Initialize();
            guarita.DataChegadaVeiculo = DateTime.Now;
            guarita.Situacao = SituacaoCargaGuarita.AguardandoLiberacao;

            LiberarProximaEtapa(fluxoGestaoPatio);
            repositorioGuarita.Atualizar(guarita, _auditado);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataChegadaVeiculoPrevista = preSetTempoEtapa.DataChegadaVeiculoPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataChegadaVeiculo.HasValue)
                return false;

            fluxoGestaoPatio.TempoAgChegadaVeiculo = tempoEtapaAnterior;
            fluxoGestaoPatio.DataChegadaVeiculo = data;

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataChegadaVeiculo;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataChegadaVeiculoPrevista;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (guarita == null)
                return false;

            if (fluxoGestaoPatio.CargaBase.Veiculo == null)
                return false;

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();
            bool chegadaVeiculoInformada = configuracaoGestaoPatio.ChegadaVeiculoPermiteInformarComEtapaBloqueada && guarita.DataChegadaVeiculo.HasValue;

            guarita.EtapaGuaritaLiberada = true;

            if (!chegadaVeiculoInformada)
                guarita.Situacao = SituacaoCargaGuarita.AgChegadaVeiculo;

            repositorioGuarita.Atualizar(guarita);

            return true;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (guarita == null || fluxoGestaoPatio.CargaBase.Veiculo == null)
                return;

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();
            bool chegadaVeiculoInformada = configuracaoGestaoPatio.ChegadaVeiculoPermiteInformarComEtapaBloqueada && guarita.DataChegadaVeiculo.HasValue;

            guarita.EtapaGuaritaLiberada = false;

            if (!chegadaVeiculoInformada)
                guarita.Situacao = SituacaoCargaGuarita.AgChegadaVeiculo;

            repositorioGuarita.Atualizar(guarita);
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAgChegadaVeiculo = 0;
            fluxoGestaoPatio.DataChegadaVeiculo = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataChegadaVeiculoPrevista.HasValue)
                fluxoGestaoPatio.DataChegadaVeiculoReprogramada = fluxoGestaoPatio.DataChegadaVeiculoPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
