using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class DocumentoEntradaItemAbastecimento : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento>
    {
        public DocumentoEntradaItemAbastecimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento>();
            query = from obj in query where obj.Codigo == codigo select obj;
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento> BuscarPorDocumentoEntradaItem(int codigoDocumentoEntradaItem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento>();
            query = from obj in query where obj.DocumentoEntradaItem.Codigo == codigoDocumentoEntradaItem select obj;
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento> BuscarPorAbastecimentoDocumentoEntradaItem(int codigoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento>();
            query = from obj in query where obj.Abastecimento.Codigo == codigoAbastecimento select obj;
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento> BuscarPorDocumentoEntrada(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento>();

            query = from obj in query where obj.DocumentoEntradaItem.DocumentoEntrada.Codigo == codigoDocumentoEntrada select obj;

            return query.ToList();
        }

        #endregion
    }
}
