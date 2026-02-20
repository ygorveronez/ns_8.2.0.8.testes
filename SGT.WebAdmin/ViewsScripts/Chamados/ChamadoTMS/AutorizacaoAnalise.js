/// <reference path="../../Enumeradores/EnumFormaAutorizacaoPagamentoChamado.js" />
/// <reference path="Analise.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _autorizacaoAnalise;

var AutorizacaoAnalise = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.FormaAutorizacaoPagamento = PropertyEntity({ val: ko.observable(EnumFormaAutorizacaoPagamentoChamado.AutorizarValorRecibo), options: EnumFormaAutorizacaoPagamentoChamado.obterOpcoes(), def: EnumFormaAutorizacaoPagamentoChamado.AutorizarValorRecibo, text: "*Forma Autorização: ", required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.NovoValorAutorizado = PropertyEntity({ text: "*Novo Valor Autorizado:", def: "", val: ko.observable(""), getType: typesKnockout.decimal, maxlength: 15, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: false } });
    this.JustificativaAutorizacao = PropertyEntity({ val: ko.observable(""), def: "", text: "*Justificativa:", required: ko.observable(true), enable: ko.observable(true) });

    this.SalvarAutorizacao = PropertyEntity({ eventClick: SalvarAutorizacaoClick, type: types.event, text: "Salvar Autorização", visible: ko.observable(true), enable: ko.observable(true) });

    this.FormaAutorizacaoPagamento.val.subscribe(function (valor) {
        if (valor === EnumFormaAutorizacaoPagamentoChamado.AutorizarValorDiferente) {
            _autorizacaoAnalise.NovoValorAutorizado.required(true);
            _autorizacaoAnalise.NovoValorAutorizado.visible(true);
        } else {
            _autorizacaoAnalise.NovoValorAutorizado.required(false);
            _autorizacaoAnalise.NovoValorAutorizado.visible(false);
        }
    });
};

//*******EVENTOS*******

function loadAutorizacaoAnalise() {
    _autorizacaoAnalise = new AutorizacaoAnalise();
    KoBindings(_autorizacaoAnalise, "tabAutorizacaoAnalise");
}

function SalvarAutorizacaoClick(e, sender) {
    Salvar(_autorizacaoAnalise, "ChamadoTMSAnalise/SalvaAutorizacaoAnalise", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Autorização da análise salvo com sucesso");
                BuscarChamadoTMSPorCodigo(arg.Data);
                recarregarGridPesquisaChamadosTMS();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function ControleCamposAutorizacaoAnalise(status) {
    SetarEnableCamposKnockout(_autorizacaoAnalise, status);

    _autorizacaoAnalise.SalvarAutorizacao.visible(status);
}

function limparCamposAutorizacaoAnalise() {
    LimparCampos(_autorizacaoAnalise);
}