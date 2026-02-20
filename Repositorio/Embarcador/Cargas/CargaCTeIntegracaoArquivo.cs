using System.Threading;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaCTeIntegracaoArquivo : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>
    {
        #region Construtores

        public CargaCTeIntegracaoArquivo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CargaCTeIntegracaoArquivo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
