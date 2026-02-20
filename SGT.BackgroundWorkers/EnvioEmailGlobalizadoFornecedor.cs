using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 900000)]

    public class EnvioEmailGlobalizadoFornecedor : LongRunningProcessBase<EnvioEmailGlobalizadoFornecedor>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarEmailsPendentesEnvio(unitOfWork);
        }

        private void VerificarEmailsPendentesEnvio(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Email.EmailGlobalizadoFornecedor repEmailGlobalizadoFornecedor = new Repositorio.Embarcador.Email.EmailGlobalizadoFornecedor(unitOfWork);

                Servicos.Embarcador.Email.EnvioEmailFornecedor ServEnvioEmailFornecedor = new Servicos.Embarcador.Email.EnvioEmailFornecedor(unitOfWork);

                List<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor> emailsFornecedor = repEmailGlobalizadoFornecedor.BuscarEmailsPendentesEnvio();
                if (emailsFornecedor.Count == 0)
                    return;

                foreach (Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor emailFornecedor in emailsFornecedor)
                    ServEnvioEmailFornecedor.EnviarEmailFornecedores(emailFornecedor);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }
    }
}