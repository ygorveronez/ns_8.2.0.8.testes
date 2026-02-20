using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class ClienteObservacaoCTe : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ClienteObservacaoCTe>
    {
        public ClienteObservacaoCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteObservacaoCTe> BuscarPorPessoa(double cpfCnpjPessoa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteObservacaoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteObservacaoCTe>();

            query = query.Where(o => o.Cliente.CPF_CNPJ == cpfCnpjPessoa);

            return query.ToList();
        }
    }
}
