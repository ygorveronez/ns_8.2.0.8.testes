using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoFS : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFS>
    {
        #region Construtores

        public IntegracaoFS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoFS(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
