using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoGNRE : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGNRE>
    {
        public IntegracaoGNRE(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoGNRE(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGNRE Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGNRE>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }

}