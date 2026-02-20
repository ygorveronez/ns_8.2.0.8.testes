var _PracaPedagioAtualizarPontos;
var _PracaPedagioPontosNaoLocalizados;

function atualizarPontosObtidosPracaPedagio(callback) {
    if (_PracaPedagioAtualizarPontos.length > 0) {
        var pracaPedagiosPontos = new Array();
        for (var i = 0; i < _PracaPedagioAtualizarPontos.length; i++) {
            var pracaPedagio = _PracaPedagioAtualizarPontos[i];
            pracaPedagiosPontos.push({ Codigo: pracaPedagio.Codigo, Latitude: pracaPedagio.Latitude, Longitude: pracaPedagio.Longitude });
        }
        var dados = { PracasPontos: JSON.stringify(pracaPedagiosPontos) };
        executarReST("PracaPedagio/AtualizarLatLngPracas", dados, function (arg) {
            if (!arg.Success) {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.falha);
            }
            finalizarControleManualRequisicao();
            callback();
        });
    } else {
        finalizarControleManualRequisicao();
        callback();
    }
}

var _pracaPedagiosSemPontos;

function atualizarPontosFaltantesPracaPedagio(index, callback) {

    if (index == null) {
        _PracaPedagioAtualizarPontos = new Array();
        _PracaPedagioPontosNaoLocalizados = new Array();
        index = 0;
        iniciarControleManualRequisicao();
    }

    if (index < _pracaPedagiosSemPontos.length) {
        BuscarCoordenadasPracaPedagio(_pracaPedagiosSemPontos[index],
            function (idx) {
                atualizarPontosFaltantesPracaPedagio(idx, callback);
            },
            index);
    } else {
        atualizarPontosObtidosPracaPedagio(callback);
    }
}

function VerificarPracaPedagioSemLatLng(pracaPedagio) {
    if (pracaPedagio.Latitude == "" || pracaPedagio.Latitude == null || pracaPedagio.Longitude == "" || pracaPedagio.Longitude == null) {
        _pracaPedagiosSemPontos.push(pracaPedagio);
    }
}


function BuscarCoordenadasPracaPedagio(pracaPedagio, callback, index) {

    var address = pracaPedagio.Descricao;

    if (pracaPedagio.Rodovia !== null && pracaPedagio.Rodovia !== "")
        address += ", " + pracaPedagio.Rodovia;

    if (pracaPedagio.KM !== null && pracaPedagio.KM !== "")
        address += " - KM " + pracaPedagio.KM;

    dadosEndereco = new DadosEndereco(address, '', '', '', '', '', '', '', true);

    new MapaGoogleGeoLocalizacao().buscarCoordenadas(dadosEndereco, function (resposta) {
        if (resposta.status === 'OK') {
            pracaPedagio.Latitude = resposta.latitude.toString();
            pracaPedagio.Longitude = resposta.longitude.toString();
            _PracaPedagioAtualizarPontos.push(pracaPedagio);
        } else {
            _PracaPedagioPontosNaoLocalizados.push(pracaPedagio);
        }

        if (callback != null)
            callback(index + 1);
    });

    //_geocoder.geocode({ 'address': address }, function (results, status) {
    //    if (status === 'OK') {
    //        pracaPedagio.Latitude = results[0].geometry.location.lat().toString();
    //        pracaPedagio.Longitude = results[0].geometry.location.lng().toString();
    //        _PracaPedagioAtualizarPontos.push(pracaPedagio);
    //    } else {
    //        _PracaPedagioPontosNaoLocalizados.push(pracaPedagio);
    //    }

    //    if (callback != null)
    //        callback(index + 1);
    //});
}