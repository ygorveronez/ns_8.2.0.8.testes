var _alteracaoMoedaCargaCTeAgrupado;

var AlteracaoMoedaCargaCTeAgrupado = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ValorCotacaoMoeda = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor Cotação Moeda:", val: ko.observable(""), def: "", required: true, configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.Moeda = PropertyEntity({ text: "Moeda:", options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), def: EnumMoedaCotacaoBancoCentral.Real, issue: 0, visible: ko.observable(true) });

    this.Alterar = PropertyEntity({ eventClick: AlterarMoedaCargaCTeAgrupadoClick, type: types.event, text: "Alterar a Moeda", icon: "fal fa-dollar-sign", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharTelaAlteracaoMoedaCargaCTeAgrupado, type: types.event, text: "Fechar", icon: "fal fa-window-close", visible: ko.observable(true) });
};

////*******EVENTOS*******

function LoadAlteracaoMoedaCargaCTeAgrupado() {
    _alteracaoMoedaCargaCTeAgrupado = new AlteracaoMoedaCargaCTeAgrupado();
    KoBindings(_alteracaoMoedaCargaCTeAgrupado, "knockoutAlteracaoMoedaCargaCTeAgrupado");
}

function AlterarMoedaCargaCTeAgrupadoClick(e, sender) {
    Salvar(_alteracaoMoedaCargaCTeAgrupado, "CargaCTeAgrupado/AlterarMoedaCTeAgrupado", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Moeda alterada com sucesso!");
                FecharTelaAlteracaoMoedaCargaCTeAgrupado();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

////*******METODOS*******

function LimparCamposAlteracaoMoedaCargaCTeAgrupado() {
    LimparCampos(_alteracaoMoedaCargaCTeAgrupado);
}

function AbrirTelaAlteracaoMoedaCargaCTeAgrupado() {
    LimparCamposAlteracaoMoedaCargaCTeAgrupado();
    _alteracaoMoedaCargaCTeAgrupado.Codigo.val(_cargaCTeAgrupado.Codigo.val());
    Global.abrirModal("knockoutAlteracaoMoedaCargaCTeAgrupado");
}

function FecharTelaAlteracaoMoedaCargaCTeAgrupado() {
    Global.fecharModal('knockoutAlteracaoMoedaCargaCTeAgrupado');
    LimparCamposAlteracaoMoedaCargaCTeAgrupado();
}