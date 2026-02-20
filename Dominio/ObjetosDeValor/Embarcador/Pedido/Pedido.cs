using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class Pedido
    {
        public int ProtocoloPedido { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public string CodigoFilialEmbarcador { get; set; }
        public Dominio.ObjetosDeValor.Localidade Origem { get; set; }
        public bool UsarOutroEnderecoOrigem { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEndereco EnderecoOrigem { get; set; }
        public Dominio.ObjetosDeValor.Localidade Destino { get; set; }
        public bool UsarOutroEnderecoDestino { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEndereco EnderecoDestino { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Logistica.Fronteira Fronteira { get; set; }
        public string PrevisaoEntrega { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido SituacaoPedido { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public string DataCarregamentoPedido { get; set; }
        public Dominio.ObjetosDeValor.CTe.Cliente Destinatario { get; set; }
        public Dominio.ObjetosDeValor.CTe.Cliente Recebedor { get; set; }
        public Dominio.ObjetosDeValor.CTe.Cliente Expedidor { get; set; }
        public Dominio.ObjetosDeValor.CTe.Cliente Remetente { get; set; }
        public Dominio.ObjetosDeValor.CTe.Cliente Tomador { get; set; }
        public int NumeroPaletes { get; set; }
        public decimal PesoTotalPaletes { get; set; }
        public decimal ValorTotalPaletes { get; set; }
        public int Numero { get; set; }
        public TipoColeta TipoColeta { get; set; }
        public TipoCarga TipoCarga { get; set; }
        public string Observacao { get; set; }
        public string ObservacaoCTe { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta Requisitante { get; set; }
        public Dominio.Enumeradores.TipoPagamento TipoPagamento { get; set; }
        public Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }
        public decimal PesoTotalCarga { get; set; }
        public decimal ValorTotalNotasFiscais { get; set; }
        public string CodigoPedidoCliente { get; set; }
        public string DataInicialColeta { get; set; }
        public string DataFinalColeta { get; set; }
        public List<int> CodigosCargasMultiEmbarcador { get; set; }
    }
}
