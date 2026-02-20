////// <autosync enabled="true" />
///// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
///// <reference path="../../../../../js/Global/CRUD.js" />
///// <reference path="../../../../../js/Global/knockout-3.1.0.js" />
///// <reference path="../../../../../js/Global/Rest.js" />
///// <reference path="../../../../../js/Global/Mensagem.js" />
///// <reference path="../../../../../js/Global/Grid.js" />
///// <reference path="../../../../../js/bootstrap/bootstrap.js" />
///// <reference path="../../../../../js/libs/jquery.blockui.js" />
///// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
///// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
///// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
///// <reference path="../../../../../js/libs/jquery.twbsPagination.js" />
///// <reference path="../../../../../js/libs/jquery.globalize.js" />
///// <reference path="../../../../../js/libs/jquery.globalize.pt-BR.js" />
///// <reference path="../../../../Global/SignalR/SignalR.js" />
///// <reference path="../../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
///// <reference path="../../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
///// <reference path="../../DadosEmissao/Configuracao.js" />
///// <reference path="../../DadosEmissao/DadosEmissao.js" />
///// <reference path="../../DadosEmissao/Geral.js" />
///// <reference path="../../DadosEmissao/Lacre.js" />
///// <reference path="../../DadosEmissao/LocaisPrestacao.js" />
///// <reference path="../../DadosEmissao/Observacao.js" />
///// <reference path="../../DadosEmissao/Passagem.js" />
///// <reference path="../../DadosEmissao/Percurso.js" />
///// <reference path="../../DadosEmissao/Rota.js" />
///// <reference path="../../DadosEmissao/Seguro.js" />
///// <reference path="../../DadosTransporte/DadosTransporte.js" />
///// <reference path="../../DadosTransporte/Motorista.js" />
///// <reference path="../../DadosTransporte/Tipo.js" />
///// <reference path="../../DadosTransporte/Transportador.js" />
///// <reference path="../../Documentos/CTe.js" />
///// <reference path="../../Documentos/MDFe.js" />
///// <reference path="../../Documentos/NFS.js" />
///// <reference path="../../Documentos/PreCTe.js" />
///// <reference path="../../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
///// <reference path="../../DocumentosEmissao/ConsultaReceita.js" />
///// <reference path="../../DocumentosEmissao/CTe.js" />
///// <reference path="../../DocumentosEmissao/Documentos.js" />
///// <reference path="../../DocumentosEmissao/DropZone.js" />
///// <reference path="../../DocumentosEmissao/EtapaDocumentos.js" />
///// <reference path="../../DocumentosEmissao/NotaFiscal.js" />
///// <reference path="../../Frete/Complemento.js" />
///// <reference path="../../Frete/Componente.js" />
///// <reference path="../../Frete/EtapaFrete.js" />
///// <reference path="../../Frete/Frete.js" />
///// <reference path="../../Frete/SemTabela.js" />
///// <reference path="../../Frete/TabelaCliente.js" />
///// <reference path="../../Frete/TabelaComissao.js" />
///// <reference path="../../Frete/TabelaRota.js" />
///// <reference path="../../Frete/TabelaSubContratacao.js" />
///// <reference path="../../Frete/TabelaTerceiros.js" />
///// <reference path="../../Impressao/Impressao.js" />
///// <reference path="../../Integracao/Integracao.js" />
///// <reference path="../../Integracao/IntegracaoCarga.js" />
///// <reference path="../../Integracao/IntegracaoCTe.js" />
///// <reference path="../../Integracao/IntegracaoEDI.js" />
///// <reference path="../../../Terceiro/ContratoFrete.js" />
///// <reference path="../../../DadosCarga/Carga.js" />
///// <reference path="../../../DadosCarga/DataCarregamento.js" />
///// <reference path="../../../DadosCarga/Leilao.js" />
///// <reference path="../../../DadosCarga/Operador.js" />
///// <reference path="../../../DadosCarga/SignalR.js" />
///// <reference path="../../../../Consultas/Tranportador.js" />
///// <reference path="../../../../Consultas/Localidade.js" />
///// <reference path="../../../../Consultas/ModeloVeicularCarga.js" />
///// <reference path="../../../../Consultas/TipoCarga.js" />
///// <reference path="../../../../Consultas/Motorista.js" />
///// <reference path="../../../../Consultas/Veiculo.js" />
///// <reference path="../../../../Consultas/GrupoPessoa.js" />
///// <reference path="../../../../Consultas/TipoOperacao.js" />
///// <reference path="../../../../Consultas/Filial.js" />
///// <reference path="../../../../Consultas/Cliente.js" />
///// <reference path="../../../../Consultas/Usuario.js" />
///// <reference path="../../../../Consultas/TipoCarga.js" />
///// <reference path="../../../../Consultas/RotaFrete.js" />
///// <reference path="../../../../Enumeradores/EnumSituacoesCarga.js" />
///// <reference path="../../../../Enumeradores/EnumTipoFreteEscolhido.js" />
///// <reference path="../../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
///// <reference path="../../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
///// <reference path="../../../../Enumeradores/EnumTipoContratacaoCarga.js" />
///// <reference path="../../../../Enumeradores/EnumSituacaoContratoFrete.js" />
///// <reference path="../../../../Enumeradores/EnumStatusCTe.js" />
///// <reference path="../../../../Enumeradores/EnumTipoPagamento.js" />
///// <reference path="../../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
///// <reference path="../../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />
///// <reference path="../../../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />
///// <reference path="../../../../Enumeradores/EnumTipoIntegracao.js" />
///// <reference path="../../../../Enumeradores/EnumFatorConsultaAvon.js" />
///// <reference path="../../../../Enumeradores/EnumSituacaoMinutaAvon.js" />
///// <reference path="../../../../Enumeradores/EnumTipoLocalizacao.js" />
///// <reference path="../../../../Enumeradores/EnumTipoUltimoPontoRoteirizacao.js" />

