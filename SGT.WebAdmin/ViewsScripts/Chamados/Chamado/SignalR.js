/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../Enumeradores/EnumSituacaoChamado.js" />
/// <reference path="Chamado.js" />

/*
 * Declaração das Funções de Inicialização
 */

function loadChamadoSignalR() {
    SignalRChamadoAdicionadoOuAtualizadoEvent = chamadoAdicionadoOuAtualizado;
    SignalRChamadoEscalarTempoExcedidoEvent = ChamadoEscalarTempoExcedido;
    SignalRChamadoMensagemRecebidaChatEvent = processarMensagemRecebidaEvent;
    SignalRChamadoMensagemEnviadaChatEvent = processarMensagemChatEnviadaEvent;
    SignalRChamadoCanceladoEvent = chamadoCanceladoEvent;
}

function ChamadoEscalarTempoExcedido(dadosChamado) {
    let codigoChamadoAberto = _chamado?.Codigo?.val?.();
    if (codigoChamadoAberto && dadosChamado.Codigo === codigoChamadoAberto) {
        editarChamadoClick(dadosChamado);
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.ControleEntrega.MensagemEscalarTempoExcedido, "");
    }
  
}

function chamadoCanceladoEvent(dados) {
    let codigoChamadoAberto = _chamado?.Codigo?.val?.();
    if (codigoChamadoAberto && dados.CodigoChamado === codigoChamadoAberto) {
        Global.abrirModal("modalChamadoCancelado");
    }
}


function confirmarLeituraMensagem(codigoMensagem) {
    SignalRColetaEntrega.server.confirmarLeituraMensagem(codigoMensagem);
}

/*
 * Declaração das Funções Privadas
 */

function chamadoAdicionadoOuAtualizado(dadosChamado) {
    if (isAtualizarChamados(dadosChamado))
        _gridChamados.CarregarGrid();
}

function isAtualizarChamados(dadosChamado) {
    if (!_origemTelaChamado)
        return false;

    if ((_pesquisaChamados.SituacaoChamado.val() != EnumSituacaoChamado.Todas) && (_pesquisaChamados.SituacaoChamado.val() != dadosChamado.SituacaoChamado))
        return false;

    if (_pesquisaChamados.NumeroInicial.val() > dadosChamado.NumeroChamado)
        return false;

    if ((_pesquisaChamados.NumeroFinal.val() > 0) && (_pesquisaChamados.NumeroFinal.val() < dadosChamado.NumeroChamado))
        return false;

    if ((_pesquisaChamados.Responsavel.codEntity() > 0) && (dadosChamado.CodigoResponsavel > 0) && (_pesquisaChamados.Responsavel.codEntity() != dadosChamado.CodigoResponsavel))
        return false;

    if ((_pesquisaChamados.Filial.codEntity() > 0) && (dadosChamado.CodigoFilial > 0) && (_pesquisaChamados.Filial.codEntity() != dadosChamado.CodigoFilial))
        return false;

    var dataCriacaoChamado = Global.criarData(dadosChamado.DataCriacaoChamado);

    if (_pesquisaChamados.DataInicial.val()) {
        var dataInicial = Global.criarData(_pesquisaChamados.DataInicial.val());

        if (moment(dataInicial).isAfter(dataCriacaoChamado))
            return false;
    }

    if (_pesquisaChamados.DataFinal.val()) {
        var dataFinal = Global.criarData(_pesquisaChamados.DataFinal.val());

        if (moment(dataFinal).isBefore(dataCriacaoChamado))
            return false;
    }

    var listaCodigosTransportador = recursiveMultiplesEntities(_pesquisaChamados.Transportador);

    if ((listaCodigosTransportador.length > 0) && (listaCodigosTransportador.indexOf(dadosChamado.CodigoTransportador.toString()) < 0))
        return false;

    return true;
}
