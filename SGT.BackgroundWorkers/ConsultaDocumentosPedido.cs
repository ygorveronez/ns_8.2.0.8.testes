using SGT.BackgroundWorkers.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 21600000)]

    public class ConsultaDocumentosPedido : LongRunningProcessBase<ConsultaDocumentosPedido>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            ConsultarNotasFiscaisPedidoEmillenium(unitOfWork, _tipoServicoMultisoftware);
        }

        private void ConsultarNotasFiscaisPedidoEmillenium(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.BuscarPrimeiroRegistro();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaEmillenium) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioEmillenium))
                    return;


                var serIntegracaoEmillenium = new Servicos.Embarcador.Integracao.Emillenium.IntegracaoEmillenium(unitOfWork, _tipoServicoMultisoftware, _stringConexao);
                serIntegracaoEmillenium.BuscarNotasPedidosAgNFe();

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("ConsultaDocumentosPedido: \n" + ex, "Notas_Emillenium_Pedidos_Aguardando");
            }
        }

    }
}