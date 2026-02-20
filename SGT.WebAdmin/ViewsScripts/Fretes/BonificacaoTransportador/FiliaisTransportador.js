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
/// <reference path="BonificacaoTransportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFilialTransportador;
var _filialTransportador;

var FilialTransportador = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.FiliaisDoTransportador = PropertyEntity({ type: types.event, text: "Adicionar Filial Transportador", idBtnSearch: guid() });
}

//*******EVENTOS*******

function loadFilialTransportador() {

    _filialTransportador = new FilialTransportador();
    KoBindings(_filialTransportador, "knockoutFilialTransportador");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirFilialTransportadorClick(_filialTransportador.FiliaisDoTransportador, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }];

    _gridFilialTransportador = new BasicDataTable(_filialTransportador.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTransportadores(_filialTransportador.FiliaisDoTransportador, null, null, null, _gridFilialTransportador, null, null, null, null, null, null, null, null, null, null, null, _bonificacaoTransportador.Empresa);
    _filialTransportador.FiliaisDoTransportador.basicTable = _gridFilialTransportador;

    recarregarGridFilialTransportador();
}

function recarregarGridFilialTransportador() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_bonificacaoTransportador.FiliaisTransportador.val())) {
        $.each(_bonificacaoTransportador.FiliaisTransportador.val(), function (i, filialTransportador) {
            var filialTransportadorGrid = new Object();

            filialTransportadorGrid.Codigo = filialTransportador.Codigo;
            filialTransportadorGrid.Descricao = filialTransportador.Descricao;

            data.push(filialTransportadorGrid);
        });
    }

    _gridFilialTransportador.CarregarGrid(data);
}


function excluirFilialTransportadorClick(knoutFilialTransportador, data) {
    var filialTransportadorGrid = knoutFilialTransportador.basicTable.BuscarRegistros();

    for (var i = 0; i < filialTransportadorGrid.length; i++) {
        if (data.Codigo == filialTransportadorGrid[i].Codigo) {
            filialTransportadorGrid.splice(i, 1);
            break;
        }
    }

    knoutFilialTransportador.basicTable.CarregarGrid(filialTransportadorGrid);
}

function limparCamposFilialTransportador() {
    LimparCampos(_filialTransportador);
}