////*******MAPEAMENTO KNOUCKOUT*******

//var _HTMLMapaRoteirizacao;
//var _roteirizadorMapRequest;
//var _gridReorder;
//var _ListaMapRequestOrdenada;

//var locations = new Array();
//var pessoasRetorno;

//var map, dir;
//var latLngDefault = { lat: -10.861639, lng: -53.104038 };
//var _gridNarrativa;
//var _tiposRota = [
//    { text: "Mais Rápida", value: "fastest" },
//    { text: "Menor distância", value: "shortest" }
//];

//var RoteirizadorMapRequest = function () {
//    this.Map = PropertyEntity({ idGrid: guid(), visible: ko.observable(true) });
//    this.SemPontos = PropertyEntity({ idGrid: guid(), visible: ko.observable(false) });
//    this.Narativa = PropertyEntity({ idGrid: guid(), visible: ko.observable(false) });
//    this.Carga = PropertyEntity();
//    this.Origem = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Origem : ", def: "", enable: false });

//    this.Distancia = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Distância : ", def: "", enable: false });

//    this.TipoRota = PropertyEntity({ val: ko.observable("fastest"), options: _tiposRota, def: "fastest", text: "Tipo da Rota: ", required: true });

//    this.TipoUltimoPontoRoteirizacao = PropertyEntity({ val: ko.observable(EnumTipoUltimoPontoRoteirizacao.Retornando), options: EnumTipoUltimoPontoRoteirizacao.obterOpcoes(), def: EnumTipoUltimoPontoRoteirizacao.Retornando, text: "Ultimo Ponto: ", required: true });
//    this.Pessoas = PropertyEntity({ val: ko.observable(new Array()), type: types.map, getType: typesKnockout.dynamic });
//    //this.Reordenar = PropertyEntity({ eventClick: reordenarPosicoesEntregas, type: types.event, text: "Reordenas", visible: ko.observable(true) });
//    this.RoterizarViaMapRequest = PropertyEntity({ eventClick: RoterizarViaMapRequestClick, type: types.event, text: "Buscar Rota (mapquest)", visible: ko.observable(true) });
//    this.SalvarRota = PropertyEntity({ eventClick: salvarRotaClick, type: types.event, text: "Salvar Rota da Carga", visible: ko.observable(false) });
//}

//function loadRoteirizadorMapRequest() {
//    var data = { Carga: _cargaAtual.Codigo.val() };
//    executarReST("MapRequest/BuscarDadosRoteirizacao", data, function (arg) {
//        if (arg.Success) {
//            _mapaRenderizado = false;
//            _roteirizadorMapRequest = new RoteirizadorMapRequest();
//            $("#tabRoteirizacaoMapRequest_" + _cargaAtual.DadosEmissaoFrete.id + "_li").show();
//            $("#tabRoteirizacaoMapRequest_" + _cargaAtual.DadosEmissaoFrete.id).html(_HTMLMapaRoteirizacao.replace(/#CodigoMapRequest/g, _roteirizadorMapRequest.Map.id));
//            KoBindings(_roteirizadorMapRequest, "tabRoteirizacaoMapRequest_" + _cargaAtual.DadosEmissaoFrete.id);
//            $("#OpenMapRequest_" + _roteirizadorMapRequest.Map.id + "_li").on("click", renderizarMapaRequestClick);
//            $("#Narativa_" + _roteirizadorMapRequest.Map.id + "_li").on("click", renderizarNarrativaMapaRequest);

