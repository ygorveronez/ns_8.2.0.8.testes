/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOrdemServicoFrota.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridOrdensServico;
var _gridServicosOrdemServico;
var _ordemServico;
var _pesquisaOrdemServico;
var _rejeicao;
var _situacaoOrdemServicoUltimaPesquisa = EnumSituacaoOrdemServicoFrota.AguardandoAprovacao;
var $modalDetalhesOrdemServico;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarOrdensServicoSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var OrdemServico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Cidade = PropertyEntity({ text: "Cidade: ", visible: ko.observable(true) });
    this.Data = PropertyEntity({ text: "Data: ", visible: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true) });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.Operador = PropertyEntity({ text: "Operador: ", visible: ko.observable(true) });
    this.Fornecedor = PropertyEntity({ text: "Fornecedor: ", visible: ko.observable(true) });
    this.Placa = PropertyEntity({ text: "Placa: ", visible: ko.observable(true) });
    this.Servicos = PropertyEntity({ text: "Serviços: ", visible: ko.observable(true) });
    this.ValorProdutos = PropertyEntity({ text: "Valor dos Produtos: ", visible: ko.observable(true) });
    this.ValorMaoDeObra = PropertyEntity({ text: "Valor Mão de Obra: ", visible: ko.observable(true) });
    this.ValorTotalServicos = PropertyEntity({ text: "Valor Total do Serviço: ", visible: ko.observable(true) });
    this.AbrirOrdemServicoCarregada = PropertyEntity({ eventClick: abrirOrdemServicoCarregadaClick, type: types.event, text: "Abrir Ordem de Serviço", visible: ko.observable(true) });
};

var PesquisaOrdemServico = function () {
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoOrdemServicoFrota.AguardandoAprovacao), options: EnumSituacaoOrdemServicoFrota.ObterOpcoesPesquisa(), def: EnumSituacaoOrdemServicoFrota.AguardandoAprovacao, text: "Situação: " });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid() });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasOrdensServicoClick, text: "Aprovar Ordens de Serviço", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridOrdensServico, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasOrdensServicoClick, text: "Rejeitar Ordens de Serviço", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Ordens de Serviço", visible: ko.observable(false) });
};

var ServicosOrdemServico = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            _gridServicosOrdemServico.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _ordemServico = new OrdemServico();
    KoBindings(_ordemServico, "knockoutOrdemServico");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoOrdemServico");

    _pesquisaOrdemServico = new PesquisaOrdemServico();
    KoBindings(_pesquisaOrdemServico, "knockoutPesquisaOrdemServico");

    _servicosOrdemServico = new ServicosOrdemServico();
    KoBindings(_servicosOrdemServico, "knockoutServicosOrdemServico");

    loadGridServicosOrdemServico();
    loadGridOrdensServico();
    loadRegras();
    loadDelegar();

    $modalDetalhesOrdemServico = $("#divModalOrdemServico");

    new BuscarClientes(_pesquisaOrdemServico.Fornecedor);
    new BuscarFuncionario(_pesquisaOrdemServico.Operador);
    new BuscarFuncionario(_pesquisaOrdemServico.Usuario);

    loadDadosUsuarioLogado(atualizarGridOrdensServico);
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaOrdemServico.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaOrdemServico.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridOrdensServico() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharOrdemServico,
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
        SelecionarTodosKnout: _pesquisaOrdemServico.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configuracaoExportacao = {
        url: "AutorizacaoOrdemServico/ExportarPesquisa",
        titulo: "Autorização Ordem de Serviço"
    };

    _gridOrdensServico = new GridView(_pesquisaOrdemServico.Pesquisar.idGrid, "AutorizacaoOrdemServico/Pesquisa", _pesquisaOrdemServico, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

function loadGridServicosOrdemServico() {
    _gridServicosOrdemServico = new GridView(_servicosOrdemServico.Pesquisar.idGrid, "AutorizacaoOrdemServico/PesquisarServicosOrdemServico", _ordemServico);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasOrdensServicoClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as Ordens de Serviço selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaOrdemServico);

        dados.SelecionarTodos = _pesquisaOrdemServico.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridOrdensServico.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridOrdensServico.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoOrdemServico/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridOrdensServico();
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

    Global.fecharModal('divModalRejeitarOrdemServico');
}

function exibirDelegarSelecionadosClick() {
    Global.abrirModal('divModalDelegarSelecionados');
}

function rejeitarMultiplasOrdensServicoClick() {
    LimparCampos(_rejeicao);

    Global.abrirModal('divModalRejeitarOrdemServico');
}

function rejeitarOrdensServicoSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as Ordens de Serviço selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaOrdemServico);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaOrdemServico.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridOrdensServico.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridOrdensServico.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoOrdemServico/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridOrdensServico();
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

function atualizarGridOrdensServico() {
    _pesquisaOrdemServico.SelecionarTodos.val(false);
    _pesquisaOrdemServico.AprovarTodas.visible(false);
    _pesquisaOrdemServico.DelegarTodas.visible(false);
    _pesquisaOrdemServico.RejeitarTodas.visible(false);

    _gridOrdensServico.CarregarGrid();

    _situacaoOrdemServicoUltimaPesquisa = _pesquisaOrdemServico.Situacao.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaOrdemServico.AprovarTodas.visible(false);
    _pesquisaOrdemServico.DelegarTodas.visible(false);
    _pesquisaOrdemServico.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridOrdensServico.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaOrdemServico.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoOrdemServicoUltimaPesquisa == EnumSituacaoOrdemServicoFrota.AguardandoAprovacao) {
            _pesquisaOrdemServico.AprovarTodas.visible(true);
            _pesquisaOrdemServico.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaOrdemServico.RejeitarTodas.visible(true);
        }
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharOrdemServico(registroSelecionado) {
    limparCamposOrdemServico();

    var pesquisa = RetornarObjetoPesquisa(_pesquisaOrdemServico);

    _ordemServico.Codigo.val(registroSelecionado.Codigo);
    _ordemServico.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_ordemServico, "AutorizacaoOrdemServico/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();

                _gridServicosOrdemServico.CarregarGrid();

                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoOrdemServicoFrota.AguardandoAprovacao);

                Global.abrirModal("divModalOrdemServico");
                $modalDetalhesOrdemServico.one('hidden.bs.modal', function () {
                    limparCamposOrdemServico();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function limparCamposOrdemServico() {
    Global.ResetarAbas();

    limparRegras();
}

function abrirOrdemServicoCarregadaClick() {
    CODIGO_ORDEM_SERVICO_PARA_TELA_ORDEM_SERVICO = _ordemServico.Codigo.val();
    location.pathname = "";

    var linkUrl = `/#Frota/OrdemServico?ordem=${CODIGO_ORDEM_SERVICO_PARA_TELA_ORDEM_SERVICO}`;

    window.open(linkUrl, '_blank'); 
}