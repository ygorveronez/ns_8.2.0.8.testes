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
/// <reference path="TempoDescarregamento.js" />
/// <reference path="CentroDescarregamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridVeiculoPermitido;
var _veiculoPermitido;
var _veiculosPermitidos = new Array();

var VeiculoPermitido = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.ModeloVeiculo = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.CentroDescarregamento.AdicionarModeloVeiculo, idBtnSearch: guid() });
}


//*******EVENTOS*******

function LoadVeiculoPermitido() {
    _veiculoPermitido = new VeiculoPermitido();
    KoBindings(_veiculoPermitido, "knockoutVeiculoPermitido");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirVeiculoPermitidoClick(_veiculoPermitido.ModeloVeiculo, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }];

    _gridVeiculoPermitido = new BasicDataTable(_veiculoPermitido.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarModelosVeicularesCarga(_veiculoPermitido.ModeloVeiculo, function (r) {
        if (r != null) {
            for (var i = 0; i < r.length; i++)
                _veiculosPermitidos.push({ Codigo: r[i].Codigo, Descricao: r[i].Descricao });

            _gridVeiculoPermitido.CarregarGrid(_veiculosPermitidos);
        }
    }, null, null, null, null, _gridVeiculoPermitido);
    _veiculoPermitido.ModeloVeiculo.basicTable = _gridVeiculoPermitido;

    RecarregarGridVeiculoPermitido();
}

function RecarregarGridVeiculoPermitido() {
    _gridVeiculoPermitido.CarregarGrid(_centroDescarregamento.VeiculosPermitidos.val());
}

function ExcluirVeiculoPermitidoClick(knoutVeiculoPermitido, data) {
    var VeiculoPermitidoGrid = knoutVeiculoPermitido.basicTable.BuscarRegistros();

    for (var i = 0; i < VeiculoPermitidoGrid.length; i++) {
        if (data.Codigo == VeiculoPermitidoGrid[i].Codigo) {
            VeiculoPermitidoGrid.splice(i, 1);
            break;
        }
    }

    knoutVeiculoPermitido.basicTable.CarregarGrid(VeiculoPermitidoGrid);
}

function LimparCamposVeiculoPermitido() {
    LimparCampos(_veiculoPermitido);
    _gridVeiculoPermitido.CarregarGrid(new Array());
    _veiculosPermitidos = new Array();
}
