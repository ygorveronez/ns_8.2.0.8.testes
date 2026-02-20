using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoHUB : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoHUB>
    {
        #region Construtores

        public IntegracaoHUB(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoHUB(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoHUB Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoHUB>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
