using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoMongo : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMongo>
    {
        public ConfiguracaoMongo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMongo BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMongo> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMongo>();

            return query.FirstOrDefault();
        }
    }
}