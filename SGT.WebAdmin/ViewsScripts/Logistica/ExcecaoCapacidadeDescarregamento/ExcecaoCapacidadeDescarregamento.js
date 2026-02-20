/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CentrosDescarregamento.js" />
/// <reference path="../../Enumeradores/EnumTipoCapacidadeDescarregamentoPorPeso.js" />
/// <reference path="PeriodoDescarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDExcecaoCapacidadeDescarregamento;
var _excecaoCapacidadeDescarregamento;
var _excecaoPeriodoDescarregamento;
var _quantidadePorTipoDeCarga;
var _gridExcecaoCapacidadeDescarregamento;
var _pesquisaExcecaoCapacidadeDescarregamento;

/*
 * Declaração das Classes
 */

var CRUDExcecaoCapacidadeDescarregamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar + " / " + Localization.Resources.Gerais.Geral.Novo, visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",

        UrlImportacao: "ExcecaoCapacidadeDescarregamento/Importar",
        UrlConfiguracao: "ExcecaoCapacidadeDescarregamento/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O068_ExcecaoCapacidadeDescarregamento,
        CallbackImportacao: function () {
            _gridExcecaoCapacidadeDescarregamento.CarregarGrid();
        }
    });
}

var ExcecaoCapacidadeDescarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CentroDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.CentroDescarregamento.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, eventChange: centroDescarregamentoBlur });
    this.Descricao = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), maxlength: 150, required: true, visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.DataInicial.getRequiredFieldDescription(), getType: typesKnockout.date, required: true, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.DataFinal.getRequiredFieldDescription(), getType: typesKnockout.date, required: true, visible: ko.observable(true) });
    
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.CapacidadeDescarregamento = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.CapacidadeDescarregamentoKG.getFieldDescription(), required: false, visible: ko.observable(false) });

    this.PeriodosDescarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });
    this.QuantidadesPorTipoDeCargaDescarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });
    this.PrevisoesDescarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });
}

var PesquisaExcecaoCapacidadeDescarregamento = function () {
    this.CentroDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.CentroDescarregamento.getFieldDescription(), idBtnSearch: guid() });
    this.DataInicial = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.DataInicial.getFieldDescription(), getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.DataLimite.getFieldDescription(), getType: typesKnockout.date });

    this.DataInicial.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicial;

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: Localization.Resources.Gerais.Geral.FiltroPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridExcecaoCapacidadeDescarregamento, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridExcecaoCapacidadeDescarregamento() {
    const opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    const configuracoesExportacao = { url: "ExcecaoCapacidadeDescarregamento/ExportarPesquisa", titulo: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.ExcecaoCapacidadeDescarregamento_ };

    _gridExcecaoCapacidadeDescarregamento = new GridViewExportacao(_pesquisaExcecaoCapacidadeDescarregamento.Pesquisar.idGrid, "ExcecaoCapacidadeDescarregamento/Pesquisa", _pesquisaExcecaoCapacidadeDescarregamento, menuOpcoes, configuracoesExportacao);
    _gridExcecaoCapacidadeDescarregamento.CarregarGrid();
}

function loadExcecaoCapacidadeDescarregamento() {
    _excecaoCapacidadeDescarregamento = new ExcecaoCapacidadeDescarregamento();
    KoBindings(_excecaoCapacidadeDescarregamento, "knockoutExcecaoCapacidadeDescarregamento");

    HeaderAuditoria("ExcecaoCapacidadeDescarregamento", _excecaoCapacidadeDescarregamento);

    _CRUDExcecaoCapacidadeDescarregamento = new CRUDExcecaoCapacidadeDescarregamento();
    KoBindings(_CRUDExcecaoCapacidadeDescarregamento, "knockoutCRUDExcecaoCapacidadeDescarregamento");

    _pesquisaExcecaoCapacidadeDescarregamento = new PesquisaExcecaoCapacidadeDescarregamento();
    KoBindings(_pesquisaExcecaoCapacidadeDescarregamento, "knockoutPesquisaExcecaoCapacidadeDescarregamento", false, _pesquisaExcecaoCapacidadeDescarregamento.Pesquisar.id);

    BuscarCentrosDescarregamento(_pesquisaExcecaoCapacidadeDescarregamento.CentroDescarregamento);
    BuscarCentrosDescarregamento(_excecaoCapacidadeDescarregamento.CentroDescarregamento, retornoConsultaCentroDescarregamento);

    loadPeriodoDescarregamento();
    loadQuantidadePorTipoDeCarga();
    loadGridExcecaoCapacidadeDescarregamento();
}

function loadPeriodoDescarregamento() {
    _excecaoPeriodoDescarregamento = new PeriodoDescarregamento("knockoutPeriodoDescarregamento_Excecao", _excecaoCapacidadeDescarregamento.PeriodosDescarregamento);
    _excecaoPeriodoDescarregamento.Load();
}

