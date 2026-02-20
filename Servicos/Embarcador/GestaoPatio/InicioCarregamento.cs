using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class InicioCarregamento : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga
    {
        #region Construtores

        public InicioCarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public InicioCarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.InicioCarregamento, cliente) { }

        #endregion

        #region Métodos Privados

        private void AvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento inicioCarregamento, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.InicioCarregamentoAvancarEtapa inicioCarregamentoAvancarEtapa)
        {
            if (inicioCarregamento == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (!inicioCarregamento.EtapaInicioCarregamentoLiberada)
                throw new ServicoException("A liberação do início do carregamento ainda não foi autorizada.");

            inicioCarregamento.Initialize();
            inicioCarregamento.DataCarregamentoIniciado = DateTime.Now;
            inicioCarregamento.Situacao = SituacaoInicioCarregamento.CarregamentoIniciado;

            if (inicioCarregamentoAvancarEtapa != null)
                inicioCarregamento.Pesagem = inicioCarregamentoAvancarEtapa.Pesagem;

            LiberarProximaEtapa(fluxoGestaoPatio);

            new Repositorio.Embarcador.GestaoPatio.InicioCarregamento(_unitOfWork).Atualizar(inicioCarregamento, _auditado);
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            Repositorio.Embarcador.GestaoPatio.InicioCarregamento repositorioInicioCarregamento = new Repositorio.Embarcador.GestaoPatio.InicioCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento inicioCarregamento = repositorioInicioCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (inicioCarregamento != null)
                return;
            
            inicioCarregamento = new Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento()
            {
                FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                EtapaInicioCarregamentoLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada,
                Situacao = SituacaoInicioCarregamento.AguardandoInicioCarregamento
            };

            repositorioInicioCarregamento.Inserir(inicioCarregamento);
        }

        public void Avancar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.InicioCarregamentoAvancarEtapa inicioCarregamentoAvancarEtapa)
        {
            Repositorio.Embarcador.GestaoPatio.InicioCarregamento repositorioInicioCarregamento = new Repositorio.Embarcador.GestaoPatio.InicioCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento inicioCarregamento = repositorioInicioCarregamento.BuscarPorCodigo(inicioCarregamentoAvancarEtapa.Codigo);

            AvancarEtapa(inicioCarregamento?.FluxoGestaoPatio, inicioCarregamento, inicioCarregamentoAvancarEtapa);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.GestaoPatio.InicioCarregamento repositorioInicioCarregamento = new Repositorio.Embarcador.GestaoPatio.InicioCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento inicioCarregamento = repositorioInicioCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (inicioCarregamento != null)
            {
                inicioCarregamento.Carga = carga;
                repositorioInicioCarregamento.Atualizar(inicioCarregamento);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.InicioCarregamento repositorioInicioCarregamento = new Repositorio.Embarcador.GestaoPatio.InicioCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento inicioCarregamento = repositorioInicioCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (inicioCarregamento != null)
            {
                inicioCarregamento.Carga = cargaNova;
                repositorioInicioCarregamento.Atualizar(inicioCarregamento);
            }
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataInicioCarregamentoPrevista.HasValue)
                fluxoGestaoPatio.DataInicioCarregamentoPrevista = preSetTempoEtapa.DataInicioCarregamentoPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.InicioCarregamento repositorioInicioCarregamento = new Repositorio.Embarcador.GestaoPatio.InicioCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento inicioCarregamento = repositorioInicioCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            AvancarEtapa(fluxoGestaoPatio, inicioCarregamento, inicioCarregamentoAvancarEtapa: null);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataInicioCarregamentoPrevista = preSetTempoEtapa.DataInicioCarregamentoPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataInicioCarregamento.HasValue)
                return false;

            fluxoGestaoPatio.TempoAgInicioCarregamento = tempoEtapaAnterior;
            fluxoGestaoPatio.DataInicioCarregamento = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.InicioCarregamento repositorioInicioCarregamento = new Repositorio.Embarcador.GestaoPatio.InicioCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento inicioCarregamento = repositorioInicioCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (inicioCarregamento != null)
            {
                inicioCarregamento.EtapaInicioCarregamentoLiberada = true;
                repositorioInicioCarregamento.Atualizar(inicioCarregamento);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataInicioCarregamento;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataInicioCarregamentoPrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.InicioCarregamento repositorioInicioCarregamento = new Repositorio.Embarcador.GestaoPatio.InicioCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento inicioCarregamento = repositorioInicioCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (inicioCarregamento != null)
            {
                inicioCarregamento.EtapaInicioCarregamentoLiberada = false;
                repositorioInicioCarregamento.Atualizar(inicioCarregamento);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAgInicioCarregamento = 0;
            fluxoGestaoPatio.DataInicioCarregamento = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataInicioCarregamentoPrevista.HasValue)
                fluxoGestaoPatio.DataInicioCarregamentoReprogramada = fluxoGestaoPatio.DataInicioCarregamentoPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
