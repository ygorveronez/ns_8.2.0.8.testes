using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoAgendamentoEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega>
    {
        public ConfiguracaoAgendamentoEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega>();

            return query.FirstOrDefault();
        }
    }
}