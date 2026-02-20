using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoATSLog : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoATSLog>
    {
        #region Construtores

        public IntegracaoATSLog(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoATSLog(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
