using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class PessoaFaturaVencimento : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.PessoaFaturaVencimento>
    {
        public PessoaFaturaVencimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.PessoaFaturaVencimento> BuscarPorCliente(double codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaFaturaVencimento>();

            var result = from obj in query where obj.Cliente.CPF_CNPJ == codigo select obj;

            return result.ToList();
        }
    }
}
