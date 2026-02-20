using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoDocumentosDestinados : LongRunningProcessBase<IntegracaoDocumentosDestinados>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.BuscarCTEsDestinados(unitOfWork.StringConexao, _tipoServicoMultisoftware);
                Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.BuscarDocumentosDestinados(unitOfWork.StringConexao);
            }
        }
    }
}