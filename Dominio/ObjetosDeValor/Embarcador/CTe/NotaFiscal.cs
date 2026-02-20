using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class NotaFiscal
    {
        public string Numero { get; set; }
        public string Serie { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorICMSST { get; set; }
        public decimal ValorProdutos { get; set; }
        public string PIN { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal BaseCalculoICMSST { get; set; }
        public string CFOP { get; set; }
        public decimal Peso { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloNotaFiscal ModeloNotaFiscal { get; set; }
        public string NumeroReferenciaEDI { get; set; }
        public string PINSuframa { get; set; }
        public string NCMPredominante { get; set; }
        public string NumeroControleCliente { get; set; }
    }
}
