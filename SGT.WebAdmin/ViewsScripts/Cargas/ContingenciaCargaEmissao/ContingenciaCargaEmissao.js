/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Ocorrencia.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoGestaoDocumento.js" />
/// <reference path="../../Enumeradores/EnumMotivoInconsistenciaGestaoDocumento.js" />
/// <reference path="Desconto.js" />
/// <reference path="DetalhesGestaoDocumento.js" />
/// <reference path="../../Consultas/CTe.js" />

// #region Objetos Globais do Arquivo

var _gridContingenciaCargaEmissao;
var _pesquisaContingenciaCargaEmissao;
var _cargasEmContingencia = false;
// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaContingenciaCargaEmissao = function () {
    this.NumeroCarga = PropertyEntity({ text: "Carga:", maxlength: 4600 });
    this.DataCriacaoInicial = PropertyEntity({ text: "Data de Criação Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataCriacaoFinal = PropertyEntity({ text: "Data de Criação Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.SituacaoCarga = PropertyEntity({ getType: typesKnockout.selectMultiple, val: ko.observable(0), options: EnumSituacoesCarga.obterSituacoesCargaNaoFaturadaComDescricao(), def: 0, text: "Situação: ", issue: 533 });

    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.multiplesEntities, visible: ko.observable(true), codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe) });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CargasEmContingencia = PropertyEntity({ text: "Cargas em contingência?", val: ko.observable(false), def: false, options: Global.ObterOpcoesBooleano("Sim", "Não"), getType: typesKnockout.bool });

    this.DataCriacaoInicial.dateRangeInit = this.DataCriacaoFinal;
    this.DataCriacaoFinal.dateRangeLimit = this.DataCriacaoInicial;

    this.Pesquisar = PropertyEntity({ eventClick: atualizarGridContingenciaCargaEmissao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

    // Aprovação com alçada
    this.LiberarEmissaoContingencia = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: liberarEmissaoContingenciaClick, text: "Liberar Emissão Por Contingência", visible: ko.observable(false) });
    this.EmitirNormalmente = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: emitirNormalmenteClick, text: "Emitir Normalmente", visible: ko.observable(false) });

};

// #endregion Classes

// #region Funções de Inicialização

function loadContingenciaCargaEmissao() {
    _pesquisaContingenciaCargaEmissao = new PesquisaContingenciaCargaEmissao();
    KoBindings(_pesquisaContingenciaCargaEmissao, "knockoutContingenciaCargaEmissao", false, _pesquisaContingenciaCargaEmissao.Pesquisar.id);
    
    BuscarTransportadores(_pesquisaContingenciaCargaEmissao.Empresa, null, null, true);
    BuscarFilial(_pesquisaContingenciaCargaEmissao.Filial);
    BuscarTiposOperacao(_pesquisaContingenciaCargaEmissao.TipoOperacao);
    BuscarTiposdeCarga(_pesquisaContingenciaCargaEmissao.TipoCarga);

    loadGridContingenciaCargaEmissao();
}

function loadGridContingenciaCargaEmissao() {
    let auditoria = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: exibirAuditoriaClick, tamanho: "20" };

    let menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [auditoria],
        tamanho: 7
    };

    let multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaContingenciaCargaEmissao.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    let configExportacao = {
        url: "ContingenciaCargaEmissao/ExportarPesquisa",
        titulo: "Carga Emissão Contingência"
    };

    _gridContingenciaCargaEmissao = new GridViewExportacao("grid-contingencia-carga-emissao", "ContingenciaCargaEmissao/Pesquisa", _pesquisaContingenciaCargaEmissao, menuOpcoes, configExportacao, null, 10, multiplaEscolha, 50);
    _gridContingenciaCargaEmissao.SetPermitirEdicaoColunas(true);
    _gridContingenciaCargaEmissao.SetPermitirReordenarColunas(false);
    _gridContingenciaCargaEmissao.SetSalvarPreferenciasGrid(true);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function liberarEmissaoContingenciaClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente liberar todos os registros selecionados para a emissão por contingência?", function () {
        let dados = RetornarObjetoPesquisa(_pesquisaContingenciaCargaEmissao);

        dados.SelecionarTodos = _pesquisaContingenciaCargaEmissao.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridContingenciaCargaEmissao.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridContingenciaCargaEmissao.ObterMultiplosNaoSelecionados());
        dados.Contingencia = true;
        executarReST("ContingenciaCargaEmissao/AlterarEmissaoContingencia", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cargas liberadas com sucesso.");
                    _gridContingenciaCargaEmissao.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function emitirNormalmenteClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente retornar os registros para o fluxo normal de emissão?", function () {
        let dados = RetornarObjetoPesquisa(_pesquisaContingenciaCargaEmissao);

        dados.SelecionarTodos = _pesquisaContingenciaCargaEmissao.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridContingenciaCargaEmissao.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridContingenciaCargaEmissao.ObterMultiplosNaoSelecionados());
        dados.Contingencia = false;
        executarReST("ContingenciaCargaEmissao/AlterarEmissaoContingencia", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cargas retornada com sucesso.");
                    _gridContingenciaCargaEmissao.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function exibirAuditoriaClick(carga) {
    let data = { Codigo: carga.Codigo };
    let closureAuditoria = OpcaoAuditoria("Carga", null, carga);
    closureAuditoria(data);
}


// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function exibirMultiplasOpcoes() {
    let existemRegistrosSelecionados = _gridContingenciaCargaEmissao.ObterMultiplosSelecionados().length > 0;
    let selecionadoTodos = _pesquisaContingenciaCargaEmissao.SelecionarTodos.val();

    _pesquisaContingenciaCargaEmissao.LiberarEmissaoContingencia.visible(!_cargasEmContingencia && (existemRegistrosSelecionados || selecionadoTodos));
    _pesquisaContingenciaCargaEmissao.EmitirNormalmente.visible(_cargasEmContingencia && (existemRegistrosSelecionados || selecionadoTodos));
}

function atualizarGridContingenciaCargaEmissao() {
    _pesquisaContingenciaCargaEmissao.LiberarEmissaoContingencia.val(false);
    _pesquisaContingenciaCargaEmissao.EmitirNormalmente.val(false);
    _cargasEmContingencia = _pesquisaContingenciaCargaEmissao.CargasEmContingencia.val();
    _gridContingenciaCargaEmissao.CarregarGrid();
    $("#panel-cargas-contingencia").show();
}


function isExibirLiberarEmissaoContingencia(obj) { return obj.ContingenciaCargaEmissao; }
function isExibirEmitirNormalmente(obj) { return !obj.ContingenciaCargaEmissao;  }

// #endregion Funções Privadas
