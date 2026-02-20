using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoCalculoPrevisao : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao>
    {
        public ConfiguracaoCalculoPrevisao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao>();

            return query.FirstOrDefault();
        }

    }
}
