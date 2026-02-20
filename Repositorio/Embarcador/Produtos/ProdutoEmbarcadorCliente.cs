using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Produtos
{
    public class ProdutoEmbarcadorCliente : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorCliente>
    {
        public ProdutoEmbarcadorCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorCliente BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorCliente>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorCliente BuscarPorClienteCodigo(string codigoProduto, double cnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorCliente>();
            var result = from obj in query where obj.CodigoBarras == codigoProduto && obj.Cliente.CPF_CNPJ == cnpjCliente select obj;
            return result.FirstOrDefault();
        }
    }
}
