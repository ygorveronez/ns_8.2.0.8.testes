function RespostaGeoLocalizacao(latitude, longitude, status) {
    this.latitude = latitude;
    this.longitude = longitude;
    this.status = status;
}

/*      Rua = 1,
        Avenida = 2,
        Rodovia = 3,
        Estrada = 4,
        Praca = 5,
        Outros = 99
*/
function obterTipoEndereco(tipoEndereco) {
    if (tipoEndereco == 2) {
        return 'AVENIDA ';
    } else if (tipoEndereco == 3) {
        return 'RODOVIA ';
    } else if (tipoEndereco == 4) {
        return 'ESTRADA ';
    } else if (tipoEndereco == 5) {
        return 'PRAÇA ';
    } else if (tipoEndereco == 6) {
        return 'TRAVESSA ';
    }
    return 'RUA ';
}

function DadosEndereco(endereco, numero, cidade, estado, cep, bairro, complemento, tipoEndereco, sempreGoogle) {
    this.endereco = endereco;
    this.numero = numero;
    this.cidade = cidade;
    this.estado = estado;
    this.bairro = bairro;
    this.cep = cep;
    this.complemento = complemento;
    this.TipoEndereco = tipoEndereco;
    this.SempreGoogle = sempreGoogle;
}

function MapaGoogleGeoLocalizacao() {
    var _geocoder = null;

    if (_CONFIGURACAO_TMS.Geocodificacao.GeoServiceGeocoding == 1) {
        // Nominatim
        _geocoder = new Nominatim(_CONFIGURACAO_TMS.Geocodificacao.NominatimServerUrl);
    } else {
        _geocoder = new google.maps.Geocoder();
    }

    this.buscarCoordenadas = function (dadosEndereco, callback) {

        //Nominatim em alguns casos não consegue geocodificar com o bairro...
        var bairro = '';
        var address = "";

        if (dadosEndereco.endereco)
            address += dadosEndereco.endereco + ", ";

        if ((dadosEndereco.numero) && (dadosEndereco.numero != "S/N") && (dadosEndereco.numero != "SN")) {

            if (dadosEndereco.complemento)
                address += dadosEndereco.numero + " " + dadosEndereco.complemento + ", ";
            else
                address += dadosEndereco.numero + ", ";
        }

        if (dadosEndereco.bairro) {
            bairro = "bairro " + dadosEndereco.bairro + ", ";
            address += bairro;
        }

        descricaoCidadeEstado = "";

        if (dadosEndereco.estado)
            descricaoCidadeEstado = dadosEndereco.estado + ", ";

        if (dadosEndereco.cidade)
            descricaoCidadeEstado = dadosEndereco.cidade + ", ";

        if ((dadosEndereco.cidade) && (dadosEndereco.estado))
            descricaoCidadeEstado = dadosEndereco.cidade + dadosEndereco.estado + ", ";

        address += descricaoCidadeEstado;

        if (dadosEndereco.cep)
            address += dadosEndereco.cep + "";

        if (_CONFIGURACAO_TMS.Geocodificacao.GeoServiceGeocoding == 1 && dadosEndereco.SempreGoogle !== true) {

            //Nominatim
            _geocoder.searchBackend(dadosEndereco, function (response) {
                //console.log(error, response);
                callback(response);
            });


            /*address = "";

            if (dadosEndereco.endereco) {
                if (dadosEndereco.endereco.startsWith("R "))
                    address += "RUA " + dadosEndereco.endereco.replace('R ', '') + ", ";
                else if (dadosEndereco.endereco.toUpperCase().startsWith("ROD "))
                    address += "RODOVIA " + dadosEndereco.endereco.toUpperCase().replace("ROD ", "").trim() + ", ";
                else if (dadosEndereco.endereco.toUpperCase().startsWith("AV ") || dadosEndereco.endereco.toUpperCase().startsWith("AV. "))
                    address += "AVENIDA " + dadosEndereco.endereco.toUpperCase().replace("AV ", "").replace("AV. ", "").trim() + ", ";
                else if (dadosEndereco.endereco.toUpperCase().indexOf("RUA") < 0 && dadosEndereco.endereco.toUpperCase().indexOf("AVENIDA") < 0 && dadosEndereco.endereco.toUpperCase().indexOf("Rodovia") < 0) {
                    address += obterTipoEndereco(dadosEndereco.TipoEndereco) + dadosEndereco.endereco.trim() + ", ";
                }
                else
                    address += dadosEndereco.endereco.trim() + ", ";

                // Problema geocodificar nominatim....
                if (address.toUpperCase().startsWith("RUA BC "))
                    address = address.replace("RUA BC ", "RUA BECO ");
                else if (address.toUpperCase().startsWith("BC "))
                    address = address.replace("BC ", "BECO ");
            }

            if ((dadosEndereco.numero) && (dadosEndereco.numero != "S/N") && (dadosEndereco.numero != "SN")) {
                // Não necessita nominatim...
                //if (dadosEndereco.complemento)
                //    address += dadosEndereco.numero + " " + dadosEndereco.complemento + ", ";
                //else
                address += dadosEndereco.numero + ", ";
            }

            if (dadosEndereco.bairro) {
                bairro = dadosEndereco.bairro.toUpperCase().replace("B ", "").replace("BAIRRO ", "").trim() + ", ";
                address += bairro;
            }

            descricaoCidadeEstado = "";

            if (dadosEndereco.cidade)
                descricaoCidadeEstado = dadosEndereco.cidade + ", ";

            if (dadosEndereco.estado)
                descricaoCidadeEstado = dadosEndereco.estado + ", ";

            address += descricaoCidadeEstado;

            // Se o endereço for null, vamos utilizar o CEP 
            if (!dadosEndereco.endereco && dadosEndereco.cep) {
                address += dadosEndereco.cep.replace(",", "").replace(".", "");
            }

            address = address.trim(); */
        } else {

            this.buscarCoordenadasEnderecoCompleto(address, bairro, function (resposta) {
                callback(resposta);
            });
        }
    }

    this.buscarCoordenadasEnderecoCompleto = function (endereco, bairro, callback) {

        if (_CONFIGURACAO_TMS.Geocodificacao.GeoServiceGeocoding == 1 && dadosEndereco.SempreGoogle !== true) {
            //Nominatim
            _geocoder.search(endereco, bairro, function (response) {
                //console.log(error, response);
                callback(response);
            });

        } else {
            _geocoder.geocode({ 'address': endereco }, function (results, status) {
                var resposta = "Erro";
                if (status === 'OK') {
                    resposta = new RespostaGeoLocalizacao(results[0].geometry.location.lat().toString(), results[0].geometry.location.lng().toString(), "OK");
                }
                else
                    resposta = new RespostaGeoLocalizacao(null, null, status);

                callback(resposta);

            });
        }
    }
}

