using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Usuarios
{
    public class FuncionarioEPI : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI>
    {
        #region Construtores
        public FuncionarioEPI(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI> BuscarPorUsuario(int codigoUsuario)
        {
            var consultaEPIs = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI>()
                .Where(o => o.Usuario.Codigo == codigoUsuario);

            return consultaEPIs.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI> BuscarPorUsuario(Dominio.Entidades.Usuario usuario)
        {
            var consultaEPIs = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI>()
                .Where(o => o.Usuario == usuario);

            return consultaEPIs.ToList();
        }

        #endregion

    }
}
