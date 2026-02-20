using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro.DespesaMensal
{
    public class DespesaMensalProcessamentoDespesas : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas>
    {
        public DespesaMensalProcessamentoDespesas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas> BuscarPorDespesaMensalProcessamento(int codigoDespesaMensalProcessamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas>();
            var result = from obj in query where obj.DespesaMensalProcessamento.Codigo == codigoDespesaMensalProcessamento select obj;
            return result.ToList();
        }
    }
}
