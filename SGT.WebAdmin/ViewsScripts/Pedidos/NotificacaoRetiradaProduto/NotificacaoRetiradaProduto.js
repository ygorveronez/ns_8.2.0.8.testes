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
/// <reference path="../../Consultas/Usuario.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridNotificacaoRetiradaProduto;
var _notificacaoRetiradaProduto;
var _crudNotificacaoRetiradaProduto;
var _pesquisaNotificacaoRetiradaProduto;

var PesquisaNotificacaoRetiradaProduto = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Situacao = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridNotificacaoRetiradaProduto.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var NotificacaoRetiradaProduto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", maxlength: 500, required: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "*Situação: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true) });

    this.Email = PropertyEntity({ val: ko.observable("") });

    this.TagRemetente = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagRazaoSocialCliente"); }, type: types.event, text: "Razão Social Cliente" });
    this.TagDestinatario = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagCNPJCliente"); }, type: types.event, text: "CNPJ Cliente" });
    this.TagNumeroPedido = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagEnderecoCliente"); }, type: types.event, text: "Endereço Cliente" });
    this.TagQuantidadePedidos = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagComplementoCliente"); }, type: types.event, text: "Complemento Cliente" });
    this.TagDataHoraAgendamento = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagBairroCliente"); }, type: types.event, text: "Bairro Cliente" });

    this.Destinatarios = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var CRUDNotificacaoRetiradaProduto = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadNotificacaoRetiradaProduto() {

    _pesquisaNotificacaoRetiradaProduto = new PesquisaNotificacaoRetiradaProduto();
    KoBindings(_pesquisaNotificacaoRetiradaProduto, "knockoutPesquisaNotificacaoRetiradaProduto", false, _pesquisaNotificacaoRetiradaProduto.Pesquisar.id);

    _notificacaoRetiradaProduto = new NotificacaoRetiradaProduto();
    KoBindings(_notificacaoRetiradaProduto, "knockoutCadastroNotificacaoRetiradaProduto");

    _crudNotificacaoRetiradaProduto = new CRUDNotificacaoRetiradaProduto();
    KoBindings(_crudNotificacaoRetiradaProduto, "knockoutCRUDNotificacaoRetiradaProduto");

    buscarNotificacaoRetiradaProduto();
    LoadDestinatario();

    $("#txtEditor").summernote({
        toolbar: [
            ['style', ['style']],
            ['font', ['bold', 'underline', 'clear']],
            ['fontname', ['fontname']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['table', ['table']],
            ['insert', ['link']],
            ['view', ['fullscreen', 'codeview']],
        ]
    });
}

function adicionarClick(e, sender) {
    var emailSalvar = ObterEmailSalvar();

    if (!ValidarCamposObrigatorios(_notificacaoRetiradaProduto) || string.IsNullOrWhiteSpace(emailSalvar.Email))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios!", "Informe os Campos Obrigatórios!");

    executarReST("NotificacaoRetiradaProduto/Adicionar", emailSalvar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com Sucesso!");
                _gridNotificacaoRetiradaProduto.CarregarGrid();
                limparCamposNotificacaoRetiradaProduto();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarClick(e, sender) {
    var emailSalvar = ObterEmailSalvar();

    if (!ValidarCamposObrigatorios(_notificacaoRetiradaProduto) || string.IsNullOrWhiteSpace(emailSalvar.Email) || emailSalvar.Email == "<br>")
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios!", "Informe os Campos Obrigatórios!");

    executarReST("NotificacaoRetiradaProduto/Atualizar", emailSalvar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridNotificacaoRetiradaProduto.CarregarGrid();
                limparCamposNotificacaoRetiradaProduto();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o contrato selecionado?", function () {
        ExcluirPorCodigo(_notificacaoRetiradaProduto, "NotificacaoRetiradaProduto/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridNotificacaoRetiradaProduto.CarregarGrid();
                    limparCamposNotificacaoRetiradaProduto();
                }
                else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposNotificacaoRetiradaProduto();
}

function removerArquivoClick(e, sender) {
    if (_notificacaoRetiradaProduto.CodigoAnexo.val() > 0) {
        exibirConfirmacao("Confirmação", "Realmente deseja excluir o Anexo do Contrato?", function () {
            var codigoAnexo = _notificacaoRetiradaProduto.CodigoAnexo.val();
            limparCamposContratoNotaFiscalAnexo();
            _notificacaoRetiradaProduto.CodigoAnexoRemovido.val(codigoAnexo);
        });
    }
}

//*******MÉTODOS*******

function buscarNotificacaoRetiradaProduto() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarNotificacaoRetiradaProduto, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridNotificacaoRetiradaProduto = new GridView(_pesquisaNotificacaoRetiradaProduto.Pesquisar.idGrid, "NotificacaoRetiradaProduto/Pesquisa", _pesquisaNotificacaoRetiradaProduto, menuOpcoes, null);
    _gridNotificacaoRetiradaProduto.CarregarGrid();
}

function editarNotificacaoRetiradaProduto(notificacaoRetiradaProduto) {
    limparCamposNotificacaoRetiradaProduto();
    _notificacaoRetiradaProduto.Codigo.val(notificacaoRetiradaProduto.Codigo);

    BuscarPorCodigo(_notificacaoRetiradaProduto, "NotificacaoRetiradaProduto/BuscarPorCodigo", function (arg) {
        _pesquisaNotificacaoRetiradaProduto.ExibirFiltros.visibleFade(false);
        _crudNotificacaoRetiradaProduto.Atualizar.visible(true);
        _crudNotificacaoRetiradaProduto.Cancelar.visible(true);
        _crudNotificacaoRetiradaProduto.Excluir.visible(true);
        _crudNotificacaoRetiradaProduto.Adicionar.visible(false);

        RecarregarGridDestinatarios();
        $("#txtEditor").summernote('code', arg.Data.Email);
    }, null);
}

function limparCamposNotificacaoRetiradaProduto() {
    _crudNotificacaoRetiradaProduto.Atualizar.visible(false);
    _crudNotificacaoRetiradaProduto.Cancelar.visible(false);
    _crudNotificacaoRetiradaProduto.Excluir.visible(false);
    _crudNotificacaoRetiradaProduto.Adicionar.visible(true);
    LimparCampos(_notificacaoRetiradaProduto);
    LimparGridDestinatarios();

    $("#txtEditor").summernote('code', '');

    Global.ResetarAbas();
}

function inserirTagTextoEdicao(tag) {
    $("#txtEditor").summernote('editor.insertText', tag);
}

function ObterEmailSalvar() {
    _notificacaoRetiradaProduto.Email.val($('#txtEditor').summernote('code'));

    PreencherListaDestinatarios();

    var email = RetornarObjetoPesquisa(_notificacaoRetiradaProduto);

    return email;
}

function PreencherListaDestinatarios() {
    _notificacaoRetiradaProduto.Destinatarios.val(JSON.stringify(_destinatarios.AdicionarDestinatario.basicTable.BuscarRegistros()));
}