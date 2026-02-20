using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoRelatorio : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio>
    {
        public ConfiguracaoRelatorio(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoRelatorio(UnitOfWork unitOfWork, CancellationToken cancellation) : base(unitOfWork, cancellation) { }

        public virtual Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio>();

            return query.FirstOrDefault();
        }
        public async virtual Task<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio> BuscarConfiguracaoPadraoAsync()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio>();

            return await query.FirstOrDefaultAsync();
        }
    }
}
