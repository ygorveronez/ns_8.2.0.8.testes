/// <reference path="RegrasDescarteLoteProduto.js" />
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
    KoBindings(_CRUDRegras, "knockoutCRUDRegras");
}



//*******EVENTOS*******

function atualizarClick(e, sender) {
    SalvarRegras("RegraDescarteLoteProduto/Atualizar", "Atualizado com sucesso.", e, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir as regras?", function () {
        ExcluirPorCodigo(_regraDescarteLoteProduto, "RegraDescarteLoteProduto/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRegraDescarteLoteProduto.CarregarGrid();
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
    SalvarRegras("RegraDescarteLoteProduto/Adicionar", "Cadastrado com sucesso.", e, sender);
}



//*******MÉTODOS*******

function ExibirCamposObrigatorio() {
    exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function ValidacaoGeral() {
    var msg_padrao = "Nenhuma regra por #nomeregra# cadastrada.";
    var msg_regras = [];


    //-- Valida aprovadores
    if (_regraDescarteLoteProduto.GridAprovadores.val().length < _regraDescarteLoteProduto.NumeroAprovadores.val())
        return ["O número de aprovadores selecionados deve ser maior ou igual a " + _regraDescarteLoteProduto.NumeroAprovadores.val()];


    //-- Valida as regras
    // ProdutoEmbarcador
    if (_produtoEmbarcador.Alcadas.required() && _produtoEmbarcador.Alcadas.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Produto Embarcador"));

    // Deposito
    if (_deposito.Alcadas.required() && _deposito.Alcadas.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Depósito Armazenamento"));

    // Rua
    if (_rua.Alcadas.required() && _rua.Alcadas.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Rua Armazenamento"));

    // Bloco
    if (_bloco.Alcadas.required() && _bloco.Alcadas.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Bloco Armazenamento"));

    // Posicao
    if (_posicao.Alcadas.required() && _posicao.Alcadas.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Posição Armazenamento"));

    // Quantidade
    if (_quantidade.Alcadas.required() && _quantidade.Alcadas.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Quantidade"));

    return msg_regras;
}

function SalvarRegras(url, mensagemSucesso, e, sender) {
    msg_regras = ValidacaoGeral();

    if (msg_regras.length > 0)
        exibirMensagem(tipoMensagem.atencao, "Regra(s) inválida(s)", msg_regras.join("<br>"));
    else {
        Salvar(_regraDescarteLoteProduto, url, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", mensagemSucesso);
                    _gridRegraDescarteLoteProduto.CarregarGrid();
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