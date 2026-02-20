/// <reference path="RegrasOrdemCompra.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />

//*******EVENTOS*******

function atualizarClick(e, sender) {
    SalvarRegras("RegrasOrdemCompra/Atualizar", "Atualizado com sucesso.", e, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir as regras?", function () {
        ExcluirPorCodigo(_regrasOrdemCompra, "RegrasOrdemCompra/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRegrasOrdemCompra.CarregarGrid();
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
    SalvarRegras("RegrasOrdemCompra/Adicionar", "Cadastrado com sucesso.", e, sender);
}

//*******MÉTODOS*******

function ExibirCamposObrigatorio() {
    exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function ValidacaoGeral() {
    var msg_padrao = "Nenhuma regra por #nomeregra# cadastrada.";
    var msg_regras = [];


    //-- Valida aprovadores
    if (_gridAprovadores.BuscarRegistros().length < _regrasOrdemCompra.NumeroAprovadores.val())
        return ["O número de aprovadores selecionados deve ser maior ou igual a " + _regrasOrdemCompra.NumeroAprovadores.val()];


    //-- Valida as regras
    _configRegras.Alcadas.forEach(function (alcada) {
        if (_regrasOrdemCompra["UsarRegraPor" + alcada.prop].val() && _regrasOrdemCompra["Alcadas" + alcada.prop].val().length == 0)
            msg_regras.push(msg_padrao.replace("#nomeregra#", alcada.descricao));
    });

    return msg_regras;
}

function SalvarRegras(url, mensagemSucesso, e, sender) {
    msg_regras = ValidacaoGeral();

    if (msg_regras.length > 0)
        return exibirMensagem(tipoMensagem.atencao, "Regra(s) inválida(s)", msg_regras.join("<br>"));

    if (!ValidarCamposObrigatorios(_regrasOrdemCompra))
        return ExibirCamposObrigatorio();

    executarReST(url, GeraDadosRegras(), function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", mensagemSucesso);
                _gridRegrasOrdemCompra.CarregarGrid();
                LimparTodosCampos();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function GeraDadosRegras() {
    var dados = {
        Codigo: _regrasOrdemCompra.Codigo.val(),
        Descricao: _regrasOrdemCompra.Descricao.val(),
        Vigencia: _regrasOrdemCompra.Vigencia.val(),
        NumeroAprovadores: _regrasOrdemCompra.NumeroAprovadores.val(),
        Observacao: _regrasOrdemCompra.Observacao.val(),
        Aprovadores: ListaAprovadores(),
    };

    _configRegras.Alcadas.forEach(function (alcada) {
        dados["UsarRegraPor" + alcada.prop] = _regrasOrdemCompra["UsarRegraPor" + alcada.prop].val();
        dados["Alcadas" + alcada.prop] = JSON.stringify(_regrasOrdemCompra["Alcadas" + alcada.prop].val());
    });

    return dados;
}