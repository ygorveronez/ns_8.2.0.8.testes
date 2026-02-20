//*******MAPEAMENTO KNOUCKOUT*******

var _gridColetaRotaFrete;
var _coletaRotaFrete;

var ColetaRotaFrete = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Coleta = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.RotaFrete.AdicionarColetas, idBtnSearch: guid(), issue: 0 });
};


//*******EVENTOS*******

function LoadColetaRotaFrete() {
    _coletaRotaFrete = new ColetaRotaFrete();
    KoBindings(_coletaRotaFrete, "knockoutColetaRotaFrete");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirColetaRotaFreteClick(_coletaRotaFrete.Coleta, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Latitude", visible: false },
        { data: "Longitude", visible: false },
        { data: "Endereco", visible: false },
        { data: "Numero", visible: false },
        { data: "CEP", visible: false },
        { data: "CodigoIBGE", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "50%" },
        { data: "Localidade", title: Localization.Resources.Logistica.RotaFrete.Localidades, width: "30%" }
    ];

    _gridColetaRotaFrete = new BasicDataTable(_coletaRotaFrete.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_coletaRotaFrete.Coleta, null, null, null, null, _gridColetaRotaFrete);

    _coletaRotaFrete.Coleta.basicTable = _gridColetaRotaFrete;

    RecarregarGridColetaRotaFrete();
}

function RecarregarGridColetaRotaFrete() {
    _gridColetaRotaFrete.CarregarGrid(_rotaFrete.Coletas.val());
}

function ExcluirColetaRotaFreteClick(knoutColeta, data) {

    var coletasGrid = knoutColeta.basicTable.BuscarRegistros();

    for (var i = 0; i < coletasGrid.length; i++) {
        if (data.Codigo == coletasGrid[i].Codigo) {
            coletasGrid.splice(i, 1);
            break;
        }
    }

    knoutColeta.basicTable.CarregarGrid(coletasGrid);
}

function LimparCamposColetaRotaFrete() {
    LimparCampos(_coletaRotaFrete);
    _gridColetaRotaFrete.CarregarGrid(new Array());
}