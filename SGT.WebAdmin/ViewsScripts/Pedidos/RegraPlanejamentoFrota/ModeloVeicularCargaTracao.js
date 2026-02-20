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

var _gridModeloVeicularCargaTracaoRegraPlanejamentoFrota;
var _modeloVeicularCargaTracaoRegraPlanejamentoFrota;

var ModeloVeicularCargaTracaoRegraPlanejamentoFrota = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.ModeloVeicularCargaTracao = PropertyEntity({ type: types.event, text: "Adicionar Modelo Veicular", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadModeloVeicularTracaoCargaRegraPlanejamentoFrota() {
    _modeloVeicularCargaTracaoRegraPlanejamentoFrota = new ModeloVeicularCargaTracaoRegraPlanejamentoFrota();
    KoBindings(_modeloVeicularCargaTracaoRegraPlanejamentoFrota, "knockoutModeloVeicularCargaTracaoRegraPlanejamentoFrota");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirModeloVeicularCargaTracaoClickRegraPlanejamentoFrota(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "15%" },
    ];

    _gridModeloVeicularCargaTracaoRegraPlanejamentoFrota = new BasicDataTable(_modeloVeicularCargaTracaoRegraPlanejamentoFrota.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarModelosVeicularesCarga(_modeloVeicularCargaTracaoRegraPlanejamentoFrota.ModeloVeicularCargaTracao, null, null, null, null, null, _gridModeloVeicularCargaTracaoRegraPlanejamentoFrota);

    RecarregarGridModeloVeicularCargaTracaoRegraPlanejamentoFrota();
}

function RecarregarGridModeloVeicularCargaTracaoRegraPlanejamentoFrota() {
    var data = new Array();

    if (_regraPlanejamentoFrota.ModelosVeicularesTracaoCarga.val() != "") {
        $.each(_regraPlanejamentoFrota.ModelosVeicularesTracaoCarga.val(), function (i, tipoCarga) {
            var tiposCargasGrid = new Object();

            tiposCargasGrid.Codigo = tipoCarga.Codigo;
            tiposCargasGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposCargasGrid);
        });
    }

    _gridModeloVeicularCargaTracaoRegraPlanejamentoFrota.CarregarGrid(data);
}

function ExcluirModeloVeicularCargaTracaoClickRegraPlanejamentoFrota(data) {
    var tiposCargasGrid = _gridModeloVeicularCargaTracaoRegraPlanejamentoFrota.BuscarRegistros();

    for (var i = 0; i < tiposCargasGrid.length; i++) {
        if (data.Codigo == tiposCargasGrid[i].Codigo) {
            tiposCargasGrid.splice(i, 1);
            break;
        }
    }

    _gridModeloVeicularCargaTracaoRegraPlanejamentoFrota.CarregarGrid(tiposCargasGrid);
}

function LimparCamposModeloVeicularCargaTracaoRegraPlanejamentoFrota() {
    LimparCampos(_modeloVeicularCargaTracaoRegraPlanejamentoFrota);
    _gridModeloVeicularCargaTracaoRegraPlanejamentoFrota.CarregarGrid(new Array());
}