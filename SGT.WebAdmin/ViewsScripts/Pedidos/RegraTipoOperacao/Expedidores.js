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
/// <reference path="RegraTipoOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegraTipoOperacaoExpedidores;
var _regraTipoOperacaoExpedidores;

var RegraTipoOperacaoExpedidores = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Expedidores = PropertyEntity({ type: types.event, text: "Adicionar Expedidor", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadRegraTipoOperacaoExpedidores() {
    _regraTipoOperacaoExpedidores = new RegraTipoOperacaoExpedidores();
    KoBindings(_regraTipoOperacaoExpedidores, "knockoutRegraTipoOperacaoExpedidor");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirRegraTipoOperacaoExpedidoresClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Expedidor", width: "70%" },
    ];

    _gridRegraTipoOperacaoExpedidores = new BasicDataTable(_regraTipoOperacaoExpedidores.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_regraTipoOperacaoExpedidores.Expedidores, null,null,null,null, _gridRegraTipoOperacaoExpedidores);

    RecarregarGridRegraTipoOperacaoExpedidores();
}

function RecarregarGridRegraTipoOperacaoExpedidores() {
    var data = new Array();
    if (_regraTipoOperacao.Expedidores.val() != "") {
        $.each(_regraTipoOperacao.Expedidores.val(), function (i, tipoCarga) {
            var tiposRegraTipoOperacaoExpedidorGrid = new Object();

            tiposRegraTipoOperacaoExpedidorGrid.Codigo = tipoCarga.Codigo;
            tiposRegraTipoOperacaoExpedidorGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposRegraTipoOperacaoExpedidorGrid);
        });
    }
    _gridRegraTipoOperacaoExpedidores.CarregarGrid(data);
}

function ExcluirRegraTipoOperacaoExpedidoresClick(data) {
    var tiposRegraTipoOperacaoExpedidorGrid = _gridRegraTipoOperacaoExpedidores.BuscarRegistros();

    for (var i = 0; i < tiposRegraTipoOperacaoExpedidorGrid.length; i++) {
        if (data.Codigo == tiposRegraTipoOperacaoExpedidorGrid[i].Codigo) {
            tiposRegraTipoOperacaoExpedidorGrid.splice(i, 1);
            break;
        }
    }
    _gridRegraTipoOperacaoExpedidores.CarregarGrid(tiposRegraTipoOperacaoExpedidorGrid);
}

function LimparCamposRegraTipoOperacaoExpedidor() {
    LimparCampos(_regraTipoOperacaoExpedidores);
    _gridRegraTipoOperacaoExpedidores.CarregarGrid(new Array());
}