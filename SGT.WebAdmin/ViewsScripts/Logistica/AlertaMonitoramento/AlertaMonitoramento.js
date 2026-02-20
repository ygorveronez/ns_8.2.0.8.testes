/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoAlerta.js" />
/// <reference path="../../Enumeradores/EnumTipoMonitoramentoEvento.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/CargaEntrega.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/EventoMonitoramento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDAlertaMonitoramento;
var _AlertaMonitoramento;

/*
 * Declaração das Classesm
 */

var CRUDAlertaMonitoramento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Cadastrar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var AlertaMonitoramento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 50 });
    this.Evento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Evento:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Carga:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Veículo:", idBtnSearch: guid() });
    this.DataAlerta = PropertyEntity({ text: "*Data Alerta:", required: true, getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.CargaEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Entrega/Coleta:", idBtnSearch: guid(), enable: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */


function loadAlertaMonitoramento() {
    _AlertaMonitoramento = new AlertaMonitoramento();
    KoBindings(_AlertaMonitoramento, "knockoutAlertaMonitoramento");

    HeaderAuditoria("AlertaMonitoramento", _AlertaMonitoramento);

    _CRUDAlertaMonitoramento = new CRUDAlertaMonitoramento();
    KoBindings(_CRUDAlertaMonitoramento, "knockoutCRUDAlertaMonitoramento");

    new BuscarVeiculos(_AlertaMonitoramento.Veiculo);
    new BuscarEventosMonitoramento(_AlertaMonitoramento.Evento);
    new BuscarCargas(_AlertaMonitoramento.Carga, RetornoConsultaCarga, null, [EnumSituacoesCarga.Encerrada, EnumSituacoesCarga.EmTransporte]);
    new BuscarCargaEntrega(_AlertaMonitoramento.CargaEntrega, RetornoConsultaCargaEntrega, _AlertaMonitoramento.Carga);
}


function RetornoConsultaCarga(data) {
    _AlertaMonitoramento.Carga.codEntity(data.Codigo);
    _AlertaMonitoramento.Carga.val(data.CodigoCargaEmbarcador);
}

function RetornoConsultaCargaEntrega(data) {
    console.log(data);
    _AlertaMonitoramento.CargaEntrega.codEntity(data.Codigo);
    _AlertaMonitoramento.CargaEntrega.val(data.Codigo + ' - ' + data.Remetente);
}


/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_AlertaMonitoramento, "AlertaMonitoramento/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                limparCamposAlertaMonitoramento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposAlertaMonitoramento();
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados(isEdicao) {
    _CRUDAlertaMonitoramento.Cancelar.visible(isEdicao);
    _CRUDAlertaMonitoramento.Adicionar.visible(!isEdicao);
}

function limparCamposAlertaMonitoramento() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_AlertaMonitoramento);
}
