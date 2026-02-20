using NHibernate.Linq;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoIntegracaoFTP : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracaoFTP>
    {
        public TipoOperacaoIntegracaoFTP(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public TipoOperacaoIntegracaoFTP(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracaoFTP> BuscarPorTipoOperacaoAsync(int tipoOperacao, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracaoFTP>();
            query = query.Where(o => o.TipoOperacao.Codigo == tipoOperacao);
            return query.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
