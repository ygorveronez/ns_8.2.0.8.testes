/// <reference path="RegrasCotacao.js" />
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
    SalvarRegras("RegraCotacao/Atualizar", "Atualizado com sucesso.", e, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir as regras?", function () {
        ExcluirPorCodigo(_regraCotacao, "RegraCotacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRegraCotacao.CarregarGrid();
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
    SalvarRegras("RegraCotacao/Adicionar", "Cadastrado com sucesso.", e, sender);
}

//*******MÉTODOS*******

function ExibirCamposObrigatorio() {
    exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function ValidacaoGeral() {
    var msg_padrao = "Nenhuma regra por #nomeregra# cadastrada.";
    var msg_regras = [];

    //-- Valida as regras
    // CepDestino
    if (_cepDestino.Regras.required() && _cepDestino.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Cep Destino"));

    // Cubagem
    if (_cubagem.Regras.required() && _cubagem.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Cubagem"));

    // Distancia
    if (_distancia.Regras.required() && _distancia.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Distância"));

    // Estado Destino
    if (_estadoDestino.Regras.required() && _estadoDestino.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Estado Destino"));

    // Expedidor
    if (_expedidor.Regras.required() && _expedidor.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Expedidor"));

    // GrupoProduto
    if (_grupoProduto.Regras.required() && _grupoProduto.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Grupo Produto"));

    // LinhaSeparacao
    if (_linhaSeparacao.Regras.required() && _linhaSeparacao.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "LinhaSeparacao"));

    // Marca Produto
    if (_marcaProduto.Regras.required() && _marcaProduto.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Marca Produto"));

    // Peso
    if (_peso.Regras.required() && _peso.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Peso"));

    // Produto
    if (_produto.Regras.required() && _produto.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Produto"));

    // Transportador
    if (_transportador.Regras.required() && _transportador.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Transportador"));

    // Valor Mercadoria
    if (_valorMercadoria.Regras.required() && _valorMercadoria.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Valor Mercadoria"));

    // Volume
    if (_volume.Regras.required() && _volume.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Volume"));

    // Valor da Cotação
    if (_valorCotacao.Regras.required() && _valorCotacao.Regras.val().length == 0)
        msg_regras.push(msg_padrao.replace("#nomeregra#", "Valor da Cotação"));

    return msg_regras;
}

function SalvarRegras(url, mensagemSucesso, e, sender) {
    msg_regras = ValidacaoGeral();

    if (msg_regras.length > 0)
        exibirMensagem(tipoMensagem.atencao, "Regra(s) inválida(s)", msg_regras.join("<br>"));
    else {
        Salvar(_regraCotacao, url, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", mensagemSucesso);
                    _gridRegraCotacao.CarregarGrid();
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