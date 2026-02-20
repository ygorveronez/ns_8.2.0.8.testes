using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class TermoQuitacaoControleDocumentoAssinado : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumento>
    {
        public TermoQuitacaoControleDocumentoAssinado(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumentoAssinado BuscarAnexoPorEntidade(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumentoAssinado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumentoAssinado>();

            query = from obj in query select obj;

            if (codigo > 0)
                query = query.Where(o => (o.EntidadeAnexo != null ? o.EntidadeAnexo.Codigo : 0) == codigo);

            return query.FirstOrDefault();
        }
    }
}
