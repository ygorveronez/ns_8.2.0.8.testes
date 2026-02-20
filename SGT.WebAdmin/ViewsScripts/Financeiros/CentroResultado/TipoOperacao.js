/// <reference path="../../Consultas/TipoOperacao.js" />

//Mapeamento Knockout

var _tipoOperacao;
var _gridTipoOperacao;

var TipoOperacao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.TipoOperacao = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Operação", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
}

//Eventos
function LoadTipoOperacao() {
    _tipoOperacao = new TipoOperacao();
    KoBindings(_tipoOperacao, "knockoutTipoOperacao");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirTipoOperacaoClick(_tipoOperacao.TipoOperacao, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "100%" },
    ];

    _gridTipoOperacao = new BasicDataTable(_tipoOperacao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposOperacao(_tipoOperacao.TipoOperacao, null, null, null, _gridTipoOperacao, null, null, null, null);

    _tipoOperacao.TipoOperacao.basicTable = _gridTipoOperacao;

    RecarregarGridTipoOperacao();
}

function ExcluirTipoOperacaoClick(knoutTipoOperacao, data) {
    var tipoOperacaoGrid = knoutTipoOperacao.basicTable.BuscarRegistros();

    for (var i = 0; i < tipoOperacaoGrid.length; i++) {
        if (data.Codigo == tipoOperacaoGrid[i].Codigo) {
            tipoOperacaoGrid.splice(i, 1);
            break;
        }
    }

    knoutTipoOperacao.basicTable.CarregarGrid(tipoOperacaoGrid);
}

function RecarregarGridTipoOperacao() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_centroResultado.TiposOperacao.val())) {
        $.each(_centroResultado.TiposOperacao.val(), function (i, tipoOperacao) {
            var tipoOperacaoGrid = new Object();

            tipoOperacaoGrid.Codigo = tipoOperacao.Codigo;
            tipoOperacaoGrid.Descricao = tipoOperacao.Descricao;

            data.push(tipoOperacaoGrid);
        });
    }
    _gridTipoOperacao.CarregarGrid(data);
}

function LimparCamposTipoOperacao() {
    LimparCampos(_tipoOperacao);
}