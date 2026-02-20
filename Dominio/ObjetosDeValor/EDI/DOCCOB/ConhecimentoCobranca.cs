using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.DOCCOB
{
    public class ConhecimentoCobranca
    {
        public string FilialEmissora { get; set; }
        public string FilialEmissoraCidade { get; set; }
        public string FilialEmissoraUF { get; set; }
        public string FilialEmissoraNomeFantasia { get; set; }
        public string CNPJFilialEmissora { get; set; }
        public string CodigoCentroCusto { get; set; }
        public string CodigoEstabelecimento { get; set; }
        public string CodigoAlternativoTomador { get; set; }
        public string SerieConhecimento { get; set; }
        public string NumeroConhecimento { get; set; }
        public decimal ValorFrete { get; set; }
        public DateTime DataEmissao { get; set; }
        public int NumeroFatura { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal AliquotaICMS { get; set; }
        public string Complemento { get; set; }
        public decimal FretePeso { get; set; }
        public decimal FreteValor { get; set; }
        public decimal Peso { get; set; }
        public decimal PesoDensidadeCubagem { get; set; }
        public decimal ValorMercadoria { get; set; }
        public decimal Generalidades { get; set; }
        public string CondicaoFrete { get; set; }
        public decimal ValorICMS { get; set; }
        public string SubstituicaoTributaria { get; set; }
        public decimal BaseCalculoApuracaoPISCOFINS { get; set; }
        public string ModeloDocumentoFiscal { get; set; }
        public string AbreviacaoModeloDocumentoFiscal { get; set; }
        public string SituacaoDocumentoFiscal { get; set; }
        public string ChaveCTe { get; set; }
        public string TipoCTe { get; set; }
        public string ChaveCTeReferenciaComplementado { get; set; }
        public string NumeroRomaneio { get; set; }
        public string NumeroPedido { get; set; }
        public string ProtocoloCliente { get; set; }
        public string NumeroOS { get; set; }
        public string NumeroContainer { get; set; }
        public string TipoContainer { get; set; }
        public decimal ValorADValorem { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Remetente { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Emitente { get; set; }
        public string Filler { get; set; }
        public List<NotaFiscalCobrancaConhecimento> NotasFiscais { get; set; }

        public decimal ValorADValoremComICMS { get; set; }
        public decimal ValorFreteSemICMS { get; set; }
    }
}
