using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class DocumentoEntrada
    {
        public long Protocolo { get; set; }
        public TipoMovimento TipoMovimento { get; set; }
        public NaturezaDaOperacao NaturezaOperacao { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime DataEmissao { get; set; }
        public long Numero { get; set; }
        public string Serie { get; set; }
        public string Chave { get; set; }
        public Pessoas.Pessoa Fornecedor { get; set; }
        public Pessoas.Empresa Destinatario { get; set; }
        public OrdemCompra OrdemCompra { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorBruto { get; set; }
        public decimal ValorProdutos { get; set; }
        public decimal ValorTotalDesconto { get; set; }
        public List<Pedido.Produto> Produtos { get; set; }
    }
}
