/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Enumeradores/EnumTipoCapacidadeCarregamentoPorPeso.js" />
/// <reference path="PeriodoCarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDExclusividadeCarregamento;
var _exclusividadeCarregamento;
var _excecaoPeriodoCarregamento;
var _gridExclusividadeCarregamento;
var _pesquisaExclusividadeCarregamento;

/*
 * Declaração das Classes
 */

var CRUDExclusividadeCarregamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo", visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var ExclusividadeCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Carregamento:", idBtnSearch: guid(), required: true });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador", idBtnSearch: guid() });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid() });
    this.Descricao = PropertyEntity({ val: ko.observable(""), def: "", text: "*Descrição:", maxlength: 150, required: true, visible: ko.observable(true) });

    this.DataInicial = PropertyEntity({ val: ko.observable(""), def: "", text: "*Data Inicial:", getType: typesKnockout.date, required: true });
    this.DataFinal = PropertyEntity({ val: ko.observable(""), def: "", text: "*Data Final:", getType: typesKnockout.date, required: true });
    this.Segunda = PropertyEntity({ val: ko.observable(false), def: false, text: "Seg", getType: typesKnockout.bool });
    this.Terca = PropertyEntity({ val: ko.observable(false), def: false, text: "Ter", getType: typesKnockout.bool });
    this.Quarta = PropertyEntity({ val: ko.observable(false), def: false, text: "Qua", getType: typesKnockout.bool });
    this.Quinta = PropertyEntity({ val: ko.observable(false), def: false, text: "Qui", getType: typesKnockout.bool });
    this.Sexta = PropertyEntity({ val: ko.observable(false), def: false, text: "Sex", getType: typesKnockout.bool });
    this.Sabado = PropertyEntity({ val: ko.observable(false), def: false, text: "Sab", getType: typesKnockout.bool });
    this.Domingo = PropertyEntity({ val: ko.observable(false), def: false, text: "Dom", getType: typesKnockout.bool });

    this.PeriodosCarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });
}

var PesquisaExclusividadeCarregamento = function () {
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador", idBtnSearch: guid() });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
    this.DataInicial = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Inicial:", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Final:", getType: typesKnockout.date });

    this.DataInicial.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicial;

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridExclusividadeCarregamento, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridExclusividadeCarregamento() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        tamanho: "7",
        opcoes: [opcaoEditar]
    };
    var configuracoesExportacao = { url: "ExclusividadeCarregamento/ExportarPesquisa", titulo: "Exceção de Capacidade de Carregamento" };

    _gridExclusividadeCarregamento = new GridViewExportacao(_pesquisaExclusividadeCarregamento.Pesquisar.idGrid, "ExclusividadeCarregamento/Pesquisa", _pesquisaExclusividadeCarregamento, menuOpcoes, configuracoesExportacao);
    _gridExclusividadeCarregamento.CarregarGrid();
}

function loadExclusividadeCarregamento() {
    _exclusividadeCarregamento = new ExclusividadeCarregamento();
    KoBindings(_exclusividadeCarregamento, "knockoutExclusividadeCarregamento");

    HeaderAuditoria("ExclusividadeCarregamento", _exclusividadeCarregamento);

    _CRUDExclusividadeCarregamento = new CRUDExclusividadeCarregamento();
    KoBindings(_CRUDExclusividadeCarregamento, "knockoutCRUDExclusividadeCarregamento");

    _pesquisaExclusividadeCarregamento = new PesquisaExclusividadeCarregamento();
    KoBindings(_pesquisaExclusividadeCarregamento, "knockoutPesquisaExclusividadeCarregamento", false, _pesquisaExclusividadeCarregamento.Pesquisar.id);

    new BuscarCentrosCarregamento(_pesquisaExclusividadeCarregamento.CentroCarregamento);
    new BuscarTransportadores(_pesquisaExclusividadeCarregamento.Transportador);
    new BuscarClientes(_pesquisaExclusividadeCarregamento.Cliente);
    new BuscarCentrosCarregamento(_exclusividadeCarregamento.CentroCarregamento);
    new BuscarTransportadores(_exclusividadeCarregamento.Transportador);
    new BuscarClientes(_exclusividadeCarregamento.Cliente);
    new BuscarModelosVeicularesCarga(_exclusividadeCarregamento.ModeloVeicularCarga);
    
    loadPeriodoCarregamento();
    loadGridExclusividadeCarregamento();
}

function loadPeriodoCarregamento() {
    _excecaoPeriodoCarregamento = new PeriodoCarregamento("knockoutPeriodoCarregamento", _exclusividadeCarregamento.PeriodosCarregamento);

    _excecaoPeriodoCarregamento.Load();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    if (!ValidarExclusividade()) 
        return;

    Salvar(_exclusividadeCarregamento, "ExclusividadeCarregamento/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridExclusividadeCarregamento();
                limparCamposExclusividadeCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    if (!ValidarExclusividade())
        return;

    Salvar(_exclusividadeCarregamento, "ExclusividadeCarregamento/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridExclusividadeCarregamento();
                limparCamposExclusividadeCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposExclusividadeCarregamento();
}

function editarClick(registroSelecionado) {
    BuscarExcecaoPorCodigo(registroSelecionado.Codigo);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_exclusividadeCarregamento, "ExclusividadeCarregamento/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridExclusividadeCarregamento();
                    limparCamposExclusividadeCarregamento();
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

function ValidarExclusividade() {
    if (_exclusividadeCarregamento.Transportador.codEntity() == 0 && _exclusividadeCarregamento.Cliente.codEntity() == 0 && _exclusividadeCarregamento.ModeloVeicularCarga.codEntity() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Validação", "É necessário selecionar um dos parâmetros: transportador, cliente ou modelo veicular");
        return false;
    }

    return true;
}

function controlarBotoesHabilitados() {
    var isEdicao = _exclusividadeCarregamento.Codigo.val() > 0;

    _CRUDExclusividadeCarregamento.Atualizar.visible(isEdicao);
    _CRUDExclusividadeCarregamento.Excluir.visible(isEdicao);
    _CRUDExclusividadeCarregamento.Adicionar.visible(!isEdicao);
}

function BuscarExcecaoPorCodigo(codigo, cb) {
    limparCamposExclusividadeCarregamento();

    _exclusividadeCarregamento.Codigo.val(codigo);

    BuscarPorCodigo(_exclusividadeCarregamento, "ExclusividadeCarregamento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaExclusividadeCarregamento.ExibirFiltros.visibleFade(false);

                controlarBotoesHabilitados();

                _excecaoPeriodoCarregamento.RecarregarGrid();

                if (cb) cb(retorno.Data);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function limparCamposExclusividadeCarregamento() {
    LimparCampos(_exclusividadeCarregamento);

    controlarBotoesHabilitados();

    _excecaoPeriodoCarregamento.LimparCampos();
    _excecaoPeriodoCarregamento.RecarregarGrid();
}

function recarregarGridExclusividadeCarregamento() {
    _gridExclusividadeCarregamento.CarregarGrid();
}