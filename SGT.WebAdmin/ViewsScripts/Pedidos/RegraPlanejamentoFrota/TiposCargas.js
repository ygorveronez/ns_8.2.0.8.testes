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
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="RegraPlanejamentoFrota.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTiposCargasRegraPlanejamentoFrota;
var _tiposCargasRegraPlanejamentoFrota;

var TiposCargasRegraPlanejamentoFrota = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.TipoCarga = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Carga", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadTiposCargasRegraPlanejamentoFrota() {
    _tiposCargasRegraPlanejamentoFrota = new TiposCargasRegraPlanejamentoFrota();
    KoBindings(_tiposCargasRegraPlanejamentoFrota, "knockoutTiposCargasRegraPlanejamentoFrota");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirTiposCargasClickRegraPlanejamentoFrota(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "15%" },
    ];

    _gridTiposCargasRegraPlanejamentoFrota = new BasicDataTable(_tiposCargasRegraPlanejamentoFrota.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposdeCarga(_tiposCargasRegraPlanejamentoFrota.TipoCarga, null, null, _gridTiposCargasRegraPlanejamentoFrota);

    RecarregarGridTiposCargasRegraPlanejamentoFrota();
}

function RecarregarGridTiposCargasRegraPlanejamentoFrota() {
    var data = new Array();

    if (_regraPlanejamentoFrota.TiposCargas.val() != "")
    {
        $.each(_regraPlanejamentoFrota.TiposCargas.val(), function (i, tipoCarga) {
            var tiposCargasGrid = new Object();

            tiposCargasGrid.Codigo = tipoCarga.Codigo;
            tiposCargasGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposCargasGrid);
        });
    }

    _gridTiposCargasRegraPlanejamentoFrota.CarregarGrid(data);
}

function ExcluirTiposCargasClickRegraPlanejamentoFrota(data) {
    var tiposCargasGrid = _gridTiposCargasRegraPlanejamentoFrota.BuscarRegistros();

    for (var i = 0; i < tiposCargasGrid.length; i++) {
        if (data.Codigo == tiposCargasGrid[i].Codigo) {
            tiposCargasGrid.splice(i, 1);
            break;
        }
    }

    _gridTiposCargasRegraPlanejamentoFrota.CarregarGrid(tiposCargasGrid);
}

function LimparCamposTiposCargasRegraPlanejamentoFrota() {
    LimparCampos(_tiposCargasRegraPlanejamentoFrota);
    _gridTiposCargasRegraPlanejamentoFrota.CarregarGrid(new Array());
}