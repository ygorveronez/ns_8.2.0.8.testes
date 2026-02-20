using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoTrizy : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy>
    {
        #region Construtores

        public IntegracaoTrizy(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoTrizy(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
