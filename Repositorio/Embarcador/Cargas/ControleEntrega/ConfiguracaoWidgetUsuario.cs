using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class ConfiguracaoWidgetUsuario : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario>
    {
        public ConfiguracaoWidgetUsuario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario BuscarPorUsuario(int usuario)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario>();
            var result = query.Where(obj => obj.Usuario.Codigo == usuario);
            return result.FirstOrDefault();
        }
    }
}
