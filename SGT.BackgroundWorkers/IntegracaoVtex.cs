using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoVtex : LongRunningProcessBase<IntegracaoVtex>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            var serIntegracaoVtex = new Servicos.Embarcador.Integracao.VTEX.IntegracaoVtexBuscaPedidos(unitOfWork, _tipoServicoMultisoftware, _auditado, unitOfWorkAdmin.StringConexao);
            serIntegracaoVtex.NotificarFilaIntegracao();
            serIntegracaoVtex.IntegrarListaPedidosPorFeed();
        }

        public override bool CanRun()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex repConfiguracaoVtex = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex(unitOfWork);
                var configuracoes = repConfiguracaoVtex.BuscarTodos();

                return configuracoes.Count > 0;
            }
        }
    }
}
