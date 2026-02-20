namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public class OfertaComponenteDados
    {
        public int CodigoRota { get; set; }
        public int CodigoTransportador { get; set; }
        public string RotaDescricao { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string ValorFrete { get; set; }
        public string AdicionalPorEntrega { get; set; }
        public string Ajudante { get; set; }
        public string Pedagio { get; set; }
        public string Transportador { get; set; }
        public decimal Total { get; set; }
        public int VeiculosVerdes { get; set; }
    }
}
