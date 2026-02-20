using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoBind : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBind>
    {
        #region Construtores

        public IntegracaoBind(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoBind(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
