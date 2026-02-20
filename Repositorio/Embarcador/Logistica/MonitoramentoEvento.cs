using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class MonitoramentoEvento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento>
    {
        #region Construtores

        public MonitoramentoEvento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoEvento filtrosPesquisa)
        {
            var consultaMonitoramentoEvento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaMonitoramentoEvento = consultaMonitoramentoEvento.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Tipo.HasValue)
                consultaMonitoramentoEvento = consultaMonitoramentoEvento.Where(o => o.TipoMonitoramentoEvento == filtrosPesquisa.Tipo.Value);

            if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaMonitoramentoEvento = consultaMonitoramentoEvento.Where(o => o.Ativo);
            else if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaMonitoramentoEvento = consultaMonitoramentoEvento.Where(o => !o.Ativo);

            return consultaMonitoramentoEvento;
        }

        #endregion

        #region Métodos Públicos


        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> BuscarMonitoramentoControleEntrega()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento>();

            var result = from obj in query where obj.Ativo select obj;
            result = result.Where(ent => ent.ExibirControleEntrega == true);

            return result.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> BuscarTodosAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento>();

            var result = from obj in query
                         where obj.Ativo
                         select obj;

            return result.ToList();
        }

        public int BuscarTotalAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento>();

            var result = from obj in query
                         where obj.Ativo
                         select obj;

            return result.Count();
        }

        public int BuscarTotalAtivosPorTipo(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento>();

            var result = from obj in query
                         where obj.Ativo && tiposAlerta.Contains(obj.TipoAlerta)
                         select obj;

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento BuscarAtivo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento>();

            var result = from obj in query
                         where obj.Ativo && obj.TipoAlerta == tipoAlerta
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> BuscarAtivosPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento>();

            var result = from obj in query
                         where obj.Ativo && obj.TipoAlerta == tipoAlerta
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> BuscarAtivosPorTipos(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento>();

            var result = from obj in query
                         where obj.Ativo && tiposAlerta.Contains(obj.TipoAlerta)
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> BuscarAtivosSemSinalTempoReferenteaDataCarregamentoCarga()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento>();

            var result = from obj in query
                         where obj.Ativo &&
                               obj.TipoAlerta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemSinal &&
                               obj.Gatilhos.Any(gat => gat.TempoReferenteaDataCarregamentoCarga)
                         select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento BuscarAtivo(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento>();
            var result = from obj in query where obj.Ativo && obj.Descricao == descricao.Trim() select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoEvento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMonitoramentoEvento = Consultar(filtrosPesquisa);

            return ObterLista(consultaMonitoramentoEvento, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoEvento filtrosPesquisa)
        {
            var consultaMonitoramentoEvento = Consultar(filtrosPesquisa);

            return consultaMonitoramentoEvento.Count();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> BuscarTodosMobile()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento>();

            var result = from obj in query where obj.Ativo && obj.ExibirApp select obj;


            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> BuscarEventosComTipoTratativaAutomatica(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TratativaAutomaticaMonitoramentoEvento tipoTratativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento>();
            var result = from obj in query where obj.Ativo select obj;

            return result
                .Fetch(o => o.TratativaAutomatica)
                .Where(o => o.Ativo && o.TratativaAutomatica.Any(x => x.Gatilho == tipoTratativa))
                .ToList();
        }
        #endregion
    }
}
