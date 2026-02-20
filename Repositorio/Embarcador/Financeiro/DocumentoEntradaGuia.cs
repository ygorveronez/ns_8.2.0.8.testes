using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class DocumentoEntradaGuia : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia>
    {
        public DocumentoEntradaGuia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia>();

            query = from obj in query where obj.Codigo == codigo select obj;

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia> BuscarPorDocumentoEntrada(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia>();

            query = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada orderby obj.DataVencimento ascending select obj;

            return query.ToList();
        }

        public bool PagamentoAVista(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia>();

            query = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada && obj.DataVencimento == obj.DocumentoEntrada.DataEmissao select obj;

            return query.Count() >= 1;
        }
    }
}
