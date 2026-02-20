/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _configuracaoTipoOcorrencia;

/*
 * Declaração das Classes
 */

var ConfiguracaoTipoOcorrencia = function () {
    this.EnviarEmailGeracaoOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.EnviarEmailGerarOcorrencia, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.EmailGeracaoOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.EmailParaEnvio.getFieldDescription(), maxlength: 250, getType: typesKnockout.multiplesEmails });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadConfiguracaoTipoOcorrencia() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador)
        $("#liTabConfiguracao").show();

    _configuracaoTipoOcorrencia = new ConfiguracaoTipoOcorrencia();
    KoBindings(_configuracaoTipoOcorrencia, "knockoutConfiguracao");
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposConfiguracaoTipoOcorrencia() {
    LimparCampos(_configuracaoTipoOcorrencia);
}

function preencherConfiguracaoTipoOcorrencia(dadosConfiguracao) {
    PreencherObjetoKnout(_configuracaoTipoOcorrencia, { Data: dadosConfiguracao });
}

function preencherConfiguracaoTipoOcorrenciaSalvar(tipoOcorrencia) {
    tipoOcorrencia["EnviarEmailGeracaoOcorrencia"] = _configuracaoTipoOcorrencia.EnviarEmailGeracaoOcorrencia.val();
    tipoOcorrencia["EmailGeracaoOcorrencia"] = _configuracaoTipoOcorrencia.EmailGeracaoOcorrencia.val();
}
