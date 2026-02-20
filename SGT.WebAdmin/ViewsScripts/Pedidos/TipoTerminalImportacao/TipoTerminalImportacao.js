/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoTerminalImportacao;
var _tipoTerminalImportacao;
var _pesquisaTipoTerminalImportacao;

var PesquisaTipoTerminalImportacao = function () {

    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Porto = PropertyEntity({ text: "Porto: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoTerminalImportacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var TipoTerminalImportacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 150 });
    this.CodigoIntegracao = PropertyEntity({ text: "Cód. Integração:", required: false, maxlength: 50 });
    this.CodigoDocumento = PropertyEntity({ text: "Cód. Documento:", required: false, maxlength: 50 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.CodigoTerminal = PropertyEntity({ text: "*Código Terminal:", required: ko.observable(true), maxlength: 50 });
    this.Porto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Porto:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Terminal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Terminal:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.CodigoMercante = PropertyEntity({ text: "Cod. Mercante: ", required: ko.observable(false), maxlength: 50, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.CodigoObservacaoContribuinte = PropertyEntity({ text: "Cod. OBS Contribuinte CT-e: ", required: ko.observable(false), maxlength: 500, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    
    this.QuantidadeDiasEnvioDocumentacao = PropertyEntity({ text: "Qtd. Dias Envio Documentação antes do ETA: ", required: ko.observable(false), maxlength: 50, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS), getType: typesKnockout.int });    

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadTipoTerminalImportacao() {

    _tipoTerminalImportacao = new TipoTerminalImportacao();
    KoBindings(_tipoTerminalImportacao, "knockoutCadastroTipoTerminalImportacao");

    _pesquisaTipoTerminalImportacao = new PesquisaTipoTerminalImportacao();
    KoBindings(_pesquisaTipoTerminalImportacao, "knockoutPesquisaTipoTerminalImportacao", false, _pesquisaTipoTerminalImportacao.Pesquisar.id);

    HeaderAuditoria("TipoTerminalImportacao", _tipoTerminalImportacao);

    new BuscarPorto(_tipoTerminalImportacao.Porto);
    new BuscarClientes(_tipoTerminalImportacao.Terminal);
    new BuscarTransportadores(_tipoTerminalImportacao.Empresa);

    buscarTipoTerminalImportacaos();
}


function adicionarClick(e, sender) {
    Salvar(e, "TipoTerminalImportacao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTipoTerminalImportacao.CarregarGrid();
                limparCamposTipoTerminalImportacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "TipoTerminalImportacao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoTerminalImportacao.CarregarGrid();
                limparCamposTipoTerminalImportacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o terminal selecionado?", function () {
        ExcluirPorCodigo(_tipoTerminalImportacao, "TipoTerminalImportacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridTipoTerminalImportacao.CarregarGrid();
                limparCamposTipoTerminalImportacao();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTipoTerminalImportacao();
}

//*******MÉTODOS*******


function buscarTipoTerminalImportacaos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoTerminalImportacao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoTerminalImportacao = new GridView(_pesquisaTipoTerminalImportacao.Pesquisar.idGrid, "TipoTerminalImportacao/Pesquisa", _pesquisaTipoTerminalImportacao, menuOpcoes, null);
    _gridTipoTerminalImportacao.CarregarGrid();
}

function editarTipoTerminalImportacao(tipoTerminalImportacaoGrid) {
    limparCamposTipoTerminalImportacao();
    _tipoTerminalImportacao.Codigo.val(tipoTerminalImportacaoGrid.Codigo);
    BuscarPorCodigo(_tipoTerminalImportacao, "TipoTerminalImportacao/BuscarPorCodigo", function (arg) {
        _pesquisaTipoTerminalImportacao.ExibirFiltros.visibleFade(false);
        _tipoTerminalImportacao.Atualizar.visible(true);
        _tipoTerminalImportacao.Cancelar.visible(true);
        _tipoTerminalImportacao.Excluir.visible(true);
        _tipoTerminalImportacao.Adicionar.visible(false);
    }, null);
}

function limparCamposTipoTerminalImportacao() {
    _tipoTerminalImportacao.Atualizar.visible(false);
    _tipoTerminalImportacao.Cancelar.visible(false);
    _tipoTerminalImportacao.Excluir.visible(false);
    _tipoTerminalImportacao.Adicionar.visible(true);
    LimparCampos(_tipoTerminalImportacao);
}
