/// <reference path="../../Consultas/Estado.js" />

var _angelLiraExcecao;

var AngelLiraExcecao = function () {
    this.ValorMinimo = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.ValorMinimo.getFieldDescription() });
    this.Destino = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.Destino.getFieldDescription(), idBtnSearch: guid() });
    this.ProcedimentoEmbarque = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Pedidos.TipoOperacao.ProcedimentoEmbarque.getRequiredFieldDescription(), val: ko.observable(0), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarExcecaoAngelLiraClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};


function loadAngelLiraExcecao() {
    _angelLiraExcecao = new AngelLiraExcecao();
    KoBindings(_angelLiraExcecao, "knockoutTipoOperacaoAngelLiraExcecao");

    new BuscarEstados(_angelLiraExcecao.Destino);
}

function abrirModalAngelLiraExcecao() {
    $("#divModalCadastroExcecaoAngelLira")
        .modal("show")
        .one("hidden.bs.modal", function () {
            LimparCampos(_angelLiraExcecao);
        });
}

function adicionarExcecaoAngelLiraClick() {
    if (!ValidarCamposObrigatorios(_angelLiraExcecao)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pedidos.TipoOperacao.PreenchaOsCamposObrigatorios);
        return;
    }

    if (!validarExcecaoAngelLira()) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pedidos.TipoOperacao.PreenchaOsCamposObrigatorios);
        return;
    }

    var novoRegistro = {
        Codigo: guid(),
        CodigoDestino: _angelLiraExcecao.Destino.codEntity(),
        Destino: _angelLiraExcecao.Destino.val(),
        ValorMinimo: _angelLiraExcecao.ValorMinimo.val(),
        ProcedimentoEmbarque: _angelLiraExcecao.ProcedimentoEmbarque.val()
    };
    
    _configuracaoAngelLira.Excecoes.val.push(novoRegistro);
    Global.fecharModal("divModalCadastroExcecaoAngelLira");
}

function validarExcecaoAngelLira() {
    var valorMinimo = Globalize().format(_angelLiraExcecao.ValorMinimo.val(), null, "pt");

    if (valorMinimo <= 0 && _angelLiraExcecao.Destino.codEntity() <= 0)
        return false;
    
    return true;
}