/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoLiquidacaoRelatorioDocumentoFaturamento.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoDocumentoFaturamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridDocumentoFaturamentos, _pesquisaDocumentoFaturamentos, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioDocumentoFaturamentos;

var _tipoPropriedadeVeiculo = [
    { text: "Todos", value: "" },
    { text: "Próprio", value: "P" },
    { text: "Terceiro", value: "T" }
];

var _tipoFaturamento = [
    { text: "Faturado", value: EnumTipoFaturamentoRelatorioDocumentoFaturamento.Faturado },
    { text: "Em Fatura", value: EnumTipoFaturamentoRelatorioDocumentoFaturamento.EmFatura },
    { text: "Não Faturado", value: EnumTipoFaturamentoRelatorioDocumentoFaturamento.NaoFaturado }
];

var _tipoLiquidacao = [
    { text: "Todos", value: "" },
    { text: "Pendente", value: EnumTipoLiquidacaoRelatorioDocumentoFaturamento.Pendente },
    { text: "Liquidado", value: EnumTipoLiquidacaoRelatorioDocumentoFaturamento.Liquidado }
];

var _tipoSituacao = [
    { text: "Todos", value: "" },
    { text: "Autorizado", value: EnumSituacaoDocumentoFaturamento.Autorizado },
    { text: "Cancelado", value: EnumSituacaoDocumentoFaturamento.Cancelado },
    { text: "Anulado", value: EnumSituacaoDocumentoFaturamento.Anulado }
];

