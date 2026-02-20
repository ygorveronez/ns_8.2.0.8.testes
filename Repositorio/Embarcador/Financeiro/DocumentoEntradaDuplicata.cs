using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class DocumentoEntradaDuplicata : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata>
    {
        public DocumentoEntradaDuplicata(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata>();

            query = from obj in query where obj.Codigo == codigo select obj;

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata> BuscarPorDocumentoEntrada(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata>();

            query = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada orderby obj.DataVencimento ascending select obj;

            return query.ToList();
        }

        public bool PagamentoAVista(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata>();

            query = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada && obj.DataVencimento == obj.DocumentoEntrada.DataEmissao select obj;

            return query.Count() >= 1;
        }
    }
}
