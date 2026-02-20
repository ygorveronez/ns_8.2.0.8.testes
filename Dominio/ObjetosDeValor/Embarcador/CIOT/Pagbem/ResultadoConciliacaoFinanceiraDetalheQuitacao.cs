using System;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class ResultadoConciliacaoFinanceiraDetalheQuitacao
    {
        public decimal valorTotalAjuste { get; set; }
        public decimal pesoChegada { get; set; }
        public DateTime dataChegada { get; set; }
        public decimal valorAvaria { get; set; }
        public decimal valorDiferencaFrete { get; set; }
        public decimal valorQuebra { get; set; }
    }
}
