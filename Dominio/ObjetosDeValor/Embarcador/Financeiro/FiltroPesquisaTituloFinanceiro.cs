using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaTituloFinanceiro
    {
        public DateTime DataProgramacaoPagamentoInicial { get; set; }

        public DateTime DataProgramacaoPagamentoFinal { get; set; }

        public DateTime DataInicialVencimento { get; set; }

        public DateTime DataFinalVencimento { get; set; }

        public DateTime DataInicialEmissao { get; set; }

        public DateTime DataFinalEmissao { get; set; }

        public DateTime DataBaseLiquidacaoInicial { get; set; }

        public DateTime DataBaseLiquidacaoFinal { get; set; }

        public decimal ValorAte { get; set; }

        public decimal ValorDe { get; set; }

        public int CodigoCategoriaPessoa { get; set; }

        public decimal ValorMovimento { get; set; }

        public decimal ValorPago { get; set; }

        public decimal ValorPagoAte { get; set; }

        public double CodigoPessoa { get; set; }

        public double CodigoPortador { get; set; }

        public int CodigoTitulo { get; set; }

        public int CodigoCTe { get; set; }

        public int CodigoFatura { get; set; }

        public int CodigoGrupoPessoa { get; set; }

        public int NumeroDocumentoOriginario { get; set; }

        public int CodigoVeiculo { get; set; }

        public int CodigoTipoMovimento { get; set; }

        public int CodigoRemessa { get; set; }

        public int Adiantado { get; set; }

        public string NumeroPedido { get; set; }

        public string NumeroOcorrencia { get; set; }

        public string NossoNumero { get; set; }

        public string DocumentoOriginal { get; set; }

        public List<StatusTitulo> StatusTitulo { get; set; }

        public TipoTitulo TipoTitulo { get; set; }

        public FormaTitulo FormaTitulo { get; set; }

        public ProvisaoPesquisaTitulo ProvisaoPesquisaTitulo { get; set; }

        public Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }

        public int CodigoEmpresa { get; set; }

        public TipoBoletoPesquisaTitulo TipoBoleto { get; set; }

        public string NumeroBooking { get; set; }

        public string NumeroOS { get; set; }

        public string NumeroCarga { get; set; }

        public int NumeroNota { get; set; }

        public string NumeroControleCliente { get; set; }

        public string NumeroControle { get; set; }

        public TipoPropostaMultimodal TipoProposta { get; set; }

        public int CodigoTerminalOrigem { get; set; }

        public int CodigoTerminalDestino { get; set; }

        public int CodigoViagem { get; set; }

        public List<TipoPropostaMultimodal> TiposPropostasMultimodal { get; set; }

        public Dominio.Enumeradores.TipoDocumento TipoDocumento { get; set; }

        public MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }

        public string RaizCnpjPessoa { get; set; }

        public bool VisualizarTitulosPagamentoSalario { get; set; }
        
        public TipoDocumentoPesquisaTitulo? TipoDeDocumento { get; set; }

        public bool? StatusEmAberto { get; set; }

        public bool? TipoAPagar { get; set; }
    }
}
