using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido
{
    public class LoteLiberacaoComercialPedido : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido>
    {
        public LoteLiberacaoComercialPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido>();

            int? ultimoNumero = query.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.LoteLiberacaoComercialPedido.FiltroPesquisaLoteLiberacaoComercialPedido filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido> result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.LoteLiberacaoComercialPedido.FiltroPesquisaLoteLiberacaoComercialPedido filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido> result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.LoteLiberacaoComercialPedido.FiltroPesquisaLoteLiberacaoComercialPedido filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido>();
            var queryPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.CodigosPedidos?.Count > 0)
                queryPedido = queryPedido.Where(obj => filtrosPesquisa.CodigosPedidos.Contains(obj.Pedido.Codigo));

            var resultPedido = from obj in queryPedido select obj;

            if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal > 0)
                result = result.Where(obj => obj.Numero >= filtrosPesquisa.NumeroInicial && obj.Numero <= filtrosPesquisa.NumeroFinal);
            else if (filtrosPesquisa.NumeroInicial > 0)
                result = result.Where(obj => obj.Numero == filtrosPesquisa.NumeroInicial);
            else if (filtrosPesquisa.NumeroFinal > 0)
                result = result.Where(obj => obj.Numero == filtrosPesquisa.NumeroFinal);

            if (filtrosPesquisa.Situacao != SituacaoLoteLiberacaoComercialPedido.Todos)
                result = result.Where(obj => obj.Situacao == filtrosPesquisa.Situacao);

            result = result.Where(o => queryPedido.Any(p => p.LoteLiberacaoComercialPedido.Codigo == o.Codigo));

            return result;
        }
        #endregion
    }
}
