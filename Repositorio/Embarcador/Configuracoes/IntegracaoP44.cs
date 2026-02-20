using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoP44 : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoP44>
    {
        #region Construtores

        public IntegracaoP44(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoP44(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoP44 Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoP44>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
