using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoBuntech : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBuntech>
    {
        #region Construtores

        public IntegracaoBuntech(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoBuntech(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBuntech Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBuntech>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
