/// <reference path="../../Enumeradores/EnumSituacaoValePedagio.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracao.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/TipoInfracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridValePedagio;
var _pesquisaValePedagio;
var _pesquisaHistoricoIntegracaoValePedagio;
var _modalHistoricoIntegracaoValePedagio;

var PesquisaValePedagio = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Código da carga:" });
    this.DataCargaInicial = PropertyEntity({ text: "Data carga inicial: ", getType: typesKnockout.dateTime, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataCargaFinal = PropertyEntity({ text: "Data carga final: ", getType: typesKnockout.dateTime, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataCargaInicial.dateRangeLimit = this.DataCargaFinal;
    this.DataCargaFinal.dateRangeInit = this.DataCargaInicial;
    this.SituacaoValePedagio = PropertyEntity({ text: "Situação VP:", val: ko.observable(EnumSituacaoValePedagio.Todas), options: EnumSituacaoValePedagio.obterOpcoesPesquisa(), def: EnumSituacaoValePedagio.Todas });
    this.NumeroValePedagio = PropertyEntity({ text: "Número VP" });
    this.TipoIntegracao = PropertyEntity({ text: "Tipo VP:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.SituacaoIntegracao = PropertyEntity({ text: "Situação da integração:", val: ko.observable(EnumSituacaoIntegracao.Todas), options: EnumSituacaoIntegracao.obterOpcoesPesquisa(), def: EnumSituacaoIntegracao.Todas });
    this.DataIntegracaoInicial = PropertyEntity({ text: "Data integração inicial: ", getType: typesKnockout.dateTime, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataIntegracaoFinal = PropertyEntity({ text: "Data integração final: ", getType: typesKnockout.dateTime, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataIntegracaoInicial.dateRangeLimit = this.DataIntegracaoFinal;
    this.DataIntegracaoFinal.dateRangeInit = this.DataIntegracaoInicial;
    this.NumeroParcialCarga = PropertyEntity({ text: "Número parcial da carga:", val: ko.observable(""), def: "" });
    this.Pesquisar = PropertyEntity({ text: "Pesquisar", type: types.event, idGrid: guid(), visible: ko.observable(true), eventClick: pesquisarValePedagio });
    this.ExibirFiltros = PropertyEntity({ text: "Filtros de Pesquisa", type: types.event, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true), eventClick: pesquisarExibirFiltros });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: ko.observable("Selecionar Todos"), visible: ko.observable(false) }); //PropertyEntity({ text: ko.observable("Selecionar Todos"), type: types.event, visible: ko.observable(false), eventClick: selecionarTodosClick });
    this.RecomprarSelecionados = PropertyEntity({ text: "Recomprar Selecionados", type: types.event, idGrid: guid(), visible: ko.observable(false), eventClick: RecomprarSelecionadosClick });
    
    this.SituacaoIntegracao.val.subscribe(function (novoValor) {
        if (novoValor == EnumSituacaoIntegracao.ProblemaIntegracao) {
            _pesquisaValePedagio.RecomprarSelecionados.visible(true);
            _pesquisaValePedagio.SelecionarTodos.visible(true);
            buscarValePedagio(true);
        } else {
            _pesquisaValePedagio.RecomprarSelecionados.visible(false);
            _pesquisaValePedagio.SelecionarTodos.visible(false);
            if (_pesquisaValePedagio.SelecionarTodos.val())
                _pesquisaValePedagio.SelecionarTodos.val(false);
            buscarValePedagio(false);
        }
    });
};

var PesquisaHistoricoIntegracaoValePedagio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

//*******EVENTOS*******

function loadValePedagio() {
    _pesquisaValePedagio = new PesquisaValePedagio();
    KoBindings(_pesquisaValePedagio, "knockoutPesquisaValePedagio", false, _pesquisaValePedagio.Pesquisar.id);

    new BuscarTipoIntegracao(_pesquisaValePedagio.TipoIntegracao, null, null, null, EnumGrupoTipoIntegracao.ValePedagio, false);

    _modalHistoricoIntegracaoValePedagio = new bootstrap.Modal(document.getElementById("divModalHistoricoIntegracaoValePedagio"), { backdrop: true, keyboard: true });

    buscarValePedagio(false);
}

function pesquisarValePedagio(e, sender) {
    _gridValePedagio.CarregarGrid();
}

function pesquisarExibirFiltros(e, sender) {
    if (e.ExibirFiltros.visibleFade() === true) {
        e.ExibirFiltros.visibleFade(false);
    } else {
        e.ExibirFiltros.visibleFade(true);
    }
}

function gridValePedagioCallbackRow(nRow, aData) {
    if (permiteCancelar(aData))
        $(nRow).css("background-color", "#DFF0D8");
    else if
        (aData.SituacaoValePedagio == EnumSituacaoValePedagio.EmCancelamento) $(nRow).css("background-color", "#FFF0F0");
}

function menuGridValePedagioImprimirClick(row) {
    executarDownload("CargaIntegracaoValePedagio/ImpressaoValePedagio", { Codigo: row.Codigo });
}

function menuGridValePedagioHistoricoClick(row) {
    _pesquisaHistoricoIntegracaoValePedagio = new PesquisaHistoricoIntegracaoValePedagio();
    _pesquisaHistoricoIntegracaoValePedagio.Codigo.val(row.Codigo);
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [{ descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: menuGridHistoricoIntegracaoValePedagioDownloadClick, tamanho: "10", icone: "" }] };
    _gridHistoricoIntegracaoValePedagio = new GridView("tblHistoricoIntegracaoValePedagio", "CargaIntegracaoValePedagio/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoValePedagio, menuOpcoes, { column: 1, dir: orderDir.desc }, 10);
    _gridHistoricoIntegracaoValePedagio.CarregarGrid();
    _modalHistoricoIntegracaoValePedagio.show();
}

function menuGridHistoricoIntegracaoValePedagioDownloadClick(row) {
    executarDownload("CargaIntegracaoValePedagio/DownloadArquivosHistoricoIntegracao", { Codigo: row.Codigo });
}

function menuGridValePedagioAuditoriaClick(row) {
    var data = { Codigo: row.Codigo };
    var closureAuditoria = OpcaoAuditoria("CargaIntegracaoValePedagio", null, row.Codigo);
    closureAuditoria(data);
}

function menuGridValePedagioCancelarClick(row) {
    if (permiteCancelar(row)) {
        exibirConfirmacao("Confirmação", "Você realmente deseja cancelar o VP  \"" + row.NumeroValePedagio + "\" da carga \"" + row.CodigoCargaEmbarcador + "\"?", function () {
            executarReST("/ValePedagio/Cancelar", { codigo: row.Codigo }, function (arg) {
                if (arg.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Vale pedágio \"" + row.NumeroValePedagio + "\" da carga \"" + row.CodigoCargaEmbarcador + "\" agendado para ser cancelado.");
                    _gridValePedagio.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        });
    } else {
        exibirMensagem(tipoMensagem.aviso, "Não permitido", "Apenas VPs comprados e integrados podem ser cancelados.");
    }
}

function menuGridValePedagioCancelarVisibilidade(row) {
    return permiteCancelar(row);
}

function permiteCancelar(row) {
    return (row.SituacaoValePedagio == EnumSituacaoValePedagio.Comprada || row.SituacaoValePedagio == EnumSituacaoValePedagio.Confirmada) && row.SituacaoIntegracao == EnumSituacaoIntegracao.Integrado;
}

function RecomprarSelecionadosClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente Recomprar Selecionados?", function () {
        executarReST("ValePedagio/RecompraSelecionadosValePedagio", { GridValePedagio: JSON.stringify(_gridValePedagio.ObterMultiplosSelecionados()) }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Recompra concluída com sucesso!");
                    _gridValePedagio.AtualizarRegistrosSelecionados([]);
                    _gridValePedagio.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function buscarValePedagio(ativarMultiplaEscolha) {
    var opcaoImprimir = { descricao: "Imprimir", id: guid(), evento: "onclick", metodo: menuGridValePedagioImprimirClick };
    var opcaoHistorico = { descricao: "Histórico", id: guid(), evento: "onclick", metodo: menuGridValePedagioHistoricoClick };
    var opcaoAuditoria = { descricao: "Auditoria", id: guid(), evento: "onclick", metodo: menuGridValePedagioAuditoriaClick };
    var opcaoCancelar = { descricao: "Cancelar", id: guid(), evento: "onclick", metodo: menuGridValePedagioCancelarClick, visibilidade: menuGridValePedagioCancelarVisibilidade };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoImprimir, opcaoHistorico, opcaoAuditoria, opcaoCancelar], tamanho: 5 };

    var multiplaescolha = null;

    if (ativarMultiplaEscolha) {
        multiplaescolha = {
            basicGrid: null,
            callbackSelecionado: null,
            callbackNaoSelecionado: null,
            eventos: function () { },
            selecionados: new Array(),
            naoSelecionados: new Array(),
            somenteLeitura: false,
            SelecionarTodosKnout: _pesquisaValePedagio.SelecionarTodos
        };
    }

    _gridValePedagio = new GridView(_pesquisaValePedagio.Pesquisar.idGrid, "ValePedagio/Pesquisa", _pesquisaValePedagio, menuOpcoes, null, 50, null, null, null, multiplaescolha, null, null, null, null, null, gridValePedagioCallbackRow);
    _gridValePedagio.CarregarGrid();
}