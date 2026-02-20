/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="Frete.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/MotivoSolicitacaoFrete.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Porto.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _estornoProvisao;
var _gridEstornoProvisao;
var _pesquisaEstornoProvisao;
var _rejeicao;
var _situacaoEstornoProvisaoSolicitacaoUltimaPesquisa = EnumSituacaoAprovacao.AguardandoAprovacao;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var EstornoProvisao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoSolicitacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NumeroProvisao = PropertyEntity({ text: "Número da Provisão:" });
    this.Transportador = PropertyEntity({ text: "Transportador:" });
    this.Filial = PropertyEntity({ text: "Filial: " });
    this.Tomador = PropertyEntity({ text: "Tomador: " });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: " });
    this.DataFinal = PropertyEntity({ text: "Data Final: " });
    this.Carga = PropertyEntity({ text: "Carga: " });
    this.Ocorrencia = PropertyEntity({ text: "Ocorrência: " });
    this.ValorProvisao = PropertyEntity({ text: "Valor da Provisão: " });
    this.ValorFrete = PropertyEntity({ text: "Valor Frete: " });

};

var PesquisaEstornoProvisao = function () {

    this.NumeroLote = PropertyEntity({ text: "Número Lote:", val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataGeracaoLoteInicial = PropertyEntity({ text: "Data Geração Lote Inicial: ", getType: typesKnockout.date });
    this.DataGeracaoLoteFinal = PropertyEntity({ text: "Data Geração Lote Final: ", getType: typesKnockout.date });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroProvisao = PropertyEntity({ text: "Número Provisão:", val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAprovacao.Todas), options: EnumSituacaoAprovacao.obterOpcoesPesquisa(), def: EnumSituacaoAprovacao.Todas, text: "Situação: " });

    this.DataGeracaoLoteInicial.dateRangeLimit = this.DataGeracaoLoteFinal;
    this.DataGeracaoLoteFinal.dateRangeInit = this.DataGeracaoLoteInicial;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplosClick, text: "Aprovar", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridEstornoProvisao, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplosClick, text: "Rejeitar", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todos", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar", visible: ko.observable(false) });
    this.ReprocessarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: reprocessarMultiplosClick, text: "Reprocessar", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacaoEstornoProvisao() {
    _estornoProvisao = new EstornoProvisao();
    KoBindings(_estornoProvisao, "knockoutEstornoProvisao");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoEstornoProvisao");

    _pesquisaEstornoProvisao = new PesquisaEstornoProvisao();
    KoBindings(_pesquisaEstornoProvisao, "knockoutPesquisaEstornoProvisao");

    loadGridEstornoProvisao();
    loadRegras();
    loadDelegar();
    loadAnexos();

    new BuscarTransportadores(_pesquisaEstornoProvisao.Transportador);
    new BuscarCargas(_pesquisaEstornoProvisao.Carga);
    new BuscarClientes(_pesquisaEstornoProvisao.Tomador);

    loadDadosUsuarioLogado(atualizarGridEstornoProvisao);
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaEstornoProvisao.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaEstornoProvisao.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridEstornoProvisao() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharEstornoProvisao,
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
        SelecionarTodosKnout: _pesquisaEstornoProvisao.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    };

    var configuracaoExportacao = {
        url: "AutorizacaoEstornoProvisao/ExportarPesquisa",
        titulo: "Autorização de Estorno de Provisão"
    };

    _gridEstornoProvisao = new GridView(_pesquisaEstornoProvisao.Pesquisar.idGrid, "AutorizacaoEstornoProvisao/Pesquisa", _pesquisaEstornoProvisao, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplosClick() {
    var valorExcedido = false;
    var selecionados = _gridEstornoProvisao.ObterMultiplosSelecionados()
    for (var i in selecionados) {
        if (parseFloat(selecionados[i].ValorProvisao.replace(".", "")) > 135250) {
            valorExcedido = true;
            break;
        }
    }
    exibirConfirmacao("Confirmação", valorExcedido ? "Conforme controle GFCF C4.3D (GR IR Clearing), para valores acima de EUR 25000.00 (R$ 135.250,00), é obrigatório anexar a aprovação por e-mail do controller Brasil, contendo a referência e o valor de cada documento de forma detalhada." : "Você realmente deseja aprovar todos os registros selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaEstornoProvisao);

        dados.SelecionarTodos = _pesquisaEstornoProvisao.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridEstornoProvisao.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridEstornoProvisao.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoEstornoProvisao/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridEstornoProvisao();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        })
    }, null, valorExcedido ? "Sim já anexei e estou ciente" : null, valorExcedido ? "Não, ainda não anexei" : null);
}

