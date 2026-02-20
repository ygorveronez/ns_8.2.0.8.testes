using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class RegraControleReajusteFretePlanilha : RepositorioBase<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha>
    {
        public RegraControleReajusteFretePlanilha(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }


        private IQueryable<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha> _ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha>();
            var result = from obj in query select obj;

            if (dataInicio.HasValue && dataFim.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value && o.Vigencia < dataFim.Value.AddDays(1));
            else if (dataInicio.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value);
            else if (dataFim.HasValue)
                result = result.Where(o => o.Vigencia < dataFim.Value.AddDays(1));

            if (aprovador != null)
                result = result.Where(o => o.Aprovadores.Contains(aprovador));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha> ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, aprovador, descricao);

            if (inicioRegistros > 0 && maximoRegistros > inicioRegistros)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result
                    .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                    .ToList();
        }

        public int ContarConsultaRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, aprovador, descricao);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha> AlcadasPorTipoOperacao(int tipoOperacao, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && obj.TipoOperacao.Codigo == tipoOperacao) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && obj.TipoOperacao.Codigo != tipoOperacao)
                         select obj.RegraControleReajusteFretePlanilha;

            result = result.Where(o => o.RegraPorTipoOperacao == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha> AlcadasPorFilial(int filial, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteFilial>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && obj.Filial.Codigo == filial) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && obj.Filial.Codigo != filial)
                         select obj.RegraControleReajusteFretePlanilha;

            result = result.Where(o => o.RegraPorFilial == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }
    }
}
