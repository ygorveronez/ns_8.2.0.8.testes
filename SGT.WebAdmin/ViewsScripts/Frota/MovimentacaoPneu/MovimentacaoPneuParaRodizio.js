/// <reference path= "../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path= "../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path= "../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="MovimentacaoPneu.js" />
/// <reference path="../../Enumeradores/EnumTipoContainerPneu.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _MovimentacaoPneuParaRodizio;

/*
 * Declaração das Classes
 */

var MovimentacaoPneuParaRodizio = function () {
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Hodometro = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.UtilizarDadosAdicionais = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });


    this.CodigoEixoPneuOrigem_1 = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoEstepeOrigem_1 = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoPneu_1 = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroFogo_1 = PropertyEntity({ text: "*Número de Fogo:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500, required: true, enable: false });
    this.DataHora = PropertyEntity({ text: "*Data/Hora: ", getType: typesKnockout.dateTime, required: true });
    this.SulcoAnterior_1 = PropertyEntity({ text: "*Sulco Anterior:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: true, enable: false });
    this.SulcoAtual_1 = PropertyEntity({ text: "*Sulco Atual:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true }, maxlength: 10, required: true });
    this.Calibragem_1 = PropertyEntity({ text: "*Calibragem:", getType: typesKnockout.int, maxlength: 10, configInt: { precision: 0, allowZero: true, thousands: "" }, required: true });
    this.MilimetragemUm_1 = PropertyEntity({ text: "Milimetragem 1:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: false });
    this.MilimetragemDois_1 = PropertyEntity({ text: "Milimetragem 2:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: false });
    this.MilimetragemTres_1 = PropertyEntity({ text: "Milimetragem 3:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: false });
    this.MilimetragemQuatro_1 = PropertyEntity({ text: "Milimetragem 4:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: false });

    this.CodigoEixoPneuOrigem_2 = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoEstepeOrigem_2 = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoPneu_2 = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroFogo_2 = PropertyEntity({ text: "*Número de Fogo:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500, required: true, enable: false });
    this.SulcoAnterior_2 = PropertyEntity({ text: "*Sulco Anterior:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: true, enable: false });
    this.SulcoAtual_2 = PropertyEntity({ text: "*Sulco Atual:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true }, maxlength: 10, required: true });
    this.Calibragem_2 = PropertyEntity({ text: "*Calibragem:", getType: typesKnockout.int, maxlength: 10, configInt: { precision: 0, allowZero: true, thousands: "" }, required: true });
    this.MilimetragemUm_2 = PropertyEntity({ text: "Milimetragem 1:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: false });
    this.MilimetragemDois_2 = PropertyEntity({ text: "Milimetragem 2:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: false });
    this.MilimetragemTres_2 = PropertyEntity({ text: "Milimetragem 3:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: false });
    this.MilimetragemQuatro_2 = PropertyEntity({ text: "Milimetragem 4:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: false });


    this.MotivoRodizio = PropertyEntity({ text: "*Motivo rodízio:", getType: typesKnockout.string, maxlength: 50, required: true, enable: true });
    this.ConfirmarRodizio = PropertyEntity({ eventClick: adicionarMovimentacaoPneuParaRodizioClick, type: types.event, text: "Confirmar Rodízio", visible: true });

}

MovimentacaoPneuParaRodizio.prototype = Object.create(MovimentacaoPneuDadosAdicionais.prototype);
MovimentacaoPneuParaRodizio.prototype.constructor = MovimentacaoPneuParaRodizio;

/*
 * Declaração das Funções de Inicialização
 */

function loadMovimentacaoPneuParaRodizio() {
    _MovimentacaoPneuParaRodizio = new MovimentacaoPneuParaRodizio();
    KoBindings(_MovimentacaoPneuParaRodizio, "knockoutMovimentacaoPneuParaRodizio");

}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarMovimentacaoPneuParaRodizioClick() {
    if (ValidarCamposObrigatorios(_MovimentacaoPneuParaRodizio)) {
            _MovimentacaoPneuParaRodizio.CodigoVeiculo.val(_veiculo.Codigo.val());
            _MovimentacaoPneuParaRodizio.Hodometro.val(_dadosVeiculo.Hodometro.val());
        executarReST("MovimentacaoPneu/MovimentarPneusRodizio", RetornarObjetoPesquisa(_MovimentacaoPneuParaRodizio), function (retorno) {
                if (retorno.Success) {
                    fecharModalMovimentacaoPneuParaRodizio();
                    buscarVeiculo();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
    }
    else
        exibirMensagemCamposObrigatorio();
}

/*
 * Declaração das Funções Públicas
 */

function exibirModalMovimentacaoPneuParaRodizio() {
    preencherMovimentacaoPneuParaRodizio();

    Global.abrirModal('divModalMovimentacaoPneuParaRodizio');
    $("#divModalMovimentacaoPneuParaRodizio").one('hidden.bs.modal', function () {
        LimparCampos(_MovimentacaoPneuParaRodizio);
        limparCamposMovimentacaoPneu();
    });
}

/*
 * Declaração das Funções Privadas
 */

/*
function atualizardadosPneuEstoque() {
    _pneuAdicionar.Sulco.val(_MovimentacaoPneuParaRodizio.SulcoAtual.val());
}
*/
function fecharModalMovimentacaoPneuParaRodizio() {
    Global.fecharModal('divModalMovimentacaoPneuParaRodizio');
}

function preencherMovimentacaoPneuParaRodizio() {
    _MovimentacaoPneuParaRodizio.DataHora.val(Global.DataHoraAtual());

    _MovimentacaoPneuParaRodizio.NumeroFogo_1.val(_pneuAdicionar.NumeroFogo.val());
    _MovimentacaoPneuParaRodizio.SulcoAnterior_1.val(_pneuAdicionar.Sulco.val());
    _MovimentacaoPneuParaRodizio.CodigoPneu_1.val(_pneuAdicionar.CodigoPneu.val());

    if (_tipoContainerPneuArrastado == EnumTipoContainerPneu.Estepe)
        _MovimentacaoPneuParaRodizio.CodigoEstepeOrigem_1.val(_pneuAdicionar.Codigo.val());
    else if (_tipoContainerPneuArrastado == EnumTipoContainerPneu.Veiculo)
        _MovimentacaoPneuParaRodizio.CodigoEixoPneuOrigem_1.val(_pneuAdicionar.Codigo.val());

    _MovimentacaoPneuParaRodizio.NumeroFogo_2.val(_pneuRemover.NumeroFogo.val());
    _MovimentacaoPneuParaRodizio.SulcoAnterior_2.val(_pneuRemover.Sulco.val());
    _MovimentacaoPneuParaRodizio.CodigoPneu_2.val(_pneuRemover.CodigoPneu.val());

    if (_tipoContainerPneuDestino == EnumTipoContainerPneu.Estepe)
        _MovimentacaoPneuParaRodizio.CodigoEstepeOrigem_2.val(_pneuRemover.Codigo.val());
    else if (_tipoContainerPneuDestino == EnumTipoContainerPneu.Veiculo)
        _MovimentacaoPneuParaRodizio.CodigoEixoPneuOrigem_2.val(_pneuRemover.Codigo.val());
   
}

