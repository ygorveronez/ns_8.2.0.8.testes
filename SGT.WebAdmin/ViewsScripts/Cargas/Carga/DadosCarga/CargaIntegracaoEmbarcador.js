
function BuscarCargasIntegracaoEmbarcadorClick(e, sender) {
    executarReST("CargaIntegracaoEmbarcador/BuscarCargasIntegracaoEmbarcador", {}, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CargasConsultadasComSucesso);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function AbrirConsultaCargasIntegracaoEmbarcadorClick() {
    $("#" + _pesquisaCarga.CargaIntegracaoEmbarcador.idBtnSearch).click();
}