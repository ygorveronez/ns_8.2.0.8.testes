using System.Linq;

namespace Repositorio.Embarcador.CotacaoPedido
{
    public class SolicitacaoCotacao : RepositorioBase<Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacao>
    {
        public SolicitacaoCotacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacao BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
