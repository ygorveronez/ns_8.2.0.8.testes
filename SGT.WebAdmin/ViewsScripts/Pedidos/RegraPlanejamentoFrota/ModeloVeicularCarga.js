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

var _gridModeloVeicularCargaRegraPlanejamentoFrota;
var _modeloVeicularCargaRegraPlanejamentoFrota;

var ModeloVeicularCargaRegraPlanejamentoFrota = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.event, text: "Adicionar Modelo Veicular", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadModeloVeicularCargaRegraPlanejamentoFrota() {
    _modeloVeicularCargaRegraPlanejamentoFrota = new ModeloVeicularCargaRegraPlanejamentoFrota();
    KoBindings(_modeloVeicularCargaRegraPlanejamentoFrota, "knockoutModeloVeicularCargaRegraPlanejamentoFrota");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirModeloVeicularCargaClickRegraPlanejamentoFrota(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "15%" },
    ];

    _gridModeloVeicularCargaRegraPlanejamentoFrota = new BasicDataTable(_modeloVeicularCargaRegraPlanejamentoFrota.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarModelosVeicularesCarga(_modeloVeicularCargaRegraPlanejamentoFrota.ModeloVeicularCarga, null, null, null, null, null, _gridModeloVeicularCargaRegraPlanejamentoFrota);

    RecarregarGridModeloVeicularCargaRegraPlanejamentoFrota();
}

function RecarregarGridModeloVeicularCargaRegraPlanejamentoFrota() {
    var data = new Array();

    if (_regraPlanejamentoFrota.ModelosVeicularesCarga.val() != "") {
        $.each(_regraPlanejamentoFrota.ModelosVeicularesCarga.val(), function (i, tipoCarga) {
            var tiposCargasGrid = new Object();

            tiposCargasGrid.Codigo = tipoCarga.Codigo;
            tiposCargasGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposCargasGrid);
        });
    }

    _gridModeloVeicularCargaRegraPlanejamentoFrota.CarregarGrid(data);
}

function ExcluirModeloVeicularCargaClickRegraPlanejamentoFrota(data) {
    var modelosVeicularesCarga = _gridModeloVeicularCargaRegraPlanejamentoFrota.BuscarRegistros();

    for (var i = 0; i < modelosVeicularesCarga.length; i++) {
        if (data.Codigo == modelosVeicularesCarga[i].Codigo) {
            modelosVeicularesCarga.splice(i, 1);
            break;
        }
    }

    _gridModeloVeicularCargaRegraPlanejamentoFrota.CarregarGrid(modelosVeicularesCarga);
}

function LimparCamposModeloVeicularCargaRegraPlanejamentoFrota() {
    LimparCampos(_modeloVeicularCargaRegraPlanejamentoFrota);
    _gridModeloVeicularCargaRegraPlanejamentoFrota.CarregarGrid(new Array());
}