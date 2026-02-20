var _ClienteAtualizarPontos;
var _ClientePontosNaoLocalizados;


//var _geocoder;
//function loadGeoLocalizacao() {
//    //_geocoder = new google.maps.Geocoder();
//}

function atualizarPontosObtidos(callback) {
    if (_ClienteAtualizarPontos.length > 0) {
        var clientesPontos = new Array();
        for (var i = 0; i < _ClienteAtualizarPontos.length; i++) {
            var cliente = _ClienteAtualizarPontos[i];
            clientesPontos.push({ CPFCNPJ: cliente.Codigo, Latitude: cliente.Latitude, Longitude: cliente.Longitude });
        }
        var dados = { PessoasPontos: JSON.stringify(clientesPontos) };
        executarReST("Pessoa/AtualizarLatLngClientes", dados, function (arg) {
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

var _clientesSemPontos;

function AtualizarPontosFaltantes(index, callback) {

    if (index == null) {
        _ClienteAtualizarPontos = new Array();
        _ClientePontosNaoLocalizados = new Array();
        index = 0;
        iniciarControleManualRequisicao();
    }

    if (index < _clientesSemPontos.length) {
        BuscarCoordenadas(_clientesSemPontos[index],
            function (idx) {
                AtualizarPontosFaltantes(idx, callback);
            },
            index);
    } else {
        atualizarPontosObtidos(callback);
    }
}

function VerificarClienteSemLatLngDestinatario(cliente) {
    if (cliente.Latitude == "" || cliente.Latitude == null || cliente.Longitude == "" || cliente.Longitude == null) {
        _clientesSemPontos.push(cliente);
    }
}


function BuscarCoordenadas(cliente, callback, index) {

    dadosEndereco = new DadosEndereco(cliente.Endereco, cliente.Numero, cliente.Localidade, "", cliente.CEP, "", "", "");

    new MapaGoogleGeoLocalizacao().buscarCoordenadas(dadosEndereco, function (resposta) {

        if (resposta.status === 'OK') {
            cliente.Latitude = resposta.latitude.toString();
            cliente.Longitude = resposta.longitude.toString();
            _ClienteAtualizarPontos.push(cliente);
        } else {
            _ClientePontosNaoLocalizados.push(cliente);
        }

        if (callback != null)
            callback(index + 1);
    });

    //var address = "";
    //if (cliente.Endereco != "") {
    //    address += cliente.Endereco + ", ";
    //}

    //if (cliente.Numero != "" && cliente.Numero != "S/N") {
    //    address += cliente.Numero + ", ";
    //}

    //address += cliente.Localidade + ", ";


    //if (cliente.CEP != "") {
    //    address += cliente.CEP;
    //}

    //_geocoder.geocode({ 'address': address }, function (results, status) {
    //    if (status === 'OK') {
    //        cliente.Latitude = results[0].geometry.location.lat().toString();
    //        cliente.Longitude = results[0].geometry.location.lng().toString();
    //        _ClienteAtualizarPontos.push(cliente);
    //    } else {
    //        _ClientePontosNaoLocalizados.push(cliente);
    //    }

    //    if (callback != null)
    //        callback(index + 1);
    //});
}