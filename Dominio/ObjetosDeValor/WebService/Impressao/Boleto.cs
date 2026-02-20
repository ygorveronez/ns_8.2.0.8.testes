namespace Dominio.ObjetosDeValor.WebService.Impressao
{
    public class Boleto
    {
        public int Codigo { get; set; }
        public string CodigoBanco { get; set; }
        public string NomeBanco { get; set; }
        public string Agencia { get; set; }
        public string NossoNumero { get; set; }
        public string DataVencimento { get; set; }
        public string LinhaDigitavel { get; set; }
        public string CodigoBarras { get; set; }
        public string NumeroDocumento { get; set; }
        public string DataDocumento { get; set; }
        public string LocalPagamento { get; set; }
        public string EspecieDocumento { get; set; }
        public string Aceite { get; set; }
        public string DataProcessamento { get; set; }
        public string UsoBanco { get; set; }
        public string Carteira { get; set; }
        public string EspecieMoeda { get; set; }
        public string Quantidade { get; set; }
        public string Instrucoes { get; set; }
        public string InstucoesAdicionais { get; set; }
        public string SacadoCNPJ { get; set; }
        public string SacadoIE { get; set; }
        public string SacadoNome { get; set; }
        public string SacadoRua { get; set; }
        public string SacadoNumero { get; set; }
        public string SacadoComplemento { get; set; }
        public string SacadoCEP { get; set; }
        public string SacadoBairro { get; set; }
        public string SacadoCidade { get; set; }
        public string SacadoEstado { get; set; }
        public string CedendeCodigo { get; set; }
        public string CedenteCNPJ { get; set; }
        public string CedenteIE { get; set; }
        public string CedenteNome { get; set; }
        public string CedenteRua { get; set; }
        public string CedenteNumero { get; set; }
        public string CedenteComplemento { get; set; }
        public string CedenteCEP { get; set; }
        public string CedenteBairro { get; set; }
        public string CedenteCidade { get; set; }
        public string CedenteEstado { get; set; }
        public decimal ValorDocumeno { get; set; }
        public decimal ValorDescontoAcrescimo { get; set; }
        public decimal ValorCobrado { get; set; }
        public string DigitoBanco { get; set; }
        public string CIP { get; set; }
    }
}
