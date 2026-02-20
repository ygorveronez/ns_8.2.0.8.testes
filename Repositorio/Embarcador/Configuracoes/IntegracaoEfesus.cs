using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoEfesus : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEfesus>
    {
        #region Construtores

        public IntegracaoEfesus(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoEfesus(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
