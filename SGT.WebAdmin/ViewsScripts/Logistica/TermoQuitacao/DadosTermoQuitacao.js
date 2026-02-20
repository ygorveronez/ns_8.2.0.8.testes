/// <reference path="Anexo.js" />
/// <reference path="TermoQuitacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTermoQuitacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _dadosTermoQuitacao;

/*
 * Declaração das Classes
 */

var DadosTermoQuitacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataBase = PropertyEntity({ text: "*Data Base:", getType: typesKnockout.date, required: true, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), enable: ko.observable(true) });
    this.Numero = PropertyEntity({ val: ko.observable(""), def: "", text: "Número: ", enable: false });
    this.Observacao = PropertyEntity({ val: ko.observable(""), def: "", text: "Observação: ", maxlength: 1000, enable: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Transportador:", idBtnSearch: guid(), enable: ko.observable(true)});
}

/*
 * Declaração das Funções de Inicialização
 */

function loadDadosTermoQuitacao() {
    _dadosTermoQuitacao = new DadosTermoQuitacao();
    KoBindings(_dadosTermoQuitacao, "knockoutDadosTermoQuitacao");

    new BuscarTransportadores(_dadosTermoQuitacao.Transportador);
}

/*
 * Declaração das Funções Públicas
 */

function adicionarDadosTermoQuitacao() {
    if (!validarCamposObrigatóriosDadosTermoQuitacao())
        return;

    var dadosTermoQuitacao = RetornarObjetoPesquisa(_dadosTermoQuitacao);

    executarReST("TermoQuitacao/Adicionar", dadosTermoQuitacao, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Termo de quitação adicionado com sucesso");

                _gridTermoQuitacao.CarregarGrid();

                enviarArquivosAnexados(retorno.Data.Codigo, function () { buscarTermoQuitacaoPorCodigo(retorno.Data.Codigo); });
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function atualizarDadosTermoQuitacao() {
    if (!validarCamposObrigatóriosDadosTermoQuitacao())
        return;

    var dadosTermoQuitacao = RetornarObjetoPesquisa(_dadosTermoQuitacao);

    executarReST("TermoQuitacao/Atualizar", dadosTermoQuitacao, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Termo de quitação atualizado com sucesso");

                _gridTermoQuitacao.CarregarGrid();

                buscarTermoQuitacaoPorCodigo(_dadosTermoQuitacao.Codigo.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function controlarCamposDadosTermoQuitacaoHabilitados() {
    var acessoTransportador = isAcessoTransportador();
    var novoRegistro = (_termoQuitacao.Situacao.val() === EnumSituacaoTermoQuitacao.Todas);
    var aceiteTransportadorRejeitado = (_termoQuitacao.Situacao.val() === EnumSituacaoTermoQuitacao.AceiteTransportadorRejeitado);

    _dadosTermoQuitacao.DataBase.enable(!acessoTransportador && (novoRegistro || aceiteTransportadorRejeitado));
    _dadosTermoQuitacao.Observacao.enable(!acessoTransportador && (novoRegistro || aceiteTransportadorRejeitado));
    _dadosTermoQuitacao.Transportador.enable(!acessoTransportador && novoRegistro);
}

function limparCamposDadosTermoQuitacao() {
    LimparCampos(_dadosTermoQuitacao);
}

function preencherDadosTermoQuitacao(dadosTermoQuitacao) {
    PreencherObjetoKnout(_dadosTermoQuitacao, { Data: dadosTermoQuitacao });
}

function validarCamposObrigatóriosDadosTermoQuitacao() {
    if (!ValidarCamposObrigatorios(_dadosTermoQuitacao)) {
        exibirMensagemCamposObrigatorio();
        return false;
    }

    if (!isAnexosInformados()) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Para adicionar ou atualizar do termo de quitação é necessário informar um ou mais anexos.", 6000);
        return false;
    }

    return true;
}