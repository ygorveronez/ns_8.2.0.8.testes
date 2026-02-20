using SGT.BackgroundWorkers.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 3600000)]
    public class Abastecimento : LongRunningProcessBase<Abastecimento>
    {

        public override bool CanRun()
        {
            var atual = DateTime.Now;
            var inicio = new TimeSpan(0, 0, 0);    // 00h00
            var fim = new TimeSpan(1, 1, 0);       // 01h01
            var isHoraExecucao = (atual.TimeOfDay >= inicio && atual.TimeOfDay <= fim); // Executa entre 12h00 e 01h00

            // O serviço rodará de hora em hora, por isso o intervalo de 1 hora para execução
            if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || !isHoraExecucao)
                return false;

            return true;
        }

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Abastecimento.Abastecimento.CancelarAbastecimentosPorPeriodo(unitOfWork);
        }

    }
}