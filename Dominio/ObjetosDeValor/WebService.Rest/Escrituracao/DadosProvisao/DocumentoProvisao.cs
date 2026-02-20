using System;

namespace Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao
{
    public class DocumentoProvisao
    {
        #region atributos
        public int NumeroDocumento { get; set; }
        public DateTime? DataEmissao { get; set; }
        public string? CpfCnpjRemetente { get; set; }
        public string? Remetente { get; set; }
        public string? CpfCnpjDestinatario { get; set; }
        public string? Destinatario { get; set; }
        public string? CodigoIntegracaoTransportador { get; set; }
        public string? Origem { get; set; }
        public string? Destino { get; set; }
        public string? CpfCnpjTomador { get; set; }
        public string? Tomador { get; set; }
        public string? CpfCnpjExpedidor { get; set; }
        public string? Expedidor { get; set; }
        public string? CpfCnpjRecebedor { get; set; }
        public string? Recebedor { get; set; }
        public bool? Reentrega { get; set; }
        public int? NumeroCTe { get; set; }
        public string? SerieCTe { get; set; }
        public DateTime? DataEmissaoCte { get; set; }
        public DateTime? DataEmissaoNfsManual { get; set; }
        public int? Ocorrencia { get; set; }
        public string? TipoOcorrencia { get; set; }
        public DateTime? DataEmissaoOcorrencia { get; set; }
        public string? Cst { get; set; }
        public string? TipoCanhoto { get; set; }
        public int? CanhotoSerie { get; set; }
        public string? Situacao { get; set; }
        public string? SituacaoDigitalizacao { get; set; }
        public decimal? PesoBruto { get; set; }
        public decimal? ValorNf { get; set; }
        public decimal? ValorFrete { get; set; }
        public decimal? Icms { get; set; }
        public decimal? ValorIss { get; set; }
        public decimal? ValorIssRetido { get; set; }
        public decimal? Aliquota { get; set; }
        public decimal? AliquotaIss { get; set; }
        public decimal? AliquotaPis { get; set; }
        public decimal? AliquotaCofins { get; set; }
        public decimal? ValorProvisao { get; set; }
        public bool IcmsInclusoNaBc { get; set; }
        public bool IssInclusoNaBc { get; set; }
        public decimal? TaxaDescarga { get; set; }
        public decimal? TotalReceber { get; set; }

        #endregion 
    }
}
