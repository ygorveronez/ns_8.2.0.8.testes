using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class ContratoFinanciamentoValor : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoValor>
    {
        public ContratoFinanciamentoValor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoValor> BuscarPorContratoFinanciamento(int codigoContratoFinanciamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoValor>();
            var result = from obj in query where obj.ContratoFinanciamento.Codigo == codigoContratoFinanciamento select obj;
            return result.ToList();
        }
    }
}
