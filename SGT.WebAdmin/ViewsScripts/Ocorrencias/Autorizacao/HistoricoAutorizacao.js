/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Autorizacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _historicoAutorizacao;
var _gridHistoricoAutorizacao;

var HistoricoAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Entidade = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.string });

    this.HistoricoAutorizacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
};

//*******EVENTOS*******

function loadHistoricoAutorizacao() {
    _historicoAutorizacao = new HistoricoAutorizacao();
    KoBindings(_historicoAutorizacao, "knockoutHistoricoAutorizacao");

    _gridHistoricoAutorizacao = new GridView(_historicoAutorizacao.HistoricoAutorizacao.idGrid, "Auditoria/ConsultarAuditoria", _historicoAutorizacao);
}

//*******MÉTODOS*******

function CarregarHistoricoAutorizacao(ocorrencia) {
    _historicoAutorizacao.Codigo.val(ocorrencia);
    _historicoAutorizacao.Entidade.val("CargaOcorrencia");
    _gridHistoricoAutorizacao.CarregarGrid();
}