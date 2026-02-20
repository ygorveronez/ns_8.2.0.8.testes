/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />

// #region Objetos Globais do Arquivo

var _pagamentoAutorizacao;
var _pagamentoAutorizacaoDetalhe;
var _gridPagamentoAutorizacao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PagamentoAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.AprovacoesNecessarias = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações Necessárias:", enable: ko.observable(true) });
    this.Aprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações:", enable: ko.observable(true) });
    this.DescricaoSituacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Situação:", enable: ko.observable(true) });
    this.Reprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Reprovações:", enable: ko.observable(true) });
    this.PossuiRegras = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.ExibirAprovacao = PropertyEntity({ eventClick: exibirPagamentoAprovacaoClick, type: types.event, text: ko.observable("Exibir Aprovação"), fadeVisible: ko.observable(false), visible: ko.observable(false) });
    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasPagamentoClick, type: types.event, text: ko.observable("Reprocessar Regras") });
}

var PagamentoAutorizacaoDetalhe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Regra = PropertyEntity({ text: "Regra:", val: ko.observable("") });
    this.Data = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: "Usuário:", val: ko.observable("") });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridPagamentoAutorizacao() {
    var opcaoDetalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharPagamentoAutorizacaoClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };

    _gridPagamentoAutorizacao = new GridView("grid-pagamento-autorizacao", "Pagamento/PesquisaAutorizacoes", _pagamentoAutorizacao, menuOpcoes);
    _gridPagamentoAutorizacao.CarregarGrid();
}

function loadPagamentoAutorizacao() {
    if (_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoPagamento) {
        _pagamentoAutorizacao = new PagamentoAutorizacao();
        KoBindings(_pagamentoAutorizacao, "knockoutPagamentoAutorizacao");

        _pagamentoAutorizacaoDetalhe = new PagamentoAutorizacaoDetalhe();
        KoBindings(_pagamentoAutorizacaoDetalhe, "knockoutPagamentoAutorizacaoDetalhe");

        loadGridPagamentoAutorizacao();
    }
    else {
        $("#knockoutPagamentoAutorizacao").remove();
        $("#divModalPagamentoAutorizacao").remove();
    }
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function detalharPagamentoAutorizacaoClick(registroSelecionado) {
    executarReST("Pagamento/DetalhesAutorizacao", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_pagamentoAutorizacaoDetalhe, retorno);
                exibirModalPagamentoAutorizacao();
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function exibirPagamentoAprovacaoClick() {
    controlarExibicaoPagamentoAutorizacao(_pagamentoAutorizacao.ExibirAprovacao.fadeVisible() == false);
}

function reprocessarRegrasPagamentoClick() {
    executarReST("Pagamento/ReprocessarRegras", { Codigo: _pagamentoAutorizacao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Regras de aprovação reprocessadas com sucesso.");
                preencherPagamentoAutorizacao(_pagamentoAutorizacao.Codigo.val());
                _gridPagamento.CarregarGrid();
                BuscarPagamentoPorCodigo(_pagamentoAutorizacao.Codigo.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Sem Regra", retorno.Msg || "Nenhuma regra de aprovação encontrada.");
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparPagamentoAutorizacao() {
    if (_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoPagamento) {
        LimparCampos(_pagamentoAutorizacao);
        controlarExibicaoPagamentoAutorizacao(false);
        _pagamentoAutorizacao.ExibirAprovacao.visible(false);
        _gridPagamentoAutorizacao.CarregarGrid();
    }
}

function preencherPagamentoAutorizacao(codigoPagamento) {
    if (_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoPagamento) {
        executarReST("Pagamento/BuscarResumoAprovacao", { Codigo: codigoPagamento }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    PreencherObjetoKnout(_pagamentoAutorizacao, retorno);

                    if (retorno.Data.PossuiAlcada) {
                        _pagamentoAutorizacao.ExibirAprovacao.visible(true);
                        _gridPagamentoAutorizacao.CarregarGrid();
                    }
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarExibicaoPagamentoAutorizacao(exibir) {
    _pagamentoAutorizacao.ExibirAprovacao.fadeVisible(exibir);
    _pagamentoAutorizacao.ExibirAprovacao.text(exibir ? "Ocultar Aprovação" : "Exibir Aprovação");
}

function exibirModalPagamentoAutorizacao() {
    Global.abrirModal('divModalPagamentoAutorizacao');
    $("#divModalPagamentoAutorizacao").one('hidden.bs.modal', function () {
        LimparCampos(_pagamentoAutorizacaoDetalhe);
    });
}

// #endregion Funções Privadas
