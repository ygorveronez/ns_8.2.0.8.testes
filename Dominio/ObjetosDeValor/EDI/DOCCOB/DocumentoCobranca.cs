using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.DOCCOB
{
    public class DocumentoCobranca
    {
        public int SequenciaGeracaoArquivo { get; set; }
        public string CodigoCentroCusto { get; set; }
        public string CodigoEstabelecimento { get; set; }
        public string CNPJFilialEmissora { get; set; }
        public string FilialEmissora { get; set; }
        public string TipoDocumento { get; set; }
        public string SerieDocumento { get; set; }
        public int NumeroDocumento { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public decimal ValorDocumento { get; set; }
        public string TipoCobranca { get; set; }
        public decimal ValorTotalICMS { get; set; }
        public decimal ValorJurosDiaAtraso { get; set; }
        public DateTime? DataLimitePagamentoComDesconto { get; set; }
        public decimal ValorDesconto { get; set; }
        public string IdentificacaoAgenteCobranca { get; set; }
        public string NumeroAgencia { get; set; }
        public string DigitoVerificadorAgencia { get; set; }
        public string NumeroContaCorrente { get; set; }
        public string DigitoVerificadorContaCorrente { get; set; }
        public string AcaoDocumento { get; set; }
        public string Filler { get; set; }
        public string NomeCliente { get; set; }
        public string TipoFrete { get; set; }
        public string ModalidadeFrete { get; set; }
        public string CodigoDeposito { get; set; }
        public string CodigoTransportadora { get; set; }
        public string CNPJSacado { get; set; }
        public string IESacado { get; set; }
        public string CodigoBanco { get; set; }
        public string DescricaoBanco { get; set; }
        public string NossoNumero { get; set; }
        public string ExistePreFatura { get; set; }
        public long NumeroPreFatura { get; set; }
        public DateTime DataInicialFatura { get; set; }
        public DateTime DataFinalFatura { get; set; }
        public List<ConhecimentoCobranca> Conhecimentos { get; set; }
        public string CNPJRemetente { get; set; }
        public DocumentoCobrancaImposto Imposto { get; set; }

        public string CodigoAlternativoTomador { get; set; }
        public string FilialEmissoraCidade { get; set; }
        public string FilialEmissoraUF { get; set; }
        public string FilialEmissoraNomeFantasia { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa RemetenteDetalhes { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa DestinatarioDetalhes { get; set; }
    }
}
