using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Usuarios
{
    public class FuncionarioMetaVendaDireta : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaVendaDireta>
    {
        #region Construtores
        public FuncionarioMetaVendaDireta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaVendaDireta> BuscarPorUsuario(int codigoUsuario)
        {
            var consultaEPIs = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaVendaDireta>()
                .Where(o => o.Funcionario.Codigo == codigoUsuario);

            return consultaEPIs.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaVendaDireta> BuscarPorUsuario(Dominio.Entidades.Usuario usuario)
        {
            var consultaEPIs = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaVendaDireta>()
                .Where(o => o.Funcionario == usuario);

            return consultaEPIs.ToList();
        }

        #endregion

    }
}
