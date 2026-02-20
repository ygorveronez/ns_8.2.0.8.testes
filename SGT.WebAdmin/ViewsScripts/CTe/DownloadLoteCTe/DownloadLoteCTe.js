/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Enumeradores/EnumSituacaoDownloadLoteCTe.js" />

var _gridConsultaLoteCTe;
var _pesquisaLoteCTe;
var _visualizarCTes;
var _gridCTes;

var PesquisaLoteCTe = function () {
    this.DataImportacaoInicial = PropertyEntity({ text: "Data de Importação Inicial:", getType: typesKnockout.date, val: ko.observable("") });
    this.DataImportacaoFinal = PropertyEntity({ text: "Data de Importação Final:", getType: typesKnockout.date, val: ko.observable("") });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0, options: EnumSituacaoDownloadLoteCTe.obterOpcoes(), text: "Situação:", enable: ko.observable(true) });

    this.DataImportacaoFinal.dateRangeInit = this.DataImportacaoInicial;
    this.DataImportacaoInicial.dateRangeLimit = this.DataImportacaoFinal;

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        UrlImportacao: "DownloadLoteCTe/Importar",
        UrlConfiguracao: "DownloadLoteCTe/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O030_LoteCTe,
        CallbackImportacao: function () {
            _gridConsultaLoteCTe.CarregarGrid();
        }
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConsultaLoteCTe.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid()
    });
}

var VisualizarCTes = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.CTes = PropertyEntity({ type: types.listEntity, list: new Array(), visibleFade: ko.observable(false), required: false, codEntity: ko.observable(0), idGrid: guid() });
}

function LoadDownloadLoteCTe() {
    _pesquisaLoteCTe = new PesquisaLoteCTe();
    KoBindings(_pesquisaLoteCTe, "knockoutConsultaLoteCTe");

    _visualizarCTes = new VisualizarCTes();
    KoBindings(_visualizarCTes, "knockoutVisualizarCTes");

    HeaderAuditoria("DownloadLoteCTe", _pesquisaLoteCTe);

    LoadGridLoteCTe();
}

function LoadGridLoteCTe() {
    var visualizarCTes = { descricao: "Visualizar CTes", id: guid(), evento: "onclick", metodo: VisualizarCTesClick, tamanho: "20", icone: "", visibilidade: true };
    var cancelar = { descricao: "Cancelar", id: guid(), evento: "onclick", metodo: CancelarClick, tamanho: "20", icone: "", visibilidade: VerificarVisibilidadeCancelar };
    var reprocessar = { descricao: "Reprocessar", id: guid(), evento: "onclick", metodo: ReprocessarClick, tamanho: "20", icone: "", visibilidade: VerificarVisibilidadeReprocessar };
    var downloadLoteXML = { descricao: "Download XML", id: guid(), evento: "onclick", metodo: DownloadXMLClick, tamanho: "20", icone: "", visibilidade: VerificarVisibilidadeDownloadXML };
    var downloadLotePDF = { descricao: "Download PDF", id: guid(), evento: "onclick", metodo: DownloadPDFClick, tamanho: "20", icone: "", visibilidade: VerificarVisibilidadeDownloadPDF };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [visualizarCTes, cancelar, reprocessar, downloadLoteXML, downloadLotePDF],
        tamanho: 7
    };

    _gridConsultaLoteCTe = new GridView(_pesquisaLoteCTe.Pesquisar.idGrid, "DownloadLoteCTe/Pesquisa", _pesquisaLoteCTe, menuOpcoes, null, 10);
    _gridConsultaLoteCTe.CarregarGrid();
}

function BuscarNotas() {
    _gridCTes = new GridView(_visualizarCTes.CTes.idGrid, "DownloadLoteCTe/PesquisaCTesLote", _visualizarCTes, null, null, 10);
    _gridCTes.CarregarGrid();
}

//#region Funções Click

function VisualizarCTesClick(registroSelecionado) {
    _visualizarCTes.Codigo.val(registroSelecionado.Codigo);
    BuscarNotas();
    Global.abrirModal('knockoutVisualizarCTes');
}

function ReprocessarClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Deseja reprocessar do Lote N° " + registroSelecionado.Codigo + "?", function () {
        executarReST("DownloadLoteCTe/ReprocessarLote", { Codigo: registroSelecionado.Codigo }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.Success, "Sucesso", "O Reprocessamento do lote foi iniciado.");
                _gridConsultaLoteCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function DownloadXMLClick(registroSelecionado) {
    executarDownload("DownloadLoteCTe/DownloadLoteXML", { Codigo: registroSelecionado.Codigo });
}

function DownloadPDFClick(registroSelecionado) {
    executarDownload("DownloadLoteCTe/DownloadLotePDF", { Codigo: registroSelecionado.Codigo });
}

function CancelarClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Deseja cancelar o processamento do Lote N° " + registroSelecionado.Codigo + "?", function () {
        executarReST("DownloadLoteCTe/CancelarLote", { Codigo: registroSelecionado.Codigo }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.Success, "Sucesso", "O Processamento do lote foi cancelado.");
                _gridConsultaLoteCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

//#endregion

//#region Métodos Privados

function VerificarVisibilidadeCancelar(registro) {
    if (registro.SituacaoCodigo == EnumSituacaoDownloadLoteCTe.Pendente)
        return true;

    return false;
}

function VerificarVisibilidadeReprocessar(registro) {
    if (registro.SituacaoCodigo == EnumSituacaoDownloadLoteCTe.Falha || registro.SituacaoCodigo == EnumSituacaoDownloadLoteCTe.Cancelado)
        return true;

    return false;
}

function VerificarVisibilidadeDownloadXML(registro) {
    if (registro.SituacaoCodigo == EnumSituacaoDownloadLoteCTe.Finalizado)
        return true;

    return false;
}

function VerificarVisibilidadeDownloadPDF(registro) {
    if (registro.SituacaoCodigo == EnumSituacaoDownloadLoteCTe.Finalizado)
        return true;

    return false;
}

//#endregion