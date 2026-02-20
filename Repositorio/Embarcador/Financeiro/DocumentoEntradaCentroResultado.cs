using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class DocumentoEntradaCentroResultado : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultado>
    {
        public DocumentoEntradaCentroResultado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultado BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultado>();

            query = from obj in query where obj.Codigo == codigo select obj;

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultado> BuscarPorDocumentoEntrada(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultado>();

            query = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada select obj;

            return query.ToList();
        }
    }
}