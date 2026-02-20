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

var _gridJanelaColetaUF;
var _janelaColetaUF;
var _listaUFs = new Array();

var JanelaColetaUF = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.UF = PropertyEntity({ type: types.event, text: "Adicionar UF", idBtnSearch: guid() });
}


//*******EVENTOS*******

function LoadJanelaColetaUF() {
    
    _janelaColetaUF = new JanelaColetaUF();
    KoBindings(_janelaColetaUF, "knockoutJanelaColetaUF");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirJanelaColetaUFClick(_janelaColetaUF.UF, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: "Descrição", width: "80%" }];

    _gridJanelaColetaUF = new BasicDataTable(_janelaColetaUF.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarEstados(_janelaColetaUF.UF, function (r) {
        if (r != null) {
            
            _listaUFs.push({ Codigo: r.Codigo, Descricao: r.Descricao });

            _gridJanelaColetaUF.CarregarGrid(_listaUFs);
        }
    });
    _janelaColetaUF.UF.basicTable = _gridJanelaColetaUF;

    RecarregarGridJanelaColetaUF();
}

function RecarregarGridJanelaColetaUF() {
    _gridJanelaColetaUF.CarregarGrid(_janelaColeta.UFs.val());


    if (_janelaColeta.UFs.val() != "") {

        _listaUFs = new Array();

        for (var i = 0; i < _janelaColeta.UFs.val().length; i++) {

            var item = _janelaColeta.UFs.val()[i];
            _listaUFs.push({ Codigo: item.Codigo, Descricao: item.Descricao });

        }
    }

}

function ExcluirJanelaColetaUFClick(knoutJanelaColetaUF, data) {
    var JanelaColetaUFGrid = knoutJanelaColetaUF.basicTable.BuscarRegistros();

    for (var i = 0; i < JanelaColetaUFGrid.length; i++) {
        if (data.Codigo == JanelaColetaUFGrid[i].Codigo) {
            JanelaColetaUFGrid.splice(i, 1);
            break;
        }
    }

    knoutJanelaColetaUF.basicTable.CarregarGrid(JanelaColetaUFGrid);
}

function LimparCamposJanelaColetaUF() {
    LimparCampos(_janelaColetaUF);
    _gridJanelaColetaUF.CarregarGrid(new Array());
    _listaUFs = new Array();
}
