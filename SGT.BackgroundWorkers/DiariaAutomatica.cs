using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 60000)]

    public class DiariaAutomatica : LongRunningProcessBase<DiariaAutomatica>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Monitoramento.DiariaAutomatica srvDiariaAutomatica = new Servicos.Embarcador.Monitoramento.DiariaAutomatica(unitOfWork);
            srvDiariaAutomatica.CriarDiariasAutomaticasSeNecessario(_tipoServicoMultisoftware);
            srvDiariaAutomatica.AtualizarEstadoDiariasAutomaticas(_tipoServicoMultisoftware);
        }

        public override bool CanRun()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoDiariaAutomatica repConfiguracaoDiariaAutomatica = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDiariaAutomatica(unitOfWork);
                var configuracaoDiariaAutomatica = repConfiguracaoDiariaAutomatica.BuscarConfiguracaoPadrao();
                return configuracaoDiariaAutomatica.HabilitarDiariaAutomatica;
            }
        }
    }
}
