using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Chamados
{
    public class RegrasAnaliseChamados : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados>
    {
        public RegrasAnaliseChamados(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public RegrasAnaliseChamados(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados> _ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados>();
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

        public Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados> ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
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

        public List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados> BuscarRegraPorMotivoChamada(int codigoMotivoChamado, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasMotivoChamado>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.MotivoChamado.Codigo == codigoMotivoChamado) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.MotivoChamado.Codigo != codigoMotivoChamado)
                         select obj.RegrasAnaliseChamados;

            result = result.Where(o => o.RegraPorMotivoChamado == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados>> BuscarRegraPorMotivoChamadaAsync(int codigoMotivoChamado, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasMotivoChamado>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.MotivoChamado.Codigo == codigoMotivoChamado) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.MotivoChamado.Codigo != codigoMotivoChamado)
                         select obj.RegrasAnaliseChamados;

            result = result.Where(o => o.RegraPorMotivoChamado == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados> BuscarRegraPorFilial(int codigoFilial, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosFilial>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.Filial.Codigo == codigoFilial) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.Filial.Codigo != codigoFilial)
                         select obj.RegrasAnaliseChamados;

            result = result.Where(o => o.RegraPorFilial == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados>> BuscarRegraPorFilialAsync(int codigoFilial, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosFilial>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.Filial.Codigo == codigoFilial) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.Filial.Codigo != codigoFilial)
                         select obj.RegrasAnaliseChamados;

            result = result.Where(o => o.RegraPorFilial == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados> BuscarRegraPorRegiaoDestino(List<int> codigosRegiaoDestino, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosRegiaoDestino>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && codigosRegiaoDestino.Contains(obj.Regiao.Codigo)) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && !codigosRegiaoDestino.Contains(obj.Regiao.Codigo))
                         select obj.RegrasAnaliseChamados;

            result = result.Where(o => o.RegraPorRegiaoDestino && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados>> BuscarRegraPorRegiaoDestinoAsync(List<int> codigosRegiaoDestino, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosRegiaoDestino>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && codigosRegiaoDestino.Contains(obj.Regiao.Codigo)) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && !codigosRegiaoDestino.Contains(obj.Regiao.Codigo))
                         select obj.RegrasAnaliseChamados;

            result = result.Where(o => o.RegraPorRegiaoDestino && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToListAsync(CancellationToken);
        }

        #endregion
    }
}