function cancelarRejeicaoSelecionadosClick() {
    LimparCampos(_rejeicao);

    Global.fecharModal("divModalRejeitarEstornoProvisao");
}

function exibirDelegarSelecionadosClick() {
    Global.abrirModal("divModalDelegarSelecionados");
}

function rejeitarMultiplosClick() {
    LimparCampos(_rejeicao);

    Global.abrirModal("divModalRejeitarEstornoProvisao");
}

function rejeitarSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todos os cancelamentos de cargas selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaEstornoProvisao);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaEstornoProvisao.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridEstornoProvisao.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridEstornoProvisao.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoEstornoProvisao/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridEstornoProvisao();
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

function reprocessarMultiplosClick() {
    var dados = RetornarObjetoPesquisa(_pesquisaEstornoProvisao);

    dados.SelecionarTodos = _pesquisaEstornoProvisao.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridEstornoProvisao.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridEstornoProvisao.ObterMultiplosNaoSelecionados());

    executarReST("AutorizacaoEstornoProvisao/ReprocessarMultiplos", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.RegrasReprocessadas > 0) {
                    if (retorno.Data.RegrasReprocessadas > 1)
                        exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasReprocessadas + " cancelamentos de cargas foram reprocessados com sucesso.");
                    else
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "1 cancelamento de carga foi reprocessado com sucesso.");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Regras de Aprovação", "Nenhuma regra encontrada para os cancelamentos de cargas selecionados.");

                atualizarGridEstornoProvisao();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções
 */

function atualizarGridEstornoProvisao() {
    _pesquisaEstornoProvisao.SelecionarTodos.val(false);
    _pesquisaEstornoProvisao.AprovarTodas.visible(false);
    _pesquisaEstornoProvisao.DelegarTodas.visible(false);
    _pesquisaEstornoProvisao.RejeitarTodas.visible(false);

    _gridEstornoProvisao.CarregarGrid();

    _situacaoEstornoProvisaoSolicitacaoUltimaPesquisa = _pesquisaEstornoProvisao.Situacao.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaEstornoProvisao.AprovarTodas.visible(false);
    _pesquisaEstornoProvisao.DelegarTodas.visible(false);
    _pesquisaEstornoProvisao.RejeitarTodas.visible(false);
    _pesquisaEstornoProvisao.ReprocessarTodas.visible(false);

    var existemRegistrosSelecionados = _gridEstornoProvisao.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaEstornoProvisao.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoEstornoProvisaoSolicitacaoUltimaPesquisa == EnumSituacaoAprovacao.AguardandoAprovacao) {
            _pesquisaEstornoProvisao.AprovarTodas.visible(true);
            _pesquisaEstornoProvisao.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaEstornoProvisao.RejeitarTodas.visible(true);
        }
        else if (_situacaoEstornoProvisaoSolicitacaoUltimaPesquisa == EnumSituacaoAprovacao.SemRegraAprovacao)
            _pesquisaEstornoProvisao.ReprocessarTodas.visible(true);
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharEstornoProvisao(registroSelecionado) {
    var pesquisa = RetornarObjetoPesquisa(_pesquisaEstornoProvisao);

    _estornoProvisao.Codigo.val(registroSelecionado.Codigo);
    LimparCampos(_anexos)
    BuscarPorCodigo(_estornoProvisao, "AutorizacaoEstornoProvisao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(true);
                Global.abrirModal("divModalEstornoProvisao");
                _anexos.Anexos.list = retorno.Data.Anexos;
                RecarregarGridAnexoAdicionar();
                $("#divModalEstornoProvisao").one('hidden.bs.modal', function () {
                    limparCamposCarga();
                });
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    }, null);
}

function limparCamposCarga() {
    $("#myTab a:first").tab("show");

    limparRegras();
}
