using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoMercosul : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMercosul>
    {
        public ConfiguracaoMercosul(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMercosul BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMercosul> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMercosul>();

            return query.FirstOrDefault();
        }
    }
}
