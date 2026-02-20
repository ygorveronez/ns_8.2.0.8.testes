using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.GestaoEntregas
{
    public class EntregaPedido
    {
        public static List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEntrega> PedidoPendentesIntegracao(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedido)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEntrega> listaFormatada = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEntrega>();

            listaFormatada = (from o in cargaEntregaPedido
                              select new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEntrega()
                              {
                                  Protocolo = new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEntregaProtocolo()
                                  {
                                      ProtocoloCarga = o.CargaPedido.CargaOrigem.Protocolo,
                                      ProtocoloPedido = o.CargaPedido.Pedido.Protocolo,
                                  },
                                  Situacao = (o.CargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntregaPedido.Entregue : (o.CargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntregaPedido.Rejeitado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntregaPedido.NaoEntregue)),
                                  DataConfirmacao = o.CargaEntrega.DataConfirmacao.HasValue ? o.CargaEntrega.DataConfirmacao.Value.ToString("dd/MM/yyyy HH:mm:ss") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                              }).ToList();

            return listaFormatada;
        }

    }
}
