using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoYMS : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYMS>
    {
        #region Construtores

        public IntegracaoYMS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoYMS(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos   

        #endregion
    }
}
