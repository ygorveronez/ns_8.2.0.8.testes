/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAgendamentoEntrega.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _agendamentoEntrega;
var _pesquisaAgendamentoEntrega;
var _gridAgendamentoEntrega;

/*
 * Declaração das Classes
 */

var AgendamentoEntrega = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SenhaAgendamento = PropertyEntity({ text: "Número Agendamento:", required: false, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(false) });
    this.Senha = PropertyEntity({ text: "Senha Agendamento:", required: false, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(false) });
    this.Situacao = PropertyEntity({ text: "Situação:", required: false, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(false) });
    this.TipoAplicacaoColetaEntrega = PropertyEntity({ val: ko.observable(EnumTipoAplicacaoColetaEntrega.Entrega), options: EnumTipoAplicacaoColetaEntrega.obterOpcoes(), def: EnumTipoAplicacaoColetaEntrega.Entrega, text: "Aplicação da retificação para: ", visible: ko.observable(false) });
    this.DataAgendamento = PropertyEntity({ text: "*Data Agendamento:", required: true, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, required: true, codEntity: ko.observable(0), text: "*Destinatário:", issue: 52, idBtnSearch: guid() });
    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Carga:", issue: 53, required: true, idBtnSearch: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, visible: ko.observable(true), idBtnSearch: guid() , text: "*Modelo Veicular:", issue: 44 });

    this.Descricao = PropertyEntity({ text: "*Senha Agendamento:", required: false, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(false) });
    this.Motorista = PropertyEntity({ text: "Motorista:", required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.Transportador = PropertyEntity({ text: "Transportador:", required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.Placa = PropertyEntity({ text: "Placa:", required: false, getType: typesKnockout.string, val: ko.observable("") });

    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var PesquisaAgendamentoEntrega = function () {
    this.SenhaAgendamento = PropertyEntity({ text: "Senha Agendamento:", required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.Destinatario = PropertyEntity({ type: types.entity, required: false, codEntity: ko.observable(0), text: "Destinatário:", issue: 52, idBtnSearch: guid() });
    this.Placa = PropertyEntity({ text: "Placa:", required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAgendamentoEntrega.Todas), options: EnumSituacaoAgendamentoEntrega.obterOpcoesPesquisa(), def: EnumSituacaoAgendamentoEntrega.Todas, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAgendamentoEntrega.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAgendamentoEntrega() {
    _pesquisaAgendamentoEntrega = new PesquisaAgendamentoEntrega();
    KoBindings(_pesquisaAgendamentoEntrega, "knockoutPesquisaAgendamentoEntrega", false, _pesquisaAgendamentoEntrega.Pesquisar.id);
    
    _agendamentoEntrega = new AgendamentoEntrega();
    KoBindings(_agendamentoEntrega, "knockoutAgendamentoEntrega");

    HeaderAuditoria("AgendamentoEntrega", _agendamentoEntrega);

    new BuscarClientes(_agendamentoEntrega.Destinatario);
    new BuscarClientes(_pesquisaAgendamentoEntrega.Destinatario);
    new BuscarModelosVeicularesCarga(_agendamentoEntrega.ModeloVeicularCarga);
    new BuscarTiposdeCarga(_agendamentoEntrega.TipoDeCarga);

    loadGridAgendamentoEntrega();
}

function loadGridAgendamentoEntrega() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarAgendamentoEntregaClick, tamanho: "7", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };
    var configExportacao = {
        url: "AgendamentoEntrega/ExportarPesquisa",
        titulo: "Motivo Rejeição"
    };

    _gridAgendamentoEntrega = new GridViewExportacao(_pesquisaAgendamentoEntrega.Pesquisar.idGrid, "AgendamentoEntrega/Pesquisa", _pesquisaAgendamentoEntrega, menuOpcoes, configExportacao);
    _gridAgendamentoEntrega.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_agendamentoEntrega, "AgendamentoEntrega/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridAgendamentoEntrega.CarregarGrid();
                limparCamposAgendamentoEntrega();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposAgendamentoEntrega();
}

function editarAgendamentoEntregaClick(itemGrid) {
    limparCamposAgendamentoEntrega();

    _agendamentoEntrega.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_agendamentoEntrega, "AgendamentoEntrega/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaAgendamentoEntrega.ExibirFiltros.visibleFade(false);
                controlarBotoesHabilitados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

/*
 * Declaração das Funções Privadas
 */

function controlarBotoesHabilitados() {
    var isEdicao = _agendamentoEntrega.Codigo.val() > 0;

    _agendamentoEntrega.Adicionar.visible(!isEdicao);
}

function limparCamposAgendamentoEntrega() {
    LimparCampos(_agendamentoEntrega);
    controlarBotoesHabilitados();
}
