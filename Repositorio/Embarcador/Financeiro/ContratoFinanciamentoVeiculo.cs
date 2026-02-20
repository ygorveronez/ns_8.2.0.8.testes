using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class ContratoFinanciamentoVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoVeiculo>
    {
        public ContratoFinanciamentoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoVeiculo BuscarPorVeiculoEContrato(int codigoVeiculo, int codigoContratoFinanciamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoVeiculo>();
            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo && obj.ContratoFinanciamento.Codigo == codigoContratoFinanciamento select obj;
            return result.FirstOrDefault();
        }
    }
}
