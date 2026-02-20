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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />

var _pedidosSemPontos;
var _pedidoMapa;
var $liMap;
var _PedidosAtualizarPontos;
var _PedidosPontosNaoLocalizados;

var PedidoMapa = function () {
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Localidade, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Rota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Rota, idBtnSearch: guid(), enable: ko.observable(true) });
    this.TotalPedidos = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.MontagemCargaMapa.TotalDePontos.getFieldDescription() });
    this.Buscar = PropertyEntity({ eventClick: buscarCargasRotaClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.AdicionarPedidos, visible: ko.observable(true), enable: ko.observable(true) });
    this.RemoverPedidos = PropertyEntity({ eventClick: removerPedidosRotaClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.RemoverPedidos, visible: ko.observable(true), enable: ko.observable(true) });
}

function loadPedidoMapa() {
    _pedidoMapa = new PedidoMapa();
    KoBindings(_pedidoMapa, "knoutMapa");

    new BuscarRotasFrete(_pedidoMapa.Rota);
    new BuscarLocalidades(_pedidoMapa.Localidade);

    $liMap = $("#liMapa");
}

function VerificarOcorrenciaCodigosEmCodigos(codigos, emCodigos) {
    for (var i in codigos) {
        if ($.inArray(codigos[i], emCodigos) >= 0) {
            return true;
        }
    }
    return false;
}

function RemarcarPontosPedidosMapa() {
    LimparPontoMarcados();

    if (!_arrayMarker || _arrayMarker.length === 0) ObterPontosPedidos(true);

    var codigoPedidos = ObterCodigoPedidosSelecionados();
    for (var i = 0; i < _arrayMarker.length; i++) {
        var marker = _arrayMarker[i];
        if (VerificarOcorrenciaCodigosEmCodigos(marker.codigos, codigoPedidos))
            marker.selecionado = true;
        _arrayMarker[i] = marker;
    }
    setarPontosSelecionados(false);
    drawPolylineSelecionados();
}

function removerPedidosRotaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.RealmenteDesejaRemoverTodosOsPedidosQueEstaoNaRota, function () {
        var markersRemovidos = [];
        var codigoLocalidade = _pedidoMapa.Localidade.codEntity();
        var codigoRota = _pedidoMapa.Rota.codEntity();

        if (codigoRota > 0 || codigoLocalidade > 0) {
            var remove = false;
            for (var i = 0; i < _arrayMarker.length; i++) {
                var marker = _arrayMarker[i];
                for (var p = 0; p < marker.pedidos.length; p++) {
                    var pedido = marker.pedidos[p];
                    if ((codigoRota > 0 && pedido.RotaFrete.Codigo == codigoRota) || (codigoLocalidade > 0 && ObterLocalidadePedido(pedido, marker.distribuidor) == codigoLocalidade)) {
                        //marker.selecionado = false;
                        markersRemovidos.push(pedido);
                    }
                }
            }

            var pedidosJaRemovidos = [];
            EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = true;
            markersRemovidos.map(function (markerRemove) {
                var pedido = ObterPedidoPorCodigo(markerRemove.Codigo);
                if (pedido.Selecionado === false || $.inArray(pedido.Codigo, pedidosJaRemovidos) >= 0) return;

                pedidosJaRemovidos.push(pedido.Codigo);
                SelecionarPedido(pedido, true);
                remove = true;
            });
            EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = false;
            _precisarSetarPedidosSelecionadosTabelaPedidosSessao = true;
            PedidosSelecionadosChange();

            if (!remove) 
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Cargas.MontagemCargaMapa.NaoFoiEncontradaNenhumaEntregaParaEssaConfiguracao);
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCargaMapa.Obrigatorio, Localization.Resources.Cargas.MontagemCargaMapa.ObrigatorioInformarRotaOuLocalidade);
        }
        setarPontosSelecionados(true);
    });
}

