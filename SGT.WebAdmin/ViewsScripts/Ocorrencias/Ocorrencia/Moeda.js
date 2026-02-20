var _alteracaoMoedaOcorrencia;

var AlteracaoMoedaOcorrencia = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ValorCotacaoMoeda = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Ocorrencias.Ocorrencia.ValorCotacaoMoeda.getRequiredFieldDescription(), val: ko.observable(""), def: "", required: true, configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.Moeda = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Moeda.getFieldDescription(), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), def: EnumMoedaCotacaoBancoCentral.Real, issue: 0, visible: ko.observable(true) });

    this.Alterar = PropertyEntity({ eventClick: AlterarMoedaOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.AlterarMoeda, icon: "fa fa-dollar", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharTelaAlteracaoMoedaOcorrencia, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Fechar, icon: "fa fa-window-close", visible: ko.observable(true) });
};

////*******EVENTOS*******

function LoadAlteracaoMoedaOcorrencia() {
    _alteracaoMoedaOcorrencia = new AlteracaoMoedaOcorrencia();
    KoBindings(_alteracaoMoedaOcorrencia, "knockoutAlteracaoMoedaOcorrencia");
}

function AlterarMoedaOcorrenciaClick(e, sender) {
    Salvar(_alteracaoMoedaOcorrencia, "Ocorrencia/AlterarMoedaOcorrencia", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.Ocorrencia.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.MoedaAlteradaComSucesso);
                FecharTelaAlteracaoMoedaOcorrencia();
                buscarOcorrenciaPorCodigo();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

////*******METODOS*******

function LimparCamposAlteracaoMoedaOcorrencia() {
    LimparCampos(_alteracaoMoedaOcorrencia);
}

function AbrirTelaAlteracaoMoedaOcorrencia() {
    LimparCamposAlteracaoMoedaOcorrencia();
    _alteracaoMoedaOcorrencia.Codigo.val(_ocorrencia.Codigo.val());
    Global.abrirModal('knockoutAlteracaoMoedaOcorrencia');
}

function FecharTelaAlteracaoMoedaOcorrencia() {
    Global.fecharModal('knockoutAlteracaoMoedaOcorrencia');
    LimparCamposAlteracaoMoedaOcorrencia();
}

function ConverterValorMoedaOcorrencia(ocorrencia) {
    var valorCotacaoMoeda = Globalize.parseFloat(ocorrencia.ValorCotacaoMoeda.val() || "0");
    var valorTotalMoeda = Globalize.parseFloat(ocorrencia.ValorTotalMoeda.val() || "0");

    if (isNaN(valorCotacaoMoeda))
        valorCotacaoMoeda = 0;
    if (isNaN(valorTotalMoeda))
        valorTotalMoeda = 0;

    var valorTotalConvertido = valorCotacaoMoeda * valorTotalMoeda;

    if (valorTotalConvertido > 0)
        ocorrencia.ValorOcorrencia.val(Globalize.format(valorTotalConvertido, "n2"));
    else
        ocorrencia.ValorOcorrencia.val("");
}