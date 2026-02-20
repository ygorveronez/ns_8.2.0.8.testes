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
/// <reference path="../../Consultas/NivelCooperado.js" />
/// <reference path="RegraPlanejamentoFrota.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridNivelCooperadoRegraPlanejamentoFrota;
var _nivelCooperadoRegraPlanejamentoFrota;

var NivelCooperadoRegraPlanejamentoFrota = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.NivelCooperado = PropertyEntity({ type: types.event, text: "Adicionar Modelo Veicular", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadNivelCooperadoRegraPlanejamentoFrota() {
    _nivelCooperadoRegraPlanejamentoFrota = new NivelCooperadoRegraPlanejamentoFrota();
    KoBindings(_nivelCooperadoRegraPlanejamentoFrota, "knockoutNivelCooperadoRegraPlanejamentoFrota");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirNivelCooperadoClickRegraPlanejamentoFrota(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "15%" },
    ];

    _gridNivelCooperadoRegraPlanejamentoFrota = new BasicDataTable(_nivelCooperadoRegraPlanejamentoFrota.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTipoTerceiro(_nivelCooperadoRegraPlanejamentoFrota.NivelCooperado, null, _gridNivelCooperadoRegraPlanejamentoFrota);

    RecarregarGridNivelCooperadoRegraPlanejamentoFrota();
}

function RecarregarGridNivelCooperadoRegraPlanejamentoFrota() {
    var data = new Array();

    if (_regraPlanejamentoFrota.NiveisCooperados.val() != "") {
        $.each(_regraPlanejamentoFrota.NiveisCooperados.val(), function (i, tipoCarga) {
            var tiposCargasGrid = new Object();

            tiposCargasGrid.Codigo = tipoCarga.Codigo;
            tiposCargasGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposCargasGrid);
        });
    }

    _gridNivelCooperadoRegraPlanejamentoFrota.CarregarGrid(data);
}

function ExcluirNivelCooperadoClickRegraPlanejamentoFrota(data) {
    var niveisCooperados = _gridNivelCooperadoRegraPlanejamentoFrota.BuscarRegistros();

    for (var i = 0; i < niveisCooperados.length; i++) {
        if (data.Codigo == niveisCooperados[i].Codigo) {
            niveisCooperados.splice(i, 1);
            break;
        }
    }

    _gridNivelCooperadoRegraPlanejamentoFrota.CarregarGrid(niveisCooperados);
}

function LimparCamposNivelCooperadoRegraPlanejamentoFrota() {
    LimparCampos(_nivelCooperadoRegraPlanejamentoFrota);
    _gridNivelCooperadoRegraPlanejamentoFrota.CarregarGrid(new Array());
}