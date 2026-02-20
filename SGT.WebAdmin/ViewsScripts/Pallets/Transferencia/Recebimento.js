/// <reference path="CruzamentoInformacoes.js" />
/// <reference path="Transferencia.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTransferenciaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _recebimentoTransferenciaPallet;

/*
 * Declaração das Classes
 */

var RecebimentoTransferenciaPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Data = PropertyEntity({ text: "*Data: ", getType: typesKnockout.date, required: true, enable: ko.observable(true) });
    this.Quantidade = PropertyEntity({ val: ko.observable(""), def: "", text: "*Quantidade: ", getType: typesKnockout.int, required: true, enable: ko.observable(true) });
    this.Recebedor = PropertyEntity({ val: ko.observable(""), def: "", text: "*Recebedor: ", required: true, enable: ko.observable(true) });

    this.Quantidade.val.subscribe(preencherQuantidadeRecebida);
}

/*
 * Declaração das Funções de Inicialização
 */

function loadRecebimentoTransferenciaPallet() {
    _recebimentoTransferenciaPallet = new RecebimentoTransferenciaPallet();
    KoBindings(_recebimentoTransferenciaPallet, "knockoutRecebimentoTransferenciaPallet");
}

/*
 * Declaração das Funções
 */

function adicionarRecebimento() {
    exibirConfirmacao("Confirmação", "Realmente deseja finalizar a transferência de pallets?", function () {
        if (ValidarCamposObrigatorios(_recebimentoTransferenciaPallet)) {
            var transferenciaRecebimento = {
                Codigo: _transferenciaPallet.Codigo.val(),
                Recebimento: JSON.stringify(RetornarObjetoPesquisa(_recebimentoTransferenciaPallet))
            };

            executarReST("Transferencia/AdicionarRecebimento", transferenciaRecebimento, function (retorno) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Transferência de pallets finalizada com sucesso");

                    _gridTransferenciaPallet.CarregarGrid();

                    limparCamposTransferenciaPallet();
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
                }
            }, null);
        }
        else
            exibirMensagemCamposObrigatorio();
    });
}

function controlarCamposRecebimentoHabilitados() {
    var habilitarCampo = (_transferenciaPallet.Situacao.val() === EnumSituacaoTransferenciaPallet.AguardandoRecebimento);

    _recebimentoTransferenciaPallet.Data.enable(habilitarCampo);
    _recebimentoTransferenciaPallet.Quantidade.enable(habilitarCampo);
    _recebimentoTransferenciaPallet.Recebedor.enable(habilitarCampo);
}

function limparCamposRecebimento() {
    LimparCampos(_recebimentoTransferenciaPallet);
}

function preencherRecebimento(dadosRecebimento) {
    if (dadosRecebimento)
        PreencherObjetoKnout(_recebimentoTransferenciaPallet, { Data: dadosRecebimento });
}