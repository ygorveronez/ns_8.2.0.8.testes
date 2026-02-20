using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Relatorios;

public enum ReportType
{
    TermoResponsabilidade = 0,
    Fatura = 1,
    CanhotoAvulso = 2,
    ImpressaoProtocolo = 3,
    CargaAgrupadaDivisao = 4,
    CarregamentoRelatorioTroca = 5,
    ChamadoAtendimento = 6,
    RelatorioChamados = 7,
    PdfRelacaoEntrega = 8,
    PdfRelacaoEntregaPorPedido = 9,
    PdfPlanoViagem = 10,
    PdfRelatorioDeEmbarque = 11,
    PdfRelacaoEmbarque = 12,
    PdfDemonstrativoEstadia = 13,
    PdfComprovanteColeta = 14,
    MinutaTransporte = 15,
    PdfDiarioBordo = 16,
    PdfCRT = 17,
    PdfMinutaFrete = 18,
    MicDta = 19,
    ValePedagioPagBem = 20,
    OrdemCompra = 21,
    VeiculosQrCode = 22,
    ProdutoEtiqueta = 23,
    ProdutoEtiquetaCodigoBarra = 24,
    ComprovanteEntrega = 25,
    Dacce = 26,
    CIOT = 27,
    RelatorioContratoFreteAditivo = 28,
    RelatorioContratoFretePadrao = 29,
    RomaneioEntrega = 30,
    OutrosDocumentos = 31,
    RelacaoDocumentosNFSManual = 32,
    ImpressaoProtocoloEntrega = 33,
    EtapaQrCode = 34,
    ResumoAvisoPeriodico = 35,
    Bordero = 36,
    TermoQuitacao = 37,
    ResumoTermoQuitacao = 38,
    ValeAvulso = 39,
    OrdemServico = 40,
    ResumoAgendamentoColeta = 41,
    ResumoAgendamentoColetaSams = 42,
    AnuenciaTransportador = 43,
    AuditoriaDeOs = 44,
    ViaCega = 45,
    SinteseMateriais = 46,
    BoletimViagemEmbarque = 47,
    EtiquetaDeposito = 48,
    BoletimViagem = 49,
    Romaneio = 50,
    TermoAceite = 51,
    TermoAvaria = 52,
    AreaVeiculoPosicoesQrCode = 53,
    PamcardReciboVP = 54,
    ValePedagioSemparar = 55,
    ImpressaoProdutos = 56,
    AtendimentoCliente = 57,
    DiarioBordoSemanal = 58,
    PlanejamentoPedido = 59,
    ComprovanteCargaInformadaJanelaDescarga = 60,
    ComprovanteSaidaGuarita = 61,
    TicketBalanca = 62,
    ComprovanteMontagemCarga = 63,
    CheckListGuarita = 64,
    RomaneioTotalizador = 65,
    RomaneioDetalhado = 66,
    CapaViagem = 67,
    ComprovanteEntregaCanhoto = 68,
    DevolucaoMercadoria = 69,
    FechamentoFrete = 70,
    ConciliacaoBancaria = 71,
    CaixaFuncionario = 72,
    RemessaPagamento = 73,
    RetornoPagamento = 74,
    ReciboInfracao = 75,
    EtiquetaControleVisita = 76,
    RequisicaoMercadoria = 77,
    AcertoFechamento = 78,
    DespesaAcertoViagem = 79,
    GerarEtiquetasVolume = 80,
    VolumeFaltante = 81,
    ComissaoMotoristas = 82,
    OrdemServicoVenda = 83,
    PedidoVenda = 84,
    CotacaoCompra = 85,
    CotacaoPedido = 86,
    PreDacte = 87,
    Francesinha = 88,
    DREGerencial = 89,
    FaturamentoMensalGrafico = 90,
    FluxoCaixa = 91,
    PedidoVendaContrato = 92,
    ReciboPagamentoAgregado = 93,
    ExtratoPagamentoAgregado = 94,
    FaturaPagamentoAgregado = 95,
    EtiquetaVolumeNFe = 96,
    GerarRelatorioDANFE = 97,
    GerarRelatorioCCeNFe = 98,
    PlanoOrcamentario = 99,
    TituloFinanceiro = 100,
    ReciboFinanceiro = 101,
    MovimentacaoDePlacas = 102,
    ImpressaoPedido = 103,
    HistoricoParada = 104,
    FaturaPadrao = 105,
    FaturaImpressaoMultimodal = 106,
    FaturaImpressaoPadrao = 107,
    FaturaImpressaoParcelasSeparadas = 108,
    FaturaImpressaoPorCte = 109,
    FaturaImpressaoPorDocumentos = 110,
    RelacaoEntrega = 111,
    RelacaoSeparacaoVolume = 112,
    RegistroTemperaturaETrocaDeGelo = 113,
    AutorizacaoEmbarqueOpenTech = 114,
    CargaComposicaoFrete = 115,
    DetalhesCarga = 116,
    OrdemColetaGuarita = 117,
    DetalheCTeMDF = 118,
    RecebimentoProduto = 119,
    DivergenciaPreFatura = 120,
    GeracaoEscala = 121,
    EmpresasFaturamento = 122,
    MovimentacaoMotorista = 123,
    EtiquetaControleTacografo = 124,
    TermoRecolhimentoMaterial = 125,
    TermoBaixaMaterial = 126,
    ImpressaoRecibo = 127,
    ImpressaoReciboMotorista = 128,
    DocumentoCargaRiachuelo = 129,
    FichaMotoristaCarga = 130,
    BoletoRetornoArquivo = 131,
    GerarRelatorioEmbarcador = 132,
    ConsultaTabelaFrete = 133,
    ValoresCotacao = 134,
    SimulacaoFrete = 135,
    AjusteTabelaFrete = 136,
    OrdemColeta = 137,
    Dinamic = 138,
    GerarRelatorioGestao = 139,
    ComparativoMensalFaturamentoGrupoPessoas = 140,
    FalhaNumeracaoCTe = 141,
    MagazineLuiza = 142,
    ConfiguracaoSubcontratacaoTabelaFrete = 143,
    CheckList = 144,
    ComprovanteCargaInformada = 145,
    FluxoHorario = 146,
    MovimentacaoPneuVeiculo = 147,
    Repom = 148,
    RomaneioDetalhadoResumido = 149,
    RelacaoPedidoPacoteCarga = 150,
    FaturaChaveCTe = 151,
    ResumoAgendamentoColetaPallet = 152,
    GestaoDevolucaoLaudo = 153,
    ResumoAgendamentoPallet = 154,
    CargaMDFe = 155,
    CCe = 156,
    OrdemServicoPet = 157,
    CheckListMinutaTransporte = 158,
    ValePedagioSempararMDFe = 159,
}

public enum ExecutionType
{
    Async,
    Sync
}

public enum ExecutionStatus
{
    Finished,
    Fail,
    InBackground
}

public class ReportRequest
{
    public ReportType ReportType { get; set; }
    public ExecutionType ExecutionType { get; set; }
    public Dictionary<string, string> ExtraData { get; set; } = new();
    public string ApiKey { get; set; }

    public ReportRequest()
    {
    }

    public static ReportRequest WithType(ReportType reportType, string apiKey = "9f8d29af-f0b5-41ac-81f0-a3fae7a05801")
    {
        return new ReportRequest
        {
            ReportType = reportType,
            ApiKey = apiKey
        };
    }

    public ReportRequest WithExecutionType(ExecutionType executionType)
    {
        ExecutionType = executionType;
        return this;
    }

    public ReportRequest AddExtraData(string key, string value)
    {
        ExtraData.Add(key, value);
        return this;
    }
    public ReportRequest AddExtraData(string key, object value)
    {
        ExtraData.Add(key, value.ToString());
        return this;
    }

}