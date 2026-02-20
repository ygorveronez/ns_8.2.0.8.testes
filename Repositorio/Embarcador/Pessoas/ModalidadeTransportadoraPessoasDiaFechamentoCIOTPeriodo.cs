using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo>
    {
        public ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo> BuscarPorModalidadeTransportador(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo>();
            var result = from obj in query where obj.ModalidadeTransportadoraPessoas.Codigo == codigo select obj;
            return result.ToList();
        }
    }
}
