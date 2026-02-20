/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCheckin.js" />
/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />

// #region Objetos Globais do Arquivo

var _recusaCheckin;
var _gridRecusaCheckin;
var _pesquisaRecusaCheckin;
var _rejeicao;
var _situacaoCheckinUltimaPesquisa = EnumSituacaoCheckin.AguardandoAprovacao;
var _modalDelegarSelecionados;
var _modalRejeitarRecusaCheckin;
var $modalDetalhesRecusaCheckin;

// #endregion Objetos Globais do Arquivo

// #region Classes

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.Rejeitar = PropertyEntity({ eventClick: rejeitarRecusasCheckinSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var RecusaCheckin = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.NumeroCTe = PropertyEntity({ text: "Número do CT-e:" });
    this.Peso = PropertyEntity({ text: "Peso: " });
    this.Valor = PropertyEntity({ text: "Valor: " });
};

var PesquisaRecusaCheckin = function () {
    this.NumeroCTe = PropertyEntity({ text: "Número do CT-e:", val: ko.observable(""), def: "" });
    this.Carga = PropertyEntity({ text: "Número Carga:", val: ko.observable(""), def: "", visible: true });
    this.DataCriacaoCarga = PropertyEntity({ text: "Data Criação Carga:", getType: typesKnockout.dateTime, val: ko.observable(""), def: "", visible: true });
    this.Serie = PropertyEntity({ text: "Serie:", val: ko.observable(""), def: "", visible: true });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCheckin.AguardandoAprovacao), options: EnumSituacaoCheckin.obterOpcoesPesquisaAprovacao(), def: EnumSituacaoCheckin.AguardandoAprovacao, text: "Situação: " });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid(), visible: true });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid(), visible: true });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: true });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: true });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasRecusasCheckinClick, text: "Aprovar Recusas de Checkin", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridRecusaCheckin, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasRecusasCheckinClick, text: "Rejeitar Recusas de Checkin", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Recusas de Checkin", visible: ko.observable(false) });
    this.ReprocessarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: reprocessarMultiplasRecusasCheckinClick, text: "Reprocessar Recusas de Checkin", visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadAutorizacao() {
    _recusaCheckin = new RecusaCheckin();
    KoBindings(_recusaCheckin, "knockoutRecusaCheckin");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoRecusaCheckin");

    _pesquisaRecusaCheckin = new PesquisaRecusaCheckin();
    KoBindings(_pesquisaRecusaCheckin, "knockoutPesquisaRecusaCheckin", false);

    loadGridRecusaCheckin();
    loadRegras();
    loadDelegar();

    $modalDetalhesRecusaCheckin = $("#divModalRecusaCheckin");

    new BuscarFuncionario(_pesquisaRecusaCheckin.Usuario);
    new BuscarFilial(_pesquisaRecusaCheckin.Filial);
    new BuscarTiposOperacao(_pesquisaRecusaCheckin.TipoOperacao);
    new BuscarTransportadores(_pesquisaRecusaCheckin.Transportador);

    loadDadosUsuarioLogado(atualizarGridRecusaCheckin);

    _modalDelegarSelecionados = new bootstrap.Modal(document.getElementById("divModalDelegarSelecionados"), { backdrop: true, keyboard: true });
    _modalRejeitarRecusaCheckin = new bootstrap.Modal(document.getElementById("divModalRejeitarRecusaCheckin"), { backdrop: true, keyboard: true });
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaRecusaCheckin.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaRecusaCheckin.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridRecusaCheckin() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharRecusaCheckin,
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
        SelecionarTodosKnout: _pesquisaRecusaCheckin.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    };

    var configuracaoExportacao = {
        url: "AutorizacaoRecusaCheckin/ExportarPesquisa",
        titulo: "Autorização de Recusa de Checkins"
    };

    _gridRecusaCheckin = new GridView(_pesquisaRecusaCheckin.Pesquisar.idGrid, "AutorizacaoRecusaCheckin/Pesquisa", _pesquisaRecusaCheckin, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function aprovarMultiplasRecusasCheckinClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as recusas de Checkin selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaRecusaCheckin);

        dados.SelecionarTodos = _pesquisaRecusaCheckin.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridRecusaCheckin.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridRecusaCheckin.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoRecusaCheckin/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridRecusaCheckin();
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

    _modalRejeitarRecusaCheckin.hide();
}

