using System;

namespace Dominio.ObjetosDeValor.MDFe
{
    public class ParcelaPagamento
    {
        public int ParcelaNumero { get; set; }
        public DateTime ParcelaVencimento { get; set; }
        public decimal ParcelaValor { get; set; }
    }
}
