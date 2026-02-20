using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class MunicipioCarregamentoMDFe : RepositorioBase<Dominio.Entidades.MunicipioCarregamentoMDFe>, Dominio.Interfaces.Repositorios.MunicipioCarregamentoMDFe
    {
        public MunicipioCarregamentoMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public MunicipioCarregamentoMDFe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.MunicipioCarregamentoMDFe BuscarPorCodigo(int codigo, int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioCarregamentoMDFe>();
            var result = from obj in query where obj.Codigo == codigo && obj.MDFe.Codigo == codigoMDFe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.MunicipioCarregamentoMDFe> BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioCarregamentoMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result
                .Fetch(obj => obj.Municipio)
                .ToList();
        }

        public Task<List<Dominio.Entidades.MunicipioCarregamentoMDFe>> BuscarPorMDFeAsync(int codigoMDFe, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioCarregamentoMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result
                .Fetch(obj => obj.Municipio)
                .ToListAsync(cancellationToken);
        }
    }
}
