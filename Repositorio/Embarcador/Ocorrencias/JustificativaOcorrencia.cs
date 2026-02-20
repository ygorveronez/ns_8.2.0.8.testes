using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class JustificativaOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.JustificativaOcorrencia>
    {
        #region Construtores

        public JustificativaOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Ocorrencias.JustificativaOcorrencia> Consultar(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaJustificativaOcorrencia filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaJustificativaOcorrencia filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.JustificativaOcorrencia> Consultar(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaJustificativaOcorrencia filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.JustificativaOcorrencia>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Ativo);

            return result;
        }

        #endregion
    }
}