using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class AbaFiltroPesquisaPersonalizado : RepositorioBase<Dominio.Entidades.Global.AbaFiltroPersonalizado>
    {
        public AbaFiltroPesquisaPersonalizado(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.Global.AbaFiltroPersonalizado BuscarFiltroPesquisaCliente(int codigoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Global.AbaFiltroPersonalizado>();
            return query.Where(o => o.Cliente == codigoCliente).FirstOrDefault();

        }

        public Dominio.Entidades.Global.AbaFiltroPersonalizado BuscarFiltroPadrao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Global.AbaFiltroPersonalizado>();
            return query.FirstOrDefault();

        }

    }
}
