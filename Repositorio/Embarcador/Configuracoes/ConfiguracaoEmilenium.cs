using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoEmilenium : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmilenium>
    {
        public ConfiguracaoEmilenium(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmilenium BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmilenium> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmilenium>();

            return query.FirstOrDefault();
        }
    }
}
