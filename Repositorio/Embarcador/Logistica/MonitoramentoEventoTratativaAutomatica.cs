using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoEventoTratativaAutomatica : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativaAutomatica>
    {
        public MonitoramentoEventoTratativaAutomatica(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativaAutomatica BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativaAutomatica>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
