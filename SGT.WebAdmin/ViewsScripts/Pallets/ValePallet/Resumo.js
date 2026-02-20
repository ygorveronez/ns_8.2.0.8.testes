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
var _resumoValePallet;

var ResumoValePallet = function () {
    this.Resumo = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.Numero = PropertyEntity({ text: "Número:" });
    this.Data = PropertyEntity({ text: "Data:" });
    this.Filial = PropertyEntity({ text: "Filial:" });
    this.Situacao = PropertyEntity({ text: "Situação:" });
    this.Cliente = PropertyEntity({ text: "Cliente:" });
    this.Quantidade = PropertyEntity({ text: "Quantidade:" });
    this.Pedido = PropertyEntity({ text: "Pedido:" });
}



//*******EVENTOS*******
function LoadResumoValePallet() {
    _resumoValePallet = new ResumoValePallet();
    KoBindings(_resumoValePallet, "knockoutResumoValePallet");
}


//*******MÉTODOS*******
function PreencherResumoValePallet(dados) {
    PreencherObjetoKnout(_resumoValePallet, { Data: dados.Resumo });
    _resumoValePallet.Resumo.val(true);
}

function LimparResumoValePallet() {
    LimparCampos(_resumoValePallet);
}