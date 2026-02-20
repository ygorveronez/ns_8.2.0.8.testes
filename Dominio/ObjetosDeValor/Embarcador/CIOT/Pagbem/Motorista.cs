namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class Motorista
    {
        public string CPF { get; set; }
        public string nome { get; set; }
        public string RNTRC { get; set; }
        public string nomeMae { get; set; }
        public string dataNascimento { get; set; }
        public Endereco endereco { get; set; }
        public Telefone telefone1 { get; set; }
        public Telefone telefone2 { get; set; }
        public Telefone telefoneCelular { get; set; }
        public string email { get; set; }
        public RG RG { get; set; }
        public CNH CNH { get; set; }
        public string PIS { get; set; }
    }
}