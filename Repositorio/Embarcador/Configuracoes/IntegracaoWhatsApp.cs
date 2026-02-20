using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoWhatsApp : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoWhatsApp>
    {
        #region Construtores

        public IntegracaoWhatsApp(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoWhatsApp(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoWhatsApp Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoWhatsApp>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
