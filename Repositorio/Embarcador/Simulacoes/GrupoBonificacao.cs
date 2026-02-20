using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Simulacoes
{
    public class GrupoBonificacao : RepositorioBase<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao>
    {
        public GrupoBonificacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Simulacoes.FiltroPesquisaGrupoBonificacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Simulacoes.FiltroPesquisaGrupoBonificacao filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao> BuscarPorCodigosIntegracao(List<string> codigosIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao>();

            var result = from obj in query where codigosIntegracao.Contains(obj.CodigoIntegracao) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao>();

            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;

            return result.FirstOrDefault();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Simulacoes.FiltroPesquisaGrupoBonificacao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao.Contains(filtrosPesquisa.CodigoIntegracao));

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Situacao);
            else if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Situacao);

            return result;
        }

        #endregion Métodos Privados
    }
}
