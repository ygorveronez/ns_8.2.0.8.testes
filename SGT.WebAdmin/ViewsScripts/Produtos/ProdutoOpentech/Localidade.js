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
/// <reference path="../../Consultas/Localidade.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridLocalidade;
var _localidade;

var Localidade = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Localidade = PropertyEntity({ type: types.event, text: "Adicionar Localidade", idBtnSearch: guid() });
}


//*******EVENTOS*******

function LoadLocalidade() {
    _localidade = new Localidade();
    KoBindings(_localidade, "knockoutLocalidade");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirLocalidadeClick(_localidade.Localidade, data)
            }
        }]

    };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" },
        { data: "Estado", title: "UF", width: "20%" },
    ];

    _gridLocalidade = new BasicDataTable(_localidade.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarLocalidadesPorEstados(_localidade.Localidade, 'Pesquisa de Localidades', 'Localidades', null, _gridLocalidade, null, _produtoOpentech.Estados);

    _localidade.Localidade.basicTable = _gridLocalidade;
    _localidade.Localidade.basicTable.CarregarGrid(new Array());
}

function RecarregarGridLocalidade() {
    _gridLocalidade.CarregarGrid(_produtoOpentech.Localidades.val()); 
}

function ExcluirLocalidadeClick(knockoutLocalidade, data) {
    var localidadeGrid = knockoutLocalidade.basicTable.BuscarRegistros();

    for (var i = 0; i < localidadeGrid.length; i++) {
        if (data.Codigo == localidadeGrid[i].Codigo) {
            localidadeGrid.splice(i, 1);
            break;
        }
    }

    knockoutLocalidade.basicTable.CarregarGrid(localidadeGrid);
}

function LimparCamposLocalidade() {
    LimparCampos(_localidade);
    _gridLocalidade.CarregarGrid(new Array());
}
