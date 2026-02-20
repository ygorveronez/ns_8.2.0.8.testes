namespace Dominio.ObjetosDeValor.Embarcador.CIOT.TruckPad
{
    public class retCIOTPadrao
    {
        public string id { get; set; }
        public string status { get; set; }
        public string code { get; set; }
        public string verifier_code { get; set; }
        public string message { get; set; }
        public bool is_warning { get; set; }
    }
}