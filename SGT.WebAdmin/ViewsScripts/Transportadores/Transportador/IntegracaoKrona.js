/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="Transportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _integracaoKrona

/*
 * Declaração das Classes
 */

var IntegracaoKrona = function () {
    this.Usuario = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Usuario, maxlength: 100, visible: ko.observable(true), required: false });
    this.Senha = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Senha, maxlength: 100, visible: ko.observable(true), required: false });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadIntegracaoKrona() {
    _integracaoKrona = new IntegracaoKrona();
    KoBindings(_integracaoKrona, "knockoutIntegracaoKrona");
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposIntegracaoKrona() {
    LimparCampos(_integracaoKrona);
}

function preencherIntegracaoKrona(dadosIntegracaoKrona) {
    PreencherObjetoKnout(_integracaoKrona, { Data: dadosIntegracaoKrona });
}

function preencherIntegracaoKronaSalvar(transportador) {
    transportador["IntegracaoKrona"] = JSON.stringify({
        Senha: _integracaoKrona.Senha.val(),
        Usuario: _integracaoKrona.Usuario.val()
    });
}