using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoGlobus : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGlobus>
    {
        public IntegracaoGlobus(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoGlobus(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGlobus Buscar()
        {
            var consultaIntegracaoGlobus = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGlobus>();

            return consultaIntegracaoGlobus.FirstOrDefault();
        }

        #endregion
    }
}
