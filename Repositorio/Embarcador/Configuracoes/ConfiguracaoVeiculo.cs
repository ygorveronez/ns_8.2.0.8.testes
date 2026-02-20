using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo>
    {
        public ConfiguracaoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoVeiculo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo>();

            return query.FirstOrDefault();
        }
    }
}
