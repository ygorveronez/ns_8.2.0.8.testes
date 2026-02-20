using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoCassol : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCassol>
    {
        #region Construtores

        public IntegracaoCassol(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoCassol(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}