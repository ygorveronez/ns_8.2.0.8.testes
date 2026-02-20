using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoEventoHorario : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoHorario>
    {
        public MonitoramentoEventoHorario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoHorario BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoHorario>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoHorario BuscarPorEvento(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoHorario>();
            var result = from obj in query
                         where obj.MonitoramentoEvento == monitoramentoEvento
                         select obj;
            return result.FirstOrDefault();
        }

    }
}
