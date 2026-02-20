using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoasLayoutEDI : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>
    {
        public GrupoPessoasLayoutEDI(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> BuscarPorGrupoPessoas(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();

            var result = from obj in query where obj.GrupoPessoas.Codigo == codigo select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> BuscarDisponiveisParaLeituraFTP()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();

            query = query.Where(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP && o.UtilizarLeituraArquivos && o.GrupoPessoas.Ativo);            

            return query.ToList();
        }
    }
}
