/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
///<reference path="../../../ViewsScripts/Enumeradores/EnumAtivoInativo.js"/>
///<reference path="../../../ViewsScripts/Enumeradores/EnumTipoRegraQualidadeMonitoramento.js"/>

//#region Objetos Globais do Arquivo
var _pesquisaRegrasQualidadeMonitoramento;
var _gridPesquisaRegrasQualidadeMonitoramento;
var _regraQualidadeMonitoramento;
var _crudRegraQualidadeMonitoramento;
// #endregion Objetos Globais do Arquivo

//#region Classes
var PesquisaRegrasQualidadeMonitoramento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable(""), def: "" });
    this.TipoRegra = PropertyEntity({ val: ko.observable(EnumTipoRegraQualidadeMonitoramento.Todas), options: EnumTipoRegraQualidadeMonitoramento.obterOpcoesPesquisa(), def: EnumTipoRegraQualidadeMonitoramento.Todas, text: "Tipo de regra: " });
    this.Status = PropertyEntity({ text: "Status:", val: ko.observable(EnumAtivoInativo.Todos), def: ko.observable(EnumAtivoInativo.Todos), options: EnumAtivoInativo.obterOpcoesPesquisa() });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: loadGridRegrasQualidadeMonitoramento, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Exibir Filtros", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

var RegraQualidadeMonitoramento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", val: ko.observable(""), def: "", required: true });
    this.TipoRegra = PropertyEntity({ text: "*Tipo de regra: ", val: ko.observable(EnumTipoRegraQualidadeMonitoramento.PreEmbarque), options: EnumTipoRegraQualidadeMonitoramento.obterOpcoes(), def: EnumTipoRegraQualidadeMonitoramento.PreEmbarque, required: true });
    this.Status = PropertyEntity({ text: "*Status:", val: ko.observable(EnumAtivoInativo.Ativo), def: ko.observable(EnumAtivoInativo.Ativo), options: EnumAtivoInativo.obterOpcoes(), required: true });
};

var CRUDRegraQualidadeMonitoramento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//#endregion Classes

// #region Funções de Inicialização
function loadRegrasQualidadeMonitoramento() {

    _pesquisaRegrasQualidadeMonitoramento = new PesquisaRegrasQualidadeMonitoramento();
    KoBindings(_pesquisaRegrasQualidadeMonitoramento, "knockoutPesquisaRegrasQualidadeMonitoramento", false, _pesquisaRegrasQualidadeMonitoramento.Pesquisar.id);

    _regraQualidadeMonitoramento = new RegraQualidadeMonitoramento();
    KoBindings(_regraQualidadeMonitoramento, "knockoutRegraQualidadeMonitoramento");

    _crudRegraQualidadeMonitoramento = new CRUDRegraQualidadeMonitoramento();
    KoBindings(_crudRegraQualidadeMonitoramento, "knockoutCRUDRegraQualidadeMonitoramento");

    loadGridRegrasQualidadeMonitoramento();
}

function loadGridRegrasQualidadeMonitoramento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPesquisaRegrasQualidadeMonitoramento = new GridView(_pesquisaRegrasQualidadeMonitoramento.Pesquisar.idGrid, "RegraQualidadeMonitoramento/Pesquisa", _pesquisaRegrasQualidadeMonitoramento, menuOpcoes, null);
    _gridPesquisaRegrasQualidadeMonitoramento.CarregarGrid();
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function adicionarClick() {

    if (!ValidarCamposObrigatorios(_regraQualidadeMonitoramento))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios!", "Informe os Campos Obrigatórios!");

    var regraSalvar = RetornarObjetoPesquisa(_regraQualidadeMonitoramento);
    executarReST("RegraQualidadeMonitoramento/Adicionar", regraSalvar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Regra adicionada com Sucesso!");
                recarregarRegraQualidadeMonitoramento();
                limparCamposRegraQualidadeMonitoramento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarClick() {
    if (!ValidarCamposObrigatorios(_regraQualidadeMonitoramento))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios!", "Informe os Campos Obrigatórios!");

    var regraSalvar = RetornarObjetoPesquisa(_regraQualidadeMonitoramento);
    executarReST("RegraQualidadeMonitoramento/Atualizar", regraSalvar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Regra atualizada com Sucesso!");
                recarregarRegraQualidadeMonitoramento();
                limparCamposRegraQualidadeMonitoramento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function editarClick(e) {
    limparCamposRegraQualidadeMonitoramento();
    _regraQualidadeMonitoramento.Codigo.val(e.Codigo);
    BuscarPorCodigo(_regraQualidadeMonitoramento, "RegraQualidadeMonitoramento/BuscarPorCodigo", function (arg) {
        _pesquisaRegrasQualidadeMonitoramento.ExibirFiltros.visibleFade(false);
        _crudRegraQualidadeMonitoramento.Atualizar.visible(true);
        _crudRegraQualidadeMonitoramento.Cancelar.visible(true);
        _crudRegraQualidadeMonitoramento.Adicionar.visible(false);

        PreencherObjetoKnout(_regraQualidadeMonitoramento, { Data: arg.Data });
    }, null);
}

function cancelarClick() {
    limparCamposRegraQualidadeMonitoramento();
}
// #endregion Funções Associadas a Eventos

// #region Funções Privadas
function recarregarRegraQualidadeMonitoramento() {
    _gridPesquisaRegrasQualidadeMonitoramento.CarregarGrid();
}

function limparCamposRegraQualidadeMonitoramento() {
    LimparCampos(_regraQualidadeMonitoramento);

    _crudRegraQualidadeMonitoramento.Atualizar.visible(false);
    _crudRegraQualidadeMonitoramento.Cancelar.visible(false);
    _crudRegraQualidadeMonitoramento.Adicionar.visible(true);
}
// #endregion Funções Privadas