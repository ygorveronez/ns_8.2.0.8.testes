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

var _pedido;
var _mapEntrega;
var _markerEntrega;
var _markerCliente;
var _geocoderEntrega;

var Pedido = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Numero = PropertyEntity({ text: "Número:" });
    this.Destinatario = PropertyEntity({ text: "Destinatário:" });
    this.NomeRecebedor = PropertyEntity({ text: "Nome Recebedor:" });
    this.DocumentoRecebedor = PropertyEntity({ text: "Documento Recebedor:" });
    this.Localidade = PropertyEntity({ text: "Localidade:" });
    this.Situacao = PropertyEntity({ text: "Situação:" });
    this.DataPrevisaoEntrega = PropertyEntity({ text: "Data Previsão da Entrega:", visible: ko.observable(false) });
    this.DataEntrega = PropertyEntity({ text: "Data da Entrega:", visible: ko.observable(false) });
    this.DataRejeitado = PropertyEntity({ text: "Data da Rejeição:", visible: ko.observable(false) });
    this.Avaliacao = PropertyEntity({ text: "Avalicação da Entrega:", estrelas: [3, 2, 1], val: ko.observable(0), def: 0 });
    this.Observacao = PropertyEntity({ text: "Observação do Cliente:" });
    this.JanelaDescarga = PropertyEntity({ text: "Janela de Descarga:" });

    this.LocalEntrega = PropertyEntity({ type: types.local });
    this.Imagens = PropertyEntity({ val: ko.observableArray([]) });
    this.ExpandirImagem = PropertyEntity({ eventClick: ExpandirImagem, type: types.event });

    this.ConfirmarEntrega = PropertyEntity({ eventClick: ConfirmarEntregaClick, type: types.event, text: "Confirmar Entrega", visible: ko.observable(false) });
    this.RejeitarEntrega = PropertyEntity({ eventClick: RejeitarEntregaClick, type: types.event, text: "Rejeitar Entrega", visible: ko.observable(false) });
}




//********EVENTO*******
function LoadPedido() {
    _pedido = new Pedido();
    KoBindings(_pedido, "knockoutPedido");

    CarregarMapaEntrega();
}

function ExibirDetalhesPedido(knoutFluxo, pedido) {
    _fluxoAtual = knoutFluxo;
    LimparCamposDetalhesPedido(_pedido);
    executarReST("FluxoEntrega/BuscarDetalhesEntrega", { Codigo: pedido.Codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                PreencherObjetoKnout(_pedido, arg);
                TituloModalPedido(pedido.Numero);
                //$("a[href='#tab-local-entrega']").click();

                _pedido.DataPrevisaoEntrega.visible(false);
                _pedido.DataEntrega.visible(false);
                _pedido.DataRejeitado.visible(false);

                _pedido.Avaliacao.get$().find("#star_" + _pedido.Avaliacao.val()).prop('checked', true);

                if (arg.Data.EnumSituacao == EnumSituacaoEntregaPedido.Entregue)
                    _pedido.DataEntrega.visible(true);
                else if (arg.Data.EnumSituacao == EnumSituacaoEntregaPedido.Rejeitado)
                    _pedido.DataRejeitado.visible(true);
                else
                    _pedido.DataPrevisaoEntrega.visible(true);

                _pedido.ConfirmarEntrega.visible(false);
                _pedido.RejeitarEntrega.visible(false);

                if (_fluxoAtual.Situacao.val() == EnumSituacaoEtapaFluxoGestaoEntrega.Aguardando) {
                    if (arg.Data.EnumSituacao == EnumSituacaoEntregaPedido.NaoEntregue) {
                        _pedido.ConfirmarEntrega.visible(true);
                        _pedido.RejeitarEntrega.visible(true);
                    } else if (arg.Data.EnumSituacao == EnumSituacaoEntregaPedido.Entregue) {
                        _pedido.RejeitarEntrega.visible(true);
                    } else if (arg.Data.EnumSituacao == EnumSituacaoEntregaPedido.Rejeitado) {
                        _pedido.ConfirmarEntrega.visible(true);
                    }
                }

                ExibeModalEtapa('#divModalPedido', function () {
                    var positionCliente = {
                        lat: arg.Data.LocalCliente.Latitude,
                        lng: arg.Data.LocalCliente.Longitude
                    };
                    var positionEntrega = {
                        lat: arg.Data.LocalEntrega.Latitude,
                        lng: arg.Data.LocalEntrega.Longitude,
                    };
                    SetarCoordenadasEntrega(positionCliente, positionEntrega);
                });
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
        }
    });
}

