namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas
{
    public sealed class Pessoa
    {
        public bool ClienteExterior { get; set; }

        public string CPFCNPJ { get; set; }

        public string CodigoIntegracao { get; set; }

        public Dominio.Enumeradores.TipoPessoa TipoPessoa { get; set; }

        public int CodigoAtividade { get; set; }

        public string RGIE { get; set; }

        public string IM { get; set; }

        public string InscricaoSuframa { get; set; }

        public string RazaoSocial { get; set; }

        public string NomeFantasia { get; set; }

        public string CNAE { get; set; }

        public Localidade.Endereco Endereco { get; set; }

        public string Email { get; set; }

        public bool AtualizarEnderecoPessoa { get; set; }

        public string EmailContador { get; set; }

        public string EmailContato { get; set; }

        public bool EnviarEmialContador { get; set; }

        public bool EnviarEmailContato { get; set; }

        public bool EnviarEmail { get; set; }

        public string CPFCNPJSemFormato { get; set; }

        public string Codigo { get; set; }

        public string CodigoCategoria { get; set; }

        public string CodigoDocumento { get; set; }

        public string RNTRC { get; set; }

        public string NumeroCartaoCIOT { get; set; }

        public bool? GerarCIOT { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT? TipoFavorecidoCIOT { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? TipoPagamentoCIOT { get; set; }

        public GrupoPessoa GrupoPessoa { get; set; }
    }
}
