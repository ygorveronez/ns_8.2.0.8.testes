using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoasTipoCargaEmbarcador : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador>
    {
        public GrupoPessoasTipoCargaEmbarcador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador> BuscarPorGrupoPessoas(int codigoGrupoPessoas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador>();

            query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.OrderBy(o => o.DescricaoTipoCargaEmbarcador).ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador BuscarPorGrupoPessoasECodigoTipoCargaEmbarcador(int codigoGrupoPessoas, string codigoTipoCargaEmbarcador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador>();

            query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas && o.CodigoTipoCargaEmbarcador == codigoTipoCargaEmbarcador);

            return query.FirstOrDefault();
        }
    }
}