function loadQuantidadePorTipoDeCarga() {
    _quantidadePorTipoDeCarga = new QuantidadePorDeTipoCarga("knockoutQuantidadeTipoDeCarga_Excecao", _excecaoCapacidadeDescarregamento.QuantidadesPorTipoDeCargaDescarregamento);
    _quantidadePorTipoDeCarga.Load();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    const chamadaApi = function () {
        Salvar(_excecaoCapacidadeDescarregamento, "ExcecaoCapacidadeDescarregamento/Adicionar", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);

                    recarregarGridExcecaoCapacidadeDescarregamento();
                    limparCamposExcecaoCapacidadeDescarregamento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, sender);
    }

    if (_excecaoPeriodoDescarregamento.GetIsEditando())
        return exibirConfirmacao(Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.PeriodoEditacao, Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.UmPeriodoEstaEmEdicao, chamadaApi);

    chamadaApi();
}

function atualizarClick(e, sender) {
    const chamadaApi = function () {
        Salvar(_excecaoCapacidadeDescarregamento, "ExcecaoCapacidadeDescarregamento/Atualizar", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);

                    recarregarGridExcecaoCapacidadeDescarregamento();
                    limparCamposExcecaoCapacidadeDescarregamento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, sender);
    }

    if (_excecaoPeriodoDescarregamento.GetIsEditando())
        return exibirConfirmacao(Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.PeriodoEditacao, Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.UmPeriodoEstaEmEdicao, chamadaApi);

    chamadaApi();
}

function cancelarClick() {
    limparCamposExcecaoCapacidadeDescarregamento();
}

function centroDescarregamentoBlur() {
    if (_excecaoCapacidadeDescarregamento.CentroDescarregamento.val() == "")
        controlarVisibilidadeCamposCapacidadeDescarregamentoPorPeso(EnumTipoCapacidadeDescarregamentoPorPeso.Todos);
}

function editarClick(registroSelecionado) {
    limparCamposExcecaoCapacidadeDescarregamento();

    _excecaoCapacidadeDescarregamento.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_excecaoCapacidadeDescarregamento, "ExcecaoCapacidadeDescarregamento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaExcecaoCapacidadeDescarregamento.ExibirFiltros.visibleFade(false);

                controlarBotoesHabilitados();
                controlarVisibilidadeCamposCapacidadeDescarregamentoPorPeso(retorno.Data.CentroDescarregamento.TipoCapacidadeDescarregamentoPorPeso);
                _excecaoPeriodoDescarregamento.CarregarGrid();
                _quantidadePorTipoDeCarga.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.UmPeriodoEstaEmEdicao, function () {
        ExcluirPorCodigo(_excecaoCapacidadeDescarregamento, "ExcecaoCapacidadeDescarregamento/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);

                    recarregarGridExcecaoCapacidadeDescarregamento();
                    limparCamposExcecaoCapacidadeDescarregamento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
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
    const isEdicao = _excecaoCapacidadeDescarregamento.Codigo.val() > 0;

    _CRUDExcecaoCapacidadeDescarregamento.Atualizar.visible(isEdicao);
    _CRUDExcecaoCapacidadeDescarregamento.Excluir.visible(isEdicao);
    _CRUDExcecaoCapacidadeDescarregamento.Adicionar.visible(!isEdicao);
}

function controlarVisibilidadeCamposCapacidadeDescarregamentoPorPeso(tipoCapacidadeDescarregamentoPorPeso) {
    const exibirCapacidadeDescarregamentoPorDiaSemana = (tipoCapacidadeDescarregamentoPorPeso == EnumTipoCapacidadeDescarregamentoPorPeso.DiaSemana);
    const exibirCapacidadeDescarregamentoPorPeriodoDescarregamento = (tipoCapacidadeDescarregamentoPorPeso == EnumTipoCapacidadeDescarregamentoPorPeso.PeriodoDescarregamento);

    _excecaoCapacidadeDescarregamento.CapacidadeDescarregamento.visible(exibirCapacidadeDescarregamentoPorDiaSemana);
    _excecaoPeriodoDescarregamento.ControlarExibicaoCapacidadeDescarregamento(exibirCapacidadeDescarregamentoPorPeriodoDescarregamento);
}

function limparCamposExcecaoCapacidadeDescarregamento() {
    LimparCampos(_excecaoCapacidadeDescarregamento);
    _excecaoPeriodoDescarregamento.LimparCampos();
    $("#tab-capacidade-descarregamento a:first").tab("show");

    controlarBotoesHabilitados();
    controlarVisibilidadeCamposCapacidadeDescarregamentoPorPeso(EnumTipoCapacidadeDescarregamentoPorPeso.Todos);

    _excecaoPeriodoDescarregamento.CarregarGrid();
    _quantidadePorTipoDeCarga.CarregarGrid();
}

function recarregarGridExcecaoCapacidadeDescarregamento() {
    _gridExcecaoCapacidadeDescarregamento.CarregarGrid();
}

function retornoConsultaCentroDescarregamento(registroSelecionado) {
    _excecaoCapacidadeDescarregamento.CentroDescarregamento.codEntity(registroSelecionado.Codigo);
    _excecaoCapacidadeDescarregamento.CentroDescarregamento.entityDescription(registroSelecionado.Descricao);
    _excecaoCapacidadeDescarregamento.CentroDescarregamento.val(registroSelecionado.Descricao);

    controlarVisibilidadeCamposCapacidadeDescarregamentoPorPeso(registroSelecionado.TipoCapacidadeDescarregamentoPorPeso);
}
