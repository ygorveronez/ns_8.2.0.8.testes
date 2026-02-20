/// <reference path="RegrasAutorizacaoOcorrencia.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _CRUDRegras;

var CRUDRegras = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Adicionar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Cancelar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Excluir, visible: ko.observable(false) });
}

function loadCRUDRegras() {
    _CRUDRegras = new CRUDRegras();
    KoBindings(_CRUDRegras, "knockoutCRUDRegras");
}



//*******EVENTOS*******

function atualizarClick(e, sender) {
    SalvarRegras("RegrasAutorizacaoOcorrencia/Atualizar", Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.AtualizadoComSucesso, e, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Confirmacao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RealmenteDesejaExcluirRegras, function () {
        ExcluirPorCodigo(_regraOcorrencia, "RegrasAutorizacaoOcorrencia/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ExcluidoComSucesso);
                    _gridRegraOcorrencia.CarregarGrid();
                    LimparTodosCampos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e, sender) {
    LimparTodosCampos();
}

function adicionarClick(e, sender) {
    SalvarRegras("RegrasAutorizacaoOcorrencia/Adicionar", Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.CadastradoComSucesso, e, sender);
}



//*******MÉTODOS*******

function ExibirCamposObrigatorio() {
    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
}

function ValidacaoGeral() {

    var msg_padrao = Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.NenhumaRegraPorNomeRegraCadastrada;
    var msg_regras = [];
    var totalAprovadores = _regraOcorrencia.GridAprovadores.val().length;
    var numeroAprovadores = _regraOcorrencia.NumeroAprovadores.val() || 0;

    if (totalAprovadores < numeroAprovadores)
        return [Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.NumeroAprovadoresSelecionadosDevesSerMaiorIgual.format(numeroAprovadores)];

    //-- Valida as regras
    // TipoOcorrencia
    if (_tipoOcorrencia.Regras.required() && _tipoOcorrencia.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoOcorrencia));

    // VomponenteFrete
    if (_componenteFrete.Regras.required() && _componenteFrete.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ComponenteFrete));

    // FilialEmissao
    if (_filialEmissao.Regras.required() && _filialEmissao.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.FilialEmissao));

    // TomadorOcorrencia
    if (_tomadorOcorrencia.Regras.required() && _tomadorOcorrencia.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TomadorOcorrencia));

    // ValorOcorrencia
    if (_valorOcorrencia.Regras.required() && _valorOcorrencia.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ValorOcorrencia));

    // TipoOperacao
    if (_tipoOperacao.Regras.required() && _tipoOperacao.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoOperacao));

    return msg_regras;
}

function SalvarRegras(url, mensagemSucesso, e, sender) {
    msg_regras = ValidacaoGeral();

    if (msg_regras.length > 0)
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegrasInvalidas, msg_regras.join("<br>"));
    else {
        Salvar(_regraOcorrencia, url, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, mensagemSucesso);
                    _gridRegraOcorrencia.CarregarGrid();
                    LimparTodosCampos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender, ExibirCamposObrigatorio)
    }
}