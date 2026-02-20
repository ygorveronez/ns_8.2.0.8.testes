using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class NotaFiscalBoleto : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalBoleto>
    {
        public NotaFiscalBoleto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalBoleto> BuscarPorNotaFiscal(List<int> numeroNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalBoleto>();
            var result = from obj in query where numeroNotaFiscal.Contains(obj.XMLNotaFiscal.Numero) select obj;

            return result.ToList();
        }

        #endregion

    }
}
