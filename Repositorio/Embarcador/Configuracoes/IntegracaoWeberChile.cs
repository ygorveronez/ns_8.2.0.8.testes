using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoWeberChile : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoWeberChile>
    {
        #region Construtores

        public IntegracaoWeberChile(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoWeberChile(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
