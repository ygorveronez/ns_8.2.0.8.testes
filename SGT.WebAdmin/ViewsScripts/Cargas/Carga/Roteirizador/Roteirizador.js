//// <autosync enabled="true" />
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

//*******MAPEAMENTO KNOUCKOUT*******

var _HTMLMapaRoteirizacao;
var _HTMLSimulacaoFrete;
var _roteirizador;
var _simulacaoFrete;
var _gridOrdemEntrega;
//var _gridReorder2;
var _ListaOrdenada;

var locations = [];
var pessoasRetorno;
var _mapaCarga = null;

var map;
var dir;
var _gridNarrativa;
var _gridBlocos;

var Roteirizador = function () {
    this.Map = PropertyEntity({ idGrid: guid(), visible: ko.observable(true), idGrid2: guid(), visibleReorder: ko.observable(false), visibleReorderClick: visibleReorderClick });
    this.SemPontos = PropertyEntity({ idGrid: guid(), visible: ko.observable(false) });
    this.Narativa = PropertyEntity({ idGrid: guid(), visible: ko.observable(false) });
    this.Carga = PropertyEntity();
    //this.Origem = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Origem : ", def: "", enable: false });
    this.Coletas = PropertyEntity({ val: ko.observable(""), options: ko.observable(new Array()), def: "", text: ko.observable("Origem : "), required: false, visible: ko.observable(true) });

    this.Distancia = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Distância : ", def: "", enable: false });
    this.TipoRota = PropertyEntity({ text: "Tipo da Rota: ", descricao: ko.observable("") });

    this.TipoUltimoPontoRoteirizacao = PropertyEntity({ text: "Último Ponto: ", descricao: ko.observable("") });
    this.Pessoas = PropertyEntity({ val: ko.observable(new Array()), type: types.map, getType: typesKnockout.dynamic });
    //this.Reordenar = PropertyEntity({ eventClick: reordenarPosicoesEntregas, type: types.event, text: "Reordenas", visible: ko.observable(true) });

    this.PolilinhaRota = PropertyEntity({});
    this.TempoDeViagemEmMinutos = PropertyEntity({});
    this.PontosDaRota = PropertyEntity({});

    this.Roteirizado = PropertyEntity({ visible: ko.observable(false) });
    this.ModoEdicao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });

    this.Blocos = PropertyEntity({ idGrid: guid() });

    this.Roteirizar = PropertyEntity({ eventClick: RoteirizarClick, type: types.event, text: "Buscar Rota", visible: ko.observable(false) });
    this.SalvarRota = PropertyEntity({ eventClick: salvarRotaClick, type: types.event, text: "Salvar Rota da Carga", visible: ko.observable(false) });
    this.AtualizarRota = PropertyEntity({ eventClick: atualizarRotaClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
}

function SimulacaoFrete() {
    this.Carga = PropertyEntity({ container: guid() });
    this.Distancia = PropertyEntity({ text: "Distância: ", val: ko.observable("") });
    this.ValorFrete = PropertyEntity({ text: "Valor Frete: ", val: ko.observable("") });
    this.ValorPorPeso = PropertyEntity({ text: "Valor por Quilo: ", val: ko.observable("") });
    this.PercentualSobValorMercadoria = PropertyEntity({ text: "Percentual sob a relação da mercadoria: ", val: ko.observable("") });
    this.ValorMercadoria = PropertyEntity({ text: "Valor Total da Mercadoria: ", val: ko.observable("") });
    this.PesoFrete = PropertyEntity({ text: "Peso Total: ", val: ko.observable("") });
}

function visibleReorderClick(e) {
    e.Map.visibleReorder(!e.Map.visibleReorder());
}

function loadRoteirizador() {
    var data = { Carga: _cargaAtual.Codigo.val() };
    executarReST("Roteirizador/BuscarDadosRoteirizacao", data, function (arg) {
        if (arg.Success) {
            if (arg.Data.roteirizado) {
                _mapaRenderizado = false;
                _roteirizador = new Roteirizador();
                _simulacaoFrete = new SimulacaoFrete();
                $("#tabRoteirizacao_" + _cargaAtual.DadosEmissaoFrete.id + "_li").show();
                $("#tabRoteirizacao_" + _cargaAtual.DadosEmissaoFrete.id).html(_HTMLMapaRoteirizacao.replace(/#Codigo/g, _roteirizador.Map.id));

                KoBindings(_roteirizador, "tabRoteirizacao_" + _cargaAtual.DadosEmissaoFrete.id);

                LocalizeCurrentPage();

                loadSimulacaoFrete();

                $("#Open_" + _roteirizador.Map.id + "_li").on("click", renderizarGoogleMapsClick);
                $("#Narativa_" + _roteirizador.Map.id + "_li").on("click", renderizarNarrativa);

                //var headHtml = '<tr><th width="5%"></th><th width="50%">Cliente</th><th width="45%">Cidade</th></tr>';
                var header = [
                    { data: "Ordem", visible: false },
                    { data: "Codigo", visible: false },
                    { data: "Cliente", title: "Cliente", width: "65%", className: "text-align-left", orderable: false },
                    { data: "Cidade", title: "Cidade", width: "35%", className: "text-align-left", orderable: false },
                ];
                _gridOrdemEntrega = new BasicDataTable(_roteirizador.Map.idGrid, header, null, { column: 0, dir: orderDir.asc });
                _gridOrdemEntrega.CarregarGrid([]);

                _roteirizador.Carga.val(_cargaAtual.Codigo.val());
                _roteirizador.TipoRota.descricao("");
                _roteirizador.TipoUltimoPontoRoteirizacao.descricao("");

                if (arg.Data.roteirizado) {
                    PreecherOrdemEntrega(arg.Data.rotasInformacaoPessoa);
                    _roteirizador.Distancia.val(FormataPadraoDistanciaKM(arg.Data.DistanciaKM));
                    _roteirizador.TipoRota.val(arg.Data.TipoRota);
                    _roteirizador.TipoRota.descricao(ObterDescricaoTipoRota(arg.Data.TipoRota));
                    _roteirizador.TipoUltimoPontoRoteirizacao.val(arg.Data.TipoUltimoPontoRoteirizacao);
                    _roteirizador.TipoUltimoPontoRoteirizacao.descricao(EnumTipoUltimoPontoRoteirizacao.obterDescricao(arg.Data.TipoUltimoPontoRoteirizacao));
                    _roteirizador.PolilinhaRota.val(arg.Data.PolilinhaRota);
                    _roteirizador.TempoDeViagemEmMinutos.val(arg.Data.TempoDeViagemEmMinutos);
                    _roteirizador.PontosDaRota.val(arg.Data.PontosDaRota);
                }

                _gridBlocos = new GridView(_roteirizador.Blocos.idGrid, "Roteirizador/BlocosCarga", _roteirizador, null, null, 10, null, null, null, null, 300);
                _gridBlocos.CarregarGrid(function () {
                    if (_gridBlocos.NumeroRegistros() > 0)
                        $("#Blocos_" + _roteirizador.Map.id + "_li").show();
                    else
                        $("#Blocos_" + _roteirizador.Map.id + "_li").hide();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function loadSimulacaoFrete() {
    $("#tabRoteirizacao_" + _cargaAtual.DadosEmissaoFrete.id + " #SimulacaoFrete_" + _roteirizador.Map.id).html(_HTMLSimulacaoFrete.replace(/#Codigo/g, _simulacaoFrete.Carga.container));
    KoBindings(_simulacaoFrete, "SimulacaoFrete_" + _roteirizador.Map.id);

    LocalizeCurrentPage();

    var data = { Carga: _cargaAtual.Codigo.val() };
    executarReST("Roteirizador/BuscarSimulacaoFrete", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                PreencherObjetoKnout(_simulacaoFrete, arg);
                $("#SimulacaoFrete_" + _roteirizador.Map.id + "_li").show();
            } else {
                $("#SimulacaoFrete_" + _roteirizador.Map.id + "_li").hide();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}



function salvarRotaClick() {
    if (verificarMudouPosicao()) {
        renderizarMapaRequest(function () {
            salvarRota();
            _mapaRenderizado = false;
        });
    } else {
        salvarRota();
    }
}

function salvarRota() {
    var pessoasOrdenadas = [];
    reordenarPosicoesEntregas();
    pessoasOrdenadas.push({ CPFCNPJ: pessoasRetorno[0].pessoa.CPFCNPJ });
    for (var i = 0; i < _pessoasReordenadas.length; i++) {
        pessoasOrdenadas.push({ CPFCNPJ: _pessoasReordenadas[i].pessoa.CPFCNPJ });
    }
    _roteirizador.Pessoas.val(JSON.stringify(pessoasOrdenadas));
    Salvar(_roteirizador, "Roteirizador/SalvarRotaCarga", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Roteirização salva com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}


function desenharPolilinhaCarga() {
    _mapaCarga = new Mapa(_roteirizador.Map.id, false);

    setTimeout(function () {

        _mapaCarga.desenharPolilinha(_roteirizador.PolilinhaRota.val(), true);
        _mapaCarga.adicionarMarcadorComPontosDaRota(_roteirizador.PontosDaRota.val());

    }, 300);
}

function renderizarGoogleMapsClick() {
    if (_roteirizador.PolilinhaRota.val() != "" && _roteirizador.PolilinhaRota.val() != null)
        desenharPolilinhaCarga();
    else
        renderizarGoogleMaps();
}

function renderizarMapaRequestClick() {
    renderizarMapaRequest();
}

function atualizarRotaClick() {

    //var html = $("#" + _roteirizador.Map.idGrid2 + "tbody").html();
    //_gridOrdemEntrega.RecarregarGrid(html);
    //var listaOrdenada = _gridReorder2.ObterOrdencao();
    //locations = reordenarPosicoesEntregas(_gridReorder2);
    roterizarGoogleMaps();

}

function renderizarNarrativa() {
    renderizarGoogleMaps();
}

var _mapaRenderizado = false;
var _pessoasReordenadas;

function reordenarPosicoesEntregas(grid) {

    if (grid == null)
        _ListaOrdenada = _gridOrdemEntrega.ObterOrdencao();
    else
        _ListaOrdenada = grid.ObterOrdencao();

    _pessoasReordenadas = [];
    var locationsReorder = [];
    locationsReorder.push(locations[0]);
    $.each(_ListaOrdenada, function (i, ordem) {
        locationsReorder.push(locations[ordem.id.split("_")[1]]);
        _pessoasReordenadas.push(pessoasRetorno[ordem.id.split("_")[1]]);
    });
    if (_roteirizador.TipoUltimoPontoRoteirizacao.val() == EnumTipoUltimoPontoRoteirizacao.AteOrigem) {
        locationsReorder.push(locations[locations.length - 1]);
    }
    return locationsReorder;
}

function verificarMudouPosicao() {
    var mudou = false;
    var listaOrdenada = _gridOrdemEntrega.ObterOrdencao();
    if (_ListaOrdenada != null) {
        $.each(_ListaOrdenada, function (i, ordem) {
            if (ordem.id != listaOrdenada[i].id) {
                _mapaRenderizado = false;
                mudou = true;
                return false;
            }
        });
    }
    return mudou;
}

function ObterDescricaoTipoRota(tipo) {
    switch (tipo) {
        case "fastest": return "Mais Rápida";
        case "shortest": return "Menor distância";
        default: return "";
    }
}

function PreecherOrdemEntrega(retorno) {
    _ListaOrdenada = null;
    _mapaRenderizado = false;
    pessoasRetorno = retorno;

    var montaObjLocation = function (rotaPessoa) {
        return {
            latLng: { lat: rotaPessoa.coordenadas.latitude, lng: rotaPessoa.coordenadas.longitude },
            pessoa: rotaPessoa.pessoa,
            coordenadas: rotaPessoa.coordenadas
        }
    }

    var CoordenadasValidas = function (rotaPessoa) {
        var latLng = { lat: rotaPessoa.coordenadas.latitude, lng: rotaPessoa.coordenadas.longitude };

        if (rotaPessoa.coordenadas.tipoLocalizacao != EnumTipoLocalizacao.naoEncontrado)
            return true;

        if (latLng.lat == "" || latLng.lat == null)
            return false;

        if (latLng.lng == "" || latLng.lng == null)
            return false;

        return true;
    }

    locations = [];

    var dadosEntrega = [];
    var pessoasNaoEncontradas = [];
    var distancia = 0;
    var deferenteAteOrigem = _roteirizador.TipoUltimoPontoRoteirizacao.val() != EnumTipoUltimoPontoRoteirizacao.AteOrigem;

    for (var i = 0; i < retorno.length; i++) {
        var rotaPessoa = retorno[i];

        distancia += rotaPessoa.distancia;

        if (CoordenadasValidas(rotaPessoa)) {
            locations.push(montaObjLocation(rotaPessoa));

            if (i == 0) {
                var coletas = new Array();
                coletas.push({ text: retorno[0].pessoa.RazaoSocial + " (" + retorno[0].pessoa.Endereco.Cidade.Descricao + ' - ' + retorno[0].pessoa.Endereco.Cidade.SiglaUF + ")", value: retorno[0].pessoa.Codigo });
                _roteirizador.Coletas.options(coletas);
                //_roteirizador.Origem.val(rotaPessoa.pessoa.RazaoSocial + " (" + rotaPessoa.pessoa.Endereco.Cidade.Descricao + ' - ' + rotaPessoa.pessoa.Endereco.Cidade.SiglaUF + ")")
            }
            else if (deferenteAteOrigem || i != (retorno.length - 1)) {
                dadosEntrega.push({
                    Ordem: i,
                    Codigo: rotaPessoa.pessoa.CPFCNPJ,
                    Cliente: (rotaPessoa.pessoa.Codigo ? rotaPessoa.pessoa.Codigo + " - " : "") + rotaPessoa.pessoa.RazaoSocial,
                    Cidade: rotaPessoa.pessoa.Endereco.Cidade.Descricao + ' - ' + rotaPessoa.pessoa.Endereco.Cidade.SiglaUF
                });
            }
        }
        else {
            pessoasNaoEncontradas.push({
                Codigo: rotaPessoa.pessoa.CPFCNPJ,
                CPFCNPJ: rotaPessoa.pessoa.CPFCNPJ,
                RazaoSocial: rotaPessoa.pessoa.RazaoSocial,
                Localidade: rotaPessoa.pessoa.Endereco.Cidade.Descricao + ' - ' + rotaPessoa.pessoa.Endereco.Cidade.SiglaUF
            });
        }
    }

    if (pessoasNaoEncontradas.length == 0) {
        _roteirizador.Map.visible(true);
        _roteirizador.SemPontos.visible(false);
        _roteirizador.Distancia.val(FormataPadraoDistanciaKM(distancia));
        _roteirizador.Roteirizado.visible(true);

        _gridOrdemEntrega.CarregarGrid(dadosEntrega);
    }
    else {
        exibirGridPontosNaoEncontrados(pessoasNaoEncontradas);

        _roteirizador.SalvarRota.visible(false);
        _roteirizador.Roteirizado.visible(false);
    }
}

//function PreecherOrdemEntrega(retorno) {
//    _ListaOrdenada = null;
//    _mapaRenderizado = false;
//    pessoasRetorno = retorno;

//    var montaObjLocation = function (rotaPessoa) {
//        return {
//            latLng: { lat: rotaPessoa.coordenadas.latitude, lng: rotaPessoa.coordenadas.longitude },
//            pessoa: rotaPessoa.pessoa,
//            coordenadas: rotaPessoa.coordenadas
//        }
//    }

//    var CoordenadasValidas = function (rotaPessoa) {
//        var latLng = { lat: rotaPessoa.coordenadas.latitude, lng: rotaPessoa.coordenadas.longitude };

//        if (rotaPessoa.coordenadas.tipoLocalizacao != EnumTipoLocalizacao.naoEncontrado)
//            return true;

//        if (latLng.lat == "" || latLng.lat == null)
//            return false;

//        if (latLng.lng == "" || latLng.lng == null)
//            return false;

//        return true;
//    }

//    var naoEncontradas = false;
//    var pessoasNaoEncontradas = [];
//    var indexInicialLocations = 1;
//    locations = [];

//    if (retorno[0].coordenadas.tipoLocalizacao != EnumTipoLocalizacao.naoEncontrado) {
//        _roteirizador.Origem.val(retorno[0].pessoa.RazaoSocial + " (" + retorno[0].pessoa.Endereco.Cidade.Descricao + ' - ' + retorno[0].pessoa.Endereco.Cidade.SiglaUF + ")")
//        locations.push(montaObjLocation(retorno[0]));
//    } else {
//        indexInicialLocations = 0;
//        naoEncontradas = true;
//    }

//    var dadosEntrega = [];
//    var distancia = 0;
//    var deferenteAteOrigem = _roteirizador.TipoUltimoPontoRoteirizacao.val() != EnumTipoUltimoPontoRoteirizacao.AteOrigem;

//    for (var i = indexInicialLocations; i < retorno.length; i++) {
//        var rotaPessoa = retorno[i];
//        distancia += rotaPessoa.distancia;

//        if (i != 0) {
//            //if (rotaPessoa.coordenadas.tipoLocalizacao != EnumTipoLocalizacao.naoEncontrado) {
//            if (CoordenadasValidas(rotaPessoa)) {
//                locations.push(montaObjLocation(rotaPessoa));

//                if (deferenteAteOrigem || i != (retorno.length - 1)) {
//                    dadosEntrega.push({
//                        Ordem: i,
//                        Codigo: rotaPessoa.pessoa.CPFCNPJ,
//                        Cliente: (rotaPessoa.pessoa.Codigo ? rotaPessoa.pessoa.Codigo + " - " : "") + rotaPessoa.pessoa.RazaoSocial,
//                        Cidade: rotaPessoa.pessoa.Endereco.Cidade.Descricao + ' - ' + rotaPessoa.pessoa.Endereco.Cidade.SiglaUF
//                    });
//                }
//            } else {
//                naoEncontradas = true;
//                pessoasNaoEncontradas.push({
//                    Codigo: rotaPessoa.pessoa.CPFCNPJ,
//                    CPFCNPJ: rotaPessoa.pessoa.CPFCNPJ,
//                    RazaoSocial: rotaPessoa.pessoa.RazaoSocial,
//                    Localidade: rotaPessoa.pessoa.Endereco.Cidade.Descricao + ' - ' + rotaPessoa.pessoa.Endereco.Cidade.SiglaUF
//                });
//            }
//        }
//    }
//    if (!naoEncontradas) {
//        _roteirizador.Map.visible(true);
//        _roteirizador.SemPontos.visible(false);
//        _roteirizador.Distancia.val(FormataPadraoDistanciaKM(distancia));
//        //_roteirizador.SalvarRota.visible(true);
//        _roteirizador.Roteirizado.visible(true);
//        _gridOrdemEntrega.CarregarGrid(dadosEntrega);
//        //_gridReorder2.RecarregarGrid(html);
//    } else {
//        exibirGridPontosNaoEncontrados(pessoasNaoEncontradas);
//        _roteirizador.SalvarRota.visible(false);
//        _roteirizador.Roteirizado.visible(false);
//    }
//}

function RoteirizarClick(e) {
    //var data = { Carga: _cargaAtual.Codigo.val(), TipoUltimoPontoRoteirizacao: _roteirizador.TipoUltimoPontoRoteirizacao.val(), TipoRota: _roteirizador.TipoRota.val() }
    //executarReST("GoogleMaps/CriarRotaCarga", data, function (arg) {
    //    if (arg.Success) {
    //        if (arg.Data !== false) {
    //            PreecherOrdemEntrega(arg.Data);
    //        } else {
    //            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
    //        }
    //    } else {
    //        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    //    }

    //});
}

function exibirGridPontosNaoEncontrados(pessoasNaoEncontradas) {
    exibirMensagem(tipoMensagem.atencao, "Pontos não encontrados", "Não foi possivel roteirizar a carga, pois alguns pontos não foram encontrados, verifique a lista e ajuste manualmente.");
    var header = [
        { data: "Codigo", visible: false },
        { data: "CPFCNPJ", title: "CNPJ/CPF", width: "25%", className: "text-align-left", orderable: false },
        { data: "RazaoSocial", title: "Pessoa", width: "50%", className: "text-align-left", orderable: true },
        { data: "Localidade", title: "Localidade", width: "25%", className: "text-align-left", orderable: true }
    ];
    var gridSemPontos = new BasicDataTable(_roteirizador.SemPontos.idGrid, header, null);
    gridSemPontos.CarregarGrid(pessoasNaoEncontradas);

    _roteirizador.Map.visible(false);
    _roteirizador.SemPontos.visible(true);
}

function setarRota(locations) {
    dir.route({
        locations: locations,
        options: {
            unit: "k",
            routeType: _roteirizador.TipoRota.val(),
            locale: "pt"
        }
    });
    map.addLayer(new CustomRouteLayer({
        directions: dir,
        fitBounds: true,
        draggable: false,
        ribbonOptions: {
            draggable: false,
            ribbonDisplay: { opacity: 0.3 }
        }
    }));
}

function preencherRetornoRoteirizacao(responseArray, indexUltimoPonto) {
    var html = "";
    var pessoas = [];
    var total = 0;
    var distancia = 0;

    for (var j = 0; j < responseArray.length; j++) {
        pessoas.push(locations[total]);
        retorno = responseArray[j];

        for (var i = 0; i < retorno.routes[0].waypoint_order.length; i++) {
            var index = (retorno.routes[0].waypoint_order[i] + 1) + total;
            if (index >= indexUltimoPonto && indexUltimoPonto > 0)
                index++;
            pessoas.push(locations[index]);
        }

        for (var i = 0; i < retorno.routes[0].legs.length; i++) {
            distancia += retorno.routes[0].legs[i].distance.value;
        }

        total += retorno.routes[0].waypoint_order.length + 1;
    }

    if (indexUltimoPonto > 0)
        pessoas.push(locations[indexUltimoPonto]);

    //_roteirizador.Origem.val(pessoas[0].pessoa.RazaoSocial + " (" + pessoas[0].pessoa.Endereco.Cidade.Descricao + ' - ' + pessoas[0].pessoa.Endereco.Cidade.SiglaUF + ")");

    locations = pessoas;

    _roteirizador.SemPontos.visible(false);
    _roteirizador.Distancia.val(FormataPadraoDistanciaKM(distancia / 1000));
}

function FormataPadraoDistanciaKM(distancia) {
    return Globalize.format(Math.floor(distancia), "n0") + " KM";
}