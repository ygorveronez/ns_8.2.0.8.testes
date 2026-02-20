using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoObramaxCTE : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxCTE>
    {
        #region Construtores

        public IntegracaoObramaxCTE(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoObramaxCTE(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxCTE Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxCTE>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
