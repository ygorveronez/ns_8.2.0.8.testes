using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class SMViagemMDFeLog : RepositorioBase<Dominio.Entidades.SMViagemMDFeLog>, Dominio.Interfaces.Repositorios.SMViagemMDFeLog
    {
        public SMViagemMDFeLog(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.SMViagemMDFeLog BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SMViagemMDFeLog>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.SMViagemMDFeLog> BuscarPorSMViagemMDFe(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SMViagemMDFeLog>();
            var result = from obj in query where obj.SMViagemMDFe.Codigo == codigo select obj;
            return result.ToList();
        }
    }
}
