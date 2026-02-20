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
var _resumo;

var Resumo = function () {
    var self = this;
    var fechamentoPorKm = (_CONFIGURACAO_TMS.TipoFechamentoFrete == EnumTipoFechamentoFrete.FechamentoPorKm);

    this.Resumo = PropertyEntity({ type: types.map, visible: ko.observable(false) });

    this.NaoEmitirComplemento = PropertyEntity({ val: ko.observable(!fechamentoPorKm), getType: typesKnockout.bool, text: "Não emitir documentos complementos complementares?", def: !fechamentoPorKm, visible: ko.observable(fechamentoPorKm), enable: ko.observable(true) });

    this.ValorPagar = PropertyEntity({ text: "Valor já Pago:", val: ko.observable("0,00"), def: "0,00" });
    this.TotalDescontos = PropertyEntity({ text: "Ocorrências de Descontos:", val: ko.observable("0,00"), def: "0,00" });
    this.TotalAcrescimos = PropertyEntity({ text: "Ocorrências de Acréscimos:", val: ko.observable("0,00"), def: "0,00" });
    this.TotalDescontosAplicar = PropertyEntity({ text: "Total Descontos:", val: ko.observable("0,00"), def: "0,00" , visible: ko.observable(true) });
    this.TotalAcrescimosAplicar = PropertyEntity({ text: "Total Acréscimos:", val: ko.observable("0,00"), def: "0,00", visible: ko.observable(true) });
    this.ValorFinal = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, visible: false });
   
    this.ValorComplemento = PropertyEntity({ text: "Valor a Complementar", type: types.local, visible: ko.observable(fechamentoPorKm), val: ko.observable("0,00") });

    this.ValorTotal = PropertyEntity({
        text: "Valor Final", type: types.local, val: ko.computed(function () {
            let valor = self.NaoEmitirComplemento.val() ? Globalize.parseFloat(self.ValorFinal.val()) : Globalize.parseFloat(self.ValorComplemento.val()) + Globalize.parseFloat(self.ValorFinal.val());
            return Globalize.format(valor, "n2");
        })
    });
}

//*******EVENTOS*******
function LoadResumo() {
    _resumo = new Resumo();
    KoBindings(_resumo, "knockoutResumo");
}

//*******METODOS*******
function EditarResumo(data) {
    PreencherObjetoKnout(_resumo, { Data: data.Resumo });
    ControleCamposResumo(true);
    AtualizarValoresFechamento();
}

function ControleCamposResumo(status) {
    _resumo.Resumo.visible(status);
}

function AtualizarValoresFechamento() {
    let valorDiferenca = _sumarizadoFechamentoPeriodo.Diferenca.val(); //Valor que falta ser pago sem os acrescimos aplicar e descontos aplicar
    let valorPagar = Globalize.parseFloat(_resumo.ValorPagar.val()); //Valor que já foi pago
    let acrescimo = Globalize.parseFloat(_resumo.TotalAcrescimosAplicar.val());
    let desconto = Globalize.parseFloat(_resumo.TotalDescontosAplicar.val());
    let acrescimoOcorrencias = Globalize.parseFloat(_resumo.TotalAcrescimos.val());
    let descontoOcorrencias = Globalize.parseFloat(_resumo.TotalDescontos.val());
    let valorComplemento = (valorDiferenca - desconto + acrescimo);
    let valorFinal = valorPagar - descontoOcorrencias + acrescimoOcorrencias;
   
    _resumo.ValorComplemento.val(Globalize.format(valorComplemento, "n2"));
    _resumo.ValorFinal.val(Globalize.format(valorFinal, "n2"));
}