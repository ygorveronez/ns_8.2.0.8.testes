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
/// <reference path="../../Consultas/ModeloVeicularFechamento.js" />
/// <reference path="../../Consultas/TipoFechamento.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="Fechamento.js" />
/// <reference path="FechamentoCTe.js" />
/// <reference path="../../Enumeradores/EnumSituacoesFechamento.js" />
/// <reference path="../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="FechamentoLeilao.js" />
/// <reference path="../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="FechamentoPercurso.js" />
/// <reference path="../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="FechamentoFreteComissao.js" />
/// <reference path="../../Configuracao/ConfiguracaoTMS.js" />
/// <reference path="FechamentoComplementoFrete.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Global/SignalR/SignalR.js" />
/// <reference path="../../Configuracao/ConfiguracaoTMS.js" />
/// <reference path="FechamentoDadosTransporte.js" />
/// <reference path="FechamentoMotorista.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="FechamentoFreteTMS.js" />
/// <reference path="../../Global/SignalR/SignalR.js" />
/// <reference path="../../Enumeradores/EnumTipoAcaoFechamento.js" />


function LoadConexaoSignalRFluxoColetaEntrega() {
    SignalRFluxoColetaEntregaAlteradaEvent = VerificarFluxoColetaEntregaAlteradaEvent;
}

function VerificarFluxoColetaEntregaAlteradaEvent(retorno) {
    if (retorno.CodigoFluxoColetaEntrega == _fluxoAtual.Codigo.val()) {
        atualizarFluxoColetaEntrega();
    }
}