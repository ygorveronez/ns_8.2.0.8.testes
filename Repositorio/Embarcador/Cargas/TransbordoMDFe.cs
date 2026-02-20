using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class TransbordoMDFe : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TransbordoMDFe>
    {
        public TransbordoMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.TransbordoMDFe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TransbordoMDFe>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TransbordoMDFe> BuscarPorTransbordo(int codigoTransbordo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TransbordoMDFe>();
            var result = from obj in query where obj.Transbordo.Codigo == codigoTransbordo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TransbordoMDFe> Consultar(int transbordo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TransbordoMDFe>();

            var result = from obj in query select obj;

            if (transbordo > 0)
                result = result.Where(obj => obj.Transbordo.Codigo == transbordo);
            

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int transbordo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TransbordoMDFe>();

            var result = from obj in query select obj;

            if (transbordo > 0)
                result = result.Where(obj => obj.Transbordo.Codigo == transbordo);
            
            return result.Count();
        }
    }
}
