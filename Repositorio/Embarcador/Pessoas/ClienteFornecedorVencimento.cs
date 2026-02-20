using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class ClienteFornecedorVencimento : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento>
    {
        public ClienteFornecedorVencimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento> BuscarPorPessoa(double cnpjCpfPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento>();
            query = query.Where(o => o.Cliente.CPF_CNPJ == cnpjCpfPessoa);
            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento BuscarDiaVencimento(double cnpjFornecedor, int diaEmissao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento>();
            query = query.Where(o => o.Cliente.CPF_CNPJ == cnpjFornecedor && o.DiaEmissaoInicial <= diaEmissao && o.DiaEmissaoFinal >= diaEmissao);
            return query.FirstOrDefault();
        }

        #endregion
    }
}
