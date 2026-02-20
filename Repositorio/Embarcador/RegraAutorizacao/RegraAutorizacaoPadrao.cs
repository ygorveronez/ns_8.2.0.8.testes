using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.RegraAutorizacao
{
    /// <summary>
    /// A classe fornece uma implementação padrão para as regras que não utilizam etapas.
    /// As regras que utilizam etapas devem ter uma implementação própria, podendo também estender a classe RegraAutorizacao.
    /// </summary>
    public class RegraAutorizacaoPadrao<TRegra> : RegraAutorizacao<TRegra> where TRegra : Dominio.Entidades.Embarcador.RegraAutorizacao.RegraAutorizacao
    {
        #region Construtores

        public RegraAutorizacaoPadrao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<TRegra> ConsultarRegras(Dominio.ObjetosDeValor.Embarcador.RegraAutorizacao.FiltroPesquisaRegraAutorizacaoPadrao filtrosPesquisa)
        {
            var listaRegras = this.SessionNHiBernate.Query<TRegra>();
            var listaRegrasRetornar = from obj in listaRegras select obj;

            if (filtrosPesquisa.DataInicial.HasValue)
                listaRegrasRetornar = listaRegrasRetornar.Where(o => o.Vigencia >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataLimite.HasValue)
                listaRegrasRetornar = listaRegrasRetornar.Where(o => o.Vigencia <= filtrosPesquisa.DataLimite.Value.Add(System.DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Aprovador != null)
                listaRegrasRetornar = listaRegrasRetornar.Where(o => o.Aprovadores.Contains(filtrosPesquisa.Aprovador));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                listaRegrasRetornar = listaRegrasRetornar.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                var ativo = filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                listaRegrasRetornar = listaRegrasRetornar.Where(o => o.Ativo == ativo);
            }

            return listaRegrasRetornar;
        }

        #endregion

        #region Métodos Públicos

        public List<TRegra> BuscarPorAtiva()
        {
            System.DateTime dataAtual = System.DateTime.Today;

            var listaRegrasAutorizacao = this.SessionNHiBernate.Query<TRegra>()
                .Where(regra => regra.Ativo && (regra.Vigencia == null || regra.Vigencia >= dataAtual))
                .ToList();

            listaRegrasAutorizacao = listaRegrasAutorizacao.Where(regra => regra.IsAlcadaAtiva()).ToList();

            return listaRegrasAutorizacao;
        }

        public List<TRegra> Consultar(Dominio.ObjetosDeValor.Embarcador.RegraAutorizacao.FiltroPesquisaRegraAutorizacaoPadrao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var listaRegrasAutorizacao = ConsultarRegras(filtrosPesquisa);

            return ObterLista(listaRegrasAutorizacao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.RegraAutorizacao.FiltroPesquisaRegraAutorizacaoPadrao filtrosPesquisa)
        {
            var listaRegrasAutorizacao = ConsultarRegras(filtrosPesquisa);

            return listaRegrasAutorizacao.Count();
        }

        #endregion
    }
}
