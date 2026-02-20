using System;
using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class FiltroPesquisaCteRelatorio
    {
        public List<int> gruposPessoasDiferente { get; set; }
        public double cpfCnpjTerceiro { get; set; }
        public List<int> modeloDocumento { get; set; }
        public List<int> codigosFilial { get; set; }
        public List<int> codigosFilialVenda { get; set; }
        public string nfe { get; set; }
        public bool exibirNotasFiscais { get; set; }
        public bool AnuladoGerencialmente { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public int codigoContratoFrete { get; set; }
        public List<int> codigoCarga { get; set; }
        public string preCarga { get; set; }
        public string pedido { get; set; }
        public List<int> codigosTipoOperacao { get; set; }
        public int codigoOrigem { get; set; }
        public int codigoDestino { get; set; }
        public string estadoOrigem { get; set; }
        public string estadoDestino { get; set; }
        public double cpfCnpjRemetente { get; set; }
        public double cpfCnpjDestinatario { get; set; }
        public List<double> cpfCnpjDestinatarios { get; set; }
        public List<double> CpfCnpjTomadores { get; set; }
        public List<string> statusCTe { get; set; }
        public DateTime dataInicialEmissao { get; set; }
        public DateTime dataFinalEmissao { get; set; }
        public DateTime dataInicialAutorizacao { get; set; }
        public DateTime dataFinalAutorizacao { get; set; }
        public DateTime dataInicialCancelamento { get; set; }
        public DateTime dataFinalCancelamento { get; set; }
        public DateTime dataInicialAnulacao { get; set; }
        public DateTime dataFinalAnulacao { get; set; }
        public DateTime dataInicialImportacao { get; set; }
        public DateTime dataFinalImportacao { get; set; }
        public bool ctesNaoExistentesEmMinutas { get; set; }
        public bool ctesNaoExistentesEmFaturas { get; set; }
        public List<int> tiposServicos { get; set; }
        public List<int> tiposTomadores { get; set; }
        public List<int> gruposPessoas { get; set; }
        public Dominio.Entidades.Cliente transportadorTerceiro { get; set; }
        public List<string> placasVeiculos { get; set; }
        public int numeroInicial { get; set; }
        public int numeroFinal { get; set; }
        public bool? pago { get; set; }
        public string tipoPropriedadeVeiculo { get; set; }
        public int serie { get; set; }
        public bool? cteVinculadoACarga { get; set; }
        public bool? cargaEmissaoFinalizada { get; set; }
        public DateTime dataInicialFatura { get; set; }
        public DateTime dataFinalFatura { get; set; }
        public SituacaoFatura? situacaoFatura { get; set; }
        public bool? faturado { get; set; }
        public int codigoCFOP { get; set; }
        public TipoDocumentoCreditoDebito tipoDocumentoCreditoDebito { get; set; }
        public DateTime dataInicialEntrega { get; set; }
        public DateTime dataFinalEntrega { get; set; }
        public bool? possuiDataEntrega { get; set; }
        public List<int> codigosTipoCarga { get; set; }
        public bool? possuiNFSManual { get; set; }
        public List<TipoICMS> CST { get; set; }
        public List<Dominio.Enumeradores.TipoCTE> tiposCTe { get; set; }
        public List<int> SegmentoVeiculo { get; set; }
        public List<TipoPropostaMultimodal> TipoProposta { get; set; }
        public List<TipoServicoMultimodal> TipoServicoMultimodal { get; set; }
        public Dominio.Enumeradores.OpcaoSimNaoPesquisa VeioPorImportacao { get; set; }
        public bool SomenteCTeSubstituido { get; set; }
        public bool ApenasCTeEnviadoMercante { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroOS { get; set; }
        public string NumeroControle { get; set; }
        public SituacaoCarga SituacaoCarga { get; set; }
        public List<SituacaoCargaMercante> SituacoesCargaMercante { get; set; }
        public int CodigoPortoOrigem { get; set; }
        public int CodigoPortoDestino { get; set; }
        public int CodigoViagem { get; set; }
        public List<int> CodigosCentroResultado { get; set; }
        public List<int> CodigosTipoOcorrencia { get; set; }
        public int CodigoContainer { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
        public int CodigoMotorista { get; set; }
        public bool NaoExibirValorFreteCTeComplementar { get; set; }
        public DateTime DataInicialColeta { get; set; }
        public DateTime DataFinalColeta { get; set; }
        public List<int> GruposPessoasRemetente { get; set; }
        public DateTime DataPagamentoInicial { get; set; }
        public DateTime DataPagamentoFinal { get; set; }
        public DateTime DataVencimentoInicial { get; set; }
        public DateTime DataVencimentoFinal { get; set; }
        public string ChaveCTe { get; set; }
        public string NumeroDocumentoRecebedor { get; set; }
        public string NumeroPedidoCliente { get; set; }
        public List<int> codigosCTes { get; set; }
        public int ModeloVeiculo { get; set; }
        public TipoCarroceria? TipoCarroceria { get; set; }
        public DateTime DataConfirmacaoDocumentosInicial { get; set; }
        public DateTime DataConfirmacaoDocumentosFinal { get; set; }
        public Dominio.Enumeradores.TipoProprietarioVeiculo? TipoProprietarioVeiculo { get; set; }
        public TipoModal TipoModal { get; set; }
        public bool? PermiteGerarFaturamento { get; set; }
        public int CodigoContratoFreteTerceiro { get; set; }
        public List<int> FuncionarioResponsavel { get; set; }
        public List<int> Vendedor { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<int> CodigosFiliaisVenda { get; set; }
        public List<double> CodigosRecebedores { get; set; }
        public List<TipoOSConvertido> TipoOSConvertido { get; set; }
        public List<TipoOS> TipoOS { get; set; }
        public double ProvedorOS { get; set; }
        public int CentroDeCustoViagemCodigo { get; set; }
        public string CentroDeCustoViagemDescricao { get; set; }
        public bool CNPJDivergenteCTeMDFe { get; set; }
        public TipoEmissao TipoEmissao { get; set; }
        public bool RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes { get; set; }
        public string NumeroCRT { get; set; }
        public string MicDTA { get; set; }

    }
}
