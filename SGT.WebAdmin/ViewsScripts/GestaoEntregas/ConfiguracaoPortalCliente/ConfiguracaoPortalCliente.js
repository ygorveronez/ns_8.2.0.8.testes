/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoPortalCliente;
var _CRUDConfiguracaoPortalCliente;

var ConfiguracaoPortalCliente = function () {
    this.ExibirMapa = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.ExibirMapa, val: ko.observable(false) });
    this.ExibirDetalhesPedido = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.ExibirDetalhesPedido, val: ko.observable(true), def: true });
    this.ExibirHistoricoPedido = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.ExibirHistoricoPedido, val: ko.observable(true), def: true });
    this.ExibirDetalhesMotorista = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.ExibirDetalhesMotorista, val: ko.observable(true), def: true });
    this.ExibirDetalhesProduto = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.ExibirDetalhesProduto, val: ko.observable(true), def: true });
    this.ExibirProduto = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.ExibirProduto, val: ko.observable(true), def: true });
    this.HabilitarAvaliacao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.HabilitarAvaliacao, val: ko.observable(false) });
    this.HabilitarPrevisaoEntrega = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.HabilitarPrevisaoEntrega, val: ko.observable(false), def: false });
    this.HabilitarObservacao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.HabilitarObservacao, val: ko.observable(false), def: false });
    this.HabilitarNumeroPedidoCliente = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.HabilitarNumeroPedidoCliente, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.HabilitarNumeroOrdemCompra = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.HabilitarNumeroOrdemCompra, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.PesoBruto = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.PesoBruto, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.PesoLiquido = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.PesoLiquido, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.QuantidadeVolumes = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.QuantidadeVolumes, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.PermitirAdicionarAnexos = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.PermitirAdicionarAnexos, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.HabilitarVisualizacaoFotosPortal = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.HabilitarVisualizacaoFotosPortal, val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.EnviarSMS = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.EnviarSMS, val: ko.observable(false) });
    this.EnviarEmail = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.EnviarEmail, val: ko.observable(false) });
    this.ParaHabilitarVerifiqueCustoJuntoAMultisoftware = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.ParaHabilitarVerifiqueCustoJuntoAMultisoftware, visible: ko.observable(true) });

    this.LinkAvaliacaoExterna = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.LinkAvaliacaoExterna, val: ko.observable("") });
    this.LinkAcessoPortalMultiCliFor = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.LinkAcessoPortalMultiCliFor, val: ko.observable("") });
    this.HabilitarAcessoPortalMultiCliFor = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.HabilitarAcessoPortalMultiCliFor, val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.TipoAvaliacao = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.TipoAvaliacao, val: ko.observable(EnumTipoAvaliacaoPortalCliente.Geral), options: EnumTipoAvaliacaoPortalCliente.obterOpcoes() });
    this.TipoAvaliacao.val.subscribe(valor => {
        switch (valor) {
            case EnumTipoAvaliacaoPortalCliente.Geral:
                this.PermitirAdicionarAnexos.visible(false);
                $("#tabAvaliacao")['hide']();
                break;
            case EnumTipoAvaliacaoPortalCliente.Individual:
                this.PermitirAdicionarAnexos.visible(true);
                $("#tabAvaliacao")['show']();
                break;
        }
    });
}

var CRUDConfiguracaoPortalCliente = function () {
    this.Salvar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, icon: "fa fa-save", visible: ko.observable(true) });
};


//*******EVENTOS*******
function loadConfiguracaoPortalCliente() {
    _configuracaoPortalCliente = new ConfiguracaoPortalCliente();
    KoBindings(_configuracaoPortalCliente, "knockoutConfiguracaoPortalCliente");

    _CRUDConfiguracaoPortalCliente = new CRUDConfiguracaoPortalCliente();
    KoBindings(_CRUDConfiguracaoPortalCliente, "knockoutCRUDConfiguracaoPortalCliente");

    HeaderAuditoria("ConfiguracaoPortalCliente", _configuracaoPortalCliente);
    
    if (_CONFIGURACAO_TMS.Pais == EnumPaises.Exterior) {
        _configuracaoPortalCliente.HabilitarNumeroPedidoCliente.visible(true);
        _configuracaoPortalCliente.HabilitarNumeroOrdemCompra.visible(true);
    }

    loadConfiguracaoAvaliacao();

    buscarConfiguracaoPortalCliente();

    if (_CONFIGURACAO_TMS.PossuiIntegracaoWhatsApp) {
        _configuracaoPortalCliente.ParaHabilitarVerifiqueCustoJuntoAMultisoftware.visible(false);
    }
}

function atualizarClick(e, sender) {
    if (ValidarCamposObrigatorios(_configuracaoPortalCliente)) {
        executarReST("ConfiguracaoPortalCliente/Atualizar", obterDadosConfiguracao(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

function buscarConfiguracaoPortalCliente() {
    executarReST("ConfiguracaoPortalCliente/ObterConfiguracao", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_configuracaoPortalCliente, retorno);
                $("#divContent").removeClass("hidden");
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function obterDadosConfiguracao() {
    var configuracao = RetornarObjetoPesquisa(_configuracaoPortalCliente);

    return configuracao;
}