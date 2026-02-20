using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras.AlcadasOrdemCompra
{
    public class AlcadaFornecedor : RepositorioBase<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor>
    {
        public AlcadaFornecedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor> BuscarPorRegra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor>();
            var result = from obj in query where obj.RegrasOrdemCompra.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}
