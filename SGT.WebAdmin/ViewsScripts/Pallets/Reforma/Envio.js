/// <reference path="EnvioQuantidade.js" />
/// <reference path="Reforma.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoReformaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _envioReformaPallet;

/*
 * Declaração das Classes
 */

var EnvioReformaPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Data = PropertyEntity({ getType: typesKnockout.date, required: true, text: "*Data: ", enable: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: !_isTMS, text: "*Filial:", idBtnSearch: guid(), enable: ko.observable(true), visible: !_isTMS });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Fornecedor:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Numero = PropertyEntity({ val: ko.observable(""), def: "", text: "Número: ", enable: false });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: _isTMS, text: "*Empresa/Filial:", idBtnSearch: guid(), enable: ko.observable(true), visible: _isTMS });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadEnvioReformaPallet() {
    _envioReformaPallet = new EnvioReformaPallet();
    KoBindings(_envioReformaPallet, "knockoutEnvioReformaPallet");

    new BuscarFilial(_envioReformaPallet.Filial);
    new BuscarClientes(_envioReformaPallet.Fornecedor);
    new BuscarTransportadores(_envioReformaPallet.Transportador);
}

/*
 * Declaração das Funções
 */

function adicionarEnvio() {
    exibirConfirmacao("Confirmação", "Realmente deseja adicionar o envio para reforma de pallets?", function () {
        if (ValidarCamposObrigatorios(_envioReformaPallet)) {
            if (validarSituacoesInformadas()) {
                var envioReforma = RetornarObjetoPesquisa(_envioReformaPallet);

                envioReforma["QuantidadesEnviadas"] = obterSituacoes();

                executarReST("Reforma/AdicionarEnvio", envioReforma, function (retorno) {
                    if (retorno.Success) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Envio para reforma adicionado com sucesso");

                        novaReforma();
                    }
                    else
                        exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
                }, null);
            }
        }
        else
            exibirMensagemCamposObrigatorio();
    });
}

function controlarCamposEnvioHabilitados() {
    var habilitarCampo = (_reformaPallet.Situacao.val() === EnumSituacaoReformaPallet.Todas);

    _envioReformaPallet.Data.enable(habilitarCampo);
    _envioReformaPallet.Filial.enable(habilitarCampo);
    _envioReformaPallet.Fornecedor.enable(habilitarCampo);
    _envioReformaPallet.Transportador.enable(habilitarCampo);
}

function limparCamposEnvio() {
    LimparCampos(_envioReformaPallet);
}

function preencherEnvio(dadosEnvio) {
    PreencherObjetoKnout(_envioReformaPallet, { Data: dadosEnvio });
}