function buscarCargasRotaClick() {

    var encontrou = false;
    var codigoLocalidade = _pedidoMapa.Localidade.codEntity();
    var codigoRota = _pedidoMapa.Rota.codEntity();

    if (codigoRota > 0 || codigoLocalidade > 0) {
        // Busca pedidos compativeis com o filtro
        var markersAdicionar = [];
        for (var i = 0; i < _arrayMarker.length; i++) {
            var marker = _arrayMarker[i];
            for (var p = 0; p < marker.pedidos.length; p++) {
                var pedido = marker.pedidos[p];
                if ((codigoRota > 0 && pedido.RotaFrete.Codigo == codigoRota) || (codigoLocalidade > 0 && ObterLocalidadePedido(pedido, marker.distribuidor) == codigoLocalidade)) {
                    if (verificarLimiteCapacidadeVeiculoPorPedido(pedido)) {
                        //marker.selecionado = true;
                        markersAdicionar.push(pedido);
                    }
                }
            }
        }

        var add = false;
        var pedidosJaInseridos = [];
        EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = true;
        markersAdicionar.map(function (markerAdd) {
            var pedido = ObterPedidoPorCodigo(markerAdd.Codigo);
            if (pedido.Selecionado === true || $.inArray(pedido.Codigo, pedidosJaInseridos) >= 0) return;
            
            pedidosJaInseridos.push(pedido.Codigo);
            SelecionarPedido(pedido);
            add = true;
        });
        EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = false;
        _precisarSetarPedidosSelecionadosTabelaPedidosSessao = true;
        PedidosSelecionadosChange();
        
        if (!add) 
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Cargas.MontagemCargaMapa.NaoFoiEncontradaNenhumaEntregaParaEssaConfiguracao);
        
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCargaMapa.Obrigatorio, Localization.Resources.Cargas.MontagemCargaMapa.ObrigatorioInformarRotaOuLocalidade);
    }
    setarPontosSelecionados(true);
}

function verificarLimiteCapacidadeVeiculoPorPedido(pedido) {
    if (_carregamento.ModeloVeicularCarga.codEntity() > 0) {
        var pesoPedido = Globalize.parseFloat(pedido.Peso);
        var palletsPedido = Globalize.parseFloat(pedido.TotalPallets);
        var cubagemPedido = Globalize.parseFloat(pedido.Cubagem);

        var capacidadePeso = Globalize.parseFloat(_carregamento.CapacidadePeso.val());
        var capacidadePallets = Globalize.parseFloat(_carregamento.CapacidadePallets.val());
        var capacidadeCubagem = Globalize.parseFloat(_carregamento.CapacidadeCubagem.val());

        var peso = Globalize.parseFloat(_carregamento.Peso.val());
        var pallets = Globalize.parseFloat(_carregamento.Pallets.val());
        var cubagem = Globalize.parseFloat(_carregamento.Cubagem.val());

        if ((peso + pesoPedido) > capacidadePeso)
            return false;

        if (capacidadePallets > 0) {
            if ((pallets + palletsPedido) > capacidadePallets)
                return false;
        }

        if (capacidadeCubagem > 0) {
            if ((cubagem + cubagemPedido) > capacidadeCubagem)
                return false;
        }
    }

    return true;

}

function LimparPontoMarcados() {
    for (var i = 0; i < _arrayMarker.length; i++) {
        var marker = _arrayMarker[i];
        marker.selecionado = false;
        _arrayMarker[i] = marker;
    }
    //Voltando a cor da linha para original removendo a cor preta de quando selecionada.
    resetColorPolylines();
    //ObterPontosPedidos();
}

function obterMarkersSelecionados() {
    var result = [];
    for (var i = 0; i < _arrayMarker.length; i++) {
        var marker = _arrayMarker[i];
        if (marker.selecionado === true)
            result.push(marker);
    }
    return result;
}

function AtualizarPontosFaltantes(index) {
    if (index == null) {
        _PedidosAtualizarPontos = new Array();
        _PedidosPontosNaoLocalizados = new Array();
        index = 0;
       
        //iniciarControleManualRequisicao();
    }

    if (_pedidosSemPontos != null && (index < _pedidosSemPontos.length)) {
        BuscarCoordenadas(_pedidosSemPontos[index], AtualizarPontosFaltantes, index);
    } else {
        AtualizarPontosObtidos();
    }
}

