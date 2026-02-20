using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Produtos
{
    public class ProdutoEmbarcadorOrganizacao : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao>
    {
        public ProdutoEmbarcadorOrganizacao(UnitOfWork unitOfWork) : base (unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao> BuscarFiliaisPorProdutoEmbarcador(int codigoProdutoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao>()
                .Where(o => o.ProdutoEmbarcador.Codigo == codigoProdutoEmbarcador);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao BuscarPorProdutoEmbarcadorEOrganizacao(int codigoProdutoEmbarcador, int codigoOrganizacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao>()
                .Where(o => o.ProdutoEmbarcador.Codigo == codigoProdutoEmbarcador && o.Organizacao.Codigo == codigoOrganizacao);

            return query.FirstOrDefault();
        }

        public bool ProdutoEmbarcadorOrganizacaoExistente(int codigoProdutoEmbarcador, int codigoOrganizacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao>()
                .Where(o => o.ProdutoEmbarcador.Codigo == codigoProdutoEmbarcador && o.Organizacao.Codigo == codigoOrganizacao);

            return query.Any();
        }

        #endregion
    }
}
