//***********MAPEAMENTO KNOCKOUT***********

var _gridTipoCargaRotaFrete;
var _TipoCargaRotaFrete;

var TipoCargaRotaFrete = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.TipoCarga = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.RotaFrete.AdicionarTipoCarga, idBtnSearch: guid() });
};

//**********EVENTOS**********

function LoadTipoCargaRotaFrete() {
    _TipoCargaRotaFrete = new TipoCargaRotaFrete();
    KoBindings(_TipoCargaRotaFrete, "knockoutTipoCargaRotaFrete");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirTipoCargaRotaFreteClick(_TipoCargaRotaFrete.TipoCarga, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "90%" }
    ];

    _gridTipoCargaRotaFrete = new BasicDataTable(_TipoCargaRotaFrete.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposdeCarga(_TipoCargaRotaFrete.TipoCarga, null, null, _gridTipoCargaRotaFrete);

    _TipoCargaRotaFrete.TipoCarga.basicTable = _gridTipoCargaRotaFrete;

    RecarregarGridTipoCargaRotaFrete();
}

function RecarregarGridTipoCargaRotaFrete() {
    _gridTipoCargaRotaFrete.CarregarGrid(_rotaFrete.TipoCargas.val());
}

function ExcluirTipoCargaRotaFreteClick(knoutTipoCarga, data) {

    var TipoCargaGrid = knoutTipoCarga.basicTable.BuscarRegistros();

    for (var i = 0; i < TipoCargaGrid.length; i++) {
        if (data.Codigo == TipoCargaGrid[i].Codigo) {
            TipoCargaGrid.splice(i, 1);
            break;
        }
    }

    knoutTipoCarga.basicTable.CarregarGrid(TipoCargaGrid);
}

function LimparCamposTipoCargaRotaFrete() {
    LimparCampos(_TipoCargaRotaFrete);
    _gridTipoCargaRotaFrete.CarregarGrid(new Array());
}