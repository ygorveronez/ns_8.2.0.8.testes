using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class ConfiguracaoTipoOperacaoCanhoto : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCanhoto>
    {
        public ConfiguracaoTipoOperacaoCanhoto(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoTipoOperacaoCanhoto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCanhoto BuscarConfiguracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCanhoto>();
            var result = from obj in query select obj;
            return result.FirstOrDefault();
        }
    }
}