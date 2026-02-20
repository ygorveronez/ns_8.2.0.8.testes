/// <reference path="AnexoTransportador.js" />
/// <reference path="TermoQuitacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTermoQuitacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _dadosAceiteTransportador;

/*
 * Declaração das Classes
 */

var DadosAceiteTransportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Observacao = PropertyEntity({ text: "Observação: ", maxlength: 1000, enable: ko.observable(true) });
    this.ObservacaoAnterior = PropertyEntity({ maxlength: 1000, visible: false });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadDadosAceiteTransportador() {
    _dadosAceiteTransportador = new DadosAceiteTransportador();
    KoBindings(_dadosAceiteTransportador, "knockoutDadosAceiteTransportador");
}

/*
 * Declaração das Funções Públicas
 */

function aprovarTermoQuitacao() {
    if (!isAnexosTransportadorInformados()) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Para realizar a aprovação do termo de quitação é necessário informar um ou mais anexos.", 6000);
        return;
    }

    exibirConfirmacao("Confirmação", "Deseja realmente aprovar o termo de quitação?", function () {
        var dadosAceiteTransportador = RetornarObjetoPesquisa(_dadosAceiteTransportador);

        executarReST("TermoQuitacao/Aprovar", dadosAceiteTransportador, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Termo de quitação aprovado com sucesso");

                    _gridTermoQuitacao.CarregarGrid();

                    buscarTermoQuitacaoPorCodigo(_dadosAceiteTransportador.Codigo.val());
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function controlarCamposDadosAceiteTransportadorHabilitados() {
    var habilitarCampo = (
        (
            (_termoQuitacao.Situacao.val() === EnumSituacaoTermoQuitacao.AguardandoAceiteTransportador) ||
            (_termoQuitacao.Situacao.val() === EnumSituacaoTermoQuitacao.AprovacaoRejeitada)
        ) &&
        isAcessoTransportador()
    );
    
    _dadosAceiteTransportador.Observacao.enable(habilitarCampo);
}

function limparCamposDadosAceiteTransportador() {
    LimparCampos(_dadosAceiteTransportador);
}

function preencherDadosAceiteTransportador(dadosAceiteTransportador) {
    PreencherObjetoKnout(_dadosAceiteTransportador, { Data: dadosAceiteTransportador });
}

function reprovarTermoQuitacao() {
    var mensagemConfirmacao = "Deseja realmente reprovar o termo de quitação?";

    if (!_dadosAceiteTransportador.Observacao.val())
        mensagemConfirmacao = "Nenhuma observação foi informada, deseja reprovar o termo de quitação mesmo assim?"
    else if (_dadosAceiteTransportador.Observacao.val() == _dadosAceiteTransportador.ObservacaoAnterior.val())
        mensagemConfirmacao = "A observação não foi alterada, deseja reprovar o termo de quitação mesmo assim?"

    exibirConfirmacao("Confirmação", mensagemConfirmacao, function () {
        var dadosAceiteTransportador = RetornarObjetoPesquisa(_dadosAceiteTransportador);

        executarReST("TermoQuitacao/Reprovar", dadosAceiteTransportador, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Termo de quitação reprovado com sucesso");

                    _gridTermoQuitacao.CarregarGrid();

                    buscarTermoQuitacaoPorCodigo(_dadosAceiteTransportador.Codigo.val());
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}
