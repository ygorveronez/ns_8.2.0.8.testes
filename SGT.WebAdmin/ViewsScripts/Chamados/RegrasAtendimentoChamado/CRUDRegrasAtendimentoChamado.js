/// <reference path="RegrasAtendimentoChamado.js" />
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
    SalvarRegras("RegrasAtendimentoChamados/Atualizar", "Atualizado com sucesso.", e, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir as regras?", function () {
        ExcluirPorCodigo(_regraAtendimentoChamado, "RegrasAtendimentoChamados/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRegraAtendimentoChamado.CarregarGrid();
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
    SalvarRegras("RegrasAtendimentoChamados/Adicionar", "Cadastrado com sucesso.", e, sender);
}

//*******MÉTODOS*******

function ExibirCamposObrigatorio() {
    exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function ValidacaoGeral() {
    var msg_padrao = "Nenhuma regra por #nomeregra# cadastrada.";
    var msg_regras = [];


    //-- Valida aprovadores
    if (_regraAtendimentoChamado.NumeroAprovadores.val() < 0)
        return ["O número de aprovadores deve ser maior ou igual a 0"];

    if (_regraAtendimentoChamado.GridAprovadores.val().length < _regraAtendimentoChamado.NumeroAprovadores.val())
        return ["O número de aprovadores selecionados deve ser maior ou igual a " + _regraAtendimentoChamado.NumeroAprovadores.val()];


    //-- Valida as regras
    if (_canalVenda.Regras.required() && _canalVenda.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Canal de Venda"));

    if (_filial.Regras.required() && _filial.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Filial"));

    if (_tipoOperacao.Regras.required() && _tipoOperacao.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Tipo da Operação"));

    if (_transportador.Regras.required() && _transportador.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Transportador"));
    
    if (_estado.Regras.required() && _estado.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Estado"));

    return msg_regras;
}

function SalvarRegras(url, mensagemSucesso, e, sender) {
    msg_regras = ValidacaoGeral();

    if (msg_regras.length > 0)
        exibirMensagem(tipoMensagem.atencao, "Regra(s) inválida(s)", msg_regras.join("<br>"));
    else {
        Salvar(_regraAtendimentoChamado, url, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", mensagemSucesso);
                    _gridRegraAtendimentoChamado.CarregarGrid();
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