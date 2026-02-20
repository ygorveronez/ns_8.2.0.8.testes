using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class NotaFiscalCancelamento : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCancelamento>
    {
        public NotaFiscalCancelamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCancelamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCancelamento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCancelamento BuscarPorNota(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCancelamento>();
            var result = from obj in query where obj.NotaFiscal.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
