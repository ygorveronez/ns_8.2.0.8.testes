namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas
{
    public sealed class Empresa
    {
        public int Atividade { get; set; }

        public string CNPJ { get; set; }

        public string RazaoSocial { get; set; }

        public string NomeFantasia { get; set; }

        public string CodigoIntegracao { get; set; }

        public string IE { get; set; }

        public string InscricaoMunicipal { get; set; }

        public string InscricaoST { get; set; }

        public string RNTRC { get; set; }

        public bool SimplesNacional { get; set; }

        public string Emails { get; set; }

        public bool EmissaoDocumentosForaDoSistema { get; set; }

        public bool LiberacaoParaPagamentoAutomatico { get; set; }

        public Localidade.Endereco Endereco { get; set; }

        public DadosBancarios DadosBancarios { get; set; }

        public Certificado Certificado { get; set; }

        public string CodigoDocumento { get; set; }
    }
}
