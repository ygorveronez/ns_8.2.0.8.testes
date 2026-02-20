using System;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT
{
    public class FaturamentoEFretePagamento
    {
        public DateTime Data { get; set; }

        public decimal Valor { get; set; }

        public decimal Juros { get; set; }

        public decimal Multa { get; set; }
    }
}
