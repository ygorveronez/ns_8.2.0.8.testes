using System.Collections.Generic;
using System.Linq;


namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoasConfiguracaoComponentesFrete : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete>
    {
        public GrupoPessoasConfiguracaoComponentesFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete> BuscarPorGrupoPessoas(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete>();

            var result = from obj in query where obj.GrupoPessoas.Codigo == codigo select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete BuscarPorOutraDescricaoCTe(int codigo, string outraDescricaoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete>();

            var result = from obj in query where obj.GrupoPessoas.Codigo == codigo && obj.OutraDescricaoCTe == outraDescricaoCTe select obj;

            return result.FirstOrDefault();
        }
    }
}