function ConfirmarEntregaClick(e, sender) {
    exibirConfirmacao("Confirmação Entrega", "Realmente deseja confirmar entrega do pedido?", function () {
        executarReST("FluxoEntrega/ConfirmarPedido", { Codigo: e.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Confirmado com sucesso");
                    AtualizarFluxoEntrega();
                    Global.fecharModal('divModalPedido');
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function RejeitarEntregaClick(e, sender) {
    exibirConfirmacao("Rejeição Pedido", "Realmente deseja rejeitar a entrega do pedido?", function () {
        executarReST("FluxoEntrega/RejeitarPedido", { Codigo: e.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Rejeitado com sucesso");
                    AtualizarFluxoEntrega();
                    Global.fecharModal('divModalPedido');
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}



//********MÉTODOS*******
function LimparCamposDetalhesPedido() {
    LimparCampos(_pedido);
    $("#knockoutPedido input[name='avalicacao']:checked").prop('checked', false);
}

function TituloModalPedido(numero) {
    $(".modal-pedido-title").html("Pedido - " + numero);
}

function ExpandirImagem(codigo) {
    window.open("FluxoEntrega/ExibirAnexo?Codigo=" + codigo);
}

function CarregarMapaEntrega() {
    _mapEntrega = new google.maps.Map(document.getElementById(_pedido.LocalEntrega.id));

    _geocoderEntrega = new google.maps.Geocoder;

    _markerEntrega = new google.maps.Marker({
        map: _mapEntrega,
        icon: "http://maps.google.com/mapfiles/kml/pal2/icon13.png"
    });

    _markerCliente = new google.maps.Marker({
        map: _mapEntrega
    });
}

function SetarCoordenadasEntrega(positionCliente, positionEntrega) {
    if (_mapEntrega != null) {
        var bounds = new google.maps.LatLngBounds();
        
        if (PosicaoValida(positionCliente)) {
            _markerCliente.setPosition(positionCliente);
            _markerCliente.setMap(_mapEntrega);
            _geocoderEntrega.geocode({ 'location': positionCliente }, function (results, status) {
                if (status === 'OK' && results.length >= 0) {
                    _pedido.LocalEntrega.val(results[0].formatted_address);
                }
            });

            bounds.extend(new google.maps.LatLng(_markerCliente.position.lat(), _markerCliente.position.lng()));
        } else {
            _markerCliente.setMap(null);
            _pedido.LocalEntrega.val("");
        }

        if (positionEntrega != null && PosicaoValida(positionEntrega)) {
            _markerEntrega.setPosition(positionEntrega);
            _markerEntrega.setMap(_mapEntrega);

            bounds.extend(new google.maps.LatLng(_markerEntrega.position.lat(), _markerEntrega.position.lng()));
        } else {
            _markerEntrega.setMap(null);
        }

        if (_markerEntrega.getMap() == null && _markerCliente.getMap() == null) {
            _mapEntrega.setCenter(PosicionamentoPadraoEntrega());
            _mapEntrega.setZoom(4);
        } else {
            _mapEntrega.fitBounds(bounds);
            _mapEntrega.panToBounds(bounds); 
        }
    }
}

function PosicaoValida(position) {
    return position.lat != 0 && position.lng != 0;
}

function PosicionamentoPadraoEntrega() {
    return {
        lat: -10.861639,
        lng: -53.104038
    };
}