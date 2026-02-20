namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class RetornoCarga
    {
        public int Protocol { get; set; }
        public string shipment { get; set; }
        public bool Status { get; set; }
        public string dataRetorno { get; set; }
        public string Mensagem { get; set; }
        public int codigoMensagem { get; set; }
        public string SAPRequest { get; set; }
    }
}
