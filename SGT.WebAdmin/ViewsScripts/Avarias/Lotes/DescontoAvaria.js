/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/MotivoDescontoAvaria.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _descontoAvaria;
var $modalDescontoAvaria;

var DescontoAvaria = function () {
    this.Solicitacao = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    //this.Lote = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo:", idBtnSearch: guid(), required: true });
    this.Desconto = PropertyEntity({ text: "Desconto:", val: ko.observable("0,00"), def: "0,00", type: types.map, getType: typesKnockout.decimal});

    this.Atualizar = PropertyEntity({ eventClick: atualizarDescontoAvariaClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
}


//*******EVENTOS*******
function loadDescontoAvaria() {
    _descontoAvaria = new DescontoAvaria();
    KoBindings(_descontoAvaria, "knockoutDescontoAvaria");

    new BuscarMotivoDescontoAvaria(_descontoAvaria.Motivo);

    $modalDescontoAvaria = $("#divModalDescontoAvaria");
    $modalDescontoAvaria.on('hidden.bs.modal', function () {
        LimparCampos(_descontoAvaria);
    });
}

function atualizarDescontoAvariaClick(e, sender) {
    Salvar(_descontoAvaria, "Lotes/DescontarAvaria", function (arg) {
        if (arg.Success) {
            _gridAvarias.CarregarGrid();
            $modalDescontoAvaria.modal('hide');
            AtualizarValoresDoLote();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
        }
    }, sender, exibirCamposObrigatorios);
}


//*******MÉTODOS*******
function DescontarDaAvaria(dataGrid) {
    // Preenche informacoes
    _descontoAvaria.Solicitacao.val(dataGrid.Codigo);
    _descontoAvaria.Desconto.val(dataGrid.Desconto);
    _descontoAvaria.Motivo.val(dataGrid.MotivoDescricao);
    _descontoAvaria.Motivo.codEntity(dataGrid.MotivoCodigo);

    $modalDescontoAvaria.modal('show');
    $modalDescontoAvaria.one('hidden.bs.modal', function () {
        LimpaDescontoAvaria();
    });
}

function LimpaDescontoAvaria() {
    LimparCampoEntity(_descontoAvaria.Motivo);
    _descontoAvaria.Desconto.val(_descontoAvaria.Desconto.def);
}