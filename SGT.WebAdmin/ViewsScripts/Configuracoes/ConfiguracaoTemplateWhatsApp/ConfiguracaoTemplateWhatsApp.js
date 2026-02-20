/// <reference path="../../../Enumeradores/EnumIdiomaTemplateMensagemWhatsApp.js" />

var _mensagem;
var _gridListaTemplates;
var _pesquisaTemplates;

var Mensagem = function () {
    this.Codigo = PropertyEntity({ type: types.int });
    this.NomeTemplate = PropertyEntity({ text: Localization.Resources.Configuracoes.ConfiguracaoTemplateWhatsApp.NomeTemplate, val: ko.observable(""), def: "", maxlength: 50, enable: ko.observable(true) });
    this.CorpoMensagem = PropertyEntity({ text: Localization.Resources.Configuracoes.ConfiguracaoTemplateWhatsApp.CorpoMensagem, enable: ko.observable(true), maxlength: 1000, val: ko.observable("") });
    this.Grid = PropertyEntity({ type: types.local });
    this.TagNumeroPedidoWhatsApp = PropertyEntity({ eventClick: function (e) { InserirTag(this.CorpoMensagem.id, "#NumeroPedido"); }, type: types.event, text: Localization.Resources.Configuracoes.ConfiguracaoTemplateWhatsApp.NumeroPedido, enable: ko.observable(true), visible: ko.observable(false) });
    this.TagLinkAcompanhamentoEntrega = PropertyEntity({ eventClick: function (e) { InserirTag(this.CorpoMensagem.id, "#LinkAcompanhamentoEntrega"); }, type: types.event, text: Localization.Resources.Configuracoes.ConfiguracaoTemplateWhatsApp.LinkAcompanhamentoEntrega, enable: ko.observable(true), visible: ko.observable(false) });
    this.TagNomeCliente = PropertyEntity({ eventClick: function (e) { InserirTag(this.CorpoMensagem.id, "#NomeCliente"); }, type: types.event, text: Localization.Resources.Configuracoes.ConfiguracaoTemplateWhatsApp.NomeCliente, enable: ko.observable(true), visible: ko.observable(false) });
    this.Idioma = PropertyEntity({ val: ko.observable(""), def: EnumIdiomaTemplateMensagemWhatsApp.Portugues, text: Localization.Resources.Configuracoes.ConfiguracaoTemplateWhatsApp.Idioma, options: EnumIdiomaTemplateMensagemWhatsApp.obterOpcoes(), required: true, enable: ko.observable(true) });
    this.TipoTemplate = PropertyEntity({ val: ko.observable(""), def: EnumTipoTemplateWhatsApp.AcompanhamentoCarga, text: Localization.Resources.Configuracoes.ConfiguracaoTemplateWhatsApp.TipoTemplate, options: EnumTipoTemplateWhatsApp.obterOpcoes(), required: true, enable: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarTemplateClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
}

var PesquisaTemplates = function () {
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridTemplateMensagemWhatsApp, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

function loadMensagem() {
    _mensagem = new Mensagem();
    KoBindings(_mensagem, "knockoutMensagemWhatsApp");

    _pesquisaTemplates = new PesquisaTemplates();
    KoBindings(_pesquisaTemplates, "knockoutPesquisaTemplates", false, _pesquisaTemplates.Pesquisar.id);

    $("#corpoMensagem").summernote({
        toolbar: [
            ['font', ['bold', 'underline', 'clear']]
        ]
    });

    ControlarVisibilidadeTags();

    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.VerDetalhes, id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridListaTemplates = new GridView(_pesquisaTemplates.Pesquisar.idGrid, "ConfiguracaoTemplateWhatsApp/Pesquisa", _pesquisaTemplates, menuOpcoes);
    _gridListaTemplates.CarregarGrid();
}

function editarClick(registroSelecionado) {
    _mensagem.Codigo.val(registroSelecionado.Codigo);
    BuscarPorCodigo(_mensagem, "ConfiguracaoTemplateWhatsApp/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_mensagem, { Data: retorno.Data });
                _mensagem.Adicionar.visible(false);
                HabilitarCampos(false);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function adicionarTemplateClick() {
    var dados = {
        Nome: _mensagem.NomeTemplate.val(),
        Idioma: _mensagem.Idioma.val(),
        CorpoMensagem: _mensagem.CorpoMensagem.val(),
        TipoTemplate: _mensagem.TipoTemplate.val()
    };

    executarReST("ConfiguracaoTemplateWhatsApp/Adicionar", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                limparCamposConfiguracaoMensagemWhatsApp();
                _gridListaTemplates.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function cancelarClick() {
    limparCamposTemplateWhatsApp();
    HabilitarCampos(true);
    _mensagem.Adicionar.visible(true);
}

function limparCamposTemplateWhatsApp() {
    LimparCampos(_mensagem);
}
function recarregarGridTemplateMensagemWhatsApp() {
    _gridListaTemplates.CarregarGrid();
}

function limparCamposConfiguracaoMensagemWhatsApp() {
    LimparCampos(_mensagem);
}

function HabilitarCampos(e) {
    _mensagem.CorpoMensagem.enable(e);
    _mensagem.NomeTemplate.enable(e);
    _mensagem.Idioma.enable(e);
    _mensagem.Idioma.enable(e);
    _mensagem.TagLinkAcompanhamentoEntrega.enable(e);
    _mensagem.TagNumeroPedidoWhatsApp.enable(e);
    _mensagem.TagNomeCliente.enable(e);
}

function ControlarVisibilidadeTags() {
    if (_mensagem.TipoTemplate.val() == EnumTipoTemplateWhatsApp.AcompanhamentoEntrega) {
        _mensagem.TagLinkAcompanhamentoEntrega.visible(true);
        _mensagem.TagNumeroPedidoWhatsApp.visible(true);
        _mensagem.TagNomeCliente.visible(true);
    }
}