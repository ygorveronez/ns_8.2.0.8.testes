using Dominio.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class DocumentoFaturamento
    {
        //public int Codigo { get; set; }
        public string Filial { get; set; }
        public string Numero { get; set; }
        public string NumeroCarga { get; set; }
        public int NumeroOcorrencia { get; set; }
        public string NumeroPedidoCliente { get; set; }
        public string NumeroOcorrenciaCliente { get; set; }
        public string DataEmissaoDocumentoOriginario { get; set; }
        public int NumeroDocumentoOriginario { get; set; }
        public int Serie { get; set; }
        public string Tomador { get; set; }
        public double CNPJTomador { get; set; }
        public string TipoTomador { get; set; }
        public int CodigoDocumento { get; set; }
        public string CPFCNPJTomadorFormatado
        {
            get
            {
                if (TipoTomador == "E")
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    return TipoTomador == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJTomador) : string.Format(@"{0:000\.000\.000\-00}", CNPJTomador);
                }
            }
        }
        public string GrupoTomador { get; set; }
        public string CidadeTomador { get; set; }
        public string CNPJEmpresa { get; set; }
        public string CNPJEmpresaFormatado
        {
            get
            {
                if (long.TryParse(CNPJEmpresa, out long lCNPJEmpresa))
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", lCNPJEmpresa);
                else
                    return string.Empty;
            }
        }
        public string Empresa { get; set; }
        public double CNPJRemetente { get; set; }
        public string TipoRemetente { get; set; }
        public string CPFCNPJRemetenteFormatado
        {
            get
            {
                if (TipoRemetente == "E")
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    if (CNPJRemetente > 0D)
                        return TipoRemetente == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJRemetente) : string.Format(@"{0:000\.000\.000\-00}", CNPJRemetente);
                    else
                        return string.Empty;
                }
            }
        }
        public string Remetente { get; set; }
        public double CNPJDestinatario { get; set; }
        public string TipoDestinatario { get; set; }
        public string CPFCNPJDestinatarioFormatado
        {
            get
            {
                if (TipoDestinatario == "E")
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    if (CNPJDestinatario > 0D)
                        return TipoDestinatario == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJDestinatario) : string.Format(@"{0:000\.000\.000\-00}", CNPJDestinatario);
                    else
                        return string.Empty;
                }
            }
        }
        public string Destinatario { get; set; }
        public string Expedidor { get; set; }
        public double CNPJExpedidor { get; set; }
        public string TipoExpedidor { get; set; }
        public string CPFCNPJExpedidorFormatado
        {
            get
            {
                if (TipoExpedidor == "E")
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    if (CNPJExpedidor > 0D)
                        return TipoExpedidor == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJExpedidor) : string.Format(@"{0:000\.000\.000\-00}", CNPJExpedidor);
                    else
                        return string.Empty;
                }
            }
        }
        public string Recebedor { get; set; }
        public double CNPJRecebedor { get; set; }
        public string TipoRecebedor { get; set; }
        public string CPFCNPJRecebedorFormatado
        {
            get
            {
                if (TipoRecebedor == "E")
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    if (CNPJRecebedor > 0D)
                        return TipoRecebedor == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJRecebedor) : string.Format(@"{0:000\.000\.000\-00}", CNPJRecebedor);
                    else
                        return string.Empty;
                }
            }
        }
        //public decimal ValorReceber { get; set; }
        //public string ProprioTerceiro { get; set; }
        public string Frotas { get; set; }
        public string Placas { get; set; }
        public string Motoristas { get; set; }
        public DateTime DataEmissao { get; set; }
        //public string Notas { get; set; }
        public string Origem { get; set; }
        public string UFOrigem { get; set; }
        public string Destino { get; set; }
        public string UFDestino { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento TipoDocumento { get; set; }
        public string AbreviacaoModeloDocumentoFiscal { get; set; }
        public decimal ValorDocumento { get; set; }
        public decimal ValorAcrescimo { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorEmFatura { get; set; }
        public decimal ValorAFaturar { get; set; }
        public string Faturas { get; set; }
        public string VencimentosFaturas { get; set; }
        public string EmissoesFaturas { get; set; }
        public string DataAutorizacao { get; set; }
        public string DataCancelamento { get; set; }
        public string DataAnulacao { get; set; }
        public string DataEnvioUltimoCanhoto { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento Situacao { get; set; }

        public decimal AliquotaICMS { get; set; }
        public decimal AliquotaISS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal ValorImpostos { get; set; }

        public string CanhotosRecebidos { get; set; }
        public string CanhotosDigitalizados { get; set; }

        public string Observacao { get; set; }

        public string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Autorizado:
                        return "Autorizado";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Cancelado:
                        return "Cancelado";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Anulado:
                        return "Anulado";
                    default:
                        return "";
                }
            }
        }
        public string DescricaoAbreviacao
        {
            get
            {
                if (TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe)
                {
                    return AbreviacaoModeloDocumentoFiscal;
                }
                else
                {
                    return "Carga";
                }
            }
        }
        public string ChaveAcesso { get; set; }
        public string NumeroTitulos { get; set; }
        public string StatusTitulos { get; set; }
        public decimal ValorPendente { get; set; }
        public string DataLiquidacao { get; set; }
        public TipoCTE TipoCTe { get; set; }
        public string DescricaoTipoCTe 
        { 
            get { return TipoCTe.ObterDescricao(); }
        }
        public TipoServico TipoServico { get; set; }
        public string DescricaoTipoServico 
        { 
            get { return TipoServico.ObterDescricao(); }
        }

        public string DataBaseLiquidacao { get; set; }

        public string TipoOcorrencia { get; set; }

        public string ObservacaoFatura { get; set; }
        public string NumeroNotaFiscal { get; set; }
        public string NumeroDocumentoAnterior { get; set; }
        public string ValoresPagos { get; set; }

        public string DataVencimentoTitulo{ get; set; }
        public string TipoOperacao { get; set; }
    }
}
