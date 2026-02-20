using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoObramaxNFE : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxNFE>
    {
        #region Construtores

        public IntegracaoObramaxNFE(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoObramaxNFE(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxNFE Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxNFE>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
