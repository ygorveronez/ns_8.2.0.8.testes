/// <reference path="../../Enumeradores/EnumEtapaCarga.js" />
/// <reference path="../../Cargas/Carga/DadosCarga/Carga.js" />

var knoutCargaAgendamentoColeta;

//#region Métodos Globais
function CarregarEtapaCarga() {
    BuscarDivCarga();
}
//#endregion

//#region Métodos Privados
function BuscarDivCarga() {
    var data = { Carga: _agendamentoColeta.CodigoCarga.val() };
    if (data.Carga > 0 && _agendamentoColeta.Etapa.val() >= EnumEtapaAgendamentoColeta.NFe) {
        executarReST("Carga/BuscarCargaPorCodigo", data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    knoutCargaAgendamentoColeta = exibirEtapaCarga("fdsCarga", retorno.Data, EnumEtapaCarga.NotaFiscal);

                    if (_agendamentoColeta.ForcarEtapaNFe.val()) {
                        knoutCargaAgendamentoColeta.SituacaoCarga.val(EnumSituacoesCarga.AgNFe);
                        EtapaNotaFiscalAguardando(knoutCargaAgendamentoColeta);
                        $("#" + knoutCargaAgendamentoColeta[EnumEtapaCarga.obterNomeEtapa(EnumEtapaCarga.NotaFiscal)].idGrid).click();
                    }
                    
                    setTimeout(function () {
                        var _divId = "#" + knoutCargaAgendamentoColeta.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao footer";
                        //$($(_divId)[4]).hide();
                        $(_divId).hide();
                    }, 2000);
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}
//#endregion