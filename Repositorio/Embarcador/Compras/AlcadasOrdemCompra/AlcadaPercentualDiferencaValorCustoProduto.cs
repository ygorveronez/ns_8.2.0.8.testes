using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras.AlcadasOrdemCompra
{
    public class AlcadaPercentualDiferencaValorCustoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto>
    {
        public AlcadaPercentualDiferencaValorCustoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto> BuscarPorRegra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto>();
            var result = from obj in query where obj.RegrasOrdemCompra.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}
