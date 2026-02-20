/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaConfiguracaoVtex;
var _configuracaoVtex;
var _gridConfiguracaoVtex;
var _CRUDConfiguracaoVtex;
var _bindConfiguracoesVtex;

var PesquisaConfiguracaoVtex = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial: ", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _statusPesquisa, def: 0, text: "Situação: " });
    this.Pesquisar = PropertyEntity({ eventClick: function (e) { _gridConfiguracaoVtex.CarregarGrid(); }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.ExibirFiltros = PropertyEntity({ eventClick: function (e) { e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade()); }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
};

var ConfiguracaoVtex = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial: ", idBtnSearch: guid() });
    this.AccountName = PropertyEntity({ text: "*accountName: ", required: true, visible: ko.observable(true) });
    this.Environment = PropertyEntity({ text: "*environment: ", required: true, visible: true});
    this.XVtexApiAppToken = PropertyEntity({ text: "*X-VTEX-API-AppToken: ", required: true, visible: true, maxlength: 500 });
    this.XVtexApiAppKey = PropertyEntity({ text: "*X-VTEX-API-AppKey: ", required: true, visible: true });
    this.QuantidadeNotificacao = PropertyEntity({ text: "Quantidade registros pendentes para notificar: ", required: false, visible: true, getType: typesKnockout.int });
    this.EmailsNotificacao = PropertyEntity({ text: "E-mails para notificação: ", required: false, visible: true });

    this.Situacao = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
};

var CRUDConfiguracaoVtex = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadConfiguracaoVtex() {
    _configuracaoVtex = new ConfiguracaoVtex();
    KoBindings(_configuracaoVtex, "knockoutConfiguracaoVtex");

    _CRUDConfiguracaoVtex = new CRUDConfiguracaoVtex();
    KoBindings(_CRUDConfiguracaoVtex, "knoutCRUDConfiguracaoVtex");

    _pesquisaConfiguracaoVtex = new PesquisaConfiguracaoVtex();
    KoBindings(_pesquisaConfiguracaoVtex, "knockoutPesquisaConfiguracaoVtex", _pesquisaConfiguracaoVtex.Pesquisar.id);

    HeaderAuditoria("ConfiguracaoVtex", _configuracaoVtex);

    new BuscarFilial(_configuracaoVtex.Filial);
    new BuscarFilial(_pesquisaConfiguracaoVtex.Filial);

    BuscarConfiguracaoVtex();
}


function ValidarConfiguracaoVtex() {
    return true;
}

function adicionarClick(e, sender) {
    if (!ValidarConfiguracaoVtex())
        return;

    Salvar(_configuracaoVtex, "ConfiguracaoVtex/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridConfiguracaoVtex.CarregarGrid();
                LimparCamposConfiguracaoVtex();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    if (!ValidarConfiguracaoVtex())
        return;

    Salvar(_configuracaoVtex, "ConfiguracaoVtex/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridConfiguracaoVtex.CarregarGrid();
                LimparCamposConfiguracaoVtex();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);

}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esta configuração?", function () {
        ExcluirPorCodigo(_configuracaoVtex, "ConfiguracaoVtex/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridConfiguracaoVtex.CarregarGrid();
                    LimparCamposConfiguracaoVtex();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    LimparCamposConfiguracaoVtex();
}

//*******MÉTODOS*******

function editarConfiguracaoVtex(config) {
    LimparCamposConfiguracaoVtex();
    _configuracaoVtex.Codigo.val(config.Codigo);
    BuscarPorCodigo(_configuracaoVtex, "ConfiguracaoVtex/BuscarPorCodigo", function (arg) {
        _pesquisaConfiguracaoVtex.ExibirFiltros.visibleFade(false);
        _CRUDConfiguracaoVtex.Atualizar.visible(true);
        _CRUDConfiguracaoVtex.Cancelar.visible(true);
        _CRUDConfiguracaoVtex.Excluir.visible(true);
        _CRUDConfiguracaoVtex.Adicionar.visible(false);
    }, null);
}

function BuscarConfiguracaoVtex() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConfiguracaoVtex, tamanho: "20", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridConfiguracaoVtex = new GridView(_pesquisaConfiguracaoVtex.Pesquisar.idGrid, "ConfiguracaoVtex/Pesquisar", _pesquisaConfiguracaoVtex, menuOpcoes, null);
    _gridConfiguracaoVtex.CarregarGrid();
}

function LimparCamposConfiguracaoVtex() {
    _CRUDConfiguracaoVtex.Atualizar.visible(false);
    _CRUDConfiguracaoVtex.Cancelar.visible(false);
    _CRUDConfiguracaoVtex.Excluir.visible(false);
    _CRUDConfiguracaoVtex.Adicionar.visible(true);
    LimparCampos(_configuracaoVtex);

    $("#myTab a:first").click();
}
