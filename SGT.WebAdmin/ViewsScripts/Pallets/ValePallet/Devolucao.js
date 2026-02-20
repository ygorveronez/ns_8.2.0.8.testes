/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _devolucao;

var Devolucao = function () {
    this.Situacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.Pendente = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.Cancelado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.Devolvido = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
}



//*******EVENTOS*******
function LoadDevolucao() {
    _devolucao = new Devolucao();
    KoBindings(_devolucao, "knockoutDevolucao");

    _devolucao.Situacao.val.subscribe(function (val) {
        SituacaoValePalletModificada(val);
    });
}


//*******MÉTODOS*******
function DadosDevolucao(dados) {
    _devolucao.Situacao.val(dados.Situacao);
}

function SituacaoValePalletModificada(situacao) {
    _devolucao.Pendente.val(false);
    _devolucao.Cancelado.val(false);
    _devolucao.Devolvido.val(false);

    if (situacao == EnumSituacaoValePallet.AgDevolucao)
        _devolucao.Pendente.val(true);
    else if (situacao == EnumSituacaoValePallet.Cancelado)
        _devolucao.Cancelado.val(true);
    else
        _devolucao.Devolvido.val(true);
}
