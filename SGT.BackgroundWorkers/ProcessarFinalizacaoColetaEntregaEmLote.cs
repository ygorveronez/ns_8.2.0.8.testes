using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]
    public class ProcessarFinalizacaoColetaEntregaEmLote : LongRunningProcessBase<ProcessarFinalizacaoColetaEntregaEmLote>
    {

        #region Atributos

        public Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Metodos Publicos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            _unitOfWork = unitOfWork;

            Processar();
        }

        #endregion

        #region Metodos Privados
        private void Processar()
        {
            try
            {
                _unitOfWork.Start();

                Repositorio.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote repProcessamentoFinalizacaoColetaEntregaEmLote = new Repositorio.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote(_unitOfWork);

                List<int> codigosProcessamentoFinalizacaoColetaEntregaEmLote = repProcessamentoFinalizacaoColetaEntregaEmLote.BuscarCodigosCargasAgurdandoProcessamento();
                
                if (codigosProcessamentoFinalizacaoColetaEntregaEmLote.Count > 0)
                {
                    Servicos.Embarcador.Carga.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote servicoProcessamentoFinalizacaoColetaEntregaEmLote = new Servicos.Embarcador.Carga.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote();
                    servicoProcessamentoFinalizacaoColetaEntregaEmLote.FinalizarColetaEntregaEmMassa(codigosProcessamentoFinalizacaoColetaEntregaEmLote, _tipoServicoMultisoftware, _auditado, _codigoEmpresa, _clienteMultisoftware, _unitOfWork);
                }

                _unitOfWork.FlushAndClear();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        #endregion
    }
}