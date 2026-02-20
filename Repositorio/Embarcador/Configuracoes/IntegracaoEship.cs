using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoEShip : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEship>
    {
        #region Construtores

        public IntegracaoEShip(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoEShip(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEship Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEship>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
