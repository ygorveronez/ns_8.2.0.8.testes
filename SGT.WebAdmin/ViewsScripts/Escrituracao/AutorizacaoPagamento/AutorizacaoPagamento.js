/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="Documentos.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPagamento.js" />

// #region Objetos Globais do Arquivo

var _pagamento;
var _gridPagamentos;
var _pesquisaPagamento;
var _rejeicao;
var _situacaoPagamentoUltimaPesquisa = EnumSituacaoPagamento.AguardandoAprovacao;
var $modalDetalhesPagamento;
var _modalPagamento;
var _modalRejeitarPagamento;
var _modalDelegarSelecionados;
// #endregion Objetos Globais do Arquivo

// #region Classes

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarPagamentosSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var Pagamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:" });
    this.Filial = PropertyEntity({ text: "Filial: ", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.NumeroPagamento = PropertyEntity({ text: "Número do Pagamento:" });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação: " });
    this.Transportador = PropertyEntity({ text: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS ? "Transportador: " : "Empresa/Filial: " });
    this.ValorPagamento = PropertyEntity({ text: "Valor do Pagamento: " });
    this.ValorPagamentoSemIcms = PropertyEntity({ text: "Valor do Pagamento sem ICMS: " });
};

var PesquisaPagamento = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:" });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS ? "Transportador:" : "Empresa/Filial"), idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoPagamento.AguardandoAprovacao), options: EnumSituacaoPagamento.obterOpcoesPesquisa(), def: EnumSituacaoPagamento.AguardandoAprovacao, text: "Situação: " });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplosPagamentosClick, text: "Aprovar Pagamentos", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridPagamentos, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplosPagamentosClick, text: "Rejeitar Pagamentos", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Pagamentos", visible: ko.observable(false) });
    this.ReprocessarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: reprocessarMultiplosPagamentosClick, text: "Reprocessar Pagamentos", visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadAutorizacao() {
    buscarDetalhesOperador(function () {
        _pagamento = new Pagamento();
        KoBindings(_pagamento, "knockoutPagamento");

        _rejeicao = new RejeitarSelecionados();
        KoBindings(_rejeicao, "knockoutRejeicaoPagamento");

        _pesquisaPagamento = new PesquisaPagamento();
        KoBindings(_pesquisaPagamento, "knockoutPesquisaPagamento");

        loadGridPagamentos();
        loadRegras();
        loadDelegar();
        loadDocumentos();

        $modalDetalhesPagamento = $("#divModalPagamento");

        new BuscarFilial(_pesquisaPagamento.Filial);
        new BuscarTransportadores(_pesquisaPagamento.Transportador);
        new BuscarFuncionario(_pesquisaPagamento.Usuario);

        loadDadosUsuarioLogado(atualizarGridPagamentos);
        _modalPagamento = new bootstrap.Modal(document.getElementById("divModalPagamento"), { backdrop: true, keyboard: true });
        _modalRejeitarPagamento = new bootstrap.Modal(document.getElementById("divModalRejeitarPagamento"), { backdrop: true, keyboard: true });
        _modalDelegarSelecionados = new bootstrap.Modal(document.getElementById("divModalDelegarSelecionados"), { backdrop: true, keyboard: true });
    });
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaPagamento.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaPagamento.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridPagamentos() {
    var opcaoDetalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharPagamento, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };

    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaPagamento.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    };

    var configuracaoExportacao = {
        url: "AutorizacaoPagamento/ExportarPesquisa",
        titulo: "Autorização de Pagamento"
    };

    _gridPagamentos = new GridView("grid-autorizacao-pagamentos", "AutorizacaoPagamento/Pesquisa", _pesquisaPagamento, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
    _gridPagamentos.SetPermitirEdicaoColunas(true);
    _gridPagamentos.SetSalvarPreferenciasGrid(true);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function aprovarMultiplosPagamentosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todos os pagamento selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaPagamento);

        dados.SelecionarTodos = _pesquisaPagamento.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridPagamentos.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridPagamentos.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoPagamento/AprovarMultiplosItens", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    if (retorno.Data.RegrasModificadas > 0) {
                        if (retorno.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alçadas foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alçada foi aprovada.");
                    }
                    else if (retorno.Data.Msg == "")
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");

                    atualizarGridPagamentos();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        })
    });
}

function cancelarRejeicaoSelecionadosClick() {
    LimparCampos(_rejeicao);

    _modalRejeitarPagamento.hide();
}

function exibirDelegarSelecionadosClick() {
    _modalDelegarSelecionados.show();
}

function rejeitarMultiplosPagamentosClick() {
    LimparCampos(_rejeicao);

    _modalRejeitarPagamento.show();
}

function rejeitarPagamentosSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todos os pagamentos selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaPagamento);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaPagamento.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridPagamentos.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridPagamentos.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoPagamento/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridPagamentos();
                    cancelarRejeicaoSelecionadosClick();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function reprocessarMultiplosPagamentosClick() {
    var dados = RetornarObjetoPesquisa(_pesquisaPagamento);

    dados.SelecionarTodos = _pesquisaPagamento.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridPagamentos.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridPagamentos.ObterMultiplosNaoSelecionados());

    executarReST("AutorizacaoPagamento/ReprocessarMultiplosPagamentos", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.RegrasReprocessadas > 0) {
                    if (retorno.Data.RegrasReprocessadas > 1)
                        exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasReprocessadas + " pagamentos foram reprocessados com sucesso.");
                    else
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "1 pagamento foi reprocessado com sucesso.");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Regras de Aprovação", "Nenhuma regra encontrada para os pagamentos selecionados.");

                atualizarGridPagamentos();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções

function atualizarGridPagamentos() {
    _pesquisaPagamento.SelecionarTodos.val(false);
    _pesquisaPagamento.AprovarTodas.visible(false);
    _pesquisaPagamento.DelegarTodas.visible(false);
    _pesquisaPagamento.RejeitarTodas.visible(false);

    _gridPagamentos.CarregarGrid();

    _situacaoPagamentoUltimaPesquisa = _pesquisaPagamento.Situacao.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaPagamento.AprovarTodas.visible(false);
    _pesquisaPagamento.DelegarTodas.visible(false);
    _pesquisaPagamento.RejeitarTodas.visible(false);
    _pesquisaPagamento.ReprocessarTodas.visible(false);

    var existemRegistrosSelecionados = _gridPagamentos.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaPagamento.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoPagamentoUltimaPesquisa == EnumSituacaoPagamento.AguardandoAprovacao) {
            _pesquisaPagamento.AprovarTodas.visible(true);
            _pesquisaPagamento.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaPagamento.RejeitarTodas.visible(true);
        }
        else if (_situacaoPagamentoUltimaPesquisa == EnumSituacaoPagamento.SemRegraAprovacao)
            _pesquisaPagamento.ReprocessarTodas.visible(true);
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharPagamento(registroSelecionado) {
    var pesquisa = RetornarObjetoPesquisa(_pesquisaPagamento);

    _pagamento.Codigo.val(registroSelecionado.Codigo);
    _pagamento.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_pagamento, "AutorizacaoPagamento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoPagamento.AguardandoAprovacao);
                preencherDocumentos(retorno.Data.ListaDocumentosPorCarga);

                _modalPagamento.show();
                $modalDetalhesPagamento.one('hidden.bs.modal', function () {
                    limparCamposPagamento();
                });
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    }, null);
}

function limparCamposPagamento() {
    $("#myTab a:first").tab("show");

    limparRegras();
    limparDocumentos();
}

// #endregion Funções
