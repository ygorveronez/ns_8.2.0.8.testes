//***********MAPEAMENTO KNOCKOUT***********

var _gridFilialRotaFrete;
var _FilialRotaFrete;

var FilialRotaFrete = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Filial = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.RotaFrete.AdicionarFilial, idBtnSearch: guid() });
};

//**********EVENTOS**********

function LoadFilialRotaFrete() {
    _FilialRotaFrete = new FilialRotaFrete();
    KoBindings(_FilialRotaFrete, "knockoutFilialRotaFrete");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirFilialRotaFreteClick(_FilialRotaFrete.Filial, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "90%" }
    ];

    _gridFilialRotaFrete = new BasicDataTable(_FilialRotaFrete.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarFilial(_FilialRotaFrete.Filial, null, _gridFilialRotaFrete, null, null);

    _FilialRotaFrete.Filial.basicTable = _gridFilialRotaFrete;

    RecarregarGridFilialRotaFrete();
}

function RecarregarGridFilialRotaFrete() {
    _gridFilialRotaFrete.CarregarGrid(_rotaFrete.Filiais.val());
}

function ExcluirFilialRotaFreteClick(knoutFilial, data) {

    var FilialGrid = knoutFilial.basicTable.BuscarRegistros();

    for (var i = 0; i < FilialGrid.length; i++) {
        if (data.Codigo == FilialGrid[i].Codigo) {
            FilialGrid.splice(i, 1);
            break;
        }
    }

    knoutFilial.basicTable.CarregarGrid(FilialGrid);
}

function LimparCamposFilialRotaFrete() {
    LimparCampos(_FilialRotaFrete);
    _gridFilialRotaFrete.CarregarGrid(new Array());
}