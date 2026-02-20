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
/// <reference path="ConfigOperador.js" />
/// <reference path="../../Consultas/Filial.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFilialVenda;
var _filialVenda;

var FilialVenda = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.FilialVenda = PropertyEntity({ type: types.event, text: Localization.Resources.Operacional.ConfigOperador.AdicionarFilialVenda, idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadFilialVenda() {
    _filialVenda = new FilialVenda();
    KoBindings(_filialVenda, "knockoutOperadorFilialVenda");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Operacional.ConfigOperador.Excluir, id: guid(), metodo: function (data) {
                ExcluirFilialVendaClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Operacional.ConfigOperador.Descricao, width: "80%" }
    ];

    _gridFilialVenda = new BasicDataTable(_filialVenda.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarFilial(_filialVenda.FilialVenda, null, _gridFilialVenda, true);
    _filialVenda.FilialVenda.basicTable = _gridFilialVenda;

    RecarregarGridFilialVenda();
}

function RecarregarGridFilialVenda() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_operador.FiliaisVenda.val())) {
        $.each(_operador.FiliaisVenda.val(), function (i, filialVenda) {
            var filialVendaGrid = new Object();

            filialVendaGrid.Codigo = filialVenda.Codigo;
            filialVendaGrid.Descricao = filialVenda.Descricao;

            data.push(filialVendaGrid);
        });
    }
    _gridFilialVenda.CarregarGrid(data);
}

function ExcluirFilialVendaClick(data) {
    var filialVendaGrid = _filialVenda.FilialVenda.basicTable.BuscarRegistros();

    for (var i = 0; i < filialVendaGrid.length; i++) {
        if (data.Codigo == filialVendaGrid[i].Codigo) {
            filialVendaGrid.splice(i, 1);
            break;
        }
    }

    _filialVenda.FilialVenda.basicTable.CarregarGrid(filialVendaGrid);
}

function LimparCamposFilialVenda() {
    LimparCampos(_filialVenda);
    _filialVenda.FilialVenda.basicTable.CarregarGrid(new Array());
}