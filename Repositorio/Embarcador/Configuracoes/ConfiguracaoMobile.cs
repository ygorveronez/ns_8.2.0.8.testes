using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoMobile : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMobile>
    {
        public ConfiguracaoMobile(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMobile BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMobile> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMobile>();

            return query.FirstOrDefault();
        }
    }
}
