using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro.Despesa
{
    public class TipoDespesaFinanceira : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira>
    {
        public TipoDespesaFinanceira(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira> BuscarPorCodigos(List<long> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira> Consultar(string descricao, long codigoGrupoDespesa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            if (codigoGrupoDespesa > 0)
                query = query.Where(o => o.GrupoDespesa.Codigo == codigoGrupoDespesa);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(string descricao, long codigoGrupoDespesa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int codigoEmpresa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            if (codigoGrupoDespesa > 0)
                query = query.Where(o => o.GrupoDespesa.Codigo == codigoGrupoDespesa);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return query.Count();
        }
    }
}
