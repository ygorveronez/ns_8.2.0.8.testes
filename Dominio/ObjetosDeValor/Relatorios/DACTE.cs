using Dominio.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class DACTE
    {
        #region Dados CT-e

        public int Codigo { get; set; }

        public string Chave { get; set; }

        public string ChaveCTeSubcontratacaoComplementar { get; set; }

        public byte[] CodigoDeBarras { get; set; }

        public byte[] MarcaDagua { get; set; }

        public byte[] QRCode { get; set; }

        private string _chaveFormatada;
        public string ChaveFormatada
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._chaveFormatada))
                {
                    if (this.Chave != null && this.Chave.Length == 44)
                    {
                        this._chaveFormatada = this.Chave;

                        _chaveFormatada = _chaveFormatada.Insert(4, " ");
                        _chaveFormatada = _chaveFormatada.Insert(9, " ");
                        _chaveFormatada = _chaveFormatada.Insert(14, " ");
                        _chaveFormatada = _chaveFormatada.Insert(19, " ");
                        _chaveFormatada = _chaveFormatada.Insert(24, " ");
                        _chaveFormatada = _chaveFormatada.Insert(29, " ");
                        _chaveFormatada = _chaveFormatada.Insert(34, " ");
                        _chaveFormatada = _chaveFormatada.Insert(39, " ");
                        _chaveFormatada = _chaveFormatada.Insert(44, " ");
                        _chaveFormatada = _chaveFormatada.Insert(49, " ");
                    }
                    else
                    {
                        this._chaveFormatada = string.Empty;
                    }
                }

                return this._chaveFormatada;
            }
        }

        public int Numero { get; set; }

        public int Serie { get; set; }

        public string Modelo { get; set; }

        public string DescricaoModeloFiscal { get; set; }

        public string AbreviacaoModeloFiscal { get; set; }

        public Dominio.Enumeradores.TipoCTE TipoCTe { get; set; }

        public string IndicadorCTeGlobalizado { get; set; }

        public string DescricaoTipoCTe
        {
            get
            {
                switch (this.TipoCTe)
                {
                    case Enumeradores.TipoCTE.Anulacao:
                        return "Anulação";
                    case Enumeradores.TipoCTE.Complemento:
                        return "Complemento";
                    case Enumeradores.TipoCTE.Normal:
                        return "Normal";
                    case Enumeradores.TipoCTE.Substituto:
                        return "Substituição";
                    default:
                        return "";
                }
            }
        }

        public Dominio.Enumeradores.TipoServico TipoServico { get; set; }

        public string DescricaoTipoServico
        {
            get
            {
                switch (this.TipoServico)
                {
                    case Enumeradores.TipoServico.Normal:
                        return "Normal";
                    case Enumeradores.TipoServico.Redespacho:
                        return "Redespacho";
                    case Enumeradores.TipoServico.RedIntermediario:
                        return "Redespacho Intermediário";
                    case Enumeradores.TipoServico.SubContratacao:
                        return "Subcontratação";
                    default:
                        return "";
                }
            }
        }

        public Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }

        public string DescricaoTipoTomador
        {
            get
            {
                switch (this.TipoTomador)
                {
                    case Enumeradores.TipoTomador.Destinatario:
                        return "Destinatário";
                    case Enumeradores.TipoTomador.Expedidor:
                        return "Expedidor";
                    case Enumeradores.TipoTomador.Outros:
                        return "Outros";
                    case Enumeradores.TipoTomador.Recebedor:
                        return "Recebedor";
                    case Enumeradores.TipoTomador.Remetente:
                        return "Remetente";
                    default:
                        return "";
                }
            }
        }

        public TipoPagamento TipoPagamento { get; set; }

        public string DescricaoTipoPagamento
        {
            get { return TipoPagamento.ObterDescricao(); }
        }

        public string CFOP { get; set; }

        public string NaturezaOperacao { get; set; }

        public string ProtocoloAutorizacao { get; set; }

        public DateTime? DataAutorizacao { get; set; }

        public DateTime? DataEmissao { get; set; }

        public string ProdutoPredominante { get; set; }

        public string OutrasCaracteristicasCarga { get; set; }

        public decimal ValorTotalMercadoria { get; set; }

        public bool SuprimirImpostos { get; set; }

        public string CSTICMS { get; set; }

        public string DescricaoCSTICMS
        {
            get
            {
                switch (this.CSTICMS)
                {
                    case "91":
                        return "ICMS Outros";
                    case "90":
                        return "ICMS devido à UF de origem da prestação, quando diferente da UF do emitente";
                    case "51":
                        return "ICMS Diferido";
                    case "40":
                        return "ICMS Isento";
                    case "41":
                        return "ICMS Não Tributado";
                    case "00":
                        return "ICMS Tributado Integralmente";
                    case "60":
                        return "ICMS Cobrado por Substituição Tributária";
                    case "20":
                        return "ICMS com Redução na Base de Cálculo";
                    case "":
                        return "Simples Nacional";
                    default:
                        return "";
                }
            }
        }

        public decimal BaseCalculoICMS { get; set; }

        public decimal AliquotaICMS { get; set; }

        public decimal ValorICMS { get; set; }

        public decimal PercentualReducaoBaseCalculoICMS { get; set; }

        public decimal ValorICMSST { get; set; }

        public decimal ValorTotalServico { get; set; }

        public decimal ValorTotalReceber { get; set; }

        public string Observacoes { get; set; }

        public string Mensagem { get; set; }

        public string Lotacao { get; set; }

        public string FilialEntrega { get; set; }

        public string UFOrigemPrestacao { get; set; }

        public string CidadeOrigemPrestacao { get; set; }

        public string UFDestinoPrestacao { get; set; }

        public string CidadeDestinoPrestacao { get; set; }

        public decimal QuantidadeCarga1 { get; set; }

        public string UnidadeMedida1 { get; set; }

        public decimal QuantidadeCarga2 { get; set; }

        public string UnidadeMedida2 { get; set; }

        public decimal QuantidadeCarga3 { get; set; }

        public string UnidadeMedida3 { get; set; }

        public decimal QuantidadeCarga4 { get; set; }

        public string UnidadeMedida4 { get; set; }

        public string CPFMotorista { get; set; }

        public string NomeMotorista { get; set; }

        public Dominio.Enumeradores.TipoAmbiente Ambiente { get; set; }

        public Dominio.Enumeradores.TipoImpressao TipoImpressao { get; set; }

        public decimal PercentualTributosMunicipal { get; set; }

        public decimal PercentualTributosEstadual { get; set; }

        public decimal PercentualTributosNacional { get; set; }

        public decimal PercentualTributosInternacional { get; set; }

        public decimal ValorICMSUFDestino { get; set; }

        public string Status { get; set; }

        #endregion

        #region Dados Seguro

        public string NomeSeguradora { get; set; }

        public Dominio.Enumeradores.TipoSeguro ResponsavelSeguro { get; set; }

        public string DescricaoResponsavelSeguro
        {
            get
            {
                switch (this.ResponsavelSeguro)
                {
                    case Enumeradores.TipoSeguro.Destinatario:
                        return "DESTINATÁRIO";
                    case Enumeradores.TipoSeguro.Emitente_CTE:
                        return "EMITENTE";
                    case Enumeradores.TipoSeguro.Expedidor:
                        return "EXPEDIDOR";
                    case Enumeradores.TipoSeguro.Recebedor:
                        return "RECEBEDOR";
                    case Enumeradores.TipoSeguro.Remetente:
                        return "REMETENTE";
                    case Enumeradores.TipoSeguro.Tomador_Servico:
                        return "TOMADOR";
                    default:
                        return "";
                }
            }
        }

        public string NumeroApoliceSeguro { get; set; }

        public string NumeroAverbacaoSeguro { get; set; }

        #endregion

        #region Dados Emitente

        public string NomeEmitente { get; set; }

        public string CNPJEmitente { get; set; }

        public string IEEmitente { get; set; }

        public string RNTRCEmitente { get; set; }

        public string LogradouroEmitente { get; set; }

        public string NumeroEmitente { get; set; }

        public string BairroEmitente { get; set; }

        public string CidadeEmitente { get; set; }

        public string UFEmitente { get; set; }

        public string CEPEmitente { get; set; }

        public string TelefoneEmitente { get; set; }

        public int TotalDocumentos { get; set; }

        public byte[] Logo { get; set; }

        #endregion

        #region Dados Remetente

        public string NomeRemetente { get; set; }

        public string LogradouroRemetente { get; set; }

        public string NumeroRemetente { get; set; }

        public string CidadeRemetente { get; set; }

        public string UFRemetente { get; set; }

        public string CPFCNPJRemetente { get; set; }

        public string CEPRemetente { get; set; }

        public string PaisRemetente { get; set; }

        public string IERemetente { get; set; }

        public string TelefoneRemetente { get; set; }

        #endregion

        #region Dados Destinatário

        public string NomeDestinatario { get; set; }

        public string LogradouroDestinatario { get; set; }

        public string NumeroDestinatario { get; set; }

        public string CidadeDestinatario { get; set; }

        public string UFDestinatario { get; set; }

        public string CPFCNPJDestinatario { get; set; }

        public string CEPDestinatario { get; set; }

        public string PaisDestinatario { get; set; }

        public string IEDestinatario { get; set; }

        public string TelefoneDestinatario { get; set; }

        #endregion

        #region Dados Expedidor

        public string NomeExpedidor { get; set; }

        public string LogradouroExpedidor { get; set; }

        public string NumeroExpedidor { get; set; }

        public string CidadeExpedidor { get; set; }

        public string UFExpedidor { get; set; }

        public string CPFCNPJExpedidor { get; set; }

        public string CEPExpedidor { get; set; }

        public string PaisExpedidor { get; set; }

        public string IEExpedidor { get; set; }

        public string TelefoneExpedidor { get; set; }

        #endregion

        #region Dados Recebedor

        public string NomeRecebedor { get; set; }

        public string LogradouroRecebedor { get; set; }

        public string NumeroRecebedor { get; set; }

        public string CidadeRecebedor { get; set; }

        public string UFRecebedor { get; set; }

        public string CPFCNPJRecebedor { get; set; }

        public string CEPRecebedor { get; set; }

        public string PaisRecebedor { get; set; }

        public string IERecebedor { get; set; }

        public string TelefoneRecebedor { get; set; }

        #endregion

        #region Dados Tomador

        public string NomeTomador { get; set; }

        public string LogradouroTomador { get; set; }

        public string NumeroTomador { get; set; }

        public string CidadeTomador { get; set; }

        public string UFTomador { get; set; }

        public string CPFCNPJTomador { get; set; }

        public string CEPTomador { get; set; }

        public string PaisTomador { get; set; }

        public string IETomador { get; set; }

        public string TelefoneTomador { get; set; }

        #endregion

        #region Componentes da Prestação

        public string DescricaoComponentePrestacao1 { get; set; }

        public decimal ValorComponentePrestacao1 { get; set; }

        public string DescricaoComponentePrestacao2 { get; set; }

        public decimal ValorComponentePrestacao2 { get; set; }

        public string DescricaoComponentePrestacao3 { get; set; }

        public decimal ValorComponentePrestacao3 { get; set; }

        public string DescricaoComponentePrestacao4 { get; set; }

        public decimal ValorComponentePrestacao4 { get; set; }

        public string DescricaoComponentePrestacao5 { get; set; }

        public decimal ValorComponentePrestacao5 { get; set; }

        public string DescricaoComponentePrestacao6 { get; set; }

        public decimal ValorComponentePrestacao6 { get; set; }

        public string DescricaoComponentePrestacao7 { get; set; }

        public decimal ValorComponentePrestacao7 { get; set; }

        public string DescricaoComponentePrestacao8 { get; set; }

        public decimal ValorComponentePrestacao8 { get; set; }

        public string DescricaoComponentePrestacao9 { get; set; }

        public decimal ValorComponentePrestacao9 { get; set; }

        #endregion
    }
}