//            var headHtml = '<tr><th width="5%"></th><th width="50%">Cliente</th><th width="45%">Cidade</th></tr>';
//            _gridReorder = new GridReordering("Mova as linhas conforme a prefêrencia de prioridades", _roteirizadorMapRequest.Map.idGrid, headHtml, "");
//            _gridReorder.CarregarGrid();

//            _roteirizadorMapRequest.Carga.val(_cargaAtual.Codigo.val());
//            var header = [
//                      { data: "Codigo", visible: false },
//                      { data: "iconUrl", title: "", width: "10%", className: "text-align-left", orderable: false },
//                      { data: "narrative", title: "Narrativa", width: "70%", className: "text-align-left", orderable: false },
//                      { data: "distance", title: "Distância (km)", width: "20%", className: "text-align-left", orderable: false }
//            ];
//            _gridNarrativa = new BasicDataTable(_roteirizadorMapRequest.Narativa.idGrid, header, null, { column: 0, dir: orderDir.asc });
//            _gridNarrativa.CarregarGrid(new Array());

//            if (arg.Data.roteirizado) {
//                PreecherOrdemEntrega(arg.Data.rotasInformacaoPessoa);
//                _roteirizadorMapRequest.Distancia.val(Globalize.format(arg.Data.DistanciaKM, "n2") + " km");
//                _roteirizadorMapRequest.TipoRota.val(arg.Data.TipoRota);
//                _roteirizadorMapRequest.TipoUltimoPontoRoteirizacao.val(arg.Data.TipoUltimoPontoRoteirizacao);
//            }
//        } else {
//            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//        }
//    });
//}

//function salvarRotaClick() {
//    if (verificarMudouPosicao()) {
//        renderizarMapaRequest(function () {
//            salvarRota();
//            _mapaRenderizado = false;
//        });
//    } else {
//        salvarRota();
//    }
//}

//function salvarRota() {
//    var pessoasOrdenadas = new Array();
//    reordenarPosicoesEntregas();
//    pessoasOrdenadas.push({ CPFCNPJ: pessoasRetorno[0].pessoa.CPFCNPJ });
//    for (var i = 0; i < _pessoasReordenadas.length; i++) {
//        pessoasOrdenadas.push({ CPFCNPJ: _pessoasReordenadas[i].pessoa.CPFCNPJ });
//    }
//    _roteirizadorMapRequest.Pessoas.val(JSON.stringify(pessoasOrdenadas));
//    Salvar(_roteirizadorMapRequest, "MapRequest/SalvarRotaCarga", function (arg) {
//        if (arg.Success) {
//            if (arg.Data !== false) {
//                exibirMensagem(tipoMensagem.ok, "Sucesso", "Roteirização salva com sucesso.");
//            } else {
//                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
//            }
//        } else {
//            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//        }
//    });
//}

//function renderizarMapaRequestClick() {
//    renderizarMapaRequest();
//}

//function renderizarNarrativaMapaRequest() {

//    renderizarMapaRequest(function () {
//        _mapaRenderizado = false;
//    })
//}

//var _mapaRenderizado = false;
//var _pessoasReordenadas;

//function renderizarMapaRequest(callbackRender) {
//    verificarMudouPosicao();
//    if (!_mapaRenderizado) {
//        iniciarRequisicao();
//        if (map != null) {
//            if ($("#" + map._container.id)[0] != null) {
//                map.remove();
//            }
//            map = null;
//        };
//        setTimeout(function () {
//            map = L.map(_roteirizadorMapRequest.Map.id, {
//                layers: MQ.mapLayer(),
//                center: latLngDefault,
//                zoom: 9
//            });
//            dir = MQ.routing.directions().on('success', function (data) {
//                var legs = data.route.legs;
//                var distancia = 0;
//                if (legs && legs.length) {
//                    _mapaRenderizado = true;

//                    var narrativas = new Array();
//                    var codigo = 1;
//                    for (var i = 0; i < legs.length; i++) {
//                        distancia += legs[i].distance;
//                        for (var j = 0; j < legs[i].maneuvers.length; j++) {
//                            narrativas.push({ Codigo: codigo, iconUrl: "<img src='" + legs[i].maneuvers[j].iconUrl + "'/>", narrative: legs[i].maneuvers[j].narrative, distance: Globalize.format(legs[i].maneuvers[j].distance, "n2") });
//                            codigo++;
//                        }
//                    }
//                    _roteirizadorMapRequest.Distancia.val(Globalize.format(distancia, "n2") + " km");
//                    _gridNarrativa.CarregarGrid(narrativas);

