using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoProduto>
    {
        public ConfiguracaoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoProduto BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoProduto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoProduto>();

            return query.FirstOrDefault();
        }

    }
}
