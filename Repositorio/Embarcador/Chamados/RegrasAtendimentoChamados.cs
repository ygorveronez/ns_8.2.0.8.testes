using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Chamados
{
    public class RegrasAtendimentoChamados : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>
    {
        public RegrasAtendimentoChamados(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados> _ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>();
            var result = from obj in query select obj;

            if (dataInicio.HasValue && dataFim.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value && o.Vigencia < dataFim.Value.AddDays(1));
            else if (dataInicio.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value);
            else if (dataFim.HasValue)
                result = result.Where(o => o.Vigencia < dataFim.Value.AddDays(1));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados> ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, descricao);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsultaRegras(DateTime? dataInicio, DateTime? dataFim, string descricao)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, descricao);

            return result.Count();
        }

        public bool BuscarSeExisteRegra()
        {
            var regrasAtendimentoChamados = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>();

            return regrasAtendimentoChamados.Any();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados> BuscarRegraPorFilial(int codigoFilial, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoFilial>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.Filial.Codigo == codigoFilial) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.Filial.Codigo != codigoFilial)
                         select obj.RegrasAtendimentoChamados;

            result = result.Where(o => o.RegraPorFilial == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>> BuscarRegraPorFilialAsync(int codigoFilial, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoFilial>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.Filial.Codigo == codigoFilial) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.Filial.Codigo != codigoFilial)
                         select obj.RegrasAtendimentoChamados;

            result = result.Where(o => o.RegraPorFilial == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToListAsync(CancellationToken);
        }


        public List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados> BuscarRegraPorTipoOperacao(int codigoTipoOperacao, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTipoOperacao>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.TipoOperacao.Codigo == codigoTipoOperacao) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.TipoOperacao.Codigo != codigoTipoOperacao)
                         select obj.RegrasAtendimentoChamados;

            result = result.Where(o => o.RegraPorTipoOperacao == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>> BuscarRegraPorTipoOperacaoAsync(int codigoTipoOperacao, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTipoOperacao>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.TipoOperacao.Codigo == codigoTipoOperacao) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.TipoOperacao.Codigo != codigoTipoOperacao)
                         select obj.RegrasAtendimentoChamados;

            result = result.Where(o => o.RegraPorTipoOperacao == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados> BuscarRegraPorTransportador(int codigoTransportador, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTransportador>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.Empresa.Codigo == codigoTransportador) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.Empresa.Codigo != codigoTransportador)
                         select obj.RegrasAtendimentoChamados;

            result = result.Where(o => o.RegraPorTransportador == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>> BuscarRegraPorTransportadorAsync(int codigoTransportador, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTransportador>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.Empresa.Codigo == codigoTransportador) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.Empresa.Codigo != codigoTransportador)
                         select obj.RegrasAtendimentoChamados;

            result = result.Where(o => o.RegraPorTransportador == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>> BuscarRegraPorEstadoAsync(string siglaEstado, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoEstado>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.Estado.Sigla == siglaEstado) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.Estado.Sigla != siglaEstado)
                         select obj.RegrasAtendimentoChamados;

            result = result.Where(o => o.RegraPorEstado == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados> BuscarRegraPorCanalVenda(int codigoCanalVenda, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoCanalVenda>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.CanalVenda.Codigo == codigoCanalVenda) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.CanalVenda.Codigo != codigoCanalVenda)
                         select obj.RegrasAtendimentoChamados;

            result = result.Where(o => o.RegraPorCanalVenda == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>> BuscarRegraPorCanalVendaAsync(int codigoCanalVenda, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoCanalVenda>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.CanalVenda.Codigo == codigoCanalVenda) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.CanalVenda.Codigo != codigoCanalVenda)
                         select obj.RegrasAtendimentoChamados;

            result = result.Where(o => o.RegraPorCanalVenda == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToListAsync(CancellationToken);
        }

        #endregion
    }
}

