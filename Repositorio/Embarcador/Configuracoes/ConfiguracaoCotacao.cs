using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoCotacao : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCotacao>
    {
        public ConfiguracaoCotacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCotacao BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCotacao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCotacao>();

            return query.FirstOrDefault();
        }
    }
}
