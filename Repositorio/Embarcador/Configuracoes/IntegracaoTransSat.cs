using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoTransSat : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTransSat>
    {
        public IntegracaoTransSat(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoTransSat(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTransSat Buscar()
        {
            var consultaIntegracaoTransSat = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTransSat>();

            return consultaIntegracaoTransSat.FirstOrDefault();
        }

        #endregion
    }
}
