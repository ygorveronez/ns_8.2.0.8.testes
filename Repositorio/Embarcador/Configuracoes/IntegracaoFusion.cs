using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoFusion : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFusion>
    {
        #region Construtores

        public IntegracaoFusion(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoFusion(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
