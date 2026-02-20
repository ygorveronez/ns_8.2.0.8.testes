using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 600000)]

    public class Canhoto : LongRunningProcessBase<Canhoto>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            ProcessarCanhotosAguardandoLeituraNumeroDocumento(unitOfWork);
        }

        private void ProcessarCanhotosAguardandoLeituraNumeroDocumento(Repositorio.UnitOfWork unitOfWork)
        {
            string apiLink = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().APIOCRLink;
            string apiKey = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().APIOCRKey;

            if (string.IsNullOrWhiteSpace(apiLink) || string.IsNullOrWhiteSpace(apiKey))
                return;

            Servicos.Embarcador.Canhotos.CanhotoEsperandoVinculo servicoCanhotoEsperandoVinculo = new Servicos.Embarcador.Canhotos.CanhotoEsperandoVinculo(unitOfWork);

            servicoCanhotoEsperandoVinculo.ProcessarAguardandoLeituraNumeroDocumento(apiLink, apiKey, _tipoServicoMultisoftware, _clienteMultisoftware);
        }
    }
}
