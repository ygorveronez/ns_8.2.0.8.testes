using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoEmilleniumBuscaMassiva : LongRunningProcessBase<IntegracaoEmilleniumBuscaMassiva>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoEmilenium repIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEmilenium(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmilenium configuracaoIntegracao = repIntegracao.BuscarConfiguracaoPadrao();

            // Integração E-Millenium
            var serIntegracaoEmillenium = new Servicos.Embarcador.Integracao.Emillenium.IntegracaoEmillenium(unitOfWork, _tipoServicoMultisoftware, _stringConexao);

            if (configuracaoIntegracao != null && !configuracaoIntegracao.DataFinalizacaoBuscaMassiva.HasValue)
                serIntegracaoEmillenium.BuscaNotasMassiva();
        }

        public override bool CanRun()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.BuscarPrimeiroRegistro();

                if (configuracaoIntegracao == null || string.IsNullOrEmpty(configuracaoIntegracao.URLEmillenium) || string.IsNullOrEmpty(configuracaoIntegracao.UsuarioEmillenium) || string.IsNullOrEmpty(configuracaoIntegracao.SenhaEmillenium))
                    return false;
                else
                    return true;
            }
        }
    }
}