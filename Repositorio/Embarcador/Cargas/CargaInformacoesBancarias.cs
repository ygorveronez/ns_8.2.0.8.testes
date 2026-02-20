using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{

    public class CargaInformacoesBancarias : RepositorioBase<Dominio.Entidades.Global.CargaInformacoesBancarias>
    {
        #region Construtores

        public CargaInformacoesBancarias(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaInformacoesBancarias(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Metodos Públicos
        public Dominio.Entidades.Global.CargaInformacoesBancarias BuscarPorCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Global.CargaInformacoesBancarias>();

            query = query.Where(o => o.Carga.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Global.CargaInformacoesBancarias> BuscarPorCargaAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Global.CargaInformacoesBancarias>();

            query = query.Where(o => o.Carga.Codigo == codigo);

            return query.FirstOrDefaultAsync();
        }

        public Task<bool> RegistradoPeloEmbarcadorPorCargaAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Global.CargaInformacoesBancarias>();

            query = query.Where(o => o.Carga.Codigo == codigo);

            return query.Select(x => x.RegistradoPeloEmbarcador ?? false).FirstOrDefaultAsync(CancellationToken);
        }
        #endregion

    }
}