//                    if (callbackRender != null)
//                        callbackRender();

//                    finalizarRequisicao();
//                }
//            }).on('error', function (data) {
//                //exibirMensagem(tipoMensagem.falha, "Falha", "Ocorreu uma falha renderizar o Mapa.");
//                finalizarRequisicao();
//            });

//            CustomRouteLayer = MQ.Routing.RouteLayer.extend({
//                createStopMarker: function (location, stopNumber) {
//                    var custom_icon,
//                      marker;

//                    custom_icon = L.icon({
//                        iconUrl: 'https://www.mapquestapi.com/staticmap/geticon?uri=poi-red_1.png',
//                        iconSize: [20, 29],
//                        iconAnchor: [10, 29],
//                        popupAnchor: [0, -29]
//                    });
//                    var route = pessoasRetorno[stopNumber - 1];

//                    switch (route.coordenadas.tipoLocalizacao) {
//                        case EnumTipoLocalizacao.cidade:
//                            backColor = "#C00";
//                            break;
//                        case EnumTipoLocalizacao.rua:
//                        case EnumTipoLocalizacao.cep:
//                            backColor = "#00F";
//                            break;
//                        case EnumTipoLocalizacao.endereco:
//                        case EnumTipoLocalizacao.esquina:
//                        case EnumTipoLocalizacao.ponto:
//                            backColor = "#009933";
//                            break;
//                        default:
//                            backColor = "#000";
//                            break;
//                    }
//                    var myIcon = L.divIcon({ html: "<div class='pin' style='background: " + backColor + "'><div class='spanpin'>" + stopNumber + "</span></div>" });
//                    marker = L.marker(location.latLng, { icon: myIcon })
//                    .bindPopup(route.pessoa.RazaoSocial + "(" + route.pessoa.Endereco.Cidade.Descricao + ' - ' + route.pessoa.Endereco.Cidade.SiglaUF + ")")
//                    .openPopup()
//                    .addTo(map);

//                    return marker;
//                }
//            });
//            setarRotaMapRequest(reordenarPosicoesEntregas());

//        }, 200);
//    }
//}

//function reordenarPosicoesEntregas() {
//    _ListaMapRequestOrdenada = _gridReorder.ObterOrdencao();
//    _pessoasReordenadas = new Array();
//    var locationsReorder = new Array();
//    locationsReorder.push(locations[0]);
//    $.each(_ListaMapRequestOrdenada, function (i, ordem) {
//        locationsReorder.push(locations[ordem.id.split("_")[1]]);
//        _pessoasReordenadas.push(pessoasRetorno[ordem.id.split("_")[1]]);
//    });
//    if (_roteirizadorMapRequest.TipoUltimoPontoRoteirizacao.val() == EnumTipoUltimoPontoRoteirizacao.AteOrigem) {
//        locationsReorder.push(locations[locations.length - 1]);
//    }
//    return locationsReorder;
//}

//function verificarMudouPosicao() {
//    var mudou = false;
//    var listaOrdenada = _gridReorder.ObterOrdencao();
//    if (_ListaMapRequestOrdenada != null) {
//        $.each(_ListaMapRequestOrdenada, function (i, ordem) {
//            if (ordem.id != listaOrdenada[i].id) {
//                _mapaRenderizado = false;
//                mudou = true;
//                return false;
//            }
//        });
//    }
//    return mudou;
//}

//function PreecherOrdemEntrega(retorno) {
//    _ListaMapRequestOrdenada = null;
//    _mapaRenderizado = false;
//    pessoasRetorno = retorno;

//    locations = new Array();
//    var naoEncontradas = false;
//    var pessoasNaoEncontradas = new Array();

//    if (retorno[0].coordenadas.tipoLocalizacao != EnumTipoLocalizacao.naoEncontrado) {
//        _roteirizadorMapRequest.Origem.val(retorno[0].pessoa.RazaoSocial + " (" + retorno[0].pessoa.Endereco.Cidade.Descricao + ' - ' + retorno[0].pessoa.Endereco.Cidade.SiglaUF + ")")
//        locations.push({ latLng: { lat: retorno[0].coordenadas.latitude, lng: retorno[0].coordenadas.longitude } })
//    } else {
//        pessoasNaoEncontradas.push({ Codigo: retorno[0].pessoa.CPFCNPJ, CPFCNPJ: retorno[0].pessoa.CPFCNPJ, RazaoSocial: retorno[0].pessoa.RazaoSocial, Localidade: retorno[0].pessoa.Endereco.Cidade.Descricao + ' - ' + retorno[0].pessoa.Endereco.Cidade.SiglaUF });
//        naoEncontradas = true;
//    }

