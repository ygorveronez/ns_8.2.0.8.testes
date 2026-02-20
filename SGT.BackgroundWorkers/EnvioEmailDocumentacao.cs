using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 60000)]

    public class EnvioEmailDocumentacao : LongRunningProcessBase<EnvioEmailDocumentacao>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarCargasPendentesEnvioEmailDocumentacao(unitOfWork);
        }

        private void VerificarCargasPendentesEnvioEmailDocumentacao(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Servicos.Embarcador.Email.EnvioEmailDocumentacao servicoEnvioEmailDocumentacao = new Servicos.Embarcador.Email.EnvioEmailDocumentacao(unitOfWork, _tipoServicoMultisoftware, _auditado, _clienteMultisoftware);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarCargasPendentesEnvioEmailDocumentacao();
                if (cargas.Count == 0)
                    return;

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                    servicoEnvioEmailDocumentacao.EnviarEmailDocumentacao(carga);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }
    }
}