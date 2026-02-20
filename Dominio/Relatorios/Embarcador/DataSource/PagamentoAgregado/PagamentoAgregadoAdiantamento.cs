using System;

namespace Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado
{
    public class PagamentoAgregadoAdiantamento
    {
        public DateTime Data { get; set; }
        public DateTime DataPagamento { get; set; }
        public int Numero { get; set; }
        public string Observacao { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
    }
}
