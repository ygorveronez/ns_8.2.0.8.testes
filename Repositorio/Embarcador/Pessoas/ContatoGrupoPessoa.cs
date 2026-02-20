using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class ContatoGrupoPessoa : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoa>
    {
        public ContatoGrupoPessoa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoa> Consultar(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaContatoGrupoPessoa filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(filtrosPesquisa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaContatoGrupoPessoa filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoa> Consultar(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaContatoGrupoPessoa filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoa>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
                result = result.Where(o => o.GrupoPessoa.Codigo == filtrosPesquisa.CodigoGrupoPessoa);

            return result;
        }

        #endregion
    }
}
