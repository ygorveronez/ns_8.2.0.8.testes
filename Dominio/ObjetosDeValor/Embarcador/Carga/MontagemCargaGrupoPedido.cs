using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class MontagemCargaGrupoPedido
    {
        public List<Entidades.Embarcador.Pedidos.Pedido> Pedidos { get; set; }
        
        public Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }
        
        public int CodigoFilial { get; set; }

        public Entidades.Empresa Transportador { get; set; }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        public DateTime DataCarregamento { get; set; }

        public Dictionary<int, decimal> PedidosPesos { get; set; }

        public List<MontagemCargaGrupoPedidoProduto> Produtos { get; set; }

        public List<MontagemCargaPonto> PontosDeApoio { get; set; }

        public int QtdeEntregas { get; set; }

        public decimal ValorFreteVencedor { get; set; }

        public bool ExigeIsca { get; set; }

        public Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco MontagemCarregamentoBloco { get; set; }
    }
}
