using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class ValePedagioMDFe : RepositorioBase<Dominio.Entidades.ValePedagioMDFe>, Dominio.Interfaces.Repositorios.ValePedagioMDFe
    {
        public ValePedagioMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ValePedagioMDFe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.ValePedagioMDFe BuscarPorCodigo(int codigo, int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValePedagioMDFe>();
            var result = from obj in query where obj.Codigo == codigo && obj.MDFe.Codigo == codigoMDFe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ValePedagioMDFe> BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValePedagioMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.ToList();
        }

        public decimal BuscarValorValePedagioPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValePedagioMDFe>();

            return query
                .Where(x => x.MDFe.Codigo == codigoMDFe)
                .Sum(x => (decimal?)x.ValorValePedagio) ?? 0m;
        }

        public Task<List<Dominio.Entidades.ValePedagioMDFe>> BuscarPorMDFeAsync(int codigoMDFe, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValePedagioMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;

            return result.ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.ValePedagioMDFe> BuscarPorMdfeENroComprovante(int codigoMdfe, string numeroComprovante)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValePedagioMDFe>();
            query = query.Where(o => o.MDFe.Codigo == codigoMdfe && o.NumeroComprovante == numeroComprovante);
            return query.ToList();
        }
    }
}
