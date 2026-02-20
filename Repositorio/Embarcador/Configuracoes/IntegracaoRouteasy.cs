using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoRouteasy : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy>
    {
        #region Construtores

        public IntegracaoRouteasy(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoRouteasy(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy Buscar()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy> consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion Métodos Públicos
    }
}
