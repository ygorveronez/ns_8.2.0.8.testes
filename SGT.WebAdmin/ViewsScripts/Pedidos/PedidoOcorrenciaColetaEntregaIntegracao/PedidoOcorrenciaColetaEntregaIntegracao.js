/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Consultas/TipoIntegracao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pesquisaPedidoOcorrenciaColetaEntregaIntegracao;
var _pesquisaPedidoOcorrenciaColetaEntregaIntegracaoHistorico;
var _gridPedidoOcorrenciaColetaEntregaIntegracao;
var _gridPedidoOcorrenciaColetaEntregaIntegracaoHistorico;
var _detalhePedidoOcorrenciaColetaEntregaIntegracao;

var PesquisaPedidoOcorrenciaColetaEntregaIntegracao = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Carga: " });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Pedido: " });
    this.TipoDeOcorrencia = PropertyEntity({ text: ko.observable("Tipo de ocorrência:"), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: ko.observable("Transportador:"), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ text: "Tomador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoIntegracao = PropertyEntity({ text: "Tipo de integração:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    var dataPadrao = Global.DataAtual();
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", val: ko.observable(dataPadrao), def: dataPadrao, getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", val: ko.observable(dataPadrao), def: dataPadrao, getType: typesKnockout.date });
    this.SituacaoIntegracao = PropertyEntity({ text: "Situação da integração:", val: ko.observable(EnumSituacaoIntegracao.Todas), def: EnumSituacaoIntegracao.Todas, options: EnumSituacaoIntegracao.obterOpcoesPesquisa() });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({ text: "Pesquisar", type: types.event, eventClick: pesquisarPedidoOcorrenciaColetaEntregaIntegracaoClick, idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false) });
    this.ReenviarIntegracoesComFalha = PropertyEntity({ text: "Reenviar Integrações com Falha", type: types.event, eventClick: reenviarIntegracoesComFalhaClick, idGrid: guid(), visible: ko.observable(false), visibleFade: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ text: "Filtros de Pesquisa", type: types.event, eventClick: exibirFiltrosPedidoOcorrenciaColetaEntregaIntegracaoClick, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });
}

var PesquisaPedidoOcorrenciaColetaEntregaIntegracaoHistorico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var DetalhesPedidoOcorrenciaColetaEntregaIntegracao = function () {
    this.Codigo = PropertyEntity({ text: "Código: ", getType: typesKnockout.string, visible: ko.observable(true) });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Carga: ", getType: typesKnockout.string, visible: ko.observable(true) });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Pedido: ", getType: typesKnockout.string, visible: ko.observable(true) });
    this.Origem = PropertyEntity({ text: "Origem: ", getType: typesKnockout.string, visible: ko.observable(true) });
    this.Destino = PropertyEntity({ text: "Destino: ", getType: typesKnockout.string, visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ text: "Remetente: ", getType: typesKnockout.string, visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ text: "Tomador: ", getType: typesKnockout.string, visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ text: "Destinatario: ", visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: "Transportador: ", getType: typesKnockout.string, visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "Veículo: ", getType: typesKnockout.string, visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ text: "Motorista: ", getType: typesKnockout.string, visible: ko.observable(true) });
    this.TipoOcorrencia = PropertyEntity({ text: "Tipo da ocorrência: ", getType: typesKnockout.string, visible: ko.observable(true) });
    this.DataOcorrencia = PropertyEntity({ text: "Data da ocorrência: ", getType: typesKnockout.string, visible: ko.observable(true) });
    this.GridNotasFiscais = PropertyEntity({ def: [], val: ko.observable([]), id: guid() });
}

