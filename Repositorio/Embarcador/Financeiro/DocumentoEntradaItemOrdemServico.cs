using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class DocumentoEntradaItemOrdemServico : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico>
    {
        public DocumentoEntradaItemOrdemServico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico>();
            query = from obj in query where obj.Codigo == codigo select obj;
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico> BuscarPorDocumentoEntrada(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico>();

            query = from obj in query where obj.DocumentoEntradaItem.DocumentoEntrada.Codigo == codigoDocumentoEntrada select obj;

            return query.ToList();
        }

        #endregion
    }
}
