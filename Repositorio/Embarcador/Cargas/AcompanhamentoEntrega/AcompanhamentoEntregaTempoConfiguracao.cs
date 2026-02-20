using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao
{
    public class AcompanhamentoEntregaTempoConfiguracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao>
    {
        public AcompanhamentoEntregaTempoConfiguracao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public AcompanhamentoEntregaTempoConfiguracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao BuscarConfiguracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao>();
            var result = from obj in query select obj;
            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao> BuscarConfiguracaoAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao>();

            var result = from obj in query select obj;

            return await result.FirstOrDefaultAsync(CancellationToken);
        }
    }
}
