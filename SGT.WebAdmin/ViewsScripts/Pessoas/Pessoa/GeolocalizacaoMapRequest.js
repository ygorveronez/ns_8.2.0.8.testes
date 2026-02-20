///// <reference path="../../Enumeradores/EnumTipoPessoa.js" />
///// <reference path="../../../js/libs/jquery-2.1.1.js" />
///// <reference path="../../../js/Global/CRUD.js" />
///// <reference path="../../../js/Global/knockout-3.1.0.js" />
///// <reference path="../../../js/Global/Rest.js" />
///// <reference path="../../../js/Global/Mensagem.js" />
///// <reference path="../../../js/Global/Grid.js" />
///// <reference path="../../../js/bootstrap/bootstrap.js" />
///// <reference path="../../../js/libs/jquery.blockui.js" />
///// <reference path="../../../js/Global/knoutViewsSlides.js" />
///// <reference path="../../../js/libs/jquery.maskMoney.js" />
///// <reference path="TransportadorTerceiro.js" />
///// <reference path="Fornecedor.js" />
///// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
///// <reference path="../../Consultas/Cliente.js" />
///// <reference path="../../Consultas/Atividade.js" />
///// <reference path="../../Consultas/Localidade.js" />
///// <reference path="../../Consultas/Endereco.js" />
///// <reference path="ListaEmail.js" />
///// <reference path="ListaEndereco.js" />
///// <reference path="DadoBancario.js" />
///// <reference path="Emissao.js" />
///// <reference path="../../Enumeradores/EnumTipoEmissaoCTeDocumentos.js" />
///// <reference path="../../Enumeradores/EnumTipoLocalizacao.js" />



//var _pesquisaGeolocalizacaoMapRequest;
//var mapRequest;
//var popup;

//var latLngDefault = { lat: -10.861639, lng: -53.104038 };

//var PesquisaGeolocalizacaoMapRequest = function () {

//    this.Map = PropertyEntity();
//    this.BuscarCoordenadas = PropertyEntity({ eventClick: BuscarCoordenadasClick, type: types.event, text: "Buscar Coordenadas do Endereço", visible: ko.observable(true) });
//    this.PrecisaoCoordenadas = PropertyEntity({});
//}

//function loadGeolocalizacaoMapRequest() {
//    _pesquisaGeolocalizacaoMapRequest = new PesquisaGeolocalizacaoMapRequest();
//    KoBindings(_pesquisaGeolocalizacaoMapRequest, "knockoutGeolocalizacaoMapRequest");

//}

//function CarregarMapa() {
//    if (mapRequest == null) {
//        setTimeout(function () {
//            var mapLayer = MQ.mapLayer();
//            mapRequest = L.map(_pesquisaGeolocalizacaoMapRequest.Map.id, {
//                layers: mapLayer,
//                center: [latLngDefault.lat, latLngDefault.lng],
//                zoom: 3
//            });

//            L.control.layers({
//                'Mapa': mapLayer,
//                'Satélite': MQ.satelliteLayer()
//            }).addTo(mapRequest);

//            var mapQuestMarker = L.icon({
//                iconUrl: MQ.mapConfig.getConfig("imagePath") + 'poi.png',
//                iconRetinaUrl: MQ.mapConfig.getConfig("imagePath") + 'poi@2x.png',
//                iconSize: [36, 35],
//                iconAnchor: [15, 35],
//                popupAnchor: [-1, -30]
//            });

//            popup = L.marker(latLngDefault, { icon: mapQuestMarker, draggable: true }).addTo(mapRequest);

//            popup.on('dragend', function (event) {
//                var marker = event.target;
//                var position = marker.getLatLng().wrap();
//                setarGeoLocalizacaoPessoa(position.lat, position.lng, EnumTipoLocalizacao.ponto);
//                setarPrecisaoCoordenadas(EnumTipoLocalizacao.ponto);
//            });
//            aplicarGeoLocalizacaoMapa(_pessoa.Latitude.val(), _pessoa.Longitude.val(), _pessoa.TipoLocalizacao.val());
//        }, 200);
//    } else {
//        aplicarGeoLocalizacaoMapa(_pessoa.Latitude.val(), _pessoa.Longitude.val(), _pessoa.TipoLocalizacao.val());
//    }
//}

//function setarMapaDefault() {
//    if (mapRequest != null) {
//        mapRequest.setView(latLngDefault, 3);
//        popup.setLatLng(latLngDefault);
//    }
//}

//function limparCamposMapaRequest() {
//    setarMapaDefault();
//    _pesquisaGeolocalizacaoMapRequest.PrecisaoCoordenadas.val("");
//}

//function BuscarCoordenadasClick(e, sender) {
//    BuscarCoordenadas();
//}

