/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoAplicacaoColetaEntrega.js" />
/// <reference path="../../Enumeradores/EnumTipoColetaEntregaDevolucao.js" />
/// <reference path="../ControleEntregaDevolucao/ControleEntregaDevolucao.js" />
/// <reference path="Entrega.js" />

// #region Objetos Globais do Arquivo

var _controleEntregaDevolucaoDados;
var _controleEntregaDevolucao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var ControleEntregaDevolucaoDados = function () {
    this.ExibirDadosDevolucao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.MotivoRejeicao = PropertyEntity({ val: ko.observable(0), options: ko.observable(new Array()), def: 0, text: Localization.Resources.Cargas.ControleEntrega.MotivoRejeicao.getFieldDescription(), visible: ko.observable(true), enable: ko.observable(true) });
    this.PermitirEntregarMaisTarde = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PermitirQueEntregaSejaFeitaMaisTarde, val: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoDevolucao = PropertyEntity({ val: ko.observable(EnumTipoColetaEntregaDevolucao.Total), options: ko.observable(EnumTipoColetaEntregaDevolucao.obterOpcoes()), def: EnumTipoColetaEntregaDevolucao.Total, text: Localization.Resources.Cargas.ControleEntrega.TipoDeDevolucao.getFieldDescription(), visible: ko.observable(true), enable: ko.observable(true), idFade: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadControleEntregaDevolucaoDados() {
    _controleEntregaDevolucaoDados = new ControleEntregaDevolucaoDados();
    _controleEntregaDevolucao = new ControleEntregaDevolucaoContainer("controle-entrega-devolucao-container");
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function confirmarRejeicaoColetaEntregaClick() {
    if (!ValidarAcaoMultiCTe())
        return;

    var dados = {
        Codigo: _entrega.Codigo.val(),
        MotivoRejeicao: _controleEntregaDevolucaoDados.MotivoRejeicao.val(),
        MotivoRetificacao: _entrega.MotivoRetificacao.val(),
        PermitirEntregarMaisTarde: _controleEntregaDevolucaoDados.PermitirEntregarMaisTarde.val(),
        Observacao: _entrega.Observacao.val(),
        TipoDevolucao: _controleEntregaDevolucaoDados.TipoDevolucao.val(),
        ItensDevolver: _controleEntregaDevolucao.obter()
    };

    executarReST("ControleEntregaEntrega/RejeitarEntrega", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.RejeitadoComSucesso);
                atualizarControleEntrega();

                Global.fecharModal("divModalEntrega");
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function preencherControleEntregaDevolucaoDados(dadosDevolucao, codigoChamado) {
    carregarMotivosRejeicao(function () {
        PreencherObjetoKnout(_controleEntregaDevolucaoDados, { Data: dadosDevolucao });
        _controleEntregaDevolucao.preencher(_entrega.Codigo.val(), _entrega.RejeitarEntrega.visible(), codigoChamado, null, null, true);
        controlarCamposHabilitadosControleEntregaDevolucaoDados(_entrega.RejeitarEntrega.visible());
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function carregarMotivosRejeicao(callback) {
    _controleEntregaDevolucaoDados.MotivoRejeicao.options([]);

    var tipoAplicacaoColetaEntrega = _entrega.Coleta.val() ? EnumTipoAplicacaoColetaEntrega.Coleta : EnumTipoAplicacaoColetaEntrega.Entrega;

    executarReST("TipoOcorrencia/BuscarOcorrenciasMobile", { UsadoParaMotivoRejeicaoColetaEntrega: true, TipoAplicacaoColetaEntrega: tipoAplicacaoColetaEntrega, Codigo: _entrega.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var opcoesMotivoRejeicao = [];

                for (var i = 0; i < retorno.Data.length; i++)
                    opcoesMotivoRejeicao.push({ text: retorno.Data[i].Descricao, value: retorno.Data[i].Codigo });

                _controleEntregaDevolucaoDados.MotivoRejeicao.options(opcoesMotivoRejeicao);
                callback();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function controlarCamposHabilitadosControleEntregaDevolucaoDados(habilitar) {
    _controleEntregaDevolucaoDados.MotivoRejeicao.enable(habilitar);
    _controleEntregaDevolucaoDados.PermitirEntregarMaisTarde.enable(habilitar);
    _controleEntregaDevolucaoDados.TipoDevolucao.enable(habilitar);
}

// #endregion Funções Privadas
