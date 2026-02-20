/// <reference path="../../../../../ViewsScripts/Consultas/AcertoViagem.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioAcertoDeViagem, _gridAcertoDeViagem, _pesquisaAcertoDeViagem, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _situacaoPesquisa = [{ text: "Todos", value: 0 },
{ text: "Em Andamento", value: 1 },
{ text: "Fechados", value: 2 },
{ text: "Cancelados", value: 3 }];

var _tipoMotorista = [
    { text: "Todos", value: 0 },
    { text: "Próprio", value: 1 },
    { text: "Terceiro", value: 2 }
];

var PesquisaAcertoDeViagem = function () {
    this.DataInicial = PropertyEntity({ text: "Período inicial do acerto: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialFechamento = PropertyEntity({ text: "Período inicial do fechamento: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalFechamento = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _situacaoPesquisa, def: 0, text: "Situação: " });
    this.AcertoViagem = PropertyEntity({ text: "Acerto de Viagem:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ text: "Motorista:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Segmento = PropertyEntity({ text: "Segmento Veícular:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.VeiculoTracao = PropertyEntity({ text: "Veículo de Tração:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.VeiculoReboque = PropertyEntity({ text: "Veículo de Reboque:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });

    this.StatusMotorista = PropertyEntity({ val: ko.observable(""), options: _statusPesquisa, def: "", text: "Status Motorista: " });
    this.TipoMotorista = PropertyEntity({ val: ko.observable(0), options: _tipoMotorista, def: 0, text: "Tipo Motorista: " });
    this.UltimoAcerto = PropertyEntity({ text: "Visualizar apenas o Acerto mais recente de cada motorista?", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAcertoDeViagem.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioAcertoDeViagem() {

    _pesquisaAcertoDeViagem = new PesquisaAcertoDeViagem();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridAcertoDeViagem = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/AcertoDeViagem/Pesquisa", _pesquisaAcertoDeViagem, null, null, 10);
    _gridAcertoDeViagem.SetPermitirEdicaoColunas(true);

    _relatorioAcertoDeViagem = new RelatorioGlobal("Relatorios/AcertoDeViagem/BuscarDadosRelatorio", _gridAcertoDeViagem, function () {
        _relatorioAcertoDeViagem.loadRelatorio(function () {
            KoBindings(_pesquisaAcertoDeViagem, "knockoutPesquisaAcertoDeViagem");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaAcertoDeViagem");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaAcertoDeViagem");

            new BuscarMotoristasPorStatus(_pesquisaAcertoDeViagem.Motorista);
            new BuscarSegmentoVeiculo(_pesquisaAcertoDeViagem.Segmento);
            new BuscarVeiculos(_pesquisaAcertoDeViagem.VeiculoTracao);
            new BuscarReboques(_pesquisaAcertoDeViagem.VeiculoReboque);
            new BuscarAcertoViagem(_pesquisaAcertoDeViagem.AcertoViagem, RetornoBuscarAcertoViagem);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAcertoDeViagem);

}

function RetornoBuscarAcertoViagem(data) {
    _pesquisaAcertoDeViagem.AcertoViagem.codEntity(data.Codigo);
    _pesquisaAcertoDeViagem.AcertoViagem.val(data.Numero);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioAcertoDeViagem.gerarRelatorio("Relatorios/AcertoDeViagem/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioAcertoDeViagem.gerarRelatorio("Relatorios/AcertoDeViagem/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
