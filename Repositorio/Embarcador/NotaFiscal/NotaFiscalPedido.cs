using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class NotaFiscalPedido : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalPedido>
    {
        public NotaFiscalPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalPedido> BuscarPorNota(int codigoNota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalPedido>();
            var result = from obj in query where obj.NotaFiscal.Codigo == codigoNota select obj;
            return result.ToList();
        }
    }
}
