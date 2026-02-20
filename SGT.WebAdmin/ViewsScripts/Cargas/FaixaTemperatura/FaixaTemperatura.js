/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../Consultas/Cliente.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDFaixaTemperatura;
var _faixaTemperatura;
var _gridFaixaTemperatura;
var _pesquisaFaixaTemperatura;

/*
 * Declaração das Classes
 */

var CRUDFaixaTemperatura = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var FaixaTemperatura = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.FaixaFinal = PropertyEntity({ getType: typesKnockout.decimal, text: "*Temperatura Final:", required: true, maxlength: 10, configDecimal: { precision: 2, allowZero: true, allowNegative: true } });
    this.FaixaInicial = PropertyEntity({ getType: typesKnockout.decimal, text: "*Temperatura Inicial:", required: true, maxlength: 10, configDecimal: { precision: 2, allowZero: true, allowNegative: true } });
    this.Carimbo = PropertyEntity({ text: "Carimbo:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CarimboDescricao = PropertyEntity({ text: "Descrição do Carimbo:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", getType: typesKnockout.string, val: ko.observable("") });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", issue: 121, idBtnSearch: guid(), visible: ko.observable(true) });
    this.ProcedimentoEmbarque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Procedimento de Embarque:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true) });
}

var PesquisaFaixaTemperatura = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração: ", col: 3, getType: typesKnockout.int });
    this.ProcedimentoEmbarque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Procedimento de Embarque:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridFaixaTemperatura, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridFaixaTemperatura() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "FaixaTemperatura/ExportarPesquisa", titulo: "Motivo Seleção de Motorista Fora da Ordem" };

    _gridFaixaTemperatura = new GridViewExportacao(_pesquisaFaixaTemperatura.Pesquisar.idGrid, "FaixaTemperatura/Pesquisa", _pesquisaFaixaTemperatura, menuOpcoes, configuracoesExportacao);
    _gridFaixaTemperatura.CarregarGrid();
}

function loadFaixaTemperatura() {
    loadFaixaTemperaturaMensagemValidacao();
    
    _faixaTemperatura = new FaixaTemperatura();
    KoBindings(_faixaTemperatura, "knockoutFaixaTemperatura");

    HeaderAuditoria("FaixaTemperatura", _faixaTemperatura);

    _CRUDFaixaTemperatura = new CRUDFaixaTemperatura();
    KoBindings(_CRUDFaixaTemperatura, "knockoutCRUDFaixaTemperatura");

    _pesquisaFaixaTemperatura = new PesquisaFaixaTemperatura();
    KoBindings(_pesquisaFaixaTemperatura, "knockoutPesquisaFaixaTemperatura", false, _pesquisaFaixaTemperatura.Pesquisar.id);

    new BuscarTiposOperacao(_faixaTemperatura.TipoOperacao);
    new BuscarProcedimentoEmbarque(_faixaTemperatura.ProcedimentoEmbarque);
    new BuscarProcedimentoEmbarque(_pesquisaFaixaTemperatura.ProcedimentoEmbarque);
    new BuscarClientes(_faixaTemperatura.Remetente);

    setarVisibilidadeMensagens();
    loadGridFaixaTemperatura();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    $.extend(_faixaTemperatura, _faixaTemperaturaMensagemValidacao);

    Salvar(_faixaTemperatura, "FaixaTemperatura/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridFaixaTemperatura();
                limparCamposFaixaTemperatura();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    $.extend(_faixaTemperatura, _faixaTemperaturaMensagemValidacao);

    Salvar(_faixaTemperatura, "FaixaTemperatura/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridFaixaTemperatura();
                limparCamposFaixaTemperatura();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposFaixaTemperatura();
}

function editarClick(registroSelecionado) {
    limparCamposFaixaTemperatura();

    _faixaTemperatura.Codigo.val(registroSelecionado.Codigo);

    executarReST("FaixaTemperatura/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaFaixaTemperatura.ExibirFiltros.visibleFade(false);
                PreencherObjetoKnout(_faixaTemperatura, { Data: retorno.Data.FaixaTemperatura });
                PreencherObjetoKnout(_faixaTemperaturaMensagemValidacao, { Data: retorno.Data.FaixaTemperaturaMensagemValidacao });
                controlarBotoesHabilitados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_faixaTemperatura, "FaixaTemperatura/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridFaixaTemperatura();
                    limparCamposFaixaTemperatura();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados() {
    var isEdicao = _faixaTemperatura.Codigo.val() > 0;

    _CRUDFaixaTemperatura.Atualizar.visible(isEdicao);
    _CRUDFaixaTemperatura.Excluir.visible(isEdicao);
    _CRUDFaixaTemperatura.Cancelar.visible(isEdicao);
    _CRUDFaixaTemperatura.Adicionar.visible(!isEdicao);
}

function limparCamposFaixaTemperatura() {
    controlarBotoesHabilitados();
    LimparCampos(_faixaTemperatura);
    LimparCampos(_faixaTemperaturaMensagemValidacao);
    Global.ResetarAbas();
}

function recarregarGridFaixaTemperatura() {
    _gridFaixaTemperatura.CarregarGrid();
}

function obterTiposIntegracao() {
    var p = new promise.Promise();
    var _tiposIntegracao = new Array();

    executarReST("TipoIntegracao/BuscarTodos", {
        Tipos: JSON.stringify([
            EnumTipoIntegracao.NaoPossuiIntegracao,
            EnumTipoIntegracao.Marfrig,
            EnumTipoIntegracao.Minerva,
            EnumTipoIntegracao.InteliPost,
            EnumTipoIntegracao.Magalog,
            EnumTipoIntegracao.Boticario,
            EnumTipoIntegracao.Riachuelo
        ])
    }, function (r) {
        if (r.Success) {
            for (var i = 0; i < r.Data.length; i++)
                _tiposIntegracao.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }

        p.done(_tiposIntegracao);
    });

    return p;
}

function setarVisibilidadeMensagens() {
    obterTiposIntegracao().then(function (listaTiposIntegracao) {
        for (var i = 0; i < listaTiposIntegracao.length; i++) {
            if (listaTiposIntegracao[i].value == EnumTipoIntegracao.Marfrig) {
                $("#liTabMensagensValidacao").show();
            }
        }
    });
}