using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 5000)]

    public class ControleCargaEmFinalizacaoEmissaoIntegracao: LongRunningProcessBase<ControleCargaEmFinalizacaoEmissaoIntegracao>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            SolicitarFinalizacaoCargas(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
        }

        private void SolicitarFinalizacaoCargas(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Carga.Documentos svcDocumentos = new Servicos.Embarcador.Carga.Documentos(unitOfWork);

                List<int> codigosCargas = repCarga.BuscarCodigosCargasEmFinalizacao(100, true);

                foreach (int codigoCarga in codigosCargas)
                    svcDocumentos.FinalizarCargaEmFinalizacao(codigoCarga, tipoServicoMultisoftware, unitOfWork, _auditado);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }
    }
}