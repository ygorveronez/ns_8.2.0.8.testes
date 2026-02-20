using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class FimDescarregamento : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga
    {
        #region Construtores

        public FimDescarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public FimDescarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.FimDescarregamento, cliente) { }

        #endregion

        #region Métodos Privados

        private void AvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento fimDescarregamento, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FimDescarregamentoAvancarEtapa fimDescarregamentoAvancarEtapa)
        {
            if (fimDescarregamento == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (!fimDescarregamento.EtapaFimDescarregamentoLiberada)
                throw new ServicoException("A liberação do fim do descarregamento ainda não foi autorizada.");

            fimDescarregamento.Initialize();
            fimDescarregamento.DataDescarregamentoFinalizado = DateTime.Now;
            fimDescarregamento.Situacao = SituacaoFimDescarregamento.DescarregamentoFinalizado;

            if (fimDescarregamentoAvancarEtapa != null)
                fimDescarregamento.Pesagem = fimDescarregamentoAvancarEtapa.Pesagem;

            LiberarProximaEtapa(fluxoGestaoPatio);

            new Repositorio.Embarcador.GestaoPatio.FimDescarregamento(_unitOfWork).Atualizar(fimDescarregamento, _auditado);
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            Repositorio.Embarcador.GestaoPatio.FimDescarregamento repositorioFimDescarregamento = new Repositorio.Embarcador.GestaoPatio.FimDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento fimDescarregamento = repositorioFimDescarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (fimDescarregamento != null)
                return;

            fimDescarregamento = new Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento()
            {
                FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                EtapaFimDescarregamentoLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada,
                Situacao = SituacaoFimDescarregamento.AguardandoFimDescarregamento
            };

            repositorioFimDescarregamento.Inserir(fimDescarregamento);
        }

        public void Avancar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FimDescarregamentoAvancarEtapa fimDescarregamentoAvancarEtapa)
        {
            Repositorio.Embarcador.GestaoPatio.FimDescarregamento repositorioFimDescarregamento = new Repositorio.Embarcador.GestaoPatio.FimDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento fimDescarregamento = repositorioFimDescarregamento.BuscarPorCodigo(fimDescarregamentoAvancarEtapa.Codigo);

            AvancarEtapa(fimDescarregamento?.FluxoGestaoPatio, fimDescarregamento, fimDescarregamentoAvancarEtapa);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.GestaoPatio.FimDescarregamento repositorioFimDescarregamento = new Repositorio.Embarcador.GestaoPatio.FimDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento fimDescarregamento = repositorioFimDescarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (fimDescarregamento != null)
            {
                fimDescarregamento.Carga = carga;
                repositorioFimDescarregamento.Atualizar(fimDescarregamento);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.FimDescarregamento repositorioFimDescarregamento = new Repositorio.Embarcador.GestaoPatio.FimDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento fimDescarregamento = repositorioFimDescarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (fimDescarregamento != null)
            {
                fimDescarregamento.Carga = cargaNova;
                repositorioFimDescarregamento.Atualizar(fimDescarregamento);
            }
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataFimDescarregamentoPrevista.HasValue)
                fluxoGestaoPatio.DataFimDescarregamentoPrevista = preSetTempoEtapa.DataFimDescarregamentoPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.FimDescarregamento repositorioFimDescarregamento = new Repositorio.Embarcador.GestaoPatio.FimDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento fimDescarregamento = repositorioFimDescarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            AvancarEtapa(fluxoGestaoPatio, fimDescarregamento, fimDescarregamentoAvancarEtapa: null);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataFimDescarregamentoPrevista = preSetTempoEtapa.DataFimDescarregamentoPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataFimDescarregamento.HasValue)
                return false;

            fluxoGestaoPatio.TempoAguardandoFimDescarregamento = tempoEtapaAnterior;
            fluxoGestaoPatio.DataFimDescarregamento = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.FimDescarregamento repositorioFimDescarregamento = new Repositorio.Embarcador.GestaoPatio.FimDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento fimDescarregamento = repositorioFimDescarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (fimDescarregamento != null)
            {
                fimDescarregamento.EtapaFimDescarregamentoLiberada = true;
                repositorioFimDescarregamento.Atualizar(fimDescarregamento);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataFimDescarregamento;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataFimDescarregamentoPrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.FimDescarregamento repositorioFimDescarregamento = new Repositorio.Embarcador.GestaoPatio.FimDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento fimDescarregamento = repositorioFimDescarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (fimDescarregamento != null)
            {
                fimDescarregamento.EtapaFimDescarregamentoLiberada = false;
                repositorioFimDescarregamento.Atualizar(fimDescarregamento);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAguardandoFimDescarregamento = 0;
            fluxoGestaoPatio.DataFimDescarregamento = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataFimDescarregamentoPrevista.HasValue)
                fluxoGestaoPatio.DataFimDescarregamentoReprogramada = fluxoGestaoPatio.DataFimDescarregamentoPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
