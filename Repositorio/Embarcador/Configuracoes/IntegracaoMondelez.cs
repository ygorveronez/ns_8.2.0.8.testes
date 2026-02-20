using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoMondelez : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMondelez>
    {
        #region Construtores

        public IntegracaoMondelez(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoMondelez(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos
        #endregion
    }
}

