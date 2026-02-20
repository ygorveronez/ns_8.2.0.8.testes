/// <reference path="FluxoPatio.js" />
/// <reference path="../FluxoPatioPesagem/Pesagem.js" />
/// <reference path="../FluxoPatioPesagem/PesagemFinal.js" />

function loadFluxoPatioSignalR() {
    SignalRGestaoPatioFluxoCargaAtualizadoEvent = function (data) {
        var dadosSignalR = {};

        try {
            dadosSignalR = JSON.parse(data);
        } catch (e) {
            dadosSignalR = null;
        }

        if (dadosSignalR != null)
            AtualizarFluxoCargaSignalR(dadosSignalR);
    };

    SignalRFluxoPatioPesagemInicialAtualizadaEvent = AtualizarPesagemInicialEvent;
    SignalRFluxoPatioPesagemFinalAtualizadaEvent = AtualizarPesagemFinalEvent;
}


function AtualizarFluxoCargaSignalR(data) {
    var codigoFluxo = data.Codigo;

    if (codigoFluxo in _knoutsFluxosGestaoPatio) {
        var fluxoPatio = _knoutsFluxosGestaoPatio[codigoFluxo];
        var requisicaoOculta = true;

        BuscarFluxoPatioPorCodigo(fluxoPatio, requisicaoOculta);
    }
}

function AtualizarPesagemInicialEvent(retorno) {
    if (retorno.CodigoPesagem == _pesagem.CodigoPesagem.val()) {
        atualizarPesagemInicialPorEvento(retorno.PesagemInicial);
    }
}

function AtualizarPesagemFinalEvent(retorno) {
    if (retorno.CodigoPesagem == _pesagemFinal.CodigoPesagem.val()) {
        atualizarPesagemFinalPorEvento(retorno.PesagemFinal);
    }
}