using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoGeralOpenTech : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralOpenTech>
    {
        #region Construtores

        public IntegracaoGeralOpenTech(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoGeralOpenTech(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralOpenTech Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralOpenTech>();

            return consultaIntegracao.FirstOrDefault();
        }
    }
}
