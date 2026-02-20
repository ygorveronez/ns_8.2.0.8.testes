
var _detalhesCargaSumarizada;
var _gridDetalhesCargaSumarizada;

var DetalhesCargaSumarizada = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Documentos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
}


function loadDetalhesCargaSumarizada() {
    _detalhesCargaSumarizada = new DetalhesCargaSumarizada();
    KoBindings(_detalhesCargaSumarizada, "knockoutDetalhesCargaSumarizada");

    //-- Grid Documentos
    // Menu
    var menuOpcoes = null;

    // Grid
    var configExport = {
        url: "CargaOcorrencia/ExportarDetalhesDocumentos",
        titulo: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DetalhesDocumento
    };
    _gridDetalhesCargaSumarizada = new GridViewExportacao(_detalhesCargaSumarizada.Documentos.idGrid, "CargaOcorrencia/DetalhesCargaSumarizada", _detalhesCargaSumarizada, menuOpcoes, configExport);
}

function detalhesCargaSumarizada(itemGrid) {
    _detalhesCargaSumarizada.CodigoVeiculo.val(itemGrid.CodigoVeiculo);
    _detalhesCargaSumarizada.Ocorrencia.val(_ocorrencia.Codigo.val());
    _gridDetalhesCargaSumarizada.CarregarGrid();
    Global.abrirModal('divModalDetalhesCarga');
}