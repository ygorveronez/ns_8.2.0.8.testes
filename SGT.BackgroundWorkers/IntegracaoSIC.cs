using AdminMultisoftware.Repositorio;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoSIC : LongRunningProcessBase<IntegracaoSIC>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarCadastrosSIC(unitOfWork);
        }

        private void VerificarCadastrosSIC(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSIC repositorioIntegracaoSIC = new Repositorio.Embarcador.Configuracoes.IntegracaoSIC(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracaoSIC = repositorioIntegracaoSIC.Buscar();

            if (configuracaoIntegracaoSIC?.PossuiIntegracaoSIC ?? false)
            {
                new Servicos.Embarcador.Integracao.SIC.IntegracaoSIC(unitOfWork).ConsultarVeiculos(configuracaoIntegracaoSIC);
                new Servicos.Embarcador.Integracao.SIC.IntegracaoSIC(unitOfWork).ConsultarMotoristas(configuracaoIntegracaoSIC);
                new Servicos.Embarcador.Integracao.SIC.IntegracaoSIC(unitOfWork).ConsultarClientes(configuracaoIntegracaoSIC);
                new Servicos.Embarcador.Integracao.SIC.IntegracaoSIC(unitOfWork).ConsultarClientesTerceiro(configuracaoIntegracaoSIC);
                new Servicos.Embarcador.Integracao.SIC.IntegracaoSIC(unitOfWork).ConsultarTransportadorTerceiro(configuracaoIntegracaoSIC);
            }
        }
    }
}