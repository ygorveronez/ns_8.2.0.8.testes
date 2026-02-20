using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira
{
    public class ConsultaAbastecimentoAngelliraVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelLiraVeiculo>
    {
        public ConsultaAbastecimentoAngelliraVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelLiraVeiculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelLiraVeiculo>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }
    }
}
