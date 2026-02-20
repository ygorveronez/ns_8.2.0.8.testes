/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Estado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Pais.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoAbastecimento.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoAbastecimentoInternoExterno.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoAbastecimentoAcertoViagem.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoRecebimentoAbastecimento.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoAbastecimento.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridAbastecimento, _pesquisaAbastecimento, _CRUDRelatorio, _relatorioAbastecimento, _CRUDFiltrosRelatorio;

var _tipoAbastecimentoInternoExterno = [
    { text: "Todos", value: EnumTipoAbastecimentoInternoExterno.Todos },
    { text: "Interno", value: EnumTipoAbastecimentoInternoExterno.Interno },
    { text: "Externo", value: EnumTipoAbastecimentoInternoExterno.Externo }
];

var _situacaoAbastecimentoAcertoViagem = [
    { text: "Todos", value: EnumSituacaoAbastecimentoAcertoViagem.Todos },
    { text: "Em Acerto", value: EnumSituacaoAbastecimentoAcertoViagem.EmAcerto },
    { text: "Não Utilizado em Acerto", value: EnumSituacaoAbastecimentoAcertoViagem.NaoConstaEmAcerto }
];

var _tipoPropriedade = [
    { text: "Todos", value: "0" },
    { text: "Própria", value: "P" },
    { text: "Terceiros", value: "T" }
];

var PesquisaAbastecimento = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.dateTime, val: ko.observable(""), def: "" });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.dateTime, val: ko.observable(""), def: "" });

    this.Status = PropertyEntity({ val: ko.observable(EnumSituacaoAbastecimento.Todos), options: EnumSituacaoAbastecimento.obterOpcoesPesquisa(), def: EnumSituacaoAbastecimento.Todos, text: "Status: " });
    this.TipoAbastecimentoInternoExterno = PropertyEntity({ val: ko.observable(EnumTipoAbastecimentoInternoExterno.Todos), options: _tipoAbastecimentoInternoExterno, def: EnumTipoAbastecimentoInternoExterno.Todos, text: "Tipo do Abastecimento Interno/Externo: " });
    this.TipoRecebimentoAbastecimento = PropertyEntity({ text: "Tipo do Recebimento:", val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoRecebimentoAbastecimento.obterOpcoes() });
    this.TipoAbastecimento = PropertyEntity({ val: ko.observable(EnumTipoAbastecimento.Todos), options: EnumTipoAbastecimento.obterOpcoesPesquisa(), def: EnumTipoAbastecimento.Todos, text: "Tipo Abastecimento: ", enable: ko.observable(true) });
    this.SituacaoAcertoViagem = PropertyEntity({ val: ko.observable(EnumSituacaoAbastecimentoAcertoViagem.Todos), options: _situacaoAbastecimentoAcertoViagem, def: EnumSituacaoAbastecimentoAcertoViagem.Todos, text: "Situação em Acerto: " });
    this.TipoPropriedade = PropertyEntity({ val: ko.observable("0"), options: _tipoPropriedade, def: "0", text: "Tipo da Propriedade: " });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Produtos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Combustível:", idBtnSearch: guid(), issue: 0 });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Segmento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Segmento do Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ProprietarioVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Proprietário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid() });
    this.UFFornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "UF Fornecedor:", idBtnSearch: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid() });
    this.Pais = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "País:", idBtnSearch: guid() });
    this.Moeda = PropertyEntity({ getType: typesKnockout.selectMultiple, options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), text: "Moeda:", val: ko.observable(new Array()), def: new Array() });
    this.DataBaseCRTInicial = PropertyEntity({ text: "Data base CRT Inicial: ", getType: typesKnockout.dateTime, val: ko.observable(""), def: "" });
    this.DataBaseCRTFinal = PropertyEntity({ text: "Data base CRT Final: ", getType: typesKnockout.dateTime, val: ko.observable(""), def: "" });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local de Armazenamento:", idBtnSearch: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.RequisicaoOC = PropertyEntity({ text: "Requisição (O.C.)", maxlength: 700 });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAbastecimento.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaAbastecimento.Visible.visibleFade()) {
                _pesquisaAbastecimento.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaAbastecimento.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioAbastecimento() {

    _pesquisaAbastecimento = new PesquisaAbastecimento();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridAbastecimento = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/Abastecimento/Pesquisa", _pesquisaAbastecimento, null, null, 30);
    _gridAbastecimento.SetPermitirEdicaoColunas(true);

    _relatorioAbastecimento = new RelatorioGlobal("Relatorios/Abastecimento/BuscarDadosRelatorio", _gridAbastecimento, function () {
        _relatorioAbastecimento.loadRelatorio(function () {
            KoBindings(_pesquisaAbastecimento, "knockoutPesquisaAbastecimento");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaAbastecimento");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaAbastecimento");

            new BuscarClientes(_pesquisaAbastecimento.Fornecedor);
            new BuscarClientes(_pesquisaAbastecimento.ProprietarioVeiculo);
            new BuscarVeiculos(_pesquisaAbastecimento.Veiculo);
            new BuscarSegmentoVeiculo(_pesquisaAbastecimento.Segmento);
            new BuscarTipoCombustiveisTMS(_pesquisaAbastecimento.Produtos, null, true);
            new BuscarMotoristas(_pesquisaAbastecimento.Motorista);
            new BuscarEquipamentos(_pesquisaAbastecimento.Equipamento);
            new BuscarGruposPessoas(_pesquisaAbastecimento.GrupoPessoas);
            new BuscarCentroResultado(_pesquisaAbastecimento.CentroResultado);
            new BuscarEstados(_pesquisaAbastecimento.UFFornecedor);
            new BuscarModelosVeicularesCarga(_pesquisaAbastecimento.ModeloVeicularCarga);
            new BuscarPaises(_pesquisaAbastecimento.Pais);
            new BuscarLocalArmazenamentoProduto(_pesquisaAbastecimento.LocalArmazenamento);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAbastecimento);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioAbastecimento.gerarRelatorio("Relatorios/Abastecimento/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioAbastecimento.gerarRelatorio("Relatorios/Abastecimento/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
