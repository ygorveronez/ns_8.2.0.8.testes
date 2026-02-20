using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoFlora : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFlora>
    {
        #region Construtores

        public IntegracaoFlora(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoFlora(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFlora Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFlora>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion Métodos Públicos
    }
}

