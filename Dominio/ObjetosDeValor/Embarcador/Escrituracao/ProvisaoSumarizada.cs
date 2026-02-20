using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class ProvisaoSumarizada
    {
        public int Codigo { get; set; }
        public double? Tomador { get; set; }
        public double? Expedidor { get; set; }
        public double? Recebedor { get; set; }
        public int? GrupoTomador { get; set; }
        public double? Remetente { get; set; }
        public int? GrupoRemetente { get; set; }
        public double? Destinatario { get; set; }
        public int? GrupoDestinatario { get; set; }
        public int? Origem { get; set; }
        public int? Destino { get; set; }
        public decimal? PesoBruto { get; set; }
        public int? TipoOperacao { get; set; }
        public int? Empresa { get; set; }
        public int? XMLNotaFiscal { get; set; }
        public int? CTeTerceiro { get; set; }
        public int? PedidoCTeParaSubContratacao { get; set; }
        public int? ModeloDocumentoFiscal { get; set; }
        public int? TipoOcorrencia { get; set; }
        public int? RotaFrete { get; set; }
        public int? Stage { get; set; }
        public int? ImpostoValorAgregado { get; set; }
        public string CodigoImpostoValorAgregado { get; set; }
        public Dominio.Enumeradores.TipoDocumento? TipoDocumentoEmissao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento Situacao { get; set; }
        public int NumeroDocumento { get; set; }
        public int SerieDocumento { get; set; }
        public DateTime? DataEmissao { get; set; }
        public int? Carga { get; set; }
        public int? Filial { get; set; }
        public int? CargaOcorrencia { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal PercentualAliquota { get; set; }
        public string CST { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal BaseCalculoISS { get; set; }
        public decimal PercentualAliquotaISS { get; set; }
        public decimal ValorRetencaoISS { get; set; }
        public bool ICMSInclusoBC { get; set; }
        public bool ISSInclusoBC { get; set; }
        public decimal ValorAdValorem { get; set; }
        public decimal ValorDescarga { get; set; }
        public decimal ValorPedagio { get; set; }
        public decimal ValorGris { get; set; }
        public decimal ValorEntrega { get; set; }
        public decimal ValorPernoite { get; set; }
        public decimal ValorContratoFrete { get; set; }
        public decimal ValorFrete { get; set; }
        public Enumeradores.TipoValorFreteDocumentoProvisao TipoValorFrete { get; set; }
        public decimal ValorDesconto { get; set; }

        #region Imposto IBS/CBS

        public Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas OutrasAliquotas { get; set; }
        public string CSTIBSCBS { get; set; }
        public string ClassificacaoTributariaIBSCBS { get; set; }
        public decimal BaseCalculoIBSCBS { get; set; }
        public decimal AliquotaIBSEstadual { get; set; }
        public decimal PercentualReducaoIBSEstadual { get; set; }
        public decimal ValorIBSEstadual { get; set; }
        public decimal AliquotaIBSMunicipal { get; set; }
        public decimal PercentualReducaoIBSMunicipal { get; set; }
        public decimal ValorIBSMunicipal { get; set; }
        public decimal AliquotaCBS { get; set; }
        public decimal PercentualReducaoCBS { get; set; }
        public decimal ValorCBS { get; set; }

        #endregion Imposto CBS/IBS
    }
}