function exibirDelegarSelecionadosClick() {
    _modalDelegarSelecionados.show();
}

function rejeitarMultiplasRecusasCheckinClick() {
    LimparCampos(_rejeicao);

    _modalRejeitarRecusaCheckin.show();
}

function rejeitarRecusasCheckinSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as recusas de checkin selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaRecusaCheckin);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaRecusaCheckin.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridRecusaCheckin.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridRecusaCheckin.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoRecusaCheckin/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridRecusaCheckin();
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

function reprocessarMultiplasRecusasCheckinClick() {
    var dados = RetornarObjetoPesquisa(_pesquisaRecusaCheckin);

    dados.SelecionarTodos = _pesquisaRecusaCheckin.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridRecusaCheckin.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridRecusaCheckin.ObterMultiplosNaoSelecionados());

    executarReST("AutorizacaoRecusaCheckin/ReprocessarMultiplasRecusasCheckin", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.RegrasReprocessadas > 0) {
                    if (retorno.Data.RegrasReprocessadas > 1)
                        exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasReprocessadas + " recusas de checkin foram reprocessadas com sucesso.");
                    else
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "1 recusa de checkin foi reprocessada com sucesso.");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Regras de Aprovação", "Nenhuma regra encontrada para as recusas de checkin selecionadas.");

                atualizarGridRecusaCheckin();
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

function atualizarGridRecusaCheckin() {
    _pesquisaRecusaCheckin.SelecionarTodos.val(false);

    _gridRecusaCheckin.AtualizarRegistrosSelecionados([]);
    _gridRecusaCheckin.CarregarGrid();

    _situacaoCheckinUltimaPesquisa = _pesquisaRecusaCheckin.Situacao.val();

    exibirMultiplasOpcoes();
}

function exibirMultiplasOpcoes() {
    _pesquisaRecusaCheckin.AprovarTodas.visible(false);
    _pesquisaRecusaCheckin.DelegarTodas.visible(false);
    _pesquisaRecusaCheckin.RejeitarTodas.visible(false);
    _pesquisaRecusaCheckin.ReprocessarTodas.visible(false);

    var existemRegistrosSelecionados = _gridRecusaCheckin.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaRecusaCheckin.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoCheckinUltimaPesquisa == EnumSituacaoCheckin.AguardandoAprovacao) {
            _pesquisaRecusaCheckin.AprovarTodas.visible(true);
            _pesquisaRecusaCheckin.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaRecusaCheckin.RejeitarTodas.visible(true);
        }
        else if (_situacaoCheckinUltimaPesquisa == EnumSituacaoCheckin.SemRegraAprovacao)
            _pesquisaRecusaCheckin.ReprocessarTodas.visible(true);
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharRecusaCheckin(registroSelecionado) {
    var pesquisa = RetornarObjetoPesquisa(_pesquisaRecusaCheckin);

    _recusaCheckin.Codigo.val(registroSelecionado.Codigo);
    _recusaCheckin.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_recusaCheckin, "AutorizacaoRecusaCheckin/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoCheckin.AguardandoAprovacao);

                $modalDetalhesRecusaCheckin.modal("show");
                $modalDetalhesRecusaCheckin.one('hidden.bs.modal', function () {
                    limparCamposRecusaCheckin();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function limparCamposRecusaCheckin() {
    $("#myTab a:first").tab("show");

    limparRegras();
}

// #endregion Funções
