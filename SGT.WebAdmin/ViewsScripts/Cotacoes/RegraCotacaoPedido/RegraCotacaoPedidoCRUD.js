/// <reference path="RegraCotacaoPedido.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />


//*******EVENTOS*******

function atualizarClick(e, sender) {
    SalvarRegras("RegraCotacaoPedido/Atualizar", "Atualizado com sucesso.", e, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir as regras?", function () {
        ExcluirPorCodigo(_regraCotacaoPedido, "RegraCotacaoPedido/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRegraCotacaoPedido.CarregarGrid();
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
    SalvarRegras("RegraCotacaoPedido/Adicionar", "Cadastrado com sucesso.", e, sender);
}



//*******MÉTODOS*******

function ExibirCamposObrigatorio() {
    exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function ValidacaoGeral() {
    var msg_padrao = "Nenhuma regra por #nomeregra# cadastrada.";
    var msg_regras = [];


    //-- Valida aprovadores
    if (_gridAprovadores.BuscarRegistros().length < _regraCotacaoPedido.NumeroAprovadores.val())
        return ["O número de aprovadores selecionados deve ser maior ou igual a " + _regraCotacaoPedido.NumeroAprovadores.val()];


    //-- Valida as regras
    _configRegras.Alcadas.forEach(function (alcada) {
        if (_regraCotacaoPedido["UsarRegraPor" + alcada.prop].val() && _regraCotacaoPedido["Alcadas" + alcada.prop].val().length == 0)
            msg_regras.push(msg_padrao.replace("#nomeregra#", alcada.descricao));
    });

    return msg_regras;
}

function SalvarRegras(url, mensagemSucesso, e, sender) {
    msg_regras = ValidacaoGeral();

    if (msg_regras.length > 0)
        return exibirMensagem(tipoMensagem.atencao, "Regra(s) inválida(s)", msg_regras.join("<br>"));

    if (!ValidarCamposObrigatorios(_regraCotacaoPedido))
        return ExibirCamposObrigatorio();

    executarReST(url, GeraDadosRegras(), function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", mensagemSucesso);
                _gridRegraCotacaoPedido.CarregarGrid();
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
        Codigo: _regraCotacaoPedido.Codigo.val(),
        Descricao: _regraCotacaoPedido.Descricao.val(),
        Vigencia: _regraCotacaoPedido.Vigencia.val(),
        NumeroAprovadores: _regraCotacaoPedido.NumeroAprovadores.val(),
        Observacao: _regraCotacaoPedido.Observacao.val(),
        Aprovadores: ListaAprovadores(),
    };

    _configRegras.Alcadas.forEach(function (alcada) {
        dados["UsarRegraPor" + alcada.prop] = _regraCotacaoPedido["UsarRegraPor" + alcada.prop].val();
        dados["Alcadas" + alcada.prop] = JSON.stringify(_regraCotacaoPedido["Alcadas" + alcada.prop].val());
    });

    return dados;
}