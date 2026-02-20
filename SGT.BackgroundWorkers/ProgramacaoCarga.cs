using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 30000)]
    public class ProgramacaoCarga : LongRunningProcessBase<ProgramacaoCarga>
    {
        #region Métodos Privados

        private void VerificarPreCargasDisponibilizarParaTransportadoresPorDataLiberacao(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                new Servicos.Embarcador.PreCarga.PreCargaOfertaTransportador(unitOfWork).DisponibilizarParaTransportadoresPorDataLiberacao();
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void VerificarPreCargasVincularFilaCarregamento(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.UnitOfWorkContainer unitOfWorkContainer = new Repositorio.UnitOfWorkContainer(unitOfWork);
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Sistema);
                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWorkContainer.UnitOfWork);
                List<int> codigosPreCargas = repositorioPreCarga.ConsultarCodigosParaVincularFilaCarregamento(limiteRegistros: 20);

                foreach (int codigoPreCarga in codigosPreCargas)
                {
                    try
                    {
                        unitOfWorkContainer.UnitOfWork.FlushAndClear();
                        unitOfWorkContainer.UnitOfWork.FlushAndClear();
                        unitOfWorkContainer.StartContainer();

                        Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repositorioPreCarga.BuscarPorCodigo(codigoPreCarga, true);
                        Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.AlocarParaPrimeiroDaFila(preCarga, tipoServicoMultisoftware);

                        unitOfWorkContainer.CommitChangesContainer();

                        servicoFilaCarregamentoVeiculo.NotificarAlteracao(filaCarregamentoVeiculo);
                    }
                    catch
                    {
                        unitOfWorkContainer.RollbackContainer();
                    }
                    finally
                    {
                        repositorioPreCarga.AtualizarTentativasVincularFilaCarregamento(codigoPreCarga);
                    }
                }

                if (codigosPreCargas.Count <= 0)
                    repositorioPreCarga.ReiniciarTentativasVincularFilaCarregamentoComLimiteAtingido();
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void VerificarTransportadoresTempoAceiteEncerrado(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                new Servicos.Embarcador.PreCarga.PreCargaOfertaTransportador(unitOfWork).RejeitarOfertaTransportadorPorTempoAceiteEncerrado();
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        #endregion Métodos Privados

        #region Métodos Sobrescritos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarPreCargasDisponibilizarParaTransportadoresPorDataLiberacao(unitOfWork);
            VerificarTransportadoresTempoAceiteEncerrado(unitOfWork);
            VerificarPreCargasVincularFilaCarregamento(_tipoServicoMultisoftware, unitOfWork);
        }

        public override bool CanRun()
        {
            return _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
        }

        #endregion Métodos Sobrescritos
    }
}