using System.Linq;

namespace Repositorio.Embarcador.Produtos
{
    public class GrupoProdutoOpenTech : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoOpenTech>
    {
        public GrupoProdutoOpenTech(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Produtos.GrupoProdutoOpenTech BuscarPorGrupoProduto(int codigoGrupoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoOpenTech>();

            var result = from obj in query where obj.GrupoProduto.Codigo == codigoGrupoProduto select obj;

            return result.FirstOrDefault();
        }
    }
}
