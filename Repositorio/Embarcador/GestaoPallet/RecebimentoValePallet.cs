using System.Threading;

namespace Repositorio.Embarcador.GestaoPallet
{
    public class RecebimentoValePallet : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPallet.RecebimentoValePallet>
    {

        #region Construtores

        public RecebimentoValePallet(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public RecebimentoValePallet(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        #endregion Métodos Públicos
    }
}
