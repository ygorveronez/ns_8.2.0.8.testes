using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Checklist
{
    public class Checklist : RepositorioBase<Dominio.Entidades.Embarcador.Checklist.Checklist>
    {
        public Checklist(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.Embarcador.Checklist.Checklist BuscarPorCodigoFecht(int codigo)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Checklist.Checklist>();

            query = query.Where(o => o.Codigo == codigo).Fetch(o => o.Perguntas);
            return query.FirstOrDefault();

        }

        public List<Dominio.Entidades.Embarcador.Checklist.Checklist> Consultar(Dominio.ObjetosDeValor.Embarcador.Checklist.FiltroPesquisaChecklist filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Checklist.FiltroPesquisaChecklist filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Checklist.Checklist> Consultar(Dominio.ObjetosDeValor.Embarcador.Checklist.FiltroPesquisaChecklist filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Checklist.Checklist>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                query = query.Where(o => o.CodigoIntegracao == filtrosPesquisa.CodigoIntegracao);

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query;
        }
    }
}
