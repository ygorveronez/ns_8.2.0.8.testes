using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Produtos
{
    public class ProdutoComposicao : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.ProdutoComposicao>
    {
        public ProdutoComposicao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoComposicao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoComposicao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
