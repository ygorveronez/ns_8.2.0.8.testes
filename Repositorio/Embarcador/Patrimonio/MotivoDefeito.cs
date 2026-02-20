using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Patrimonio
{
    public class MotivoDefeito : RepositorioBase<Dominio.Entidades.Embarcador.Patrimonio.MotivoDefeito>
    {
        public MotivoDefeito(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Patrimonio.MotivoDefeito> Consultar(Dominio.ObjetosDeValor.Embarcador.Patrimonio.FiltroPesquisaMotivoDefeito filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Patrimonio.FiltroPesquisaMotivoDefeito filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Patrimonio.MotivoDefeito> Consultar(Dominio.ObjetosDeValor.Embarcador.Patrimonio.FiltroPesquisaMotivoDefeito filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.MotivoDefeito>();

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

