using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.WMS
{
    public class AlcadaProdutoEmbarcador : RepositorioBase<Dominio.Entidades.Embarcador.WMS.AlcadaProdutoEmbarcador>
    {
        public AlcadaProdutoEmbarcador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.WMS.AlcadaProdutoEmbarcador BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.AlcadaProdutoEmbarcador>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.WMS.AlcadaProdutoEmbarcador> BuscarPorRegra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.AlcadaProdutoEmbarcador>();
            var result = from obj in query where obj.RegraDescarte.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}
