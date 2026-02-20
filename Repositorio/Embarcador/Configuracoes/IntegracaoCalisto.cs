using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoCalisto : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCalisto>
    {
        #region Construtores

        public IntegracaoCalisto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoCalisto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCalisto Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCalisto>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}