function loadGridPedidoOcorrenciaColetaEntregaIntegracao() {
    var detalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: detalhesPedidoOcorrenciaColetaEntregaIntegracaoClick };
    var historico = { descricao: "Histórico de Integração", id: guid(), evento: "onclick", metodo: exibirHistoricoPedidoOcorrenciaColetaEntregaIntegracaoClick };
    var downloadEDI = { descricao: "Download EDI", id: guid(), evento: "onclick", metodo: downloadEdiPedidoOcorrenciaColetaEntregaIntegracaoClick, visibilidade: VisibilidadeDownloadEDI };
    var auditoria = { descricao: "Auditoria", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("PedidoOcorrenciaColetaEntregaIntegracao"), visibilidade: VisibilidadeOpcaoAuditoria };
    var reenviar = { descricao: "Reenviar", id: guid(), evento: "onclick", metodo: reenviarPedidoOcorrenciaColetaEntregaIntegracaoClick, visibilidade: reenviarPedidoOcorrenciaColetaEntregaIntegracaoVisible };
    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 5, opcoes: [detalhes, downloadEDI, historico, auditoria, reenviar] };
    var configuracoesExportacao = { url: "PedidoOcorrenciaColetaEntregaIntegracao/ExportarPesquisa", titulo: "PedidoOcorrenciaColetaEntregaIntegracao" };
    _gridPedidoOcorrenciaColetaEntregaIntegracao = new GridView(_pesquisaPedidoOcorrenciaColetaEntregaIntegracao.Pesquisar.idGrid, "PedidoOcorrenciaColetaEntregaIntegracao/Pesquisa", _pesquisaPedidoOcorrenciaColetaEntregaIntegracao, menuOpcoes, null, 10, null, true, false, undefined, 100, undefined, configuracoesExportacao);
    _gridPedidoOcorrenciaColetaEntregaIntegracao.CarregarGrid();
}

function loadGridPedidoOcorrenciaColetaEntregaIntegracaoHistorico() {
    var opcaoDownload = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: downloadArquivosPedidoOcorrenciaColetaEntregaIntegracaoHistoricoClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };
    _gridPedidoOcorrenciaColetaEntregaIntegracaoHistorico = new GridView("grid-pedido-ocorrencia-coleta-entrega-integracao-historico", "PedidoOcorrenciaColetaEntregaIntegracao/PesquisaIntegracaoHistorico", _pesquisaPedidoOcorrenciaColetaEntregaIntegracaoHistorico, menuOpcoes, { column: 1, dir: orderDir.desc });
}

function loadPedidoOcorrenciaColetaEntregaIntegracao() {
    _pesquisaPedidoOcorrenciaColetaEntregaIntegracao = new PesquisaPedidoOcorrenciaColetaEntregaIntegracao();
    KoBindings(_pesquisaPedidoOcorrenciaColetaEntregaIntegracao, "knockoutPesquisaPedidoOcorrenciaColetaEntregaIntegracao", false, _pesquisaPedidoOcorrenciaColetaEntregaIntegracao.Pesquisar.id);

    new BuscarTipoOcorrencia(_pesquisaPedidoOcorrenciaColetaEntregaIntegracao.TipoDeOcorrencia, null, null, null, null, null, null, null, true);
    new BuscarTransportadores(_pesquisaPedidoOcorrenciaColetaEntregaIntegracao.Transportador, null, null, true);
    new BuscarClientes(_pesquisaPedidoOcorrenciaColetaEntregaIntegracao.Tomador);
    new BuscarTipoIntegracao(_pesquisaPedidoOcorrenciaColetaEntregaIntegracao.TipoIntegracao, null, null, null, EnumGrupoTipoIntegracao.OcorrenciaEntrega, false);

    _pesquisaPedidoOcorrenciaColetaEntregaIntegracaoHistorico = new PesquisaPedidoOcorrenciaColetaEntregaIntegracaoHistorico();
    loadGridPedidoOcorrenciaColetaEntregaIntegracao();
    loadGridPedidoOcorrenciaColetaEntregaIntegracaoHistorico();

    _detalhePedidoOcorrenciaColetaEntregaIntegracao = new DetalhesPedidoOcorrenciaColetaEntregaIntegracao();
    KoBindings(_detalhePedidoOcorrenciaColetaEntregaIntegracao, "knockoutPedidoOcorrenciaColetaEntregaIntegracaoDetalhes");
}

