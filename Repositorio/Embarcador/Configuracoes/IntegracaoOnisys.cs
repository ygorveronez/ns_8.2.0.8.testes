using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoOnisys : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoOnisys>
    {
        #region Construtores
        public IntegracaoOnisys(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IntegracaoOnisys(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }
        #endregion Construtores
    }
}
