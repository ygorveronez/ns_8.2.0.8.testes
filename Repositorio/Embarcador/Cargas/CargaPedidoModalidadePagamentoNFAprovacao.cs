using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoModalidadePagamentoNFAprovacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao>
    {
        public CargaPedidoModalidadePagamentoNFAprovacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao BuscarPorCargaPedido(int codigoCargapedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargapedido select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == carga select obj;
            return result.ToList();
        }

        public int BuscarQuantidadePorCargaESituacao(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoModalidadePagamento situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == carga && obj.SituacaoAutorizacaoModalidadePagamento == situacao select obj;
            return result.Count();
        }
    }
}
