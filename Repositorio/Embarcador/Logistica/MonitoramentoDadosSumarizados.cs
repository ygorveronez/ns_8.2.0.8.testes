using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoDadosSumarizados : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoDadosSumarizados>
    {
        #region Atributos Públicos
        public MonitoramentoDadosSumarizados(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoDadosSumarizados> BuscarPorMonitoramento(int codigoMonitoramento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.MonitoramentoDadosSumarizados> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoDadosSumarizados>();
            query = query.Where(ent => ent.Monitoramento.Codigo == codigoMonitoramento);
            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoDadosSumarizados BuscarPorMonitoramentoeRegra(int codigoMonitoramento, int codigoRegra)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.MonitoramentoDadosSumarizados> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoDadosSumarizados>();
            query = query.Where(ent => ent.Monitoramento.Codigo == codigoMonitoramento && ent.RegraQualidadeMonitoramento.Codigo == codigoRegra);
            return query.FirstOrDefault();
        }

        #endregion
    }
}
