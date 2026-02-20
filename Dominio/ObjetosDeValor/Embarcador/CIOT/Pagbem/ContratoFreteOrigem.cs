namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class ContratoFreteOrigem
    {
        public string filialContratante { get; set; }
        public string remetente { get; set; }
        public string CNPJRemetente { get; set; }
        public ContratoFreteOrigemEndereco endereco { get; set; }
    }
}