var PesquisaDocumentoFaturamentos = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.DataInicialEmissao = PropertyEntity({ text: "Data Emissão Inicial: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalEmissao = PropertyEntity({ text: "Data Emissão Final: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialEmissao.dateRangeLimit = this.DataFinalEmissao;
    this.DataFinalEmissao.dateRangeInit = this.DataInicialEmissao;

    this.DataAutorizacaoInicial = PropertyEntity({ text: "Data Autorização Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataAutorizacaoFinal = PropertyEntity({ text: "Data Autorização Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataAutorizacaoInicial.dateRangeLimit = this.DataAutorizacaoFinal;
    this.DataAutorizacaoFinal.dateRangeInit = this.DataAutorizacaoInicial;

    this.DataCancelamentoInicial = PropertyEntity({ text: "Data Cancelamento Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataCancelamentoFinal = PropertyEntity({ text: "Data Cancelamento Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataCancelamentoInicial.dateRangeLimit = this.DataCancelamentoFinal;
    this.DataCancelamentoFinal.dateRangeInit = this.DataCancelamentoInicial;

    this.DataAnulacaoInicial = PropertyEntity({ text: "Data Anulação Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataAnulacaoFinal = PropertyEntity({ text: "Data Anulação Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataAnulacaoInicial.dateRangeLimit = this.DataAnulacaoFinal;
    this.DataAnulacaoFinal.dateRangeInit = this.DataAnulacaoInicial;

    this.DataBaseLiquidacaoInicial = PropertyEntity({ text: "Data Base Liquidação Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataBaseLiquidacaoFinal = PropertyEntity({ text: "Data Base Liquidação Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataBaseLiquidacaoInicial.dateRangeLimit = this.DataBaseLiquidacaoFinal;
    this.DataBaseLiquidacaoFinal.dateRangeInit = this.DataBaseLiquidacaoInicial;

    this.DataLiquidacaoInicial = PropertyEntity({ text: "Data Liquidação Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataLiquidacaoFinal = PropertyEntity({ text: "Data Liquidação Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataLiquidacaoInicial.dateRangeLimit = this.DataLiquidacaoFinal;
    this.DataLiquidacaoFinal.dateRangeInit = this.DataLiquidacaoInicial;

    this.NumeroInicial = PropertyEntity({ text: "Nº Inicial: ", getType: typesKnockout.int, visible: ko.observable(true) });
    this.NumeroFinal = PropertyEntity({ text: "Nº Final: ", getType: typesKnockout.int, visible: ko.observable(true) });
    this.NumeroFatura = PropertyEntity({ text: "Nº Fatura: ", getType: typesKnockout.int });
    this.NumeroPedidoCliente = PropertyEntity({ text: "Nº Pedido Cliente:" });
    this.NumeroOcorrenciaCliente = PropertyEntity({ text: "Nº Ocorrência Cliente:" });
    this.NumeroOcorrencia = PropertyEntity({ text: "Nº Ocorrência:", getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: false, thousands: '' } });
    this.NumeroDocumentoOriginario = PropertyEntity({ text: "Nº Doc. Originário: ", getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: false, thousands: '' } });

    this.ValorInicial = PropertyEntity({ text: "Valor Inicial: ", getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.ValorFinal = PropertyEntity({ text: "Valor Final: ", getType: typesKnockout.decimal, visible: ko.observable(true) });

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "Veiculo:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });
    this.Filial = PropertyEntity({ text: "Filial:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Transportador = PropertyEntity({ text: "Empresa/Filial:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid(), issue: 16, visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid(), issue: 16, visible: ko.observable(true) });
    this.EstadoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado de Origem:", idBtnSearch: guid(), issue: 12, visible: ko.observable(true) });
    this.EstadoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado de Destino:", idBtnSearch: guid(), issue: 12, visible: ko.observable(true) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoPropriedadeVeiculo = PropertyEntity({ val: ko.observable(""), options: _tipoPropriedadeVeiculo, def: "", text: "Tipo de Propriedade:", visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });
    this.GruposPessoas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Pessoas:", issue: 58, idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOcorrencia = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Ocorrência:",  idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Mod. Documento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.GruposPessoasDiferente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Pessoas Diferente de:", issue: 58, idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoFaturamento = PropertyEntity({ getType: typesKnockout.selectMultiple, text: "Faturamento:", options: _tipoFaturamento, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação:", options: _tipoSituacao, val: ko.observable(EnumSituacaoDocumentoFaturamento.Autorizado.toString()), def: EnumSituacaoDocumentoFaturamento.Autorizado.toString(), issue: 0, visible: ko.observable(true) });
    this.TipoLiquidacao = PropertyEntity({ text: "Liquidação:", options: _tipoLiquidacao, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.DocumentoComCanhotosRecebidos = PropertyEntity({ text: "Doc. com canhotos recebidos:", options: Global.ObterOpcoesPesquisaBooleano("Sim", "Não"), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.DocumentoComCanhotosDigitalizados = PropertyEntity({ text: "Doc. com canhotos digitalizados:", options: Global.ObterOpcoesPesquisaBooleano("Sim", "Não"), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.TipoCTe = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoCTe.ObterOpcoes(), text: "Tipo do CT-e:" });
    this.TipoServico = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoServicoCTe.obterOpcoes(), text: "Tipo de Serviço:" });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentoFaturamentos.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaDocumentoFaturamentos.Visible.visibleFade()) {
                _pesquisaDocumentoFaturamentos.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaDocumentoFaturamentos.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioDocumentoFaturamentos() {
    _pesquisaDocumentoFaturamentos = new PesquisaDocumentoFaturamentos();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridDocumentoFaturamentos = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/DocumentoFaturamento/Pesquisa", _pesquisaDocumentoFaturamentos);

    _gridDocumentoFaturamentos.SetPermitirEdicaoColunas(true);
    _gridDocumentoFaturamentos.SetQuantidadeLinhasPorPagina(10);

    _relatorioDocumentoFaturamentos = new RelatorioGlobal("Relatorios/DocumentoFaturamento/BuscarDadosRelatorio", _gridDocumentoFaturamentos, function () {
        _relatorioDocumentoFaturamentos.loadRelatorio(function () {
            KoBindings(_pesquisaDocumentoFaturamentos, "knockoutPesquisaDocumentoFaturamentos", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaDocumentoFaturamentos", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaDocumentoFaturamento", false);

            new BuscarClientes(_pesquisaDocumentoFaturamentos.Remetente);
            new BuscarClientes(_pesquisaDocumentoFaturamentos.Destinatario);
            new BuscarClientes(_pesquisaDocumentoFaturamentos.Tomador);
            new BuscarTransportadores(_pesquisaDocumentoFaturamentos.Transportador);
            new BuscarLocalidades(_pesquisaDocumentoFaturamentos.Origem);
            new BuscarLocalidades(_pesquisaDocumentoFaturamentos.Destino);
            new BuscarEstados(_pesquisaDocumentoFaturamentos.EstadoOrigem);
            new BuscarEstados(_pesquisaDocumentoFaturamentos.EstadoDestino);
            new BuscarVeiculos(_pesquisaDocumentoFaturamentos.Veiculo);
            new BuscarFilial(_pesquisaDocumentoFaturamentos.Filial);
            new BuscarModeloDocumentoFiscal(_pesquisaDocumentoFaturamentos.ModeloDocumento, null, null, null, null, true);
            new BuscarGruposPessoas(_pesquisaDocumentoFaturamentos.GruposPessoas);
            new BuscarTipoOcorrencia(_pesquisaDocumentoFaturamentos.TipoOcorrencia);
            new BuscarGruposPessoas(_pesquisaDocumentoFaturamentos.GruposPessoasDiferente);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaDocumentoFaturamentos);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioDocumentoFaturamentos.gerarRelatorio("Relatorios/DocumentoFaturamento/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioDocumentoFaturamentos.gerarRelatorio("Relatorios/DocumentoFaturamento/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}