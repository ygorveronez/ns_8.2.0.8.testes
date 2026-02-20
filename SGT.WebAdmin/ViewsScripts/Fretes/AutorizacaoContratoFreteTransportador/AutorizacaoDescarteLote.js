/// <reference path="../../../js/Global/CRUD.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoContratoFreteTransportador.js" />
/// <reference path="AutorizarRegras.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _contratoFrete;
var _pesquisaContrato;
var _rejeicao;
var _autorizacao;
var _gridContratoFrete;
var $modalContratoFrete;
var _modalContratoFrete;
var _modalRejeitarContratoFrete;
var _gridAcordos;

//var _situacaoContrato = [
//    { text: "Todas", value: "" },
//    { text: "Ag. Aprovação", value: EnumSituacaoContratoFreteTransportador.AgAprovacao },
//    { text: "Finalizado", value: EnumSituacaoContratoFreteTransportador.Finalizado },
//    { text: "Rejeitada", value: EnumSituacaoContratoFreteTransportador.Rejeitada }
//];

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarContratoSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var ContratoFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EnumSituacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: "Descrição: ", visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", visible: ko.observable(true) });
    this.ValorMensal = PropertyEntity({ text: "Valor Mensal: ", visible: ko.observable(true) });
    this.TipoFechamento = PropertyEntity({ text: "Tipo Fechamento: ", visible: ko.observable(true) });
    this.QuantidadeMensalCargas = PropertyEntity({ text: "Quantidade Mensal de Cargas: ", visible: ko.observable(true) });
    this.TipoCargas = PropertyEntity({ text: "Tipos de Cargas: ", visible: ko.observable(true) });
    this.CanaisEntrega = PropertyEntity({ text: "Canais de Entrega: ", visible: ko.observable(true) });
    //this.Acordos = PropertyEntity({ text: "Modelos de Veículos: ", visible: ko.observable(true) });
    this.Acordos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
    // Campos para filtro
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
}

var PesquisaContrato = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int, val: ko.observable(""), def: "" });

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoContratoFreteTransportador.AgAprovacao), options: EnumSituacaoContratoFreteTransportador.ObterOpcoes(), def: EnumSituacaoContratoFreteTransportador.AgAprovacao, text: "Situação: " });
    this.Situacao.val.subscribe(function () {
        exibirMultiplasOpcoes();
    });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });

    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCT });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarContratos();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplosContratosClick, text: "Aprovar Contratos", visible: ko.observable(false) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplosContratosClick, text: "Rejeitar Contratos", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadAutorizacao() {
    _contratoFrete = new ContratoFrete();
    KoBindings(_contratoFrete, "knockoutContrato");

    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoContratoFrete");

    _pesquisaContrato = new PesquisaContrato();
    KoBindings(_pesquisaContrato, "knockoutPesquisaContratosFrete");

    $modalContratoFrete = $("#divModalContratoFrete");
    // Busca componentes pesquisa
    new BuscarFuncionario(_pesquisaContrato.Usuario);
    new BuscarTransportadores(_pesquisaContrato.Transportador);

    // Load modulos
    loadRegras();

    // Busca 
    loadDadosUsuarioLogado(BuscarContratos);
    _modalContratoFrete = new bootstrap.Modal(document.getElementById("divModalContratoFrete"), { backdrop: true, keyboard: true });
    _modalRejeitarContratoFrete = new bootstrap.Modal(document.getElementById("divModalRejeitarContratoFrete"), { backdrop: true, keyboard: true });
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaContrato.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaContrato.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function rejeitarContratoSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas os contratos selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaContrato);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaContrato.SelecionarTodos.val();
        dados.ContratosSelecionados = JSON.stringify(_gridContratoFrete.ObterMultiplosSelecionados());
        dados.ContratosNaoSelecionados = JSON.stringify(_gridContratoFrete.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoContratoFreteTransportador/ReprovarMultiplosContratos", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas de contratos foram reprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada de contratos foi reprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    BuscarContratos();
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
    _modalRejeitarContratoFrete.hide();
}

function rejeitarMultiplosContratosClick() {
    LimparCampos(_rejeicao);
    _modalRejeitarContratoFrete.show();
}




//*******MÉTODOS*******


function BuscarContratos() {
    //-- Cabecalho
    var detalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharContrato,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    //-- Reseta
    _pesquisaContrato.SelecionarTodos.val(false);
    _pesquisaContrato.AprovarTodas.visible(false);
    _pesquisaContrato.RejeitarTodas.visible(false);

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaContrato.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configExportacao = {
        url: "AutorizacaoContratoFreteTransportador/ExportarPesquisa",
        titulo: "Autorização Contrato"
    };

    _gridContratoFrete = new GridView(_pesquisaContrato.Pesquisar.idGrid, "AutorizacaoContratoFreteTransportador/Pesquisa", _pesquisaContrato, menuOpcoes, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridContratoFrete.CarregarGrid();
}

function exibirMultiplasOpcoes(e) {
    var situacaoPesquisa = _pesquisaContrato.Situacao.val();
    var possuiSelecionado = _gridContratoFrete.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaContrato.SelecionarTodos.val();
    var situacaoPermiteSelecao = (situacaoPesquisa == EnumSituacaoContratoFreteTransportador.AgAprovacao);

    // Esconde todas opções
    _pesquisaContrato.AprovarTodas.visible(false);
    _pesquisaContrato.RejeitarTodas.visible(false);

    if (possuiSelecionado || selecionadoTodos) {
        if (situacaoPermiteSelecao) {
            _pesquisaContrato.AprovarTodas.visible(true);
            _pesquisaContrato.RejeitarTodas.visible(true);
        }
    }
}

function aprovarMultiplosContratosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas os contratos selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaContrato);

        dados.SelecionarTodos = _pesquisaContrato.SelecionarTodos.val();
        dados.ContratosSelecionados = JSON.stringify(_gridContratoFrete.ObterMultiplosSelecionados());
        dados.ContratosNaoSelecionados = JSON.stringify(_gridContratoFrete.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoContratoFreteTransportador/AprovarMultiplosContratos", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        var msg = "";
                        if (arg.Data.RegrasModificadas > 1) msg = arg.Data.RegrasModificadas + " alçadas de contrato foram aprovadas.";
                        else msg = arg.Data.RegrasModificadas + " alçada de contrato foi aprovada.";

                        exibirMensagem(tipoMensagem.ok, "Sucesso", msg);
                    }
                    else if (arg.Data.Msg == "") {
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    }

                    if (arg.Data.Msg != "")
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Data.Msg);

                    BuscarContratos();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function detalharContrato(itemGrid) {
    limparCamposContrato();
    var pesquisa = RetornarObjetoPesquisa(_pesquisaContrato);
    _contratoFrete.Codigo.val(itemGrid.Codigo);
    _contratoFrete.Usuario.val(pesquisa.Usuario);

    _contratoFrete.Acordos.visible = false;
        
    BuscarPorCodigo(_contratoFrete, "AutorizacaoContratoFreteTransportador/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                AtualizarGridRegras();

                _gridAcordos = new GridView(_contratoFrete.Acordos.idGrid, "AutorizacaoContratoFreteTransportador/BuscarGridAcordos", _contratoFrete, null);
                _gridAcordos.CarregarGrid();
                _contratoFrete.Acordos.visible = true;
                // Abre modal 
                _modalContratoFrete.show();
                $modalContratoFrete.one('hidden.bs.modal', function () {
                    limparCamposContrato();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, null);
}

function limparCamposContrato() {
    resetarTabs();
    limparRegras();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}
