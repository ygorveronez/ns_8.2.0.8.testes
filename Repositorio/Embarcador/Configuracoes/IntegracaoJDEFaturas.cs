using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoJDEFaturas : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoJDEFaturas>
    {
        #region Construtores

        public IntegracaoJDEFaturas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoJDEFaturas(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
