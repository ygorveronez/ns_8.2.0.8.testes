using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class XMLNotaFiscalComprovanteEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.XMLNotaFiscalComprovanteEntrega>
    {
        public XMLNotaFiscalComprovanteEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.XMLNotaFiscalComprovanteEntrega> _Consultar()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.XMLNotaFiscalComprovanteEntrega>();

            var result = from obj in query select obj;

            // Filtros
            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.XMLNotaFiscalComprovanteEntrega> Consultar(string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar();

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.XMLNotaFiscalComprovanteEntrega> BuscarPorLoteComprovanteEntrega(int codigoLoteComprovanteEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.XMLNotaFiscalComprovanteEntrega>();
            var result = query.Where(o => o.LoteComprovanteEntrega.Codigo == codigoLoteComprovanteEntrega);

            return result
                .Fetch(obj => obj.DadosRecebedor)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.XMLNotaFiscalComprovanteEntrega BuscarPorLoteEXmlNotaFiscal(int codigoLote, int codigoXmlNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.XMLNotaFiscalComprovanteEntrega>();
            var result = query.Where(o => o.LoteComprovanteEntrega.Codigo == codigoLote && o.PedidoXMLNotaFiscal != null && o.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == codigoXmlNotaFiscal);

            return result.FirstOrDefault();
        }

        public int ContarConsulta()
        {
            var result = _Consultar();
            return result.Count();
        }
    }

}