using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PagamentoAgregado
{
    public class AlcadaValorPagamentoAgregado : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado>
    {
        public AlcadaValorPagamentoAgregado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado> BuscarPorRegra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado>();
            var result = from obj in query where obj.RegraPagamentoAgregado.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}
