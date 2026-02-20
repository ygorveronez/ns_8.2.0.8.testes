using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoasModeloVeicularEmbarcador : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador>
    {
        public GrupoPessoasModeloVeicularEmbarcador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador> BuscarPorGrupoPessoas(int codigoGrupoPessoas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador>();

            query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.OrderBy(o => o.DescricaoModeloVeicularEmbarcador).ToList();
        }
    }
}
