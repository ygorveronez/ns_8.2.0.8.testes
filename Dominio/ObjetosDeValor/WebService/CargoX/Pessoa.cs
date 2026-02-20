namespace Dominio.ObjetosDeValor.WebService.CargoX
{
    public class Pessoa
    {
        public string CPFCNPJ { get; set; }
        public string Bairro { get; set; }
        public string CEP { get; set; }
        public int CodigoAtividade { get; set; }
        public string Complemento { get; set; }
        public string Email { get; set; }
        public int IBGE { get; set; }
        public string InscricaoEstadual { get; set; }
        public string NomeFantasia { get; set; }
        public string Numero { get; set; }
        public string RazaoSocial { get; set; }
        public string Rua { get; set; }
        public string RNTRC { get; set; }
        public string Telefone { get; set; }
        public Dominio.Enumeradores.TipoPessoa TipoPessoa { get; set; }
    }
}
