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
/// <reference path="../../Consultas/TipoTerceiro.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoTerceiro;
var _tiposTerceiros;

var TiposTerceiro = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Terceiro = PropertyEntity({ type: types.event, text: "Adicionar Tipos de Terceiro", idBtnSearch: guid() });
}

//*******EVENTOS*******

function loadTiposTerceiros() {
   _tiposTerceiros = new TiposTerceiro();
    KoBindings(_tiposTerceiros, "knockoutTiposTerceiro");
    
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                excluirTerceiroClick(_tiposTerceiros.Terceiro, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "20%" },
        { data: "DescricaoSituacao", title: Localization.Resources.Gerais.Geral.Situacao, width: "20%" }
    ];

    _gridTipoTerceiro = new BasicDataTable(_tiposTerceiros.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTipoTerceiro(_tiposTerceiros.Terceiro, null, _gridTipoTerceiro);
    _tiposTerceiros.Terceiro.basicTable = _gridTipoTerceiro;

    RecarregarGridTipoTerceiro();
}

function RecarregarGridTipoTerceiro() {
    var data = new Array();
    debugger;
    if (!string.IsNullOrWhiteSpace(_tipoOperacao.TiposTerceiros.val())) {

        $.each(_tipoOperacao.TiposTerceiros.val(), function (i, tiposTerceiros) {
            var tipoTerceiroGrid = new Object();

            tipoTerceiroGrid.Codigo = tiposTerceiros.Codigo;
            tipoTerceiroGrid.Descricao = tiposTerceiros.Descricao;
            tipoTerceiroGrid.DescricaoSituacao = tiposTerceiros.DescricaoSituacao;

            data.push(tipoTerceiroGrid);
        });
    }
    _tipoOperacao.TiposTerceiros.val(RetornarObjetoPesquisa(_tiposTerceiros));
    _gridTipoTerceiro.CarregarGrid(data);
}


function excluirTerceiroClick(knoutTiposTerceiros, data) {
    var tiposTerceirosGrid = knoutTiposTerceiros.basicTable.BuscarRegistros();

    for (var i = 0; i < tiposTerceirosGrid.length; i++) {
        if (data.Codigo == tiposTerceirosGrid[i].Codigo) {
            tiposTerceirosGrid.splice(i, 1);
            break;
        }
    }
    knoutTiposTerceiros.basicTable.CarregarGrid(tiposTerceirosGrid);
}

function limparCamposTiposTerceiro() {
    LimparCampos(_tiposTerceiros);
}