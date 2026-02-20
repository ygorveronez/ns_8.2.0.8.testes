using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.TorreControle
{
    public class AlertaAcompanhamentoCarga : RepositorioBase<Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga>
    {
        public AlertaAcompanhamentoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga BuscarAlertaAbertoPorAlertaMonitoramento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.AlertaMonitor.Codigo == codigo && ent.AlertaTratado == false);

            return result.FirstOrDefault();

        }


        public Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga BuscarAlertaAbertoAlertaEventoCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.CargaEvento.Codigo == codigo && ent.AlertaTratado == false);

            return result.FirstOrDefault();

        }

        public List<Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga>();
            var result = from obj in query select obj;
            result = result.Where(obj => obj.Carga.Codigo == carga);
            return result.ToList();
        }
    }
}
