using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoSalesforce : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSalesforce>
    {
        #region Construtores

        public IntegracaoSalesforce(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoSalesforce(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos
        #endregion
    }
}

