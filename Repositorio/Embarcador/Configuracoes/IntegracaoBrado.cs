using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoBrado : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado>
    {
        #region Construtores

        public IntegracaoBrado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoBrado(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
