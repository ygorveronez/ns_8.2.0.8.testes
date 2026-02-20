using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Usuarios
{
    public class UsuarioFormularioFavorito : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.UsuarioFormularioFavorito>
    {
        #region Construtores 

        public UsuarioFormularioFavorito(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Globais

        public List<int> BuscarCodigosFormulariosPorUsuario(int codigoUsuario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Usuarios.UsuarioFormularioFavorito> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.UsuarioFormularioFavorito>();

            query = query.Where(o => o.Usuario.Codigo == codigoUsuario);

            return query.Select(o => o.CodigoFormulario).ToList();
        }

        public Dominio.Entidades.Embarcador.Usuarios.UsuarioFormularioFavorito BuscarPorUsuarioECodigoFormulario(int codigoUsuario, int codigoFormulario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Usuarios.UsuarioFormularioFavorito> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.UsuarioFormularioFavorito>();

            query = query.Where(o => o.Usuario.Codigo == codigoUsuario && o.CodigoFormulario == codigoFormulario);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.UsuarioFormularioFavorito> BuscarPorUsuario(int codigoUsuario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Usuarios.UsuarioFormularioFavorito> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.UsuarioFormularioFavorito>();

            query = query.Where(o => o.Usuario.Codigo == codigoUsuario);

            return query.ToList();
        }

        #endregion
    }
}
