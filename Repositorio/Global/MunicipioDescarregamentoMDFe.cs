using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class MunicipioDescarregamentoMDFe : RepositorioBase<Dominio.Entidades.MunicipioDescarregamentoMDFe>, Dominio.Interfaces.Repositorios.MunicipioDescarregamentoMDFe
    {
        public MunicipioDescarregamentoMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public MunicipioDescarregamentoMDFe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.MunicipioDescarregamentoMDFe BuscarPorCodigo(int codigo, int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioDescarregamentoMDFe>();
            var result = from obj in query where obj.Codigo == codigo && obj.MDFe.Codigo == codigoMDFe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.MunicipioDescarregamentoMDFe> BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioDescarregamentoMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.MunicipioDescarregamentoMDFe>> BuscarPorMDFeAsync(int codigoMDFe, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioDescarregamentoMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;

            return result.ToListAsync(cancellationToken);
        }

        public Dominio.Entidades.MunicipioDescarregamentoMDFe BuscarPrimeiroPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioDescarregamentoMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.MunicipioDescarregamentoMDFe BuscarPorMunicipioDescarregamento(int codigoMDFe, int codigoMunicipio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioDescarregamentoMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.Municipio.Codigo == codigoMunicipio select obj;
            return result.FirstOrDefault();
        }
    }
}
