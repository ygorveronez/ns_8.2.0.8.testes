/*ControleContainer.js*/
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/MapaDraw.js" />
/// <reference path="../../../js/Global/MapaGoogle.js" />
/// <reference path="../../../js/Global/Mapa.js"/>
/// <reference path="../../../js/Global/MapaGoogleSearch.js"/>
/// <reference path="../../../js/Global/Charts.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../Enumeradores/EnumstatusColetaContainer.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />
/// <reference path="../../Logistica/tracking/tracking.lib.js" />
/// <reference path="../../../js/Global/PermissoesPersonalizadas.js" />
/// <reference path="../../Consultas/JustificativaContainer.js" />

var _gridControleContainer;
var _pesquisaControleContainer;
var _mapaMonitoramento;
var _mapaHistoricos;
var _gridHistoricos;
var _gridHistoricosJustificativas;
var _justificativaColetaContainer;

/*
 * Declaração das Classes
 */


var _editarColetaContainer;
var ColetaContainer = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.TipoContainer = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });

    this.Carga = PropertyEntity({ text: "", val: ko.observable("") });
    this.DiasFreeTime = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: "Dias freeTime:", visible: ko.observable(true), required: true });
    this.ValorDiaria = PropertyEntity({ text: "Valor Diaria: ", getType: typesKnockout.decimal, visible: ko.observable(true), required: true });
    this.StatusContainer = PropertyEntity({ val: ko.observable(EnumStatusColetaContainer.Todas), options: EnumStatusColetaContainer.obterOpcoesPesquisa(), def: EnumStatusColetaContainer.Todas, text: "Status: ", visible: ko.observable(true) });
    this.DataColeta = PropertyEntity({ text: "Data Coleta: ", getType: typesKnockout.date, val: ko.observable(null), visible: ko.observable(false) });
    this.DataEmbarque = PropertyEntity({ text: "Data Porto: ", getType: typesKnockout.date, val: ko.observable(null), visible: ko.observable(false) });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Container:", idBtnSearch: guid() });
    this.LocalColeta = PropertyEntity({ text: "Area de Coleta:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });

    this.StatusContainer.val.subscribe(function (valor) {
        _editarColetaContainer.DataEmbarque.visible(valor == EnumStatusColetaContainer.Porto);
        _editarColetaContainer.DataColeta.visible(valor != EnumStatusColetaContainer.AguardandoColeta);
    });

    this.Salvar = PropertyEntity({ type: types.event, eventClick: atualizarColetaContainerClick, text: "Atualizar" });
};

var PesquisaControleContainer = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaControleContainer)) {
                _pesquisaControleContainer.ExibirFiltros.visibleFade(false);
                _gridControleContainer.CarregarGrid();
            }
            else
                exibirMensagemCamposObrigatorio();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Carga: ", col: 12 });
    this.NumeroContainer = PropertyEntity({ text: "Número Container: ", col: 12 });
    this.DataInicialColeta = PropertyEntity({ text: "Data Inicial Coleta: ", getType: typesKnockout.date, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataFinalColeta = PropertyEntity({ text: "Data Final Coleta: ", getType: typesKnockout.date, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataInicialColeta.dateRangeLimit = this.DataFinalColeta;
    this.DataFinalColeta.dateRangeInit = this.DataInicialColeta;
    this.StatusContainer = PropertyEntity({ val: ko.observable(EnumStatusColetaContainer.Todas), options: EnumStatusColetaContainer.obterOpcoesPesquisa(), def: EnumStatusColetaContainer.Todas, text: "Situação Container: " });
    this.DiasPosseInicio = PropertyEntity({ text: "Dias em Posse Inicial: ", col: 12 });
    this.DiasPosseFim = PropertyEntity({ text: "Dias em Posse Final: ", col: 12 });

    this.LocalEsperaVazio = PropertyEntity({ text: "Local de Espera Vazio:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalColeta = PropertyEntity({ text: "Local de Coleta:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalAtual = PropertyEntity({ text: "Local Atual:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataEmbarque = PropertyEntity({ text: "Data Porto: ", getType: typesKnockout.date, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataUltimaMovimentacao = PropertyEntity({ text: "Data Movimentação: ", getType: typesKnockout.date, val: ko.observable(null), cssClass: ko.observable("") });
    this.NumeroPedido = PropertyEntity({ text: "Número Pedido: ", col: 12 });
    this.NumeroBooking = PropertyEntity({ text: "Número Booking: ", col: 12 });
    this.NumeroEXP = PropertyEntity({ text: "Número EXP: ", col: 12 });

    this.FilialAtual = PropertyEntity({ text: "Filial:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoContainer = PropertyEntity({ text: "Tipo Container:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataEmbarqueNavioInicial = PropertyEntity({ text: "Data Embarque no Navio de:", getType: typesKnockout.date, visible: ko.observable(true), def: Global.DataHora(EnumTipoOperacaoDate.Subtract, 3, EnumTipoOperacaoObjetoDate.Months), val: ko.observable(Global.DataHora(EnumTipoOperacaoDate.Subtract, 3, EnumTipoOperacaoObjetoDate.Months))});
    this.DataEmbarqueNavioFinal = PropertyEntity({ text: "Data Embarque no Navio até:", getType: typesKnockout.date, visible: ko.observable(true), def: Global.DataHoraAtual(), val: ko.observable(Global.DataHoraAtual()) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });


};

var JustificativaColetaContainer = function () {
    this.Codigo = PropertyEntity({ text: "Código: " });
    this.Justificativa = PropertyEntity({ text: "Justificativa: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });
    this.StatusContainer = PropertyEntity({ val: ko.observable(EnumStatusColetaContainer.AguardandoColeta), options: ko.observableArray([]), def: EnumStatusColetaContainer.AguardandoColeta, text: "Situação:", required: true });
    this.JustificativaDescritiva = PropertyEntity({ text: "Justificativa Descritiva: ", maxlength: 300, getType: typesKnockout.string, val: ko.observable(""), required: true });

    this.Confirmar = PropertyEntity({ text: "Adicionar", type: types.event, val: ko.observable(false), eventClick: ConfirmarAlterarJustificativaClick, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarJustificativaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirJustificativaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarJustificativaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

function loadPesquisaControleColeta() {
    _pesquisaControleContainer = new PesquisaControleContainer();
    KoBindings(_pesquisaControleContainer, "knockoutPesquisaContainer", false, _pesquisaControleContainer.Pesquisar.id);
}

function loadJustificativaColeta() {
    _justificativaColetaContainer = new JustificativaColetaContainer();
    KoBindings(_justificativaColetaContainer, "knockoutAlterarJustificativa");

    HeaderAuditoria("ColetaContainerJustificativa", _justificativaColetaContainer);
}

function loadEditarColetaContainer() {
    _editarColetaContainer = new ColetaContainer();
    KoBindings(_editarColetaContainer, "knockoutAlterarColetaContainer");
}

function loadGridControleContainer() {
    var draggableRows = false;
    var draggableRows = false;
    var limiteRegistros = 100;
    var totalRegistrosPorPagina = 100;

    var opcaoVisualizarMapa = { descricao: "Visualizar no mapa", id: guid(), evento: "onclick", metodo: visualizarMapaClick, tamanho: "10", icone: "" };
    var opcaoHistoricos = { descricao: "Históricos", id: guid(), evento: "onclick", metodo: visualizarHistoricosClick, tamanho: "8", icone: "" };
    var opcaoJustificativa = { descricao: "Justificativa", id: guid(), evento: "onclick", metodo: visualizarJustificativaClick, tamanho: "8", icone: "" };
    var opcaoMovimentarContainer = { descricao: "Movimentar Container", id: guid(), evento: "onclick", metodo: movimentarContainerClick, tamanho: "8", icone: "", visibilidade: obterVisibilidadeOpcaoMovimentarContainer };
    var opcaoAlterarDados = { descricao: "Alterar Dados", id: guid(), evento: "onclick", metodo: alterarDadosClick, tamanho: "8", icone: "", visibilidade: obterVisibilidadeOpcaoMovimentarContainer };
    var auditar = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("ColetaContainer", null, _editarColetaContainer), tamanho: "10", icone: "", visibilidade: true };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoVisualizarMapa, opcaoHistoricos, opcaoJustificativa, opcaoMovimentarContainer, opcaoAlterarDados, auditar], tamanho: 5, };
    var configuracoesExportacao = { url: "ControleContainer/ExportarPesquisa", titulo: "Controle Container" };

    _gridControleContainer = new GridView("grid-controle-container", "ControleContainer/Pesquisa", _pesquisaControleContainer, menuOpcoes, null, totalRegistrosPorPagina, null, true, draggableRows, undefined, limiteRegistros, undefined, configuracoesExportacao);
    _gridControleContainer.SetPermitirEdicaoColunas(true);
    _gridControleContainer.SetSalvarPreferenciasGrid(true);
    _gridControleContainer.CarregarGrid();
}

function loadControleContainer() {
    loadPesquisaControleColeta();
    loadGridControleContainer();
    loadJustificativaColeta();
    loadMovimentarContainer();
    loadEditarColetaContainer();

    new BuscarFilial(_pesquisaControleContainer.FilialAtual);
    new BuscarTiposContainer(_pesquisaControleContainer.TipoContainer);

    new BuscarClientes(_pesquisaControleContainer.LocalEsperaVazio);
    new BuscarClientes(_pesquisaControleContainer.LocalColeta);
    new BuscarClientes(_pesquisaControleContainer.LocalAtual);
    new BuscarClientes(_editarColetaContainer.LocalColeta);
    new BuscarContainers(_editarColetaContainer.Container, null, null, true);

    new BuscarJustificativaContainer(_justificativaColetaContainer.Justificativa, null, null, _justificativaColetaContainer.StatusContainer);
}

function visualizarMapaClick(containerSelecionado) {
    $(".title-carga-codigo-embarcador").html(containerSelecionado.CargaEmbarcador);

    ExibirModalMapa();
    loadMapa();
    carregarDadosMapa(containerSelecionado);

    var configuracoesExportacao = { url: "Monitoramento/ExportarParadas?codigo=" + containerSelecionado.Codigo, titulo: "ParadasCarga" };
    _gridMapaParadas = new GridView("grid-mapa-paradas", "Monitoramento/ObterParadas?codigo=" + containerSelecionado.Codigo, null, null, null, 10, null, true, null, null, null, true, configuracoesExportacao, null, true, null, false);
    _gridMapaParadas.CarregarGrid();
}

function ExibirModalMapa() {
    Global.abrirModal('divModalMapa');
    $("#divModalMapa").one('hidden.bs.modal', function () {
        _mapaMonitoramento.direction.limparMapa();
    });
}

function loadMapa() {
    if (!_mapaMonitoramento) {
        var opcoesmapa = new OpcoesMapa(false, false);
        _mapaMonitoramento = new MapaGoogle("map", false, opcoesmapa);
    }
}

function carregarDadosMapa(selecionado) {
    _mapaMonitoramento.clear();
    executarReST("Monitoramento/ObterDadosMapa", {
        Carga: selecionado.Carga
    }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                setTimeout(function () {
                    TrackingDesenharInformacoesMapa(_mapaMonitoramento, arg.Data);
                    TrackingCriarMarkerVeiculo(_mapaMonitoramento, arg.Data.Veiculo, false, 0)
                }, 500);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}


function visualizarHistoricosClick(row) {
    $(".title-numero-container").html(row.NumeroContainer);
    ExibirModalHistorico();
    var configuracoesExportacao = { url: "ControleContainer/ExportarHistoricoColetaContainer?codigo=" + row.Codigo, titulo: "HistóricoContainer" };

    _gridHistoricos = new GridView("grid-historicos", "ControleContainer/ObterHistoricoColetaContainer?codigo=" + row.Codigo, null, null, null, 10, null, true, null, null, 10000, true, configuracoesExportacao, null, true, null, false);
    _gridHistoricos.CarregarGrid();
}

function ExibirModalHistorico() {
    Global.abrirModal('divModalHistoricos');
}

function movimentarContainerClick(row) {
    console.log(row.CodigoTipoContainer);

    _movimentarContainer.Codigo.val(row.Codigo);
    _movimentarContainer.TipoContainer.val(row.TipoContainer);
    _movimentarContainer.TipoContainer.codEntity(row.CodigoTipoContainer);

    _statusMovimentacaoContainerAnterior = row.StatusContainer;

    Global.abrirModal('divModalMovimentarContainer');

    $("#divModalMovimentarContainer").one("hidden.bs.modal", function () {
        LimparCampos(_movimentarContainer);
    });
}


function alterarDadosClick(row) {
    console.log(row);
    _editarColetaContainer.Codigo.val(row.Codigo);
    _editarColetaContainer.Carga.val("Carga: " + row.CargaEmbarcador);

    executarReST("ControleContainer/BuscarPorCodigo", { Codigo: row.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_editarColetaContainer, retorno);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });

    $("#divModalColetaContainer")
        .modal("show")
        .one("hidden.bs.modal", function () {
            LimparCampos(_editarColetaContainer);
        });
}

function atualizarColetaContainerClick() {
    if (ValidarCamposObrigatorios(_editarColetaContainer)) {
        exibirConfirmacao("Confirmação", "Você realmente deseja alterar as informações da coleta de Container?", function () {
            Salvar(_editarColetaContainer, "ControleContainer/AtualizarColetaContainer", function (retorno) {
                if (retorno.Success) {

                    if (retorno.Data != null) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
                        CancelarAlterarColetaContainerClick();
                        _gridControleContainer.CarregarGrid();
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.atencao, retorno.Msg);
                    }

                } else exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    }
}

function CancelarAlterarColetaContainerClick() {
    Global.fecharModal("divModalColetaContainer");
}

function obterVisibilidadeOpcaoMovimentarContainer() {
    return VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ControleContainer_PermiteMovimentarContainer, _PermissoesPersonalizadasControleContainer);
}

/* INÍCIO REGIÃO DAS JUSTIFICATIVAS*/

function ResetarModalJustificativa() {
    Global.fecharModal("divModalJustificativa");
    limparCamposJustificativa();
}
function ConfirmarAlterarJustificativaClick() {
    if (ValidarCamposObrigatorios(_justificativaColetaContainer)) {
        exibirConfirmacao("Confirmação", "Você realmente deseja alterar a Justificativa da coleta de Container?", function () {
            Salvar(_justificativaColetaContainer, "ControleContainer/AlterarJustificativaColetaContainer", function (retorno) {
                if (retorno.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
                    ResetarModalJustificativa();
                    _gridControleContainer.CarregarGrid();
                } else exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    }
}

function visualizarJustificativaClick(row) {
    console.log(row);
    executarReST("ControleContainer/ObterMultiplasEtapasDisponiveis", { CodigoColetaContainer: row.Codigo }, function (retorno) {
        if (retorno.Success && retorno.Data) {
            var statusContainers = [];

            limparCamposJustificativa();

            Global.abrirModal('divModalJustificativa');

            _justificativaColetaContainer.Codigo.val(row.Codigo);
            _justificativaColetaContainer.Justificativa.val(row.Justificativa);

            var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarJustificativa, tamanho: "10", icone: "" };
            var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

            _gridHistoricosJustificativas = new GridView("grid-historicos-justificativas", "ControleContainer/ObterColetaContainerJustificativa", _justificativaColetaContainer, menuOpcoes, null, 10, null, true, null, null, 10000, true, null, null, true, null, false);
            _gridHistoricosJustificativas.CarregarGrid();
            for (var i = 0; i < retorno.Data.length; i++) {
                statusContainers.push({
                    text: EnumStatusColetaContainer.obterDescricao(retorno.Data[i]),
                    value: retorno.Data[i]
                });
            }

            _justificativaColetaContainer.StatusContainer.options(statusContainers);
        }
    });
}

function editarJustificativa(justificativaGrid) {
    LimparCampos(_justificativaColetaContainer);
    _justificativaColetaContainer.Codigo.val(justificativaGrid.Codigo);
    BuscarPorCodigo(_justificativaColetaContainer, "ControleContainer/BuscarJustificativaPorCodigo", function (arg) {
        var statusContainers = [];

        statusContainers.push({
            text: EnumStatusColetaContainer.obterDescricao(arg.Data.SituacaoContainer),
            value: arg.Data.SituacaoContainer
        });
        _justificativaColetaContainer.StatusContainer.options(statusContainers);

        _justificativaColetaContainer.Atualizar.visible(true);
        _justificativaColetaContainer.Cancelar.visible(true);
        _justificativaColetaContainer.Excluir.visible(true);
        _justificativaColetaContainer.Confirmar.visible(false);
    }, null);
}

function excluirJustificativaClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Justificativa?", function () {
        ExcluirPorCodigo(_justificativaColetaContainer, "ControleContainer/ExcluirJustificativaPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    ResetarModalJustificativa();
                    _gridControleContainer.CarregarGrid();
                    limparCamposJustificativa();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function atualizarJustificativaClick(e, sender) {
    Salvar(_justificativaColetaContainer, "ControleContainer/AtualizarJustificativaPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                ResetarModalJustificativa();
                _gridControleContainer.CarregarGrid();
                limparCamposJustificativa();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function limparCamposJustificativa() {
    _justificativaColetaContainer.Atualizar.visible(false);
    _justificativaColetaContainer.Cancelar.visible(false);
    _justificativaColetaContainer.Excluir.visible(false);
    _justificativaColetaContainer.Confirmar.visible(true);
    LimparCampos(_justificativaColetaContainer);
}

function cancelarJustificativaClick(e) {
    limparCamposJustificativa();
}

/* FIM REGIÃO DAS JUSTIFICATIVAS*/