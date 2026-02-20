/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMonitoramentoPosicao;
var _MonitoramentoPosicao;


/*
 * Declaração das Classes
 */

var CRUDMonitoramentoPosicao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var MonitoramentoPosicao = function () {
    var dataHoraAtual = moment().format("DD/MM/YYYY HH:mm");
    var latitudeInicial = "-27,054701";
    var longitudeInicial = "-52,370601";
    
    this.Codigo = PropertyEntity({ val: ko.observable(0), required: true, def: 0, getType: typesKnockout.int });
    this.Data = PropertyEntity({ val: ko.observable(dataHoraAtual), def: dataHoraAtual, text: "Data: ", getType: typesKnockout.dateTime });
    this.Local = PropertyEntity({ text: "*Local:", required: true, getType: typesKnockout.string, val: ko.observable("Posição de teste") });
    this.IDEquipamento = PropertyEntity({ text: "*ID Equipamento:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Latitude = PropertyEntity({ val: ko.observable(latitudeInicial), def: latitudeInicial, getType: typesKnockout.decimal, text: "*Latitude:", required: true, maxlength: 10, configDecimal: { precision: 6, allowZero: false, allowNegative: true, thousands: ""} });
    this.Longitude = PropertyEntity({ val: ko.observable(longitudeInicial), def: longitudeInicial, getType: typesKnockout.decimal, text: "*Longitude:", required: true, maxlength: 10, configDecimal: { precision: 6, allowZero: false, allowNegative: true, thousands: "" } });
    this.Velocidade = PropertyEntity({ text: "*Velocidade:", required: true, getType: typesKnockout.int, val: ko.observable("70") });
    this.Temperatura = PropertyEntity({ text: "*Temperatura:", required: true, getType: typesKnockout.int, val: ko.observable("-10"), configInt: { precision: 0, allowZero: true, allowNegative: true } });
    this.Ignicao = PropertyEntity({ text: "*Ignicão:", required: true, getType: typesKnockout.bool, val: ko.observable(1) });
    this.SensorTemperatura = PropertyEntity({ text: "*Sensor de Temperatura:", required: true, getType: typesKnockout.bool, val: ko.observable(1) });
}

/*
 * Declaração das Funções de Inicialização
 */


function loadMonitoramentoPosicao() {
    _MonitoramentoPosicao = new MonitoramentoPosicao();
    KoBindings(_MonitoramentoPosicao, "knockoutMonitoramentoPosicao");

    HeaderAuditoria("MonitoramentoPosicao", _MonitoramentoPosicao);

    _CRUDMonitoramentoPosicao = new CRUDMonitoramentoPosicao();
    KoBindings(_CRUDMonitoramentoPosicao, "knockoutCRUDMonitoramentoPosicao");


}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_MonitoramentoPosicao, "MonitoramentoPosicao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                
                //limparCamposMonitoramentoPosicao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}