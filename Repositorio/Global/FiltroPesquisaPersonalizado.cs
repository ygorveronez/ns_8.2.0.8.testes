using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class FiltroPesquisaPersonalizado : RepositorioBase<Dominio.Entidades.Global.FiltroPersonalizado>
	{
		public FiltroPesquisaPersonalizado(UnitOfWork unitOfWork) :base(unitOfWork) { }

        public List<Dominio.Entidades.Global.FiltroPersonalizado> BuscarFiltrosCliente(int codigoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Global.FiltroPersonalizado>();
            return query.Where(o => o.AbaFiltroPersonalizado.Cliente == codigoCliente).ToList();

        }

        public List<Dominio.Entidades.Global.FiltroPersonalizado> BuscarFiltrosPorAba(int codigoAba)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Global.FiltroPersonalizado>();
            return query.Where(o => o.AbaFiltroPersonalizado.Codigo == codigoAba).ToList();

        }

        public bool ExisteFiltroJaCadastrado(string nomeFiltro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Global.FiltroPersonalizado>();
            return query.Any(f => f.NomeFiltro == nomeFiltro);
        }
    }
}
