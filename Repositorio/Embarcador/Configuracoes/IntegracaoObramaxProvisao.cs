using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoObramaxProvisao : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxProvisao>
    {
        #region Construtores

        public IntegracaoObramaxProvisao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoObramaxProvisao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxProvisao Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxProvisao>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
