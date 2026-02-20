namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class RetornoMotivoDevolucaoEntrega
    {
        public bool success { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public RetornoMotivoDevolucaoEntregaRetorno? partialDeliveryReason { get; set; }
        public RetornoMotivoDevolucaoEntregaRetorno? notDeliveredReason { get; set; }
    }

    public class RetornoMotivoDevolucaoEntregaRetorno
    {
        public string _id { get; set; }
    }
}