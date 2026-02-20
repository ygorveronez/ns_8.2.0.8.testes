using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoDiageo : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDiageo>
    {
        #region Construtores

        public IntegracaoDiageo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoDiageo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDiageo Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDiageo>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
