using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{

    public class AlertaTratativa : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.AlertaTratativa>
    {
        public AlertaTratativa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.AlertaTratativa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaTratativa>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.AlertaTratativa BuscarPorAlerta(int codigoAlerta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaTratativa>();
            var result = from obj in query select obj;
            result = result.Where(ent => ent.AlertaMonitor.Codigo == codigoAlerta);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaTratativa> BuscarPorCargasETiposAlertas(List<int> cargas, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaTratativa>();
            query = query.Where(ent => cargas.Contains(ent.AlertaMonitor.Carga.Codigo) && tiposAlerta.Contains(ent.AlertaMonitor.TipoAlerta));
            return query
                .Fetch(obj => obj.AlertaTratativaAcao)
                .Fetch(obj => obj.AlertaMonitor)
                .ToList();
        }


        public List<Dominio.Entidades.Embarcador.Logistica.AlertaTratativa> BuscarTodos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaTratativa>();

            var result = from obj in query select obj;

            return result.ToList();

        }


    }

}
