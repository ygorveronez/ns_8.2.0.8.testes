using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class GeracaoPDFDANFE: LongRunningProcessBase<GeracaoPDFDANFE>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Documentos.EnvioDocumentacaoLote.ProcessarNotasSemPDF(unitOfWork);
        }
    }
}