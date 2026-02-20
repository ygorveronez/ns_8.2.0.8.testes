using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio
{
    public class Menu : RepositorioBase<Dominio.Entidades.Menu>, Dominio.Interfaces.Repositorios.Menu
    {
        public Menu(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Menu> BuscarSubMenus(int codigoMenu)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Menu>();

            var result = from obj in query where obj.MenuPai.Codigo == codigoMenu select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Menu> ConsultarMenus(int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Menu>();

            var result = from obj in query select obj;

            return result
                        .Skip(inicioRegistros)
                        .Take(maximoRegistros)
                        .Timeout(120)
                        .ToList();
        }

        public int ContarConsultaMenus()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Menu>();

            var result = from obj in query select obj;

            return result.Count();
        }

        public Dominio.Entidades.Menu BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Menu>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Menu BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Menu>();

            var result = from obj in query where obj.Descricao.Contains(descricao) select obj;

            return result.FirstOrDefault();
        }

    }
}
