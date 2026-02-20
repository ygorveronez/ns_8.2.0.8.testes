using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class RegraContratoFreteTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador>
    {
        public RegraContratoFreteTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador> BuscarParaExcluir(List<int> codigosAManter)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador>();
            var result = from obj in query where !codigosAManter.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador> _ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador>();
            var result = from obj in query select obj;

            if (dataInicio.HasValue && dataFim.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value && o.Vigencia < dataFim.Value.AddDays(1));
            else if (dataInicio.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value);
            else if (dataFim.HasValue)
                result = result.Where(o => o.Vigencia < dataFim.Value.AddDays(1));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Ativo);

            if (aprovador != null)
                result = result.Where(o => o.Aprovadores.Contains(aprovador));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador> ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, aprovador, descricao, ativo);

            if (inicioRegistros > 0 && maximoRegistros > inicioRegistros)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result
                    .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                    .ToList();
        }

        public int ContarConsultaRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, aprovador, descricao, ativo);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador> AlcadasPorTransportador(int transportador, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AlcadaTransportador>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && obj.Transportador.Codigo == transportador) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && obj.Transportador.Codigo != transportador)
                         select obj.RegraContratoFreteTransportador;

            result = result.Where(o => o.RegraPorTransportador == true && o.Ativo && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador> AlcadasPorFilial(List<int> filiais, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AlcadaFilial>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && filiais.Contains(obj.Filial.Codigo)) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && !filiais.Contains(obj.Filial.Codigo))
                         select obj.RegraContratoFreteTransportador;

            result = result.Where(o => o.RegraPorFilial == true && o.Ativo && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador> AlcadasPorValor(DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegrasContratoValorContrato>();

            var result = from obj in query select obj.RegraContratoFreteTransportador;
            result = result.Where(o => o.RegraPorValorContrato == true && o.Ativo && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }
    }
}
