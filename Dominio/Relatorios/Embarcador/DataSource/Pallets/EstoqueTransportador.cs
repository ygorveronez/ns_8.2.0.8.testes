using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pallets
{
    public class EstoqueTransportador
    {
        public int Codigo { get; set; }
        public string Transportador { get; set; }
        public string TransportadorCnpj { get; set; }
        public string TransportadorCodigoIntegracao { get; set; }
        public DateTime Data { get; set; }
        public string TipoLancamento { get; set; }
        public string Observacao { get; set; }
        public int Entrada { get; set; }
        public int Saida { get; set; }
        public int Descarte { get; set; }
        public int SaldoTotal { get; set; }
    }
}
