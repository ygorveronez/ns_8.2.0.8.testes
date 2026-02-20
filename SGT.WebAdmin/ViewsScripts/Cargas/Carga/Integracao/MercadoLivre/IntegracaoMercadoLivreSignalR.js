function LoadConexaoSignalRMercadoLivre() {
    SignalRMercadoLivreHUAtualizadoEvent = VerificarHUMercadoLivreAtualizadoEvent;
}

function VerificarHUMercadoLivreAtualizadoEvent(retorno) {
    if (_listaKnoutsCarga != null) {

        var podeAtualizar = false;
        var indiceKnout;

        $.each(_listaKnoutsCarga, function (i, knoutCarga) {
            if (knoutCarga.Codigo.val() == retorno.CodigoCarga) {
                podeAtualizar = true;
                indiceKnout = i;
                return false;
            }
        });

        if (podeAtualizar)
            HandleHUMercadoLivreAtualizadoSignalR(retorno, _listaKnoutsCarga[indiceKnout]);
    }
}