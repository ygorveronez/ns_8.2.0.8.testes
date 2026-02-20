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
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="RegraPlanejamentoFrota.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridModeloVeicularCargaReboqueRegraPlanejamentoFrota;
var _modeloVeicularCargaReboqueRegraPlanejamentoFrota;

var ModeloVeicularCargaReboqueRegraPlanejamentoFrota = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.ModeloVeicularCargaReboque = PropertyEntity({ type: types.event, text: "Adicionar Modelo Veicular", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadModeloVeicularReboqueCargaRegraPlanejamentoFrota() {
    _modeloVeicularCargaReboqueRegraPlanejamentoFrota = new ModeloVeicularCargaReboqueRegraPlanejamentoFrota();
    KoBindings(_modeloVeicularCargaReboqueRegraPlanejamentoFrota, "knockoutModeloVeicularCargaReboqueRegraPlanejamentoFrota");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirModeloVeicularCargaReboqueClickRegraPlanejamentoFrota(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "15%" },
    ];

    _gridModeloVeicularCargaReboqueRegraPlanejamentoFrota = new BasicDataTable(_modeloVeicularCargaReboqueRegraPlanejamentoFrota.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarModelosVeicularesCarga(_modeloVeicularCargaReboqueRegraPlanejamentoFrota.ModeloVeicularCargaReboque, null, null, null, null, null, _gridModeloVeicularCargaReboqueRegraPlanejamentoFrota);

    RecarregarGridModeloVeicularCargaReboqueRegraPlanejamentoFrota();
}

function RecarregarGridModeloVeicularCargaReboqueRegraPlanejamentoFrota() {
    var data = new Array();

    if (_regraPlanejamentoFrota.ModelosVeicularesReboqueCarga.val() != "") {
        $.each(_regraPlanejamentoFrota.ModelosVeicularesReboqueCarga.val(), function (i, tipoCarga) {
            var tiposCargasGrid = new Object();

            tiposCargasGrid.Codigo = tipoCarga.Codigo;
            tiposCargasGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposCargasGrid);
        });
    }

    _gridModeloVeicularCargaReboqueRegraPlanejamentoFrota.CarregarGrid(data);
}

function ExcluirModeloVeicularCargaReboqueClickRegraPlanejamentoFrota(data) {
    var tiposCargasGrid = _gridModeloVeicularCargaReboqueRegraPlanejamentoFrota.BuscarRegistros();

    for (var i = 0; i < tiposCargasGrid.length; i++) {
        if (data.Codigo == tiposCargasGrid[i].Codigo) {
            tiposCargasGrid.splice(i, 1);
            break;
        }
    }

    _gridModeloVeicularCargaReboqueRegraPlanejamentoFrota.CarregarGrid(tiposCargasGrid);
}

function LimparCamposModeloVeicularCargaReboqueRegraPlanejamentoFrota() {
    LimparCampos(_modeloVeicularCargaReboqueRegraPlanejamentoFrota);
    _gridModeloVeicularCargaReboqueRegraPlanejamentoFrota.CarregarGrid(new Array());
}