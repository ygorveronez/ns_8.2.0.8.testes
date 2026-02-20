using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoEventoStatusViagem : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoStatusViagem>
    {
        public MonitoramentoEventoStatusViagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoStatusViagem BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoStatusViagem>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoStatusViagem BuscarPorEventoStatus(int codigoMonitoramentoEvento, int codigoMonitoramentoStatusViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoStatusViagem>();
            var result = from obj in query
                         where obj.MonitoramentoEvento.Codigo == codigoMonitoramentoEvento && obj.MonitoramentoStatusViagem.Codigo == codigoMonitoramentoStatusViagem
                         select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoStatusViagem> BuscarPorEvento(int codigoMonitoramentoEvento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoStatusViagem>();
            var result = from obj in query
                         where obj.MonitoramentoEvento.Codigo == codigoMonitoramentoEvento
                         select obj;
            return result.ToList();
        }

    }
}
