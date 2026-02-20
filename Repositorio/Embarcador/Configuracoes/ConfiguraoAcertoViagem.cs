using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguraoAcertoViagem : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem>
    {
        public ConfiguraoAcertoViagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem>();

            return query.FirstOrDefault();
        }
    }
}
