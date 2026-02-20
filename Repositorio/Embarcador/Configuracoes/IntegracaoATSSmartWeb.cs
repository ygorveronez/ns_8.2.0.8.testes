using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoATSSmartWeb : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoATSSmartWeb>
    {
        #region Construtores

        public IntegracaoATSSmartWeb(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoATSSmartWeb(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoATSSmartWeb Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoATSSmartWeb>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
