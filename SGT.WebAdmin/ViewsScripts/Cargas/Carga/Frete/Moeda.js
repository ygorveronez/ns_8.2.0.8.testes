var _alteracaoMoedaCarga;

var AlteracaoMoedaCarga = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ValorCotacaoMoeda = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Cargas.Carga.ValorCotacaoMoeda.getRequiredFieldDescription(), val: ko.observable(""), def: "", required: true, configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.Moeda = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Moeda.getFieldDescription(), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), def: EnumMoedaCotacaoBancoCentral.Real, issue: 0, visible: ko.observable(true) });

    this.Alterar = PropertyEntity({ eventClick: AlterarMoedaCargaClick, type: types.event, text: Localization.Resources.Cargas.Carga.AlterarMoeda, icon: "fal fa-dollar-sign", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharTelaAlteracaoMoedaCarga, type: types.event, text: Localization.Resources.Cargas.Carga.Fechar, icon: "fal fa-window-close", visible: ko.observable(true) });
};

////*******EVENTOS*******

function LoadAlteracaoMoedaCarga() {
    _alteracaoMoedaCarga = new AlteracaoMoedaCarga();
    KoBindings(_alteracaoMoedaCarga, "knockoutAlteracaoMoedaCarga");

    LocalizeCurrentPage();
}

function AlterarMoedaCargaClick(e, sender) {
    Salvar(_alteracaoMoedaCarga, "CargaFrete/AlterarMoedaCarga", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.MoedaAlteradoComSucesso);
                FecharTelaAlteracaoMoedaCarga();
                verificarFrete(_cargaAtual);

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

////*******METODOS*******

function LimparCamposAlteracaoMoedaCarga() {
    LimparCampos(_alteracaoMoedaCarga);
}

function AbrirTelaAlteracaoMoedaCarga() {
    LimparCamposAlteracaoMoedaCarga();
    _alteracaoMoedaCarga.Codigo.val(_cargaAtual.Codigo.val());
    Global.abrirModal("knockoutAlteracaoMoedaCarga");
}

function FecharTelaAlteracaoMoedaCarga() {
    Global.fecharModal("knockoutAlteracaoMoedaCarga");
    LimparCamposAlteracaoMoedaCarga();
}