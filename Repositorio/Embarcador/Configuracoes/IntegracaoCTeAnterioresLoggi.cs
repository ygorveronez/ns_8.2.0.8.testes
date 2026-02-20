using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoCTeAnterioresLoggi : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi>
    {
        #region Construtores

        public IntegracaoCTeAnterioresLoggi(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoCTeAnterioresLoggi(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}

