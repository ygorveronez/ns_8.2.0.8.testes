using SGT.BackgroundWorkers.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]

    public class ProcessarIntegracaoNotificacaoApp : LongRunningProcessBase<ProcessarIntegracaoNotificacaoApp>
    {

        #region Atributos

        public Repositorio.UnitOfWork _unitOfWork;
        public AdminMultisoftware.Repositorio.UnitOfWork _unitOfWorkAdmin;

        #endregion Atributos

        #region Metodos Publicos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkAdmin = unitOfWorkAdmin;

            Processar();
        }

        #endregion

        #region Metodos Privados
        private void Processar()
        {
            try
            {
                Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp servicoIntegracaoNotificacaoApp = new Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp(_unitOfWork);
                servicoIntegracaoNotificacaoApp.Iniciar();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        #endregion
    }
}