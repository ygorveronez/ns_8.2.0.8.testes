namespace Dominio.ObjetosDeValor
{
    public class Empresa
    {
        public string CodigoIntegracao { get; set; }

        public string ANTT { get; set; }

        public string Bairro { get; set; }

        public string CEP { get; set; }

        public string CNAE { get; set; }

        public string CNPJ { get; set; }

        public int CodigoIBGECidade { get; set; }

        public string Complemento { get; set; }

        public string Contato { get; set; }

        public string DataEmissaoANTT { get; set; }

        public string DataFinalCertificado { get; set; }

        public string DataInicialCertificado { get; set; }

        public string DataValidadeANTT { get; set; }

        public string Email { get; set; }

        public string EmailAdministrativo { get; set; }

        public string EmailContador { get; set; }

        public string Endereco { get; set; }

        public string Fax { get; set; }

        public string Inscricao_ST { get; set; }

        public string InscricaoEstadual { get; set; }

        public string NomeContador { get; set; }

        public string NomeFantasia { get; set; }

        public string Numero { get; set; }

        public bool? OptanteSimplesNacional { get; set; }

        public string RazaoSocial { get; set; }

        public string Responsavel { get; set; }

        public string SenhaCertificado { get; set; }

        public string SerieCertificado { get; set; }

        public string Suframa { get; set; }

        public bool StatusEmail { get; set; }

        public bool StatusEmailAdministrativo { get; set; }

        public bool StatusEmailContador { get; set; }

        public string Telefone { get; set; }

        public string TelefoneContador { get; set; }

        public string TelefoneContato { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoInclusaoPedagioBaseCalculoICMS TipoInclusaoPedagioBaseCalculoICMS { get; set; }

        public string Status { get; set; }
    }
}
