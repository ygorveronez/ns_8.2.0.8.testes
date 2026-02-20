using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Simulacoes
{
    public class RegrasAutorizacaoSimulacao : RepositorioBase<Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao>
    {
        #region Construtores

        public RegrasAutorizacaoSimulacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao> Consultar(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao>();
            var result = from obj in query select obj;

            if (dataInicio.HasValue && dataFim.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value && o.Vigencia < dataFim.Value.AddDays(1));
            else if (dataInicio.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value);
            else if (dataFim.HasValue)
                result = result.Where(o => o.Vigencia < dataFim.Value.AddDays(1));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Situacao);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Situacao);

            if (aprovador != null)
                result = result.Where(o => o.Aprovadores.Contains(aprovador));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result;
        }

        #endregion

        #region Métodos Publicos

        public Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao> BuscarRegraPorFilial(int codigoFilial, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoFilialEmissao>();
            var result = from obj in query
                         where
                            (obj.RegrasAutorizacaoSimulacao.Situacao) &&
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoSimulacao.IgualA && obj.Filial.Codigo == codigoFilial ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoSimulacao.DiferenteDe && obj.Filial.Codigo != codigoFilial)
                         select obj.RegrasAutorizacaoSimulacao;

            result = result.Where(o => o.RegraPorFilialEmissao == true && (o.Vigencia >= data || o.Vigencia == null) && o.Situacao);

            return result.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao> BuscarRegraPorTipoOperacao(int codigoTipoOperacao, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao>();
            var result = from obj in query
                         where
                            (obj.RegrasAutorizacaoSimulacao.Situacao) &&
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoSimulacao.IgualA && obj.TipoOperacao.Codigo == codigoTipoOperacao ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoSimulacao.DiferenteDe && obj.TipoOperacao.Codigo != codigoTipoOperacao)
                         select obj.RegrasAutorizacaoSimulacao;

            result = result.Where(o => o.RegraPorTipoOperacao == true && (o.Vigencia >= data || o.Vigencia == null) && o.Situacao);

            return result.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao> ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(dataInicio, dataFim, aprovador, descricao, ativo);

            if (inicioRegistros > 0 && maximoRegistros > inicioRegistros)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result
                    .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                    .ToList();
        }

        public int ContarConsultaRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var result = Consultar(dataInicio, dataFim, aprovador, descricao, ativo);

            return result.Count();
        }

        #endregion
    }
}
