using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoEventoTratativa : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativa>
    {
        public MonitoramentoEventoTratativa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativa>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();

        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativa BuscarTodosPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta TipoAlerta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativa>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.MonitoramentoEvento.TipoAlerta == TipoAlerta);

            return result.FirstOrDefault();

        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativa> BuscarTodosAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativa>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.EnvioEmail == true || ent.EnvioEmailTransportador == true || ent.EnvioEmailCliente == true);

            return result.ToList();

        }

    }

}
