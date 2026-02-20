using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class DocumentoEntradaDuplicata
    {
        public int Sequencia { get; set; }
        public string Numero { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? DataPagamento { get; set; }
        public bool Pago { get; set; }
        public string Observacao { get; set; }
        public string NumeroBoleto { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo Forma { get; set; }
        public Cliente Portador { get; set; }
    }
}
