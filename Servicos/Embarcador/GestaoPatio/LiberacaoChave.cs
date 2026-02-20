using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class LiberacaoChave :  FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga, IFluxoGestaoPatioEtapaRetornada
    {
        #region Construtores

        public LiberacaoChave(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public LiberacaoChave(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.LiberacaoChave, cliente) { }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (travamentoChave != null)
                return;
            
            travamentoChave = new Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave()
            {
                FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                Situacao = SituacaoTravamentoChave.Travada,
                EtapaLiberacaoChaveLiberada = false,
                DataTravamento = DateTime.Now,
                DataLiberacao = DateTime.Now
            };

            repositorioTravamentoChave.Inserir(travamentoChave);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (travamentoChave != null)
            {
                travamentoChave.Carga = carga;
                repositorioTravamentoChave.Atualizar(travamentoChave);
            }
        }

        public void EtapaRetornada(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (travamentoChave != null)
            {
                travamentoChave.Situacao = SituacaoTravamentoChave.Travada;
                repositorioTravamentoChave.Atualizar(travamentoChave);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (travamentoChave != null)
            {
                travamentoChave.Carga = cargaNova;
                repositorioTravamentoChave.Atualizar(travamentoChave);
            }
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataLiberacaoChavePrevista.HasValue)
                fluxoGestaoPatio.DataLiberacaoChavePrevista = preSetTempoEtapa.DataLiberacaoChavePrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (travamentoChave == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (!travamentoChave.EtapaLiberacaoChaveLiberada)
                throw new ServicoException("A liberação da chave ainda não foi autorizada.");

            travamentoChave.Initialize();
            travamentoChave.DataLiberacao = DateTime.Now;
            travamentoChave.Situacao = SituacaoTravamentoChave.Liberada;

            if (_auditado != null)
                Auditoria.Auditoria.Auditar(_auditado, travamentoChave, null, "Liberou a Chave", _unitOfWork);

            LiberarProximaEtapa(fluxoGestaoPatio);
            repositorioTravamentoChave.Atualizar(travamentoChave, _auditado);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataLiberacaoChavePrevista = preSetTempoEtapa.DataLiberacaoChavePrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataLiberacaoChave.HasValue)
                return false;

            fluxoGestaoPatio.TempoAgLiberacaoChave = tempoEtapaAnterior;
            fluxoGestaoPatio.DataLiberacaoChave = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (travamentoChave != null)
            {
                travamentoChave.EtapaLiberacaoChaveLiberada = true;
                travamentoChave.Situacao = SituacaoTravamentoChave.Travada;
                repositorioTravamentoChave.Atualizar(travamentoChave);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataLiberacaoChave;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataLiberacaoChavePrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (travamentoChave != null)
            {
                travamentoChave.EtapaLiberacaoChaveLiberada = false;
                travamentoChave.Situacao = SituacaoTravamentoChave.Travada;
                repositorioTravamentoChave.Atualizar(travamentoChave);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAgLiberacaoChave = 0;
            fluxoGestaoPatio.DataLiberacaoChave = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataLiberacaoChavePrevista.HasValue)
                fluxoGestaoPatio.DataLiberacaoChaveReprogramada = fluxoGestaoPatio.DataLiberacaoChavePrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
