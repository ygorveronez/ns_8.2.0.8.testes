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

var _gridRegraTipoOperacaoCanalVenda;
var _regraTipoOperacaoCanalVenda;

var RegraTipoOperacaoCanalVenda = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.CanalVenda = PropertyEntity({ type: types.event, text: "Adicionar Canal Venda", idBtnSearch: guid(), codEntity: ko.observable(0) });
};

//*******EVENTOS*******

function LoadRegraTipoOperacaoCanalVenda() {
    _regraTipoOperacaoCanalVenda = new RegraTipoOperacaoCanalVenda();
    KoBindings(_regraTipoOperacaoCanalVenda, "knockoutRegraTipoOperacaoCanalVenda");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirRegraTipoOperacaoCanalVendaClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Canal Venda", width: "70%" },
    ];

    _gridRegraTipoOperacaoCanalVenda = new BasicDataTable(_regraTipoOperacaoCanalVenda.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarCanaisVenda(_regraTipoOperacaoCanalVenda.CanalVenda, null, _gridRegraTipoOperacaoCanalVenda );

    RecarregarGridRegraTipoOperacaoCanalVenda();
}

function RecarregarGridRegraTipoOperacaoCanalVenda() {
    var data = new Array();
    if (_regraTipoOperacao.CanalVenda.val() != "") {
        $.each(_regraTipoOperacao.CanalVenda.val(), function (i, tipoCarga) {
            var tiposRegraTipoOperacaoCanalVendaGrid = new Object();

            tiposRegraTipoOperacaoCanalVendaGrid.Codigo = tipoCarga.Codigo;
            tiposRegraTipoOperacaoCanalVendaGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposRegraTipoOperacaoCanalVendaGrid);
        });
    }
    _gridRegraTipoOperacaoCanalVenda.CarregarGrid(data);
}

function ExcluirRegraTipoOperacaoCanalVendaClick(data) {
    var tiposRegraTipoOperacaoCanalVendaGrid = _gridRegraTipoOperacaoCanalVenda.BuscarRegistros();

    for (var i = 0; i < tiposRegraTipoOperacaoCanalVendaGrid.length; i++) {
        if (data.Codigo == tiposRegraTipoOperacaoCanalVendaGrid[i].Codigo) {
            tiposRegraTipoOperacaoCanalVendaGrid.splice(i, 1);
            break;
        }
    }
    _gridRegraTipoOperacaoCanalVenda.CarregarGrid(tiposRegraTipoOperacaoCanalVendaGrid);
}

function LimparCamposRegraTipoOperacaoCanalVenda() {
    LimparCampos(_regraTipoOperacaoCanalVenda);
    _gridRegraTipoOperacaoCanalVenda.CarregarGrid(new Array());
}