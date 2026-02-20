using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoLog : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLog>
    {
        public ConfiguracaoLog(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLog BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLog> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLog>();

            return query.FirstOrDefault();
        }
    }
}