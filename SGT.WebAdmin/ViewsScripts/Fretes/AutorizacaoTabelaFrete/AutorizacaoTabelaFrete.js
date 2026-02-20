/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAlteracaoTabelaFrete.js" />
/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="ValoresAlterados.js" />
/// <reference path="VigenciaAnexo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridTabelaFrete;
var _pesquisaTabelaFrete;
var _rejeicao;
var _situacaoTabelaFreteUltimaPesquisa = EnumSituacaoAlteracaoTabelaFrete.AguardandoAprovacao;
var _tabelaFrete;
var _gerarPlanilhaExcelValoresAlterados;
var $modalDetalhesTabelaFrete;
var _modalTabelaFrete;
var _modalRejeitarTabelaFrete;
var _modalDelegarSelecionados;
var _gridValoresAlteradosTabelasSeleciona;
/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarTabelasFreteSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var TabelaFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoTabelaFrete = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Descricao = PropertyEntity({ text: "Descrição: ", visible: ko.observable(true) });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
}

var GerarPlanilhaExcelValoresAlterados = function () {
    this.GerarPlanilhaExcel = PropertyEntity({ eventClick: gerarPlanilhaExcelClick, type: types.event, text: "Gerar Planilha Excel", visible: ko.observable(true) });

    this.Preview = PropertyEntity({ eventClick: atualizarValoresAlterados, type: types.event, text: "Preview", idGrid: "grid-valores-alterados", visible: ko.observable(true) });
}

var PesquisaTabelaFrete = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAlteracaoTabelaFrete.AguardandoAprovacao), options: EnumSituacaoAlteracaoTabelaFrete.obterOpcoesPesquisaTabelaFrete(), def: EnumSituacaoAlteracaoTabelaFrete.AguardandoAprovacao, text: "Situação: " });
    this.Tabela = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tabela:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe });

    this.DataInicial.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicial;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasTabelasFreteClick, text: "Aprovar Tabelas de Frete", visible: ko.observable(false) });
    this.ExportarDetalhes = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exportarDetalhesClick, text: "Exportar Detalhes", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridTabelaFrete, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasTabelasFreteClick, text: "Rejeitar Tabelas de Frete", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Tabelas de Frete", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _tabelaFrete = new TabelaFrete();
    KoBindings(_tabelaFrete, "knockoutTabelaFrete");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoTabelaFrete");

    _pesquisaTabelaFrete = new PesquisaTabelaFrete();
    KoBindings(_pesquisaTabelaFrete, "knockoutPesquisaTabelaFrete");

    _gerarPlanilhaExcelValoresAlterados = new GerarPlanilhaExcelValoresAlterados();
    KoBindings(_gerarPlanilhaExcelValoresAlterados, "knockoutGerarPlanilhaExcelValoresAlterados");

    loadGridTabelaFrete();
    loadVigenciaAnexo();
    loadValoresAlterados();
    loadRegras();
    loadDelegar();
    loadObservacoes();
    loadGridValoresAlteradosSelecionados();

    $modalDetalhesTabelaFrete = $("#divModalTabelaFrete");

    new BuscarFuncionario(_pesquisaTabelaFrete.Usuario);
    new BuscarTabelasDeFrete(_pesquisaTabelaFrete.Tabela);
    new BuscarTiposOperacao(_pesquisaTabelaFrete.TipoOperacao);

    loadDadosUsuarioLogado(atualizarGridTabelaFrete);
    _modalTabelaFrete = new bootstrap.Modal(document.getElementById("divModalTabelaFrete"), { backdrop: true, keyboard: true });
    _modalRejeitarTabelaFrete = new bootstrap.Modal(document.getElementById("divModalRejeitarTabelaFrete"), { backdrop: true, keyboard: true });
    _modalDelegarSelecionados = new bootstrap.Modal(document.getElementById("divModalDelegarSelecionados"), { backdrop: true, keyboard: true });
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaTabelaFrete.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaTabelaFrete.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridTabelaFrete() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharTabelaFrete,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [opcaoDetalhes]
    };

    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaTabelaFrete.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configuracaoExportacao = {
        url: "AutorizacaoTabelaFrete/ExportarPesquisa",
        titulo: "Autorização de Tabela de Frete"
    };

    _gridTabelaFrete = new GridView(_pesquisaTabelaFrete.Pesquisar.idGrid, "AutorizacaoTabelaFrete/Pesquisa", _pesquisaTabelaFrete, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasTabelasFreteClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as tabelas de frete selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaTabelaFrete);

        dados.SelecionarTodos = _pesquisaTabelaFrete.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridTabelaFrete.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridTabelaFrete.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoTabelaFrete/AprovarMultiplosItens", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    if (retorno.Data.RegrasModificadas > 0) {
                        if (retorno.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alçadas foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alçada foi aprovada.");
                    }
                    else if (retorno.Data.Msg == "") {
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    }

                    atualizarGridTabelaFrete();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        })
    });
}

