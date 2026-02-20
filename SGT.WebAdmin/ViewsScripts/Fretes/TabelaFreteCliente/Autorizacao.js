/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _tabelaFreteClienteAutorizacao;
var _tabelaFreteClienteAutorizacaoDetalhe;
var _gridTabelaFreteClienteAutorizacao;

/*
 * Declaração das Classes
 */

var TabelaFreteClienteAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.AprovacoesNecessarias = PropertyEntity({ type: types.map, val: ko.observable(""), text: Localization.Resources.Fretes.TabelaFreteCliente.AprovacoesNecessarias.getFieldDescription(), enable: ko.observable(true) });
    this.Aprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: Localization.Resources.Fretes.TabelaFreteCliente.Aprovacoes.getFieldDescription(), enable: ko.observable(true) });
    this.DescricaoSituacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: Localization.Resources.Fretes.TabelaFreteCliente.Situacao.getFieldDescription(), enable: ko.observable(true) });
    this.Reprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: Localization.Resources.Fretes.TabelaFreteCliente.Reprovacoes.getFieldDescription(), enable: ko.observable(true) });
    this.PossuiRegras = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.ExibirAprovacao = PropertyEntity({ eventClick: exibirTabelaFreteClienteAprovacaoClick, type: types.event, text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.ExibirAprovacao), fadeVisible: ko.observable(false), visible: ko.observable(false) });
    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasTabelaFreteClienteClick, type: types.event, text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.ReprocessarRegras) });
}

var TabelaFreteClienteAutorizacaoDetalhe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Regra = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.Regra.getFieldDescription(), val: ko.observable("") });
    this.Data = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.Data.getFieldDescription() , val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.Situacao.getFieldDescription(), val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.Usuario.getFieldDescription(), val: ko.observable("") });
    this.Motivo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.Motivo.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridTabelaFreteClienteAutorizacao() {
    var opcaoDetalhes = { descricao: Localization.Resources.Fretes.TabelaFreteCliente.Detalhes, id: "clasEditar", evento: "onclick", metodo: detalharTabelaFreteClienteAutorizacaoClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };

    _gridTabelaFreteClienteAutorizacao = new GridView("grid-tabela-frete-cliente-autorizacao", "TabelaFreteCliente/PesquisaAutorizacoes", _tabelaFreteClienteAutorizacao, menuOpcoes);
    _gridTabelaFreteClienteAutorizacao.CarregarGrid();
}

function loadTabelaFreteClienteAutorizacao() {
    if (_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoTabelaFrete) {
        _tabelaFreteClienteAutorizacao = new TabelaFreteClienteAutorizacao();
        KoBindings(_tabelaFreteClienteAutorizacao, "knockoutTabelaFreteClienteAutorizacao");

        _tabelaFreteClienteAutorizacaoDetalhe = new TabelaFreteClienteAutorizacaoDetalhe();
        KoBindings(_tabelaFreteClienteAutorizacaoDetalhe, "knockoutTabelaFreteClienteAutorizacaoDetalhe");

        loadGridTabelaFreteClienteAutorizacao();
    }
    else {
        $("#knockoutTabelaFreteClienteAutorizacao").remove();
        $("#divModalTabelaFreteClienteAutorizacao").remove();
    }
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function detalharTabelaFreteClienteAutorizacaoClick(registroSelecionado) {
    executarReST("TabelaFreteCliente/DetalhesAutorizacao", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_tabelaFreteClienteAutorizacaoDetalhe, retorno);
                exibirModalTabelaFreteClienteAutorizacao();
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function exibirTabelaFreteClienteAprovacaoClick() {
    controlarExibicaoTabelaFreteClienteAutorizacao(_tabelaFreteClienteAutorizacao.ExibirAprovacao.fadeVisible() == false);
}

function reprocessarRegrasTabelaFreteClienteClick() {
    executarReST("TabelaFreteCliente/ReprocessarRegras", { Codigo: _tabelaFreteClienteAutorizacao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Fretes.TabelaFreteCliente.RegrasDeAprovacaoReprocessadasComSucesso);
                preencherTabelaFreteClienteAutorizacao(_tabelaFreteClienteAutorizacao.Codigo.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFreteCliente.SemRegra, retorno.Msg || Localization.Resources.Fretes.TabelaFreteCliente.NenhumaRegraDeAprovacaoEncontrada);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function limparTabelaFreteClienteAutorizacao() {
    if (_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoTabelaFrete) {
        LimparCampos(_tabelaFreteClienteAutorizacao);
        controlarExibicaoTabelaFreteClienteAutorizacao(false);
        _tabelaFreteClienteAutorizacao.ExibirAprovacao.visible(false);
        _gridTabelaFreteClienteAutorizacao.CarregarGrid();
    }
}

function preencherTabelaFreteClienteAutorizacao(codigoTabelaFreteCliente) {
    if (_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoTabelaFrete) {
        executarReST("TabelaFreteCliente/BuscarResumoAprovacao", { Codigo: codigoTabelaFreteCliente }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    PreencherObjetoKnout(_tabelaFreteClienteAutorizacao, retorno);

                    if (retorno.Data.PossuiAlcada) {
                        _tabelaFreteClienteAutorizacao.ExibirAprovacao.visible(true);
                        _gridTabelaFreteClienteAutorizacao.CarregarGrid();
                    }
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

/*
 * Declaração das Funções Privadas
 */

function controlarExibicaoTabelaFreteClienteAutorizacao(exibir) {
    _tabelaFreteClienteAutorizacao.ExibirAprovacao.fadeVisible(exibir);
    _tabelaFreteClienteAutorizacao.ExibirAprovacao.text(exibir ? Localization.Resources.Fretes.TabelaFreteCliente.OcultarAprovacao : Localization.Resources.Fretes.TabelaFreteCliente.ExibirAprovacao);
}

function exibirModalTabelaFreteClienteAutorizacao() {
    Global.abrirModal('divModalTabelaFreteClienteAutorizacao');
    $("#divModalTabelaFreteClienteAutorizacao").one('hidden.bs.modal', function () {
        LimparCampos(_tabelaFreteClienteAutorizacaoDetalhe);
    });
}