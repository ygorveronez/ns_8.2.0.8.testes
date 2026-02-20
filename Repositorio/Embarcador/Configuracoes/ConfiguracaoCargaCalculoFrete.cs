using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoCargaCalculoFrete : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaCalculoFrete>
    {
        public ConfiguracaoCargaCalculoFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaCalculoFrete BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaCalculoFrete> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaCalculoFrete>();

            return query.FirstOrDefault();
        }
    }
}
