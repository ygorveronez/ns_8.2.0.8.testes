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


//*******MAPEAMENTO KNOUCKOUT*******

var _gridJanelaColetaLocalidade;
var _janelaColetaLocalidade;
var _listaLocalidades = new Array();

var JanelaColetaLocalidade = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Localidade = PropertyEntity({ type: types.event, text: "Adicionar Localidade", idBtnSearch: guid() });
}


//*******EVENTOS*******

function LoadJanelaColetaLocalidade() {
    _janelaColetaLocalidade = new JanelaColetaLocalidade();
    KoBindings(_janelaColetaLocalidade, "knockoutJanelaColetaLocalidade");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirJanelaColetaLocalidadeClick(_janelaColetaLocalidade.Localidade, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: "Descrição", width: "80%" }];

    _gridJanelaColetaLocalidade = new BasicDataTable(_janelaColetaLocalidade.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLocalidades(_janelaColetaLocalidade.Localidade, null, null, function (r) {
        if (r != null) {
            
            _listaLocalidades.push({ Codigo: r.Codigo, Descricao: r.Descricao });

            _gridJanelaColetaLocalidade.CarregarGrid(_listaLocalidades);
        }
    }, null, null, null, null, _gridJanelaColetaLocalidade);
    _janelaColetaLocalidade.Localidade.basicTable = _gridJanelaColetaLocalidade;

    RecarregarGridJanelaColetaLocalidade();
}

function RecarregarGridJanelaColetaLocalidade() {
    _gridJanelaColetaLocalidade.CarregarGrid(_janelaColeta.Localidades.val());


    if (_janelaColeta.Localidades.val() != "") {

        _listaLocalidades = new Array();

        for (var i = 0; i < _janelaColeta.Localidades.val().length; i++) {

            var item = _janelaColeta.Localidades.val()[i];
            _listaLocalidades.push({ Codigo: item.Codigo, Descricao: item.Descricao });
        }
    }

}

function ExcluirJanelaColetaLocalidadeClick(knoutJanelaColetaLocalidade, data) {
    var JanelaColetaLocalidadeGrid = knoutJanelaColetaLocalidade.basicTable.BuscarRegistros();

    for (var i = 0; i < JanelaColetaLocalidadeGrid.length; i++) {
        if (data.Codigo == JanelaColetaLocalidadeGrid[i].Codigo) {
            JanelaColetaLocalidadeGrid.splice(i, 1);
            break;
        }
    }

    knoutJanelaColetaLocalidade.basicTable.CarregarGrid(JanelaColetaLocalidadeGrid);
}

function LimparCamposJanelaColetaLocalidade() {
    LimparCampos(_janelaColetaLocalidade);
    _gridJanelaColetaLocalidade.CarregarGrid(new Array());
    _listaLocalidades = new Array();
}
