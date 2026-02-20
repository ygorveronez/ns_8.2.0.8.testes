/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//********MAPEAMENTO*******

var _posicao;
var _map;
var _marker;
var _geocoder;

var Posicao = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Mapa = PropertyEntity({ types: types.local });

    this.UltimaAtualizacao = PropertyEntity({ text: "Última Atualização:", val: ko.observable(""), def: "" });
    this.Endereco = PropertyEntity({ text: "Endereço:", val: ko.observable(""), def: "" });
}




//********EVENTO*******
function LoadPosicao() {
    _posicao = new Posicao();
    KoBindings(_posicao, "knockoutPosicao");
    
    CarregarMapaRastreamento();
}

function ExibirRastreioCarga(knoutFluxo) {
    _fluxoAtual = knoutFluxo;
    LimparCampos(_posicao);
    executarReST("RastreamentoCarga/BuscarPorCarga", { Carga: knoutFluxo.Carga.val() }, function (arg) {
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
                    _posicao.UltimaAtualizacao.val("Nenhuma integração recebida.");
                }

                ExibeModalEtapa('#divModalPosicao');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
        }
    });
}



//********MÉTODOS*******
function CarregarMapaRastreamento() {
    var opcoesmapa = {
        scaleControl: true,
        gestureHandling: 'greedy'
    };

    _map = new google.maps.Map(document.getElementById(_posicao.Mapa.id), opcoesmapa);

    _geocoder = new google.maps.Geocoder;

    _marker = new google.maps.Marker({
        map: _map
    });

    var posPadrao = PosicionamentoPadrao();

    setTimeout(function () {
        SetarCoordenadas(posPadrao);
    }, 300);
    
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