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

var _gridRegraTipoOperacaoCanalEntrega;
var _regraTipoOperacaoCanalEntrega;

var RegraTipoOperacaoCanalEntrega = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.CanalEntrega = PropertyEntity({ type: types.event, text: "Adicionar Canal Entrega", idBtnSearch: guid(), codEntity: ko.observable(0) });
};

//*******EVENTOS*******

function LoadRegraTipoOperacaoCanalEntrega() {
    _regraTipoOperacaoCanalEntrega = new RegraTipoOperacaoCanalEntrega();
    KoBindings(_regraTipoOperacaoCanalEntrega, "knockoutRegraTipoOperacaoCanalEntrega");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirRegraTipoOperacaoCanalEntregaClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Canal Entrega", width: "70%" },
    ];

    _gridRegraTipoOperacaoCanalEntrega = new BasicDataTable(_regraTipoOperacaoCanalEntrega.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarCanaisEntrega(_regraTipoOperacaoCanalEntrega.CanalEntrega, null, _gridRegraTipoOperacaoCanalEntrega);
    
    RecarregarGridRegraTipoOperacaoCanalEntrega();
}

function RecarregarGridRegraTipoOperacaoCanalEntrega() {
    var data = new Array();
    if (_regraTipoOperacao.CanalEntrega.val() != "") {
        $.each(_regraTipoOperacao.CanalEntrega.val(), function (i, tipoCarga) {
            var tiposRegraTipoOperacaoCanalEntregaGrid = new Object();

            tiposRegraTipoOperacaoCanalEntregaGrid.Codigo = tipoCarga.Codigo;
            tiposRegraTipoOperacaoCanalEntregaGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposRegraTipoOperacaoCanalEntregaGrid);
        });
    }
    _gridRegraTipoOperacaoCanalEntrega.CarregarGrid(data);
}

function ExcluirRegraTipoOperacaoCanalEntregaClick(data) {
    var tiposRegraTipoOperacaoCanalEntregaGrid = _gridRegraTipoOperacaoCanalEntrega.BuscarRegistros();

    for (var i = 0; i < tiposRegraTipoOperacaoCanalEntregaGrid.length; i++) {
        if (data.Codigo == tiposRegraTipoOperacaoCanalEntregaGrid[i].Codigo) {
            tiposRegraTipoOperacaoCanalEntregaGrid.splice(i, 1);
            break;
        }
    }
    _gridRegraTipoOperacaoCanalEntrega.CarregarGrid(tiposRegraTipoOperacaoCanalEntregaGrid);
}

function LimparCamposRegraTipoOperacaoCanalEntrega() {
    LimparCampos(_regraTipoOperacaoCanalEntrega);
    _gridRegraTipoOperacaoCanalEntrega.CarregarGrid(new Array());
}