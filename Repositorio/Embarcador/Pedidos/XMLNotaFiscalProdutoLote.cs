using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class XMLNotaFiscalProdutoLote : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProdutoLote>
    {
        public XMLNotaFiscalProdutoLote(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProdutoLote BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProdutoLote>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProdutoLote> BuscarPorNotaFiscal(int codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProdutoLote>();
            var result = from obj in query where codigoXMLNotaFiscal == obj.XMLNotaFiscalProduto.XMLNotaFiscal.Codigo select obj;
            return result
                .Fetch(obj => obj.XMLNotaFiscalProduto)
                .ThenFetch(obj => obj.Produto)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProdutoLote> BuscarPorNotaFiscais(List<int> codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProdutoLote>();
            var result = from obj in query where codigoXMLNotaFiscal.Contains(obj.XMLNotaFiscalProduto.XMLNotaFiscal.Codigo) select obj;
            return result
                .Fetch(obj => obj.XMLNotaFiscalProduto)
                .ToList();
        }

        public void ExcluirTodosPorXMLNotaFiscal(int codigoXMLNotaFiscal)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE XMLNotaFiscalProdutoLote obj WHERE obj.XMLNotaFiscalProduto in (select pro.Codigo from XMLNotaFiscalProduto pro where pro.XMLNotaFiscal.Codigo =:XMLNotaFiscal)")
                             .SetInt32("XMLNotaFiscal", codigoXMLNotaFiscal)
                             .ExecuteUpdate();
        }
        
        public Task ExcluirTodosPorXMLNotaFiscalAsync(int codigoXMLNotaFiscal)
        {
            return UnitOfWork.Sessao.CreateQuery("DELETE XMLNotaFiscalProdutoLote obj WHERE obj.XMLNotaFiscalProduto in (select pro.Codigo from XMLNotaFiscalProduto pro where pro.XMLNotaFiscal.Codigo =:XMLNotaFiscal)")
                             .SetInt32("XMLNotaFiscal", codigoXMLNotaFiscal)
                             .ExecuteUpdateAsync();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProdutoLote BuscarXMLNotaFiscalProdutoLotePorXMLNotaFiscalProduto(int codigoXMLNotaFiscalProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProdutoLote>()
                .Where(obj => obj.XMLNotaFiscalProduto.Codigo == codigoXMLNotaFiscalProduto);

            return query.FirstOrDefault();
        }
    }
}
