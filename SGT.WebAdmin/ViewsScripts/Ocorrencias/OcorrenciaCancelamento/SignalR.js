/// <reference path="CancelamentoOcorrencia.js" />
/// <reference path="DadosCancelamento.js" />
/// <reference path="Documentos.js" />
/// <reference path="EtapasCancelamento.js" />

function loadConexaoSignalRCancelamentoOcorrencia() {
    SignalRCancelamentoOcorrenciaAlteradaEvent = VerificarCancelamentoOcorrenciaAlteradaEvent;
    SignalRCancelamentoOcorrenciaDocumentoAlteradoEvent = VerificarCancelamentoOcorrenciaDocumentoAlteradoEvent;
}

function VerificarCancelamentoOcorrenciaAlteradaEvent(retorno) {
    if (retorno.CodigoCancelamento == _cancelamentoOcorrencia.Codigo.val()) {
        _RequisicaoIniciada = true;
        BuscarCancelamentoPorCodigo(_cancelamentoOcorrencia.Codigo.val(), function () {
            _RequisicaoIniciada = false;
        });
    }
}

var gridDocumentosEstaAtualizando = false;
var gridIntergacaoEstaAtualizando = false;
var gridIntergacaoCTeEstaAtualizando = false;
function VerificarCancelamentoOcorrenciaDocumentoAlteradoEvent(retorno) {
    if (retorno.CodigoCancelamento == _cancelamentoOcorrencia.Codigo.val()) {
        /* gridDocumentosEstaAtualizando
         * Para evitar multiplas atualizacoes da grid
         * Só atualiza a grid quando ela não esta sendo atualizada
         */
        if (!gridDocumentosEstaAtualizando) {
            gridDocumentosEstaAtualizando = true;
            _gridDocumentos.CarregarGrid(function () {
                gridDocumentosEstaAtualizando = false;
            });
        }

        if (!gridIntergacaoEstaAtualizando) {
            gridIntergacaoEstaAtualizando = true;
            _gridIntegracaoOcorrenciaCancelamento.CarregarGrid(function () {
                gridIntergacaoEstaAtualizando = false;
            });
        }

        if (!gridIntergacaoCTeEstaAtualizando) {
            gridIntergacaoCTeEstaAtualizando = true;
            _gridIntegracaoCTeOcorrenciaCancelamento.CarregarGrid(function () {
                gridIntergacaoCTeEstaAtualizando = false;
            });
        }
    }
}