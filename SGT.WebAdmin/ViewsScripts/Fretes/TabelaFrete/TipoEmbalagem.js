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

var _gridTipoEmbalagem;
var _tipoEmbalagem;

var TipoEmbalagem = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.TipoEmbalagem = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFrete.AdicionarTipoEmbalagem, idBtnSearch: guid() });
}


//*******EVENTOS*******

function loadTipoEmbalagem() {
    _tipoEmbalagem = new TipoEmbalagem();
    KoBindings(_tipoEmbalagem, "knockoutTipoEmbalagem");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: function (data) {
                excluirTipoEmbalagemClick(_tipoEmbalagem.TipoEmbalagem, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "80%" }];

    _gridTipoEmbalagem = new BasicDataTable(_tipoEmbalagem.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTipoEmbalagem(_tipoEmbalagem.TipoEmbalagem, null, _gridTipoEmbalagem);
    _tipoEmbalagem.TipoEmbalagem.basicTable = _gridTipoEmbalagem;

    recarregarGridTipoEmbalagem();
}

function recarregarGridTipoEmbalagem() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_tabelaFrete.TipoEmbalagens.val())) {

        $.each(_tabelaFrete.TipoEmbalagens.val(), function (i, tipoEmbalagem) {
            var tipoEmbalagemGrid = new Object();

            tipoEmbalagemGrid.Codigo = tipoEmbalagem.TipoEmbalagem.Codigo;
            tipoEmbalagemGrid.Descricao = tipoEmbalagem.TipoEmbalagem.Descricao;

            data.push(tipoEmbalagemGrid);
        });
    }

    _gridTipoEmbalagem.CarregarGrid(data);
}


function excluirTipoEmbalagemClick(knoutTipoEmbalagem, data) {
    var tipoEmbalagemGrid = knoutTipoEmbalagem.basicTable.BuscarRegistros();

    for (var i = 0; i < tipoEmbalagemGrid.length; i++) {
        if (data.Codigo == tipoEmbalagemGrid[i].Codigo) {
            tipoEmbalagemGrid.splice(i, 1);
            break;
        }
    }

    knoutTipoEmbalagem.basicTable.CarregarGrid(tipoEmbalagemGrid);
}

function limparCamposTipoEmbalagem() {
    LimparCampos(_tipoEmbalagem);
}