using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.Ofertas
{
    public class ParametrosOfertasDadosOferta : RepositorioRelacionamentoParametrosOfertas<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOferta>, Dominio.Interfaces.Embarcador.Cargas.Ofertas.IRepositorioRelacionamentoParametrosOfertas
    {
        #region Construtores

        public ParametrosOfertasDadosOferta(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Publicos

        public Task<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOferta> BuscarPorCodigoParametrosOfertaAsync(long codigoParametroOferta, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOferta> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOferta>()
                .Where(p => p.ParametrosOfertas.Codigo == codigoParametroOferta);

            return query.FirstOrDefaultAsync(cancellationToken);
        }
        #endregion
    }
}
