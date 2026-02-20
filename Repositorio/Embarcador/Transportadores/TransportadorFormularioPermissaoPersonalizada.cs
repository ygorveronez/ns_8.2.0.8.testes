using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Transportadores
{
    public class TransportadorFormularioPermissaoPersonalizada : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.TransportadorFormularioPermissaoPersonalizada>
    {
        public TransportadorFormularioPermissaoPersonalizada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Transportadores.TransportadorFormularioPermissaoPersonalizada BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorFormularioPermissaoPersonalizada>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
