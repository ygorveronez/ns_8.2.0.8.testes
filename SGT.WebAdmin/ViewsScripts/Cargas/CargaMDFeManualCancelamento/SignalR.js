/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularOcorrencia.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="Ocorrencia.js" />
/// <reference path="OcorrenciaCTe.js" />
/// <reference path="../../Enumeradores/EnumSituacoesOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="OcorrenciaLeilao.js" />
/// <reference path="../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="OcorrenciaPercurso.js" />
/// <reference path="../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="OcorrenciaFreteComissao.js" />
/// <reference path="../../Configuracao/ConfiguracaoTMS.js" />
/// <reference path="OcorrenciaComplementoFrete.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Global/SignalR/SignalR.js" />
/// <reference path="../../Configuracao/ConfiguracaoTMS.js" />
/// <reference path="OcorrenciaDadosTransporte.js" />
/// <reference path="OcorrenciaMotorista.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="OcorrenciaFreteTMS.js" />
/// <reference path="../../Global/SignalR/SignalR.js" />
/// <reference path="../../Enumeradores/EnumTipoAcaoOcorrencia.js" />


function LoadConexaoSignalRCargaMDFeManualCancelamento() {
    SignalRCargaMDFeManualAlteradoCancelamentoEvent = VerificarCargaMDFeManaulAlteradaCancelamentoEvent;
}

function VerificarCargaMDFeManaulAlteradaCancelamentoEvent(retorno) {
    if (retorno.CodigoCargaMDFeManualCancelamento == _cancelamento.Codigo.val()) {
        BuscarCancelamentoPorCodigo(false);
    }
}