/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />

// #region Objetos Globais do Arquivo

var _posicaoAtualVeiculo;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PosicaoAtualVeiculo = function () {
    this.Mapa;
    this.Marcador;
    this.Geocodificador;

    this.Endereco = PropertyEntity({ text: "Endereço: " });
    this.UltimaAtualizacao = PropertyEntity({ text: "Última Atualização: " });
}

// #endregion Classes

// #region Funções de Inicialização

function loadHtmlPosicaoAtualVeiculo(callback) {
    $.get("Content/Static/Logistica/PosicaoAtualVeiculo.html?dyn=" + guid(), function (data) {
        var $containerModalPosicaoAtualVeiculo = $("#containerModalPosicaoAtualVeiculo");

        if ($containerModalPosicaoAtualVeiculo.length == 0) {
            $("#widget-grid").append("<div id='containerModalPosicaoAtualVeiculo'></div>");
            $containerModalPosicaoAtualVeiculo = $("#containerModalPosicaoAtualVeiculo");
        }

        $containerModalPosicaoAtualVeiculo.html(data);
        callback();
    });
}

function loadPosicaoAtualVeiculo() {
    loadHtmlPosicaoAtualVeiculo(function () {
        _posicaoAtualVeiculo = new PosicaoAtualVeiculo();
        KoBindings(_posicaoAtualVeiculo, "knockoutPosicaoAtualVeiculo");

        var opcoesmapa = {
            scaleControl: true,
            gestureHandling: 'greedy'
        };

        _posicaoAtualVeiculo.Mapa = new google.maps.Map(document.getElementById("mapa-posicao-atual-veiculo"), opcoesmapa);
        _posicaoAtualVeiculo.Geocodificador = new google.maps.Geocoder;
        _posicaoAtualVeiculo.Marcador = new google.maps.Marker({ map: _posicaoAtualVeiculo.Mapa });
    });
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function exibirPosicaoAtualVeiculoPorCarga(codigoCarga) {
    executarReST("PosicaoAtual/BuscarPorCarga", { Carga: codigoCarga }, exibirPosicaoAtualVeiculo);
}

function exibirPosicaoAtualVeiculoPorVeiculo(codigoVeiculo) {
    executarReST("PosicaoAtual/BuscarPorVeiculo", { Veiculo: codigoVeiculo }, exibirPosicaoAtualVeiculo);
}

// #endregion Funções Públicas

// #region Funções Privadas

function definirCoordenadasMapaPosicaoAtualVeiculo(posicao) {
    var zoom = posicao.zoom || 4;

    _posicaoAtualVeiculo.Mapa.setZoom(zoom);
    _posicaoAtualVeiculo.Mapa.setCenter(posicao);
    _posicaoAtualVeiculo.Marcador.setPosition(posicao);
}

function definirMarcadorMapaPosicaoAtualVeiculo(posicao) {
    _posicaoAtualVeiculo.Marcador.setMap(_posicaoAtualVeiculo.Mapa);
    _posicaoAtualVeiculo.Geocodificador.geocode({ 'location': posicao }, function (results, status) {
        if ((status === 'OK') && (results.length >= 0))
            _posicaoAtualVeiculo.Endereco.val(results[0].formatted_address);
    })
}

function exibirPosicaoAtualVeiculo(retorno) {
    LimparCampos(_posicaoAtualVeiculo);

    if (retorno.Success) {
        if (retorno.Data) {
            PreencherObjetoKnout(_posicaoAtualVeiculo, retorno);

            var posicao = {
                lat: retorno.Data.Latitude,
                lng: retorno.Data.Longitude,
                zoom: 16
            };

            definirCoordenadasMapaPosicaoAtualVeiculo(posicao);
            definirMarcadorMapaPosicaoAtualVeiculo(posicao);

            Global.abrirModal('divModalPosicaoAtualVeiculo');
        }
        else
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
    }
    else
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
}

// #endregion Funções Privadas
