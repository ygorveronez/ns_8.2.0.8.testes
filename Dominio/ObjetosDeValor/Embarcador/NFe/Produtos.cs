using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public class Produtos
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public GrupoProduto GrupoProduto { get; set; }
        public string NCM { get; set; }
        public int CFOP { get; set; }
        public string UnidadeComercial { get; set; }
        public decimal QuantidadeComercial { get; set; }
        public decimal ValorUnitarioComercial { get; set; }
        public decimal ValorTotal { get; set; }
        public string UnidadeTributaria { get; set; }
        public string CodigoCEAN { get; set; }
        public decimal QuantidadeTributaria { get; set; }
        public decimal ValorUnitarioTributaria { get; set; }
        public string NumeroPedidoCompra { get; set; }
        public ComplementoProduto ComplementoProduto { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.NFe.Lote> Lotes { get; set; }
        public string CSTICMS { get; set; }
        public string OrigemMercadoria { get; set; }
        public string CodigoNFCI { get; set; }
    }
}
