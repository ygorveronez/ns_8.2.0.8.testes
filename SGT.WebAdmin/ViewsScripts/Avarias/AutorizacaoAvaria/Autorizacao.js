/// <reference path="Delegar.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/MotivoAvaria.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAvaria.js" />
/// <reference path="../../Enumeradores/EnumEtapaAutorizacaoAvaria.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeMotivoAvaria.js" />
/// <reference path="AutorizarRegras.js" />
/// <reference path="Anexos.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _solicitacaoAvaria;
var _pesquisaAvarias;
var _rejeicao;
var _valores;
var _autorizacao;
var _gridAvaria;
var _modalAvaria;


var _situacaoAvaria = [
    { text: "Todas", value: EnumSituacaoAvaria.Todas },
    { text: "Ag Lote", value: EnumSituacaoAvaria.AgLote },
    { text: "Ag Aprovação", value: EnumSituacaoAvaria.AgAprovacao },
    { text: "Em Criação", value: EnumSituacaoAvaria.EmCriacao },
    { text: "Finalizada", value: EnumSituacaoAvaria.Finalizada },
    { text: "Rejeitada", value: EnumSituacaoAvaria.RejeitadaAutorizacao }
];

var _etapaAutorizacaoAvaria = [
    { text: "Todas", value: EnumEtapaAutorizacaoAvaria.Todas },
    { text: "Aprovação", value: EnumEtapaAutorizacaoAvaria.Aprovacao },
    { text: "Integração", value: EnumEtapaAutorizacaoAvaria.Integracao },
    { text: "Lote", value: EnumEtapaAutorizacaoAvaria.Lote }
];

const RegraAvaria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

const Autorizacao = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Justificativa = PropertyEntity({ text: "*Justificativa:", type: types.entity, codEntity: ko.observable(0), required: true, enable: true, idBtnSearch: guid(), visible: ko.observable(false) });
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarAvariaClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: aprovarMultiplasRegrasClick, text: "Aprovar Regras" });
}

