var _tempoParaTroca = 30000;
var _indiceSlide = 0;
var _knoutSlides;

var _pause = false;

var _timeOutSlides;

var _arrayMetodos = [
    'carregarCentrosCarregamento',
    'obterTopVeiculosAtrazados',
    'obterTopVeiculosEmCarregamento',
    'obterTopVeiculosAgFaturamento',
    'buscarCargasAtrasadas',
    'obterDadosChamados_DadosTotais',
    'obterDadosChamados_RelacaoPorUsuario',
    'obterDadosChamados_ChamadosAtrasados',
    /*'obterDadosChamados_ValoresPorMotivo',*/
    'obterDadosChamados_RelacaoPorOcorrencia',
];

var KnoutSlides = function () {
    this.Stop = PropertyEntity({ eventClick: stopClick, type: types.event, visible: ko.observable(true) });
    this.Play = PropertyEntity({ eventClick: playClick, type: types.event, visible: ko.observable(false) });
    this.Back = PropertyEntity({ eventClick: backClick, type: types.event, visible: ko.observable(true) });
    this.Forward = PropertyEntity({ eventClick: forwardClick, type: types.event, visible: ko.observable(true) });
}

function loadSlides() {
    _knoutSlides = new KnoutSlides();
    KoBindings(_knoutSlides, "knoutSlides");
}

function playClick() {
    _pause = false;
    _knoutSlides.Stop.visible(true);
    _knoutSlides.Play.visible(false);
}

function stopClick() {
    _knoutSlides.Play.visible(true);
    _knoutSlides.Stop.visible(false);
    _pause = true;
}

function backClick() {
    LimparTimeOut();
    _controleAutomatico = false;
    var metodo = _arrayMetodos[_indiceSlide];
    if (metodo == "carregarCentrosCarregamento") {
        if (_indiceCentro == -1) {
            obterIndiceAnterior();
            InvocarMetodoSequencia();
        } else {
            carregarCentrosCarregamento(_indiceCentro - 1);
        }
    } else {
        obterIndiceAnterior();
        var metodo = _arrayMetodos[_indiceSlide];
        if (metodo == "carregarCentrosCarregamento") {
            carregarCentrosCarregamento(_indicadoresQuantidadesCarga.CentrosCarregamento.options().length - 1);
        } else {
            InvocarMetodoSequencia();
        }
    }
}

function forwardClick() {
    LimparTimeOut();
    _controleAutomatico = false;
    var metodo = _arrayMetodos[_indiceSlide];
    if (metodo == "carregarCentrosCarregamento") {
        if (_indiceCentro == _indicadoresQuantidadesCarga.CentrosCarregamento.options().length - 1) {
            obterProximoIndice()
            InvocarMetodoSequencia();
        } else {
            carregarCentrosCarregamento(_indiceCentro + 1);
        }
    } else {
        obterProximoIndice();
        var metodo = _arrayMetodos[_indiceSlide];
        if (metodo == "carregarCentrosCarregamento") {
            carregarCentrosCarregamento(-1);
        } else {
            InvocarMetodoSequencia();
        }
    }
}

function LimparTimeOut() {
    if (_timeOutSlides != null)
        clearTimeout(_timeOutSlides);
    if (_timeOutCentros != null)
        clearTimeout(_timeOutCentros);
}

function CarregarSlide() {
    _timeOutSlides = setTimeout(function () {
        //LimparTimeOut();
        if (!_pause) {
            obterProximoIndice();
        }
        InvocarMetodoSequencia();
    }, _tempoParaTroca);
}

function obterProximoIndice() {
    if (_indiceSlide == _arrayMetodos.length - 1)
        _indiceSlide = 0;
    else
        _indiceSlide++;
}

function obterIndiceAnterior() {
    if (_indiceSlide == 0)
        _indiceSlide = _arrayMetodos.length - 1;
    else
        _indiceSlide--;
}

function InvocarMetodoSequencia() {
    var metodo = _arrayMetodos[_indiceSlide];

    if (metodo == "carregarCentrosCarregamento") {
        retornoCentrosCarregamento();
    }
    else if (metodo == "obterTopVeiculosAtrazados") {
        obterTopVeiculosAtrazados(CarregarSlide);
    }
    else if (metodo == "obterTopVeiculosEmCarregamento") {
        obterTopVeiculosEmCarregamento(CarregarSlide);
    }
    else if (metodo == "obterTopVeiculosAgFaturamento") {
        obterTopVeiculosAgFaturamento(CarregarSlide);
    }
    else if (metodo == "buscarCargasAtrasadas") {
        buscarCargasAtrasadas(CarregarSlide);
    }
    else if (metodo.startsWith("obterDadosChamados")) {
        var subMetodo = metodo.replace("obterDadosChamados_", "");
        ObterDadosChamados(subMetodo, CarregarSlide);
    }
}

function ExibirSlide() {
    $("#divConteudoGrafico").hide();
    $("#VeiculosAtrasados").hide();
    $("#knockoutCargasAtrazadas").hide();
    $("#knockoutIndicadoresQuantidadeCarga").hide();
    $("#divGeralGraficoProdutosExpedidos").hide();
    $("#div-chamados-dados-totais").hide();
    $("#div-chamados-chamados-atrasados").hide();
    $("#div-chamados-valores-por-motivo").hide();
    $("#div-chamados-relacao-por-usuario").hide();
    $("#div-chamados-relacao-por-ocorrencia").hide();
    $("#knockoutChamados").hide();

    for(id in arguments)
        $("#" + arguments[id]).show();
}