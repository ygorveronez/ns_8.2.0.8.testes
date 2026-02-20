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
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="RegraTipoOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegraTipoOperacaoFiliais;
var _regraTipoOperacaoFiliais;

var RegraTipoOperacaoFiliais = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.CadastrarFiliais = PropertyEntity({ type: types.event, text: "Adicionar Filiais", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadRegraTipoOperacaoFiliais() {
    _regraTipoOperacaoFiliais = new RegraTipoOperacaoFiliais();
    KoBindings(_regraTipoOperacaoFiliais, "knockoutRegraTipoOperacaoFiliais");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirRegraTipoOperacaoFiliaisClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Filial", width: "70%" },
    ];

    _gridRegraTipoOperacaoFiliais = new BasicDataTable(_regraTipoOperacaoFiliais.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarFilial(_regraTipoOperacaoFiliais.CadastrarFiliais, null, _gridRegraTipoOperacaoFiliais);

    RecarregarGridRegraTipoOperacaoFiliais();
}

function RecarregarGridRegraTipoOperacaoFiliais() {
    var data = new Array();
    if (_regraTipoOperacao.Filiais.val() != "") {
        $.each(_regraTipoOperacao.Filiais.val(), function (i, tipoCarga) {
            var tiposRegraTipoOperacaoFiliaisGrid = new Object();

            tiposRegraTipoOperacaoFiliaisGrid.Codigo = tipoCarga.Codigo;
            tiposRegraTipoOperacaoFiliaisGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposRegraTipoOperacaoFiliaisGrid);
        });
    }
    _gridRegraTipoOperacaoFiliais.CarregarGrid(data);
}

function ExcluirRegraTipoOperacaoFiliaisClick(data) {
    var tiposRegraTipoOperacaoFiliaisGrid = _gridRegraTipoOperacaoFiliais.BuscarRegistros();

    for (var i = 0; i < tiposRegraTipoOperacaoFiliaisGrid.length; i++) {
        if (data.Codigo == tiposRegraTipoOperacaoFiliaisGrid[i].Codigo) {
            tiposRegraTipoOperacaoFiliaisGrid.splice(i, 1);
            break;
        }
    }

    _gridRegraTipoOperacaoFiliais.CarregarGrid(tiposRegraTipoOperacaoFiliaisGrid);
}

function LimparCamposRegraTipoOperacaoFiliais() {
    LimparCampos(_regraTipoOperacaoFiliais);
    _gridRegraTipoOperacaoFiliais.CarregarGrid(new Array());
}