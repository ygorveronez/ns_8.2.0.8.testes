/// <reference path="../../Consultas/TipoOperacao.js" />
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
/// <reference path="TabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFilial;
var _filial;

var Filial = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Filial = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFrete.AdicionarFilial, idBtnSearch: guid() });
}


//*******EVENTOS*******

function loadFilial() {
    _filial = new Filial();
    KoBindings(_filial, "knockoutFilial");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: function (data) {
                excluirFilialClick(_filial.Filial, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "80%" }];

    _gridFilial = new BasicDataTable(_filial.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarFilial(_filial.Filial, null, _gridFilial);
    _filial.Filial.basicTable = _gridFilial;

    recarregarGridFilial();
}

function recarregarGridFilial() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_tabelaFrete.Filiais.val())) {

        $.each(_tabelaFrete.Filiais.val(), function (i, filial) {
            var filialGrid = new Object();

            filialGrid.Codigo = filial.Filial.Codigo;
            filialGrid.Descricao = filial.Filial.Descricao;

            data.push(filialGrid);
        });
    }

    _gridFilial.CarregarGrid(data);
}


function excluirFilialClick(knoutFilial, data) {
    var filialGrid = knoutFilial.basicTable.BuscarRegistros();

    for (var i = 0; i < filialGrid.length; i++) {
        if (data.Codigo == filialGrid[i].Codigo) {
            filialGrid.splice(i, 1);
            break;
        }
    }

    knoutFilial.basicTable.CarregarGrid(filialGrid);
}

function limparCamposFilial() {
    LimparCampos(_filial);
}