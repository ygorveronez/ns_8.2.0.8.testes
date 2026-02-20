using NHibernate.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoCargaEmissaoDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento>
    {
        public ConfiguracaoCargaEmissaoDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento>();

            return query.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento> BuscarConfiguracaoPadraoAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento>();

            return await query.FirstOrDefaultAsync();
        }
    }
}
