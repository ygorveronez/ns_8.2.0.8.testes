using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class InicioDescarregamento : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga
    {
        #region Construtores

        public InicioDescarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public InicioDescarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.InicioDescarregamento, cliente) { }

        #endregion

        #region Métodos Privados

        private void AvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento inicioDescarregamento, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.InicioDescarregamentoAvancarEtapa inicioDescarregamentoAvancarEtapa)
        {
            if (inicioDescarregamento == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (!inicioDescarregamento.EtapaInicioDescarregamentoLiberada)
                throw new ServicoException("A liberação do início do descarregamento ainda não foi autorizada.");

            inicioDescarregamento.Initialize();
            inicioDescarregamento.DataDescarregamentoIniciado = DateTime.Now;
            inicioDescarregamento.Situacao = SituacaoInicioDescarregamento.DescarregamentoIniciado;

            if (inicioDescarregamentoAvancarEtapa != null)
                inicioDescarregamento.Pesagem = inicioDescarregamentoAvancarEtapa.Pesagem;

            LiberarProximaEtapa(fluxoGestaoPatio);

            new Repositorio.Embarcador.GestaoPatio.InicioDescarregamento(_unitOfWork).Atualizar(inicioDescarregamento, _auditado);
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            Repositorio.Embarcador.GestaoPatio.InicioDescarregamento repositorioInicioDescarregamento = new Repositorio.Embarcador.GestaoPatio.InicioDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento inicioDescarregamento = repositorioInicioDescarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (inicioDescarregamento != null)
                return;

            inicioDescarregamento = new Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento()
            {
                FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                EtapaInicioDescarregamentoLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada,
                Situacao = SituacaoInicioDescarregamento.AguardandoInicioDescarregamento
            };

            repositorioInicioDescarregamento.Inserir(inicioDescarregamento);
        }

        public void Avancar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.InicioDescarregamentoAvancarEtapa inicioDescarregamentoAvancarEtapa)
        {
            Repositorio.Embarcador.GestaoPatio.InicioDescarregamento repositorioInicioDescarregamento = new Repositorio.Embarcador.GestaoPatio.InicioDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento inicioDescarregamento = repositorioInicioDescarregamento.BuscarPorCodigo(inicioDescarregamentoAvancarEtapa.Codigo);

            AvancarEtapa(inicioDescarregamento?.FluxoGestaoPatio, inicioDescarregamento, inicioDescarregamentoAvancarEtapa);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.GestaoPatio.InicioDescarregamento repositorioInicioDescarregamento = new Repositorio.Embarcador.GestaoPatio.InicioDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento inicioDescarregamento = repositorioInicioDescarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (inicioDescarregamento != null)
            {
                inicioDescarregamento.Carga = carga;
                repositorioInicioDescarregamento.Atualizar(inicioDescarregamento);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.InicioDescarregamento repositorioInicioDescarregamento = new Repositorio.Embarcador.GestaoPatio.InicioDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento inicioDescarregamento = repositorioInicioDescarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (inicioDescarregamento != null)
            {
                inicioDescarregamento.Carga = cargaNova;
                repositorioInicioDescarregamento.Atualizar(inicioDescarregamento);
            }
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataInicioDescarregamentoPrevista.HasValue)
                fluxoGestaoPatio.DataInicioDescarregamentoPrevista = preSetTempoEtapa.DataInicioDescarregamentoPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.InicioDescarregamento repositorioInicioDescarregamento = new Repositorio.Embarcador.GestaoPatio.InicioDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento inicioDescarregamento = repositorioInicioDescarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            AvancarEtapa(fluxoGestaoPatio, inicioDescarregamento, inicioDescarregamentoAvancarEtapa: null);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataInicioDescarregamentoPrevista = preSetTempoEtapa.DataInicioDescarregamentoPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataInicioDescarregamento.HasValue)
                return false;

            fluxoGestaoPatio.TempoAguardandoInicioDescarregamento = tempoEtapaAnterior;
            fluxoGestaoPatio.DataInicioDescarregamento = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.InicioDescarregamento repositorioInicioDescarregamento = new Repositorio.Embarcador.GestaoPatio.InicioDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento inicioDescarregamento = repositorioInicioDescarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (inicioDescarregamento != null)
            {
                inicioDescarregamento.EtapaInicioDescarregamentoLiberada = true;
                repositorioInicioDescarregamento.Atualizar(inicioDescarregamento);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataInicioDescarregamento;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataInicioDescarregamentoPrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.InicioDescarregamento repositorioInicioDescarregamento = new Repositorio.Embarcador.GestaoPatio.InicioDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento inicioDescarregamento = repositorioInicioDescarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (inicioDescarregamento != null)
            {
                inicioDescarregamento.EtapaInicioDescarregamentoLiberada = false;
                repositorioInicioDescarregamento.Atualizar(inicioDescarregamento);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAguardandoInicioDescarregamento = 0;
            fluxoGestaoPatio.DataInicioDescarregamento = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataInicioDescarregamentoPrevista.HasValue)
                fluxoGestaoPatio.DataInicioDescarregamentoReprogramada = fluxoGestaoPatio.DataInicioDescarregamentoPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
