using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pessoas
{
    public class Empresa
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
        public Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco Endereco { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.DadosBancarios DadosBancarios { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Certificado Certificado { get; set; }
        public string CodigoDocumento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario RegimeTributario { get; set; }
        public bool? Ativo { get; set; }
        public int Protocolo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoCIOT? TipoCIOT { get; set; }
        public int Codigo { get; set; }
        public string IMO { get; set; }
        public DateTime? DataValidadeIMO { get; set; }

    }
}
