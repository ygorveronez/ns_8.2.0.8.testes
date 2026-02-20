using NHibernate.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.TorreControle
{
    public class ConfiguracaoUsuarioMonitoramentoMapa : RepositorioBase<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoUsuarioMonitoramentoMapa>
    {
        public ConfiguracaoUsuarioMonitoramentoMapa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public async Task<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoUsuarioMonitoramentoMapa> BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoUsuarioMonitoramentoMapa> consultaConfiguracaoUsuarioMonitoramentoMapa = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoUsuarioMonitoramentoMapa>();

            return await consultaConfiguracaoUsuarioMonitoramentoMapa.OrderBy(obj => obj.Codigo).FirstOrDefaultAsync();
        }

        public async Task<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoUsuarioMonitoramentoMapa> BuscarConfiguracaoPorUsuarioAsync(int codigoUsuario)
        {
            IQueryable<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoUsuarioMonitoramentoMapa> consultaConfiguracaoUsuarioMonitoramentoMapa = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoUsuarioMonitoramentoMapa>();

            return consultaConfiguracaoUsuarioMonitoramentoMapa.Where(obj => obj.Usuario.Codigo == codigoUsuario).FirstOrDefault();
        }

        #endregion
    }
}
