using NHibernate;
using System.Linq;


namespace Repositorio.Embarcador.Pedidos
{
    public class NavioOperador : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.NavioOperador>
    {
        public NavioOperador(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.Entidades.Embarcador.Pedidos.NavioOperador BuscarPorNavioID(string idOperador, int navioCodigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.NavioOperador>();

            var result = from obj in query where obj.IdOperador == idOperador && obj.Navio.Codigo == navioCodigo select obj;

            return result.FirstOrDefault();
        }
    }
}
