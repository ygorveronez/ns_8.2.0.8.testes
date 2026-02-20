/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="AutorizacaoNaoConformidade.js" />
/// <reference path="AutorizarRegras.js" />

// #region Objetos Globais do Arquivo

var _ajusteNumeroPedido;
var _ajustePeso;
var _ajusteDeParaProdutos;

// #endregion Objetos Globais do Arquivo

// #region Classes

var AjusteNumeroPedido = function () {
    this.NumeroPedido = PropertyEntity({ text: "*Número do Pedido:", val: ko.observable(""), def: "", required: true });

    this.AprovarConformeCarga = PropertyEntity({ type: types.event, eventClick: ajustarNumeroPedidoNaoConformidadeClick, text: "Aprovar conforme N° Ordem da Carga" });
}

var AjustePeso = function () {
    this.Peso = PropertyEntity({ text: "*Peso:", val: ko.observable(""), def: "", required: true, getType: typesKnockout.decimal });

    this.Ajustar = PropertyEntity({ type: types.event, eventClick: ajustarPesoNaoConformidadeClick, text: "Ajustar" });
}

var AjusteDeParaProdutos = function () {

    this.Ajustar = PropertyEntity({ type: types.event, eventClick: ajustarDeParaProdutosNaoConformidadeClick, text: "Ajustar" });
}

// #endregion Classes

// #region Funções de Inicialização

function loadAjuste() {
    _ajusteNumeroPedido = new AjusteNumeroPedido();
    KoBindings(_ajusteNumeroPedido, "knockoutAjustarNumeroPedido");

    _ajustePeso = new AjustePeso();
    KoBindings(_ajustePeso, "knockoutAjustarPeso");

    _ajusteDeParaProdutos = new AjusteDeParaProdutos();
    KoBindings(_ajusteDeParaProdutos, "knockoutAjustarDeParaProdutos");
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function ajustarNumeroPedidoNaoConformidadeClick() {
    var dados = {
        Codigo: _naoConformidade.Codigo.val(),
        NumeroPedido: _ajusteNumeroPedido.NumeroPedido.val()
    };

    executarReST("AutorizacaoNaoConformidade/AprovarConformeCarga", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Não conformidade aprovada com sucesso.");
                atualizarGridNaoConformidade();
                atualizarNaoConformidade();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function ajustarPesoNaoConformidadeClick() {
    var dados = {
        Codigo: _naoConformidade.Codigo.val(),
        Peso: _ajustePeso.Peso.val()
    };

    executarReST("AutorizacaoNaoConformidade/AjustarPeso", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.aviso, "Sucesso", "Não conformidade ajustada com sucesso.");
                atualizarGridNaoConformidade();
                atualizarNaoConformidade();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function ajustarDeParaProdutosNaoConformidadeClick() {
    var dados = {
        Codigo: _naoConformidade.Codigo.val()
    };

    executarReST("AutorizacaoNaoConformidade/AjustarDeParaProdutos", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.aviso, "Sucesso", "Não conformidade ajustada com sucesso.");
                atualizarGridNaoConformidade();
                atualizarNaoConformidade();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos
