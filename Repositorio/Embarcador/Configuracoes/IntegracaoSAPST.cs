using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoSAPST : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAPST>
    {
        #region Construtores

        public IntegracaoSAPST(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoSAPST(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
