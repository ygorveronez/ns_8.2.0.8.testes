namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class ContratoFreteDestino
    {
        public string destinatario { get; set; }
        public string CNPJDestinatario { get; set; }
        public ContratoFreteOrigemEndereco endereco { get; set; }
    }
}
