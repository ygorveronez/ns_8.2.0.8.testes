/// <reference path="Destinatario.js" />
/// <reference path="Destinatario.js" />
/// <reference path="Coleta.js" />
/// <reference path="PontosPassagemPreDefinido.js" />

var Roteirizacao = function () {
    this.Roteirizar = PropertyEntity({ eventClick: roteirizarClick, type: types.event, text: "Roteirizar", visible: ko.observable(true) });
    this.TipoUltimoPontoRoteirizacao = PropertyEntity({ val: ko.observable(EnumTipoUltimoPontoRoteirizacao.PontoMaisDistante), options: EnumTipoUltimoPontoRoteirizacao.obterOpcoes(), def: EnumTipoUltimoPontoRoteirizacao.PontoMaisDistante, text: "Último Ponto: ", issue: 1292, required: true, enable: ko.observable(true) });

    this.TipoUltimoPontoOperacao = PropertyEntity({});
};
var _roteirizador;
var _rotamapa;
var clienteOrigem;
var _rotaFreteOrigem;
var _rotaFreteExpedidor;
var _origemRota;

var RoteirizacaoMapa = function () {
    //#region métodos privados

    this._mapa = null;

    var self = this;

    this._instanceMapa = function () {
        self._mapa = new Mapa("map", false, null, null, true);
        self._mapa.setarCallbackAlteracaoRota(this._callbackAlteracaoRota);
    }

    this._buscarPracas = function (callback) {

        var ultimoPonto = _roteirizador.TipoUltimoPontoRoteirizacao.val();

        var dados = { polilinha: _rotaFrete.PolilinhaRota.val(), pontosDaRota: _rotaFrete.PontosDaRota.val(), tipoUltimoPontoRoteirizacao: ultimoPonto };
        var path = "RotaFrete/BuscarPracasPedagioPolilinha";
        executarReST(path, dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    callback(arg.Data);
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Logistica.RotaFrete.PracasObtidasComSucesso);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                    callback(null);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                callback(null);
            }
        });
    };

    this._setPracas = function (callback) {
        if (!_integracaoSemParar) {
            if (callback)
                callback(false);

            return;
        }


        self._buscarPracas(function (pracas) {

            if ((_rotaFrete) && (pracas)) {
                _rotaFrete.PracaPedagios.val(pracas);

                if (_gridPracaPedagioRotaFrete)
                    _gridPracaPedagioRotaFrete.CarregarGrid(_rotaFrete.PracaPedagios.val());
            }

            if (callback)
                callback(true);

            var pontospraca = [];
            if (pracas) {
                self._adicionarListaPracasPedagio(pontospraca, pracas);

                self._mapa.adicionarPracasPedagio(pontospraca);
            }

        });
    };

    this._buscarZonasExclusaoRota = function (callback) {

        var dados = { TipoLocal: 6 /* Zona de exclusão de Rotas */, CodigosFiliais: [] };
        var path = "Locais/BuscarPorTipoEFiliais";
        executarReST(path, dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    callback(arg.Data);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                    callback(null);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                callback(null);
            }
        });
    };

    this._setZonasExclusaoRota = function (callback) {

        self._mapa.draw.deleteAll();

        self._buscarZonasExclusaoRota(function (zonas) {

            if (callback)
                callback(true);

            if (zonas) {
                for (var i = 0; i < zonas.length; i++) {
                    self._mapa.draw.setJson(zonas[i].Area, true, true);
                }
            }

        });
    };

    this._callbackAlteracaoRota = function (resposta) {
        self._setInfoResposta(resposta);
        ExibirBuscarPracaRota();
    };

    this._validarOrigemDestino = function (clientes) {

        if ((_origemRota == null) || (_origemRota.Codigo == null)) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.RotaFrete.ParaGerarRoteireizacaoNecessarioRemetente);
            return false;
        }


        if (clientes.length <= 0) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.RotaFrete.ParaGerarRoteireizacaoNecessarioDestinatario);
            return false;
        }



        if ((_origemRota.Latitude == null) || (_origemRota.Longitude == null)) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.RotaFrete.ParaGerarRoteireizacaoNecessarioLatitudeLongitute);
            return false;
        }

        return true;
    };

    this._validarClientesSemPontos = function (clientes, callback) {

        _clientesSemPontos = new Array();

        for (var i = 0; i < clientes.length; i++) {
            VerificarClienteSemLatLngDestinatario(clientes[i]);
        }


        if (_clientesSemPontos.length === 0)
            callback(true);

        if (_clientesSemPontos.length > 0) {
            AtualizarPontosFaltantes(null, function () {
                if (_ClientePontosNaoLocalizados.length > 0) {
                    var strClientes = "";
                    for (var i = 0; i < _ClientePontosNaoLocalizados.length; i++) {
                        strClientes += _ClientePontosNaoLocalizados[i].Descricao + " ";
                    }
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.RotaFrete.NaoFoiPossivelRoteirizarClientes + strClientes);
                    callback(false);
                }
                else callback(true);
            });
        }
    };

    this._validarColetasSemPontos = function (clientes, callback) {

        _clientesSemPontos = new Array();

        for (var i = 0; i < clientes.length; i++) {
            VerificarClienteSemLatLngDestinatario(clientes[i]);
        }


        if (_clientesSemPontos.length === 0)
            callback(true);

        if (_clientesSemPontos.length > 0) {
            AtualizarPontosFaltantes(null, function () {
                if (_ClientePontosNaoLocalizados.length > 0) {
                    var strClientes = "";
                    for (var i = 0; i < _ClientePontosNaoLocalizados.length; i++) {
                        strClientes += _ClientePontosNaoLocalizados[i].Descricao + " ";
                    }
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.RotaFrete.NaoFoiPossivelRoteirizarClientes + strClientes);
                    callback(false);
                }
                else callback(true);
            });
        }
    };

    this._adicionarClientes = function (pontos, clientes) {
        for (var i = 0; i < clientes.length; i++) {
            var pt = {
                descricao: clientes[i].Descricao,
                pedagio: false,
                fronteira: false,
                lat: self._formatToFloat(clientes[i].Latitude),
                lng: self._formatToFloat(clientes[i].Longitude),
                codigo: clientes[i].Codigo,
                tipoponto: EnumTipoPontoPassagem.Entrega,
                localDeParqueamento: false,
                ordem: clientes[i].Ordem,
                utilizaLocalidade: clientes[i].utilizaLocalidade
            };

            pontos.push(pt);
        }
    };

    this._adicionarColetas = function (pontos, coletas) {
        for (var i = 0; i < coletas.length; i++) {
            var pt = { descricao: coletas[i].Descricao, pedagio: false, fronteira: false, lat: self._formatToFloat(coletas[i].Latitude), lng: self._formatToFloat(coletas[i].Longitude), codigo: coletas[i].Codigo, tipoponto: EnumTipoPontoPassagem.Coleta, localDeParqueamento: false };
            pontos.push(pt);
        }
    };

    this._adicionarPontosPassagemPreDefinidos = function (pontos, pontosPassagem) {
        for (var i = 0; i < pontosPassagem.length; i++) {
            var pt = {
                descricao: pontosPassagem[i].Descricao,
                pedagio: false,
                fronteira: false,
                tempoEstimadoPermanencia: !!pontosPassagem[i].TempoEstimadoPermanenciaMinutos ? pontosPassagem[i].TempoEstimadoPermanenciaMinutos : 0,
                lat: self._formatToFloat(pontosPassagem[i].Latitude),
                lng: self._formatToFloat(pontosPassagem[i].Longitude),
                codigo: pontosPassagem[i].CodigoCliente,
                tipoponto: EnumTipoPontoPassagem.Passagem,
                localDeParqueamento: pontosPassagem[i].LocalDeParqueamento == "Sim" ? true : false,
                pontopassagem: true,
            };

            pontos.push(pt);
        }
    };

    this._adicionarPostosFiscais = function (pontos, postosFiscais) {
        for (var i = 0; i < postosFiscais.length; i++) {
            var pt = {
                descricao: postosFiscais[i].Descricao,
                pedagio: false,
                fronteira: false,
                tempoEstimadoPermanencia: !!postosFiscais[i].TempoEstimadoPermanenciaMinutos ? postosFiscais[i].TempoEstimadoPermanenciaMinutos : 0,
                lat: self._formatToFloat(postosFiscais[i].Latitude),
                lng: self._formatToFloat(postosFiscais[i].Longitude),
                codigo: postosFiscais[i].CodigoCliente,
                tipoponto: EnumTipoPontoPassagem.PostoFiscal,
                localDeParqueamento: false,
                pontopassagem: true,
            };

            pontos.push(pt);
        }
    };

    this._adicionarListaPracasPedagio = function (pontos, pracasPedagio) {
        for (var i = 0; i < pracasPedagio.length; i++) {
            var pt = { descricao: pracasPedagio[i].Descricao, pedagio: true, fronteira: false, lat: self._formatToFloat(pracasPedagio[i].Latitude), lng: self._formatToFloat(pracasPedagio[i].Longitude), codigo: pracasPedagio[i].CodigoIntegracao, tipoponto: EnumTipoPontoPassagem.Pedagio, localDeParqueamento: false };
            pontos.push(pt);
        }
    };

    this._adicionarFronteiras = function (pontos, fronteiras) {
        for (var i = 0; i < fronteiras.length; i++) {
            var pt = {
                descricao: fronteiras[i].Descricao,
                pedagio: false,
                fronteira: true,
                lat: self._formatToFloat(fronteiras[i].Latitude),
                lng: self._formatToFloat(fronteiras[i].Longitude),
                codigo: fronteiras[i].Codigo,
                tipoponto: EnumTipoPontoPassagem.Fronteira,
                localDeParqueamento: false,
                tempoEstimadoPermanencia: !!fronteiras[i].TempoMedioPermanenciaFronteiraMinutos ? fronteiras[i].TempoMedioPermanenciaFronteiraMinutos : 0,
            };
            pontos.push(pt);
        }
    };

    this._adicionarOrigem = function (pontos) {
        const cli = {
            descricao: _origemRota.Descricao,
            pedagio: false,
            fronteira: false,
            lat: self._formatToFloat(_origemRota.Latitude),
            lng: self._formatToFloat(_origemRota.Longitude),
            codigo: _origemRota.Codigo,
            tipoponto: EnumTipoPontoPassagem.Coleta,
            localDeParqueamento: false,
            utilizaLocalidade: _origemRota.UtilizaLocalidade != null ? _origemRota.UtilizaLocalidade : false
        };
        pontos.push(cli);
    };

    var setOrigemRota = function () {

        // Se tiver Expedidor, 
        //      Origem Expedidor
        // Se Remetente
        //      Origem Remetente
        // Senão
        //      Origem da Rota Frete.

        _origemRota = _rotaFreteExpedidor;

        if ((_origemRota == null) || (_origemRota.Codigo == null || _origemRota.Codigo == 0))
            _origemRota = clienteOrigem;

        if ((_origemRota == null) || (_origemRota.Codigo == null || _origemRota.Codigo == 0))
            _origemRota = _rotaFreteOrigem;
    }

    this._formatToFloat = function (value) {
        if (value != null)
            return parseFloat(value.toString().replace(',', '.'));
        else
            return 0;
    }

    this._validarDados = function (callback) {

        var clientes = [];

        var dadosDestinatario = _destinatarioRotaFrete.Destinatario.basicTable.BuscarRegistros();

        for (var i = 0; i < dadosDestinatario.length; i++)
            clientes.push(dadosDestinatario[i]);

        var dadosLocalidade = _gridLocalidade.BuscarRegistros()

        for (var i = 0; i < dadosLocalidade.length; i++) {
            clientes.push(dadosLocalidade[i]);
            clientes[clientes.length - 1].utilizaLocalidade = true;
        }


        var coletas = _coletaRotaFrete.Coleta.basicTable.BuscarRegistros();
        var pracasPedagio = _pracaPedagioIdaVolta.Pracas.basicTable.BuscarRegistros();
        var fronteiras = _fronteiraRotaFrete.Fronteira.basicTable.BuscarRegistros();

        var origemdestinovalidos = self._validarOrigemDestino(clientes);

        if (origemdestinovalidos)
            clientesvalidos = self._validarClientesSemPontos(clientes, function (clientesvalido) {

                coletasvalidas = self._validarColetasSemPontos(coletas, function (coletasvalidas) {
                    if (clientesvalido && coletasvalidas) {
                        callback(clientes, coletas, pracasPedagio, fronteiras);
                        //self._validarPracasPedagioSemPontos(pracasPedagio, function (pracasvalidas) {

                        //    if (pracasvalidas)
                        //        callback(clientes, coletas, pracasPedagio);
                        //});
                    };
                });

            });
    };

    this._roteirizar = function (clientes, coletas, fronteiras, callback) {
        var pontos = new Array();

        self._adicionarOrigem(pontos);

        self._adicionarColetas(pontos, coletas);

        var pontosPassagem = _pontosPassagemPreDefinidoRotaFrete.PontosPassagemPreDefinido.basicTable.BuscarRegistros();
        self._adicionarPontosPassagemPreDefinidos(pontos, pontosPassagem);

        var postosFiscais = _postoFiscal.PostosFiscais.basicTable.BuscarRegistros();
        self._adicionarPostosFiscais(pontos, postosFiscais);

        self._adicionarClientes(pontos, clientes);

        self._adicionarFronteiras(pontos, fronteiras);

        var call = callback;

        this._mapa.roteirizar(pontos, _roteirizador.TipoUltimoPontoRoteirizacao.val(), function (resposta) {
            if (resposta.status != "OK") {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.ErroAoRoteirizar + resposta.status);
            }

            call(resposta);

        });
    };

    this._setInfoResposta = function (resposta) {
        if (resposta.status != "OK") {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.ErroAoRoteirizar + resposta.status);
        }
        else {
            var minutos = Math.trunc(resposta.tempo / 60);
            var km = parseInt(resposta.distancia / 1000);
            if (_rotaFrete.Quilometros.val() > 0 && _rotaFrete.Quilometros.val() != km) {
                exibirConfirmacao("Confirmação", `Deseja alterar o KM de ${_rotaFrete.Quilometros.val()} para ${km}?`, function () {
                    _rotaFrete.PadraoTempo.val(EnumPadraoTempoDiasMinutos.Minutos);
                    _rotaFrete.PolilinhaRota.val(resposta.polilinha);
                    _rotaFrete.Quilometros.val(parseInt(km));
                    _rotaFrete.PontosDaRota.val(resposta.pontosroteirizacao);

                    if (!_CONFIGURACAO_TMS.NaoCalcularTempoDeViagemAutomatico)
                        _rotaFrete.TempoDeViagemEmMinutos.val(Global.convertMinsToHrsMins(minutos));

                }, function () {
                    _rotamapa.roteirizarComPontosDaRota();
                });
            } else {
                _rotaFrete.PadraoTempo.val(EnumPadraoTempoDiasMinutos.Minutos);
                _rotaFrete.PolilinhaRota.val(resposta.polilinha);
                _rotaFrete.Quilometros.val(parseInt(km));
                _rotaFrete.PontosDaRota.val(resposta.pontosroteirizacao);

                if (!_CONFIGURACAO_TMS.NaoCalcularTempoDeViagemAutomatico)
                    _rotaFrete.TempoDeViagemEmMinutos.val(Global.convertMinsToHrsMins(minutos));
            }
        }

    }

    this.limparMapa = function () {
        this._mapa.limparMapa();
    };

    this.roteirizarComPontosDaRota = function () {

        if (_rotaFrete.SituacaoDaRoteirizacao.val() == 3) {
            $("#divFalhaRoteirizacao").show();
            $("#pMotivoProblemaFechamento").text(_rotaFrete.MotivoFalhaRoteirizacao.val());
        } else {
            $("#divFalhaRoteirizacao").hide();
            $("#pMotivoProblemaFechamento").text("");
        }

        var pontosdarota = _rotaFrete.PontosDaRota.val();
        if (pontosdarota) {

            iniciarControleManualRequisicao();

            var pontospraca = [];
            var pracasPedagio = _pracaPedagioIdaVolta.Pracas.basicTable.BuscarRegistros();
            self._adicionarListaPracasPedagio(pontospraca, pracasPedagio);

            this._mapa.roteirizarComPontosDaRota(pontosdarota, function (resposta) {

                if (resposta.status != "OK")
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.ErroAoRoteirizar + resposta.status);

                self._mapa.adicionarMarcador(pontospraca);

                finalizarControleManualRequisicao();

            });
        }

    }

    this.roteirizar = function () {
        LimparBotaoPracaPedagio();
        setOrigemRota();
        var self = this;
        this._validarDados(function (clientes, coletas, pracasPedagio, fronteiras) {
            iniciarControleManualRequisicao();

            self._roteirizar(clientes, coletas, fronteiras, function (resposta) {

                if (resposta.status == "OK") {
                    $("#divFalhaRoteirizacao").hide();
                    $("#pMotivoProblemaFechamento").text("");

                    self._setInfoResposta(resposta);

                    self._setPracas(function () {

                        finalizarControleManualRequisicao();

                    });
                }
                else {
                    $("#divFalhaRoteirizacao").show();
                    $("#pMotivoProblemaFechamento").text(Localization.Resources.Logistica.NaoFoiPossivelRoteirizar);
                    finalizarControleManualRequisicao();
                }

            })
        });
    };

    //#endregion 

    //#region init

    this._instanceMapa();

    //#endregion
};

