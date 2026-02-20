using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoAbastecimentos : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAbastecimentos>
    {
        public ConfiguracaoAbastecimentos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAbastecimentos BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAbastecimentos> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAbastecimentos>();

            return query.FirstOrDefault();
        }
    }
}