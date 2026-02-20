using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class MontagemCargaPatio : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga
    {
        #region Construtores

        public MontagemCargaPatio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public MontagemCargaPatio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.MontagemCarga, cliente) { }

        #endregion

        #region Métodos Privados

        private void AvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.MontagemCargaPatioAvancar montagemCargaPatioAvancar)
        {
            if (montagemCargaPatio == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (!montagemCargaPatio.EtapaMontagemCargaLiberada)
                throw new ServicoException("A liberação da montagem de carga do pátio ainda não foi autorizada.");

            montagemCargaPatio.Initialize();
            montagemCargaPatio.DataMontagemCargaIniciada = DateTime.Now;
            montagemCargaPatio.Situacao = SituacaoMontagemCargaPatio.MontagemCargaFinalizada;

            if (montagemCargaPatioAvancar != null)
            {
                montagemCargaPatio.QuantidadeCaixas = montagemCargaPatioAvancar.QuantidadeCaixas;
                montagemCargaPatio.QuantidadeItens = montagemCargaPatioAvancar.QuantidadeItens;
                montagemCargaPatio.QuantidadePalletsFracionados = montagemCargaPatioAvancar.QuantidadePalletsFracionados;
                montagemCargaPatio.QuantidadePalletsInteiros = montagemCargaPatioAvancar.QuantidadePalletsInteiros;
            }

            LiberarProximaEtapa(fluxoGestaoPatio);

            new Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio(_unitOfWork).Atualizar(montagemCargaPatio, _auditado);
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio repositorioMontagemCargaPatio = new Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio = repositorioMontagemCargaPatio.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (montagemCargaPatio != null)
                return;

            montagemCargaPatio = new Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio()
            {
                FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                EtapaMontagemCargaLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada,
                Situacao = SituacaoMontagemCargaPatio.AguardandoMontagemCarga
            };

            repositorioMontagemCargaPatio.Inserir(montagemCargaPatio);
        }

        public void Avancar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.MontagemCargaPatioAvancar montagemCargaPatioAvancar)
        {
            Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio repositorioMontagemCargaPatio = new Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio = repositorioMontagemCargaPatio.BuscarPorCodigo(montagemCargaPatioAvancar.Codigo);

            AvancarEtapa(montagemCargaPatio?.FluxoGestaoPatio, montagemCargaPatio, montagemCargaPatioAvancar);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio repositorioMontagemCargaPatio = new Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio = repositorioMontagemCargaPatio.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (montagemCargaPatio != null)
            {
                montagemCargaPatio.Carga = carga;
                repositorioMontagemCargaPatio.Atualizar(montagemCargaPatio);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio repositorioMontagemCargaPatio = new Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio = repositorioMontagemCargaPatio.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (montagemCargaPatio != null)
            {
                montagemCargaPatio.Carga = cargaNova;
                repositorioMontagemCargaPatio.Atualizar(montagemCargaPatio);
            }
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataMontagemCargaPrevista.HasValue)
                fluxoGestaoPatio.DataMontagemCargaPrevista = preSetTempoEtapa.DataMontagemCargaPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio repositorioMontagemCargaPatio = new Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio = repositorioMontagemCargaPatio.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            AvancarEtapa(fluxoGestaoPatio, montagemCargaPatio, montagemCargaPatioAvancar: null);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataMontagemCargaPrevista = preSetTempoEtapa.DataMontagemCargaPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataMontagemCarga.HasValue)
                return false;

            fluxoGestaoPatio.TempoAguardandoMontagemCarga = tempoEtapaAnterior;
            fluxoGestaoPatio.DataMontagemCarga = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio repositorioMontagemCargaPatio = new Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio = repositorioMontagemCargaPatio.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (montagemCargaPatio != null)
            {
                montagemCargaPatio.EtapaMontagemCargaLiberada = true;
                repositorioMontagemCargaPatio.Atualizar(montagemCargaPatio);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataMontagemCarga;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataMontagemCargaPrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio repositorioMontagemCargaPatio = new Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio = repositorioMontagemCargaPatio.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (montagemCargaPatio != null)
            {
                montagemCargaPatio.EtapaMontagemCargaLiberada = false;
                repositorioMontagemCargaPatio.Atualizar(montagemCargaPatio);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAguardandoMontagemCarga = 0;
            fluxoGestaoPatio.DataMontagemCarga = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataMontagemCargaPrevista.HasValue)
                fluxoGestaoPatio.DataMontagemCargaReprogramada = fluxoGestaoPatio.DataMontagemCargaPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
