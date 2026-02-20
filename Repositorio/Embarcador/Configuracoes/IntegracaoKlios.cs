using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoKlios : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKlios>
    {
        #region Constutores

        public IntegracaoKlios(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoKlios(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Constutores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKlios Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKlios>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
