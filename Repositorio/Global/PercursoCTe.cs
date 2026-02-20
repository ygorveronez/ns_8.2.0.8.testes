using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class PercursoCTe : RepositorioBase<Dominio.Entidades.PercursoCTe>, Dominio.Interfaces.Repositorios.PercursoCTe
    {
        public PercursoCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.PercursoCTe BuscarPorCodigo(int codigo, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoCTe>();
            var result = from obj in query where obj.Codigo == codigo && obj.CTe.Codigo == codigoCTe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.PercursoCTe> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoCTe>();
            return query.Where(x => x.CTe.Codigo == codigoCTe)
                .Fetch(x => x.Estado)
                .ToList();

        }
    }
}
