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
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="RegraPlanejamentoFrota.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoOperacaoRegraPlanejamentoFrota;
var _tipoOperacaoRegraPlanejamentoFrota;

var GruposPessoasRegraPlanejamentoFrota = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Operação", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadTipoOperacaoRegraPlanejamentoFrota() {
    _tipoOperacaoRegraPlanejamentoFrota = new GruposPessoasRegraPlanejamentoFrota();
    KoBindings(_tipoOperacaoRegraPlanejamentoFrota, "knockoutTipoOperacaoRegraPlanejamentoFrota");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirTipoOperacaoClickRegraPlanejamentoFrota(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "15%" },
    ];

    _gridTipoOperacaoRegraPlanejamentoFrota = new BasicDataTable(_tipoOperacaoRegraPlanejamentoFrota.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposOperacao(_tipoOperacaoRegraPlanejamentoFrota.TipoOperacao, null, null, null, _gridTipoOperacaoRegraPlanejamentoFrota);

    RecarregarGridTipoOperacaoRegraPlanejamentoFrota();
}

function RecarregarGridTipoOperacaoRegraPlanejamentoFrota() {
    var data = new Array();

    if (_regraPlanejamentoFrota.TiposOperacao.val() != "")
    {
        $.each(_regraPlanejamentoFrota.TiposOperacao.val(), function (i, tipoCarga) {
            var tiposCargasGrid = new Object();

            tiposCargasGrid.Codigo = tipoCarga.Codigo;
            tiposCargasGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposCargasGrid);
        });
    }

    _gridTipoOperacaoRegraPlanejamentoFrota.CarregarGrid(data);
}

function ExcluirTipoOperacaoClickRegraPlanejamentoFrota(data) {
    var tiposCargasGrid = _gridTipoOperacaoRegraPlanejamentoFrota.BuscarRegistros();

    for (var i = 0; i < tiposCargasGrid.length; i++) {
        if (data.Codigo == tiposCargasGrid[i].Codigo) {
            tiposCargasGrid.splice(i, 1);
            break;
        }
    }

    _gridTipoOperacaoRegraPlanejamentoFrota.CarregarGrid(tiposCargasGrid);
}

function LimparCamposTipoOperacaoRegraPlanejamentoFrota() {
    LimparCampos(_tipoOperacaoRegraPlanejamentoFrota);
    _gridTipoOperacaoRegraPlanejamentoFrota.CarregarGrid(new Array());
}