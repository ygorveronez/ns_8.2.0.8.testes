using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras
{
    public class AlcadaMotivo : RepositorioBase<Dominio.Entidades.Embarcador.Compras.AlcadaMotivo>
    {
        public AlcadaMotivo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Compras.AlcadaMotivo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadaMotivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AlcadaMotivo> BuscarPorRegra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadaMotivo>();
            var result = from obj in query where obj.RegrasRequisicaoMercadoria.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}
