/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />2
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaJanelaCarregamentoCotacao.js" />
/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />

// #region Objetos Globais do Arquivo

var _leilao;
var _gridLeilao;
var _pesquisaLeilao;
var _rejeicao;
var _situacaoCargaJanelaCarregamentoCotacaoUltimaPesquisa = EnumSituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao;
var _modalDelegarSelecionados;
var _modalRejeitarLeilao;
var $modalDetalhesLeilao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.Rejeitar = PropertyEntity({ eventClick: rejeitarLeiloesSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var Leilao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao), options: EnumSituacaoCargaJanelaCarregamentoCotacao.obterOpcoesAprovacao(), def: EnumSituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao });

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:" });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação:" });
    this.Transportador = PropertyEntity({ text: "Transportador:" });
    this.ValoresFrete = PropertyEntity({ val: ko.observableArray([]) });
};

var PesquisaLeilao = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:", val: ko.observable(""), def: "" });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao), options: EnumSituacaoCargaJanelaCarregamentoCotacao.obterOpcoesPesquisaAprovacao(), def: EnumSituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao, text: "Situação: " });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplosLeiloesClick, text: "Aprovar Leilões", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridLeilao, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplosLeiloesClick, text: "Rejeitar Leilões", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Leilões", visible: ko.observable(false) });
    this.ReprocessarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: reprocessarMultiplosLeiloesClick, text: "Reprocessar Leilões", visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadAutorizacao() {
    loadDadosUsuarioLogado(function () {
        _leilao = new Leilao();
        KoBindings(_leilao, "knockoutLeilao");

        _rejeicao = new RejeitarSelecionados();
        KoBindings(_rejeicao, "knockoutRejeicaoLeilao");

        _pesquisaLeilao = new PesquisaLeilao();
        KoBindings(_pesquisaLeilao, "knockoutPesquisaLeilao", false);

        loadGridLeilao();
        loadRegras();
        loadDelegar();

        $modalDetalhesLeilao = $("#divModalLeilao");

        new BuscarFuncionario(_pesquisaLeilao.Usuario);

        atualizarGridLeilao();

        _modalDelegarSelecionados = new bootstrap.Modal(document.getElementById("divModalDelegarSelecionados"), { backdrop: true, keyboard: true });
        _modalRejeitarLeilao = new bootstrap.Modal(document.getElementById("divModalRejeitarLeilao"), { backdrop: true, keyboard: true });
    });
}

function loadDadosUsuarioLogado(callback) {
    executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario) {
                _pesquisaLeilao.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaLeilao.Usuario.val(retorno.Data.Nome);
            }

            callback();
        }
    });
}

function loadGridLeilao() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharLeilao,
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
        SelecionarTodosKnout: _pesquisaLeilao.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    };

    var configuracaoExportacao = {
        url: "AutorizacaoLeilao/ExportarPesquisa",
        titulo: "Autorização de Leilões"
    };

    _gridLeilao = new GridView(_pesquisaLeilao.Pesquisar.idGrid, "AutorizacaoLeilao/Pesquisa", _pesquisaLeilao, menuOpcoes, null, 25, null, null, null, multiplaEscolha, 10000, null, configuracaoExportacao);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function aprovarMultiplosLeiloesClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todos os leilões selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaLeilao);

        dados.SelecionarTodos = _pesquisaLeilao.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridLeilao.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridLeilao.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoLeilao/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridLeilao();
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

    _modalRejeitarLeilao.hide();
}

function exibirDelegarSelecionadosClick() {
    _modalDelegarSelecionados.show();
}

function rejeitarMultiplosLeiloesClick() {
    LimparCampos(_rejeicao);

    _modalRejeitarLeilao.show();
}

function rejeitarLeiloesSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todos os leilões selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaLeilao);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaLeilao.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridLeilao.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridLeilao.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoLeilao/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridLeilao();
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

function reprocessarMultiplosLeiloesClick() {
    var dados = RetornarObjetoPesquisa(_pesquisaLeilao);

    dados.SelecionarTodos = _pesquisaLeilao.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridLeilao.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridLeilao.ObterMultiplosNaoSelecionados());

    executarReST("AutorizacaoLeilao/ReprocessarMultiplasCotacoes", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.RegrasReprocessadas > 0) {
                    if (retorno.Data.RegrasReprocessadas > 1)
                        exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasReprocessadas + " leilões foram reprocessados com sucesso.");
                    else
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "1 leilão foi reprocessado com sucesso.");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Regras de Aprovação", "Nenhuma regra encontrada para os leilões selecionados.");

                atualizarGridLeilao();
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

function atualizarGridLeilao() {
    _pesquisaLeilao.SelecionarTodos.val(false);

    _gridLeilao.AtualizarRegistrosSelecionados([]);
    _gridLeilao.CarregarGrid();

    _situacaoCargaJanelaCarregamentoCotacaoUltimaPesquisa = _pesquisaLeilao.Situacao.val();

    exibirMultiplasOpcoes();
}

function exibirMultiplasOpcoes() {
    _pesquisaLeilao.AprovarTodas.visible(false);
    _pesquisaLeilao.DelegarTodas.visible(false);
    _pesquisaLeilao.RejeitarTodas.visible(false);
    _pesquisaLeilao.ReprocessarTodas.visible(false);

    var existemRegistrosSelecionados = _gridLeilao.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaLeilao.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoCargaJanelaCarregamentoCotacaoUltimaPesquisa == EnumSituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao) {
            _pesquisaLeilao.AprovarTodas.visible(true);
            _pesquisaLeilao.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaLeilao.RejeitarTodas.visible(true);
        }
        else if (_situacaoCargaJanelaCarregamentoCotacaoUltimaPesquisa == EnumSituacaoCargaJanelaCarregamentoCotacao.SemRegraAprovacao)
            _pesquisaLeilao.ReprocessarTodas.visible(true);
    }
}

function controlarExibicaoAbas(dadosLeilao) {
    if ((dadosLeilao.Situacao === EnumSituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao) && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharLeilao(registroSelecionado) {
    var pesquisa = RetornarObjetoPesquisa(_pesquisaLeilao);

    _leilao.Codigo.val(registroSelecionado.Codigo);
    _leilao.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_leilao, "AutorizacaoLeilao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras(function () { controlarExibicaoAbas(retorno.Data); });

                $modalDetalhesLeilao.modal("show");
                $modalDetalhesLeilao.one('hidden.bs.modal', function () {
                    limparCamposLeilao();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function limparCamposLeilao() {
    $("#myTab a:first").tab("show");

    limparRegras();
}

// #endregion Funções