function exibirHistoricoPedidoOcorrenciaColetaEntregaIntegracaoClick(row) {
    _pesquisaPedidoOcorrenciaColetaEntregaIntegracaoHistorico.Codigo.val(row.Codigo);
    _gridPedidoOcorrenciaColetaEntregaIntegracaoHistorico.CarregarGrid();
    Global.abrirModal("divModalPedidoOcorrenciaColetaEntregaIntegracaoHistorico");
}

function downloadEdiPedidoOcorrenciaColetaEntregaIntegracaoClick(row) {
    executarDownload("PedidoOcorrenciaColetaEntregaIntegracao/DownloadEdiPedidoOcorrenciaColetaEntregaIntegracao", { Codigo: row.Codigo });
}

function downloadArquivosPedidoOcorrenciaColetaEntregaIntegracaoHistoricoClick(row) {
    executarDownload("PedidoOcorrenciaColetaEntregaIntegracao/DownloadArquivosIntegracaoHistorico", { Codigo: row.Codigo });
}

function pesquisarPedidoOcorrenciaColetaEntregaIntegracaoClick() {
    _pesquisaPedidoOcorrenciaColetaEntregaIntegracao.ReenviarIntegracoesComFalha.visible(_pesquisaPedidoOcorrenciaColetaEntregaIntegracao.SituacaoIntegracao.val() == EnumSituacaoIntegracao.ProblemaIntegracao);

    _gridPedidoOcorrenciaColetaEntregaIntegracao.CarregarGrid();
}

function reenviarIntegracoesComFalhaClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar as integrações com falha?", function () {
        executarReST("PedidoOcorrenciaColetaEntregaIntegracao/ReenviarTodasIntegracoesComFalha", {
            DataInicial: _pesquisaPedidoOcorrenciaColetaEntregaIntegracao.DataInicial.val(),
            DataFinal: _pesquisaPedidoOcorrenciaColetaEntregaIntegracao.DataFinal.val()
        }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvios realizados com sucesso.");
                    _gridPedidoOcorrenciaColetaEntregaIntegracao.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso!", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
            }
        });
    });
}

function exibirFiltrosPedidoOcorrenciaColetaEntregaIntegracaoClick() {
    _pesquisaPedidoOcorrenciaColetaEntregaIntegracao.ExibirFiltros.visibleFade(!_pesquisaPedidoOcorrenciaColetaEntregaIntegracao.ExibirFiltros.visibleFade());
}

function detalhesPedidoOcorrenciaColetaEntregaIntegracaoClick(row) {
    executarReST("PedidoOcorrenciaColetaEntregaIntegracao/ObterDetalhes", { Codigo: row.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_detalhePedidoOcorrenciaColetaEntregaIntegracao, retorno);
                Global.abrirModal("divModalPedidoOcorrenciaColetaEntregaIntegracaoDetalhes");
                new BasicDataTable(_detalhePedidoOcorrenciaColetaEntregaIntegracao.GridNotasFiscais.id, _detalhePedidoOcorrenciaColetaEntregaIntegracao.GridNotasFiscais.val().header, null, null, null, 100).CarregarGrid(_detalhePedidoOcorrenciaColetaEntregaIntegracao.GridNotasFiscais.val().data);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso!", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
}

function auditoriaPedidoOcorrenciaColetaEntregaIntegracaoClick(row) {
    OpcaoAuditoria("PedidoOcorrenciaColetaEntregaIntegracao");
}

function reenviarPedidoOcorrenciaColetaEntregaIntegracaoVisible(row) {
    return (row.CodigoSituacaoIntegracao == EnumSituacaoIntegracao.ProblemaIntegracao);
}

function VisibilidadeDownloadEDI(row) {
    if (row.LayoutEDI > 0)
        return true;
    else
        return false;
}

function reenviarPedidoOcorrenciaColetaEntregaIntegracaoClick(row) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar a integração?", function () {
        executarReST("PedidoOcorrenciaColetaEntregaIntegracao/Reenviar", { Codigo: row.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio executado com sucesso.");
                    _gridPedidoOcorrenciaColetaEntregaIntegracao.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso!", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
            }
        });
    });
}