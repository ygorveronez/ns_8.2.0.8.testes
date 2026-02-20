using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class AgendamentoColetaPedidoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto>
    {
        #region Construtores

        public AgendamentoColetaPedidoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> BuscarPorCodigoAgendamentoColetaPedido(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto>();
            var result = from obj in query where obj.AgendamentoColetaPedido.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> BuscarPorCodigoAgendamentoColeta(int codigoAgendamentoColeta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto>();
            var result = from obj in query where obj.AgendamentoColetaPedido.AgendamentoColeta.Codigo == codigoAgendamentoColeta select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> BuscarPorPedidoAgendamentoColetaAgendado(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto>();
            var result = from obj in query
                         where codigosPedidos.Contains(obj.AgendamentoColetaPedido.Pedido.Codigo) &&
                            (obj.AgendamentoColetaPedido.AgendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.Agendado ||
                            obj.AgendamentoColetaPedido.AgendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.AguardandoConfirmacao ||
                            obj.AgendamentoColetaPedido.AgendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.AguardandoGeracaoSenha ||
                            obj.AgendamentoColetaPedido.AgendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.AguardandoCTes ||
                             obj.AgendamentoColetaPedido.AgendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.Finalizado)
                         select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto BuscarPorCodigoPedidoProduto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto>();
            var result = from obj in query where obj.PedidoProduto.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> BuscarPorListaCodigoAgendamentoColetaPedido(List<int> listaCodigoAgendamentoColeta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto>();
            var result = from obj in query
                         where listaCodigoAgendamentoColeta.Contains(obj.AgendamentoColetaPedido.Codigo)
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> BuscarPorCodigoPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto>();
            var result = from obj in query where obj.AgendamentoColetaPedido.Pedido.Codigo == codigoPedido select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> BuscarPorCodigoPedidoAgendado(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto>();
            var result = from obj in query
                         where obj.AgendamentoColetaPedido.Pedido.Codigo == codigoPedido &&
                            (obj.AgendamentoColetaPedido.AgendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.Agendado ||
                             obj.AgendamentoColetaPedido.AgendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.Finalizado)
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> BuscarPorListaCodigoPedidoAgendado(List<int> codigoPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto>();
            var result = from obj in query
                         where codigoPedidos.Contains(obj.AgendamentoColetaPedido.Pedido.Codigo) &&
                            (obj.AgendamentoColetaPedido.AgendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.Agendado ||
                             obj.AgendamentoColetaPedido.AgendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.Finalizado)
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> BuscarPorListaCodigoPedido(List<int> codigoPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto>();
            var result = from obj in query
                         where codigoPedidos.Contains(obj.AgendamentoColetaPedido.Pedido.Codigo)
                         select obj;
            return result.ToList();
        }
        #endregion
    }
}