/// <reference path="../../Consultas/TipoTerceiro.js" />

var _gridTipoTerceiro = null, _tipoTerceiro;

var TipoTerceiro = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.TipoTerceiro = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFrete.AdicionarTipoTerceiro, idBtnSearch: guid() });
};


function LoadTiposTerceiros() {

    _tipoTerceiro = new TipoTerceiro();
    KoBindings(_tipoTerceiro, "knockoutTiposTerceiros");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: function (data) {
                ExcluirTipoTerceiroClick(_tipoTerceiro.TipoTerceiro, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "80%" }
    ];

    _gridTipoTerceiro = new BasicDataTable(_tipoTerceiro.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTipoTerceiro(_tipoTerceiro.TipoTerceiro, null, _gridTipoTerceiro);

    _tipoTerceiro.TipoTerceiro.basicTable = _gridTipoTerceiro;
    _tipoTerceiro.TipoTerceiro.basicTable.CarregarGrid(new Array());
}

function RecarregarGridTipoTerceiro() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_tabelaFrete.TiposTerceiros.val())) {

        $.each(_tabelaFrete.TiposTerceiros.val(), function (i, tipoTerceiro) {
            var tipoTerceiroGrid = new Object();

            tipoTerceiroGrid.Codigo = tipoTerceiro.Codigo;
            tipoTerceiroGrid.Descricao = tipoTerceiro.Descricao;

            data.push(tipoTerceiroGrid);
        });
    }

    _gridTipoTerceiro.CarregarGrid(data);
}


function ExcluirTipoTerceiroClick(knoutTipoTerceiro, data) {
    var tipoTerceiroGrid = knoutTipoTerceiro.basicTable.BuscarRegistros();

    for (var i = 0; i < tipoTerceiroGrid.length; i++) {
        if (data.Codigo == tipoTerceiroGrid[i].Codigo) {
            tipoTerceiroGrid.splice(i, 1);
            break;
        }
    }

    knoutTipoTerceiro.basicTable.CarregarGrid(tipoTerceiroGrid);
}

function LimparCamposTipoTerceiro() {
    LimparCampos(_tipoTerceiro);
}