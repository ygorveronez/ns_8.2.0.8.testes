using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ProdutoFornecedor : RepositorioBase<Dominio.Entidades.ProdutoFornecedor>, Dominio.Interfaces.Repositorios.ProdutoFornecedor
    {
        public ProdutoFornecedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ProdutoFornecedor BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ProdutoFornecedor>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.ProdutoFornecedor BuscarPorCodigoProdutoEFornecedor(int codigoEmpresa, string codigoProduto, double fornecedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ProdutoFornecedor>();

            var result = from obj in query where obj.Produto.Empresa.Codigo == codigoEmpresa && obj.CodigoProduto.Equals(codigoProduto) && obj.Fornecedor.CPF_CNPJ == fornecedor && obj.Produto.Status == "A" select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ProdutoFornecedor BuscarPorProdutoEFornecedor(string codigoProduto, double fornecedor, int codigoEmpresa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ProdutoFornecedor>();

            var result = from obj in query where obj.CodigoProduto.Equals(codigoProduto) && obj.Fornecedor.CPF_CNPJ == fornecedor && obj.Produto.Status == "A" select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Produto.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ProdutoFornecedor BuscaPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ProdutoFornecedor>();

            var result = from obj in query where obj.Codigo == codigo && obj.Produto.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ProdutoFornecedor> Consultar(int codigoEmpresa, string numero, string produto, double cnpjFornecedor, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ProdutoFornecedor>();

            var result = from obj in query where obj.Produto.Empresa.Codigo == codigoEmpresa && obj.Produto.Status == "A" select obj;

            if (!string.IsNullOrWhiteSpace(numero))
                result = result.Where(o => o.CodigoProduto.Contains(numero));

            if (!string.IsNullOrWhiteSpace(produto))
                result = result.Where(o => o.Produto.Descricao.Contains(produto));

            if (cnpjFornecedor > 0)
                result = result.Where(o => o.Fornecedor.CPF_CNPJ == cnpjFornecedor);

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string numero, string produto, double cnpjFornecedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ProdutoFornecedor>();

            var result = from obj in query where obj.Produto.Empresa.Codigo == codigoEmpresa && obj.Produto.Status == "A" select obj;

            if (!string.IsNullOrWhiteSpace(numero))
                result = result.Where(o => o.CodigoProduto.Contains(numero));

            if (!string.IsNullOrWhiteSpace(produto))
                result = result.Where(o => o.Produto.Descricao.Contains(produto));

            if (cnpjFornecedor > 0)
                result = result.Where(o => o.Fornecedor.CPF_CNPJ == cnpjFornecedor);

            return result.Count();
        }
    }
}
