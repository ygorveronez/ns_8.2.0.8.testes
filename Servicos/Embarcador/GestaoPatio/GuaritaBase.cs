using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    sealed class GuaritaBase
    {
        #region Atributos Protegidos

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public GuaritaBase(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar, EtapaFluxoGestaoPatio etapaFluxo)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (guarita != null)
                return;

            FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio);
            DateTime dataProgramadaParaChegada = fluxoGestaoPatioEtapaAdicionar.CargaJanelaCarregamento != null ? fluxoGestaoPatioEtapaAdicionar.CargaJanelaCarregamento.InicioCarregamento : DateTime.Now;
            
            guarita = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita()
            {
                FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                HorarioEntradaDefinido = true,
                CargaJanelaCarregamento = fluxoGestaoPatioEtapaAdicionar.CargaJanelaCarregamento,
                EtapaGuaritaLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada,
                DataProgramadaParaChegada = dataProgramadaParaChegada,
                TipoChegadaGuarita = TipoChegadaGuarita.Carregamento,
                Situacao = (sequenciaGestaoPatio?.ChegadaVeiculo ?? false) ? SituacaoCargaGuarita.AgChegadaVeiculo : SituacaoCargaGuarita.AguardandoLiberacao
            };

            repositorioGuarita.Inserir(guarita);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (guarita != null)
            {
                guarita.Carga = carga;
                repositorioGuarita.Atualizar(guarita);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (guarita != null)
            {
                guarita.Carga = cargaNova;
                repositorioGuarita.Atualizar(guarita);
            }
        }
        
        #endregion
    }
}