function roteirizarClick() {
    _rotamapa.roteirizar();

}

function abaRoteirizacaoClick() {

    setTimeout(function () {
        if (_rotaFrete.ApenasObterPracasPedagio.val() === true) {

            if (_rotaFrete.PolilinhaRota.val())
                _rotamapa._mapa.desenharPolilinha(_rotaFrete.PolilinhaRota.val(), true);

            if (_rotaFrete.PontosDaRota.val())
                _rotamapa._mapa.adicionarMarcadorComPontosDaRota(_rotaFrete.PontosDaRota.val());

            //var pracasPedagio = _pracaPedagioRotaFrete.PracaPedagio.basicTable.BuscarRegistros();

            //if (pracasPedagio != null && pracasPedagio.length > 0) {
            //    var pontos = [];
            //    for (var i = 0; i < pracasPedagio.length; i++) {
            //        var pt = { descricao: pracasPedagio[i].Descricao, pedagio: true, lat: parseFloat(pracasPedagio[i].Latitude), lng: parseFloat(pracasPedagio[i].Longitude), codigo: pracasPedagio[i].CodigoIntegracao, tipoponto: EnumTipoPontoPassagem.Pedagio };
            //        pontos.push(pt);
            //    }

            //    _rotamapa._mapa.adicionarPracasPedagio(pontos);
            //}

        } else {
            if (_rotaFrete.PontosDaRota.val()) {
                _rotamapa.roteirizarComPontosDaRota();
            }
            else {
                _rotamapa.roteirizar();
            }
        }
    }, 300);
}

function loadRoteirizacao() {
    _rotamapa = new RoteirizacaoMapa();

    _roteirizador = new Roteirizacao();
    KoBindings(_roteirizador, "knockoutRoteirizacao");

    _rotaFrete.TipoUltimoPontoRoteirizacao = _roteirizador.TipoUltimoPontoRoteirizacao;
    _rotaFrete.TipoUltimoPontoOperacao = _roteirizador.TipoUltimoPontoOperacao;
    //_rotaFrete.TipoOperacao = _roteirizador.TipoOperacao;

    buscarZonasExclusaoRota();
}

function loadMapaGoogle() {
    _rotamapa.limparMapa();
}

function limparMapa() {
    _rotamapa.limparMapa();
}

function buscarZonasExclusaoRota() {
    if (_rotamapa)
        _rotamapa._setZonasExclusaoRota();
}
