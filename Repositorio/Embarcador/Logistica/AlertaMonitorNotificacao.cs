using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class AlertaMonitorNotificacao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.AlertaMonitorNotificacao>
    {
        public AlertaMonitorNotificacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.AlertaMonitorNotificacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitorNotificacao>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitorNotificacao> BuscarTodos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitorNotificacao>();

            var result = from obj in query select obj;

            return result.ToList();

        }

        public Dominio.Entidades.Embarcador.Logistica.AlertaMonitorNotificacao BuscarUltimoPorAlerta(int codigoAlerta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitorNotificacao>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.AlertaMonitor.Codigo == codigoAlerta);

            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

    }

}
