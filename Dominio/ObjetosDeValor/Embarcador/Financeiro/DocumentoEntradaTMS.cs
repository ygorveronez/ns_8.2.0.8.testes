using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class DocumentoEntradaTMS
    {
        public EspecieDocumentoFiscal Especie { get; set; }
        public NotaFiscal.ModeloDocumentoFiscal Modelo { get; set; }
        public Cliente Fornecedor { get; set; }
        public SituacaoLancamentoDocumentoEntrada SituacaoLancamentoDocumentoEntrada { get; set; }
        public OrdemCompra OrdemCompra { get; set; }
        public Veiculo Veiculo { get; set; }
        public CFOP CFOP { get; set; }
        public NaturezaDaOperacao NaturezaOperacao { get; set; }
        public Pessoas.Empresa Destinatario { get; set; }
        public Financeiro.TipoMovimento TipoMovimento { get; set; }
        public Frota.OrdemServicoFrota OrdemServico { get; set; }
        public string Chave { get; set; }
        public int NumeroLancamento { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime DataEmissao { get; set; }
        public int NumeroOLD { get; set; }
        public Int64 Numero { get; set; }
        public string Serie { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal ValorTotalICMS { get; set; }
        public decimal BaseCalculoICMSST { get; set; }
        public decimal ValorTotalICMSST { get; set; }
        public decimal ValorTotalIPI { get; set; }
        public decimal ValorTotalPIS { get; set; }
        public decimal ValorTotalCOFINS { get; set; }
        public decimal ValorTotalCreditoPresumido { get; set; }
        public decimal ValorTotalDiferencial { get; set; }
        public decimal ValorTotalDesconto { get; set; }
        public decimal ValorTotalOutrasDespesas { get; set; }
        public decimal ValorTotalFrete { get; set; }
        public decimal ValorTotalSeguro { get; set; }
        public decimal ValorTotalFreteFora { get; set; }
        public decimal ValorTotalOutrasDespesasFora { get; set; }
        public decimal ValorTotalDescontoFora { get; set; }
        public decimal ValorTotalImpostosFora { get; set; }
        public decimal ValorTotalDiferencialFreteFora { get; set; }
        public decimal ValorTotalICMSFreteFora { get; set; }
        public decimal ValorTotalCusto { get; set; }
        public decimal ValorTotalRetencaoPIS { get; set; }
        public decimal ValorTotalRetencaoCOFINS { get; set; }
        public decimal ValorTotalRetencaoINSS { get; set; }
        public decimal ValorTotalRetencaoIPI { get; set; }
        public decimal ValorTotalRetencaoCSLL { get; set; }
        public decimal ValorTotalRetencaoOutras { get; set; }
        public decimal ValorTotalRetencaoIR { get; set; }
        public decimal ValorTotalRetencaoISS { get; set; }
        public decimal ValorProdutos { get; set; }
        public decimal ValorBruto { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada IndicadorPagamento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada Situacao { get; set; }
        public bool? DocumentoEmEBS { get; set; }
        public DateTime? DataAbastecimento { get; set; }
        public int KMAbastecimento { get; set; }
        public decimal BaseSTRetido { get; set; }
        public decimal ValorSTRetido { get; set; }
        public DateTime? DataFinalizacao { get; set; }
        public bool? EncerrarOrdemServico { get; set; }
        public int Horimetro { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Veiculos.Equipamento Equipamento { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }
        public DateTime? DataBaseCRT { get; set; }
        public decimal ValorMoedaCotacao { get; set; }
        public string ChaveNotaAnulacao { get; set; }
        public string Observacao { get; set; }
        public Usuarios.Usuario OperadorLancamentoDocumento { get; set; }
        public Usuarios.Usuario OperadorFinalizaDocumento { get; set; }
        public Cliente Expedidor { get; set; }
        public Cliente Recebedor { get; set; }
        public Financeiro.ContratoFinanciamento ContratoFinanciamento { get; set; }
        public Dominio.Enumeradores.ModalidadeFrete? TipoFrete { get; set; }
        public Dominio.ObjetosDeValor.Localidade LocalidadeInicioPrestacao { get; set; }
        public Dominio.ObjetosDeValor.Localidade LocalidadeTerminoPrestacao { get; set; }
        public bool DocumentoFinalizadoAutomaticamente { get; set; }
        public string Motivo { get; set; }
        public NotaFiscal.Servico Servico { get; set; }
        public Dominio.ObjetosDeValor.Localidade LocalidadePrestacaoServico { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoServico? TipoDocumento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTServico? CSTServico { get; set; }
        public decimal AliquotaSimplesNacional { get; set; }
        public bool DocumentoFiscalProvenienteSimplesNacional { get; set; }
        public bool TributaISSNoMunicipio { get; set; }
        public bool Integrado { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntradaItem> Itens { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntradaDuplicata> Duplicatas { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntradaCentroResultado> CentrosResultados { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa> CentrosResultadosTipoDespesa { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Veiculos.Veiculo> Veiculos { get; set; }
    }
}
