/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/CFOP.js" />
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
/// <reference path="../../Enumeradores/EnumUnidadeMedida.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridGuia, _guia;

var Guia = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

//*******EVENTOS*******

function LoadGuia() {

    _guia = new Guia();
    KoBindings(_guia, "knockoutGuias");

    var header = [{ data: "Codigo", visible: false },
    { data: "Numero", title: "Número", width: "10%" },
    { data: "DataVencimento", title: "Data de Vencimento", width: "10%" },
    { data: "Valor", title: "Valor", width: "10%" },
    { data: "CodigoTitulo", title: "Cód. Título", width: "10%" },
    { data: "ValorTitulo", title: "Valor Título", width: "10%" },
    { data: "StatusTitulo", title: "Status Título", width: "10%" },
    { data: "DataPagamento", title: "Data Pagamento", width: "10%" }];

    _gridGuia = new BasicDataTable(_guia.Grid.id, header, null, { column: 1, dir: orderDir.asc });

    RecarregarGridGuia();
}

function RecarregarGridGuia() {

    var data = new Array();

    $.each(_documentoEntrada.Guias.list, function (i, guia) {
        var guiaGrid = new Object();

        guiaGrid.Codigo = guia.Codigo.val;
        guiaGrid.Numero = guia.Numero.val;
        guiaGrid.DataVencimento = guia.DataVencimento.val;
        guiaGrid.Valor = guia.Valor.val;
        if (guia.CodigoTitulo != undefined) {
            guiaGrid.CodigoTitulo = guia.CodigoTitulo.val;
            guiaGrid.ValorTitulo = guia.ValorTitulo.val;
            guiaGrid.StatusTitulo = guia.StatusTitulo.val;
            guiaGrid.DataPagamento = guia.DataPagamento.val;
        } else {
            guiaGrid.CodigoTitulo = "";
            guiaGrid.ValorTitulo = "";
            guiaGrid.StatusTitulo = "";
            guiaGrid.DataPagamento = "";
        }

        data.push(guiaGrid);
    });

    _gridGuia.CarregarGrid(data);
}