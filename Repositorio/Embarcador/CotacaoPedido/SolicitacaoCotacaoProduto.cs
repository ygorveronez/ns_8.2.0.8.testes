using System.Linq;

namespace Repositorio.Embarcador.CotacaoPedido
{
    public class SolicitacaoCotacaoProduto : RepositorioBase<Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacaoProduto>
    {
        public SolicitacaoCotacaoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacaoProduto BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacaoProduto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
