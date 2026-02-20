using System;

namespace Dominio.ObjetosDeValor.Embarcador.Fatura
{
    public class FaturaIntegracaoParcela
    {
        public decimal Valor { get; set; }
        public decimal ValorTotalMoeda { get; set; }
        public decimal Desconto { get; set; }
        public decimal Acrescimo { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public int Sequencia { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo? FormaTitulo { get; set; }
        public int CodigoTitulo { get; set; }
        public DateTime VencimentoTitulo { get; set; }
        public string NossoNumeroBoleto { get; set; }
    }
}
