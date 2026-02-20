using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class MotoristaMDFe : RepositorioBase<Dominio.Entidades.MotoristaMDFe>, Dominio.Interfaces.Repositorios.MotoristaMDFe
    {
        public MotoristaMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public MotoristaMDFe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.MotoristaMDFe BuscarPorCodigo(int codigo, int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaMDFe>();
            var result = from obj in query where obj.Codigo == codigo && obj.MDFe.Codigo == codigoMDFe select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.MotoristaMDFe BuscarPorCpf(int codigoMDFe, string cpf, Dominio.Enumeradores.TipoMotoristaMDFe tipo)
        {
            var motorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaMDFe>()
                .Where(o => o.MDFe.Codigo == codigoMDFe && o.CPF == cpf && o.Tipo == tipo)
                .FirstOrDefault();

            return motorista;
        }

        public List<Dominio.Entidades.MotoristaMDFe> BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && (obj.Tipo == Dominio.Enumeradores.TipoMotoristaMDFe.Normal || obj.Tipo == Dominio.Enumeradores.TipoMotoristaMDFe.Incluido) select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.MotoristaMDFe>> BuscarPorMDFeAsync(int codigoMDFe, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && (obj.Tipo == Dominio.Enumeradores.TipoMotoristaMDFe.Normal || obj.Tipo == Dominio.Enumeradores.TipoMotoristaMDFe.Incluido) select obj;
            
            return result.ToListAsync(cancellationToken);
        }

        public Dominio.Entidades.MotoristaMDFe BuscarPorTipo(int codigoMDFe, Dominio.Enumeradores.TipoMotoristaMDFe tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.Tipo == Dominio.Enumeradores.TipoMotoristaMDFe.SolicitadoEventoInclusao select obj;
            return result.FirstOrDefault();
        }
    }
}
