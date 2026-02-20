using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class LacreMDFe : RepositorioBase<Dominio.Entidades.LacreMDFe>, Dominio.Interfaces.Repositorios.LacreMDFe
    {
        public LacreMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public LacreMDFe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.LacreMDFe BuscarPorCodigo(int codigo, int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LacreMDFe>();
            var result = from obj in query where obj.Codigo == codigo && obj.MDFe.Codigo == codigoMDFe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.LacreMDFe> BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LacreMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.LacreMDFe>> BuscarPorMDFeAsync(int codigoMDFe, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LacreMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;

            return result.ToListAsync(cancellationToken);
        }
    }
}
