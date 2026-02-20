using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoCargaFrete : LongRunningProcessBase<IntegracaoCargaFrete>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarIntegracoesPendentes(unitOfWork, cancellationToken, _tipoServicoMultisoftware);
        }

        private void VerificarIntegracoesPendentes(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Integracao.IntegracaoCarga serIntegracaoCarga = new Servicos.Embarcador.Integracao.IntegracaoCarga(unitOfWork, _clienteMultisoftware, cancellationToken);
            serIntegracaoCarga.VerificarIntegracoesFreteIntegracaoPendente(unitOfWork, _auditado, tipoServicoMultisoftware, _webServiceConsultaCTe, _clienteUrlAcesso);

        }
     
    }
}