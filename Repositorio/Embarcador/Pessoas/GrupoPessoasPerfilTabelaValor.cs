using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoasPerfilTabelaValor : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasPerfilTabelaValor>
    {
        public GrupoPessoasPerfilTabelaValor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasPerfilTabelaValor> BuscarPorGrupoPessoas(int codigoGrupoPessoas)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasPerfilTabelaValor>()
                .Where(o => o.GrupoPessoa.Codigo == codigoGrupoPessoas);

            return consulta.ToList();
        }
    }
}
