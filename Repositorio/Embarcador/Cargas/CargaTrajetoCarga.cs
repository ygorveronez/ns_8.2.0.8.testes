using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaTrajetoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga>
    {
        public CargaTrajetoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public int BuscarOrdemPorCargaTrajeto(int codigoCargaTrajeto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga>();

            query = query.Where(obj => obj.Codigo == codigoCargaTrajeto);

            return query.OrderByDescending(obj => obj.Ordem).Select(obj => obj.Ordem).FirstOrDefault();
        }
        
        public Task<int> BuscarOrdemPorCargaTrajetoAsync(int codigoCargaTrajeto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga>();

            query = query.Where(obj => obj.Codigo == codigoCargaTrajeto);

            return query.OrderByDescending(obj => obj.Ordem).Select(obj => obj.Ordem).FirstOrDefaultAsync();
        }
        
        public Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }
        
        public List<Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga> BuscarPorCargaTrajeto(List<int> codigosCargaTrajeto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga>()
                .Where(obj => codigosCargaTrajeto.Contains(obj.CargaTrajeto.Codigo));

            return query
                .Fetch(obj => obj.CargaTrajeto)
                .Fetch(obj => obj.Carga)
                .ToList();
        }
    }
}
