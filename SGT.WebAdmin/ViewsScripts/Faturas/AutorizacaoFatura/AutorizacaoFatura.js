/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumFatura.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridFaturas;
var _fatura;
var _pesquisaFatura;
var _rejeicao;
var _situacaoFaturaUltimaPesquisa = EnumSituacoesFatura.AguardandoAprovacao;
var $modalDetalhesFatura;

/*
 * Declaração das Classes
 */

var _situacaoPesquisa = [
    { text: "Todas", value: "" },
    { text: "Em Andamento", value: EnumSituacoesFatura.EmAndamento },
    { text: "Em Fechamento", value: EnumSituacoesFatura.EmFechamento },
    { text: "Em Cancelamento", value: EnumSituacoesFatura.EmCancelamento },
    { text: "Fechada", value: EnumSituacoesFatura.Fechado },
    { text: "Liquidada", value: EnumSituacoesFatura.Liquidado },
    { text: "Cancelada", value: EnumSituacoesFatura.Cancelado },
    { text: "Sem Regra", value: EnumSituacoesFatura.SemRegraAprovacao },
    { text: "Aguardando Aprovação", value: EnumSituacoesFatura.AguardandoAprovacao },
    { text: "Aprovação Rejeitada", value: EnumSituacoesFatura.AprovacaoRejeitada }
];

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarFaturasSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var Fatura = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Cidade = PropertyEntity({ text: "Cidade: ", visible: ko.observable(true) });
    this.Data = PropertyEntity({ text: "Data: ", visible: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true) });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.Operador = PropertyEntity({ text: "Operador: ", visible: ko.observable(true) });
    this.Pessoa = PropertyEntity({ text: "Pessoa: ", visible: ko.observable(true) });
};

var PesquisaFatura = function () {
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _situacaoPesquisa, def: 0, text: "Situação: " });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid() });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });
    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasFaturasClick, text: "Aprovar Faturas", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridFaturas, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasFaturasClick, text: "Rejeitar Faturas", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Faturas", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _fatura = new Fatura();
    KoBindings(_fatura, "knockoutFatura");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoFatura");

    _pesquisaFatura = new PesquisaFatura();
    KoBindings(_pesquisaFatura, "knockoutPesquisaFatura");

    loadGridFaturas();
    loadRegras();
    loadDelegar();



    $modalDetalhesFatura = $("#divModalFatura");

    new BuscarClientes(_pesquisaFatura.Pessoa);
    new BuscarFuncionario(_pesquisaFatura.Operador);
    new BuscarFuncionario(_pesquisaFatura.Usuario);

    loadDadosUsuarioLogado(atualizarGridFaturas);
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaFatura.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaFatura.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridFaturas() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharFatura,
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
        SelecionarTodosKnout: _pesquisaFatura.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configuracaoExportacao = {
        url: "AutorizacaoFatura/ExportarPesquisa",
        titulo: "Autorização Fatura"
    };

    _gridFaturas = new GridView(_pesquisaFatura.Pesquisar.idGrid, "AutorizacaoFatura/Pesquisa", _pesquisaFatura, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasFaturasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as Faturas selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaFatura);

        dados.SelecionarTodos = _pesquisaFatura.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridFaturas.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridFaturas.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoFatura/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridFaturas();
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

    Global.fecharModal("divModalRejeitarFatura");
}

function exibirDelegarSelecionadosClick() {
    Global.abrirModal("divModalDelegarSelecionados");
}

function rejeitarMultiplasFaturasClick() {
    LimparCampos(_rejeicao);

    $("#divModalRejeitarFatura").modal("show");
}

function rejeitarFaturasSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as Faturas selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaFatura);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaFatura.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridFaturas.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridFaturas.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoFatura/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridFaturas();
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

function atualizarGridFaturas() {
    _pesquisaFatura.SelecionarTodos.val(false);
    _pesquisaFatura.AprovarTodas.visible(false);
    _pesquisaFatura.DelegarTodas.visible(false);
    _pesquisaFatura.RejeitarTodas.visible(false);

    _gridFaturas.CarregarGrid();

    _situacaoFaturaUltimaPesquisa = _pesquisaFatura.Situacao.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaFatura.AprovarTodas.visible(false);
    _pesquisaFatura.DelegarTodas.visible(false);
    _pesquisaFatura.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridFaturas.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaFatura.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoFaturaoUltimaPesquisa == EnumSituacoesFatura.AguardandoAprovacao) {
            _pesquisaFatura.AprovarTodas.visible(true);
            _pesquisaFatura.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaFatura.RejeitarTodas.visible(true);
        }
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharFatura(registroSelecionado) {
    limparCamposFatura();

    var pesquisa = RetornarObjetoPesquisa(_pesquisaFatura);

    _fatura.Codigo.val(registroSelecionado.Codigo);
    _fatura.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_fatura, "AutorizacaoFatura/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacoesFatura.AguardandoAprovacao);

                Global.abrirModal("divModalFatura");
                $modalDetalhesFatura.one('hidden.bs.modal', function () {
                    limparCamposFatura();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function limparCamposFatura() {
    $("#myTab a:first").tab("show");

    limparRegras();
}