using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]

    public class PedidoOcorrenciaColetaEntregaIntegracao : LongRunningProcessBase<PedidoOcorrenciaColetaEntregaIntegracao>
    {
        #region Métodos protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await repConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();

            Servicos.Embarcador.Carga.ControleEntrega.PedidoOcorrenciaColetaEntregaIntegracao servicoPedidoOcorrenciaColetaEntregaIntegracao = new Servicos.Embarcador.Carga.ControleEntrega.PedidoOcorrenciaColetaEntregaIntegracao(unitOfWork, _urlAcesso);
            servicoPedidoOcorrenciaColetaEntregaIntegracao.ProcessarIntegracoesPendentes(configuracaoTMS, _tipoServicoMultisoftware, unitOfWorkAdmin, _clienteMultisoftware, _codigoEmpresa);
        }

        #endregion
    }
}