/// <reference path="../../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../ParametrosOfertas.js" />

var _gridFiliais;
var _filiais;

var Filiais = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.Filial = PropertyEntity({ type: types.event, text: "Adicionar Filiais", idBtnSearch: guid() });
};

function LoadFiliais() {
    _filiais = new Filiais();
    KoBindings(_filiais, "knockoutFiliais");

    let menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirFiliaisClick(data)
            }
        }]
    };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%" },
    ];

    _gridFiliais = new BasicDataTable(_filiais.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _filiais.Filial.basicTable = _gridFiliais;

    new BuscarFilial(_filiais.Filial, null, _gridFiliais);

    RecarregarGridFiliais();
}

function RecarregarGridFiliais() {

    let data = new Array();

    if (_parametrosOfertas.Filiais.val() != "" && _parametrosOfertas.Filiais.val().length > 0) {
        $.each(_parametrosOfertas.Filiais.val(), function (i, fil) {
            let filiaisGrid = new Object();

            filiaisGrid.Codigo = fil.Codigo;
            filiaisGrid.Descricao = fil.Descricao;
            filiaisGrid.CodigoRelacionamento = fil.CodigoRelacionamento;

            data.push(filiaisGrid);
        });
    }

    _gridFiliais.CarregarGrid(data);
}

function PreencherFiliais(listaFiliaisRetornadas) {
    _parametrosOfertas.Filiais.val(listaFiliaisRetornadas);
    RecarregarGridFiliais();
}

function ExcluirFiliaisClick(data) {
    let filiaisGrid = _gridFiliais.BuscarRegistros();

    for (let i = 0; i < filiaisGrid.length; i++) {
        if (data.Codigo == filiaisGrid[i].Codigo) {
            filiaisGrid.splice(i, 1);
            break;
        }
    }

    _gridFiliais.CarregarGrid(filiaisGrid);
}

function LimparCamposFiliais() {
    LimparCampos(_filiais);
    _gridFiliais.CarregarGrid(new Array());
}