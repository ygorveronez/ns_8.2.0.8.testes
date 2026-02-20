using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Produtos
{
    public class OrdemDeCompraHistorial : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.OrdemCompraHistorial>
    {
        #region Constructores
        public OrdemDeCompraHistorial(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos
        public List<Dominio.Entidades.Embarcador.Produtos.OrdemCompraHistorial> BuscarPorCodigoIntegracaoEmbarcador(int codigoIntegracaoEmbarcador)
        {
            var query = Consultar(codigoIntegracaoEmbarcador, numeroDocumento: "", numeroItemDocumento: "", codigoProdutoEmbarcador: "");
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.OrdemCompraHistorial> BuscarPorNumeroItemDocumento(string numeroItemDocumento, string codigoProdutoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametros)
        {
            var query = Consultar(codigoIntegracaoEmbarcador: 0, numeroItemDocumento, numeroItemDocumento: "", codigoProdutoEmbarcador: "");
            return ObterLista(query, parametros);
        }

        #endregion

        #region Metodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Produtos.OrdemCompraHistorial> Consultar(int codigoIntegracaoEmbarcador, string numeroDocumento, string numeroItemDocumento, string codigoProdutoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.OrdemCompraHistorial>();

            if (!string.IsNullOrEmpty(numeroDocumento))
                query = query.Where(s => s.NumeroDocumento == numeroDocumento);

            if (codigoIntegracaoEmbarcador > 0)
                query = query.Where(s => s.ControleIntegracaoEmbarcador == codigoIntegracaoEmbarcador);

            if (!string.IsNullOrEmpty(numeroItemDocumento))
                query = query.Where(s => s.NumeroItemDocumento.Contains(numeroItemDocumento));

            if (!string.IsNullOrEmpty(codigoProdutoEmbarcador))
                query = query.Where(s => s.Produto.CodigoProdutoEmbarcador == codigoProdutoEmbarcador);

            return query;
        }

        #endregion
    }
}
