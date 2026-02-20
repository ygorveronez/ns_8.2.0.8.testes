/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoMonitoramentoEvento.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoEventoData.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _monitoramentoEventoGatilho;
var _textRaio = "*Raio (metros): ";
var _textTempoEvento = "*Tempo Evento (minutos): ";
var _textTempoEvento2 = "*Tempo Evento 2(minutos): ";

/*
 * Declaração das Classes
 */

var MonitoramentoEventoGatilho = function () {

    this.Tempo = PropertyEntity({ text: "*Tempo Alerta (minutos): ", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, required: true, issue: 49490 });
    this.Raio = PropertyEntity({ text: ko.observable(_textRaio), getType: typesKnockout.int, maxlength: 9, visible: ko.observable(false), required: ko.observable(false) });
    this.Velocidade = PropertyEntity({ text: "*Velocidade Inicial (km/h): ", getType: typesKnockout.int, maxlength: 3, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(false), required: ko.observable(false) });
    this.Velocidade2 = PropertyEntity({ text: "Velocidade Final (km/h): ", getType: typesKnockout.int, maxlength: 3, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(false), required: ko.observable(false) });
    this.TempoEvento = PropertyEntity({ text: ko.observable(_textTempoEvento), getType: typesKnockout.int, maxlength: 4, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(false), required: ko.observable(false), issue: 49491 });
    this.TempoEvento2 = PropertyEntity({ text: ko.observable(_textTempoEvento2), getType: typesKnockout.int, maxlength: 4, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(false), required: ko.observable(false) });
    this.Quantidade = PropertyEntity({ text: ko.observable("*Quantidade:"), getType: typesKnockout.int, maxlength: 6, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(false), required: ko.observable(false) });
    this.PontosDeApoio = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "*Pontos de Apoio:", idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false) });
    this.RaioProximidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Raio Proximidade:", idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false) });
    this.DataBase = PropertyEntity({ text: "Data base: ", val: ko.observable(EnumMonitoramentoEventoData.Padrao), options: ko.observableArray(EnumMonitoramentoEventoData.obterOpcoes()), required: ko.observable(false), visible: ko.observable(false) });
    this.DataReferencia = PropertyEntity({ text: "Data referência: ", val: ko.observable(EnumMonitoramentoEventoData.Padrao), options: ko.observableArray(EnumMonitoramentoEventoData.obterOpcoes()), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(false) });
    this.ConsiderarApenasDataNaReferencia = PropertyEntity({ text: "Considerar Apenas Data (sem hora)", val: ko.observable(false), getType: typesKnockout.bool, visible: ko.observable(false) });
    this.TratativaAutomatica = PropertyEntity({ text: "Tratativa Automática quando data Referencia inferior a data Atual", val: ko.observable(false), getType: typesKnockout.bool, visible: ko.observable(false) });
    this.EventoContinuo = PropertyEntity({ text: "Evento contínuo", val: ko.observable(false), getType: typesKnockout.bool, visible: ko.observable(true) });
    this.TempoReferenteaDataCarregamentoCarga = PropertyEntity({ text: "Tempo é Referente a Data de Carregamento da Carga", val: ko.observable(false), getType: typesKnockout.bool, visible: ko.observable(true), issue: 76346 });
    this.ValidarApenasCargasNaoIniciadas = PropertyEntity({ text: "Validar apenas Cargas não iniciadas", val: ko.observable(false), getType: typesKnockout.bool, visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadMonitoramentoEventoGatilho() {
    _monitoramentoEventoGatilho = new MonitoramentoEventoGatilho();
    KoBindings(_monitoramentoEventoGatilho, "knockoutMonitoramentoEventoGatilho");

    new BuscarLocais(_monitoramentoEventoGatilho.PontosDeApoio, null, null, 4);
    new BuscarLocaisRaioProximidade(_monitoramentoEventoGatilho.RaioProximidade);
}

/*
 * Declaração das Funções Públicas
 */

function controlarExibicaoCamposMonitoramentoEventoGatilho(tipoMonitoramentoEvento) {
    controlarExibicaoCampoRaio(false);
    controlarExibicaoCampoVelocidade(false);
    controlarExibicaoCampoVelocidade2(false);
    controlarExibicaoCampoTempoEvento(false);
    controlarExibicaoCampoTempoEvento2(false);
    controlarExibicaoCampoTempoReferenteaDataCarregamentoCarga(false);
    controlarExibicaoCampoDatas(false);
    controlarExibicaoCheckApenasData(false);
    controlarExibicaoCheckTratativaAutomatica(false);
    controlarExibicaoCampoValidarApenasCargasNaoIniciadas(false);
    controlarExibicaoCampoTempoEvento(false);
    controlarExibicaoCampoPontoApoio(false);
    controlarExibicaoCampoRaioProximidade(false);
    controlarExibicaoCampoQuantidade(false);
    _monitoramentoEventoGatilho.Raio.text(_textRaio);
    _monitoramentoEventoGatilho.TempoEvento.text(_textTempoEvento);
    _monitoramentoEventoGatilho.TempoEvento2.text(_textTempoEvento2);

    switch (tipoMonitoramentoEvento) {
        case EnumTipoMonitoramentoEvento.DesvioDeRota:
        case EnumTipoMonitoramentoEvento.FimDeViagem:
        case EnumTipoMonitoramentoEvento.FimEntrega:
        case EnumTipoMonitoramentoEvento.InicioDeViagem:
        case EnumTipoMonitoramentoEvento.InicioEntrega:
            controlarExibicaoCampoRaio(true);
            break;

        case EnumTipoMonitoramentoEvento.TemperaturaForaDaFaixa:
            controlarExibicaoCampoTempoEvento(true);
            _monitoramentoEventoGatilho.TempoEvento.text("Tempo de temperaturas sequenciais fora da faixa: (minutos)");
            break;

        case EnumTipoMonitoramentoEvento.VelocidadeExcedida:
            controlarExibicaoCampoVelocidade(true);
            controlarExibicaoCampoVelocidade2(true);
            break;

        case EnumTipoMonitoramentoEvento.AtrasoNaEntrega:
            controlarExibicaoCampoTempoEvento2(true);
            _monitoramentoEventoGatilho.TempoEvento.text("Tolerância entrega do pedido: (minutos)");
            _monitoramentoEventoGatilho.TempoEvento2.text("Tolerância descarga planejada: (minutos)");
            controlarExibicaoCheckApenasData(true);
            controlarExibicaoCheckTratativaAutomatica(true);
            controlarExibicaoCampoDatas(true);
            break;
        case EnumTipoMonitoramentoEvento.DirecaoContinuaExcessiva:
            controlarExibicaoCampoRaio(true);
            controlarExibicaoCampoTempoEvento(true);
            controlarExibicaoCampoTempoEvento2(true);
            _monitoramentoEventoGatilho.Raio.text("Raio tolerado: (metros)");
            _monitoramentoEventoGatilho.TempoEvento.text("Tempo mínimo de direção: (minutos)");
            _monitoramentoEventoGatilho.TempoEvento2.text("Tempo máximo de descanso: (minutos)");
            break;
        case EnumTipoMonitoramentoEvento.PerdaDeSinal:
            controlarExibicaoCampoTempoReferenteaDataCarregamentoCarga(true);
        case EnumTipoMonitoramentoEvento.SemSinal:
            controlarExibicaoCampoTempoReferenteaDataCarregamentoCarga(true);
        case EnumTipoMonitoramentoEvento.AtrasoNoCarregamento:
        case EnumTipoMonitoramentoEvento.AtrasoNaLiberacao:
            controlarExibicaoCampoTempoEvento(true);
            break;
        case EnumTipoMonitoramentoEvento.AtrasoNaDescarga:
        case EnumTipoMonitoramentoEvento.ForaDoPrazo:
            controlarExibicaoCampoTempoEvento(true);
            controlarExibicaoCheckApenasData(true);
            _monitoramentoEventoGatilho.TempoEvento.text("Tolerância de atraso permitida: (minutos)");
            controlarExibicaoCampoDatas(true);
            break;
        case EnumTipoMonitoramentoEvento.ParadaNaoProgramada:
        case EnumTipoMonitoramentoEvento.ParadaExcessiva:
            controlarExibicaoCampoRaio(true);
            controlarExibicaoCampoTempoEvento(true);
            break;
        case EnumTipoMonitoramentoEvento.DirecaoSemDescanso:
            controlarExibicaoCampoRaio(true);
            controlarExibicaoCampoTempoEvento(true);
            controlarExibicaoCampoTempoEvento2(true);
            _monitoramentoEventoGatilho.Raio.text("Raio tolerado: (metros)");
            _monitoramentoEventoGatilho.TempoEvento.text("Tempo máximo de direção: (minutos)");
            _monitoramentoEventoGatilho.TempoEvento2.text("Tempo mínimo de descanso: (minutos)");
            break;
        case EnumTipoMonitoramentoEvento.PermanenciaNoRaio:
        case EnumTipoMonitoramentoEvento.PermanenciaNoRaioEntrega:
            controlarExibicaoCampoTempoEvento(true);
            _monitoramentoEventoGatilho.TempoEvento.text("Tempo dentro do raio: (minutos)");
            break;
        case EnumTipoMonitoramentoEvento.PermanenciaNoPontoApoio:
            controlarExibicaoCampoTempoEvento(true);
            controlarExibicaoCampoPontoApoio(true);
            break;
        case EnumTipoMonitoramentoEvento.AusenciaDeInicioDeViagem:
            controlarExibicaoCampoTempoEvento(true);
            break;
        case EnumTipoMonitoramentoEvento.PossivelAtrasoNaOrigem:
            controlarExibicaoCampoDatas(true);
            controlarExibicaoCampoValidarApenasCargasNaoIniciadas(true);
            controlarExibicaoCampoTempoEvento(true);
            _monitoramentoEventoGatilho.TempoEvento.text("Tolerância de atraso permitida: (minutos)");
            break;
        case EnumTipoMonitoramentoEvento.ConcentracaoDeVeiculosNoRaio:
            controlarExibicaoCampoRaioProximidade(true);
            controlarExibicaoCampoQuantidade(true);
            _monitoramentoEventoGatilho.Quantidade.text("*Quantidade Máxima de Veículos:");
            break;

        case EnumTipoMonitoramentoEvento.AlertaTendenciaEntregaAdiantada:
        case EnumTipoMonitoramentoEvento.AlertaTendenciaEntregaAtrasada:
        case EnumTipoMonitoramentoEvento.AlertaTendenciaEntregaPoucoAtrasada:
            controlarExibicaoCheckApenasData(true);
            controlarExibicaoCheckTratativaAutomatica(true);
            controlarExibicaoCampoDatas(true);
            _monitoramentoEventoGatilho.DataBase.val(EnumMonitoramentoEventoData.PrevisaoEntrega);
            _monitoramentoEventoGatilho.DataReferencia.val(EnumMonitoramentoEventoData.InicioEntregaReprogramada);
            _monitoramentoEventoGatilho.DataBase.options(EnumMonitoramentoEventoData.obterOpcoesFiltradasDataBase());
            _monitoramentoEventoGatilho.DataReferencia.options(EnumMonitoramentoEventoData.obterOpcoesFiltradasDataReferencia());
            break;
        case EnumTipoMonitoramentoEvento.SensorTemperaturaComProblema:
            controlarExibicaoCampoTempoEvento(true);
            break;
    }
}

function limparCamposMonitoramentoEventoGatilho() {
    LimparCampos(_monitoramentoEventoGatilho);
}

function obterMonitoramentoEventoGatilhoSalvar() {
    return JSON.stringify(RetornarObjetoPesquisa(_monitoramentoEventoGatilho));
}

function preencherMonitoramentoEventoGatilho(dadosGatilho) {
    PreencherObjetoKnout(_monitoramentoEventoGatilho, { Data: dadosGatilho })
}

function validarCamposObrigatoriosMonitoramentoEventoGatilho() {
    return ValidarCamposObrigatorios(_monitoramentoEventoGatilho);
}

/*
 * Declaração das Funções
 */

function controlarExibicaoCampoRaio(isExibirCampo) {
    _monitoramentoEventoGatilho.Raio.visible(isExibirCampo);
    _monitoramentoEventoGatilho.Raio.required(isExibirCampo);
}

function controlarExibicaoCampoVelocidade(isExibirCampo) {
    _monitoramentoEventoGatilho.Velocidade.visible(isExibirCampo);
    _monitoramentoEventoGatilho.Velocidade.required(isExibirCampo);
}

function controlarExibicaoCampoVelocidade2(isExibirCampo) {
    _monitoramentoEventoGatilho.Velocidade2.visible(isExibirCampo);
    _monitoramentoEventoGatilho.Velocidade2.required(false);
}

function controlarExibicaoCampoTempoEvento(isExibirCampo) {
    _monitoramentoEventoGatilho.TempoEvento.visible(isExibirCampo);
    _monitoramentoEventoGatilho.TempoEvento.required(isExibirCampo);
}

function controlarExibicaoCampoTempoReferenteaDataCarregamentoCarga(isExibirCampo) {
    _monitoramentoEventoGatilho.TempoReferenteaDataCarregamentoCarga.visible(isExibirCampo);
}


function controlarExibicaoCampoPontoApoio(isExibirCampo) {
    //_monitoramentoEventoGatilho.PontoApoio.visible(isExibirCampo);
    //_monitoramentoEventoGatilho.PontoApoio.required(isExibirCampo);
    _monitoramentoEventoGatilho.PontosDeApoio.visible(isExibirCampo);
    _monitoramentoEventoGatilho.PontosDeApoio.required(isExibirCampo);
}

function controlarExibicaoCampoRaioProximidade(isExibirCampo) {
    _monitoramentoEventoGatilho.RaioProximidade.visible(isExibirCampo);
    _monitoramentoEventoGatilho.RaioProximidade.required(isExibirCampo);
}

function controlarExibicaoCampoQuantidade(isExibirCampo) {
    _monitoramentoEventoGatilho.Quantidade.visible(isExibirCampo);
    _monitoramentoEventoGatilho.Quantidade.required(isExibirCampo);
}

function controlarExibicaoCampoTempoEvento2(isExibirCampo) {
    _monitoramentoEventoGatilho.TempoEvento2.visible(isExibirCampo);
    _monitoramentoEventoGatilho.TempoEvento2.required(isExibirCampo);
}

function controlarExibicaoCheckApenasData(isExibirCampo) {
    _monitoramentoEventoGatilho.ConsiderarApenasDataNaReferencia.visible(isExibirCampo);
}

function controlarExibicaoCheckTratativaAutomatica(isExibirCampo) {
    _monitoramentoEventoGatilho.TratativaAutomatica.visible(isExibirCampo);
}

function controlarExibicaoCampoDatas(isExibirCampo) {
    _monitoramentoEventoGatilho.DataBase.visible(isExibirCampo);
    _monitoramentoEventoGatilho.DataBase.required(isExibirCampo);
    _monitoramentoEventoGatilho.DataReferencia.visible(isExibirCampo);
    _monitoramentoEventoGatilho.DataReferencia.required(isExibirCampo);
}

function controlarExibicaoCampoValidarApenasCargasNaoIniciadas(isExibirCampo) {
    _monitoramentoEventoGatilho.ValidarApenasCargasNaoIniciadas.visible(isExibirCampo);
}