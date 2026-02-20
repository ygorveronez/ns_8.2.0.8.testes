using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class LoteComprovanteEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega>
    {
        public LoteComprovanteEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega> BuscarPorCargaEntrega(int codigoCargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega>();
            //var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega>();
            var result = (from obj in query where obj.CargaEntrega.Codigo == codigoCargaEntrega select obj);

            if(result != null)
            {
                return (from obj in result select obj.LoteComprovanteEntrega).ToList();
            }

            return null;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega> _Consultar(int codigo, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega>();

            var result = from obj in query select obj;

            // Filtros

            if(codigo > 0)
            {
                result = result.Where(o => o.Codigo == codigo);
            }

            if(codigoCarga > 0)
            {
                result = result.Where(o => o.Carga.Codigo == codigoCarga);
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega> Consultar(int codigo, int codigoCarga, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigo, codigoCarga);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsulta(int codigo, int codigoCarga)
        {
            var result = _Consultar(codigo, codigoCarga);
            return result.Count();
        }
    }

}