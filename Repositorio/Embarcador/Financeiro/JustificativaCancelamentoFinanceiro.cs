using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class JustificativaCancelamentoFinanceiro : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro>
    {
        public JustificativaCancelamentoFinanceiro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro>();
            var result = from obj in query where obj.Descricao == descricao select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query.OrderBy(propriedadeOrdenar + " " + dirOrdena).ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query.Count();
        }



    }
}
