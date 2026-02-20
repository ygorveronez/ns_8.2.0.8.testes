using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class PessoaAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.PessoaAnexo>
    {
        public PessoaAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pessoas.PessoaAnexo BuscarPorCodigo(int codigo)
        {
            var anexo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaAnexo>()
                .Where(a => a.Codigo == codigo)
                .FirstOrDefault();

            return anexo;
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.PessoaAnexo> BuscarPorPessoa(double cnpjCpf)
        {
            var anexos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaAnexo>()
                .Where(a => a.Pessoa.CPF_CNPJ == cnpjCpf)
                .ToList();

            return anexos;
        }

        #endregion
    }
}
