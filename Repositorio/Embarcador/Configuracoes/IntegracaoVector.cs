using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoVector : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVector>
    {
        #region Construtores

        public IntegracaoVector(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoVector(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
