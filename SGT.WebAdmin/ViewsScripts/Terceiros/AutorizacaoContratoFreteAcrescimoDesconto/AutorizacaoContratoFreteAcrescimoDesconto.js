/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoContratoFreteAcrescimoDesconto.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridContratosFreteAcrescimoDesconto;
var _contratoFreteAcrescimoDesconto;
var _pesquisaContratoFreteAcrescimoDesconto;
var _rejeicao;
var _situacaoContratoFreteAcrescimoDescontoUltimaPesquisa = EnumSituacaoContratoFreteAcrescimoDesconto.AgAprovacao;
var $modalDetalhesContratoFreteAcrescimoDesconto;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarContratosFreteAcrescimoDescontoSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var ContratoFreteAcrescimoDesconto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Data = PropertyEntity({ text: "Data: ", visible: ko.observable(true) });
    this.NumeroContrato = PropertyEntity({ text: "Número Contrato: ", visible: ko.observable(true) });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.Operador = PropertyEntity({ text: "Operador: ", visible: ko.observable(true) });
    this.Justificativa = PropertyEntity({ text: "Justificativa: ", visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação: ", visible: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "Valor: ", visible: ko.observable(true) });
};

var PesquisaContratoFreteAcrescimoDesconto = function () {
    this.NumeroContrato = PropertyEntity({ text: "Número do Contrato: ", getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoContratoFreteAcrescimoDesconto.AgAprovacao), options: EnumSituacaoContratoFreteAcrescimoDesconto.obterOpcoesPesquisa(), def: EnumSituacaoContratoFreteAcrescimoDesconto.AgAprovacao, text: "Situação: " });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Justificativa:", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasContratosFreteAcrescimoDescontoClick, text: "Aprovar todas as selecionadas", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridContratosFreteAcrescimoDesconto, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasContratosFreteAcrescimoDescontoClick, text: "Rejeitar todas as selecionadas", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar todas as selecionadas", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _contratoFreteAcrescimoDesconto = new ContratoFreteAcrescimoDesconto();
    KoBindings(_contratoFreteAcrescimoDesconto, "knockoutContratoFreteAcrescimoDesconto");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoContratoFreteAcrescimoDesconto");

    _pesquisaContratoFreteAcrescimoDesconto = new PesquisaContratoFreteAcrescimoDesconto();
    KoBindings(_pesquisaContratoFreteAcrescimoDesconto, "knockoutPesquisaContratoFreteAcrescimoDesconto", false, _pesquisaContratoFreteAcrescimoDesconto.Pesquisar.id);

    loadGridContratosFreteAcrescimoDesconto();
    loadRegras();
    loadDelegar();

    $modalDetalhesContratoFreteAcrescimoDesconto = $("#divModalContratoFreteAcrescimoDesconto");

    new BuscarJustificativas(_pesquisaContratoFreteAcrescimoDesconto.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.ContratoFrete]);
    new BuscarFuncionario(_pesquisaContratoFreteAcrescimoDesconto.Usuario);

    loadDadosUsuarioLogado(atualizarGridContratosFreteAcrescimoDesconto);
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaContratoFreteAcrescimoDesconto.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaContratoFreteAcrescimoDesconto.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridContratosFreteAcrescimoDesconto() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharContratoFreteAcrescimoDesconto,
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
        SelecionarTodosKnout: _pesquisaContratoFreteAcrescimoDesconto.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configuracaoExportacao = {
        url: "AutorizacaoContratoFreteAcrescimoDesconto/ExportarPesquisa",
        titulo: "Autorização Acréscimo/Desconto no Contrato de Frete"
    };

    _gridContratosFreteAcrescimoDesconto = new GridView(_pesquisaContratoFreteAcrescimoDesconto.Pesquisar.idGrid, "AutorizacaoContratoFreteAcrescimoDesconto/Pesquisa", _pesquisaContratoFreteAcrescimoDesconto, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasContratosFreteAcrescimoDescontoClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todos os Acréscimo/Desconto nos Contratos de Frete selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaContratoFreteAcrescimoDesconto);

        dados.SelecionarTodos = _pesquisaContratoFreteAcrescimoDesconto.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridContratosFreteAcrescimoDesconto.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridContratosFreteAcrescimoDesconto.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoContratoFreteAcrescimoDesconto/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridContratosFreteAcrescimoDesconto();
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

    Global.fecharModal('divModalRejeitarContratoFreteAcrescimoDesconto');
}

function exibirDelegarSelecionadosClick() {
    Global.abrirModal('divModalDelegarSelecionados');
}

function rejeitarMultiplasContratosFreteAcrescimoDescontoClick() {
    LimparCampos(_rejeicao);

    Global.abrirModal('divModalRejeitarContratoFreteAcrescimoDesconto');
}

function rejeitarContratosFreteAcrescimoDescontoSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todos os Acréscimo/Desconto nos Contratos de Frete selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaContratoFreteAcrescimoDesconto);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaContratoFreteAcrescimoDesconto.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridContratosFreteAcrescimoDesconto.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridContratosFreteAcrescimoDesconto.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoContratoFreteAcrescimoDesconto/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridContratosFreteAcrescimoDesconto();
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

function atualizarGridContratosFreteAcrescimoDesconto() {
    _pesquisaContratoFreteAcrescimoDesconto.SelecionarTodos.val(false);
    _pesquisaContratoFreteAcrescimoDesconto.AprovarTodas.visible(false);
    _pesquisaContratoFreteAcrescimoDesconto.DelegarTodas.visible(false);
    _pesquisaContratoFreteAcrescimoDesconto.RejeitarTodas.visible(false);

    _gridContratosFreteAcrescimoDesconto.CarregarGrid();

    _situacaoContratoFreteAcrescimoDescontoUltimaPesquisa = _pesquisaContratoFreteAcrescimoDesconto.Situacao.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaContratoFreteAcrescimoDesconto.AprovarTodas.visible(false);
    _pesquisaContratoFreteAcrescimoDesconto.DelegarTodas.visible(false);
    _pesquisaContratoFreteAcrescimoDesconto.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridContratosFreteAcrescimoDesconto.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaContratoFreteAcrescimoDesconto.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoContratoFreteAcrescimoDescontoUltimaPesquisa == EnumSituacaoContratoFreteAcrescimoDesconto.AgAprovacao) {
            _pesquisaContratoFreteAcrescimoDesconto.AprovarTodas.visible(true);
            _pesquisaContratoFreteAcrescimoDesconto.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaContratoFreteAcrescimoDesconto.RejeitarTodas.visible(true);
        }
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharContratoFreteAcrescimoDesconto(registroSelecionado) {
    limparCamposContratoFreteAcrescimoDesconto();

    var pesquisa = RetornarObjetoPesquisa(_pesquisaContratoFreteAcrescimoDesconto);

    _contratoFreteAcrescimoDesconto.Codigo.val(registroSelecionado.Codigo);
    _contratoFreteAcrescimoDesconto.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_contratoFreteAcrescimoDesconto, "AutorizacaoContratoFreteAcrescimoDesconto/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoContratoFreteAcrescimoDesconto.AgAprovacao);

                Global.abrirModal("divModalContratoFreteAcrescimoDesconto");
                $modalDetalhesContratoFreteAcrescimoDesconto.one('hidden.bs.modal', function () {
                    limparCamposContratoFreteAcrescimoDesconto();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function limparCamposContratoFreteAcrescimoDesconto() {
    Global.ResetarAbas();

    limparRegras();
}