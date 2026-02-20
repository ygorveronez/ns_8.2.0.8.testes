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

var _controleReajusteFretePlanilha;
var _pesquisaControleReajusteFretePlanilha;
var _gridControleReajusteFretePlanilha;

var ControleReajusteFretePlanilha = function () {
    var self = this;
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "Número:", val: ko.observable(""), def: "", enable: false, getType: typesKnockout.string });

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid(), enable: ko.observable(true), enable: ko.observable(true), required: true });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Transportador:", idBtnSearch: guid(), enable: ko.observable(true), enable: ko.observable(true), required: true });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Operação:", idBtnSearch: guid(), enable: ko.observable(true), enable: ko.observable(true), required: true });
    this.Planilha = PropertyEntity({ type: types.file, val: ko.observable(""), text: "Planilha:", visible: ko.observable(true), enable: ko.observable(true), file: null, name: ko.observable("") });
    this.Planilha.val.subscribe(function () {
        var nome = self.Planilha.val().replace('C:\\fakepath\\', '');
        self.Planilha.name(nome);
    });

    this.DownloadPlanilha = PropertyEntity({ eventClick: downloadPlanilhaClick, type: types.event, text: "Planilha", visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(""), def: "", getType: typesKnockout.string, enable: ko.observable(true) });
}

//*******EVENTOS*******
function loadPlanilha() {
    //-- Knouckout
    // Instancia objeto principal
    _controleReajusteFretePlanilha = new ControleReajusteFretePlanilha();
    KoBindings(_controleReajusteFretePlanilha, "knockoutControleReajusteFretePlanilha");

    _controleReajusteFretePlanilha.Planilha.file = document.getElementById(_controleReajusteFretePlanilha.Planilha.id);

    HeaderAuditoria("ControleReajusteFretePlanilha", _controleReajusteFretePlanilha);

    // Instancia buscas
    new BuscarTransportadores(_controleReajusteFretePlanilha.Empresa);
    new BuscarFilial(_controleReajusteFretePlanilha.Filial);
    new BuscarTiposOperacao(_controleReajusteFretePlanilha.TipoOperacao);
}

function downloadPlanilhaClick() {
    var dados = {
        Codigo: _controleReajusteFretePlanilha.Codigo.val()
    };
    if (dados.Codigo > 0) {
        executarDownload("ControleReajusteFretePlanilha/DownloadPlanilha", dados);
    }
}

//*******MÉTODOS*******
function PreencheDadosPlanilhaReajuste(arg) {
    PreencherObjetoKnout(_controleReajusteFretePlanilha, arg);

    _controleReajusteFretePlanilha.Planilha.name(arg.Data.NomeArquivo);

    _controleReajusteFretePlanilha.Filial.enable(false);
    _controleReajusteFretePlanilha.TipoOperacao.enable(false);
    _controleReajusteFretePlanilha.Planilha.enable(false);
    _controleReajusteFretePlanilha.Observacao.enable(false);
}

function limparCamposPlanilha() {
    _controleReajusteFretePlanilha.Filial.enable(true);
    _controleReajusteFretePlanilha.TipoOperacao.enable(true);
    _controleReajusteFretePlanilha.Planilha.enable(true);
    _controleReajusteFretePlanilha.Observacao.enable(true);
}