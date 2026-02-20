using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoSapAPI4 : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSapAPI4>
    {
        #region Construtores

        public IntegracaoSapAPI4(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoSapAPI4(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