function cancelarRejeicaoSelecionadosClick() {
    LimparCampos(_rejeicao);

    _modalRejeitarTabelaFrete.hide();
}

function exibirDelegarSelecionadosClick() {
    _modalDelegarSelecionados.show();
}

function rejeitarMultiplasTabelasFreteClick() {
    LimparCampos(_rejeicao);

    _modalRejeitarTabelaFrete.show();
}

function rejeitarTabelasFreteSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as tabelas de frete selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaTabelaFrete);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaTabelaFrete.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridTabelaFrete.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridTabelaFrete.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoTabelaFrete/ReprovarMultiplosItens", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    if (retorno.Data.RegrasModificadas > 0) {
                        if (retorno.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alçadas foram reprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alçada foi reprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");

                    atualizarGridTabelaFrete();
                    cancelarRejeicaoSelecionadosClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        })
    });
}

/*
 * Declaração das Funções
 */

function atualizarGridTabelaFrete() {
    _pesquisaTabelaFrete.SelecionarTodos.val(false);
    _pesquisaTabelaFrete.AprovarTodas.visible(false);
    _pesquisaTabelaFrete.DelegarTodas.visible(false);
    _pesquisaTabelaFrete.RejeitarTodas.visible(false);

    _gridTabelaFrete.CarregarGrid();

    _situacaoTabelaFreteUltimaPesquisa = _pesquisaTabelaFrete.Situacao.val()
}

function atualizarTabelaFrete() {
    BuscarPorCodigo(_tabelaFrete, "AutorizacaoTabelaFrete/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoAlteracaoTabelaFrete.AguardandoAprovacao);
                controlarExibicaoAbaValoresAlterados(isSituacaoPermiteExibirAbaValoresAlterados(retorno.Data.Situacao));
                controlarExibicaoAbaAnexos(isSituacaoPermiteExibirAbaAnexos(retorno.Data.Situacao));
                atualizarGridRegras();
                carregarValoresAlterados();
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    }, null);
}

function exibirMultiplasOpcoes() {
    _pesquisaTabelaFrete.AprovarTodas.visible(false);
    _pesquisaTabelaFrete.DelegarTodas.visible(false);
    _pesquisaTabelaFrete.RejeitarTodas.visible(false);
    _pesquisaTabelaFrete.ExportarDetalhes.visible(false);

    var existemRegistrosSelecionados = _gridTabelaFrete.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaTabelaFrete.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        _pesquisaTabelaFrete.ExportarDetalhes.visible(_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente);

        if (_situacaoTabelaFreteUltimaPesquisa == EnumSituacaoAlteracaoTabelaFrete.AguardandoAprovacao) {
            _pesquisaTabelaFrete.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaTabelaFrete.AprovarTodas.visible(true);
            _pesquisaTabelaFrete.RejeitarTodas.visible(true);
        }
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharTabelaFrete(registroSelecionado) {
    var pesquisa = RetornarObjetoPesquisa(_pesquisaTabelaFrete);

    _tabelaFrete.Codigo.val(registroSelecionado.Codigo);
    _tabelaFrete.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_tabelaFrete, "AutorizacaoTabelaFrete/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoAlteracaoTabelaFrete.AguardandoAprovacao);
                controlarExibicaoAbaValoresAlterados(isSituacaoPermiteExibirAbaValoresAlterados(retorno.Data.Situacao));
                controlarExibicaoAbaAnexos(isSituacaoPermiteExibirAbaAnexos(retorno.Data.Situacao));
                recarregarGridVigenciaAnexo();
                recarregarGridObservacoes();
                atualizarGridRegras();
                recarregarGridObservacoes();
                carregarValoresAlterados();

                _modalTabelaFrete.show();
                $modalDetalhesTabelaFrete.one('hidden.bs.modal', function () {
                    limparCamposTabelaFrete();
                });
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    }, null);
}

function limparCamposTabelaFrete() {
    Global.ResetarAbas();

    LimparCampos(_tabelaFrete);
    limparRegras();
    limparValoresAlterados();
}

function exportarDetalhesClick() {
    carregarValoresAlteradosSelecionados();
}