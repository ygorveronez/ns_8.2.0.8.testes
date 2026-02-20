using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Compras
{
    public class FluxoCompraRecebimentoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Compras.FluxoCompraRecebimentoProduto>
    {
        public FluxoCompraRecebimentoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Compras.FluxoCompraRecebimentoProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.FluxoCompraRecebimentoProduto>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.FluxoCompraRecebimentoProduto> BuscarPorFluxoCompra(int codigoFluxoCompra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.FluxoCompraRecebimentoProduto>();
            var resut = from obj in query where obj.FluxoCompra.Codigo == codigoFluxoCompra select obj;
            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.FluxoCompraRecebimentoProduto> Consultar(int codigoFluxoCompra, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Compras.FluxoCompraRecebimentoProduto> result = Consultar(codigoFluxoCompra);

            result = result
                .Fetch(o => o.OrdemCompraMercadoria).ThenFetch(o => o.OrdemCompra).ThenFetch(o => o.Fornecedor)
                .Fetch(o => o.OrdemCompraMercadoria).ThenFetch(o => o.Produto);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(int codigoFluxoCompra)
        {
            IQueryable<Dominio.Entidades.Embarcador.Compras.FluxoCompraRecebimentoProduto> result = Consultar(codigoFluxoCompra);

            return result.Count();
        }

        public bool PossuiQuantidadeRecebidaNaoInformada(int codigoFluxoCompra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.FluxoCompraRecebimentoProduto>();
            var resut = from obj in query where obj.FluxoCompra.Codigo == codigoFluxoCompra && obj.QuantidadeRecebida == 0 select obj;
            return resut.Any();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Compras.FluxoCompraRecebimentoProduto> Consultar(int codigoFluxoCompra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.FluxoCompraRecebimentoProduto>();

            var result = from obj in query where obj.FluxoCompra.Codigo == codigoFluxoCompra select obj;

            return result;
        }

        #endregion
    }
}
