/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="RegrasAutorizacaoTabelaFrete.js" />

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
    KoBindings(_CRUDRegras, "knockoutCRUDRegras");
}



//*******EVENTOS*******

function atualizarClick(e, sender) {
    SalvarRegras("RegrasAutorizacaoTabelaFrete/Atualizar", "Atualizado com sucesso.", e, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir as regras?", function () {
        ExcluirPorCodigo(_regraTabelaFrete, "RegrasAutorizacaoTabelaFrete/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRegraTabelaFrete.CarregarGrid();
                    LimparTodosCampos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e, sender) {
    LimparTodosCampos();
}

function adicionarClick(e, sender) {
    SalvarRegras("RegrasAutorizacaoTabelaFrete/Adicionar", "Cadastrado com sucesso.", e, sender);
}



//*******MÉTODOS*******

function ExibirCamposObrigatorio() {
    exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function ValidacaoGeral() {
    var msg_padrao = "Nenhuma regra por #nomeregra# cadastrada.";
    var msg_regras = [];

    if ((_regraTabelaFrete.TipoAprovadorRegra.val() == EnumTipoAprovadorRegra.Usuario) && (_regraTabelaFrete.GridAprovadores.val().length < _regraTabelaFrete.NumeroAprovadores.val()))
        return ["O número de aprovadores selecionados deve ser maior ou igual a " + _regraTabelaFrete.NumeroAprovadores.val()];


    //-- Valida as regras
    // Motivo do Reajuste
    if (_motivoReajuste.Regras.required() && _motivoReajuste.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Motivo do Reajuste"));

    // Origem
    if (_origemFrete.Regras.required() && _origemFrete.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Origem"));

    // Destino
    if (_destinoFrete.Regras.required() && _destinoFrete.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Destino"));

    // Transportador
    if (_transportador.Regras.required() && _transportador.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Transportador"));

    // Valor do Frete
    if (_valorFrete.Regras.required() && _valorFrete.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Valor do Frete"));

    // Valor do Pedágio
    if (_valorPedagio.Regras.required() && _valorPedagio.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Valor do Pedágio"));

    // AdValorem
    if (_adValorem.Regras.required() && _adValorem.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "AdValorem"));

    // AdValorem
    if (_filial.Regras.required() && _filial.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Filial"));

    return msg_regras;
}

function SalvarRegras(url, mensagemSucesso, e, sender) {
    msg_regras = ValidacaoGeral();

    if (msg_regras.length > 0)
        exibirMensagem(tipoMensagem.atencao, "Regra(s) inválida(s)", msg_regras.join("<br>"));
    else {
        Salvar(_regraTabelaFrete, url, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", mensagemSucesso);
                    _gridRegraTabelaFrete.CarregarGrid();
                    LimparTodosCampos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender, ExibirCamposObrigatorio)
    }
}