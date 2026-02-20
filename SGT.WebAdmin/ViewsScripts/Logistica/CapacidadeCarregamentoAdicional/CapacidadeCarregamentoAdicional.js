/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Consultas/PeriodoCarregamento.js" />
/// <reference path="../../Enumeradores/EnumTipoCapacidadeCarregamentoPorPeso.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _capacidadeCarregamentoAdicional;
var _CRUDCapacidadeCarregamentoAdicional;
var _pesquisaCapacidadeCarregamentoAdicional;
var _gridCapacidadeCarregamentoAdicional;

/*
 * Declaração das Classes
 */

var CRUDCapacidadeCarregamentoAdicional = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar" });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var CapacidadeCarregamentoAdicional = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CapacidadeCarregamento = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "*Capacidade de Carregamento (KG):", required: true, enable: ko.observable(true) });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Carregamento:", idBtnSearch: guid(), required: true, enable: ko.observable(true), eventChange: centroCarregamentoBlur });
    this.Data = PropertyEntity({ val: ko.observable(""), def: "", text: "*Data:", getType: typesKnockout.date, required: true, enable: ko.observable(true) });
    this.HoraInicio = PropertyEntity({ text: "Hora Início:", getType: typesKnockout.time, visible: false });
    this.HoraTermino = PropertyEntity({ text: "Hora Término:", getType: typesKnockout.time, visible: false });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000, enable: ko.observable(true) });
    this.PeriodoCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Período de Carregamento:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true), eventChange: periodoCarregamentoBlur });
    this.PermitirEdicao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.CapacidadeCarregamentoVolume = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "*Capacidade de Carregamento (Volume):", required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(false) });
    this.CapacidadeCarregamentoCubagem = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "*Capacidade de Carregamento (Cubagem):", required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(false) });

    this.Data.val.subscribe(limparCamposPeriodoCarregamento);
}

var PesquisaCapacidadeCarregamentoAdicional = function () {
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", idBtnSearch: guid() });
    this.DataInicial = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Inicial:", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Final:", getType: typesKnockout.date });

    this.DataInicial.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicial;

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridCapacidadeCarregamentoAdicional, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridCapacidadeCarregamentoAdicional() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "CapacidadeCarregamentoAdicional/ExportarPesquisa", titulo: "Capacidade de Carregamento Adicional" };

    _gridCapacidadeCarregamentoAdicional = new GridViewExportacao(_pesquisaCapacidadeCarregamentoAdicional.Pesquisar.idGrid, "CapacidadeCarregamentoAdicional/Pesquisa", _pesquisaCapacidadeCarregamentoAdicional, menuOpcoes, configuracoesExportacao);
    _gridCapacidadeCarregamentoAdicional.CarregarGrid();
}

function loadCapacidadeCarregamentoAdicional() {
    _capacidadeCarregamentoAdicional = new CapacidadeCarregamentoAdicional();
    KoBindings(_capacidadeCarregamentoAdicional, "knockoutCapacidadeCarregamentoAdicional");

    HeaderAuditoria("CapacidadeCarregamentoAdicional", _capacidadeCarregamentoAdicional);

    _CRUDCapacidadeCarregamentoAdicional = new CRUDCapacidadeCarregamentoAdicional();
    KoBindings(_CRUDCapacidadeCarregamentoAdicional, "knockoutCRUDCapacidadeCarregamentoAdicional");

    _pesquisaCapacidadeCarregamentoAdicional = new PesquisaCapacidadeCarregamentoAdicional();
    KoBindings(_pesquisaCapacidadeCarregamentoAdicional, "knockoutPesquisaCapacidadeCarregamentoAdicional", false, _pesquisaCapacidadeCarregamentoAdicional.Pesquisar.id);

    new BuscarCentrosCarregamento(_pesquisaCapacidadeCarregamentoAdicional.CentroCarregamento);
    new BuscarCentrosCarregamento(_capacidadeCarregamentoAdicional.CentroCarregamento, retornoConsultaCentroCarregamento);
    new BuscarPeriodoCarregamento(_capacidadeCarregamentoAdicional.PeriodoCarregamento, retornoConsultaPeriodoCarregamento, _capacidadeCarregamentoAdicional.Data, _capacidadeCarregamentoAdicional.CentroCarregamento);

    loadGridCapacidadeCarregamentoAdicional();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_capacidadeCarregamentoAdicional, "CapacidadeCarregamentoAdicional/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridCapacidadeCarregamentoAdicional();
                limparCamposCapacidadeCarregamentoAdicional();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_capacidadeCarregamentoAdicional, "CapacidadeCarregamentoAdicional/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridCapacidadeCarregamentoAdicional();
                limparCamposCapacidadeCarregamentoAdicional();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposCapacidadeCarregamentoAdicional();
}

function centroCarregamentoBlur() {
    if (_capacidadeCarregamentoAdicional.CentroCarregamento.val() == "") {
        controlarVisibilidadeCampoPeriodoCarregamento(false);
        limparCamposPeriodoCarregamento();
    }
}

