using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro.Despesa
{
    public class RateioDespesaVeiculoValorCentroResultado : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorCentroResultado>
    {
        public RateioDespesaVeiculoValorCentroResultado(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public List<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorCentroResultado> BuscarCentroResultadoPorDespesa(Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo depesaVeiculo)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorCentroResultado>();

            query = query.Where(d => d.DespesaVeiculo.Codigo == depesaVeiculo.Codigo);
           
            return query.ToList();

        }
    }
}
