/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoTransportadorCentroCarregamento.js" />
/// <reference path="../../Consultas/Tranportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _tipoTransportadorCarga;

/*
 * Declaração das Classes
 */

var TipoTransportadorCarga = function () {
    var tipoTransportadorPadrao = _CONFIGURACAO_TMS.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela ? EnumTipoTransportadorCentroCarregamento.Todos : EnumTipoTransportadorCentroCarregamento.PorPrioridadeDeRota;

    this.Codigos;
    this.DataCarregamento = PropertyEntity({ getType: typesKnockout.date, visible: false });
    this.LiberarTodasCargas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: false });
    this.TipoTransportador = PropertyEntity({ val: ko.observable(tipoTransportadorPadrao), options: ko.observable(EnumTipoTransportadorCentroCarregamento.obterOpcoes()), def: tipoTransportadorPadrao, text: Localization.Resources.Cargas.Carga.TipoDeTransportador.getRequiredFieldDescription(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Transportador.getRequiredFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.ObservacaoTransportador = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.ObservacoesParaOsTransportadorEs.getRequiredFieldDescription(), maxlength: 500, visible: ko.observable(true) });

    this.TipoTransportador.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoTransportadorCentroCarregamento.TransportadorExclusivo) {
            _tipoTransportadorCarga.Transportador.visible(true);
            _tipoTransportadorCarga.Transportador.required = true;
        }
        else {
            _tipoTransportadorCarga.Transportador.visible(false);
            _tipoTransportadorCarga.Transportador.required = false;
            _tipoTransportadorCarga.Transportador.val("");
            _tipoTransportadorCarga.Transportador.codEntity(0);
        }
    });

    this.SelecionarTipoTransportador = PropertyEntity({ eventClick: selecionarTipoTransportadorClick, type: types.event, text: Localization.Resources.Cargas.Carga.Liberar, idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadTipoTransportadorCarga() {
    _tipoTransportadorCarga = new TipoTransportadorCarga();

    KoBindings(_tipoTransportadorCarga, "divModalSelecaoTipoTransportador");

    BuscarTransportadores(_tipoTransportadorCarga.Transportador, null, null, true);

}

/*
 * Declaração das Funções Associadas a Eventos
 */

function selecionarTipoTransportadorClick() {
    if (_tipoTransportadorCarga.LiberarTodasCargas.val())
        selecionarTipoTransportadorPorDataCarregamento();
    else
        selecionarTipoTransportador();
}


function obterConfiguracaoTipoTransportador() {
    let p = new promise.Promise();
    executarReST("ConfigOperador/ObterTipoTransportadorConfigurado", {}, (arg) => {

        if (!arg.Success) {
            p.done();
            return exibirMensagem(tipoMensagem.falha, arg.Msg);
        }
        p.done(arg.Data);
    })
    return p;
}

/*
 * Declaração das Funções Públicas
 */

function exibirTipoTransportadorCarga(dados, sender) {
    if (sender)
        sender.stopPropagation();

    var liberarCargaManualmenteParaTransportador = (_centroCarregamentoAtual.LiberarCargaManualmenteParaTransportadores && (dados.Transportador.Codigo > 0) && !_CONFIGURACAO_TMS.LiberarCargaParaCotacaoAoLiberarParaTransportadores);

    _tipoTransportadorCarga.Codigos = [dados.Codigo];
    _tipoTransportadorCarga.ObservacaoTransportador.val(dados.ObservacaoTransportador);
    _tipoTransportadorCarga.TipoTransportador.options(_centroCarregamentoAtual.PermitirLiberarCargaTransportadorExclusivo || liberarCargaManualmenteParaTransportador ? EnumTipoTransportadorCentroCarregamento.obterOpcoesPermissaoLiberarCargaTransportadorExclusivo() : EnumTipoTransportadorCentroCarregamento.obterOpcoes())

    if (liberarCargaManualmenteParaTransportador) {
        _tipoTransportadorCarga.TipoTransportador.val(EnumTipoTransportadorCentroCarregamento.TransportadorExclusivo);
        _tipoTransportadorCarga.TipoTransportador.visible(false);

        _tipoTransportadorCarga.Transportador.val(dados.Transportador.Descricao);
        _tipoTransportadorCarga.Transportador.entityDescription(dados.Transportador.Descricao);
        _tipoTransportadorCarga.Transportador.codEntity(dados.Transportador.Codigo);
        _tipoTransportadorCarga.Transportador.enable(false);
    }

    exibirModalTipoTransportadorCarga();
}

function exibirTipoTransportadorCargaPorDataCarregamento() {
    if (!_dadosPesquisaCarregamento)
        return;

    _tipoTransportadorCarga.DataCarregamento.val(_dadosPesquisaCarregamento.DataCarregamento);
    _tipoTransportadorCarga.LiberarTodasCargas.val(true);
    _tipoTransportadorCarga.TipoTransportador.options(_centroCarregamentoAtual.PermitirLiberarCargaTransportadorExclusivo ? EnumTipoTransportadorCentroCarregamento.obterOpcoesPermissaoLiberarCargaTransportadorExclusivo() : EnumTipoTransportadorCentroCarregamento.obterOpcoes())

    exibirModalTipoTransportadorCarga();
}

function exibirTipoTransportadorCargas(codigos) {
    _tipoTransportadorCarga.Codigos = codigos;
    _tipoTransportadorCarga.TipoTransportador.options(_centroCarregamentoAtual.PermitirLiberarCargaTransportadorExclusivo ? EnumTipoTransportadorCentroCarregamento.obterOpcoesPermissaoLiberarCargaTransportadorExclusivo() : EnumTipoTransportadorCentroCarregamento.obterOpcoes())

    exibirModalTipoTransportadorCarga();
}

/*
 * Declaração das Funções Privadas
 */

function exibirModalTipoTransportadorCarga() {

    obterConfiguracaoTipoTransportador().then((resul) => {

        Global.abrirModal('divModalSelecaoTipoTransportador');

        if (resul && resul.length > 0)
            _tipoTransportadorCarga.TipoTransportador.options(resul)

        $("#divModalSelecaoTipoTransportador").one('hidden.bs.modal', function () {
            LimparCampos(_tipoTransportadorCarga);
        });
    })

}

function fecharModalTipoTransportadorCarga() {
    Global.fecharModal('divModalSelecaoTipoTransportador');
}

function selecionarTipoTransportador() {
    var dados = RetornarObjetoPesquisa(_tipoTransportadorCarga);

    dados.Codigos = JSON.stringify(_tipoTransportadorCarga.Codigos);

    executarReST("JanelaCarregamento/SalvarTipoTransportadorCarga", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data != false) {
                if (retorno.Data.length == 1) {
                    var registroRetornado = retorno.Data[0];

                    if (registroRetornado.CodigoErro == 0) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, registroRetornado.MensagemRetorno);
                        fecharModalTipoTransportadorCarga();
                    }
                    else if (registroRetornado.CodigoErro == 1)
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, registroRetornado.MensagemRetorno);
                    else if (registroRetornado.CodigoErro == 2)
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, registroRetornado.MensagemRetorno);
                    else if (registroRetornado.CodigoErro == 3) {
                        BuscarPercursoParaMDFe(registroRetornado.CodigoCarga);
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, registroRetornado.MensagemRetorno);
                    }
                }
                else {
                    exibirRetornoMultiplaSelecao(retorno.Data);
                    fecharModalTipoTransportadorCarga();
                }
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function selecionarTipoTransportadorPorDataCarregamento() {
    var dados = RetornarObjetoPesquisa(_tipoTransportadorCarga);

    executarReST("JanelaCarregamento/SalvarTipoTransportadorCargaPorDataCarregamento", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.TipoDeTransportadorSelecionadoComSucesso);
                fecharModalTipoTransportadorCarga();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}
