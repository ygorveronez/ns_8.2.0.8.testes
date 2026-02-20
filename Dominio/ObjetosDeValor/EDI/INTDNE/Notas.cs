using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.INTDNE
{
    public class Notas
    {
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Pessoa { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal NotaFiscal { get; set; }
        public string TipoOperacao { get; set; }
        public string TipoNota { get; set; }
        public int TipoEntradaSaida { get; set; }
        public string ZonaTransporte { get; set; }
        public string DocumentoNegFrete { get; set; }
        public string Equipamento { get; set; }
        public string MeioTransporte { get; set; }
        public string Embalagem { get; set; }
        public string Territorio { get; set; }
        public string Vendedor { get; set; }
        public string Separador { get; set; }
        public string Lote { get; set; }
        public string Romaneio { get; set; }
        public string TipoFrete { get; set; }
        public string DocumentoVinculado { get; set; }
        public string DocumentoVinculadoSerie { get; set; }
        public string IsentoImposto { get; set; }
        public string ItemSubstituicaoTributaria { get; set; }
        public string TipoFinalidadeOperacao { get; set; }
        public string IndiceFinanceiro { get; set; }
        public string TipoCarga { get; set; }
        public string UrgenciaEntrega { get; set; }
        public string FreteDiferenciado { get; set; }
        public string DataEntrega { get; set; }
        public string DataEmbarque { get; set; }
        public string ChaveResponsavelFrete { get; set; }
        public string DataTabelaPreco { get; set; }
        public string DataPrevisaoColeta { get; set; }
        public string DataPrevisaoEntrega { get; set; }
        public decimal ValorFreteCliente { get; set; }
        public int StatusDNE { get; set; }
        public string ExcluirRegistroEmbarcador { get; set; }
        public string ExcluirItemDNE { get; set; }
        public string ExcluirReferencia { get; set; }
        public List<Itens> Itens { get; set; }
    }
}
