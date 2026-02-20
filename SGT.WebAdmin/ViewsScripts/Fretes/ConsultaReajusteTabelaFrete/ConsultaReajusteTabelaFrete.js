/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAjusteTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumEtapaAjusteTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumEtapaAutorizacaoTabelaFrete.js" />
/// <reference path="Alteracao.js" />
/// <reference path="Autorizacao.js" />
/// <reference path="VigenciaAnexo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _ajusteTabela;
var _pesquisaReajustes;
var _valores;
var _autorizacao;
var _gridAvaria;

/*
 * Declaração das Classes
 */

var RegraAvaria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var Autorizacao = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Justificativa = PropertyEntity({ text: "*Justificativa:", type: types.entity, codEntity: ko.observable(0), required: true, enable: true, idBtnSearch: guid(), visible: ko.observable(false) });
    this.MotivoRejeicao = PropertyEntity({ text: "*Motivo Rejeição:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Motivo = PropertyEntity({ text: "Observação: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarAvariaClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: aprovarMultiplasRegrasClick, text: "Aprovar Regras" });
}

var AjusteTabela = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "Número do Ajuste: ", visible: ko.observable(true), val: ko.observable("") });
    this.DescricaoTabelaFrete = PropertyEntity({ text: "Tabela de Frete: ", visible: ko.observable(true), val: ko.observable("") });
    this.DataCriacao = PropertyEntity({ text: "Data Criação: ", visible: ko.observable(true), val: ko.observable("") });
    this.DataAjuste = PropertyEntity({ text: "Data Ajuste: ", visible: ko.observable(true), val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true), val: ko.observable("") });
    this.Etapa = PropertyEntity({ text: "Etapa: ", visible: ko.observable(true), val: ko.observable("") });

    // Campos para filtro
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
}

var PesquisaReajustes = function () {
    this.Tabela = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tabela:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.EtapaAjuste = PropertyEntity({ val: ko.observable(EnumEtapaAjusteTabelaFrete.AgAprovacao), options: EnumEtapaAjusteTabelaFrete.obterOpcoesPesquisa(), def: EnumEtapaAjusteTabelaFrete.AgAprovacao, text: "Etapa do Ajuste: " });
    this.EtapaAutorizacao = PropertyEntity({ val: ko.observable(EnumEtapaAutorizacaoTabelaFrete.AprovacaoReajuste), options: EnumEtapaAutorizacaoTabelaFrete.obterOpcoesPesquisa(), def: EnumEtapaAutorizacaoTabelaFrete.AprovacaoReajuste, text: "Etapa da Aprovação: " });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAjusteTabelaFrete.AgAprovacao), options: EnumSituacaoAjusteTabelaFrete.obterOpcoesPesquisaConsultaReajusteTabelaFrete(), def: EnumSituacaoAjusteTabelaFrete.AgAprovacao, text: "Situação: " });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarReajustes();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplosAjustsClick, text: "Aprovar Reajustes", visible: ko.observable(false) });
    this.ExportarDetalhes = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exportarDetalhesReajusteClick, text: "Exportar detalhes", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _ajusteTabela = new AjusteTabela();
    KoBindings(_ajusteTabela, "knockoutAjusteTabela");
    
    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    _pesquisaReajustes = new PesquisaReajustes();
    KoBindings(_pesquisaReajustes, "knockoutPesquisaReajustes");

    new BuscarTabelasDeFrete(_pesquisaReajustes.Tabela);
    new BuscarTransportadores(_pesquisaReajustes.Transportador);
    new BuscarTiposOperacao(_pesquisaReajustes.TipoOperacao);
    new BuscarFuncionario(_pesquisaReajustes.Usuario);

    loadVigenciaAnexo();
    loadRegras();
    loadAlteracoes();
    loadDadosUsuarioLogado(BuscarReajustes);
    loadGridValoresSelecionados();

    // Valida se a tela está sendo carregado pelo link de acesso enviado via e-mail
    if (CODIGO_TABELA_FRETE_VIA_TOKEN_ACESSO_AUTORIZACAO_TABELA_FRETE.val() != "")
        carregarTabelaFreteUsuarioAcessadoViaLink(CODIGO_TABELA_FRETE_VIA_TOKEN_ACESSO_AUTORIZACAO_TABELA_FRETE.val());
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaReajustes.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaReajustes.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplosAjustsClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas os ajustes selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaReajustes);

        dados.SelecionarTodos = _pesquisaReajustes.SelecionarTodos.val();
        dados.AjustesSelecionados = JSON.stringify(_gridAvaria.ObterMultiplosSelecionados());
        dados.AjustesNaoSelecionados = JSON.stringify(_gridAvaria.ObterMultiplosNaoSelecionados());

        executarReST("ConsultaReajusteTabelaFrete/AprovarMultiplosAjustes", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    BuscarReajustes();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

/*
 * Declaração das Funções Públicas
 */

function BuscarReajustes() {
    //-- Cabecalho
    var detalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharReajuste,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    //-- Reseta
    _pesquisaReajustes.SelecionarTodos.val(false);
    _pesquisaReajustes.AprovarTodas.visible(false);
    _pesquisaReajustes.ExportarDetalhes.visible(false);

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaReajustes.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configExportacao = {
        url: "ConsultaReajusteTabelaFrete/ExportarPesquisa",
        titulo: "Autorização Ajustes Tabela Frete"
    };

    _gridAvaria = new GridView(_pesquisaReajustes.Pesquisar.idGrid, "ConsultaReajusteTabelaFrete/Pesquisa", _pesquisaReajustes, menuOpcoes, null, 20, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridAvaria.CarregarGrid();
}

function exibirMultiplasOpcoes(e) {
    var possuiSelecionado = _gridAvaria.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaReajustes.SelecionarTodos.val();

    if (possuiSelecionado || selecionadoTodos) {
        _pesquisaReajustes.AprovarTodas.visible(true);
        _pesquisaReajustes.ExportarDetalhes.visible(true);
    } else {
        _pesquisaReajustes.AprovarTodas.visible(false);
        _pesquisaReajustes.ExportarDetalhes.visible(false);
    }
}

function detalharReajuste(itemGrid) {
    LimparCamposReajuste();

    var pesquisa = RetornarObjetoPesquisa(_pesquisaReajustes);

    _ajusteTabela.Codigo.val(itemGrid.Codigo);
    _ajusteTabela.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_ajusteTabela, "ConsultaReajusteTabelaFrete/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                recarregarGridVigenciaAnexo();
                AtualizarGridRegras();
                AtualizarRelatorioAlteracoes(arg.Data);

                // Abre modal 
                Global.abrirModal('divModalAjusteTabela');
                $("#divModalAjusteTabela").one('hidden.bs.modal', function () {
                    LimparCamposReajuste();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, null);
}

function LimparCamposReajuste() {
    resetarTabs();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

function carregarTabelaFreteUsuarioAcessadoViaLink(tokenAcesso) {
    LimparCamposReajuste();

    _ajusteTabela.Codigo.val(tokenAcesso);

    BuscarPorCodigo(_ajusteTabela, "ConsultaReajusteTabelaFrete/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                recarregarGridVigenciaAnexo();
                AtualizarGridRegras();
                AtualizarRelatorioAlteracoes(arg.Data);

                // Abre modal 
                Global.abrirModal('divModalAjusteTabela');
                $("#divModalAjusteTabela").one('hidden.bs.modal', function () {
                    LimparCamposReajuste();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, null);
}

function exportarDetalhesReajusteClick() {
    carregarValoresSelecionados();
}