const RejeitarSelecionados = function () {
    this.Justificativa = PropertyEntity({ text: "*Justificativa:", type: types.entity, codEntity: ko.observable(0), required: true, enable: true, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarAvariaSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

const SolicitacaoAvaria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorAvaria = PropertyEntity({ text: "Valor da Avaria: ", visible: ko.observable(true), val: ko.observable("") });
    this.NumeroAvaria = PropertyEntity({ text: "Número da Avaria: ", visible: ko.observable(true), val: ko.observable("") });
    this.DataAvaria = PropertyEntity({ text: "Data Avaria: ", visible: ko.observable(true), val: ko.observable("") });
    this.CodigoCarga = PropertyEntity({ text: "Carga: ", visible: ko.observable(true), val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true), val: ko.observable("") });
    this.Transportador = PropertyEntity({ text: "Transportador: ", visible: ko.observable(true), val: ko.observable("") });
    this.Veiculos = PropertyEntity({ text: "Veículos: ", visible: ko.observable(true), val: ko.observable("") });

    // Campos para filtro
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
}

const PesquisaAvarias = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.NumeroAvaria = PropertyEntity({ text: "Número da Avaria:", getType: typesKnockout.int, val: ko.observable(""), def: "" });

    let situacaoPadrao = EnumSituacaoAvaria.AgAprovacao;
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        situacaoPadrao = EnumSituacaoAvaria.Todas;

    this.Situacao = PropertyEntity({ val: ko.observable(situacaoPadrao), options: _situacaoAvaria, def: situacaoPadrao, text: "Situação: " });
    this.Situacao.val.subscribe(function () {
        exibirMultiplasOpcoes();
    });
    this.NumeroCarga = PropertyEntity({ text: "Número da Carga:", getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid() });
    this.EtapaAutorizacao = PropertyEntity({ val: ko.observable(EnumEtapaAutorizacaoAvaria.Todas), options: _etapaAutorizacaoAvaria, def: EnumEtapaAutorizacaoAvaria.Todas, text: "Etapa: " });

    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Solicitacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Solicitação:", idBtnSearch: guid() });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarAvarias();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasAvariasClick, text: "Aprovar Solicitações", visible: ko.observable(false) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasAvariasClick, text: "Rejeitar Solicitações", visible: ko.observable(false) });
    this.DelegarAvarias = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: delegarMultiplasAvariasClick, text: "Delegar Solicitações", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadAutorizacao() {
    _solicitacaoAvaria = new SolicitacaoAvaria();
    KoBindings(_solicitacaoAvaria, "knockoutAvaria");

    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoAvaria");

    _pesquisaAvarias = new PesquisaAvarias();
    KoBindings(_pesquisaAvarias, "knockoutPesquisaAvarias");

    // Busca componentes pesquisa
    BuscarFuncionario(_pesquisaAvarias.Usuario);
    BuscarProdutos(_pesquisaAvarias.Produto);
    BuscarMotivoAvaria(_rejeicao.Justificativa, EnumFinalidadeMotivoAvaria.AutorizacaoAvaria);
    BuscarTransportadores(_pesquisaAvarias.Transportador);
    BuscarFilial(_pesquisaAvarias.Filial);

    // Load modulos
    loadAnexos();
    loadDelegar();
    loadRegras();

    // Busca 
    buscarAvarias();

    //Modal
    _modalAvaria = new bootstrap.Modal(document.getElementById("divModalAvaria"), { backdrop: true, keyboard: true });
}

function rejeitarAvariaSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as solicitações selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaAvarias);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Justificativa = rejeicao.Justificativa;
        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaAvarias.SelecionarTodos.val();
        dados.AvariasSelecionadas = JSON.stringify(_gridAvaria.ObterMultiplosSelecionados());
        dados.AvariasNaoSelecionadas = JSON.stringify(_gridAvaria.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoAvaria/ReprovarMultiplasSolicitacoes", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas de solicitações foram reprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada de solicitação foi reprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    buscarAvarias();
                    cancelarRejeicaoSelecionadosClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function cancelarRejeicaoSelecionadosClick() {
    LimparCampos(_rejeicao);
    Global.fecharModal('divModalRejeitarAvaria');
}

function rejeitarMultiplasAvariasClick() {
    LimparCampos(_rejeicao);
    Global.abrirModal('divModalRejeitarAvaria');
}

function delegarMultiplasAvariasClick() {
    Global.abrirModal('divModalDelegarAvaria');
}




//*******MÉTODOS*******


function buscarAvarias() {
    //-- Cabecalho
    var detalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharSolicitacao,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    //-- Reseta
    _pesquisaAvarias.SelecionarTodos.val(false);
    _pesquisaAvarias.AprovarTodas.visible(false);
    _pesquisaAvarias.RejeitarTodas.visible(false);
    _pesquisaAvarias.DelegarAvarias.visible(false);

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaAvarias.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configExportacao = {
        url: "AutorizacaoAvaria/ExportarPesquisa",
        titulo: "Autorização Avaria"
    };

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        multiplaescolha = null;
        _pesquisaAvarias.SelecionarTodos.visible(false);
    }


    _gridAvaria = new GridView(_pesquisaAvarias.Pesquisar.idGrid, "AutorizacaoAvaria/Pesquisa", _pesquisaAvarias, menuOpcoes, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridAvaria.CarregarGrid();
}

function exibirMultiplasOpcoes(e) {
    var situacaoPesquisa = _pesquisaAvarias.Situacao.val();
    var possuiSelecionado = _gridAvaria.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaAvarias.SelecionarTodos.val();
    var situacaoPermiteSelecao = (situacaoPesquisa == EnumSituacaoAvaria.AgAprovacao);
    var situacaoPermiteSelecaoDelegar = !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar && (situacaoPesquisa == EnumSituacaoAvaria.AgLote || situacaoPesquisa == EnumSituacaoAvaria.AgAprovacao);

    // Esconde todas opções
    _pesquisaAvarias.AprovarTodas.visible(false);
    _pesquisaAvarias.RejeitarTodas.visible(false);
    _pesquisaAvarias.DelegarAvarias.visible(false);

    if (possuiSelecionado || selecionadoTodos) {
        if (situacaoPermiteSelecao) {
            _pesquisaAvarias.AprovarTodas.visible(true);
            _pesquisaAvarias.RejeitarTodas.visible(true);
        }
        if (situacaoPermiteSelecaoDelegar) {
            _pesquisaAvarias.DelegarAvarias.visible(true);
        }
    }
}

function aprovarMultiplasAvariasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as solicitações selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaAvarias);

        dados.SelecionarTodos = _pesquisaAvarias.SelecionarTodos.val();
        dados.AvariasSelecionadas = JSON.stringify(_gridAvaria.ObterMultiplosSelecionados());
        dados.AvariasNaoSelecionadas = JSON.stringify(_gridAvaria.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoAvaria/AprovarMultiplasSolicitacoes", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas de solicitações foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada de solicitação foi aprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    buscarAvarias();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function detalharSolicitacao(itemGrid) {
    limparCamposAvaria();
    var pesquisa = RetornarObjetoPesquisa(_pesquisaAvarias);
    _solicitacaoAvaria.Codigo.val(itemGrid.Codigo);
    _solicitacaoAvaria.Usuario.val(pesquisa.Usuario);
    //_solicitacaoAvaria.EtapaAutorizacao.val(pesquisa.EtapaAutorizacao);

    BuscarPorCodigo(_solicitacaoAvaria, "AutorizacaoAvaria/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                // Anexos
                CarregarAnexos(_solicitacaoAvaria.Codigo.val());
                CarregarDelegar(arg.Data.EnumSituacao);

                AtualizarGridRegras();

                // Abre modal 
                _modalAvaria.show();
                //$("#divModalAvaria").one('hidden.bs.modal', function () {
                //    limparCamposAvaria();
                //});
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, null);
}

function limparCamposAvaria() {
    resetarTabs();
    limparAnexos();
    limparRegras();
    LimparDelegar();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}
