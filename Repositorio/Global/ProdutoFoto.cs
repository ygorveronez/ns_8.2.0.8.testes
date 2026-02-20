using System.Linq;

namespace Repositorio
{
    public class ProdutoFoto : RepositorioBase<Dominio.Entidades.ProdutoFoto>, Dominio.Interfaces.Repositorios.ProdutoFoto
    {
        public ProdutoFoto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.ProdutoFoto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ProdutoFoto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ProdutoFoto BuscarPorProduto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ProdutoFoto>();
            var result = from obj in query where obj.Produto.Codigo == codigo && obj.Status select obj;
            return result.FirstOrDefault();
        }

        #endregion
    }
}
