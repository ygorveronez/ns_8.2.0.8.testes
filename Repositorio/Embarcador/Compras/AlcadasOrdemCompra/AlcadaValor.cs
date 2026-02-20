using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras.AlcadasOrdemCompra
{
    public class AlcadaValor : RepositorioBase<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor>
    {
        public AlcadaValor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor> BuscarPorRegra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor>();
            var result = from obj in query where obj.RegrasOrdemCompra.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}
