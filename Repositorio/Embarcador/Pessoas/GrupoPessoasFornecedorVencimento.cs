using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoasFornecedorVencimento : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento>
    {
        public GrupoPessoasFornecedorVencimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento> BuscarPorGrupoPessoas(int codigoGrupoPessoas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento>();

            query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento BuscarDiaVencimento(int codigoGrupoPessoas, int diaEmissao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento>();

            query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas && o.DiaEmissaoInicial <= diaEmissao && o.DiaEmissaoFinal >= diaEmissao);

            return query.FirstOrDefault();
        }

        #endregion
    }
}
