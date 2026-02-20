using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class VeiculoMDFe : RepositorioBase<Dominio.Entidades.VeiculoMDFe>, Dominio.Interfaces.Repositorios.VeiculoMDFe
    {
        public VeiculoMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public VeiculoMDFe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.VeiculoMDFe BuscarPorCodigo(int codigo, int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoMDFe>();
            var result = from obj in query where obj.Codigo == codigo && obj.MDFe.Codigo == codigoMDFe select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.VeiculoMDFe BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.VeiculoMDFe> BuscarPorMDFeAsync(int codigoMDFe, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;

            return result.FirstOrDefaultAsync(cancellationToken);
        }

        public List<Dominio.Entidades.VeiculoMDFe> BuscarPorMDFes(int[] codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoMDFe>();
            var result = from obj in query where codigoMDFe.Contains(obj.MDFe.Codigo) select obj;
            return result.ToList();
        }
    }
}
