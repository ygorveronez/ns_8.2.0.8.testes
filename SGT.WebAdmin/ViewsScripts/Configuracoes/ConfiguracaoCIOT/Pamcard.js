var _configuracaoPamcard = null;

var ConfiguracaoPamcard = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Matriz = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Matriz:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.AjustarSaldoVencimentoDataEncerramento = PropertyEntity({ text: "Ajustar a data de vencimento do saldo utilizando a data de encerramento (ao encerrar o CIOT atualizará as parcelas)", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.EnviarQuantidadesMaioresQueZero = PropertyEntity({ text: "Enviar quantidades (peso, volumes e valor de mercadoria) sempre maiores que zero", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.AssociarCartaoMotoristaTransportador = PropertyEntity({ text: "Associar cartão ao Motorista/Transportador", val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.UtilizarDataAtualParaDefinirVencimentoSaldo = PropertyEntity({ text: "Utilizar a data atual para definir o vencimento do saldo", val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.UtilizarDataAtualParaDefinirVencimentoAdiantamento = PropertyEntity({ text: "Utilizar a data atual para definir o vencimento do adiantamento", val: ko.observable(false), getType: typesKnockout.bool, def: false });
};

function LoadConfiguracaoPamcard() {
    _configuracaoPamcard = new ConfiguracaoPamcard();
    KoBindings(_configuracaoPamcard, "tabPamcard");

    new BuscarTransportadores(_configuracaoPamcard.Matriz, null, null, true);
}

function LimparCamposConfiguracaoPamcard() {
    LimparCampos(_configuracaoPamcard);
}