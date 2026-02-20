using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoMotorista : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista>
    {
        public ConfiguracaoMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoMotorista(UnitOfWork unitOfWork, CancellationToken cancelationToken) : base(unitOfWork, cancelationToken) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista>();

            return query.FirstOrDefault();
        }
    }
}
