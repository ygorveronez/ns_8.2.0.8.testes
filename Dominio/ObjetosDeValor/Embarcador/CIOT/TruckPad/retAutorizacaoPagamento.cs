namespace Dominio.ObjetosDeValor.Embarcador.CIOT.TruckPad
{
    public class retAutorizacaoPagamento
    {
        public string id { get; set; }
        public string external_client_id { get; set; }
        public string code { get; set; }
        public decimal value_money { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string note { get; set; }
    }
}