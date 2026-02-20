/// <reference path="AutorizacaoCotacaoPedidoRegras.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumResponsavelOcorrencia.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pedido;
var _valores;
var _rejeicao;
var _autorizacao;
var _gridCotacaoPedido;
var _pesquisaCotacaoPedidos;
var _modalCotacaoPedido;
var _modalRejeitarCotacaoPedido;
var _modalDelegarCotacaoPedido;

var _situacaoCotacaoPedido = [
    { text: "Todos", value: 0 },
    { text: "Aberto", value: EnumSituacaoPedido.Aberto },
    { text: "Cancelado", value: EnumSituacaoPedido.Cancelado },
    { text: "Finalizado", value: EnumSituacaoPedido.Finalizado },
    { text: "Ag. Aprovação", value: EnumSituacaoPedido.AgAprovacao },
    { text: "Rejeitado", value: EnumSituacaoPedido.Rejeitado },
    { text: "Autorização Pendente", value: EnumSituacaoPedido.AutorizacaoPendente }
];

var _responsavelCotacaoPedido = [
    { text: "Remetente", value: EnumResponsavelOcorrencia.Remetente },
    { text: "Destinatário", value: EnumResponsavelOcorrencia.Destinatario }
];

var _responsavelCotacaoPedido = [
    { text: "Remetente", value: EnumResponsavelOcorrencia.Remetente },
    { text: "Destinatário", value: EnumResponsavelOcorrencia.Destinatario }
];

var RegraCotacaoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var Autorizacao = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Tomador = PropertyEntity({ text: "Responsável: ", val: ko.observable(EnumResponsavelOcorrencia.Destinatario), options: _responsavelCotacaoPedido, def: EnumResponsavelOcorrencia.Destinatario, visible: ko.observable(false), permiteSelecionarTomador: ko.observable(false) });
    this.Justificativa = PropertyEntity({ text: "*Justificativa:", type: types.entity, codEntity: ko.observable(0), required: true, enable: true, idBtnSearch: guid(), visible: ko.observable(false) });
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarCotacaoPedidoClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: aprovarMultiplasRegrasClick, text: "Aprovar Regras" });
}

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarCotacaoPedidosSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var CotacaoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorTotalCotacao = PropertyEntity({ text: "Valor Total da Cotação: ", visible: ko.observable(true), val: ko.observable("") });
    this.NumeroPedido = PropertyEntity({ text: "Número da Cotação: ", visible: ko.observable(true), val: ko.observable("") });
    this.DataPedido = PropertyEntity({ text: "Data da Cotação: ", visible: ko.observable(true), val: ko.observable("") });

    this.Situacao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true), val: ko.observable("") });
    this.TipoCarga = PropertyEntity({ text: "Tipo da Carga: ", visible: ko.observable(true), val: ko.observable("") });

    this.TipoOperacao = PropertyEntity({ text: "Tipo da Operação: ", visible: ko.observable(true), val: ko.observable("") });
    this.Destinatario = PropertyEntity({ text: "Destinatário: ", visible: ko.observable(true), val: ko.observable("") });

    this.Solicitante = PropertyEntity({ text: "Solicitante: ", visible: ko.observable(true), val: ko.observable("") });

    this.Observacao = PropertyEntity({ text: "Observação: ", visible: ko.observable(true), val: ko.observable("") });
    this.MotivoCancelamento = PropertyEntity({ text: "Motivo do Cancelamento: ", visible: ko.observable(true), val: ko.observable("") });

    // Campos para filtro
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.EtapaAutorizacao = PropertyEntity({ val: ko.observable(0), def: 0 });
}

