/// <reference path="../../Enumeradores/EnumTipoUltimoPontoRoteirizacao.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="../../../js/Global/Mapa.js" />

var _roteirizadorSimuladorFrete = null;
var _mapa = null;
var _Pontos = null;

var RoteirizadorSimuladorFrete = function () {
    this.Map = PropertyEntity({ idGrid: guid(), visible: ko.observable(true), idGrid2: guid(), visibleReorder: ko.observable(false), visibleReorderClick: visibleReorderClick });
    this.TipoUltimoPontoRoteirizacao = PropertyEntity({ val: ko.observable(EnumTipoUltimoPontoRoteirizacao.PontoMaisDistante), enable: ko.observable(true), options: EnumTipoUltimoPontoRoteirizacao.obterOpcoes(), def: _CONFIGURACAO_TMS.TipoUltimoPontoRoteirizacao, text: "Último Ponto: ", issue: 1292, required: true });
    this.Roteirizar = PropertyEntity({ eventClick: RoteirizarSimuladorFreteClick, type: types.event, text: "Roteirizar", visible: ko.observable(true) });
    this.Distancia = PropertyEntity({});
    this.TempoDeViagemEmMinutos = PropertyEntity({});
    this.PolilinhaRota = PropertyEntity({});
    this.PontosDaRota = PropertyEntity({});
    this.Descricao = PropertyEntity({});
}

function loadRoteirizador() {
    
    _roteirizadorSimuladorFrete = new RoteirizadorSimuladorFrete();
    
    KoBindings(_roteirizadorSimuladorFrete, "knockoutRoteirizacao");

    loadGoogleMapa();
}

function visibleReorderClick(e) {
    e.Map.visibleReorder(!e.Map.visibleReorder());
}

function RoteirizarSimuladorFreteClick(e) {
    criarMapaSimuladorFrete();
    atualizaMarcadoresMapa();
    gerarRoteirizacaoGoogleMapsOSM(true);
}

function criarMapaSimuladorFrete() {
    if (_mapa === null) {
        _mapa = new Mapa(_roteirizadorSimuladorFrete.Map.id, false);
        _mapa.setarCallbackAlteracaoEntrega(MontagemCargaAlterouOrdemEntrega);
        _mapa.setarCallbackAlteracaoRota(MontagemCargaAlterouRota);
    }
}

function obterIndicePonto(codigo) {
    for (var i = 0; i < _Pontos.length; i++) {
        if (_Pontos[i]) {
            if (parseInt(_Pontos[i].codigo) === parseInt(codigo)) {
                return i;
            }
        }
    }
    return - 1
}

function MontagemCargaAlterouOrdemEntrega(resposta, pontoOrigem, pontoDestino) {

    var idOrigem = obterIndicePonto(pontoOrigem.codigo)
    var idDestino = obterIndicePonto(pontoDestino.codigo)

    if ((idOrigem >= 0) && (idDestino >= 0)) {

        var pontoorigem = _Pontos[idOrigem];
        var pontodestino = _Pontos[idDestino];

        _Pontos[idDestino] = pontoorigem;
        _Pontos[idOrigem] = pontodestino;

        PreencherOrdemLocalidades(_Pontos);
        InicializarPontoOriginal();

        setInfoRespostaRoteirizacao(resposta);
    }
}

function MontagemCargaAlterouRota(resposta) {
    setInfoRespostaRoteirizacao(resposta);
}

function setInfoRespostaRoteirizacao(resposta) {
    var distanciaKM = resposta.distancia / 1000;
    _roteirizadorSimuladorFrete.PolilinhaRota.val(resposta.polilinha);
    _roteirizadorSimuladorFrete.Distancia.val(distanciaKM);
    _simuladorFrete.Distancia.val(distanciaKM);
    _roteirizadorSimuladorFrete.TempoDeViagemEmMinutos.val(Math.trunc(resposta.tempo / 60));
    _roteirizadorSimuladorFrete.PontosDaRota.val(resposta.pontosroteirizacao);
    if (_gridResultado) {
        var data = _gridResultado.BuscarRegistros();
        $.each(data, function (i, item) {
            item.Distancia = distanciaKM;
            item.ValorFrete = 'R$ 0,00';
        });
        atualizarGridResultado(data);
    }
}

