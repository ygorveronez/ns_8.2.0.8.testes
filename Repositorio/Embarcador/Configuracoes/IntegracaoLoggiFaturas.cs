using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoLoggiFaturas : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggiFaturas>
    {
        #region Construtores

        public IntegracaoLoggiFaturas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoLoggiFaturas(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggiFaturas Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggiFaturas>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
