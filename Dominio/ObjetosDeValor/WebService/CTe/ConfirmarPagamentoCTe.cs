namespace Dominio.ObjetosDeValor.WebService.CTe
{
    public class ConfirmarPagamentoCTe
    {
        public int ProtocoloCTe { get; set; }
        public int NumeroPagamento { get; set; }
        public string DataPagamento { get; set; }
        public string Observacao { get; set; }
        public decimal ValorParcelaPaga { get; set; }
        public int SequenciaParcelaPaga { get; set; }
    }
}
