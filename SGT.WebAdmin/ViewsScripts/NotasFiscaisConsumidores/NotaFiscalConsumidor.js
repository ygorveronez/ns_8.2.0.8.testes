/// <reference path="../../Enumeradores/EnumCSTPISCOFINS.js" />
/// <reference path="../../Enumeradores/EnumCSTIPI.js" />
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
/// <reference path="../../Enumeradores/EnumTipoNotaFiscalConsumidor.js" />
/// <reference path="../../Enumeradores/EnumCSTICMS.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridNotaFiscalConsumidor;
var _notaFiscalConsumidor;
var _pesquisaNotaFiscalConsumidor;

var _tipoItem = [{ value: 1, text: "Produto" },
                    { value: 2, text: "Serviço" }];

var PesquisaNotaFiscalConsumidor = function () {
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int, maxlength: 4 });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });    
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridNotaFiscalConsumidor.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var NotaFiscalConsumidor = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CPFConsumidor = PropertyEntity({ text: "CPF/CNPJ: ", required: false, maxlength: 14, getType: typesKnockout.cpfCnpj });
    this.NomeConsumidor = PropertyEntity({ text: "Nome: ", required: false, maxlength: 120 });
   
    this.Itens = PropertyEntity({ type: types.map, required: false, text: "Itens NFC-e", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });
    this.TipoItem = PropertyEntity({ val: ko.observable(1), def: 1, options: _tipoItem, text: "*Tipo Item:", required: true, enable: ko.observable(true), eventChange: TipoItemChange  });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Serviço:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.QuantidadeItem = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "*Quantidade:", getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorUnitarioItem = PropertyEntity({ def: "0,0000", val: ko.observable("0,0000"), text: "*Valor Unitário:", getType: typesKnockout.decimal, maxlength: 22, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 4, allowZero: true } });
    this.ValorTotalItem = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "*Valor Total:", getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.AdicionarItem = PropertyEntity({ eventClick: AdicionarItemClick, type: types.event, text: "Adicionar Item", visible: ko.observable(true), enable: ko.observable(true) });

    this.ValorTotal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "*Valor Total:", getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorPago = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "*Valor Pago:", getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorTroco = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "*Valor Troco:", getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });

    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar Venda", visible: ko.observable(true), enable: ko.observable(true) });
    this.Finalizar = PropertyEntity({ eventClick: FinalizarClick, type: types.event, text: "Finalizar Venda", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******


function loadNotaFiscalConsumidor() {

    _pesquisaNotaFiscalConsumidor = new PesquisaNotaFiscalConsumidor();
    KoBindings(_pesquisaNotaFiscalConsumidor, "knockoutPesquisaNotaFiscalConsumidor", false, _pesquisaNotaFiscalConsumidor.Pesquisar.id);

    _notaFiscalConsumidor = new NotaFiscalConsumidor();
    KoBindings(_notaFiscalConsumidor, "knockoutCadastroNotaFiscalConsumidor");

    buscarNotaFiscalConsumidors();
}

function AdicionarItemClick(e, sender) {

}

function TipoItemChange(e, sender) {

}

function CalcularTotalItem() {

}

function CancelarClick(e, sender) {

}

function FinalizarClick(e, sender) {

}


//*******MÉTODOS*******


function buscarNotaFiscalConsumidors() {
    var cancelarNFe = { descricao: "Cancelar NFC-e", id: guid(), metodo: cancelarNotaFiscalConsumidor, icone: "" };
    var baixarDANFE = { descricao: "Baixar DANFE", id: guid(), metodo: baixarDANFENotaFiscalConsumidor, icone: "" };
    var baixarXML = { descricao: "Baixar XML NFC-e", id: guid(), metodo: baixarXMLNotaFiscalConsumidor, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [cancelarNFe, baixarDANFE, baixarXML] };

    _gridNotaFiscalConsumidor = new GridView(_pesquisaNotaFiscalConsumidor.Pesquisar.idGrid, "NotaFiscalConsumidorNotaFiscal/Pesquisa", _pesquisaNotaFiscalConsumidor, menuOpcoes, null);
    _gridNotaFiscalConsumidor.CarregarGrid();
}

function cancelarNotaFiscalConsumidor(data) {

}

function baixarDANFENotaFiscalConsumidor(data) {

}

function baixarXMLNotaFiscalConsumidor(data) {

}

function limparCamposNotaFiscalConsumidor() {
    LimparCampos(_notaFiscalConsumidor);
}
