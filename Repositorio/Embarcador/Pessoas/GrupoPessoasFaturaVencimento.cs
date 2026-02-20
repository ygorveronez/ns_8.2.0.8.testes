using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoasFaturaVencimento : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaVencimento>
    {
        public GrupoPessoasFaturaVencimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaVencimento> BuscarPorGrupoPessoas(int codigoGrupoPessoas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaVencimento>();

            var result = from obj in query where obj.GrupoPessoas.Codigo == codigoGrupoPessoas select obj;

            return result.ToList();
        }
    }
}
