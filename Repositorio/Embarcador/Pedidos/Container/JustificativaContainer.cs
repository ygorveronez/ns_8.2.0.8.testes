using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class JustificativaContainer : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.JustificativaContainer>
    {
        #region Construtores

        public JustificativaContainer(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores
                
        #region public

        public List<Dominio.Entidades.Embarcador.Pedidos.JustificativaContainer> Consultar(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaJustificativa filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);
            return ObterLista(result, parametroConsulta);
        }

        #endregion

        #region private 

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.JustificativaContainer> Consultar(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaJustificativa filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query < Dominio.Entidades.Embarcador.Pedidos.JustificativaContainer>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.StatusContainer != null)
                result = result.Where(o => o.StatusContainer == filtrosPesquisa.StatusContainer);

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Ativo);

            return result;
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaJustificativa filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion
    }
}
