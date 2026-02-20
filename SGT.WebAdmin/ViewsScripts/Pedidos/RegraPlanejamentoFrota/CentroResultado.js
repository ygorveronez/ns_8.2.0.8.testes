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
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="RegraPlanejamentoFrota.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCentroResultadoRegraPlanejamentoFrota;
var _centroResultadoRegraPlanejamentoFrota;

var CentroResultadoRegraPlanejamentoFrota = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.CentroResultado = PropertyEntity({ type: types.event, text: "Adicionar Centro de Resultado", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadCentroResultadoRegraPlanejamentoFrota() {
    _centroResultadoRegraPlanejamentoFrota = new CentroResultadoRegraPlanejamentoFrota();
    KoBindings(_centroResultadoRegraPlanejamentoFrota, "knockoutCentroResultadoRegraPlanejamentoFrota");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirCentroResultadoClickRegraPlanejamentoFrota(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "15%" },
    ];

    _gridCentroResultadoRegraPlanejamentoFrota = new BasicDataTable(_centroResultadoRegraPlanejamentoFrota.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarCentroResultado(_centroResultadoRegraPlanejamentoFrota.CentroResultado, null, null, null, null, null, _gridCentroResultadoRegraPlanejamentoFrota);

    RecarregarGridCentroResultadoRegraPlanejamentoFrota();
}

function RecarregarGridCentroResultadoRegraPlanejamentoFrota() {
    var data = new Array();

    if (_regraPlanejamentoFrota.CentrosResultado.val() != "") {
        $.each(_regraPlanejamentoFrota.CentrosResultado.val(), function (i, tipoCarga) {
            var tiposCargasGrid = new Object();

            tiposCargasGrid.Codigo = tipoCarga.Codigo;
            tiposCargasGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposCargasGrid);
        });
    }

    _gridCentroResultadoRegraPlanejamentoFrota.CarregarGrid(data);
}

function ExcluirCentroResultadoClickRegraPlanejamentoFrota(data) {
    var tiposCargasGrid = _gridCentroResultadoRegraPlanejamentoFrota.BuscarRegistros();

    for (var i = 0; i < tiposCargasGrid.length; i++) {
        if (data.Codigo == tiposCargasGrid[i].Codigo) {
            tiposCargasGrid.splice(i, 1);
            break;
        }
    }

    _gridCentroResultadoRegraPlanejamentoFrota.CarregarGrid(tiposCargasGrid);
}

function LimparCamposCentroResultadoRegraPlanejamentoFrota() {
    LimparCampos(_centroResultadoRegraPlanejamentoFrota);
    _gridCentroResultadoRegraPlanejamentoFrota.CarregarGrid(new Array());
}