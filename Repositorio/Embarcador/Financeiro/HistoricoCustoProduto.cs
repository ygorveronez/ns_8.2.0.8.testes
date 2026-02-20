using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class HistoricoCustoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.HistoricoCustoProduto>
    {
        public HistoricoCustoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.HistoricoCustoProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.HistoricoCustoProduto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.HistoricoCustoProduto BuscarPorItemNota(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.HistoricoCustoProduto>();
            var result = from obj in query where obj.DocumentoEntradaItem.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
