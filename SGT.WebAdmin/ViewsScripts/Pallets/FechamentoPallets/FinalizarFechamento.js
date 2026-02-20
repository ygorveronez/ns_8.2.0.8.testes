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

var _finalizarFechamento;
var _gridFinalizarFechamento;

var FinalizarFechamento = function () {
    this.Aberto = PropertyEntity({ val: ko.observable(true) });
    this.Fechado = PropertyEntity({ val: ko.observable(false) });
}


//*******EVENTOS*******
function LoadFinalizarFechamento() {
    _finalizarFechamento = new FinalizarFechamento();
    KoBindings(_finalizarFechamento, "knockoutFinalizarFechamento");
}

function finalizarClick(e, sender) {
    Salvar(_fechamentoPallets, "FechamentoPallets/Finalizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {     
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Finalizado com sucesso");
                LimparCamposFechamentoPallets();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


//*******MÉTODOS*******
function EditarFinalizarFechamento(dados) {
    if (dados.Situacao == EnumSituacaoFechamentoPallets.Aberto) {
        _finalizarFechamento.Aberto.val(true);
        _finalizarFechamento.Fechado.val(false);
    } else if (dados.Situacao == EnumSituacaoFechamentoPallets.Fechado) {
        _finalizarFechamento.Fechado.val(true);
        _finalizarFechamento.Aberto.val(false);
    }
}