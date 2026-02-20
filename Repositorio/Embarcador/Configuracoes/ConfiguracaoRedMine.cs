using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoRedMine : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRedMine>
    {
        public ConfiguracaoRedMine(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRedMine BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRedMine> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRedMine>();

            return query.FirstOrDefault();
        }
    }
}
