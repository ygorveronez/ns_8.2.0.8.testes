using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoPager : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPager>
    {
        #region Construtores

        public IntegracaoPager(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoPager(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}

