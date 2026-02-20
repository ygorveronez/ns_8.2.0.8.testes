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
/// <reference path="../../Consultas/TecnologiaRastreador.js" />
/// <reference path="RegraPlanejamentoFrota.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTecnologiaRastreadorRegraPlanejamentoFrota;
var _tecnologiaRastreadorRegraPlanejamentoFrota;

var TecnologiaRastreadorRegraPlanejamentoFrota = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.TecnologiaRastreador = PropertyEntity({ type: types.event, text: "Adicionar Tecnologia Rastreador", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadTecnologiaRastreadorRegraPlanejamentoFrota() {
    _tecnologiaRastreadorRegraPlanejamentoFrota = new TecnologiaRastreadorRegraPlanejamentoFrota();
    KoBindings(_tecnologiaRastreadorRegraPlanejamentoFrota, "knockoutTecnologiaRastreadorRegraPlanejamentoFrota");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirTecnologiaRastreadorClickRegraPlanejamentoFrota(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "15%" },
    ];

    _gridTecnologiaRastreadorRegraPlanejamentoFrota = new BasicDataTable(_tecnologiaRastreadorRegraPlanejamentoFrota.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTecnologiaRastreador(_tecnologiaRastreadorRegraPlanejamentoFrota.TecnologiaRastreador, null, _gridTecnologiaRastreadorRegraPlanejamentoFrota);

    RecarregarGridTecnologiaRastreadorRegraPlanejamentoFrota();
}

function RecarregarGridTecnologiaRastreadorRegraPlanejamentoFrota() {
    var data = new Array();

    if (_regraPlanejamentoFrota.TecnologiaRastreadores.val() != "") {
        $.each(_regraPlanejamentoFrota.TecnologiaRastreadores.val(), function (i, tipoCarga) {
            var tiposCargasGrid = new Object();

            tiposCargasGrid.Codigo = tipoCarga.Codigo;
            tiposCargasGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposCargasGrid);
        });
    }

    _gridTecnologiaRastreadorRegraPlanejamentoFrota.CarregarGrid(data);
}

function ExcluirTecnologiaRastreadorClickRegraPlanejamentoFrota(data) {
    var tiposCargasGrid = _gridTecnologiaRastreadorRegraPlanejamentoFrota.BuscarRegistros();

    for (var i = 0; i < tiposCargasGrid.length; i++) {
        if (data.Codigo == tiposCargasGrid[i].Codigo) {
            tiposCargasGrid.splice(i, 1);
            break;
        }
    }

    _gridTecnologiaRastreadorRegraPlanejamentoFrota.CarregarGrid(tiposCargasGrid);
}

function LimparCamposTecnologiaRastreadorRegraPlanejamentoFrota() {
    LimparCampos(_tecnologiaRastreadorRegraPlanejamentoFrota);
    _gridTecnologiaRastreadorRegraPlanejamentoFrota.CarregarGrid(new Array());
}