function PreencherOrdemLocalidades(pontos) {
    var descricao = '';
    $.each(pontos, function (i, item) {
        if (descricao != '' && i < (pontos.length - 1))
            descricao += ', ';

        if (descricao != '' && i == (pontos.length - 1))
            descricao += ' ATÉ ';

        descricao += item.descricao;
    });
    _roteirizadorSimuladorFrete.Descricao.val(descricao);
}

function InicializarPontoOriginal() {
    _PontosOriginais = new Array();
    for (var i = 0; i < _Pontos.length; i++)
        _PontosOriginais.push(_Pontos[i]);
}

function float(valor) {
    return parseFloat(valor.replace(',', '.'));
}

function atualizaMarcadoresMapa() {
    if (_mapa != null) {

        this._mapa.limparMapa();

        _Pontos = [];
        if (_simuladorFrete.Origem.codEntity() != 0) {
            var origem = _simuladorFrete.LocalidadeOrigem.val();
            _Pontos.push(new PontosRota(origem.Descricao, float(origem.Latitude), float(origem.Longitude), false, false, 0, 0, origem.Codigo, 0, EnumTipoPontoPassagem.Coleta, origem.Descricao));
        }

        _gridLocalidade.BuscarRegistros()

        var localidades = _localidade.Localidade.basicTable.BuscarRegistros();
        for (var i = 0; i < localidades.length; i++) {
            var destino = localidades[i];
            _Pontos.push(new PontosRota(destino.Descricao, float(destino.Latitude), float(destino.Longitude), false, false, 0, 0, destino.Codigo, (i + 1), EnumTipoPontoPassagem.Entrega, destino.Descricao));
        }

        if (_Pontos.length > 0) {
            this._mapa.adicionarMarcador(_Pontos, true, false);
        }
    }
}

function gerarRoteirizacaoGoogleMapsOSM(modal_roteirizar, callback) {

    var destinosLocalidades = _localidade.Localidade.basicTable.BuscarRegistros();

    if (_simuladorFrete.Origem.codEntity() == 0 || destinosLocalidades.length == 0)
        return;

    iniciarControleManualRequisicao();
    _PontosOriginais = null;
    this._mapa.limparMapa();

    var tipo = _roteirizadorSimuladorFrete.TipoUltimoPontoRoteirizacao.val();

    this._mapa.ordenarRotaOpenStreetMap(_Pontos, tipo, function (respostaOrdenada, status) {

        if (status === "OK") {
            var retorno = Array.from(respostaOrdenada);

            this._mapa.roteirizarSemOrdem(respostaOrdenada, tipo, function (resposta) {

                if (resposta.status == "OK") {
                    OrdenarPontosGrid(resposta);
                    PreencherOrdemLocalidades(_Pontos);
                    setInfoRespostaRoteirizacao(resposta);
                }

                if (resposta.status != "OK")
                    exibirMensagem(tipoMensagem.aviso, "Roteirização", "Não foi possível encontrar uma rota. Verifique a latitude/longitude da origem e destinatários.");

                if (modal_roteirizar) {
                    finalizarControleManualRequisicao();
                } else {
                    if (callback != undefined) {
                        callback(retorno);
                    }
                }
            });

        }
        else {
            exibirMensagem(tipoMensagem.atencao, "Atenção", status);
            finalizarControleManualRequisicao();
        }
    }, false);
}

function OrdenarPontosGrid(resposta) {
    var pontosrota = JSON.parse(resposta.pontosroteirizacao);
    var novaLista = new Array();
    var ultimo = pontosrota.length;
    //if (_roteirizadorSimuladorFrete.TipoUltimoPontoRoteirizacao.val() == EnumTipoUltimoPontoRoteirizacao.PontoMaisDistante)
    //    ultimo = pontosrota.length - 1;

    for (var i = 0; i < ultimo; i++) {
        if (pontosrota[i].sequencia >= 0) {
            novaLista.push(_Pontos[pontosrota[i].sequencia]);
        }
    }
    _Pontos = novaLista;
}