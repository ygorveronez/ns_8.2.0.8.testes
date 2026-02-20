/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Enumeradores/EnumTipoCapacidadeCarregamentoPorPeso.js" />
/// <reference path="PeriodoCarregamento.js" />
/// <reference path="PrevisaoCarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDExcecaoCapacidadeCarregamento;
var _excecaoCapacidadeCarregamento;
var _excecaoPeriodoCarregamento;
var _excecaoPrevisaoCarregamento;
var _gridExcecaoCapacidadeCarregamento;
var _pesquisaExcecaoCapacidadeCarregamento;

/*
 * Declaração das Classes
 */

var CRUDExcecaoCapacidadeCarregamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var ExcecaoCapacidadeCarregamento = function () {
    var self = this;
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.CentroCarregamento.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, eventChange: centroCarregamentoBlur });
    this.Descricao = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), maxlength: 150, required: true, visible: ko.observable(true) });
    this.CapacidadeCarregamento = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.CapacidadedeCarregamentoKG.getFieldDescription(), required: false, visible: ko.observable(false) });
    this.TipoAbrangencia = PropertyEntity({ val: ko.observable(EnumTipoAbrangenciaExcecaoCapacidadeCarregamento.Dia), options: EnumTipoAbrangenciaExcecaoCapacidadeCarregamento.obterOpcoes(), def: EnumTipoAbrangenciaExcecaoCapacidadeCarregamento.Dia, text: Localization.Resources.Gerais.Geral.Tipo.getFieldDescription() });
    this.CapacidadeCarregamentoVolume = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.CapacidadeDeCarregamentoVolume.getRequiredFieldDescription(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(false) });
    this.CapacidadeCarregamentoCubagem = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.CapacidadeDeCarregamentoCubagem.getRequiredFieldDescription(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(false) });

    this.DataInicial = PropertyEntity({ val: ko.observable(""), def: "", text: ko.observable(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Data.getRequiredFieldDescription()), getType: typesKnockout.date, required: true, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.DataFinal.getRequiredFieldDescription(), getType: typesKnockout.date, required: function () { return self.DataFinal.visible() }, visible: ko.observable(false) });
    this.Segunda = PropertyEntity({ val: ko.observable(true), def: true, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Seg, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.Terca = PropertyEntity({ val: ko.observable(true), def: true, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Ter, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.Quarta = PropertyEntity({ val: ko.observable(true), def: true, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Qua, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.Quinta = PropertyEntity({ val: ko.observable(true), def: true, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Qui, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.Sexta = PropertyEntity({ val: ko.observable(true), def: true, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Sex, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.Sabado = PropertyEntity({ val: ko.observable(true), def: true, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Sab, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.Domingo = PropertyEntity({ val: ko.observable(true), def: true, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Dom, getType: typesKnockout.bool, visible: ko.observable(false) });

    this.PeriodosCarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });
    this.PrevisoesCarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });
}

var PesquisaExcecaoCapacidadeCarregamento = function () {
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.CentroCarregamento.getFieldDescription(), idBtnSearch: guid() });
    this.DataInicial = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.DataInicial.getFieldDescription(), getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.DataFinal.getFieldDescription(), getType: typesKnockout.date });

    this.DataInicial.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicial;

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: Localization.Resources.Gerais.Geral.FiltroPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridExcecaoCapacidadeCarregamento, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridExcecaoCapacidadeCarregamento() {
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var opcaoDuplicar = { descricao: Localization.Resources.Gerais.Geral.Duplicar, id: "clasDuplicar", evento: "onclick", metodo: duplicarClick, tamanho: "10", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        tamanho: "7",
        opcoes: [opcaoEditar, opcaoDuplicar]
    };
    var configuracoesExportacao = { url: "ExcecaoCapacidadeCarregamento/ExportarPesquisa", titulo: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.ExcecaoCapacidadedeCarregamento };

    _gridExcecaoCapacidadeCarregamento = new GridViewExportacao(_pesquisaExcecaoCapacidadeCarregamento.Pesquisar.idGrid, "ExcecaoCapacidadeCarregamento/Pesquisa", _pesquisaExcecaoCapacidadeCarregamento, menuOpcoes, configuracoesExportacao);
    _gridExcecaoCapacidadeCarregamento.CarregarGrid();
}

function loadExcecaoCapacidadeCarregamento() {
    _excecaoCapacidadeCarregamento = new ExcecaoCapacidadeCarregamento();
    KoBindings(_excecaoCapacidadeCarregamento, "knockoutExcecaoCapacidadeCarregamento");

    HeaderAuditoria("ExcecaoCapacidadeCarregamento", _excecaoCapacidadeCarregamento);

    _CRUDExcecaoCapacidadeCarregamento = new CRUDExcecaoCapacidadeCarregamento();
    KoBindings(_CRUDExcecaoCapacidadeCarregamento, "knockoutCRUDExcecaoCapacidadeCarregamento");

    _pesquisaExcecaoCapacidadeCarregamento = new PesquisaExcecaoCapacidadeCarregamento();
    KoBindings(_pesquisaExcecaoCapacidadeCarregamento, "knockoutPesquisaExcecaoCapacidadeCarregamento", false, _pesquisaExcecaoCapacidadeCarregamento.Pesquisar.id);

    _excecaoCapacidadeCarregamento.TipoAbrangencia.val.subscribe(tipoAbrangenciaChange);
    new BuscarCentrosCarregamento(_pesquisaExcecaoCapacidadeCarregamento.CentroCarregamento);
    new BuscarCentrosCarregamento(_excecaoCapacidadeCarregamento.CentroCarregamento, retornoConsultaCentroCarregamento);

    loadPeriodoCarregamento();
    loadPrevisaoCarregamento();
    loadGridExcecaoCapacidadeCarregamento();
}

function loadPeriodoCarregamento() {
    _excecaoPeriodoCarregamento = new PeriodoCarregamento("knockoutPeriodoCarregamento_Excecao", _excecaoCapacidadeCarregamento.PeriodosCarregamento);

    _excecaoPeriodoCarregamento.Load();
}

function loadPrevisaoCarregamento() {
    _excecaoPrevisaoCarregamento = new PrevisaoCarregamento("knockoutPrevisaoCarregamento_Excecao", _excecaoCapacidadeCarregamento.PrevisoesCarregamento);

    _excecaoPrevisaoCarregamento.Load();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    var chamadaApi = function () {
        Salvar(_excecaoCapacidadeCarregamento, "ExcecaoCapacidadeCarregamento/Adicionar", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Cadastrado com sucesso.");

                    recarregarGridExcecaoCapacidadeCarregamento();
                    limparCamposExcecaoCapacidadeCarregamento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, sender);
    }

    if (_excecaoPeriodoCarregamento.GetIsEditando())
        return exibirConfirmacao(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.PeriodoEditacao, "Um período está em edição, as alterações não salvas serão descartadas. Tem certeza que deseja continuar?", chamadaApi);

    chamadaApi();
}

function atualizarClick(e, sender) {
    var chamadaApi = function () {
        Salvar(_excecaoCapacidadeCarregamento, "ExcecaoCapacidadeCarregamento/Atualizar", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Atualizado com sucesso");

                    recarregarGridExcecaoCapacidadeCarregamento();
                    limparCamposExcecaoCapacidadeCarregamento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, sender);
    }

    if (_excecaoPeriodoCarregamento.GetIsEditando())
        return exibirConfirmacao(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.PeriodoEditacao, "Um período está em edição, as alterações não salvas serão descartadas. Tem certeza que deseja continuar?", chamadaApi);

    chamadaApi();
}

function cancelarClick() {
    limparCamposExcecaoCapacidadeCarregamento();
}

function centroCarregamentoBlur() {
    if (_excecaoCapacidadeCarregamento.CentroCarregamento.val() == "")
        controlarVisibilidadeCamposCapacidadeCarregamentoPorPeso(EnumTipoCapacidadeCarregamentoPorPeso.Todos);
}

function duplicarClick(registroSelecionado) {
    buscarExcecaoPorCodigo(registroSelecionado.Codigo, function () {
        _excecaoCapacidadeCarregamento.Codigo.val(_excecaoCapacidadeCarregamento.Codigo.def);
        controlarBotoesHabilitados();

        _excecaoCapacidadeCarregamento.PeriodosCarregamento.list = _excecaoCapacidadeCarregamento.PeriodosCarregamento.list.map(function (previsao) {
            previsao.Codigo.val = guid();
            return previsao;
        })
    });
}

function editarClick(registroSelecionado) {
    buscarExcecaoPorCodigo(registroSelecionado.Codigo);
}

function excluirClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_excecaoCapacidadeCarregamento, "ExcecaoCapacidadeCarregamento/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Excluído com sucesso");

                    recarregarGridExcecaoCapacidadeCarregamento();
                    limparCamposExcecaoCapacidadeCarregamento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Sugestao, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function tipoAbrangenciaChange() {
    if (_excecaoCapacidadeCarregamento.TipoAbrangencia.val() == EnumTipoAbrangenciaExcecaoCapacidadeCarregamento.Dia) {
        _excecaoCapacidadeCarregamento.Segunda.visible(false);
        _excecaoCapacidadeCarregamento.Terca.visible(false);
        _excecaoCapacidadeCarregamento.Quarta.visible(false);
        _excecaoCapacidadeCarregamento.Quinta.visible(false);
        _excecaoCapacidadeCarregamento.Sexta.visible(false);
        _excecaoCapacidadeCarregamento.Sabado.visible(false);
        _excecaoCapacidadeCarregamento.Domingo.visible(false);
        _excecaoCapacidadeCarregamento.DataInicial.text("*Data:");
        _excecaoCapacidadeCarregamento.DataFinal.visible(false);
    }
    else {
        _excecaoCapacidadeCarregamento.Segunda.visible(true);
        _excecaoCapacidadeCarregamento.Terca.visible(true);
        _excecaoCapacidadeCarregamento.Quarta.visible(true);
        _excecaoCapacidadeCarregamento.Quinta.visible(true);
        _excecaoCapacidadeCarregamento.Sexta.visible(true);
        _excecaoCapacidadeCarregamento.Sabado.visible(true);
        _excecaoCapacidadeCarregamento.Domingo.visible(true);
        _excecaoCapacidadeCarregamento.DataInicial.text("*Data Inicial:");
        _excecaoCapacidadeCarregamento.DataFinal.visible(true);
    }
}

/*
 * Declaração das Funções
 */

function buscarExcecaoPorCodigo(codigo, callback) {
    limparCamposExcecaoCapacidadeCarregamento();

    _excecaoCapacidadeCarregamento.Codigo.val(codigo);

    BuscarPorCodigo(_excecaoCapacidadeCarregamento, "ExcecaoCapacidadeCarregamento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaExcecaoCapacidadeCarregamento.ExibirFiltros.visibleFade(false);

                controlarBotoesHabilitados();

                retornoConsultaCentroCarregamento(retorno.Data.CentroCarregamento);
                _excecaoPeriodoCarregamento.RecarregarGrid();
                _excecaoPrevisaoCarregamento.RecarregarGrid();

                if (callback instanceof Function)
                    callback(retorno.Data);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function controlarBotoesHabilitados() {
    var isEdicao = _excecaoCapacidadeCarregamento.Codigo.val() > 0;

    _CRUDExcecaoCapacidadeCarregamento.Atualizar.visible(isEdicao);
    _CRUDExcecaoCapacidadeCarregamento.Excluir.visible(isEdicao);
    _CRUDExcecaoCapacidadeCarregamento.Adicionar.visible(!isEdicao);
}

function controlarVisibilidadeCamposCapacidadeCarregamentoPorPeso(tipoCapacidadeCarregamentoPorPeso) {
    var exibirCapacidadeCarregamentoPorDiaSemana = (tipoCapacidadeCarregamentoPorPeso == EnumTipoCapacidadeCarregamentoPorPeso.DiaSemana);
    var exibirCapacidadeCarregamentoPorPeriodoCarregamento = (tipoCapacidadeCarregamentoPorPeso == EnumTipoCapacidadeCarregamentoPorPeso.PeriodoCarregamento);

    _excecaoCapacidadeCarregamento.CapacidadeCarregamento.visible(exibirCapacidadeCarregamentoPorDiaSemana);
    _excecaoPeriodoCarregamento.ControlarExibicaoCapacidadeCarregamento(exibirCapacidadeCarregamentoPorPeriodoCarregamento);
}

function limparCamposExcecaoCapacidadeCarregamento() {
    LimparCampos(_excecaoCapacidadeCarregamento);

    $("#tab-capacidade-carregamento a:first").tab("show");

    controlarBotoesHabilitados();
    controlarVisibilidadeCamposCapacidadeCarregamentoPorPeso(EnumTipoCapacidadeCarregamentoPorPeso.Todos);

    _excecaoPeriodoCarregamento.LimparCampos();
    _excecaoPeriodoCarregamento.RecarregarGrid();
    _excecaoPrevisaoCarregamento.RecarregarGrid();
}

function recarregarGridExcecaoCapacidadeCarregamento() {
    _gridExcecaoCapacidadeCarregamento.CarregarGrid();
}

function retornoConsultaCentroCarregamento(registroSelecionado) {
    _excecaoCapacidadeCarregamento.CentroCarregamento.codEntity(registroSelecionado.Codigo);
    _excecaoCapacidadeCarregamento.CentroCarregamento.entityDescription(registroSelecionado.Descricao);
    _excecaoCapacidadeCarregamento.CentroCarregamento.val(registroSelecionado.Descricao);

    _excecaoCapacidadeCarregamento.CapacidadeCarregamentoVolume.required(registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.Volume || registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.CubagemVolume);
    _excecaoCapacidadeCarregamento.CapacidadeCarregamentoVolume.visible(registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.Volume || registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.CubagemVolume);
    _excecaoCapacidadeCarregamento.CapacidadeCarregamentoVolume.enable(registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.Volume || registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.CubagemVolume);
    _excecaoCapacidadeCarregamento.CapacidadeCarregamentoCubagem.required(registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.CubagemVolume);
    _excecaoCapacidadeCarregamento.CapacidadeCarregamentoCubagem.visible(registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.CubagemVolume);
    _excecaoCapacidadeCarregamento.CapacidadeCarregamentoCubagem.enable(registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.CubagemVolume);

    controlarVisibilidadeCamposCapacidadeCarregamentoPorPeso(registroSelecionado.TipoCapacidadeCarregamentoPorPeso);
}