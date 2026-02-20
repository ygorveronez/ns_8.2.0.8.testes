using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoPessoa : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPessoa>
    {
        public ConfiguracaoPessoa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPessoa BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPessoa> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPessoa>();

            return query.FirstOrDefault();
        }
    }
}