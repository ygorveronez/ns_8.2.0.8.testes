using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.ValePedagio
{
    public class CargaValePedagioRota : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota>
    {
        public CargaValePedagioRota(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota> BuscarPorCargaValePedagio(int codigoCargaValePedagio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota>();

            var result = from obj in query where obj.CargaValePedagio.Codigo == codigoCargaValePedagio select obj;

            return result.ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota>> BuscarPorCargaValePedagioAsync(int codigoCargaValePedagio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota>();

            var result = from obj in query where obj.CargaValePedagio.Codigo == codigoCargaValePedagio select obj;

            return await result.ToListAsync();
        }

    }
}
