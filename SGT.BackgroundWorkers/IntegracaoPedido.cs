using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoPedido : LongRunningProcessBase<IntegracaoPedido>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Pedido.Pedido.ProcessarPedidoIntegracao(unitOfWork);
            Servicos.Embarcador.Ocorrencia.Ocorrencia.ProcessarImportarOcorrencia(unitOfWork, _stringConexao, _tipoServicoMultisoftware, _clienteMultisoftware);
            Servicos.Embarcador.Pedido.Pedido.ProcessarPedidoAguardandoRetornoIntegracao(unitOfWork);
            Servicos.Embarcador.Pedido.Pedido.ProcessarPedidoEmCancelamentoIntegracao(unitOfWork);
            new Servicos.Embarcador.Integracao.IntegracaoPedidoRoterizador(unitOfWork).VerificarIntegracaoesRoteirizadorPendentes();
        }
    }
}