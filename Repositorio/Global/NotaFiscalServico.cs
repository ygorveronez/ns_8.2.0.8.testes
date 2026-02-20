using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class NotaFiscalServico : RepositorioBase<Dominio.Entidades.NotaFiscalServico>
    {
        public NotaFiscalServico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.NotaFiscalServico BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalServico>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
