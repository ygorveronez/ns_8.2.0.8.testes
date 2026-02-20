using System;

namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public class Boleto
    {
        public string Numero { get; set; }
        public int Parcela { get; set; }
        public DateTime? DataVencimento { get; set; }
        public decimal Valor { get; set; }
    }
}
