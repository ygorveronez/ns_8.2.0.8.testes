using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoValoresCTeLoggi : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi>
    {
        #region Construtores

        public IntegracaoValoresCTeLoggi(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoValoresCTeLoggi(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}

