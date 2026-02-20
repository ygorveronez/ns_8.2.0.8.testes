using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class PercursoMDFe : RepositorioBase<Dominio.Entidades.PercursoMDFe>, Dominio.Interfaces.Repositorios.PercursoMDFe
    {
        public PercursoMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PercursoMDFe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.PercursoMDFe BuscarPorCodigo(int codigo, int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoMDFe>();
            var result = from obj in query where obj.Codigo == codigo && obj.MDFe.Codigo == codigoMDFe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.PercursoMDFe> BuscarPorMDFe(int codigoMDFe)
        {
            IQueryable<Dominio.Entidades.PercursoMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoMDFe>();

            query = query.Where(o => o.MDFe.Codigo == codigoMDFe);

            return query.Fetch(o => o.Estado).ToList();

        }

        public Task<List<Dominio.Entidades.PercursoMDFe>> BuscarPorMDFeAsync(int codigoMDFe)
        {
            IQueryable<Dominio.Entidades.PercursoMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoMDFe>();

            query = query.Where(o => o.MDFe.Codigo == codigoMDFe);

            return query.Fetch(o => o.Estado).ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.PercursoMDFe>> BuscarPorMDFeAsync(int codigoMDFe, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.PercursoMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoMDFe>();

            query = query.Where(o => o.MDFe.Codigo == codigoMDFe);

            return query.Fetch(o => o.Estado).ToListAsync(cancellationToken);

        }

        public List<string> BuscarSiglaEstadoPorMDFe(int codigoMDFe)
        {
            IQueryable<Dominio.Entidades.PercursoMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoMDFe>();

            query = query.Where(o => o.MDFe.Codigo == codigoMDFe);

            return query.Select(o => o.Estado.Sigla).ToList();
        }
    }
}
