using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoEventoGatilho : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho>
    {
        public MonitoramentoEventoGatilho(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();

        }

        public new List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho> BuscarTodos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho>();

            var result = from obj in query select obj;

            return result.ToList();

        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento tipoMonitoramentoEvento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho>();

            var result = from obj in query
                         where obj.MonitoramentoEvento.TipoMonitoramentoEvento == tipoMonitoramentoEvento
                         select obj;

            return result.FirstOrDefault();
        }

    }
}
