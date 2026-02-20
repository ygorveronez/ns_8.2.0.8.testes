/*
 * Declaração das Funções Públicas
 */

function ExibirDetalhesCarga(carga) {
    executarReST("Carga/BuscarCargaPorCodigo", { Carga: carga.Carga.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                $("#fdsCarga").html('<button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true" style="position: absolute; z-index: 9999; right: 18px; top: 6px;"><i class="fal fa-times"></i></button>');
                _cargaAtual = GerarTagHTMLDaCarga("fdsCarga", retorno.Data, false);
                $("#fdsCarga .container-carga").addClass("mb-0");

                if (retorno.Data.DadosTransporte.TipoCarga.Codigo <= 0 || retorno.Data.DadosTransporte.ModeloVeicularCarga.Codigo <= 0)
                    $("#" + _cargaAtual.EtapaInicioEmbarcador.idTab).click();
                else if (retorno.Data.SituacaoCarga == EnumSituacaoCargaJanelaCarregamento.SemValorFrete)
                    $("#" + _cargaAtual.EtapaFreteEmbarcador.idTab).click();
                else if (
                    retorno.Data.SituacaoCarga == EnumSituacaoCargaJanelaCarregamento.SemTransportador ||
                    retorno.Data.SituacaoCarga == EnumSituacaoCargaJanelaCarregamento.AgAceiteTransportador ||
                    retorno.Data.SituacaoCarga == EnumSituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador ||
                    retorno.Data.SituacaoCarga == EnumSituacaoCargaJanelaCarregamento.ProntaParaCarregamento
                )
                    $("#" + _cargaAtual.EtapaDadosTransportador.idTab).click();
                else
                    $("#" + _cargaAtual.EtapaInicioEmbarcador.idTab).click();
                                
                Global.abrirModal('divModalDetalhesCarga');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}
