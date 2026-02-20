/*
 * Declaração de Objetos Globais do Arquivo
 */

var _configuracaoProvisao;
var _CRUDConfiguracaoProvisao;

/*
 * Declaração das Classes
 */

var ConfiguracaoProvisao = function () {
    this.DiasForaMes = PropertyEntity({ text: "Dias fora mês:", getType: typesKnockout.int, val: ko.observable(0), configInt: { precision: 0, allowZero: false, thousands: '' }, maxlength: 2 });
};

var CRUDConfiguracaoProvisao = function () {
    this.Salvar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Salvar", icon: "fa fa-save", visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadConfiguracaoProvisao() {
    _configuracaoProvisao = new ConfiguracaoProvisao();
    KoBindings(_configuracaoProvisao, "knockoutConfiguracaoProvisao");

    _CRUDConfiguracaoProvisao = new CRUDConfiguracaoProvisao();
    KoBindings(_CRUDConfiguracaoProvisao, "knockoutCRUDConfiguracaoProvisao");

    HeaderAuditoria("ConfiguracaoProvisao", _configuracaoProvisao);

    buscarConfiguracao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function atualizarClick() {
    if (ValidarCamposObrigatorios(_configuracaoProvisao)) {
        executarReST("ConfiguracaoProvisao/Atualizar", obterConfiguracaoSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        });
    }
}

/*
 * Declaração das Funções
 */

function buscarConfiguracao() {
    executarReST("ConfiguracaoProvisao/ObterConfiguracao", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                PreencherObjetoKnout(_configuracaoProvisao, retorno);
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function obterConfiguracaoSalvar() {
    return RetornarObjetoPesquisa(_configuracaoProvisao);
}
