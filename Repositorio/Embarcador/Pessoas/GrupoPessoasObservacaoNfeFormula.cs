using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoasObservacaoNfeFormula : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula>
    {
        public GrupoPessoasObservacaoNfeFormula(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula> BuscarPorGrupoPessoas(int codigoGrupoPessoas)
        {
            var consultaObservacaoNfeFormula = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula>()
                .Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return consultaObservacaoNfeFormula.ToList();
        }
    }
}
