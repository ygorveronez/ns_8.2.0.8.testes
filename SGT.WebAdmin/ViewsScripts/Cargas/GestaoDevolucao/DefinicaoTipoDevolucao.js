/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumTipoGestaoDevolucao.js" />

//#region Objetos Globais do Arquivo
var _definicaoTipoDevolucao;
var _gridDefinicaoTipoDevolucao;
var _gridProdutos;
var _ultimoProdutoSelecionado;

// #endregion Objetos Globais do Arquivo

//#region Classes
var DefinicaoTipoDevolucao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.Grid = PropertyEntity({ id: guid() });
    this.ProdutosSelecionados = PropertyEntity({ val: ko.observable("") });
    this.NumeroNF = PropertyEntity({ val: ko.observable(""), text: "Número da NF-E", visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ val: ko.observable(""), text: "Tomador", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ val: ko.observable(""), text: "Filial", visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ val: ko.observable(""), text: "Transportador", visible: ko.observable(true) });
    this.Emissao = PropertyEntity({ val: ko.observable(""), text: "Emissao", visible: ko.observable(true) });
    this.TipoDevolucao = PropertyEntity({ text: "Tipo de Devolução", val: ko.observable(EnumTipoGestaoDevolucao.NaoDefinido), options: ko.observable(EnumTipoGestaoDevolucao.obterOpcoes()), def: EnumTipoGestaoDevolucao.NaoDefinido, enable: ko.observable(true) });
    this.TipoNotasDevolucao = PropertyEntity({ val: ko.observable(EnumTipoNotasGestaoDevolucao.Mercadoria), visible: ko.observable(false) });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });

    this.Confirmar = PropertyEntity({
        type: types.event, eventClick: function (e) {
            confirmarDefinicaoTipoDevolucao();
        }, text: "Confirmar", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });

    this.Cancelar = PropertyEntity({
        type: types.event, eventClick: function (e) {
            mostrarGridDevolucoes();
        }, text: "Cancelar", idGrid: guid(), visible: ko.observable(true)
    });
}
//#endregion Classes

// #region Funções de Inicialização
function loadDefinicaoTipoDevolucao(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/DefinicaoTipoDevolucao.html?dyn=" + guid(), function (data) {
                $("#container-principal-content").html(data);

                _definicaoTipoDevolucao = new DefinicaoTipoDevolucao();
                KoBindings(_definicaoTipoDevolucao, "knockoutDefinicaoTipoDevolucao");

                PreencherObjetoKnout(_definicaoTipoDevolucao, r);

                _definicaoTipoDevolucao.Codigo.val(_informacoesDevolucao.CodigoDevolucao.val());
                controlarAcoesContainerPrincipal(etapa, _definicaoTipoDevolucao);


                if (_definicaoTipoDevolucao.TipoNotasDevolucao.val() == EnumTipoNotasGestaoDevolucao.Pallet) {
                    _definicaoTipoDevolucao.TipoDevolucao.options(EnumTipoGestaoDevolucao.obterOpcoesPallet());
                }

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) {
                    if (r.Data.TipoTomador == "CIF") {
                        _definicaoTipoDevolucao.TipoDevolucao.options(EnumTipoGestaoDevolucao.obterOpcoesCIF())
                    }
                    else if (r.Data.TipoTomador == "FOB") {
                        _definicaoTipoDevolucao.TipoDevolucao.options(EnumTipoGestaoDevolucao.obterOpcoesFOB())
                    }
                };

                $('#grid-devolucoes').hide();
                $('#container-principal').show();
                loadGridProdutos();

            });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function loadGridProdutos() {
    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: null,
        callbackNaoSelecionado: null,
        callbackSelecionado: null,
        callbackSelecionarTodos: null,
        somenteLeitura: false,
        permitirSelecionarSomenteUmRegistro: true
    };

    _gridProdutos = new GridView(_definicaoTipoDevolucao.Grid.id, "GestaoDevolucao/BuscarProdutosNotasFiscais", _definicaoTipoDevolucao, null, null, 25, callback, null, null, null, null, null, null, null, null, null, null);
    _gridProdutos.SetPermitirEdicaoColunas(false);
    _gridProdutos.SetSalvarPreferenciasGrid(false);
    _gridProdutos.CarregarGrid();
}

function confirmarDefinicaoTipoDevolucao() {
    executarReST("GestaoDevolucao/DefinirTipoGestaoDevolucao", { Codigo: _definicaoTipoDevolucao.Codigo.val(), TipoGestaoDevolucao: _definicaoTipoDevolucao.TipoDevolucao.val() }, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Tipo da devolução definido com sucesso");

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}
// #endregion Funções Associadas a Eventos

// #region Funções Públicas
// #endregion Funções Públicas

// #region Funções Privadas
function callback() {
    var dadosProdutos = _gridProdutos.GridViewTableData();

    if (dadosProdutos.length > 0) {
        _definicaoTipoDevolucao.NumeroNF.val(dadosProdutos[0].NFDevolucao);
        _definicaoTipoDevolucao.Tomador.val(dadosProdutos[0].Tomador);
        _definicaoTipoDevolucao.Filial.val(dadosProdutos[0].Filial);
        _definicaoTipoDevolucao.Transportador.val(dadosProdutos[0].Transportador);
        _definicaoTipoDevolucao.Emissao.val(dadosProdutos[0].Emissao);
    }
}
function limparLinhasSelecionadasGridProdutos() {
    var registrosSelecionados = _gridProdutos.ObterMultiplosSelecionados();
    _ultimoProdutoSelecionado = null;
    atualizarCamposProduto(_ultimoProdutoSelecionado);

    for (var i = 0; i < registrosSelecionados.length; i++) {
        $('#' + registrosSelecionados[i].Codigo).removeClass('selected');
    }
}
// #endregion Funções Privadas