//    var html = "";
//    var distancia = retorno[0].distancia;

//    for (var i = 1; i < retorno.length; i++) {
//        var rotaPessoa = retorno[i];
//        if (rotaPessoa.coordenadas.tipoLocalizacao != EnumTipoLocalizacao.naoEncontrado) {
//            locations.push({ latLng: { lat: rotaPessoa.coordenadas.latitude, lng: rotaPessoa.coordenadas.longitude } })

//            if (_roteirizadorMapRequest.TipoUltimoPontoRoteirizacao.val() != EnumTipoUltimoPontoRoteirizacao.AteOrigem || i != (retorno.length - 1)) {
//                html += '<tr data-position="' + i + 1 + '" id="sort_' + i + '"><td>' + (i + 1) + '</td><td>' + rotaPessoa.pessoa.RazaoSocial + '</td>';
//                html += '<td>' + rotaPessoa.pessoa.Endereco.Cidade.Descricao + ' - ' + rotaPessoa.pessoa.Endereco.Cidade.SiglaUF + '</td></tr>';
//            }
//            distancia += rotaPessoa.distancia;
//        } else {
//            naoEncontradas = true;
//            pessoasNaoEncontradas.push({ Codigo: rotaPessoa.pessoa.CPFCNPJ, CPFCNPJ: rotaPessoa.pessoa.CPFCNPJ, RazaoSocial: rotaPessoa.pessoa.RazaoSocial, Localidade: rotaPessoa.pessoa.Endereco.Cidade.Descricao + ' - ' + rotaPessoa.pessoa.Endereco.Cidade.SiglaUF });
//        }
//    }
//    if (!naoEncontradas) {
//        _roteirizadorMapRequest.Map.visible(true);
//        _roteirizadorMapRequest.SemPontos.visible(false);
//        _roteirizadorMapRequest.Distancia.val(Globalize.format(distancia, "n2") + " km");
//        _roteirizadorMapRequest.SalvarRota.visible(true);
//        _gridReorder.RecarregarGrid(html);
//    } else {
//        exibirGridPontosNaoEncontrados(pessoasNaoEncontradas);
//        _roteirizadorMapRequest.SalvarRota.visible(false);
//    }
//}

//function RoterizarViaMapRequestClick(e) {
//    var data = { Carga: _cargaAtual.Codigo.val(), TipoUltimoPontoRoteirizacao: _roteirizadorMapRequest.TipoUltimoPontoRoteirizacao.val(), TipoRota: _roteirizadorMapRequest.TipoRota.val() }
//    executarReST("MapRequest/CriarRotaCarga", data, function (arg) {
//        if (arg.Success) {
//            if (arg.Data !== false) {
//                PreecherOrdemEntrega(arg.Data);
//            } else {
//                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
//            }
//        } else {
//            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//        }

//    });
//}

//function exibirGridPontosNaoEncontrados(pessoasNaoEncontradas) {
//    exibirMensagem(tipoMensagem.atencao, "Pontos não encontrados", "Não foi possivel roteirizar a carga, pois alguns pontos não foram encontrados, verifique a lista e ajuste manualmente.");
//    var header = [{ data: "Codigo", visible: false },
//                  { data: "CPFCNPJ", title: "CNPJ/CPF", width: "25%", className: "text-align-left", orderable: false },
//                  { data: "RazaoSocial", title: "Pessoa", width: "50%", className: "text-align-left", orderable: true },
//                  { data: "Localidade", title: "Localidade", width: "25%", className: "text-align-left", orderable: true }
//    ];
//    var gridSemPontos = new BasicDataTable(_roteirizadorMapRequest.SemPontos.idGrid, header, null);
//    gridSemPontos.CarregarGrid(pessoasNaoEncontradas);

//    _roteirizadorMapRequest.Map.visible(false);
//    _roteirizadorMapRequest.SemPontos.visible(true);
//}

//function setarRotaMapRequest(locations) {
//    dir.route({
//        locations: locations,
//        options: {
//            unit: "k",
//            routeType: _roteirizadorMapRequest.TipoRota.val(),
//            locale: "pt"
//        }
//    });
//    map.addLayer(new CustomRouteLayer({
//        directions: dir,
//        fitBounds: true,
//        draggable: false,
//        ribbonOptions: {
//            draggable: false,
//            ribbonDisplay: { opacity: 0.3 }
//        }
//    }));
//}