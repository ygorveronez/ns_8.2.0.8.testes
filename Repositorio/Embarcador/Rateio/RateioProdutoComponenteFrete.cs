using System.Collections.Generic;
using System.Linq;


namespace Repositorio.Embarcador.Rateio
{
    public class RateioProdutoComponenteFrete : RepositorioBase<Dominio.Entidades.Embarcador.Rateio.RateioProdutoComponenteFrete>
    {
        public RateioProdutoComponenteFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Rateio.RateioProdutoComponenteFrete> BuscarPorRateioCargaProduto(int codigoRateio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioProdutoComponenteFrete>();
            var result = from obj in query where obj.RateioCargaPedidoProduto.Codigo == codigoRateio select obj;
            return result.ToList();
        }

        public decimal BuscarTotalPorCargaCTeCompomente(int cargaCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioProdutoComponenteFrete>();
            var result = from obj in query where obj.RateioCargaPedidoProduto.CargaCTe.Codigo == cargaCTe && obj.TipoComponenteFrete == tipoComponente select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.Select(obj => (decimal?)obj.ValorComponente).Sum() ?? 0;
        }

        public decimal BuscarTotalPorCargaNFsCompomente(int cargaNFs, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioProdutoComponenteFrete>();
            var result = from obj in query where obj.RateioCargaPedidoProduto.CargaNFS.Codigo == cargaNFs && obj.TipoComponenteFrete == tipoComponente select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.Select(obj => (decimal?)obj.ValorComponente).Sum() ?? 0;
        }
    }
}
