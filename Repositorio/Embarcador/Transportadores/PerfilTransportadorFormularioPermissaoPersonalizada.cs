using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Transportadores
{
    public class PerfilTransportadorFormularioPermissaoPersonalizada : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada>
    {
        public PerfilTransportadorFormularioPermissaoPersonalizada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada> BuscarPorPerfil(int perfil)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada>();
            var result = from obj in query
                         where obj.PerfilTransportadorFormulario.PerfilAcessoTransportador.Codigo == perfil
                         select obj;
            return result.ToList();
        }
    }
}
