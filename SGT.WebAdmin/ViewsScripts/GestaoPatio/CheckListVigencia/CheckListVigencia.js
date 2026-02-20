/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Globais.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../wwwroot/js/Global/Validacao.js" />

// #region Objetos Globais do Arquivo

var _checkListVigencia;
var _pesquisaCheckListVigencia;
var _crudCheckListVigencia;
var _gridCheckListVigencia;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CheckListVigencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(""), options: Global.ObterOpcoesBooleano("Ativo", "Inativo"), def: "", required: ko.observable(true) });
    this.DataFimVigencia = PropertyEntity({ text: "*Data Fim Vigência:", getType: typesKnockout.date, enable: ko.observable(true), required: ko.observable(true) });
};

var PesquisaCheckListVigencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ text: "Situação", val: ko.observable(EnumAtivoInativo.Ativo), options: EnumAtivoInativo.obterOpcoesPesquisa(), def: EnumAtivoInativo.Ativo, visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(Global.PrimeiraDataDoMesAtual()) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCheckList.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var CRUDCheckListVigencia = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadVigenciaCheckList() {
    _pesquisaCheckListVigencia = new PesquisaCheckListVigencia();
    KoBindings(_pesquisaCheckListVigencia, "knockoutPesquisaVigenciaCheckList");

    _checkListVigencia = new CheckListVigencia();
    KoBindings(_checkListVigencia, "knockoutCheckListVigencia");

    _crudCheckListVigencia = new CRUDCheckListVigencia();
    KoBindings(_crudCheckListVigencia, "knockoutCRUDCheckListVigencia");

    new BuscarFilial(_pesquisaCheckListVigencia.Filial);
    new BuscarTiposOperacao(_pesquisaCheckListVigencia.TipoOperacao);

    new BuscarFilial(_checkListVigencia.Filial);
    new BuscarTiposOperacao(_checkListVigencia.TipoOperacao);

    loadGridCheckList();
}

function loadGridCheckList() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridCheckList = new GridView(_pesquisaCheckListVigencia.Pesquisar.idGrid, "CheckListVigencia/Pesquisa", _pesquisaCheckListVigencia, menuOpcoes, null);
    _gridCheckList.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick() {
    if (!ValidarCamposObrigatorios(_checkListVigencia))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios!", "Informe os Campos Obrigatórios!");

    executarReST("CheckListVigencia/Adicionar", RetornarObjetoPesquisa(_checkListVigencia), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _gridCheckList.CarregarGrid();
                limparCamposCheckListVigencia();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com Sucesso!");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarClick() {

    if (!ValidarCamposObrigatorios(_checkListVigencia))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios!", "Informe os Campos Obrigatórios!");

    executarReST("CheckListVigencia/Atualizar", RetornarObjetoPesquisa(_checkListVigencia), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _gridCheckList.CarregarGrid();
                limparCamposCheckListVigencia();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Vigência de CheckList selecionada?", function () {
        ExcluirPorCodigo(_checkListVigencia, "CheckListVigencia/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridCheckList.CarregarGrid();
                    limparCamposCheckListVigencia();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
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

function editarClick(registroSelecionado) {
    limparCamposCheckListVigencia();

    _checkListVigencia.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_checkListVigencia, "CheckListVigencia/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaCheckListVigencia.ExibirFiltros.visibleFade(false);
                SetarEnableCamposKnockout(_checkListVigencia, false);
                controlarBotoesHabilitados(true);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function cancelarClick() {
    limparCamposCheckListVigencia();
}
// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function limparCamposCheckListVigencia() {
    SetarEnableCamposKnockout(_checkListVigencia, true);
    LimparCampos(_checkListVigencia);
    controlarBotoesHabilitados(false);
}

function controlarBotoesHabilitados(isEdicao) {
    _crudCheckListVigencia.Atualizar.visible(isEdicao);
    _crudCheckListVigencia.Excluir.visible(isEdicao);
    _crudCheckListVigencia.Cancelar.visible(isEdicao);
    _crudCheckListVigencia.Adicionar.visible(!isEdicao);
}

// #endregion Funções Privadas
