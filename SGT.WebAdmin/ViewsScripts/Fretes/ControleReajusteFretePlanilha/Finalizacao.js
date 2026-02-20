/// <reference path="../../../js/libs/jquery-2.1.1.js" />
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


//*******MAPEAMENTO KNOUCKOUT*******
var _finalizacao;

var Finalizacao = function () {
    this.ConfirmarFinalizacao = PropertyEntity({ eventClick: confirmarFinalizacaoClick, type: types.event, text: "Confirmar Finalização do Reajuste", visible: ko.observable(true) });
}

//*******EVENTOS*******
function loadFinalizacao() {
    //-- Knouckout
    // Instancia objeto principal
    _finalizacao = new Finalizacao();
    KoBindings(_finalizacao, "knockoutFinalizacao");
}

function confirmarFinalizacaoClick(e, sender) {
    executarReST("ControleReajusteFretePlanilha/ConfirmarFinalizacao", { Codigo: _controleReajusteFretePlanilha.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Reajuste Finalizado.");
                _gridControleReajusteFretePlanilha.CarregarGrid();
                LimparCamposControle();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}




//*******METODO*******
function EditarFinalizacao(arg) {
    if (arg.Data.Situacao == EnumSituacaoControleReajusteFretePlanilha.Aprovado)
        _finalizacao.ConfirmarFinalizacao.visible(true);
    else
        _finalizacao.ConfirmarFinalizacao.visible(false);
}