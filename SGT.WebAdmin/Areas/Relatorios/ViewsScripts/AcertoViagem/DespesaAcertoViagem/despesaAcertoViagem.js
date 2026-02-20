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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/AcertoViagem.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Justificativa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Pais.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioDespesaAcertoViagem, _gridDespesaAcertoViagem, _pesquisaDespesaAcertoViagem, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _situacaoPesquisa = [
    { text: "Todos", value: 0 },
    { text: "Em Andamento", value: 1 },
    { text: "Fechados", value: 2 },
    { text: "Cancelados", value: 3 }
];

var PesquisaDespesaAcertoViagem = function () {
    this.DataInicial = PropertyEntity({ text: "Período inicial do acerto: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _situacaoPesquisa, def: 0, text: "Situação: " });

    this.AcertoViagem = PropertyEntity({ text: "Acerto de Viagem:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ text: "Motorista:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ModeloVeiculo = PropertyEntity({ text: "Modelo Veícular:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Justificativa = PropertyEntity({ text: "Justificativa:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.VeiculoTracao = PropertyEntity({ text: "Veículo de Tração:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.VeiculoReboque = PropertyEntity({ text: "Veículo de Reboque:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });
    this.Produto = PropertyEntity({ text: "Produto:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Pais = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "País:", idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridDespesaAcertoViagem.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaDespesaAcertoViagem.Visible.visibleFade()) {
                _pesquisaDespesaAcertoViagem.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaDespesaAcertoViagem.Visible.visibleFade(true);
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

function loadRelatorioDespesaAcertoViagem() {

    _pesquisaDespesaAcertoViagem = new PesquisaDespesaAcertoViagem();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridDespesaAcertoViagem = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/DespesaAcertoViagem/Pesquisa", _pesquisaDespesaAcertoViagem, null, null, 10);
    _gridDespesaAcertoViagem.SetPermitirEdicaoColunas(true);

    _relatorioDespesaAcertoViagem = new RelatorioGlobal("Relatorios/DespesaAcertoViagem/BuscarDadosRelatorio", _gridDespesaAcertoViagem, function () {
        _relatorioDespesaAcertoViagem.loadRelatorio(function () {
            KoBindings(_pesquisaDespesaAcertoViagem, "knockoutPesquisaDespesaAcertoViagem");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaDespesaAcertoViagem");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaDespesaAcertoViagem");

            new BuscarMotoristas(_pesquisaDespesaAcertoViagem.Motorista);
            new BuscarModelosVeicularesCarga(_pesquisaDespesaAcertoViagem.ModeloVeiculo);
            new BuscarVeiculos(_pesquisaDespesaAcertoViagem.VeiculoTracao);
            new BuscarReboques(_pesquisaDespesaAcertoViagem.VeiculoReboque);
            new BuscarProdutoTMS(_pesquisaDespesaAcertoViagem.Produto);
            new BuscarJustificativas(_pesquisaDespesaAcertoViagem.Justificativa);
            new BuscarAcertoViagem(_pesquisaDespesaAcertoViagem.AcertoViagem, RetornoBuscarAcertoViagem);
            new BuscarPaises(_pesquisaDespesaAcertoViagem.Pais);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaDespesaAcertoViagem);
}

function RetornoBuscarAcertoViagem(data) {
    _pesquisaDespesaAcertoViagem.AcertoViagem.codEntity(data.Codigo);
    _pesquisaDespesaAcertoViagem.AcertoViagem.val(data.Numero);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioDespesaAcertoViagem.gerarRelatorio("Relatorios/DespesaAcertoViagem/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioDespesaAcertoViagem.gerarRelatorio("Relatorios/DespesaAcertoViagem/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
