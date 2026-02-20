/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="FluxoPatio.js" />
/// <reference path="ObservacoesEtapas.js" />

// #region Objetos Globais do Arquivo

var _posicao;
var _map;
var _marker;
var _geocoder;

// #endregion Objetos Globais do Arquivo

// #region Classes

var Posicao = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Mapa = PropertyEntity({ types: types.local });

    this.UltimaAtualizacao = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.UltimaAtualizacao.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Endereco = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Endereco.getFieldDescription(), val: ko.observable(""), def: "" });

    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaPosicaoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.Observacoes, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
};

// #endregion Classes

// #region Funções de Inicialização

function LoadPosicao() {
    _posicao = new Posicao();
    KoBindings(_posicao, "knockoutPosicao");

    CarregarMapaRastreamento();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function observacoesEtapaPosicaoClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.Posicao);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function ExibirRastreioCarga(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;
    LimparCampos(_posicao);
    executarReST("RastreamentoCarga/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                PreencherObjetoKnout(_posicao, arg);

                if (arg.Data.Latitude != 0 && arg.Data.Longitude != 0) {
                    var position = {
                        lat: arg.Data.Latitude,
                        lng: arg.Data.Longitude,
                        zoom: 16
                    };

                    SetarCoordenadas(position);
                    SetMarker(position);
                } else {
                    var posPadrao = PosicionamentoPadrao();
                    SetarCoordenadas(posPadrao);
                    _posicao.UltimaAtualizacao.val(Localization.Resources.GestaoPatio.FluxoPatio.NenhumaIntegracaoRecebida);
                }
                Global.abrirModal('divModalPosicao');
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function CarregarMapaRastreamento() {
    _map = new google.maps.Map(document.getElementById(_posicao.Mapa.id));

    _geocoder = new google.maps.Geocoder;

    _marker = new google.maps.Marker({
        map: _map
    });

    var posPadrao = PosicionamentoPadrao();
    SetarCoordenadas(posPadrao);
}

function SetarCoordenadas(position) {
    if (_map != null) {
        var zoom = position.zoom || 4;

        _map.setZoom(zoom);
        _map.setCenter(position);
        _marker.setPosition(position);
    }
}

function SetMarker(position) {
    if (_marker != null && _map != null) {
        _marker.setMap(_map);
        _geocoder.geocode({ 'location': position }, function (results, status) {
            if (status === 'OK' && results.length >= 0) {
                _posicao.Endereco.val(results[0].formatted_address);
            }
        });
    }
}

function PosicionamentoPadrao() {
    if (_marker != null)
        _marker.setMap(null);

    return {
        lat: -10.861639,
        lng: -53.104038
    };
}

// #endregion Funções Privadas
