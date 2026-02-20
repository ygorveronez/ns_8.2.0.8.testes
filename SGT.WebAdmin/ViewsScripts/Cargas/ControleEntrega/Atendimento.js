var _controleExibicaoAtendimentoCarregado = false;
var _atendimentos;


var Atendimentos = function () {
    this.VisualizarAtendimentosOperadorResponsavel = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.VisualizarAtendimentosComOperadorResponsavel.getFieldDescription(), val: ko.observable(false), def: false });
    this.VisualizarAtendimentosOperadorResponsavel.val.subscribe(function () {
        AtualizarListaAtendimentos(false);
    });
    this.Atendimentos = PropertyEntity({ val: ko.observableArray([]), view: ko.pureComputed(function () { return OrdernarAtendimentos(); }), eventClick: AbrirAtendimentoClick });
    this.Chat = PropertyEntity({ eventClick: AbrirChatAtendimentoClick });
}

//*******EVENTOS*******
function loadAtendimento() {
    ControleExibicaoAtendimento();
    _atendimentos = new Atendimentos();
    KoBindings(_atendimentos, "knoutContainerAtendimentos");

    var exibirAtendimento = _CONFIGURACAO_TMS.HabilitarWidgetAtendimento && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe;
    if (exibirAtendimento) {
        ExibirWidgetAtendimento();

        if (!_CONFIGURACAO_TMS.FiltrarWidgetAtendimentoProFiltro)
            AtualizarListaAtendimentos(false);
    } else {
        OcultarBotaoWidgetAtendimento();
    }
}

function atendimentoAtualizadoEvent(content) {
    if (!_CONFIGURACAO_TMS.HabilitarWidgetAtendimento)
        return;

    var visualizarAtendimentosOperadorResponsavel = _atendimentos.VisualizarAtendimentosOperadorResponsavel.val();
    var atendimento = JSON.parse(content);
    var atendimentoEncontrado = null;
    var atendimentos = _atendimentos.Atendimentos.val().slice();
    var index = 0;

    for (; index < atendimentos.length; index++) {
        if (atendimentos[index].Codigo == atendimento.Codigo) {
            atendimentoEncontrado = true;
            break;
        }
    }

    if (atendimentoEncontrado == null)
        return;

    if (atendimento.Responsavel != null && !visualizarAtendimentosOperadorResponsavel)
        atendimentos.splice(index, 1);
    else
        atendimentos[index] = atendimento;

    _atendimentos.Atendimentos.val(atendimentos);
}

function novoAtendimentoEvent(content) {
    if (!_CONFIGURACAO_TMS.HabilitarWidgetAtendimento)
        return;

    var visualizarAtendimentosOperadorResponsavel = _atendimentos.VisualizarAtendimentosOperadorResponsavel.val();
    var atendimento = JSON.parse(content);

    if (atendimento.Responsavel != null && !visualizarAtendimentosOperadorResponsavel)
        return;

    _atendimentos.Atendimentos.val.push(atendimento);
}

function AbrirChatAtendimentoClick(dados) {
    loadMobileChatControleEntrega({
        Carga: dados.Carga,
        NumeroMotorista: dados.NumeroMotorista,
        NomeMotorista: dados.NomeMotorista,
    });
}

function AbrirAtendimentoClick(dados) {
    AbrirChamadoPorCodigo(dados.Codigo);
}

function AbrirNovoAtendimentoClick(e) {
    CriarNovoAtendimento(e, "divModalChamadoOcorrencia");
}

//*******MÉTODOS*******
function ControleExibicaoAtendimento() {
    if (_controleExibicaoAtendimentoCarregado) return;
    _controleExibicaoAtendimentoCarregado = true;

    $('body').on('click', '.ocultar-atentimento', function () {
        OcultarWidgetAtendimento();
    });

    $('body').on('click', '.exibir-atentimento', function () {
        ExibirWidgetAtendimento();
    });
}

function OcultarBotaoWidgetAtendimento() {
    $(".exibir-atentimento").addClass("d-none");
}

function ExibirWidgetAtendimento() {
    $(".controle-entrega-atendimento").removeClass("d-none");
    $(".controle-entrega-conteudo").removeClass("col-8").addClass($(".controle-entrega-conteudo").data('class-half'));
    $(".exibir-atentimento").addClass("d-none");
}

function OcultarWidgetAtendimento() {
    $(".controle-entrega-atendimento").addClass("d-none");
    $(".controle-entrega-conteudo").removeClass($(".controle-entrega-conteudo").data('class-half')).addClass("col-8");
    $(".exibir-atentimento").removeClass("d-none");
}

function AbrirChamadoPorCodigo(codigo) {
    buscarChamadoPorCodigo(codigo, function () {
        let $modalChamado = $("#divModalChamadoOcorrencia");

        Global.abrirModal("divModalChamadoOcorrencia");

        $modalChamado.one('hidden.bs.modal', function () {
            $(window).one('keyup', function (e) {
                if (e.keyCode == 27)
                    Global.fecharModal("divModalEntrega");
            });
        });

        $(window).unbind('keyup');
        $(window).one('keyup', function (e) {
            if (e.keyCode == 27)
                Global.fecharModal("divModalChamadoOcorrencia");
        });
    });
}

function AtualizarListaAtendimentos(atualizacaoAutomatica) {
    var data = _CONFIGURACAO_TMS.FiltrarWidgetAtendimentoProFiltro ? RetornarObjetoPesquisa(_pesquisaControleEntrega) : {};

    data.VisualizarAtendimentosOperadorResponsavel = _atendimentos.VisualizarAtendimentosOperadorResponsavel.val();

    executarReST("ControleEntregaAtendimento/BuscarAtendimentos", data, function (arg) {
        if (!arg.Success) {
            if (!atualizacaoAutomatica) exibirMensagem(tipoMensagem.falha, Localization.Resouces.Gerais.Geral.Falha, arg.Msg);
            return;
        }

        if (arg.Data == false) {
            if (!atualizacaoAutomatica) exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            return;
        }

        _atendimentos.Atendimentos.val(arg.Data.Atendimento);
    });
}

function OrdernarAtendimentos() {
    return _atendimentos.Atendimentos.val.sorted(function (esquerda, direita) {
        if (esquerda.Timestamp === direita.Timestamp)
            return 0;

        if (esquerda.Timestamp < direita.Timestamp)
            return -1;
        else
            return 1;
    });
}