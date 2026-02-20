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
/// <reference path="OrdemCompra.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _qualificacao;
var _gridQualificacao;

var Qualificacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Historico = PropertyEntity({ type: types.local });
};

//*******EVENTOS*******

function LoadQualificacao() {
    _qualificacao = new Qualificacao();
    KoBindings(_qualificacao, "knockoutQualificacao");

    GridQualificao();
}

//*******MÉTODOS*******

function GridQualificao() {
    _gridQualificacao = new GridView(_qualificacao.Historico.id, "Pessoa/DocumentosFornecedor", _qualificacao);
}

function ExibirQualificacaoFornecedor() {
    _qualificacao.Codigo.val(_ordemCompra.Fornecedor.codEntity());

    _gridQualificacao.CarregarGrid(function () {        
        Global.abrirModal("divQualificacaoFornecedor");
    });
}