using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Transportadores
{
    public class PerfilTransportadorFormulario : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario>
    {
        public PerfilTransportadorFormulario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario> BuscarPorPerfil(int perfil)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario>();
            var result = from obj in query where obj.PerfilAcessoTransportador.Codigo == perfil select obj;

            return result.ToList();
        }
    }
}
