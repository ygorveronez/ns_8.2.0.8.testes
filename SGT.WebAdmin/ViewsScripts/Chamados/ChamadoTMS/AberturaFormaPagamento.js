/// <reference path="Abertura.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Banco.js" />
/// <reference path="../../Enumeradores/EnumTipoConta.js" />
/// <reference path="../../Enumeradores/EnumFormaPagamentoChamado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _aberturaFormaPagamento;

var AberturaFormaPagamento = function () {
    this.NomeTerceiro = PropertyEntity({ text: "*Nome Terceiro:", maxlength: 200, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.CnpjCpfTerceiro = PropertyEntity({ text: "*CNPJ/CPF Terceiro:", getType: typesKnockout.cpfCnpj, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.Agencia = PropertyEntity({ text: "*Agencia:", maxlength: 50, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroConta = PropertyEntity({ text: "*Número Conta:", maxlength: 50, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });

    this.MotoristaPagouSemAutorizacao = PropertyEntity({ text: "O Motorista pagou sem autorização?", getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(true) });
    this.GestorLogisticaAutorizouPagamento = PropertyEntity({ text: "O gestor de logística autorizou o pagamento?", getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(true) });

    this.FormaPagamento = PropertyEntity({ val: ko.observable(EnumFormaPagamentoChamado.PamcardMotorista), options: EnumFormaPagamentoChamado.obterOpcoes(), def: EnumFormaPagamentoChamado.PamcardMotorista, text: "*Forma de Pagamento: ", required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoContaBanco = PropertyEntity({ val: ko.observable(EnumTipoConta.Corrente), options: EnumTipoConta.obterOpcoesBordero(), def: EnumTipoConta.Corrente, text: "*Tipo da Conta: ", required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });

    this.ValorAdiantamento = PropertyEntity({ def: "", val: ko.observable(""), text: "*Valor Adiantamento:", getType: typesKnockout.decimal, maxlength: 15, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: false } });

    this.OutroMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Outro Motorista:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Banco:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });

    this.FormaPagamento.val.subscribe(function (valor) {
        _aberturaFormaPagamento.OutroMotorista.required(false);
        _aberturaFormaPagamento.NomeTerceiro.required(false);
        _aberturaFormaPagamento.CnpjCpfTerceiro.required(false);
        _aberturaFormaPagamento.OutroMotorista.visible(false);
        _aberturaFormaPagamento.NomeTerceiro.visible(false);
        _aberturaFormaPagamento.CnpjCpfTerceiro.visible(false);
        _aberturaFormaPagamento.Agencia.required(false);
        _aberturaFormaPagamento.NumeroConta.required(false);
        _aberturaFormaPagamento.Banco.required(false);
        $("#liContaBancariaFormaPagamento").hide();

        if (valor === EnumFormaPagamentoChamado.PamcardTerceiro || valor === EnumFormaPagamentoChamado.Terceiro) {
            _aberturaFormaPagamento.NomeTerceiro.required(true);
            _aberturaFormaPagamento.CnpjCpfTerceiro.required(true);
            _aberturaFormaPagamento.NomeTerceiro.visible(true);
            _aberturaFormaPagamento.CnpjCpfTerceiro.visible(true);
        } else if (valor === EnumFormaPagamentoChamado.ContaBancariaTerceiro) {
            _aberturaFormaPagamento.OutroMotorista.required(true);
            _aberturaFormaPagamento.NomeTerceiro.required(true);
            _aberturaFormaPagamento.CnpjCpfTerceiro.required(true);
            _aberturaFormaPagamento.OutroMotorista.visible(true);
            _aberturaFormaPagamento.NomeTerceiro.visible(true);
            _aberturaFormaPagamento.CnpjCpfTerceiro.visible(true);
            $("#liContaBancariaFormaPagamento").show();
            _aberturaFormaPagamento.Agencia.required(true);
            _aberturaFormaPagamento.NumeroConta.required(true);
            _aberturaFormaPagamento.Banco.required(true);
        }
    });
};

//*******EVENTOS*******

function loadAberturaFormaPagamento() {
    _aberturaFormaPagamento = new AberturaFormaPagamento();
    KoBindings(_aberturaFormaPagamento, "tabFormaPagamento");

    $("#liContaBancariaFormaPagamento").hide();

    new BuscarMotoristas(_aberturaFormaPagamento.OutroMotorista);
    new BuscarBanco(_aberturaFormaPagamento.Banco);
}

function validarCamposObrigatoriosAberturaFormaPagamento() {
    return ValidarCamposObrigatorios(_aberturaFormaPagamento);
}

function ControleCamposAberturaFormaPagamento(status) {
    SetarEnableCamposKnockout(_aberturaFormaPagamento, status);
}

function limparCamposAberturaFormaPagamento() {
    LimparCampos(_aberturaFormaPagamento);
}