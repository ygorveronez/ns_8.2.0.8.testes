using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoVedacit : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVedacit>
    {
        #region Construtores

        public IntegracaoVedacit(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoVedacit(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
