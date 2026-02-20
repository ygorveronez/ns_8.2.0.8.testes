using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoasLeituraDinamicaXml : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml>
    {
        public GrupoPessoasLeituraDinamicaXml(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml> BuscarPorGrupoPessoas(int codigoGrupoPessoas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml>();

            query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.OrderBy(o => o.Descricao).ToList();
        }
    }
}
