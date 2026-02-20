using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class PessoaDataFixaVencimento : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.PessoaDataFixaVencimento>
    {
        public PessoaDataFixaVencimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pessoas.PessoaDataFixaVencimento BuscarPorCodigo(int codigo)
        {
            var pessoaDataFixaVencimento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaDataFixaVencimento>()
                .Where(a => a.Codigo == codigo)
                .FirstOrDefault();

            return pessoaDataFixaVencimento;
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.PessoaDataFixaVencimento> BuscarPorPessoa(double cnpjCpf)
        {
            var pessoaDataFixaVencimentos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaDataFixaVencimento>()
                .Where(a => a.Pessoa.CPF_CNPJ == cnpjCpf)
                .ToList();

            return pessoaDataFixaVencimentos;
        }

        #endregion
    }
}
