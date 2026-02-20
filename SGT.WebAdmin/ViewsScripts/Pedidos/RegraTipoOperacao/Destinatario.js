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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="RegraTipoOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegraTipoOperacaoDestinatario;
var _regraTipoOperacaoDestinatario;

var RegraTipoOperacaoDestinatario = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Destinatario = PropertyEntity({ type: types.event, text: "Adicionar Destinatario", idBtnSearch: guid(), codEntity: ko.observable(0) });
};

//*******EVENTOS*******

function LoadRegraTipoOperacaoDestinatario() {
    _regraTipoOperacaoDestinatario = new RegraTipoOperacaoDestinatario();
    KoBindings(_regraTipoOperacaoDestinatario, "knockoutRegraTipoOperacaoDestinatario");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirRegraTipoOperacaoDestinatarioClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Destinatario", width: "70%" },
    ];

    _gridRegraTipoOperacaoDestinatario = new BasicDataTable(_regraTipoOperacaoDestinatario.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_regraTipoOperacaoDestinatario.Destinatario, null, null, null, null, _gridRegraTipoOperacaoDestinatario);

    RecarregarGridRegraTipoOperacaoDestinatario();
}

function RecarregarGridRegraTipoOperacaoDestinatario() {
    var data = new Array();
    if (_regraTipoOperacao.Destinatario.val() != "") {
        $.each(_regraTipoOperacao.Destinatario.val(), function (i, tipoCarga) {
            var tiposRegraTipoOperacaoDestinatarioGrid = new Object();

            tiposRegraTipoOperacaoDestinatarioGrid.Codigo = tipoCarga.Codigo;
            tiposRegraTipoOperacaoDestinatarioGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposRegraTipoOperacaoDestinatarioGrid);
        });
    }
    _gridRegraTipoOperacaoDestinatario.CarregarGrid(data);
}

function ExcluirRegraTipoOperacaoDestinatarioClick(data) {
    var tiposRegraTipoOperacaoDestinatarioGrid = _gridRegraTipoOperacaoDestinatario.BuscarRegistros();

    for (var i = 0; i < tiposRegraTipoOperacaoDestinatarioGrid.length; i++) {
        if (data.Codigo == tiposRegraTipoOperacaoDestinatarioGrid[i].Codigo) {
            tiposRegraTipoOperacaoDestinatarioGrid.splice(i, 1);
            break;
        }
    }
    _gridRegraTipoOperacaoDestinatario.CarregarGrid(tiposRegraTipoOperacaoDestinatarioGrid);
}

function LimparCamposRegraTipoOperacaoDestinatario() {
    LimparCampos(_regraTipoOperacaoDestinatario);
    _gridRegraTipoOperacaoDestinatario.CarregarGrid(new Array());
}