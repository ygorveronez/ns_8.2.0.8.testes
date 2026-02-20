using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoYandeh : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYandeh>
    {
        #region Construtores

        public IntegracaoYandeh(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoYandeh(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYandeh Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYandeh>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion Métodos Públicos
    }
}
