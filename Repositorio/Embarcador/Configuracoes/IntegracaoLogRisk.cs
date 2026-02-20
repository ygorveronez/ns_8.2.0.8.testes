using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoLogRisk : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLogRisk>
    {
        #region Construtores

        public IntegracaoLogRisk(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoLogRisk(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLogRisk Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLogRisk>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
