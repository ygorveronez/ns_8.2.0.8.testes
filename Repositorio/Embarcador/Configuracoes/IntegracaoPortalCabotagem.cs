using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoPortalCabotagem : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPortalCabotagem>
    {
        #region Construtores

        public IntegracaoPortalCabotagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoPortalCabotagem(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
