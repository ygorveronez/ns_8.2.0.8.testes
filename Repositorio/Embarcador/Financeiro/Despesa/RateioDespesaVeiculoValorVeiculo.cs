using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro.Despesa
{
    public class RateioDespesaVeiculoValorVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo>
    {
        public RateioDespesaVeiculoValorVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public List<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo> BuscarVeiculosPorDespesa(Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo depesaVeiculo)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo>();

            query = query.Where(d => d.DespesaVeiculo.Codigo == depesaVeiculo.Codigo);

            return query.ToList();

        }
    }
}
