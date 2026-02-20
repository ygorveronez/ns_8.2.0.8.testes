using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoGhost : LongRunningProcessBase<IntegracaoGhost>
    {

        #region Atributos

        public Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Metodos Publicos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            _unitOfWork = unitOfWork;

            ProcessarIntegracoesPendentes();
        }

        #endregion

        #region Metodos Privados

        private void ProcessarIntegracoesPendentes()
        {
            Repositorio.Embarcador.Integracao.IntegracaoGhost repIntegracaoGhost = new Repositorio.Embarcador.Integracao.IntegracaoGhost(_unitOfWork);
            Servicos.Embarcador.Integracao.Ghost.IntegracaoGhost svcGhost = new Servicos.Embarcador.Integracao.Ghost.IntegracaoGhost(_unitOfWork);
            try
            {
                List<Dominio.Entidades.Embarcador.Integracao.IntegracaoGhost> integracoesPendentes = repIntegracaoGhost.BuscarIntegracoesPendentes();

                foreach (var integracao in integracoesPendentes)
                    svcGhost.ProcessarIntegracao(integracao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoGhost");
            }
        }

        #endregion
    }
}