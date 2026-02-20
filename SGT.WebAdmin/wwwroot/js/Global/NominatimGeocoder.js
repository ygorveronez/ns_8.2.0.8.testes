// http://20.195.231.113:8080/nominatim/{0}?
//const endpoint = 'http://20.195.231.113:8080/nominatim/search';

function Nominatim(serverNominatim) {
    const endpoint = 'http://20.195.231.113:8080/nominatim/{0}?';

    var url = endpoint;
    if (serverNominatim)
        url = serverNominatim;

    url = url.format("search");

    this.search = function (endereco, bairro, callback) {
        console.log(endereco, bairro);
        const params = new URLSearchParams({
            q: endereco,
            format: 'json',
        });

        fetch(`${url}${params}`)
            .then(response => response.json())
            .then(data => {
                //console.log(data);
                var resposta = "Erro";
                if (data.length > 0) {
                    resposta = new RespostaGeoLocalizacao(data[0].lat, data[0].lon, "OK");
                } else {
                    if (bairro != '') {
                        endereco = endereco.replace(bairro, '');
                        return this.search(endereco, '', callback);
                    } else {
                        resposta = new RespostaGeoLocalizacao(null, null, "NotFound");
                    }
                }

                callback(resposta);
            })
            .catch(error => {
                //console.log(error);
                resposta = new RespostaGeoLocalizacao(null, null, "ErroNominatim");
                callback(resposta);
            });
    };

    this.searchBackend = function (dadosEndereco, callback) {
        var dados = {
            endereco: dadosEndereco.endereco
            , numero: dadosEndereco.numero
            , cidade: dadosEndereco.cidade
            , estado: dadosEndereco.estado
            , bairro: dadosEndereco.bairro
            , cep: dadosEndereco.cep
            , complemento: dadosEndereco.complemento
            , TipoLogradouro: dadosEndereco.TipoEndereco
            , SempreGoogle: dadosEndereco.SempreGoogle
            , pais: ''
        };

        return $.ajax({
            type: "POST",
            url: "Pessoa/GeocodificarEndereco",
            data: {
                DadosEndereco: JSON.stringify(dados)
            },
            dataType: 'json',
            success: function (retorno) {
                if (retorno.Success === true) {

                    var resposta = "Erro";
                    if (retorno.Data !== false) 
                        resposta = new RespostaGeoLocalizacao(retorno.Data.lat, retorno.Data.lon, "OK");
                    else 
                        resposta = new RespostaGeoLocalizacao(null, null, "NotFound");

                    callback(resposta);
                }
                else
                    callback(new RespostaGeoLocalizacao(null, null, "ErroNominatim"));
            },
            error: function (retorno) {
                callback(null, "Falha na requisição.");
            }
        });
    }
}