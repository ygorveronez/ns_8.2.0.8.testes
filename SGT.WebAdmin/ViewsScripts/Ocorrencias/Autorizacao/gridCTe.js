function baixarXMLCTeClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadXML", data);
}

function baixarDacteClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadDacte", data);
}

function retornoSefazClick(e, sender) {
    $('#PMensagemRetornoSefaz').html(e.RetornoSefaz);
    Global.abrirModal('divModalRetornoSefaz');
}

function emitirCTeClick(e) {
    if (e.Status == "Rejeição") {
        var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
        executarReST("CargaCTe/EmitirNovamente", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridDocumentos.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.SituacaoNaoPermiteEmissao, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AtualSituacaoCTeNaoPermiteQueEleSejaEmitidoNovamente.format(e.Status));
    }
}


function VisibilidadeMensagemSefaz(data) {
    return (data.RetornoSefaz != "" && (data.NumeroModeloDocumentoFiscal == "57" || data.NumeroModeloDocumentoFiscal == "39"));
}

function VisibilidadeRejeicao(data) {
    return (data.SituacaoCTe == EnumStatusCTe.REJEICAO);
}

function VisibilidadeOpcaoDownloadDANFSE(data) {
    return ((data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.ANULADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO) && data.NumeroModeloDocumentoFiscal == "39");
}

function VisibilidadeOpcaoDownload(data) {
    return ((data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO) && data.NumeroModeloDocumentoFiscal == "57");
}

function VisibilidadeDownloadOutrosDoc(data) {
    return (data.CodigoCTe > 0 && data.NumeroModeloDocumentoFiscal != "57" && data.NumeroModeloDocumentoFiscal != "39");
}