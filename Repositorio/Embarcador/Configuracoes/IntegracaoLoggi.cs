using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoLoggi : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi>
    {
        #region Construtores

        public IntegracaoLoggi(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoLoggi(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
