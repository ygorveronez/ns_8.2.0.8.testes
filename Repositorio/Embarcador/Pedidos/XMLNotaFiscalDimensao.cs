using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class XMLNotaFiscalDimensao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalDimensao>
    {
        public XMLNotaFiscalDimensao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalDimensao> BuscarPorXMLNotaFiscal(int codigoXMLNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalDimensao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalDimensao>();

            query = query.Where(o => o.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal);

            return query.ToList();
        }
    }
}
