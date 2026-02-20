using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Entrega
{
    public class SituacaoNotaFiscalNotasCarga
    {
        public int Numero { get; set; }
        public string Serie { get; set; }
        public string Chave { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataEmissao { get; set; }
        public String Emitente { get; set; }
        public String CNPJEmitente { get; set; }
        public String Recebedor { get; set; }
        public String CNPJRecebedor { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal SituacaoNota { get; set; }

        public string ProtocoloPedido { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }

        public List<Embarcador.Pedido.Produto> Produtos { get; set; }

        //TODO: FALTA ARRAY PRODUTOS DEVOLVIDOS
    }
}
