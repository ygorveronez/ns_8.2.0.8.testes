using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class PessoaExteriorOutraDescricao : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.PessoaExteriorOutraDescricao>
    {
        public PessoaExteriorOutraDescricao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.PessoaExteriorOutraDescricao> BuscarPorPessoa(double cpfCnpjPessoa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.PessoaExteriorOutraDescricao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaExteriorOutraDescricao>();

            query = query.Where(o => o.Pessoa.CPF_CNPJ == cpfCnpjPessoa);

            return query.ToList();
        }

        public Dominio.Entidades.Cliente BuscarPessoaPorRazaoSocialEEndereco(string razaoSocial, string endereco)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.PessoaExteriorOutraDescricao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaExteriorOutraDescricao>();

            query = query.Where(o => o.RazaoSocial.ToLower().Equals(razaoSocial.ToLower()) && o.Endereco.ToLower().Equals(endereco.ToLower()) && o.Pessoa.Ativo && o.Pessoa.Tipo == "E");

            return query.Select(o => o.Pessoa).FirstOrDefault();
        }
    }
}
