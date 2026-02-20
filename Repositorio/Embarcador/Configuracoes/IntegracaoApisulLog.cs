using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoApisulLog : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoApisulLog>
    {
        #region Construtores

        public IntegracaoApisulLog(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoApisulLog(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