function editarClick(registroSelecionado) {
    limparCamposCapacidadeCarregamentoAdicional();

    _capacidadeCarregamentoAdicional.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_capacidadeCarregamentoAdicional, "CapacidadeCarregamentoAdicional/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaCapacidadeCarregamentoAdicional.ExibirFiltros.visibleFade(false);

                controlarComponentesHabilitados();

                retornoConsultaCentroCarregamento(retorno.Data.CentroCarregamento);
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
        ExcluirPorCodigo(_capacidadeCarregamentoAdicional, "CapacidadeCarregamentoAdicional/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridCapacidadeCarregamentoAdicional();
                    limparCamposCapacidadeCarregamentoAdicional();
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

function periodoCarregamentoBlur() {
    if (_capacidadeCarregamentoAdicional.PeriodoCarregamento.val() == "") {
        _capacidadeCarregamentoAdicional.HoraInicio.val("");
        _capacidadeCarregamentoAdicional.HoraTermino.val("");
    }
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados() {
    var isEdicao = _capacidadeCarregamentoAdicional.Codigo.val() > 0;
    var edicaoHabilitada = (isEdicao && _capacidadeCarregamentoAdicional.PermitirEdicao.val());

    _CRUDCapacidadeCarregamentoAdicional.Atualizar.visible(edicaoHabilitada);
    _CRUDCapacidadeCarregamentoAdicional.Excluir.visible(edicaoHabilitada);
    _CRUDCapacidadeCarregamentoAdicional.Adicionar.visible(!isEdicao);
}

function controlarVisibilidadeCampoPeriodoCarregamento(exibir) {
    _capacidadeCarregamentoAdicional.PeriodoCarregamento.required(exibir);
    _capacidadeCarregamentoAdicional.PeriodoCarregamento.visible(exibir);
}

function controlarCamposHabilitados() {
    var edicaoHabilitada = _capacidadeCarregamentoAdicional.PermitirEdicao.val();

    _capacidadeCarregamentoAdicional.CapacidadeCarregamento.enable(edicaoHabilitada);
    _capacidadeCarregamentoAdicional.CentroCarregamento.enable(edicaoHabilitada);
    _capacidadeCarregamentoAdicional.Data.enable(edicaoHabilitada);
    _capacidadeCarregamentoAdicional.PeriodoCarregamento.enable(edicaoHabilitada);
    _capacidadeCarregamentoAdicional.Observacao.enable(edicaoHabilitada);
}

function controlarComponentesHabilitados() {
    controlarBotoesHabilitados();
    controlarCamposHabilitados();
}

function limparCamposCapacidadeCarregamentoAdicional() {
    LimparCampos(_capacidadeCarregamentoAdicional);
    controlarComponentesHabilitados();
    controlarVisibilidadeCampoPeriodoCarregamento(false);
}

function limparCamposPeriodoCarregamento() {
    _capacidadeCarregamentoAdicional.PeriodoCarregamento.codEntity(0);
    _capacidadeCarregamentoAdicional.PeriodoCarregamento.val("");
    _capacidadeCarregamentoAdicional.PeriodoCarregamento.entityDescription("");
    _capacidadeCarregamentoAdicional.HoraInicio.val("");
    _capacidadeCarregamentoAdicional.HoraTermino.val("");
}

function recarregarGridCapacidadeCarregamentoAdicional() {
    _gridCapacidadeCarregamentoAdicional.CarregarGrid();
}

function retornoConsultaCentroCarregamento(registroSelecionado) {
    _capacidadeCarregamentoAdicional.CentroCarregamento.codEntity(registroSelecionado.Codigo);
    _capacidadeCarregamentoAdicional.CentroCarregamento.val(registroSelecionado.Descricao);
    _capacidadeCarregamentoAdicional.CentroCarregamento.entityDescription(registroSelecionado.Descricao);

    _capacidadeCarregamentoAdicional.CapacidadeCarregamentoVolume.required(registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.Volume || registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.CubagemVolume);
    _capacidadeCarregamentoAdicional.CapacidadeCarregamentoVolume.visible(registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.Volume || registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.CubagemVolume);
    _capacidadeCarregamentoAdicional.CapacidadeCarregamentoVolume.enable(registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.Volume || registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.CubagemVolume);
    _capacidadeCarregamentoAdicional.CapacidadeCarregamentoCubagem.required(registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.CubagemVolume);
    _capacidadeCarregamentoAdicional.CapacidadeCarregamentoCubagem.visible(registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.CubagemVolume);
    _capacidadeCarregamentoAdicional.CapacidadeCarregamentoCubagem.enable(registroSelecionado.TipoCapacidadeCarregamento == EnumTipoCapacidadeCarregamento.CubagemVolume);


    controlarVisibilidadeCampoPeriodoCarregamento(registroSelecionado.TipoCapacidadeCarregamentoPorPeso == EnumTipoCapacidadeCarregamentoPorPeso.PeriodoCarregamento);
    limparCamposPeriodoCarregamento();
}

function retornoConsultaPeriodoCarregamento(registroSelecionado) {
    _capacidadeCarregamentoAdicional.PeriodoCarregamento.codEntity(registroSelecionado.Codigo);
    _capacidadeCarregamentoAdicional.PeriodoCarregamento.val(registroSelecionado.Descricao);
    _capacidadeCarregamentoAdicional.PeriodoCarregamento.entityDescription(registroSelecionado.Descricao);
    _capacidadeCarregamentoAdicional.HoraInicio.val(registroSelecionado.HoraInicio);
    _capacidadeCarregamentoAdicional.HoraTermino.val(registroSelecionado.HoraTermino);
}