function AtualizarPontosObtidos() {
    if (_PedidosAtualizarPontos.length > 0) {
        var clientesPontos = new Array();
        for (var i = 0; i < _PedidosAtualizarPontos.length; i++) {
            var pedido = _PedidosAtualizarPontos[i];
            var cpfcnpj = "";
            PEDIDOS.update(function (ped) { return ped.Codigo == pedido.Codigo }, function (ped) {
                if (pedido.Distribuidor) {
                    cpfcnpj = ped.EnderecoRecebedor.Destinatario;
                    ped.EnderecoRecebedor.Latitude = pedido.Latitude;
                    ped.EnderecoRecebedor.Longitude = pedido.Longitude;
                }
                else if (pedido.DisponibilizarPedidoParaColeta) {
                    cpfcnpj = ped.EnderecoRemetente.Destinatario;
                    ped.Remetente.Latitude = pedido.Latitude;
                    ped.Remetente.Longitude = pedido.Longitude;
                }
                else {
                    cpfcnpj = ped.EnderecoDestino.Destinatario;
                    ped.EnderecoDestino.Latitude = pedido.Latitude;
                    ped.EnderecoDestino.Longitude = pedido.Longitude;
                }

                return ped;
            });
            clientesPontos.push({ CPFCNPJ: cpfcnpj, Latitude: pedido.Latitude, Longitude: pedido.Longitude });
        }
        var dados = { PessoasPontos: JSON.stringify(clientesPontos) };
        executarReST("Pessoa/AtualizarLatLngClientes", dados, function (arg) {
            if (!arg.Success) {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.falha);
            }
            finalizarControleManualRequisicao();
        });
        ObterPontosPedidos();
    } else {
        finalizarControleManualRequisicao();
    }

    for (var i = 0; i < _PedidosPontosNaoLocalizados.length; i++) {
        var pedido = _PedidosPontosNaoLocalizados[i];
        PEDIDOS.update(function (ped) { return ped.Codigo == pedido.Codigo }, function (ped) {
            ped.SemLatLng = true;
            return ped;
        });
    }
}

function BuscarCoordenadas(pedido, callback, index) {
    if (endereco == null)
        return; 

    var endereco = pedido.EnderecoDestino;
    if (pedido.DisponibilizarPedidoParaColeta) 
        endereco = pedido.EnderecoRemetente;

    var filtroRedespacho = _objPesquisaMontagem.GerarCargasDeRedespacho;
    var distribuidor = false;
    if (!filtroRedespacho && pedido.EnderecoRecebedor != null) {
        endereco = pedido.EnderecoRecebedor;
        distribuidor = true;
    }

    dadosEndereco = new DadosEndereco(endereco.Logradrouro, endereco.Numero, endereco.Localidade, '', endereco.CEP, '', '', '');

    new MapaGoogleGeoLocalizacao().buscarCoordenadas(dadosEndereco, function (resposta) {
        if (resposta.status === 'OK') {
            _PedidosAtualizarPontos.push({
                Codigo: pedido.Codigo,
                Latitude: resposta.latitude.toString(),
                Longitude: resposta.longitude.toString(),
                Distribuidor: distribuidor
            });
        } else {
            _PedidosPontosNaoLocalizados.push(pedido);
        }

        if (callback != null)
            callback(index + 1);
    });

    //var address = [];
    //if (endereco.Logradrouro != "")
    //    address.push(endereco.Logradrouro);

    //if (endereco.Numero != "" && endereco.Numero != "S/N")
    //    address.push(endereco.Numero);

    //address.push(endereco.Localidade);

    //if (endereco.CEP != "")
    //    address.push(endereco.CEP); 

    //_geocoder.geocode({ 'address': address.join(", ") }, function (results, status) {
    //    if (status === 'OK') {
    //        var latLng = results[0].geometry.location;
    //        _PedidosAtualizarPontos.push({
    //            Codigo: pedido.Codigo,
    //            Latitude: latLng.lat().toString(),
    //            Longitude: latLng.lng().toString(),
    //            Distribuidor: distribuidor
    //        });
    //    } else {
    //        _PedidosPontosNaoLocalizados.push(pedido);
    //    }

    //    if (callback != null)
    //        callback(index + 1);
    //});
}

function centralizarPedidoMapa(codigo) {
    var marker = otberMarkerPedidoCodigo(codigo);
    if (marker) {
        _map.setCenter(marker.marker.getPosition());
        new google.maps.event.trigger(marker.marker, 'mouseover');
    }
}