/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Consultas/PortfolioModuloControle.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />

// #region Objetos Globais do Arquivo

var _CRUDContrato;
var _gridContrato;
var _contrato;
var _pesquisaContrato;
var _PermissoesPersonalizadas;
// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaContrato = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: Localization.Resources.Escrituracao.ContratoFreteCliente.FiltrosPesquisa.getFieldDescription(), idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.NumeroContrato = PropertyEntity({ text: Localization.Resources.Escrituracao.ContratoFreteCliente.NumeroContrato.getFieldDescription(), val: ko.observable(""), def: "", maxlentgh: 100 });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Escrituracao.ContratoFreteCliente.Descricao.getFieldDescription(), val: ko.observable(""), def: "", maxlentgh: 100 });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Escrituracao.ContratoFreteCliente.TipoOperacao.getFieldDescription(), visible: ko.observable(true), idBtnSearch: guid() });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Escrituracao.ContratoFreteCliente.Cliente.getFieldDescription(), visible: ko.observable(true), idBtnSearch: guid() });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridContrato, type: types.event, text: Localization.Resources.Escrituracao.ContratoFreteCliente.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
}

var Contrato = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroContrato = PropertyEntity({ text: Localization.Resources.Escrituracao.ContratoFreteCliente.NumeroContrato.getRequiredFieldDescription(), val: ko.observable(""), getType: typesKnockout.string, required: ko.observable(true), enable: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Escrituracao.ContratoFreteCliente.Descricao.getRequiredFieldDescription(), val: ko.observable(""), def: "", maxlentgh: 100, required: ko.observable(true), enable: ko.observable(true) });
    this.ValorContrato = PropertyEntity({ val: ko.observable(0), text: Localization.Resources.Escrituracao.ContratoFreteCliente.ValorContrato.getRequiredFieldDescription(), def: 0, getType: typesKnockout.decimal, required: ko.observable(true), enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Escrituracao.ContratoFreteCliente.TipoOperacao.getRequiredFieldDescription(), visible: ko.observable(true), idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Escrituracao.ContratoFreteCliente.Cliente.getRequiredFieldDescription(), visible: ko.observable(true), idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true) });
    this.DataInicioContrato = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Escrituracao.ContratoFreteCliente.DataInicioContrato.getRequiredFieldDescription(), required: true, enable: ko.observable(true) });
    this.DataFimContrato = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Escrituracao.ContratoFreteCliente.DataFimContrato.getRequiredFieldDescription(), required: true, enable: ko.observable(true) });
    this.Fechado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.Status = PropertyEntity({ text: Localization.Resources.Escrituracao.ContratoFreteCliente.Status.getFieldDescription(), val: ko.observable("Aberto"), visible: ko.observable(false), enable: false });
}

var CRUDContrato = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Escrituracao.ContratoFreteCliente.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Escrituracao.ContratoFreteCliente.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Escrituracao.ContratoFreteCliente.Cancelar, visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadContrato() {
    _pesquisaContrato = new PesquisaContrato();
    KoBindings(_pesquisaContrato, "knockoutPesquisaContrato", false, _pesquisaContrato.Pesquisar.id);

    _contrato = new Contrato();
    KoBindings(_contrato, "knockoutContrato");

    _CRUDContrato = new CRUDContrato();
    KoBindings(_CRUDContrato, "knockoutCRUDContrato");

    HeaderAuditoria("ContratoFreteCliente", _pesquisaContrato);

    loadGridContrato();

    loadBuscaModais();

}

function loadGridContrato() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { editarClick(item); }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridContrato = new GridView(_pesquisaContrato.Pesquisar.idGrid, "ContratoFreteCliente/Pesquisa", _pesquisaContrato, menuOpcoes);
    _gridContrato.CarregarGrid();
}

function loadBuscaModais() {
    new BuscarTiposOperacao(_pesquisaContrato.TipoOperacao, null);
    new BuscarTiposOperacao(_contrato.TipoOperacao, null);
    new BuscarClientes(_contrato.Cliente, null);
    new BuscarClientes(_pesquisaContrato.Cliente, null);
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    Salvar(_contrato, "ContratoFreteCliente/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridContrato();
                limparCamposContrato();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_contrato, "ContratoFreteCliente/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridContrato();
                limparCamposContrato();
                HabilitarApenasDataFimContratoParaAtualizacao(false);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposContrato();
    HabilitarApenasDataFimContratoParaAtualizacao(false);
}

function editarClick(registroSelecionado) {
    limparCamposContrato();

    _contrato.Codigo.val(registroSelecionado.Codigo);
    _pesquisaContrato.Codigo.val(registroSelecionado.Codigo);
    _contrato.Fechado.val(registroSelecionado.Fechado);

    BuscarPorCodigo(_contrato, "ContratoFreteCliente/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaContrato.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                controlarBotoesHabilitados(isEdicao);

                HabilitarApenasDataFimContratoParaAtualizacao(true);

                _contrato.Status.visible(true);
                _contrato.Status.val(retorno.Data.Status);
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

// #endregion Funções Associadas a Eventos

// #region Métodos privados

function controlarBotoesHabilitados(isEdicao) {
    _CRUDContrato.Atualizar.visible(isEdicao);
    _CRUDContrato.Cancelar.visible(isEdicao);
    _CRUDContrato.Adicionar.visible(!isEdicao);
}

function limparCamposContrato() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_contrato);
    _pesquisaContrato.Codigo.val(0);
    exibirFiltros();
}

function recarregarGridContrato() {
    _gridContrato.CarregarGrid();
}

function exibirFiltros() {
    if (!_pesquisaContrato.ExibirFiltros.visibleFade())
        _pesquisaContrato.ExibirFiltros.visibleFade(true);
}

function HabilitarApenasDataFimContratoParaAtualizacao(atualizandoContrato) {
    _contrato.NumeroContrato.enable(!atualizandoContrato);
    _contrato.Descricao.enable(!atualizandoContrato);
    _contrato.Cliente.enable(!atualizandoContrato);
    _contrato.TipoOperacao.enable(!atualizandoContrato);
    _contrato.ValorContrato.enable(!atualizandoContrato);
    _contrato.DataInicioContrato.enable(!atualizandoContrato);
    _contrato.Status.visible(false);

    if ((_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Alterar, _PermissoesPersonalizadas)) && !_contrato.Fechado.val()) {
        _contrato.DataFimContrato.enable(atualizandoContrato);
    } else {
        _contrato.DataFimContrato.enable(!atualizandoContrato);
    }
}

// #endregion Métodos privados