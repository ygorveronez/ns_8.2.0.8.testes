/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

var _navioOperador;
var _gridNaviosOperadores;

var NavioOperador = function () {
    this.Operadores = PropertyEntity({ type: types.local });
}

function LoadNaviosOperadores() {
    _navioOperador = new NavioOperador();
    KoBindings(_navioOperador, "knockoutNavioOperador");

    GridNaviosOperadores();
}

function GridNaviosOperadores() {
    if (_navioOperador && _navioOperador.Destroy)
        _navioOperador.Destroy();

    _navioOperador.Operadores.get$().empty();

    let header = [
        { data: "CodigoOperador", title: "Código do Operador" },
        { data: "IdOperador", title: "ID do Operador" },
        { data: "CodigoIntegracao", title: "Código de Integração" },
        { data: "DataAtivo", title: "Data Ativo" },
        { data: "DataInativo", title: "Data Inativo" },
        { data: "Status", title: "Status" },
    ];

    _gridNaviosOperadores = new BasicDataTable(_navioOperador.Operadores.id, header);

    RecarregarGridNaviosOperadores();
}
function GetNaviosOperadores() {
    return _navio.Operadores.list.slice();
}

function RecarregarGridNaviosOperadores() {
    var data = [];
    $.each(GetNaviosOperadores(), function (i, navioOperador) {
        var itemGrid = new Object();
        itemGrid.CodigoOperador = navioOperador.CodigoOperador.val;
        itemGrid.CodigoIntegracao = navioOperador.CodigoIntegracao.val;
        itemGrid.DataAtivo = navioOperador.DataAtivo.val;
        itemGrid.DataInativo = navioOperador.DataInativo.val;
        itemGrid.Status = navioOperador.Status.val;
        itemGrid.IdOperador = navioOperador.IdOperador.val;

        data.push(itemGrid);
    });

    _gridNaviosOperadores.CarregarGrid(data);
}