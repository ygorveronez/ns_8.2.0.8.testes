using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoFluxoPatio : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFluxoPatio>
    {
        public ConfiguracaoFluxoPatio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFluxoPatio BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFluxoPatio> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFluxoPatio>();

            return query.FirstOrDefault();
        }
    }
}