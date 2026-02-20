using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.AlertaCarga
{
    public sealed class ConfiguracaoAlertaCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga>
    {
        #region Construtores

        public ConfiguracaoAlertaCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAlertaCarga filtrosPesquisa)
        {
            var consultaAlertaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaAlertaCarga = consultaAlertaCarga.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.TipoAlerta.HasValue)
                consultaAlertaCarga = consultaAlertaCarga.Where(o => o.TipoCargaAlerta == filtrosPesquisa.TipoAlerta.Value);

            if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaAlertaCarga = consultaAlertaCarga.Where(o => o.Ativo);
            else if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaAlertaCarga = consultaAlertaCarga.Where(o => !o.Ativo);

            return consultaAlertaCarga;
        }

        #endregion

        #region Métodos Públicos


        public List<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga> BuscarTodosAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga>();

            var result = from obj in query
                         where obj.Ativo == true
                         select obj;

            return result.ToList();
        }

        public int BuscarTotalAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga>();

            var result = from obj in query
                         where obj.Ativo == true
                         select obj;

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga BuscarAtivo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga tipoAlerta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga>();

            var result = from obj in query
                         where obj.Ativo == true && obj.TipoCargaAlerta == tipoAlerta
                         select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga BuscarAtivo(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga>();
            var result = from obj in query where obj.Ativo == true && obj.Descricao == descricao.Trim() select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAlertaCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMonitoramentoEvento = Consultar(filtrosPesquisa);

            return ObterLista(consultaMonitoramentoEvento, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAlertaCarga filtrosPesquisa)
        {
            var consultaMonitoramentoEvento = Consultar(filtrosPesquisa);

            return consultaMonitoramentoEvento.Count();
        }

        #endregion
    }
}
