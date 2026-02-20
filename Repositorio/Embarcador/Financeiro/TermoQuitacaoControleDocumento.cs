using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class TermoQuitacaoControleDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumento>
    {
        public TermoQuitacaoControleDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumento BuscarAnexoPorEntidade(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumento>();

            query = from obj in query select obj;

            if (codigo > 0)
                query = query.Where(o => (o.EntidadeAnexo != null ? o.EntidadeAnexo.Codigo : 0) == codigo);

            return query.FirstOrDefault();
        }
    }
}
