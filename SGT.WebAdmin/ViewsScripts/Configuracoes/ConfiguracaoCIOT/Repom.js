var _configuracaoRepom = null;

var ConfiguracaoRepom = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCliente = PropertyEntity({ text: "Usuário:", maxlength: 50 });
    this.AssinaturaDigital = PropertyEntity({ text: "Senha:", maxlength: 50 });
    this.CNPJIntegrador = PropertyEntity({ text: "CNPJ Integrador:", maxlength: 14 });
    this.CodigoMovimentoINSS = PropertyEntity({ text: "Código Movimento INSS:", maxlength: 5 });
    this.CodigoMovimentoIR = PropertyEntity({ text: "Código Movimento IR:", maxlength: 5 });
    this.CodigoMovimentoSEST = PropertyEntity({ text: "Código Movimento SEST:", maxlength: 5 });
    this.CodigoMovimentoSENAT = PropertyEntity({ text: "Código Movimento SENAT:", maxlength: 5 });

    this.DataMovimento = PropertyEntity({ text: "Data do Movimento:", getType: typesKnockout.date, val: ko.observable("") });
    this.DownloadMovimentoFinanceiro = PropertyEntity({ eventClick: DownloadMovimentoFinanceiroClick, type: types.event, text: "Financeiro", visible: ko.observable(true) });
    this.DownloadMovimentoContabil = PropertyEntity({ eventClick: DownloadMovimentoContabilClick, type: types.event, text: "Contábil", visible: ko.observable(true) });
};

function LoadConfiguracaoRepom() {
    _configuracaoRepom = new ConfiguracaoRepom();
    KoBindings(_configuracaoRepom, "tabRepom");
}

function LimparCamposConfiguracaoRepom() {
    LimparCampos(_configuracaoRepom);
}

function DownloadMovimentoFinanceiroClick() {
    executarDownload("ConfiguracaoCIOT/DownloadMovimentoFinanceiro", { DataMovimento: _configuracaoRepom.DataMovimento.val() });
}

function DownloadMovimentoContabilClick() {
    executarDownload("ConfiguracaoCIOT/DownloadMovimentoContabil", { DataMovimento: _configuracaoRepom.DataMovimento.val() });
}