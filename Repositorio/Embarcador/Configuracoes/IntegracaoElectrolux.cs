using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoElectrolux : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoElectrolux>
    {
        #region Construtores

        public IntegracaoElectrolux(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoElectrolux(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoElectrolux Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoElectrolux>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