var PesquisaCotacaoPedidos = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.NumeroPedido = PropertyEntity({ text: "Número:", getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoPedido.AutorizacaoPendente), options: _situacaoCotacaoPedido, def: EnumSituacaoPedido.AutorizacaoPendente, text: "Situação: " });
    this.Situacao.val.subscribe(function () {
        exibirMultiplasOpcoes();
    });

    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo da Carga:", issue: 53, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", issue: 58, idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", issue: 121, idBtnSearch: guid() });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarCotacaoPedidos();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasCotacaoPedidosClick, text: "Aprovar Cotação Pedidos", visible: ko.observable(false) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: rejeitarMultiplasRegrasClick, text: "Rejeitar Cotacão Pedidos" });
    this.DelegarCotacaoPedido = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: delegarMultiplasCotacaoPedidosClick, text: "Delegar Cotação Pedidos", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadAutorizacaoCotacaoPedido() {
    _pedido = new CotacaoPedido();
    KoBindings(_pedido, "knockoutCotacaoPedido");

    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoCotacaoPedido");

    _pesquisaCotacaoPedidos = new PesquisaCotacaoPedidos();
    KoBindings(_pesquisaCotacaoPedidos, "knockoutPesquisaCotacaoPedido", false, _pesquisaCotacaoPedidos.Pesquisar.id);

    // Busca componentes pesquisa
    new BuscarTiposdeCarga(_pesquisaCotacaoPedidos.TipoCarga);
    new BuscarTiposOperacao(_pesquisaCotacaoPedidos.TipoOperacao);
    new BuscarGruposPessoas(_pesquisaCotacaoPedidos.GrupoPessoa);
    new BuscarFuncionario(_pesquisaCotacaoPedidos.Usuario);

    // Load modulos    
    loadRegras();

    // Filtrar Alcadas Do Usuario
    FiltrarAlcadasDoUsuario(buscarCotacaoPedidos);

    _modalCotacaoPedido = new bootstrap.Modal(document.getElementById("divModalCotacaoPedido"), { backdrop: true, keyboard: true });
    _modalRejeitarCotacaoPedido = new bootstrap.Modal(document.getElementById("divModalRejeitarCotacaoPedido"), { backdrop: true, keyboard: true });
}

//*******MÉTODOS*******
function buscarCotacaoPedidos() {
    //-- Cabecalho
    var detalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharCotacaoPedido, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhes);

    //-- Reseta
    _pesquisaCotacaoPedidos.SelecionarTodos.val(false);
    _pesquisaCotacaoPedidos.AprovarTodas.visible(false);
    _pesquisaCotacaoPedidos.RejeitarTodas.visible(false);
    _pesquisaCotacaoPedidos.DelegarCotacaoPedido.visible(false);

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaCotacaoPedidos.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var ordenacaoPadrao = { column: 2, dir: orderDir.asc };

    var configExportacao = {
        url: "AutorizacaoCotacaoPedido/ExportarPesquisa",
        titulo: "Autorização Cotação Pedido"
    };

    _gridCotacaoPedido = new GridView(_pesquisaCotacaoPedidos.Pesquisar.idGrid, "AutorizacaoCotacaoPedido/Pesquisa", _pesquisaCotacaoPedidos, menuOpcoes, ordenacaoPadrao, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridCotacaoPedido.CarregarGrid();
}

function exibirMultiplasOpcoes(e) {
    var situacaoPesquisa = _pesquisaCotacaoPedidos.Situacao.val();
    var possuiSelecionado = _gridCotacaoPedido.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaCotacaoPedidos.SelecionarTodos.val();
    var situacaoPermiteSelecao = (situacaoPesquisa == EnumSituacaoPedido.AgAprovacao || situacaoPesquisa == EnumSituacaoPedido.AutorizacaoPendente);
    var situacaoPermiteSelecaoDelegar = (situacaoPesquisa == EnumSituacaoPedido.AgAprovacao || situacaoPesquisa == EnumSituacaoPedido.AutorizacaoPendente);

    // Esconde todas opções
    _pesquisaCotacaoPedidos.AprovarTodas.visible(false);
    _pesquisaCotacaoPedidos.RejeitarTodas.visible(false);
    _pesquisaCotacaoPedidos.DelegarCotacaoPedido.visible(false);

    if (possuiSelecionado || selecionadoTodos) {
        if (situacaoPermiteSelecao) {
            _pesquisaCotacaoPedidos.AprovarTodas.visible(true);
            _pesquisaCotacaoPedidos.RejeitarTodas.visible(true);
        }
        if (situacaoPermiteSelecaoDelegar) {
            _pesquisaCotacaoPedidos.DelegarCotacaoPedido.visible(false);
        }
    }
}

function rejeitarCotacaoPedidosSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as Cotações de Pedidos selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaCotacaoPedidos);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Justificativa = rejeicao.Justificativa;
        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaCotacaoPedidos.SelecionarTodos.val();
        dados.CotacaoPedidosSelecionadas = JSON.stringify(_gridCotacaoPedido.ObterMultiplosSelecionados());
        dados.CotacaoPedidosNaoSelecionadas = JSON.stringify(_gridCotacaoPedido.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoCotacaoPedido/ReprovarMultiplasCotacaoPedidos", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas de Cotações de Pedidos foram reprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada de Cotação Pedido foi reprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    buscarCotacaoPedidos();
                    cancelarRejeicaoSelecionadosClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    });
}

function rejeitarMultiplasRegrasClick() {
    LimparCampos(_rejeicao);
    _modalRejeitarCotacaoPedido.show();

}

function delegarMultiplasCotacaoPedidosClick() {
    _modalDelegarCotacaoPedido.show();
}

function aprovarMultiplasCotacaoPedidosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as Cotações de Pedidos selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaCotacaoPedidos);

        dados.SelecionarTodos = _pesquisaCotacaoPedidos.SelecionarTodos.val();
        dados.CotacaoPedidosSelecionadas = JSON.stringify(_gridCotacaoPedido.ObterMultiplosSelecionados());
        dados.CotacaoPedidosNaoSelecionadas = JSON.stringify(_gridCotacaoPedido.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoCotacaoPedido/AprovarMultiplasCotacaoPedidos", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas de Cotações de Pedidos foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada de Cotação Pedido foi aprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    buscarCotacaoPedidos();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        })
    });
}

function detalharCotacaoPedido(ocorrenciaGrid) {
    limparCamposCotacaoPedido();
    var pesquisa = RetornarObjetoPesquisa(_pesquisaCotacaoPedidos);
    _pedido.Codigo.val(ocorrenciaGrid.Codigo);
    _pedido.Usuario.val(pesquisa.Usuario);
    _pedido.EtapaAutorizacao.val(pesquisa.EtapaAutorizacao);

    BuscarPorCodigo(_pedido, "AutorizacaoCotacaoPedido/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {

                AtualizarGridRegras();

                if (arg.Data.PermiteSelecionarTomador) {
                    _autorizacao.Tomador.val(arg.Data.Tomador);
                    _autorizacao.Tomador.permiteSelecionarTomador(true);
                }
                else
                    _autorizacao.Tomador.permiteSelecionarTomador(false);

                // Abre modal da ocorrencia
                _modalCotacaoPedido.show();
                $("#divModalCotacaoPedido").one('hidden.bs.modal', function () {
                    limparCamposCotacaoPedido();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, null);
}

function cancelarRejeicaoSelecionadosClick() {
    LimparCampos(_rejeicao);
    _modalRejeitarCotacaoPedido.hide();
}

function limparCamposCotacaoPedido() {
    resetarTabs();
    limparRegras();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}


function FiltrarAlcadasDoUsuario(callback) {
    // Oculta campos conforme configurações
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario) {
        executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false && arg.Data != null) {
                    _pesquisaCotacaoPedidos.Usuario.codEntity(arg.Data.Codigo);
                    _pesquisaCotacaoPedidos.Usuario.val(arg.Data.Nome);
                    callback();
                }
            }
        })
    } else {
        callback();
    }
}