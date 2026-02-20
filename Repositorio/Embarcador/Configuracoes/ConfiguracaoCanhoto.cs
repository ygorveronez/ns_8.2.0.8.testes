using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoCanhoto : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto>
    {
        #region Construtores

        public ConfiguracaoCanhoto(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoCanhoto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto>();

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto> BuscarConfiguracaoPadraoAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto>();

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        #endregion
    }
}
