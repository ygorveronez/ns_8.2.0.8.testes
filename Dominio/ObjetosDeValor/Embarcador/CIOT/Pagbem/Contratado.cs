namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class Contratado
    {
        public string CNPJCPF { get; set; }
        public string nome { get; set; }
        public string IE { get; set; }
        public string nomeMae { get; set; }
        public string dataNascimento { get; set; }
        public Endereco endereco { get; set; }
        public Telefone telefone1 { get; set; }
        public Telefone telefone2 { get; set; }
        public Telefone telefoneCelular { get; set; }
        public string email { get; set; }
        public string RNTRC { get; set; }
        public string PIS { get; set; }
    }
}