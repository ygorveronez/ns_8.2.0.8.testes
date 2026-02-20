using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public sealed class FiltroPesquisaRelatorioTitulo
    {
        public TipoTitulo? Tipo { get; set; }
        public List<StatusTitulo> Status { get; set; }
        public TipoDocumentoPesquisaTitulo? TipoDocumento { get; set; }
        public List<double> CnpjPessoas { get; set; }
        public double CnpjPortador { get; set; }
        public int CodigoTipoMovimento { get; set; }
        public int Adiantado { get; set; }
        public int CodigoCTe { get; set; }
        public int CodigoTitulo { get; set; }
        public int CodigoDocumentoEntrada { get; set; }
        public int CodigoFatura { get; set; }
        public string DocumentoOriginal { get; set; }
        public int NumeroDocumentoOriginario { get; set; }
        public int NumeroOcorrencia { get; set; }
        public int CodigoBordero { get; set; }
        public Dominio.Enumeradores.OpcaoSimNao Autorizados { get; set; }
        public TipoBoletoPesquisaTitulo TipoBoleto { get; set; }
        public int CodigoTipoPagamentoRecebimento { get; set; }
        public int CodigoPagamentoEletronico { get; set; }
        public List<int> GruposPessoas { get; set; }
        public DateTime DataInicialEmissao { get; set; }
        public DateTime DataFinalEmissao { get; set; }
        public DateTime DataInicialVencimento { get; set; }
        public DateTime DataFinalVencimento { get; set; }
        public DateTime DataInicialQuitacao { get; set; }
        public DateTime DataFinalQuitacao { get; set; }
        public DateTime DataInicialEmissaoDocumentoEntrada { get; set; }
        public DateTime DataFinalEmissaoDocumentoEntrada { get; set; }
        public DateTime DataInicialCancelamento { get; set; }
        public DateTime DataFinalCancelamento { get; set; }
        public DateTime DataBaseInicial { get; set; }
        public DateTime DataBaseFinal { get; set; }
        public DateTime DataPosicaoFinal { get; set; }
        public decimal ValorInicial { get; set; }
        public decimal ValorFinal { get; set; }
        public bool? NovoModeloFatura { get; set; }
        public string NumeroPedidoCliente { get; set; }
        public string NumeroOcorrenciaCliente { get; set; }
        public List<FormaTitulo> FormaTitulo { get; set; }
        public ProvisaoPesquisaTitulo ProvisaoPesquisaTitulo { get; set; }
        public List<int> CodigosEmpresa { get; set; }
        public Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public int CodigoRemessa { get; set; }
        public int CodigoCheque { get; set; }
        public DateTime DataInicialEntradaDocumentoEntrada { get; set; }
        public DateTime DataFinalEntradaDocumentoEntrada { get; set; }
        public DateTime DataAutorizacaoInicial { get; set; }
        public DateTime DataAutorizacaoFinal { get; set; }
        public DateTime DataProgramacaoPagamentoInicial { get; set; }
        public DateTime DataProgramacaoPagamentoFinal { get; set; }
        public int CodigoCategoria { get; set; }
        public MoedaCotacaoBancoCentral Moeda { get; set; }
        public List<int> ModelosDocumento { get; set; }
        public int CodigoPagamentoMotoristaTipo { get; set; }
        public DateTime DataInicialLancamento { get; set; }
        public DateTime DataFinalLancamento { get; set; }
        public List<int> CodigosTipoMovimento { get; set; }
        public bool VisualizarTitulosPagamentoSalario { get; set; }
        public int CodigoComandoBanco { get; set; }
        public int CodigoVeiculo { get; set; }
        public List<TipoPropostaMultimodal> TipoProposta { get; set; }
        public TituloRenegociado Renegociado { get; set; }
        public List<int> SituacoesContato { get; set; }
        public List<int> TiposContato { get; set; }
    }
}
