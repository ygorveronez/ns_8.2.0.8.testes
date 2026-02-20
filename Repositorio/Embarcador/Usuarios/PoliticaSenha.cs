using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Usuarios
{
    public class PoliticaSenha : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha>
    {
        public PoliticaSenha(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha BuscarPoliticaPadrao()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha>();
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha BuscarPoliticaPadraoNull()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha>();
            query = query.Where(c => c.TipoServico == null);
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha BuscarPoliticaPadraoPorServicoMultiSoftware(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha>();
            var result = from obj in query where obj.TipoServico == tipoServicoMultisoftware select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha> BuscarListaPoliticasPadrao()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha>();
            var result = from obj in query select obj;
            return result.ToList();
        }

    }
}
