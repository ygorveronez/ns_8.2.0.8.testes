using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoFatura : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura>
    {
        public ConfiguracaoFatura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura>();

            return query.FirstOrDefault();
        }
    }
}
