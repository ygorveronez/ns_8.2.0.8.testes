/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="DetalhesPedido.js" />

// #region Objetos Globais do Arquivo

var _selecaoMultiplosPedidos;
var _gridSelecaoMultiplosPedidos;

// #endregion Objetos Globais do Arquivo

// #region Classes

var SelecaoMultiplosPedidos = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
    this.Justificativa = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Justificativa.getRequiredFieldDescription(), maxlength: 50, getType: typesKnockout.text, visible: ko.observable(false), required: false });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: Localization.Resources.Gerais.Geral.MarcarTodos, visible: ko.observable(true) });
    this.RemoverPedido = PropertyEntity({ eventClick: removerPedidosCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.RemoverPedidos, visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadSelecaoMultiplosPedidos() {
    _selecaoMultiplosPedidos = new SelecaoMultiplosPedidos();
    KoBindings(_selecaoMultiplosPedidos, "divModalSelecaoMultiplosPedidos");

    if (habilitarJustificativaRemocaoDosPedidos()) {
        _selecaoMultiplosPedidos.Justificativa.visible(true);
        _selecaoMultiplosPedidos.Justificativa.required = true;
    }
        
}

// #endregion Funções de Inicialização

//#region Eventos

async function removerPedidosCargaClick() {

    if (!ValidarCamposObrigatorios(_selecaoMultiplosPedidos)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return;
    }

    if (!existeRegistrosSelecionadosNaGrid()) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.NenhumRegistroSelecionado, Localization.Resources.Gerais.Geral.NenhumRegistroSelecionado);
        return;
    }

    const confirmado = await exibirConfirmacaoAsync(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteRemoverPedidos);

    if (!confirmado) return;

    const listaPedidos = _gridSelecaoMultiplosPedidos.ObterMultiplosSelecionados();
    const totalPedidos = listaPedidos.length;

    for (const [index, registro] of listaPedidos.entries()) {
        const data = {
            CodigoPedido: registro.Codigo,
            CodigoCarga: _selecaoMultiplosPedidos.CodigoCarga.val(),
            PermitirRemoverTodos: false,
            Justificativa: _selecaoMultiplosPedidos.Justificativa.val(),
        };

        await removerPedidoDetalhePedidoAsync(data, registro.NumeroPedidoEmbarcador, index + 1, totalPedidos);
    }

    LimparCampo(_selecaoMultiplosPedidos.Justificativa);
}

//#endregion Eventos

// #region Funções Públicas

function exibirModalSelecaoMultiplosPedidos() {
    Global.abrirModal('divModalSelecaoMultiplosPedidos');
    LimparCampo(_selecaoMultiplosPedidos.Justificativa);
}

function carregarPedidosCargaMultiplaSelecao(carga) {
    BuscarPedidosMultiplaSelecaoCarga(carga);
}

// #endregion Funções Públicas

// #region Funções Privadas

function BuscarPedidosMultiplaSelecaoCarga(carga)
{
    _selecaoMultiplosPedidos.CodigoCarga.codEntity(carga);
    _selecaoMultiplosPedidos.CodigoCarga.val(carga);

    let multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _selecaoMultiplosPedidos.SelecionarTodos,
        somenteLeitura: false
    };

    _gridSelecaoMultiplosPedidos = new GridView("grid-selecao-multiplos-pedidos", "Pedido/PesquisaPedidosCarga", _selecaoMultiplosPedidos, null, null, null, null, null, null, multiplaescolha);
    _gridSelecaoMultiplosPedidos.CarregarGrid();
}

function habilitarJustificativaRemocaoDosPedidos() {
    return _CONFIGURACAO_TMS.Carga.SolicitarJustificativaAoRemoverPedidoCarga;
}

function existeRegistrosSelecionadosNaGrid() {
    return _gridSelecaoMultiplosPedidos.ObterMultiplosSelecionados().length > 0
}

async function removerPedidoDetalhePedidoAsync(data, pedidoEmbarcador, indiceAtual, totalPedidos) {

    const mensagem = Localization.Resources.Cargas.Carga.ProcessandoPedidoRemovido.format(pedidoEmbarcador, indiceAtual, totalPedidos);
    iniciarRequisicaoComMensagemPersonalizada(mensagem);

    const retorno = await executarReSTAsync("Carga/RemoverPedidoCarga", data);

    if (!retorno.Success) {
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        return;
    }

    if (!retorno.Data) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        return;
    }

    if (retorno.Data.NaoPermitirRemoverUltimoPedidoCarga) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Cargas.Carga.NaoPossivelRemoverUltimoPedidoSeNecessarioDeveSeCancelarCargaPeloFluxoDeCancelamento, 16000);
        return;
    }

    if (retorno.Data.ConfirmarRemocaoPedidoViculadoCarga) {

        const confirmado = await exibirConfirmacaoAsync(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.EsseUltimoPedidoDaCargaSuaRemocaoIraCancelarCargaDesejaProsseguir);

        if (!confirmado) return;

        await removerPedidoDetalhePedidoAsync({
            CodigoPedido: data.CodigoPedido,
            CodigoCarga: data.CodigoCarga,
            PermitirRemoverTodos: true,
            PermitirSeparacaoMercadoriaInformada: data.PermitirSeparacaoMercadoriaInformada,
            PermitirAlteracoesPedidos: data.PermitirAlteracoesPedidos
        });

        return;
    }

    if (retorno.Data.ConfirmarSeparacaoMercadoriaInformada) {
        const confirmado = await exibirConfirmacaoAsync(Localization.Resources.Gerais.Geral.Confirmacao, retorno.Data.MensagemErro + " " + Localization.Resources.Gerais.Geral.DesejaRealmenteProsseguir);

        if (!confirmado) return;

        await removerPedidoDetalhePedidoAsync({
            CodigoPedido: data.CodigoPedido,
            CodigoCarga: data.CodigoCarga,
            PermitirRemoverTodos: data.PermitirRemoverTodos,
            PermitirSeparacaoMercadoriaInformada: true,
            PermitirAlteracoesPedidos: data.PermitirAlteracoesPedidos
        });

        return;
    }

    if (retorno.Data.ConfirmarAlteracoesPedidos) {

        const confirmado = await exibirConfirmacaoAsync(Localization.Resources.Gerais.Geral.Confirmacao, retorno.Data.MensagemErro + " " + Localization.Resources.Gerais.Geral.DesejaRealmenteProsseguir);

        if (!confirmado) return;

        await removerPedidoDetalhePedidoAsync({
            CodigoPedido: data.CodigoPedido,
            CodigoCarga: data.CodigoCarga,
            PermitirRemoverTodos: data.PermitirRemoverTodos,
            PermitirSeparacaoMercadoriaInformada: data.PermitirSeparacaoMercadoriaInformada,
            PermitirAlteracoesPedidos: true
        });

        return;
    }

    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.PedidoNumeroRemovidoComSucesso.format(pedidoEmbarcador));
    exibirDetalhesPedidos(_detalhePedidoContainer.CodigoCarga.val());
    IniciarBindKnoutCarga(_cargaAtual, retorno.Data);
    finalizarRequisicao();

}

function exibirConfirmacaoAsync(titulo, mensagem) {
    return new Promise(resolve => {
        exibirConfirmacao(titulo, mensagem, () => resolve(true));
    });
}

function executarReSTAsync(url, data) {
    return new Promise((resolve, reject) => {
        executarReST(url, data, (retorno) => {
            if (retorno.Success) resolve(retorno);
            else reject(retorno);
        }, null,false);
    });
}

// #endregion Funções Privadas
