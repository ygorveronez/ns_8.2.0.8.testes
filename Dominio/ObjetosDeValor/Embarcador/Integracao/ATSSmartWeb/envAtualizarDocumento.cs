using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb
{
    public class envAtualizarDocumento
    {
        public List<envManifesto> Manifestos { get; set; }
    }

    public class envManifesto
    {
        public string CodigoExterno { get; set; }
        public string DataEmissao { get; set; }
        public List<envCTE> CTEs { get; set; }
    }

    public class envCTE
    {
        public int? NumeroCTE { get; set; }
        public int? SerieCTE { get; set; }
        public decimal? Quantidade { get; set; }
        public decimal? Peso { get; set; }
        public decimal? ValorMercadoria { get; set; }
        public decimal? ValorServico { get; set; }
        public string? DataEmissao { get; set; }
        public int? Status { get; set; }
        public string CodigoExterno { get; set; }
        public List<envNotaFiscal> NotasFiscais { get; set; }
    }

    public class envNotaFiscal
    {
        public int? Numero { get; set; }
        public int? Serie { get; set; }
        public decimal? Valor { get; set; }
        public string Shipment { get; set; }
        public List<envNotaFiscalProduto> NotaFiscalProdutos { get; set; }
    }

    public class envNotaFiscalProduto
    {
        public envProdutoNF Produto { get; set; }
    }

    public class envProdutoNF
    {
        public string CodigoExterno { get; set; }
        public decimal? MetragemCubica { get; set; }
        public string Nome { get; set; }
        public decimal? Litros { get; set; }
    }
}
