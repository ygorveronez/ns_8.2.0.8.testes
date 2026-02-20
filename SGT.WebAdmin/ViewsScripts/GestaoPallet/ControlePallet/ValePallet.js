// #region Obejos Globais do Arquivo
var _valePallet;
//#endregion Obejos Globais do Arquivo

// #region Classes
var ValePallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoMovimentacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Situacao = PropertyEntity({ text: "Situação", val: ko.observable(""), def: "" });

    this.NumeroNotaFiscalOrigem = PropertyEntity({ text: "Número Nota Fiscal Origem", val: ko.observable(""), def: "" });
    this.SerieNotaFiscalOrigem = PropertyEntity({ text: "Série Nota Fiscal Origem", val: ko.observable(""), def: "" });
    this.Cliente = PropertyEntity({ text: "Cliente", val: ko.observable(""), def: "" });

    this.NumeroValePallet = PropertyEntity({ text: "*Número Vale Pallet:", getType: typesKnockout.int, val: ko.observable(""), def: "", visible: ko.observable(true), required: ko.observable(true) });
    this.DataVencimento = PropertyEntity({ text: "*Data Vencimento:", getType: typesKnockout.dateTime, val: ko.observable(""), required: ko.observable(true), enable: ko.observable(true) });

    this.Confirmar = PropertyEntity({ eventClick: ConfirmarClick, type: types.event, text: "Confirmar", visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização
function loadValePallet() {
    _valePallet = new ValePallet();
    KoBindings(_valePallet, "knoutRecebimentoValePallet");
}
// #endregion Funções de Inicialização

// #region Ações
function exibirModalAdicionarPallet(movimentacao) {
    _valePallet.CodigoMovimentacao.val(movimentacao.Codigo);

    executarReST("ControlePallet/BuscarValePallet", { CodigoMovimentacao: _valePallet.CodigoMovimentacao.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_valePallet, { Data: retorno.Data })

                Global.abrirModal('divModalRecebimentoValePallet');
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function fecharModalRecebimentoValePallet() {
    LimparCampos(_valePallet);
    Global.fecharModal('divModalRecebimentoValePallet');
}

function ConfirmarClick() {
    if (!ValidarCamposObrigatorios(_valePallet)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja confirmar o vale pallet?", function () {
        const data = RetornarObjetoPesquisa(_valePallet);
        executarReST("ControlePallet/ConfirmarValePallet", data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    fecharModalRecebimentoValePallet();
                    _gridControlePallet.CarregarGrid();
                } else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    });
}

// #endregion Ações