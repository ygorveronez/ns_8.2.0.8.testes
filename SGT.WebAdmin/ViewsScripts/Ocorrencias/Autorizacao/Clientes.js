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
/// <reference path="gridClientes.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _clientes;
var _gridClientes;

var Clientes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Clientes = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, idGrid: guid() });


    this.Exportar = PropertyEntity({ type: types.event, eventClick: ExportarClientesAutorizacaoClick, text: Localization.Resources.Ocorrencias.Exportar, visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadClientes() {

    _clientes = new Clientes();
    KoBindings(_clientes, "knockoutClientes");

    // Gera grid
    var linhasPorPaginas = 7;
    _gridClientes = new GridView(_clientes.Clientes.idGrid, "AutorizacaoOcorrencia/ConsultarClientesDoPedidosDaCarga", _clientes, null, null);
}




//*******MÉTODOS*******

function limparClientes() {
    LimparCampos(_clientes);
    _gridClientes.CarregarGrid();
}

function CarregarClientes(ocorrencia) {
    if (_CONFIGURACAO_TMS.ExibirAssociacaoClientesNoPedido) {

        _clientes.Codigo.val(ocorrencia);
        _gridClientes.CarregarGrid(function () {
            if (_gridClientes.NumeroRegistros() > 0)
                $("#liClientes").show();
            else
                $("#liClientes").hide();
        });
    }
}


function ExportarClientesAutorizacaoClick() {
    var cnpjsClientes = [];


    var data = _gridClientes.GridViewTable().rows().data();

    if (!data)
        return;

    var count = _gridClientes.GridViewTable().rows()[0].length;

    for (var i = 0; i < count; i++) {
        cnpjsClientes.push(data[i].Codigo);

    }

    executarDownload("AutorizacaoOcorrencia/ExportarPesquisaCliente", { Clientes: JSON.stringify(cnpjsClientes), Grid: "{}" });
}