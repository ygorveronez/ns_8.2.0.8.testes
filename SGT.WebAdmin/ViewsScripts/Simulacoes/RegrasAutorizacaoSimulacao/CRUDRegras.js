/// <reference path="RegrasAutorizacaoSimulacao.js" />
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
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

function loadCRUDRegras() {
    _CRUDRegras = new CRUDRegras();
    KoBindings(_CRUDRegras, "knockoutCRUDRegrasAutorizacaoSimulacao");
}



//*******EVENTOS*******

function atualizarClick(e, sender) {
    SalvarRegras("RegrasAutorizacaoSimulacao/Atualizar", "Atualizado com sucesso", e, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir as regras?", function () {
        ExcluirPorCodigo(_regraSimulacao, "RegrasAutorizacaoSimulacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Excluido com sucesso");
                    _gridRegraSimulacao.CarregarGrid();
                    LimparTodosCampos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "", arg.Msg, 16000);
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
    SalvarRegras("RegrasAutorizacaoSimulacao/Adicionar", "Cadastrado com sucesso", e, sender);
}



//*******MÉTODOS*******

function ExibirCamposObrigatorio() {
    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
}

function ValidacaoGeral() {

    var msg_padrao = `Nenhuma regra por #nomeregra# cadastrada.`;
    var msg_regras = [];
    var totalAprovadores = _regraSimulacao.GridAprovadores.val().length;
    var numeroAprovadores = _regraSimulacao.NumeroAprovadores.val() || 0;

    if (totalAprovadores < numeroAprovadores)
        return [`O número de aprovadores selecionados deve ser maior ou igual ao número de aprovações ${numeroAprovadores}`];

    //-- Valida as regras
    // TipoOcorrencia
    //if (_tipoSimulacao.Regras.required() && _tipoSimulacao.Regras.val().length == 0)
    //    msg_regras.push(msg_padrao.replace("#nomeregra#", "Tipo de Simulação"));

    // FilialEmissao
    if (_filialEmissao.Regras.required() && _filialEmissao.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Filial de Emissão"));

    // TipoOperacao
    if (_tipoOperacao.Regras.required() && _tipoOperacao.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Tipo de Operação"));

    return msg_regras;
}

function SalvarRegras(url, mensagemSucesso, e, sender) {
    msg_regras = ValidacaoGeral();

    if (msg_regras.length > 0)
        exibirMensagem(tipoMensagem.atencao, "Regra(s) inválida(s)", msg_regras.join("<br>"));
    else {
        Salvar(_regraSimulacao, url, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, mensagemSucesso);
                    _gridRegraSimulacao.CarregarGrid();
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