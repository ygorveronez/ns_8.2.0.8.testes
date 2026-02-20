using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoDocumentoDestinado : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado>
    {
        public ConfiguracaoDocumentoDestinado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ConfiguracaoDocumentoDestinado(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado>();

            return query.FirstOrDefault();
        }
    }
}
