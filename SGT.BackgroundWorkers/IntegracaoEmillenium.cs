using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoEmillenium : LongRunningProcessBase<IntegracaoEmillenium>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.BuscarPrimeiroRegistro();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaEmillenium) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioEmillenium))
                return;

            // Integração VTEX
            var serIntegracaoVtex = new Servicos.Embarcador.Integracao.VTEX.IntegracaoVtexBuscaPedidos(unitOfWork, _tipoServicoMultisoftware, _auditado, unitOfWorkAdmin.StringConexao);
            serIntegracaoVtex.IntegrarPedidos();

            // Integração E-Millenium
            var serIntegracaoEmillenium = new Servicos.Embarcador.Integracao.Emillenium.IntegracaoEmillenium(unitOfWork, _tipoServicoMultisoftware, _stringConexao);
            serIntegracaoEmillenium.NotificarFilaIntegracao();

            serIntegracaoEmillenium.SalvarListaPedidos();
            serIntegracaoEmillenium.IntegrarPedidos();
            serIntegracaoEmillenium.GerarCargasPedidos();

            //Alterado para ser na mesma thread pois em thred separada dos pedidos a Nota tenta integrar antes do pedido estar na base
            serIntegracaoEmillenium.BuscarNotasPedidos();
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