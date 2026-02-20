using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador>
    {
        public ConfiguracaoTransportador(UnitOfWork unitOfWork) :base (unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador>();

            return query.FirstOrDefault();
        }
    }
}
