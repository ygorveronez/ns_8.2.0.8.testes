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
/// <reference path="../../Consultas/Filial.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFilial;
var _filial;
var _filiais = new Array();

var Filial = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Tipo = PropertyEntity({ type: types.event, text: "Adicionar Filial", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadFilial() {

    _filial = new Filial();
    KoBindings(_filial, "knockoutFilial");

    let menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirFilialClick(_filial.Tipo, data)
            }
        }]
    };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }
    ];

    _gridFilial = new BasicDataTable(_filial.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarFilial(_filial.Tipo, function (r) {
        if (r != null) {
            for (let i = 0; i < r.length; i++)
                _filiais.push({ Codigo: r[i].Codigo, Descricao: r[i].Descricao });

            _gridFilial.CarregarGrid(_filiais);

            RecarregarOpcoesFilial();
        }
    }, _gridFilial);
    _filial.Tipo.basicTable = _gridFilial;

    RecarregarGridFilial();
}

function RecarregarGridFilial() {
    _gridFilial.CarregarGrid(_configuracaoToleranciaPesagem.Filial.val());
    _filiais = _configuracaoToleranciaPesagem.Filial.val();
}

function ExcluirFilialClick(knoutFilial, data) {
    let filiais = knoutFilial.basicTable.BuscarRegistros();

    for (let i = 0; i < filiais.length; i++) {
        if (data.Codigo == filiais[i].Codigo) {
            filiais.splice(i, 1);
            break;
        }
    }

    knoutFilial.basicTable.CarregarGrid(filiais);

    RecarregarOpcoesFilial();
}

function LimparCamposFilial() {
    LimparCampos(_filial);
    _gridFilial.CarregarGrid(new Array());
    _filiais = new Array();
}

function RecarregarOpcoesFilial() {
    let filialGrid = _filial.Tipo.basicTable.BuscarRegistros();

    _opcoesFilial = new Array();

    for (let i = 0; i < filialGrid.length; i++)
        _opcoesFilial.push({ value: filialGrid[i].Codigo, text: filialGrid[i].Descricao });
}