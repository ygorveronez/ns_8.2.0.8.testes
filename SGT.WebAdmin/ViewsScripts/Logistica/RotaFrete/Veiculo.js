//***********MAPEAMENTO KNOCKOUT***********

var _gridVeiculoRotaFrete;
var _veiculoRotaFrete;

var VeiculoRotaFrete = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Veiculo = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.RotaFrete.AdicionarVeiculo, idBtnSearch: guid() });
};

//**********EVENTOS**********

function LoadVeiculoRotaFrete() {
    _veiculoRotaFrete = new VeiculoRotaFrete();
    KoBindings(_veiculoRotaFrete, "knockoutVeiculoRotaFrete");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirVeiculoRotaFreteClick(_veiculoRotaFrete.Veiculo, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "90%" }
    ];

    _gridVeiculoRotaFrete = new BasicDataTable(_veiculoRotaFrete.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarVeiculos(_veiculoRotaFrete.Veiculo, null, null, null, null, null, null, null, null, null, null, null, null, _gridVeiculoRotaFrete);

    _veiculoRotaFrete.Veiculo.basicTable = _gridVeiculoRotaFrete;

    RecarregarGridVeiculoRotaFrete();
}

function RecarregarGridVeiculoRotaFrete() {
    _gridVeiculoRotaFrete.CarregarGrid(_rotaFrete.Veiculos.val());
}

function ExcluirVeiculoRotaFreteClick(knoutVeiculo, data) {

    var veiculoGrid = knoutVeiculo.basicTable.BuscarRegistros();
    
    for (var i = 0; i < veiculoGrid.length; i++) {
        if (data.Codigo == veiculoGrid[i].Codigo) {
            veiculoGrid.splice(i, 1);
            break;
        }
    }

    knoutVeiculo.basicTable.CarregarGrid(veiculoGrid);
}

function LimparCamposVeiculoRotaFrete() {
    LimparCampos(_veiculoRotaFrete);
    _gridVeiculoRotaFrete.CarregarGrid(new Array());
}