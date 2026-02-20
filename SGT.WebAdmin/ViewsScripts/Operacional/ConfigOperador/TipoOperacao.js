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
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="OperadorFilial.js" />
/// <reference path="ConfigOperador.js" />
/// <reference path="OperadorTipoCarga.js" />
/// <reference path="OperadorTipoOperacao.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoOperacao;
var _tipoOperacao;

var TipoOperacao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Operacao = PropertyEntity({ type: types.event, text: Localization.Resources.Operacional.ConfigOperador.AdicionarTipoOperacao, idBtnSearch: guid() });
    this.PossuiFiltroTipoOperacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Operacional.ConfigOperador.EsteoperadorDeveFiltrarCargasPorOperacao });
    this.VisualizaCargasSemTipoOperacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Operacional.ConfigOperador.OperadorVisualizaCargasNaoPossuemTipoOperacao });
}

//*******EVENTOS*******

function loadTipoOperacao() {

    _tipoOperacao = new TipoOperacao();
    KoBindings(_tipoOperacao, "knockoutTipoOperacao");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Operacional.ConfigOperador.Excluir, id: guid(), metodo: function (data) {
                excluirTipoOperacaoClick(_tipoOperacao.Operacao, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Operacional.ConfigOperador.Descricao, width: "80%" }];

    _gridTipoOperacao = new BasicDataTable(_tipoOperacao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposOperacao(_tipoOperacao.Operacao, null, null, null, _gridTipoOperacao, null, null, false);
    _tipoOperacao.Operacao.basicTable = _gridTipoOperacao;

    recarregarGridTipoOperacao();
}

function recarregarGridTipoOperacao() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_operador.TiposOperacao.val())) {
        $.each(_operador.TiposOperacao.val(), function (i, tipoOperacao) {
            var tipoOperacaoGrid = new Object();

            tipoOperacaoGrid.Codigo = tipoOperacao.Operacao.Codigo;
            tipoOperacaoGrid.Descricao = tipoOperacao.Operacao.Descricao;

            data.push(tipoOperacaoGrid);
        });
    }

    _gridTipoOperacao.CarregarGrid(data);

    _tipoOperacao.PossuiFiltroTipoOperacao.val(_operador.PossuiFiltroTipoOperacao.val());
    _tipoOperacao.VisualizaCargasSemTipoOperacao.val(_operador.VisualizaCargasSemTipoOperacao.val());
}


function excluirTipoOperacaoClick(knoutOperacao, data) {
    var grupoGrid = knoutOperacao.basicTable.BuscarRegistros();

    for (var i = 0; i < grupoGrid.length; i++) {
        if (data.Codigo == grupoGrid[i].Codigo) {
            grupoGrid.splice(i, 1);
            break;
        }
    }
    knoutOperacao.basicTable.CarregarGrid(grupoGrid);
}

function limparCamposTipoOperacao() {
    LimparCampos(_tipoOperacao);
}