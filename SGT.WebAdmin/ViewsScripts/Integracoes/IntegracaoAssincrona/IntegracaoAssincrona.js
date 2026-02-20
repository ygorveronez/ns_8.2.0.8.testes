/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Globais.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumCodigoFiltroPesquisa.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumStatusTarefa.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumTipoRequest.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumTipoEtapaTarefa.js" />

var _pesquisaIntegracaoAssincrona;
var _gridIntegracaoAssincrona;

function inicializarIntegracaoAssincrona() {
    _pesquisaIntegracaoAssincrona = new PesquisaIntegracaoAssincrona();
    KoBindings(_pesquisaIntegracaoAssincrona, "knockoutPesquisaIntegracaoAssincrona", false, _pesquisaIntegracaoAssincrona.Pesquisar.id);

    carregarGridIntegracaoAssincrona();
    carregarDetalhesIngradoraIntegracao();
    carregarIngradoraIntegracao();
}

var PesquisaIntegracaoAssincrona = function () {
    this.NumeroPedido = PropertyEntity({ text: "Número Pedido:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroCarregamento = PropertyEntity({ text: "Nº Carregamento/Carga:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroCarga = PropertyEntity({ text: "Nº Carga:", val: ko.observable(""), def: "", getType: typesKnockout.int, visible: ko.observable(false) });
    this.DataInicialIntegracao = PropertyEntity({ text: "Dt. Inicial", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), visible: ko.observable(true) });
    this.DataFinalIntegracao = PropertyEntity({ text: "Dt. Final", getType: typesKnockout.dateTime, val: ko.observable(""), visible: ko.observable(true) });
    this.DataInicialIntegracao.dateRangeLimit = this.DataFinalIntegracao;
    this.DataFinalIntegracao.dateRangeInit = this.DataInicialIntegracao;

    this.StatusTarefa = PropertyEntity({ text: "Status", val: ko.observable(EnumStatusTarefa.Todos), options: EnumStatusTarefa.obterOpcoesPesquisa(), def: EnumStatusTarefa.Todos, visible: ko.observable(true) });
    this.TipoRequest = PropertyEntity({ text: "Tipo Request", val: ko.observable(EnumTipoRequest.Todos), options: EnumTipoRequest.obterOpcoesPesquisa(), def: EnumTipoRequest.Todos, visible: ko.observable(true) });
    this.TipoEtapaAtual = PropertyEntity({ text: "Etapa Atual", val: ko.observable(EnumTipoEtapaTarefa.Todos), options: EnumTipoEtapaTarefa.obterOpcoesPesquisa(), def: EnumTipoEtapaTarefa.Todos, visible: ko.observable(true) });
    this.JobId = PropertyEntity({ text: "Job ID:", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.ModeloFiltrosPesquisa = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloDeFiltroDePesquisa, idBtnSearch: guid(),
        tipoFiltroPesquisa: EnumCodigoFiltroPesquisa.IntegracaoAssincrona,
    });
    this.ConfiguracaoModeloFiltroPesquisa = PropertyEntity({ eventClick: function (e) { abrirConfiguracaoModeloFiltroPesquisa(EnumCodigoFiltroPesquisa.IntegracaoAssincrona, _gridIntegracaoAssincrona) }, type: types.event, text: Localization.Resources.Gerais.Geral.SalvarFiltro, visible: ko.observable(true) });
    this.CarregarFiltrosPesquisa = PropertyEntity({
        eventClick: function (e) {
            abrirBuscaFiltrosManual(e);
        }, type: types.event, text: "Carregar Filtro", idFade: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
    this.Pesquisar = PropertyEntity({
        type: types.event, eventClick: function (e) {
            _gridIntegracaoAssincrona.CarregarGrid();
            fecharFiltros();
        }, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

// #endregion Knouts

function carregarGridIntegracaoAssincrona() {
    var configuracaoExportacao = {
        url: "IntegracaoAssincrona/PesquisaExportar",
        titulo: "Retorno de Integrações Assíncronas"
    };

    var detalhesIntegracao = {
        descricao: "Detalhes",
        id: guid(),
        evento: "onclick",
        metodo: abrirDetalhesIntegracaoClick,
        tamanho: "10",
        icone: ""
    };

    var downloadRequisicaoTarefa = {
        descricao: "Download Requisição",
        id: guid(),
        evento: "onclick",
        metodo: downloadRequisicaoIntegracaoClick,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [detalhesIntegracao, downloadRequisicaoTarefa] };

    _gridIntegracaoAssincrona = new GridView("grid-gestao-carga-integracao-assincrona", "IntegracaoAssincrona/Pesquisa", _pesquisaIntegracaoAssincrona, menuOpcoes, null, 25, null, null, null, null, null, null, configuracaoExportacao);
    _gridIntegracaoAssincrona.SetSalvarPreferenciasGrid(true);
    _gridIntegracaoAssincrona.CarregarGrid();
}

function abrirBuscaFiltrosManual(e) {
    var buscaFiltros = new BuscarModeloFiltroPesquisa(e.ModeloFiltrosPesquisa, retornoBuscarFiltrosManual, EnumCodigoFiltroPesquisa.IntegracaoAssincrona);

    buscaFiltros.AbrirBusca();
}

function retornoBuscarFiltrosManual(retorno) {
    if (retorno.Codigo == 0) return;

    e.ModeloFiltrosPesquisa.codEntity(retorno.Codigo);
    e.ModeloFiltrosPesquisa.val(retorno.ModeloDescricao);

    PreencherJsonFiltroPesquisa(_gridIntegracaoAssincrona, retorno.Dados);
}

function abrirDetalhesIntegracaoClick(integracao) {
    const dados = { Codigo: integracao.Codigo };

    executarReST("IntegracaoAssincrona/BuscarPorCodigo", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _integracaoIntegradora.Codigo.val(retorno.Data.Codigo);
                PreencherObjetoKnout(_detalhesIntegracaoIntegradora, retorno);

                Global.abrirModal('modalDetalhesIntegracao');
                $("#modalDetalhesIntegracao").one('shown.bs.modal', function () {
                    recarregarGridDetalhesIntegracoes();
                });

                $("#modalDetalhesIntegracao").one('hidden.bs.modal', function () {
                    LimparCampos(_detalhesIntegracaoIntegradora);
                    LimparCampos(_integracaoIntegradora);

                    Global.ResetarAba("modalDetalhesIntegracao");
                });

            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function downloadRequisicaoIntegracaoClick(integracao) {
    var dados = { Codigo: integracao.Codigo };

    executarDownload("IntegracaoAssincrona/DownloadArquivoRequisicaoIntegracao", dados);
}