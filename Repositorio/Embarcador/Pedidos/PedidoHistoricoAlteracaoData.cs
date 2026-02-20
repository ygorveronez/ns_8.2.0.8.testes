using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoHistoricoAlteracaoData : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoHistoricoAlteracaoData>
    {
        public PedidoHistoricoAlteracaoData(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public new long Inserir(Dominio.Entidades.Embarcador.Pedidos.PedidoHistoricoAlteracaoData pedidoHistoricoAlteracaoData, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai = null)
        {
            if (pedidoHistoricoAlteracaoData.DataAlteracao == DateTime.MinValue) pedidoHistoricoAlteracaoData.DataAlteracao = DateTime.Now;
            return base.Inserir(pedidoHistoricoAlteracaoData, auditado, historioPai);
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoHistoricoAlteracaoData BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoHistoricoAlteracaoData>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoHistoricoAlteracaoData> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoHistoricoAlteracaoData>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoHistoricoAlteracaoData> BuscarPorPedidoTipoData(int codigoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDataPedido tipoData)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoHistoricoAlteracaoData>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido && obj.TipoData == tipoData select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoHistoricoAlteracaoData BuscarDataAnteriorPorPedidoTipoData(int codigoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDataPedido tipoData)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoHistoricoAlteracaoData>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido && obj.TipoData == tipoData select obj;
            result = result.OrderByDescending(obj => obj.DataAlteracao);
            return result.Skip(0).Take(1).FirstOrDefault();

        }

    }
}
