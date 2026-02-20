using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoEventoTipoDeOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeOperacao>
    {
        public MonitoramentoEventoTipoDeOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeOperacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeOperacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeOperacao BuscarPorEventoStatus(int codigoMonitoramentoEvento, int codigoTipoDeOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeOperacao>();
            var result = from obj in query
                         where obj.MonitoramentoEvento.Codigo == codigoMonitoramentoEvento && obj.TipoDeOperacao.Codigo == codigoTipoDeOperacao
                         select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeOperacao> BuscarPorEvento(int codigoMonitoramentoEvento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeOperacao>();
            var result = from obj in query
                         where obj.MonitoramentoEvento.Codigo == codigoMonitoramentoEvento
                         select obj;
            return result.ToList();
        }

    }
}
