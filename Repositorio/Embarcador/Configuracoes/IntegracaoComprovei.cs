using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoComprovei : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei>
    {
        #region Construtores

        public IntegracaoComprovei(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoComprovei(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
