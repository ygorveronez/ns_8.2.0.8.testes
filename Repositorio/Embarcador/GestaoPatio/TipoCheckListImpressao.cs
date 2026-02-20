using NHibernate.Linq;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class TipoChecklistImpressao : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.TipoChecklistImpressao>
    {
        public TipoChecklistImpressao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Task<bool> VerificarExistenciaAsync(CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.TipoChecklistImpressao>()
                .Select(x => x.Codigo)
                .AnyAsync(cancellationToken);

            return query;
        }
    }
}