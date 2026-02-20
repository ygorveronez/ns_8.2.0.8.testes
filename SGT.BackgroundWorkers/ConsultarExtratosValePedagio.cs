using SGT.BackgroundWorkers.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 85800000)]

    public class ConsultarExtratosValePedagio : LongRunningProcessBase<ConsultarExtratosValePedagio>
	{
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            ConsultarExtratosValePedagioSemParar(unitOfWork, _tipoServicoMultisoftware);
			ProcessarExtratosCreditoValePedagioPendentes(unitOfWork, _tipoServicoMultisoftware);
        }

        private void ConsultarExtratosValePedagioSemParar(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
		{
			try
			{
				Servicos.Embarcador.Integracao.SemParar.ValePedagio servicoValePedagioSemParar = new Servicos.Embarcador.Integracao.SemParar.ValePedagio();
				servicoValePedagioSemParar.ConsultarExtratoValePedagio(unitOfWork, _tipoServicoMultisoftware);
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro("ConsultarExtratoValePedagio: \n" + ex, "ConsultaExtratoValePedagio");
			}
		}

		private void ProcessarExtratosCreditoValePedagioPendentes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
		{
			try
			{
				Servicos.Embarcador.Integracao.SemParar.ValePedagio servicoValePedagioSemParar = new Servicos.Embarcador.Integracao.SemParar.ValePedagio();
				servicoValePedagioSemParar.ProcessarExtratosCreditoValePedagio(unitOfWork, tipoServicoMultisoftware);
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro("ProcessarExtratosCreditoValePedagio: \n" + ex, "ConsultaExtratoValePedagio");
			}
		}
	}
}