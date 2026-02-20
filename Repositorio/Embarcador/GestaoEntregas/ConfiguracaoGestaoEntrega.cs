using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoEntregas
{
    public class ConfiguracaoGestaoEntrega : RepositorioBase<Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoGestaoEntrega>
    {
        public ConfiguracaoGestaoEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoGestaoEntrega BuscarConfiguracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoGestaoEntrega>();
            var result = from obj in query select obj;
            return result.FirstOrDefault();
        }
    }
}
