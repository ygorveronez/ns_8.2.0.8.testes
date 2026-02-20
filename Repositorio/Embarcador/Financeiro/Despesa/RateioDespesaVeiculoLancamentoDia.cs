using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro.Despesa
{
    public class RateioDespesaVeiculoLancamentoDia : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia>
    {
        public RateioDespesaVeiculoLancamentoDia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia> Consultar(long codigoLancamentoRateioDespesaVeiculo,  string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia>();

            query = query.Where(o => o.Lancamento.Codigo == codigoLancamentoRateioDespesaVeiculo);

            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar))
                query = query.OrderBy(propriedadeOrdenar + " " + dirOrdena);

            if (limite > 0 || inicio > 0)
                query = query.Skip(inicio).Take(limite);

            return query.Fetch(o => o.Lancamento).ThenFetch(o => o.Veiculo).ToList();
        }

        public int ContarConsulta(long codigoLancamentoRateioDespesaVeiculo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia>();

            query = query.Where(o => o.Lancamento.Codigo == codigoLancamentoRateioDespesaVeiculo);

            return query.Count();
        }

        public decimal ObterTotalPorLancamento(long codigoLancamentoRateioDespesaVeiculo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia>();

            query = query.Where(o => o.Lancamento.Codigo == codigoLancamentoRateioDespesaVeiculo);

            return query.Sum(o => o.Valor);
        }
    }
}
