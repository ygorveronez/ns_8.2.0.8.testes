/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoConfiguracaoAlerta.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDConfiguracaoAlerta;
var _configuracaoAlerta;
var _pesquisaConfiguracaoAlerta;
var _gridConfiguracaoAlerta;
var _gridConfiguracaoAlertaUsuario;

/*
 * Declaração das Classes
 */

var CRUDConfiguracaoAlerta = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo" });
}

var ConfiguracaoAlerta = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.AlertarAposVencimento = PropertyEntity({ text: "Alertar após o vencimento?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.AlertarTransportador = PropertyEntity({ text: "Alertar transportador?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.DiasAlertarAntesVencimento = PropertyEntity({ text: "Dias para enviar alerta antes do vencimento:", getType: typesKnockout.int, val: ko.observable(0), configInt: { precision: 0, allowZero: true }, maxlength: 2, def: 0, visible: ko.observable(true) });
    this.DiasRepetirAlerta = PropertyEntity({ text: "Dias aguardar para repetir o alerta:", getType: typesKnockout.int, val: ko.observable(""), configInt: { precision: 0, allowZero: false }, maxlength: 2, visible: ko.observable(true) });
    this.Tipo = PropertyEntity({ text: "*Tipo: ", val: ko.observable(EnumTipoConfiguracaoAlerta.Todos), options: EnumTipoConfiguracaoAlerta.obterOpcoes(), def: EnumTipoConfiguracaoAlerta.Todos, required: true });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.CodigosRejeicoes = PropertyEntity({ val: ko.observable(""), def: "", text: "*Códigos das Rejeições (Separar Por vírgula):", required: ko.observable(false), visible: ko.observable(false) });

    this.Tipo.val.subscribe(tipoChange);

    this.ListaUsuario = PropertyEntity({ type: types.map, required: false, text: "Adicionar Usuário", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

var PesquisaConfiguracaoAlerta = function () {
    this.Tipo = PropertyEntity({ text: "Tipo: ", val: ko.observable(EnumTipoConfiguracaoAlerta.Todos), options: EnumTipoConfiguracaoAlerta.obterOpcoesPesquisa(), def: EnumTipoConfiguracaoAlerta.Todos });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridConfiguracaoAlerta, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridConfiguracaoAlerta() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "ConfiguracaoAlerta/ExportarPesquisa", titulo: "Configurações de Alerta" };

    _gridConfiguracaoAlerta = new GridViewExportacao(_pesquisaConfiguracaoAlerta.Pesquisar.idGrid, "ConfiguracaoAlerta/Pesquisa", _pesquisaConfiguracaoAlerta, menuOpcoes, configuracoesExportacao);
    _gridConfiguracaoAlerta.CarregarGrid();
}

function loadGridConfiguracaoAlertaUsuario() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerConfiguracaoAlertaUsuario, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridConfiguracaoAlertaUsuario = new BasicDataTable(_configuracaoAlerta.ListaUsuario.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarFuncionario(_configuracaoAlerta.ListaUsuario, null, _gridConfiguracaoAlertaUsuario);
    _configuracaoAlerta.ListaUsuario.basicTable = _gridConfiguracaoAlertaUsuario;

    _gridConfiguracaoAlertaUsuario.CarregarGrid([]);
}

function loadConfiguracaoAlerta() {
    _configuracaoAlerta = new ConfiguracaoAlerta();
    KoBindings(_configuracaoAlerta, "knockoutConfiguracaoAlerta");

    HeaderAuditoria("ConfiguracaoAlerta", _configuracaoAlerta);

    _CRUDConfiguracaoAlerta = new CRUDConfiguracaoAlerta();
    KoBindings(_CRUDConfiguracaoAlerta, "knockoutCRUDConfiguracaoAlerta");

    _pesquisaConfiguracaoAlerta = new PesquisaConfiguracaoAlerta();
    KoBindings(_pesquisaConfiguracaoAlerta, "knockoutPesquisaConfiguracaoAlerta", false, _pesquisaConfiguracaoAlerta.Pesquisar.id);

    loadGridConfiguracaoAlerta();
    loadGridConfiguracaoAlertaUsuario();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick() {
    if (!validarCamposObrigatoriosConfiguracaoAlerta())
        return;

    executarReST("ConfiguracaoAlerta/Adicionar", obterConfiguracaoAlertaSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                recarregarGridConfiguracaoAlerta();
                limparCamposConfiguracaoAlerta();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarClick() {
    if (!validarCamposObrigatoriosConfiguracaoAlerta())
        return;

    executarReST("ConfiguracaoAlerta/Atualizar", obterConfiguracaoAlertaSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                recarregarGridConfiguracaoAlerta();
                limparCamposConfiguracaoAlerta();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function cancelarClick() {
    limparCamposConfiguracaoAlerta();
}

function editarClick(registroSelecionado) {
    limparCamposConfiguracaoAlerta();

    _configuracaoAlerta.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_configuracaoAlerta, "ConfiguracaoAlerta/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaConfiguracaoAlerta.ExibirFiltros.visibleFade(false);
                _gridConfiguracaoAlertaUsuario.CarregarGrid(retorno.Data.Usuarios);
                controlarBotoesHabilitados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function tipoChange(tipo) {
    var exibirAlertarAposVencimento = true;
    var exibirAlertarTransportador = false;
    var exibirDiasAlertarAntesVencimento = true;
    var exibirDiasRepetirAlerta = true;

    _configuracaoAlerta.CodigosRejeicoes.visible(false);
    _configuracaoAlerta.CodigosRejeicoes.required(false);
    _configuracaoAlerta.CodigosRejeicoes.val("");

    switch (tipo) {
        case EnumTipoConfiguracaoAlerta.Antt:
        case EnumTipoConfiguracaoAlerta.ApoliceSeguro:
        case EnumTipoConfiguracaoAlerta.CertificadoDigital:
        case EnumTipoConfiguracaoAlerta.Cnh:
        case EnumTipoConfiguracaoAlerta.PendenciaNfsManual:
            exibirAlertarTransportador = true;
            break;

        case EnumTipoConfiguracaoAlerta.PedidoSemTabelaFrete:
        case EnumTipoConfiguracaoAlerta.RotaNaoCadastrada:
            exibirAlertarAposVencimento = false;
            exibirDiasAlertarAntesVencimento = false;
            exibirDiasRepetirAlerta = false;
            break;

        case EnumTipoConfiguracaoAlerta.MDFEPendenteDeEncerramento:
            _configuracaoAlerta.CodigosRejeicoes.visible(true);
            _configuracaoAlerta.CodigosRejeicoes.required(true);
            exibirAlertarTransportador = true;
            break;
    }

    _configuracaoAlerta.AlertarAposVencimento.visible(exibirAlertarAposVencimento);
    _configuracaoAlerta.AlertarTransportador.visible(exibirAlertarTransportador);
    _configuracaoAlerta.DiasAlertarAntesVencimento.visible(exibirDiasAlertarAntesVencimento);
    _configuracaoAlerta.DiasRepetirAlerta.visible(exibirDiasRepetirAlerta);

    if (!exibirAlertarAposVencimento)
        _configuracaoAlerta.AlertarAposVencimento.val(_configuracaoAlerta.AlertarAposVencimento.def);

    if (!exibirAlertarTransportador)
        _configuracaoAlerta.AlertarTransportador.val(_configuracaoAlerta.AlertarTransportador.def);

    if (!exibirDiasAlertarAntesVencimento)
        _configuracaoAlerta.DiasAlertarAntesVencimento.val(_configuracaoAlerta.DiasAlertarAntesVencimento.def);

    if (!exibirDiasRepetirAlerta)
        _configuracaoAlerta.DiasRepetirAlerta.val(_configuracaoAlerta.DiasRepetirAlerta.def);
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados() {
    var isEdicao = _configuracaoAlerta.Codigo.val() > 0;

    _CRUDConfiguracaoAlerta.Atualizar.visible(isEdicao);
    _CRUDConfiguracaoAlerta.Adicionar.visible(!isEdicao);
}

function limparCamposConfiguracaoAlerta() {
    LimparCampos(_configuracaoAlerta);

    _gridConfiguracaoAlertaUsuario.CarregarGrid([]);

    controlarBotoesHabilitados();
}

function obterConfiguracaoAlertaSalvar() {
    var configuracaoAlerta = RetornarObjetoPesquisa(_configuracaoAlerta);

    configuracaoAlerta["Usuarios"] = obterListaUsuarioSalvar();

    return configuracaoAlerta;
}

function obterListaUsuario() {
    return _gridConfiguracaoAlertaUsuario.BuscarRegistros();
}

function obterListaUsuarioSalvar() {
    var listaUsuario = obterListaUsuario();
    var listaUsuarioRetornar = new Array();

    listaUsuario.forEach(function (usuario) {
        listaUsuarioRetornar.push({
            Codigo: usuario.Codigo
        });
    });

    return JSON.stringify(listaUsuarioRetornar);
}

function removerConfiguracaoAlertaUsuario(registroSelecionado) {
    var listaUsuario = obterListaUsuario();

    for (var i = 0; i < listaUsuario.length; i++) {
        if (registroSelecionado.Codigo == listaUsuario[i].Codigo) {
            listaUsuario.splice(i, 1);
            break;
        }
    }

    _gridConfiguracaoAlertaUsuario.CarregarGrid(listaUsuario);
}

function recarregarGridConfiguracaoAlerta() {
    _gridConfiguracaoAlerta.CarregarGrid();
}

function validarCamposObrigatoriosConfiguracaoAlerta() {
    if (!ValidarCamposObrigatorios(_configuracaoAlerta)) {
        exibirMensagem("atencao", "Campos Obrigatórios", "Por Favor, informe os campos obrigatórios");
        return false;
    }

    var listaUsuario = obterListaUsuario();

    if (listaUsuario.length == 0) {
        exibirMensagem("atencao", "Campos Obrigatórios", "Por Favor, informe um ou mais usuários para receberem as notificações");
        return false;
    }

    return true;
}