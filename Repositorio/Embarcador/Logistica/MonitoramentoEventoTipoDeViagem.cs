using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoEventoTipoDeCarga : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeCarga>
    {
        public MonitoramentoEventoTipoDeCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeCarga BuscarPorEventoStatus(int codigoMonitoramentoEvento, int codigoTipoDeCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeCarga>();
            var result = from obj in query
                         where obj.MonitoramentoEvento.Codigo == codigoMonitoramentoEvento && obj.TipoDeCarga.Codigo == codigoTipoDeCarga
                         select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeCarga> BuscarPorEvento(int codigoMonitoramentoEvento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeCarga>();
            var result = from obj in query
                         where obj.MonitoramentoEvento.Codigo == codigoMonitoramentoEvento
                         select obj;
            return result.ToList();
        }

    }
}
