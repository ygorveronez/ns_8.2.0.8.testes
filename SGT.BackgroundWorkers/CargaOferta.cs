using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]

    public class CargaOferta : LongRunningProcessBase<CargaOferta>
    {

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizyOfertas servIntegracaoTrizyOfertas = new Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizyOfertas(unitOfWork);
            await servIntegracaoTrizyOfertas.VerificarIntegracoesAguardando(_tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.CargaOferta, cancellationToken);

            Servicos.Embarcador.Cargas.CargaOferta servicoCargaOferta = new Servicos.Embarcador.Cargas.CargaOferta(unitOfWork);
            await servicoCargaOferta.VerificarCargaOfertaExpiradas(cancellationToken);
        }
    }
}