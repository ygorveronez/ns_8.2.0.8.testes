// #region Referencias
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../../../MultiCTe/SGT.WebAdmin/ViewsScripts/Enumeradores/EnumTipoAlerta.js" />
// #endregion Referencias

// #region Obejos Globais do Arquivo
var _raiosProximidade = [];
var _mapaRaioProximidade;
//#endregion Obejos Globais do Arquivo

// #region Classes
var RaioProximidade = function () {
    this.Codigo = PropertyEntity({ id: guid(), val: ko.observable(0) });
    this.Raio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 6, text: "Raio (em km):", required: false, visible: ko.observable(true), configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.Identificacao = PropertyEntity({ text: "*Identificação:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Cor = PropertyEntity({ text: "*Cor:", visible: ko.observable(true), enable: ko.observable(false) });
    this.GerarAlertaAutomaticoPorPermanencia = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar alerta automático por permanência", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.Tempo = PropertyEntity({ text: "*Tempo para assumir como permanência (em minutos):", required: this.GerarAlertaAutomaticoPorPermanencia.val, getType: typesKnockout.int, val: ko.observable(0), visible: ko.observable(true) });
    this.TipoAlerta = PropertyEntity({ text: "Tipo de alerta:", val: ko.observable(null), options: EnumTipoAlerta.obterOpcoesPesquisa(), def: null, visible: ko.observable(true), required: this.GerarAlertaAutomaticoPorPermanencia.val });
    this.VerNoMapa = PropertyEntity({ eventClick: verNoMapaClick, type: types.event, text: "Ver No Mapa", visible: ko.observable(true) });
    this.RemoverRaioProximidade = PropertyEntity({ eventClick: removerRaioProximidade, type: types.event, visible: ko.observable(true) });
}
// #endregion Classes

// #region Funções de Inicialização
function adicionarNovoRaio(raio) {
    $.get("Content/Static/Logistica/Locais/RaioProximidade.html?dyn=" + guid(), function (html) {
        _raioProximidade = new RaioProximidade();
        if (raio) {
            _raioProximidade.Codigo.val(raio.Codigo);
            _raioProximidade.Raio.val(raio.Raio);
            _raioProximidade.Identificacao.val(raio.Identificacao);
            _raioProximidade.Cor.val(raio.Cor);
            _raioProximidade.GerarAlertaAutomaticoPorPermanencia.val(raio.GerarAlertaAutomaticoPorPermanencia);
            _raioProximidade.Tempo.val(raio.Tempo);
            _raioProximidade.TipoAlerta.val(raio.TipoAlerta);
        }
        var guid = (_raioProximidade.Codigo.val() > 0 ? _raioProximidade.Codigo.val() : _raioProximidade.Codigo.id);
        var knockoutCadastroRaioProximidade = "knockoutCadastroRaioProximidade";
        var knockoutCadastroRaioProximidadeDinamico = knockoutCadastroRaioProximidade + guid;

        html = html.replaceAll(knockoutCadastroRaioProximidade, knockoutCadastroRaioProximidadeDinamico);

        $("#divCadastroRaiosProximidade").append(html);

        KoBindings(_raioProximidade, knockoutCadastroRaioProximidadeDinamico);
        loadMapaRaioProximidade();

        _raiosProximidade.push(_raioProximidade);

        if (_raiosProximidade.length >= 5)
            _CRUDLocais.AdicionarNovoRaio.visible(false);
    })
}

function loadMapaRaioProximidade() {
    var opcoesMapa = new OpcoesMapa(false, false);
    _mapaRaioProximidade = new MapaGoogle("mapaRaioProximidade", true, opcoesMapa);
}


// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function removerRaioProximidade(e) {
    if (e == undefined)
        return;

    var codigo = (e.Codigo.val() > 0 ? e.Codigo.val() : e.Codigo.id)
    var index = _raiosProximidade.findIndex(function (raio) {
        return raio.Codigo.val() === codigo || raio.Codigo.id === codigo;
    });

    if (index > -1) {
        _raiosProximidade.splice(index, 1);
    }

    $('#knockoutCadastroRaioProximidade' + codigo).remove();

    if (_raiosProximidade.length < 5) {
        _CRUDLocais.AdicionarNovoRaio.visible(true);
    }
}

function verNoMapaClick(e) {
    if (e == undefined)
        return;

    var jsonMarkerMapaGeolocalizacao = _mapaLocais.draw.getJson();

    if (jsonMarkerMapaGeolocalizacao.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Necessário escolher uma base no mapa", "Favor escolher uma base no mapa da aba 'Geolocalização'");
        return;
    }

    jsonMarkerMapaGeolocalizacao = JSON.parse(_mapaLocais.draw.getJson());
    var markerMapaGeolocalizacao = jsonMarkerMapaGeolocalizacao.find(item => item.type === "marker");

    if (markerMapaGeolocalizacao == undefined) {
        exibirMensagem(tipoMensagem.atencao, "Necessário escolher um local base (pin) no mapa", "Favor escolher um local base (pin) no mapa da aba 'Geolocalização'");
        return;
    }

    if (!ValidarCamposObrigatorios(e)) {
        exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    desenharRaioMapa(e, markerMapaGeolocalizacao);

    Global.abrirModal('divModalMapaRaioProximidade');
}
// #endregion Funções Associadas a Eventos

// #region Funções Públicas
function buscarRaiosProximidade() {
    var listaRetorno = [];
    for (var i = 0; i < _raiosProximidade.length; i++) {
        var raio = _raiosProximidade[i];

        listaRetorno.push({
            Codigo: raio.Codigo.val(),
            Raio: raio.Raio.val(),
            Identificacao: raio.Identificacao.val(),
            Cor: raio.Cor.val(),
            GerarAlertaAutomaticoPorPermanencia: raio.GerarAlertaAutomaticoPorPermanencia.val(),
            Tempo: String(raio.Tempo.val()).replace(/\./g, ""),
            TipoAlerta: raio.TipoAlerta.val() == undefined ? EnumTipoAlerta.SemAlerta : raio.TipoAlerta.val()
        })
    }

    return JSON.stringify(listaRetorno);
}

function LimparRaiosProximidade() {
    _raiosProximidade = [];
    $('#divCadastroRaiosProximidade').html("");
}

function ValidarCamposObrigatoriosRaioProximidade() {
    var camposObrigatoriosPreenchidos = true;

    for (var i = 0; i < _raiosProximidade.length; i++) {
        var raio = _raiosProximidade[i];

        if (!ValidarCamposObrigatorios(raio)) {
            camposObrigatoriosPreenchidos = false;
        }
    }

    if (!camposObrigatoriosPreenchidos) {
        exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return false;
    }

    return true;
}
// #endregion Funções Públicas

// #region Funções Privadas
function desenharRaioMapa(registroSelecionado, localSelecionadoMapaGeolocalizacao) {

    _mapaRaioProximidade.draw.clear();

    var center = { lat: localSelecionadoMapaGeolocalizacao.position.lat, lng: localSelecionadoMapaGeolocalizacao.position.lng };
    var radius = registroSelecionado.Raio.val() * 1000;

    var shapeCircle = new ShapeCircle();
    shapeCircle.center = center;
    shapeCircle.radius = radius;
    shapeCircle.fillColor = registroSelecionado.Cor.val();
    shapeCircle.strokeColor = registroSelecionado.Cor.val();
    shapeCircle.strokeOpacity = 0.60;
    shapeCircle.fillOpacity = 0.30;

    var shapeMarker = new ShapeMarker();
    shapeMarker.setPosition(center.lat, center.lng);

    _mapaRaioProximidade.draw.addShape(shapeCircle);
    _mapaRaioProximidade.draw.addShape(shapeMarker);
    _mapaRaioProximidade.direction.centralizar(center.lat, center.lng);


}
// #endregion Funções Privadas
