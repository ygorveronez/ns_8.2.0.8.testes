using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]

    public class FaturaFechamento : LongRunningProcessBase<FaturaFechamento>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

            new Servicos.Embarcador.Fatura.Fatura(unitOfWork).ProcessarFaturasEmFechamento(unitOfWork, _stringConexao, _tipoServicoMultisoftware, _auditado);
            servFatura.ProcessarFaturasEmCancelamento(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
            new Servicos.Embarcador.Fechamento.FechamentoFrete(unitOfWork).IntegrarComplementosFechamento();
            new Servicos.Embarcador.Frete.FechamentoPontuacao(unitOfWork).Finalizar();
        }
    }
}