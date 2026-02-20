using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class DocumentoEntradaCentroResultadoTipoDespesa : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa>
    {
        public DocumentoEntradaCentroResultadoTipoDespesa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa> BuscarPorDocumentoEntrada(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa>();

            var result = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada select obj;

            return result.ToList();
        }
    }
}
