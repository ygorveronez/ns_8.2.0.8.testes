using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoArquivosDocumentosDestinados : LongRunningProcessBase<IntegracaoArquivosDocumentosDestinados>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.BuscarIntegracaoDocumentosDestinados(unitOfWork.StringConexao, _tipoServicoMultisoftware);                
            }
        }
    }
}