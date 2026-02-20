using System;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class ResultadoConciliacaoFinanceira
    {
        public int idViagem { get; set; }
        public string numeroViagemCliente { get; set; }
        public string tipoEvento { get; set; }
        public decimal valorEvento { get; set; }
        public string eventoLocal { get; set; }
        public string eventoTipoLocal { get; set; }
        public DateTime data { get; set; }
        public long numeroTransacao { get; set; }
        public DateTime dataVencimento { get; set; }
        public string produto { get; set; }
        public ResultadoConciliacaoFinanceiraDetalheQuitacao detalhesQuitacao { get; set; }
    }
}
