namespace Dominio.ObjetosDeValor.CTe
{
    public class Empresa
    {
        public string CNPJ { get; set; }

        public string InscricaoEstadual { get; set; }

        public string InscricaoMunicipal { get; set; }

        public string RazaoSocial { get; set; }

        public string NomeFantasia { get; set; }

        public string Telefone1 { get; set; }

        public string Telefone2 { get; set; }

        public string Contato { get; set; }

        public string TelefoneContato { get; set; }

        public int CodigoIBGECidade { get; set; }

        public string Endereco { get; set; }

        public string Complemento { get; set; }

        public string Numero { get; set; }

        public string CEP { get; set; }

        public string Responsavel { get; set; }

        public string Bairro { get; set; }

        public string RNTRC { get; set; }

        public string Email { get; set; }

        public bool StatusEmail { get; set; }

        public string EmailContador { get; set; }

        public bool StatusEmailContador { get; set; }

        public string EmailAdministrativo { get; set; }

        public bool StatusEmailAdministrativo { get; set; }

        public string NomeContador { get; set; }

        public string TelefoneContador { get; set; }

        public bool? OptanteSimplesNacional { get; set; }

        public bool Atualizar { get; set; }
    }
}
