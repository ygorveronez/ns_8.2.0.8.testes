using System;

namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public class NotaFiscalParcela
    {
        public int Codigo { get; set; }
        public decimal Valor { get; set; }
        public decimal Desconto { get; set; }
        public decimal Acrescimo { get; set; }
        public DateTime? DataEmissao { get; set; }
        public DateTime? DataVencimento { get; set; }
        public int Sequencia { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela Situacao { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo Forma { get; set; }
    }
}