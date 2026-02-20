/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

var _detalhesCargaSumarizada;
var _gridDetalhesCargaSumarizada;
var _gridDetalhesDocumentosAgrupados;

var DetalhesCargaSumarizada = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
     
    this.PeriodoInicio = PropertyEntity({ val: obterPeriodoInicio });
    this.PeriodoFim = PropertyEntity({ val: obterPeriodoFim });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoOcorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Documentos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
}

var DetalhesDocumentosAgrupados = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ContratoFreteTransportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.CnpjRemetente = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int });
    this.CnpjDestinatario = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int });
    this.ModeloDocumento = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int });
    this.Veiculo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int });

    this.PeriodoInicio = PropertyEntity({ val: obterPeriodoInicio });
    this.PeriodoFim = PropertyEntity({ val: obterPeriodoFim });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Documentos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
}

function loadDetalhesCargaSumarizada() {
    _detalhesCargaSumarizada = new DetalhesCargaSumarizada();
    KoBindings(_detalhesCargaSumarizada, "knockoutDetalhesCargaSumarizada");

    _ocorrencia.TipoOcorrencia.codEntity.subscribe(function (valor) {
        _detalhesCargaSumarizada.TipoOcorrencia.val(valor);
    });

    //-- Grid Documentos
    // Menu
    var menuOpcoes = null;

    // Grid
    var configExport = {
        url: "CargaOcorrencia/ExportarDetalhesDocumentos",
        titulo: "Detalhes Documentos"
    };
    _gridDetalhesCargaSumarizada = new GridView(_detalhesCargaSumarizada.Documentos.idGrid, "CargaOcorrencia/DetalhesCargaSumarizada", _detalhesCargaSumarizada, menuOpcoes, { column: 1, dir: orderDir.desc }, null, null, null, null, null, null, null, configExport);
}

function loadDetalhesDocumentosAgrupados() {
    _detalheDocumentoAgrupado = new DetalhesDocumentosAgrupados();
    KoBindings(_detalheDocumentoAgrupado, "knockoutDetalhesDocumentosAgrupados");

    _ocorrencia.ContratoFreteTransportador.codEntity.subscribe(function (valor) {
        _detalheDocumentoAgrupado.ContratoFreteTransportador.val(valor);
    });

    //-- Grid Documentos
    // Menu
    var menuOpcoes = null;

    _gridDetalhesDocumentosAgrupados = new GridView(_detalheDocumentoAgrupado.Documentos.idGrid, "CargaOcorrencia/DetalhesDocumentosAgrupados", _detalheDocumentoAgrupado, menuOpcoes);
}

function detalhesCargaSumarizada(itemGrid) {
    _detalhesCargaSumarizada.CodigoVeiculo.val(itemGrid.CodigoVeiculo);
    _detalhesCargaSumarizada.Ocorrencia.val(_ocorrencia.Codigo.val());

    _detalheDocumentoAgrupado.Documentos.visible(false);
    _detalhesCargaSumarizada.Documentos.visible(true);

    _gridDetalhesCargaSumarizada.CarregarGrid();

    Global.abrirModal('divModalDetalhesCarga');
}

function detalhesDocumentosAgrupados(itemGrid) {
    _detalheDocumentoAgrupado.CnpjRemetente.val(itemGrid.CnpjRemetente);
    _detalheDocumentoAgrupado.CnpjDestinatario.val(itemGrid.CnpjDestinatario);
    _detalheDocumentoAgrupado.ModeloDocumento.val(itemGrid.ModeloDocumento);
    _detalheDocumentoAgrupado.Ocorrencia.val(_ocorrencia.Codigo.val());
    _detalheDocumentoAgrupado.Veiculo.val(_ocorrencia.Veiculo.codEntity());

    _detalhesCargaSumarizada.Documentos.visible(false);
    _detalheDocumentoAgrupado.Documentos.visible(true);

    _gridDetalhesDocumentosAgrupados.CarregarGrid();
    Global.abrirModal('divModalDetalhesCarga');
}