//function BuscarCoordenadas(callback) {
//    //todo: ver quando tiver a api oficial para buscar as coordenadas quando a empresa possuir habilitada a geolocalização
//    if (_CONFIGURACAO_TMS.PermiteAdicionarNotaManualmente) {
//        if (ValidarCampoObrigatorioEntity(_pessoa.Localidade) && ValidarCampoObrigatorioMap(_pessoa.Endereco)) {
//            var data = { Localidade: _pessoa.Localidade.codEntity(), Endereco: _pessoa.Endereco.val(), Numero: _pessoa.Numero.val(), CEP: _pessoa.CEP.val() }
//            executarReST("Pessoa/ConsultarCoordenadasOpenMap", data, function (arg) {
//                if (arg.Success) {
//                    if (arg.Data !== false) {
//                        var coordenadas = arg.Data;
//                        aplicarGeoLocalizacaoMapa(coordenadas.latitude, coordenadas.longitude, coordenadas.tipoLocalizacao);
//                        setarGeoLocalizacaoPessoa(coordenadas.latitude, coordenadas.longitude, coordenadas.tipoLocalizacao);
//                    } else {
//                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
//                    }
//                } else {
//                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//                }
//                if (callback != null)
//                    callback();
//            });
//        } else {
//            exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Para buscar a localização no mapa é necessário informar a localidade e o endereço.");
//            if (callback != null)
//                callback();
//        }
//    } else {
//        if (callback != null)
//            callback();
//    }
//}

//function aplicarGeoLocalizacaoMapa(latitude, longitude, tipoLocalizacao) {
//    if (mapRequest != null) {
//        if (tipoLocalizacao != EnumTipoLocalizacao.naoEncontrado) {
//            var latlng = { lat: latitude, lng: longitude };
//            mapRequest.setView(latlng, zoomPorTipoLocalizacao(tipoLocalizacao));
//            popup.setLatLng(latlng);
//        }
//    }
//    setarPrecisaoCoordenadas(tipoLocalizacao);
//}

//function setarGeoLocalizacaoPessoa(latitude, longitude, tipoLocalizacao) {
//    if (tipoLocalizacao != EnumTipoLocalizacao.naoEncontrado) {
//        _pessoa.TipoLocalizacao.val(tipoLocalizacao);
//        _pessoa.Latitude.val(latitude);
//        _pessoa.Longitude.val(longitude);
//    }
//}

//function zoomPorTipoLocalizacao(tipoLocalizacao) {
//    var zoom = 10;
//    if (tipoLocalizacao == EnumTipoLocalizacao.cidade) {
//        zoom = 10;
//    } else if (tipoLocalizacao == EnumTipoLocalizacao.cep || tipoLocalizacao == EnumTipoLocalizacao.rua) {
//        zoom = 15;
//    } else if (tipoLocalizacao == EnumTipoLocalizacao.esquina || tipoLocalizacao == EnumTipoLocalizacao.endereco || tipoLocalizacao == EnumTipoLocalizacao.ponto) {
//        zoom = 16;
//    }

//    return zoom;
//}

//function setarPrecisaoCoordenadas(tipoLocalizacao) {
//    if (tipoLocalizacao == EnumTipoLocalizacao.cidade) {
//        _pesquisaGeolocalizacaoMapRequest.PrecisaoCoordenadas.val("Encontrou as coordenadas da Cidade");
//    } else if (tipoLocalizacao == EnumTipoLocalizacao.cep) {
//        _pesquisaGeolocalizacaoMapRequest.PrecisaoCoordenadas.val("Encontrou as coordenadas do CEP");
//    } else if (tipoLocalizacao == EnumTipoLocalizacao.rua) {
//        _pesquisaGeolocalizacaoMapRequest.PrecisaoCoordenadas.val("Encontrou as coordenadas da Rua");
//    } else if (tipoLocalizacao == EnumTipoLocalizacao.endereco) {
//        _pesquisaGeolocalizacaoMapRequest.PrecisaoCoordenadas.val("Encontrou as coordenadas do Endereço");
//    } else if (tipoLocalizacao == EnumTipoLocalizacao.esquina) {
//        _pesquisaGeolocalizacaoMapRequest.PrecisaoCoordenadas.val("Encontrou as coordenadas da Esquina");
//    } else if (tipoLocalizacao == EnumTipoLocalizacao.vizinhanca) {
//        _pesquisaGeolocalizacaoMapRequest.PrecisaoCoordenadas.val("Encontrou as coordenadas aproximadas");
//    } else if (tipoLocalizacao == EnumTipoLocalizacao.ponto) {
//        _pesquisaGeolocalizacaoMapRequest.PrecisaoCoordenadas.val("Encontrou as coordenadas do ponto informado");
//    } else if (tipoLocalizacao == EnumTipoLocalizacao.naoEncontrado) {
//        _pesquisaGeolocalizacaoMapRequest.PrecisaoCoordenadas.val("Coordenadas não informadas");
//    }
//}


