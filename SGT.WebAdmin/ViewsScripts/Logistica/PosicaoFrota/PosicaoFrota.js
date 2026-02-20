/// <reference path="../../Enumeradores/EnumSituacaoPosicaoFrota.js" />
/// <reference path="../../Enumeradores/EnumTipoModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/CategoriaPessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/ContratoFreteTransportador.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/TipoContratoFrete.js" />
/// <reference path="../../Transportadores/Transportador/Transportador.js" />
/// <reference path="../../../js/Global/MapaGoogle.js" />
/// <reference path="../../../js/Global/Mapa.js"/>
/// <reference path="../../../js/Global/MapaDraw.js"/>
/// <reference path="../../../js/Global/MapaGoogleSearch.js"/>
/// <reference path="../../Logistica/Tracking/Tracking.lib.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */
var _gridPosicaoFrota;
var _pesquisaPosicaoFrota;
var _cabecalhoPosicaoFrota;
var _pesquisaModoTv;
var _playerPosicaoFrota;
var _timerInterval = null;
var _totalSeconds = 300;
var _countSeconds = _totalSeconds;
var _map = null;
var _dadosCarrossel = null;
var intervaloAtualizacao = null;

/*
 * Declaração de Objetos Globais do Arquivo referente aos veículos agrupados no mapa
 */
var _novoMapa = null;
var _markers = null;
var _markersFilial = null;
var _marcadoresFiliais = [];
var _novoMapaMT = null;
var _markersMT = null;
var _markersFilialMT = null;
var _areasFiliais = [];
var _subAreasFiliais = [];
/*
 * Declaração das Classes
 */

var PesquisaPosicaoFrota = function () {
    this.Veiculo = PropertyEntity({ text: Localization.Resources.Logistica.PosicaoFrota.Veiculos.getFieldDescription(), type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoModeloVeicular = PropertyEntity({ text: Localization.Resources.Logistica.PosicaoFrota.TipoVeiculoModeloVeicular.getFieldDescription(), val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoModeloVeicularCarga.obterOpcoesPesquisaSemModelo(), cssClass: ko.observable("") });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.PosicaoFrota.Transportador.getFieldDescription(), issue: 69, idBtnSearch: guid(), visible: ko.observable(true), placeholder: Localization.Resources.Logistica.PosicaoFrota.Transportador });
    this.CategoriaPessoa = PropertyEntity({ text: Localization.Resources.Logistica.PosicaoFrota.Categoria.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(false) });
    this.Cliente = PropertyEntity({ text: Localization.Resources.Logistica.PosicaoFrota.Cliente.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(false) });
    this.DataInicio = PropertyEntity({ text: "Data posição atual Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual() });
    this.DataFim = PropertyEntity({ text: "Data posição atual Final: ", getType: typesKnockout.date, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual() });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.Situacao = PropertyEntity({ text: Localization.Resources.Logistica.PosicaoFrota.EmViagem.getFieldDescription().replace(":", ""), val: ko.observable(EnumSituacaoPosicaoFrota.obterOpcoes()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoPosicaoFrota.obterOpcoes(), cssClass: ko.observable(""), placeholder: Localization.Resources.Logistica.PosicaoFrota.Situacao });
    this.GrupoStatusViagem = PropertyEntity({ text: Localization.Resources.Logistica.PosicaoFrota.EtapaMonitoramento.getFieldDescription(), val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable([]), visible: ko.observable(true), placeholder: Localization.Resources.Logistica.PosicaoFrota.StatusdaViagem });
    this.GrupoTipoOperacao = PropertyEntity({ text: Localization.Resources.Logistica.PosicaoFrota.TipoOperacao.getFieldDescription(), val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable([]), visible: ko.observable(true), placeholder: Localization.Resources.Logistica.PosicaoFrota.TipoOperacao.getFieldDescription() });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Logistica.PosicaoFrota.Carga.getFieldDescription(), col: 12, placeholder: Localization.Resources.Logistica.PosicaoFrota.Carga });
    this.Filial = PropertyEntity({ text: Localization.Resources.Logistica.PosicaoFrota.Filial.getFieldDescription(), type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), placeholder: Localization.Resources.Logistica.PosicaoFrota.Filial });
    this.EmAlvo = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.ApenasVeiculosEmAlvo });
    this.VeiculosComMonitoramento = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.ApenasVeiculosComMonitoramentoAndamento });
    this.VeiculosComContratoDeFrete = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.ApenasVeiculosQuePossuemContratoFrete, visible: ko.observable(true) });
    this.ClientesComVeiculoEmAlvo = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.ClientesComVeiculosAlvo, visible: ko.observable(true) });
    this.ClientesAlvosEstrategicos = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.ClientesAlvosEstrategicos, visible: ko.observable(true) });
    this.BuscarPreCarga = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.MostrarDadosPreViagem, visible: ko.observable(true) });
    this.SituacaoVeiculo = PropertyEntity({ text: Localization.Resources.Logistica.PosicaoFrota.SituacaoVeiculo.getFieldDescription(), val: ko.observable(EnumSituacaoVeiculo.obterTodos()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoVeiculo.obterOpcoes(), visible: ko.observable(false) });
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.Logistica.PosicaoFrota.TipoOperacao.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.StatusViagemControleEntrega = PropertyEntity({ text: Localization.Resources.Logistica.PosicaoFrota.SituacaoControleEntrega.getFieldDescription(), val: ko.observable(EnumStatusViagemControleEntrega.Todas), options: EnumStatusViagemControleEntrega.obterOpcoesPesquisa(), def: EnumStatusViagemControleEntrega.Todas });

    this.Remetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.PosicaoFrota.Remetente.getFieldDescription(), idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.PosicaoFrota.Destinatario.getFieldDescription(), idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.PosicaoFrota.Motorista.getFieldDescription(), idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.PosicaoFrota.CentroResultado.getFieldDescription(), idBtnSearch: guid() });
    this.FuncionarioResponsavel = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.PosicaoFrota.ResponsavelPeloVeiculo.getFieldDescription(), idBtnSearch: guid() });
    this.GrupoPessoas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.PosicaoFrota.GrupoPessoa.getFieldDescription(), idBtnSearch: guid() });
    this.Critico = PropertyEntity({ text: Localization.Resources.Logistica.PosicaoFrota.Monitoramentocritico.getFieldDescription(), val: ko.observable(true), getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(true) });
    this.FiltrarLocais = PropertyEntity({ text: Localization.Resources.Logistica.PosicaoFrota.FiltroLocais.getFieldDescription(), val: ko.observable(true), getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(true) });
    this.VeiculosDentroDoRaioFilial = PropertyEntity({ text: Localization.Resources.Logistica.PosicaoFrota.VeiculosDentroDoRaioFilial.getFieldDescription(), val: ko.observable(true), getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(true) });
    this.RaioFilial = PropertyEntity({ text: Localization.Resources.Logistica.PosicaoFrota.RaioFilial.getFieldDescription(), val: ko.observable(), getType: typesKnockout.int, enable: ko.observable(true), visible: ko.computed(() => this.VeiculosDentroDoRaioFilial.val()) });
    this.Locais = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.PosicaoFrota.Locais.getFieldDescription(), idBtnSearch: guid() });
    this.ExibirFiliaisEBases = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.ExibirFiliaisEBases, visible: ko.observable(true) });

    this.ContratoFrete = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.PosicaoFrota.ContratoFrete.getFieldDescription(), idBtnSearch: guid() });
    this.TipoContratoFrete = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.PosicaoFrota.TipoContratoFrete.getFieldDescription(), idBtnSearch: guid() });
    this.TecnologiaRastreador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.PosicaoFrota.Tecnologia.getFieldDescription(), idBtnSearch: guid() });

    this.RastreadorOnlineOffline = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(null) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            controlarExibicoesMapaEGrid(true, true);
            Global.fecharModal('divModalFiltrosPesquisa');
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Filtrar, idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false)
    });

    this.ExibirMapa = PropertyEntity({
        eventClick: function (e) {
            e.ExibirMapa.visibleFade(true);
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.ExibirResultado = PropertyEntity({
        eventClick: function (e) {
            e.ExibirResultado.visibleFade(!e.ExibirResultado.visibleFade());
        }, type: types.event, text: Localization.Resources.Logistica.PosicaoFrota.Resultado, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.ModeloFiltrosPesquisa = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloDeFiltroDePesquisa, idBtnSearch: guid(),
        tipoFiltroPesquisa: EnumCodigoFiltroPesquisa.PosicaoFrota,
    });

    this.ConfiguracaoModeloFiltroPesquisa = PropertyEntity({ eventClick: function (e) { abrirConfiguracaoModeloFiltroPesquisa(EnumCodigoFiltroPesquisa.PosicaoFrota, _pesquisaPosicaoFrota) }, type: types.event, text: Localization.Resources.Gerais.Geral.SalvarFiltro, visible: ko.observable(true) });

    this.CarregarFiltrosPesquisa = PropertyEntity({
        eventClick: function (e) {
            abrirBuscaFiltrosManual(e);
        }, type: types.event, text: "Carregar Filtro", idFade: guid(), visible: ko.observable(true)
    });

    this.LimparFiltros = PropertyEntity({
        eventClick: function (e) {
            LimparCampos(_pesquisaPosicaoFrota);
            $("#view-online").removeClass('active');
            $("#view-offline").removeClass('active');
            _pesquisaPosicaoFrota.DataInicio.val(null);
            _pesquisaPosicaoFrota.DataFim.val(null);
        }, type: types.event, text: Localization.Resources.Gerais.Geral.LimparFiltros, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var CabecalhoPosicaoFrota = function () {
    this.Grid = PropertyEntity({ type: types.event, idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            Global.abrirModal("divModalFiltrosPesquisa");
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
    this.MostrarMapa = PropertyEntity({
        eventClick: function (e) {
            controlarExibicoesMapaEGrid(true, false);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), text: "Mapa da Frota"
    });
    this.MostrarGrid = PropertyEntity({
        eventClick: function (e) {
            controlarExibicoesMapaEGrid(false, false);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), text: "Tabela da Frota"
    });
    this.MostrarMapaTabela = PropertyEntity({
        eventClick: function () {
            controlarExibicoesMapaEGrid('mix', false);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), text: "Mapa + Tabela da Frota"
    });
    this.InformacoesReboqueClick = PropertyEntity({
        eventClick: function (e) {
            LimparCampo(_pesquisaPosicaoFrota.GrupoStatusViagem);
            LimparCampo(_pesquisaPosicaoFrota.GrupoTipoOperacao);

            if ($('#view-disponivel').hasClass("active")) {
                LimparCampo(_pesquisaPosicaoFrota.SituacaoVeiculo);

                controlarExibicoesMapaEGrid(getMode(), false);
                $(".view-select-button.active").removeClass('active');
                return;
            } else {
                $(".view-select-button.active").removeClass('active');
                $('#view-disponivel').addClass('active');
            }

            _pesquisaPosicaoFrota.SituacaoVeiculo.val([EnumSituacaoVeiculo.Disponivel]);
            controlarExibicoesMapaEGrid(getMode(), false);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), text: "Tabela da Frota"
    });
    this.OnlineClick = PropertyEntity({
        eventClick: function (e) {

            if ($('#view-online').hasClass("active")) {
                _pesquisaPosicaoFrota.RastreadorOnlineOffline.val(null);

                controlarExibicoesMapaEGrid(getMode(), false);
                $(".view-select-button.active").removeClass('active');
                return;
            } else {
                $(".view-select-button.active").removeClass('active');
                $('#view-online').addClass('active');
            }

            _pesquisaPosicaoFrota.RastreadorOnlineOffline.val(true);
            controlarExibicoesMapaEGrid(getMode(), false);
            PintarOpcaoAtivaOnlineOffline("online");
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), text: "Tabela da Frota"
    });
    this.OfflineClick = PropertyEntity({
        eventClick: function () {

            if ($('#view-offline').hasClass("active")) {
                _pesquisaPosicaoFrota.RastreadorOnlineOffline.val(null);

                controlarExibicoesMapaEGrid(getMode(), false);
                $(".view-select-button.active").removeClass('active');
                return;
            } else {
                $(".view-select-button.active").removeClass('active');
                $('#view-offline').addClass('active');
            }

            _pesquisaPosicaoFrota.RastreadorOnlineOffline.val(false);
            controlarExibicoesMapaEGrid(getMode(), false);
            PintarOpcaoAtivaOnlineOffline("offline");
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), text: "Tabela da Frota"
    });

    this.QuantidadeVeiculosDisponiveis = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.QuantidadeOnline = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.QuantidadeOffline = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });

    this.QuantidadeOnline.porcentagem = ko.computed(() => formatarPercentual("Online", this.QuantidadeOnline.val(), this.QuantidadeOffline.val()));
    this.QuantidadeOffline.porcentagem = ko.computed(() => formatarPercentual("Offline", this.QuantidadeOffline.val(), this.QuantidadeOnline.val()));
}

function formatarPercentual(tipo, quantidadePercentual, quantidadeSoma) {
    const total = quantidadePercentual + quantidadeSoma;
    const percentual = total > 0 ? Math.round((quantidadePercentual / total) * 100) : (quantidadePercentual > 0 ? 100 : 0);
    return `${tipo} - ${percentual}%`;
}

function PesquisaFiltroTelaPrincipal() {
    this.VisualizarOrigens = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.VisualizarOrigens });
    this.VisualizarDestinos = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.VisualizarDestinos });
    this.VisualizarFrotas = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.VisualizarFrotas });
    this.Placa = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.Placa });
    this.Carga = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.Carga });
    this.PercentualViagem = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.PercentualViagem });
    this.PrevisaoEntregaAtualizada = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.PrevisaoEntregaAtualizada });
    this.EnderecoDaEntrega = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.EnderecoDaEntrega });
    this.Status = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.Status });
    this.Posicao = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.Posicao });
    this.RazaoSocialTransportador = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.Transportador });
    this.TempoStatus = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.TempoAtual });
    this.MostrarVeiculosAgrupadosNoMapa = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.AgruparVeiculos });
    this.Salvar = PropertyEntity({
        eventClick: function (e) {
            salvarFiltrosModoTV(true);
            Global.fecharModal('divModalFiltrosTelaPrincipal');
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false)
    });
    this.VoltarAoPadrao = PropertyEntity({
        eventClick: function (e) {
            voltaAoPadrao();
            Global.fecharModal('divModalFiltrosTelaPrincipal');
        }, type: types.event, text: "Voltar ao Padrão", idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false)
    });
}

function PesquisaFiltroModoTV() {
    var self = this;
    this.Legendas = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.VisualizarLegendaMapa });
    this.VisualizarOrigens = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.VisualizarOrigens });
    this.VisualizarDestinos = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.VisualizarDestinos });
    this.VisualizarFrotas = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.VisualizarFrotas });
    this.FrequenciaAtualizacao = ko.observable();
    this.MostrarVeiculosAgrupadosNoMapa = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Logistica.PosicaoFrota.AgruparVeiculos });
    this.Salvar = PropertyEntity({
        eventClick: function (e) {
            salvarFiltrosModoTV(false);
            Global.fecharModal('divModalFiltrosModoTV');
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false)
    });
    this.VoltarAoPadrao = PropertyEntity({
        eventClick: function (e) {
            voltaAoPadrao();
            Global.fecharModal('divModalFiltrosModoTV');
        }, type: types.event, text: "Voltar ao Padrão", idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false)
    });

    this.iniciarAtualizacaoMapa = function () {
        if (intervaloAtualizacao) {
            clearInterval(intervaloAtualizacao);
        }

        var frequencia = self.FrequenciaAtualizacao();
        if (frequencia > 0 && isFullscreen()) {
            var tempoRestante = frequencia;
            _playerPosicaoFrota.Pause.eventClick();

            $("#contador-atualizacao").show();
            $('#contador-atualizacao span.ContadorAtualizacao').html("Atualização em " + tempoRestante + "...");

            intervaloAtualizacao = setInterval(function () {
                $('#contador-atualizacao span.ContadorAtualizacao').html("Atualização em " + tempoRestante + "...");
                tempoRestante--;

                if (tempoRestante < 0) {
                    actionObterDadosMapa(false);
                    tempoRestante = frequencia;
                }
            }, 1000)
        } else {
            $("#contador-atualizacao").hide();
        }
    };

    this.FrequenciaAtualizacao.subscribe(function () {
        self.iniciarAtualizacaoMapa();
    });
}

function FiltrosTelaPrincipal() {
    Global.abrirModal('divModalFiltrosTelaPrincipal');
}
function FiltrosModoTV() {
    var isMix = $('#mapa-grid-container').is(':visible');
    var isFull = isFullscreen();
    var alvo;

    if (isFull) {
        alvo = document.fullscreenElement;
    } else {
        alvo = isMix ? '#mapa-grid-container' : '#divMapa';
    }

    $('#divModalFiltrosModoTV').appendTo(alvo).css({ 'position': 'absolute', 'z-index': '10000' });
    Global.abrirModal('divModalFiltrosModoTV');
}

var PlayerPosicaoFrota = function () {
    var self = this;

    function getMapaAtivo() {
        var isMix = $('#mapa-grid-container').is(':visible');
        return {
            isMix: isMix,
            map: isMix ? _novoMapaMT : _novoMapa,
            reload: function (contarCarrossel) {
                var pos = this.map ? this.map.getCenter() : null;
                var args = pos ? { Zoom: this.map.getZoom(), Latitude: pos.lat, Longitude: pos.lng } : null;
                if (this.isMix) {
                    actionObterDadosMapaTabela(!!contarCarrossel, args);
                } else {
                    actionObterDadosMapa(!!contarCarrossel, args);
                }
            }
        };
    }

    this.Play = PropertyEntity({
        type: types.event,
        text: "Play",
        idGrid: guid(),
        eventClick: function () {
            if (_timerInterval == null) {
                $('#' + _playerPosicaoFrota.Play.id).addClass("disable");
                $('#' + _playerPosicaoFrota.Pause.id).removeClass("disable");

                _timerInterval = setInterval(function () {
                    if (_countSeconds == 0) {
                        _countSeconds = _totalSeconds + 1;
                        getMapaAtivo().reload(true);
                    }
                    _countSeconds--;
                    $('#knockoutPlayerPosicaoFrota span.count-seconds').html(_countSeconds);
                    $('#knockoutPlayerPosicaoFrotaMt span.count-seconds').html(_countSeconds);
                }, 1000);
            }
        }
    });

    this.Pause = PropertyEntity({
        type: types.event,
        text: "Pause",
        idGrid: guid(),
        eventClick: function () {
            if (_timerInterval != null) {
                $('#' + _playerPosicaoFrota.Play.id).removeClass("disable");
                $('#' + _playerPosicaoFrota.Pause.id).addClass("disable");
                clearInterval(_timerInterval);
                _timerInterval = null;
            }
        }
    });

    this.Refresh = PropertyEntity({
        type: types.event,
        text: Localization.Resources.Gerais.Geral.Atualizar,
        idGrid: guid(),
        eventClick: function () {
            getMapaAtivo().reload(true);
        }
    });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadPosicaoFrota() {
    loadPesquisaPosicaoFrota();
    buscaGrupoStatusViagem(loadPosicaoFrotaGoGo);
    loadCabecalhoPosicaoFrota();
}
function loadPlayeraPosicaoFrota() {
    _playerPosicaoFrota = new PlayerPosicaoFrota();
    $('#knockoutPlayerPosicaoFrota span.count-seconds').html(_countSeconds);
    KoBindings(_playerPosicaoFrota, "knockoutPlayerPosicaoFrota", false, _playerPosicaoFrota.Play.id);
    $('#knockoutPlayerPosicaoFrotaMt span.count-seconds').html(_countSeconds);
    KoBindings(_playerPosicaoFrota, "knockoutPlayerPosicaoFrotaMt", false, _playerPosicaoFrota.Play.id);
    $('#' + _playerPosicaoFrota.Pause.id).addClass("disable");
}

function mapaPosicaoFrotaFullScreen() {
    document.removeEventListener('fullscreenchange', resizeOverlay);
    document.addEventListener('fullscreenchange', resizeOverlay);
    window.removeEventListener('resize', resizeOverlay);
    window.addEventListener('resize', resizeOverlay);

    var isMix = $('#mapa-grid-container').is(':visible');
    var targetEl = isMix ? document.getElementById('map-mt') : document.getElementById('divMapa');

    if (!document.fullscreenElement) {
        targetEl && targetEl.requestFullscreen && targetEl.requestFullscreen();
    } else {
        document.exitFullscreen && document.exitFullscreen();
    }
}

function loadPosicaoFrotaLegenda() {
    $('#btn-legenda').off('click.posicaofrota').on('click.posicaofrota', function () {
        _pesquisaModoTv.Legendas.val(!_pesquisaModoTv.Legendas.val());

        var raw = localStorage.getItem('preferenciasMapa');
        var pref = raw ? JSON.parse(raw) : {};
        pref.Legendas = !!_pesquisaModoTv.Legendas.val();
        localStorage.setItem('preferenciasMapa', JSON.stringify(pref));

        aplicarVisibilidadeLegendas();
    });
}

function resizeOverlay() {
    var fsEl = document.fullscreenElement;
    var isFull = !!fsEl;
    var isMix = isFull && fsEl.id === 'map-mt';
    var mapObj = isMix ? _novoMapaMT : _novoMapa;

    var prevCenter = null;
    var prevZoom = null;
    if (mapObj && mapObj.getCenter && mapObj.getZoom) {
        prevCenter = mapObj.getCenter();
        prevZoom = mapObj.getZoom();
    }

    if (isFull) {
        var $root = $(fsEl);
        var $mapEl = isMix ? $('#map-mt') : $('#map');

        $root.addClass('fullscreen');
        $mapEl.css({ width: window.innerWidth + 'px', height: window.innerHeight + 'px' });

        var $legenda = isMix ? $('#legenda-totais-container-mt') : $('#legenda-totais-container');

        $legenda.appendTo($root);
        $('#btn-config').appendTo($root).show();
        $('#carousel-filters').hide();
        $('#fullscreen-carousel').appendTo($root).show();

        aplicarVisibilidadeLegendas();

        if (_dadosCarrossel) {
            setTimeout(function () {
                ObterDadosFrotaCarrossel(_dadosCarrossel);
            }, 100);
        }

        if (mapObj && mapObj.invalidateSize) mapObj.invalidateSize();
        if (prevCenter && prevZoom != null && mapObj && mapObj.setView) {
            mapObj.setView(prevCenter, prevZoom, { animate: false });
        }

        if (!intervaloAtualizacao && _pesquisaModoTv && _pesquisaModoTv.iniciarAtualizacaoMapa) {
            _pesquisaModoTv.iniciarAtualizacaoMapa();
        }
    } else {
        $('#divMapa, #map-mt').removeClass('fullscreen');
        $('#map, #map-mt').css({ width: '', height: '' });

        $('#fullscreen-carousel').hide();
        $('#carousel-filters').show();

        $('#legenda-totais-container').appendTo('#widget-mapa');
        $('#legenda-totais-container-mt').appendTo('#widget-mapa-mt');

        var isMixMode = $('#mapa-grid-container').is(':visible');

        if (isMixMode) {
            $('#legenda-totais-container').hide();
            $('#legenda-totais-container-mt').show();
        } else {
            $('#legenda-totais-container-mt').hide();
            $('#legenda-totais-container').show();
        }

        $('#btn-config').hide();

        if (intervaloAtualizacao) {
            clearInterval(intervaloAtualizacao);
            intervaloAtualizacao = null;
        }
        $("#contador-atualizacao").hide();

        if (_dadosCarrossel) {
            setTimeout(function () {
                ObterDadosFrotaCarrossel(_dadosCarrossel);
            }, 100);
        }

        setTimeout(function () {
            if (mapObj && mapObj.invalidateSize) mapObj.invalidateSize();
            if (prevCenter && prevZoom != null && mapObj && mapObj.setView) {
                mapObj.setView(prevCenter, prevZoom, { animate: false });
            }
        }, 50);
    }
}

function aplicarVisibilidadeLegendas() {
    var isFull = isFullscreen();
    var legendasAtivas = _pesquisaModoTv && _pesquisaModoTv.Legendas ? _pesquisaModoTv.Legendas.val() : true;

    if (isFull) {
        var fsEl = document.fullscreenElement;
        var isMix = fsEl && fsEl.id === 'map-mt';
        var $legenda = isMix ? $('#legenda-totais-container-mt') : $('#legenda-totais-container');

        if (legendasAtivas) {
            $legenda.show();
        } else {
            $legenda.hide();
        }
    } else {
        var isMixMode = $('#mapa-grid-container').is(':visible');
        var $legendaAtiva = isMixMode ? $('#legenda-totais-container-mt') : $('#legenda-totais-container');
        var $legendaInativa = isMixMode ? $('#legenda-totais-container') : $('#legenda-totais-container-mt');

        if (legendasAtivas) {
            $legendaAtiva.show();
        } else {
            $legendaAtiva.hide();
        }
        $legendaInativa.hide();
    }
}

function isFullscreen() {
    return (document.fullscreenElement || document.webkitFullscreenElement || document.mozFullScreenElement || document.msFullscreenElement) != null;
}

function loadPosicaoFrotaGoGo() {
    new BuscarVeiculos(_pesquisaPosicaoFrota.Veiculo, null, null, null, null, null, null, null, false);
    new BuscarFilial(_pesquisaPosicaoFrota.Filial);
    new BuscarCategoriaPessoa(_pesquisaPosicaoFrota.CategoriaPessoa);
    new BuscarClientes(_pesquisaPosicaoFrota.Cliente);
    new BuscarTransportadores(_pesquisaPosicaoFrota.Transportador, null, null, true);
    new BuscarTiposOperacao(_pesquisaPosicaoFrota.TipoOperacao);
    new BuscarClientes(_pesquisaPosicaoFrota.Remetente);
    new BuscarClientes(_pesquisaPosicaoFrota.Destinatario);
    new BuscarMotoristas(_pesquisaPosicaoFrota.Motorista);
    new BuscarCentroResultado(_pesquisaPosicaoFrota.CentroResultado);
    new BuscarFuncionario(_pesquisaPosicaoFrota.FuncionarioResponsavel);
    new BuscarContratoFreteTransportador(_pesquisaPosicaoFrota.ContratoFrete);
    new BuscarTipoContratoFrete(_pesquisaPosicaoFrota.TipoContratoFrete);
    new BuscarLocais(_pesquisaPosicaoFrota.Locais);
    new BuscarGruposPessoas(_pesquisaPosicaoFrota.GrupoPessoas);
    new BuscarTecnologiaRastreador(_pesquisaPosicaoFrota.TecnologiaRastreador);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaPosicaoFrota.Filial.visible(false);
        _pesquisaPosicaoFrota.Transportador.visible(false);
        _pesquisaPosicaoFrota.SituacaoVeiculo.visible(true);
        _pesquisaPosicaoFrota.TipoOperacao.visible(true);
        _pesquisaPosicaoFrota.VeiculosComContratoDeFrete.visible(false);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        _pesquisaPosicaoFrota.Filial.visible(false);
        _pesquisaPosicaoFrota.Transportador.visible(false);
        _pesquisaPosicaoFrota.TipoOperacao.visible(false);
        _pesquisaPosicaoFrota.VeiculosComContratoDeFrete.visible(false);
    }

    var cssClass = "col col-sm-12 col-2";
    if (_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramento) {
        _pesquisaPosicaoFrota.GrupoTipoOperacao.visible(true);
        buscaGrupoTipoOperacao();
        if (_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem) {
            _pesquisaPosicaoFrota.GrupoStatusViagem.visible(false);
        } else {
            _pesquisaPosicaoFrota.GrupoStatusViagem.visible(true);
            cssClass = "col col-sm-12 col-1"
        }
    } else {
        _pesquisaPosicaoFrota.GrupoTipoOperacao.visible(false);
    }
    //_pesquisaPosicaoFrota.Situacao.cssClass(cssClass);

    var configExportacao = {
        url: "PosicaoFrota/Exportar",
        titulo: "Logística - Posição da Frota"
    };
    _gridPosicaoFrota = new GridView("grid-posicao-frota", "PosicaoFrota/Pesquisa", _pesquisaPosicaoFrota, undefined, undefined, 15, undefined, true, false, undefined, undefined, undefined, configExportacao, undefined, undefined, undefined, gridPosicaoFrotaCallbackColumnDefault);
    _gridPosicaoFrota.SetPermitirEdicaoColunas(true);
    _gridPosicaoFrota.SetSalvarPreferenciasGrid(true);
    _gridPosicaoFrota.SetHabilitarModelosGrid(true);
    _gridPosicaoFrota.SetHabilitarScrollHorizontal(true, 200);

    // Inicialização do mapa
    loadMap();

    $("#" + _pesquisaPosicaoFrota.EmAlvo.id).click(verificarPesquisaEmAlvo);

    // Executa a pesquisa com os filtros iniciais
    //_pesquisaPosicaoFrota.Pesquisar.eventClick();
    $("#posicao-frota-container").hide();
    $("#sem-dados-container").show();

    loadPlayeraPosicaoFrota();

    loadFiltroPesquisaPosicaoFrota();
    loadPesquisaModoTV();
    loadPosicaoFrotaLegenda();

}
function verificarPesquisaEmAlvo(e, sender) {
    if (_pesquisaPosicaoFrota.EmAlvo.val() == true) {
        _pesquisaPosicaoFrota.Cliente.enable(true);
        _pesquisaPosicaoFrota.CategoriaPessoa.enable(true);
    } else {
        _pesquisaPosicaoFrota.Cliente.enable(false);
        _pesquisaPosicaoFrota.Cliente.val("");
        _pesquisaPosicaoFrota.CategoriaPessoa.enable(false);
        _pesquisaPosicaoFrota.CategoriaPessoa.val("");
    }
}

function callbackRowPosicaoFrota(nRow, aData) {
    var span = $(nRow).find('td').eq(4).find('span')[0];
    if (span) {
        moment.locale('pt-br');
        var permanencia = moment(aData.TempoDaUltimaPosicaoFormatada, "DD/MM/YYYY HH:mm:ss").fromNow();
        $(span).text(permanencia);
    }
}

function loadFiltroPesquisaPosicaoFrota() {
    var data = { TipoFiltro: EnumCodigoFiltroPesquisa.PosicaoFrota };

    executarReST("ModeloFiltroPesquisa/ObterFiltroPesquisaPadrao", data, function (res) {
        if (res.Success && Boolean(res.Data)) {
            PreencherJsonFiltroPesquisa(_pesquisaPosicaoFrota, res.Data.Dados);
            _pesquisaPosicaoFrota.ModeloFiltrosPesquisa.codEntity(res.Data.Codigo);
            _pesquisaPosicaoFrota.ModeloFiltrosPesquisa.val(res.Data.Descricao);
            controlarExibicoesMapaEGrid(getMode(), false);
        }
    });
}


/*
 * Declaração das Funções do Mapa
 */

function loadPesquisaPosicaoFrota() {
    _pesquisaPosicaoFrota = new PesquisaPosicaoFrota();
    KoBindings(_pesquisaPosicaoFrota, "knockoutPesquisaPosicaoFrota", _pesquisaPosicaoFrota.Pesquisar.id, _pesquisaPosicaoFrota.Pesquisar.id);
}

function loadCabecalhoPosicaoFrota() {
    _cabecalhoPosicaoFrota = new CabecalhoPosicaoFrota();
    KoBindings(_cabecalhoPosicaoFrota, "knockoutCabecalhoPosicaoFrota");
}

function loadPesquisaModoTV() {
    _pesquisaTelaPrincipal = new PesquisaFiltroTelaPrincipal();
    KoBindings(_pesquisaTelaPrincipal, "knockoutFiltrosTelaPrincipal");

    _pesquisaModoTv = new PesquisaFiltroModoTV();
    KoBindings(_pesquisaModoTv, "knockoutFiltrosModoTV");
}

function loadMap() {
    if (!_novoMapa) {
        const esriSat = L.tileLayer(
            'https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}',
            {
                attribution: 'Tiles © Esri, Maxar, Earthstar Geographics, USDA, USGS, AeroGRID, IGN, and GIS User Community',
                maxNativeZoom: 17,
                maxZoom: 19,
            }
        );

        const esriLabels = L.tileLayer(
            'https://services.arcgisonline.com/ArcGIS/rest/services/Reference/World_Boundaries_and_Places/MapServer/tile/{z}/{y}/{x}',
            {
                attribution: 'Labels © Esri',
                maxNativeZoom: 17,
                maxZoom: 19,
            }
        );

        const esriRuas = L.tileLayer(
            'https://services.arcgisonline.com/ArcGIS/rest/services/Reference/World_Transportation/MapServer/tile/{z}/{y}/{x}',
            {
                attribution: 'Roads © Esri',
                maxNativeZoom: 17,
                maxZoom: 19,
            }
        );

        const esriComLabels = L.layerGroup([esriSat, esriLabels, esriRuas]);

        const openStreetMap = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; OpenStreetMap contributors'
        });

        _novoMapa = L.map('map', {
            zoomControl: false,
            wheelPxPerZoomLevel: 90,
            layers: [openStreetMap],
            minZoom: 2,
            maxZoom: 19
        }).setView([-12.30521, -51.17696], 4);

        L.control.layers(
            {
                "Ruas": openStreetMap,
                "Satélite": esriComLabels
            },
            null,
            { position: 'topleft' }
        ).addTo(_novoMapa);

        const quantidadeZoom = L.control({ position: 'bottomleft' });
        quantidadeZoom.onAdd = function () {
            const div = L.DomUtil.create('div', 'zoom-overlay');
            div.style.background = 'rgba(0,0,0,0.6)';
            div.style.color = 'white';
            div.style.padding = '4px 8px';
            div.style.fontSize = '12px';
            div.style.borderRadius = '4px';
            div.innerHTML = 'Zoom: ' + _novoMapa.getZoom() + '/19';
            this._div = div;
            return div;
        };
        quantidadeZoom.addTo(_novoMapa);

        _novoMapa.on('zoomend', function () {
            const zoomAtual = _novoMapa.getZoom();
            quantidadeZoom._div.innerHTML = 'Zoom: ' + zoomAtual + '/19';
        });
    }
}
function loadMapaTabela() {
    if (_novoMapaMT) { setTimeout(function () { _novoMapaMT.invalidateSize(); }, 200); return; }

    const esriSat = L.tileLayer(
        'https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}',
        { attribution: 'Tiles © Esri, Maxar, Earthstar Geographics, USDA, USGS, AeroGRID, IGN, and GIS User Community', maxNativeZoom: 17, maxZoom: 19 }
    );
    const esriLabels = L.tileLayer(
        'https://services.arcgisonline.com/ArcGIS/rest/services/Reference/World_Boundaries_and_Places/MapServer/tile/{z}/{y}/{x}',
        { attribution: 'Labels © Esri', maxNativeZoom: 17, maxZoom: 19 }
    );
    const esriRuas = L.tileLayer(
        'https://services.arcgisonline.com/ArcGIS/rest/services/Reference/World_Transportation/MapServer/tile/{z}/{y}/{x}',
        { attribution: 'Roads © Esri', maxNativeZoom: 17, maxZoom: 19 }
    );
    const esriComLabels = L.layerGroup([esriSat, esriLabels, esriRuas]);

    const openStreetMap = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19, attribution: '&copy; OpenStreetMap contributors'
    });

    _novoMapaMT = L.map('map-mt', {
        zoomControl: false, wheelPxPerZoomLevel: 90, layers: [openStreetMap], minZoom: 2, maxZoom: 19
    }).setView([-12.30521, -51.17696], 4);

    L.control.layers({ "Ruas": openStreetMap, "Satélite": esriComLabels }, null, { position: 'topleft' }).addTo(_novoMapaMT);

    const quantidadeZoom = L.control({ position: 'bottomleft' });
    quantidadeZoom.onAdd = function () {
        const div = L.DomUtil.create('div', 'zoom-overlay');
        div.style.background = 'rgba(0,0,0,0.6)';
        div.style.color = 'white';
        div.style.padding = '4px 8px';
        div.style.fontSize = '12px';
        div.style.borderRadius = '4px';
        div.innerHTML = 'Zoom: ' + _novoMapaMT.getZoom() + '/19';
        this._div = div;
        return div;
    };
    quantidadeZoom.addTo(_novoMapaMT);
    _novoMapaMT.on('zoomend', function () { quantidadeZoom._div.innerHTML = 'Zoom: ' + _novoMapaMT.getZoom() + '/19'; });
}
function applyDefaultView(map) {
    if (!map) return;
    try { map.setView([-12.30521, -51.17696], 4); } catch (e) { }
}
function getMode() {
    if ($('#mapa-grid-container').is(':visible')) return 'mix';
    if ($('#mapa-container').is(':visible')) return 'map';
    return 'grid';
}

function isMixModeVisible() {
    return getMode() === 'mix';
}
function getMapaTipoContext() {
    var isMix = $('#mapa-grid-container').is(':visible');
    return {
        isMix: isMix,
        map: isMix ? _novoMapaMT : _novoMapa,
        markers: isMix ? _markersMT : _markers,
        markersFilial: isMix ? _markersFilialMT : _markersFilial,
        selLegVeiculos: isMix ? '#legenda-veiculos-mt ul' : '#legenda-veiculos ul',
        selLegReboque: isMix ? '#legenda-reboque-mt ul' : '#legenda-reboque ul',
        selLegLocais: isMix ? '#legenda-local-mt ul' : '#legenda-local ul',
        selLegContainer: isMix ? '#legenda-totais-container-mt' : '#legenda-totais-container',
        ensureMap: function () { isMix ? loadMapaTabela() : loadMap(); }
    };
}

function ensureMarkerGroups(mapaTipo) {
    const agrupar = _pesquisaModoTv.MostrarVeiculosAgrupadosNoMapa.val();
    if (mapaTipo.markers) mapaTipo.map.removeLayer(mapaTipo.markers);
    if (mapaTipo.markersFilial) mapaTipo.map.removeLayer(mapaTipo.markersFilial);
    mapaTipo.markers = agrupar ? L.markerClusterGroup() : L.layerGroup();
    mapaTipo.markersFilial = agrupar ? L.markerClusterGroup() : L.layerGroup();

    if (mapaTipo.isMix) { _markersMT = mapaTipo.markers; _markersFilialMT = mapaTipo.markersFilial; }
    else { _markers = mapaTipo.markers; _markersFilial = mapaTipo.markersFilial; }
}

function obterDadosMapa(mapaTipo, contarQuantidadeCarrossel, localizacaAnteriorMapa) {
    if (!mapaTipo.map) mapaTipo.ensureMap();

    var mapRef = mapaTipo.map;

    if (mapaTipo.isMix) {
        if (_markersMT) mapRef.removeLayer(_markersMT);
        if (_markersFilialMT) mapRef.removeLayer(_markersFilialMT);
    } else {
        if (_markers) mapRef.removeLayer(_markers);
        if (_markersFilial) mapRef.removeLayer(_markersFilial);
    }

    if (_pesquisaModoTv.MostrarVeiculosAgrupadosNoMapa.val()) {
        mapaTipo.markers = L.markerClusterGroup();
        mapaTipo.markersFilial = L.markerClusterGroup();
    } else {
        mapaTipo.markers = L.layerGroup();
        mapaTipo.markersFilial = L.layerGroup();
    }

    if (mapaTipo.isMix) {
        _markersMT = mapaTipo.markers;
        _markersFilialMT = mapaTipo.markersFilial;
    } else {
        _markers = mapaTipo.markers;
        _markersFilial = mapaTipo.markersFilial;
    }

    executarReST("PosicaoFrota/ObterDadosMapa", RetornarObjetoPesquisa(_pesquisaPosicaoFrota), function (arg) {
        if (!arg.Success) {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            return;
        }
        if (arg.Data === false) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            return;
        }

        $(mapaTipo.selLegVeiculos).html('');
        $(mapaTipo.selLegReboque).html('');
        $(mapaTipo.selLegLocais).html('');

        if (arg.Data.Veiculos) {
            pinMarkersVeiculo(arg.Data.Veiculos, mapaTipo);
            InserirLegendaReboque(mapaTipo.selLegReboque, "Disponível");
            if (mapaTipo.isMix) {
                loadSidebarMapaTabela();
                updateSidebarFrotaFromVeiculos(arg.Data.Veiculos);
            }
        }

        carregarPreferenciasMapa();
        pinAreasAlvos(arg.Data.Alvos, mapaTipo);

        if (arg.Data.Filiais) {
            pinFiliais(arg.Data.Filiais, mapaTipo);
            InserirLegendaLocais(mapaTipo.selLegLocais, "Planta", _pesquisaPosicaoFrota.ClientesComVeiculoEmAlvo.val(), _pesquisaPosicaoFrota.ClientesAlvosEstrategicos.val());
        }
        pinAreasLocais(arg.Data.Locais, mapaTipo);

        if (contarQuantidadeCarrossel) {
            _dadosCarrossel = arg.Data.GruposCarrossel;
            ObterDadosFrotaCarrossel(_dadosCarrossel);
            ObterQuantidadeInformacoesVeiculoCarrossel(arg.Data.Veiculos);
            ObterOnlineOffline(arg.Data.Veiculos);
        }

        if (_pesquisaModoTv.Legendas.val()) {
            $(mapaTipo.selLegContainer).show();
        } else {
            $(mapaTipo.selLegContainer).hide();
        }

        var $legendaInativa = mapaTipo.isMix ? $('#legenda-totais-container') : $('#legenda-totais-container-mt');
        $legendaInativa.hide();

        $('#legenda-veiculos ul, #legenda-veiculos-mt ul').html('');
        for (var i = 0; i < arg.Data.Grupos.length; i++) {
            InserirLegenda(mapaTipo.selLegVeiculos, arg.Data.Grupos[i].Descricao, arg.Data.Grupos[i].Total, arg.Data.Grupos[i].Cor);
        }

        mapRef.addLayer(mapaTipo.markers);
        mapRef.addLayer(mapaTipo.markersFilial);

        if (_pesquisaModoTv.MostrarVeiculosAgrupadosNoMapa.val() && mapaTipo.markers.getLayers().length > 0) {
            try { mapRef.fitBounds(mapaTipo.markers.getBounds()); } catch (e) { }
        }

        if (localizacaAnteriorMapa) {
            mapRef.setView([localizacaAnteriorMapa.Latitude, localizacaAnteriorMapa.Longitude], localizacaAnteriorMapa.Zoom);
        } else if (!_pesquisaModoTv.MostrarVeiculosAgrupadosNoMapa.val()) {
            try {
                var allLayers = [];
                if (mapaTipo.markers && mapaTipo.markers.getLayers) allLayers = allLayers.concat(mapaTipo.markers.getLayers());
                if (mapaTipo.markersFilial && mapaTipo.markersFilial.getLayers) allLayers = allLayers.concat(mapaTipo.markersFilial.getLayers());
                if (allLayers.length > 0) {
                    var group = L.featureGroup(allLayers);
                    mapRef.fitBounds(group.getBounds(), { padding: [50, 50], maxZoom: 15 });
                } else mapRef.setView([-12.30521, -51.17696], 4);
            } catch (e) {
                mapRef.setView([-12.30521, -51.17696], 4);
            }
        }
    }, null, true);
}

function actionObterDadosMapa(contarQuantidadeCarrossel, localizacaAnteriorMapa) {
    loadMap();
    return obterDadosMapa(getMapaTipoContext(), contarQuantidadeCarrossel, localizacaAnteriorMapa);
}

function actionObterDadosMapaTabela(contarQuantidadeCarrossel, localizacaAnteriorMapa) {
    loadMapaTabela();
    loadSidebarMapaTabela();
    return obterDadosMapa(getMapaTipoContext(), contarQuantidadeCarrossel, localizacaAnteriorMapa);
}


function gridPosicaoFrotaCallbackColumnDefault(cabecalho, valorColuna, dadosLinha) {
    if (cabecalho.name == "RastreadorOnlineOffline") {
        return obterIconeHtmlColunaRastreador(dadosLinha);
    }
}
function obterIconeHtmlColunaRastreador(dadosLinha) {

    var icone = ObterIconeStatusTracking(parseInt(dadosLinha.RastreadorOnlineOffline), 20);

    return '<div class="tracking-indicador" title="' + dadosLinha.DataDaPosicaoFormatada + '">' + icone + '</div>';
}
function clickMarkerVeiculoAgrupado(popup) {
    clickMarkerVeiculo(null, null, popup);
}
function clickMarkerVeiculo(infoWindow, marker, popup) {
    var carregarInformacoesVeiculo = false;

    if (popup.getContent().substring(0, 4) != '<!--') {
        return;
    }
    carregarInformacoesVeiculo = true;
    var codigoVeiculo = popup.getContent().substring(4, popup.getContent().indexOf('-->'));

    // Dados do veículo ainda não carregados
    if (carregarInformacoesVeiculo) {

        executarReST("PosicaoFrota/ObterDadosVeiculo", { CodigoVeiculo: codigoVeiculo }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {

                    var icone = TrackingIconRastreador(arg.Data.Rastreador);
                    var html = `<div id="InfoWindowVeiculo" style="margin:0;padding:0;">`;

                    if (_pesquisaTelaPrincipal.Placa.val()) {
                        html += `<div class="col" style="margin:0;padding:0;display:flex;align-items:center;">
                            <p class="fs-5" style="margin:0;padding:0;font-weight:bold;">${arg.Data.PlacaVeiculo || '-'}</p>
                            <span style="color:#35a5d7;margin-left:10px;font-size:9px;">Historico de Posições</span>
                        </div>`;
                    }

                    if (_pesquisaTelaPrincipal.PercentualViagem.val()) {
                        html += `<div class="col" style="margin:0;padding:0;margin-top:10px;">
                        <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0;margin-bottom:10px;font-size:11px;color:#b8b8b8;font-weight:bold;">% VIAGEM</p>
                    </div>
                    <div class="progress" style="margin:0;padding:0;margin-top:5px;max-width:100%">
                        <div class="fill" style="width:${arg.Data.PercentualViagem || 0}%;background-color:#35a5d7;margin:0;padding:0;"></div>
                    </div>`;
                    }

                    if (_pesquisaTelaPrincipal.Carga.val() && _pesquisaTelaPrincipal.PrevisaoEntregaAtualizada.val()) {
                        html += `<div class="row" style="display:flex;flex-wrap:nowrap;margin:0;margin-top:5px;padding:0;">`;
                        html += `<div class="col" style="flex:0 0 27%;max-width:27%;margin:0;padding:0;">

                            <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0;margin-bottom:5px;margin-top:5px;font-size:11px;color:#b8b8b8;font-weight:bold;">CARGA ATUAL</p>
                            <p class="fw-400 fs-5" style="cursor:pointer;margin:0;padding:0;color:#35a5d7" onclick="ClickCarga(${(arg.Data.CargaVinculada)})">${(arg.Data.UltimaCarga && _pesquisaTelaPrincipal.Carga.val()) ? arg.Data.UltimaCarga : '-'}</p>
                        </div>`;
                    }

                    if (_pesquisaTelaPrincipal.Carga.val() && !(_pesquisaTelaPrincipal.PrevisaoEntregaAtualizada.val())) {
                        html += `<div class="row" style="margin:0;margin-top:5px;padding:0;">`;
                        html += `
                            <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0;margin-bottom:5px;margin-top:5px;font-size:11px;color:#b8b8b8;font-weight:bold;">CARGA ATUAL</p>
                            <p class="fw-400 fs-5" style="cursor:pointer;margin:0;padding:0;color:#35a5d7" onclick="ClickCarga(${(arg.Data.CargaVinculada)})">${(arg.Data.UltimaCarga && _pesquisaTelaPrincipal.Carga.val()) ? arg.Data.UltimaCarga : '-'}</p>
                        </div>`;
                    }

                    if (_pesquisaTelaPrincipal.PrevisaoEntregaAtualizada.val()) {
                        html += `<div class="col" style="flex:0 0 73%;max-width:73%;margin:0;padding:0;margin-top:5px;">
                            <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0;margin-bottom:5px;font-size:11px;color:#b8b8b8;font-weight:bold;">PREVISAO ENTREGA ATUALIZADA (ETA)</p>
                            <p class="fw-400 fs-5" style="margin:0;padding:0;">
                                ${arg.Data.PrevisaoEntregaAtualizada === '01/01/0001 00:00' || !arg.Data.PrevisaoEntregaAtualizada ? '-' : arg.Data.PrevisaoEntregaAtualizada}
                            </p>
                        </div>`;
                        html += `</div>`;
                    }

                    if (_pesquisaTelaPrincipal.EnderecoDaEntrega.val()) {
                        html += `<div class="col" style="margin:0;padding:0;margin-top:5px;">
                        <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0;margin-bottom:5px;font-size:11px;color:#b8b8b8;font-weight:bold;">Endereço Da Entrega</p>
                        <p class="fw-400 fs-5" style="margin:0;padding:0;">${arg.Data.EnderecoDaEntrega || '-'}</p>
                    </div>`
                    }
                    if (_pesquisaTelaPrincipal.TempoStatus.val()) {
                        html += `<div class="col" style="margin:0;padding:0;margin-top:5px;">
                        <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0;margin-bottom:5px;font-size:11px;color:#b8b8b8;font-weight:bold;">Tempo no Status</p>
                        <p class="fw-400 fs-5" style="margin:0;padding:0;">${arg.Data.TempoStatusDescricao || '-'}</p>
                    </div>`
                    }
                    if (_pesquisaTelaPrincipal.RazaoSocialTransportador.val()) {
                        html += `<div class="col" style="margin:0;padding:0;margin-top:5px;">
                        <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0;margin-bottom:5px;font-size:11px;color:#b8b8b8;font-weight:bold;">Razão social do Transportador da carga</p>
                        <p class="fw-400 fs-5" style="margin:0;padding:0;">${arg.Data.Transportador || '-'}</p>
                    </div>`
                    }
                    if (_pesquisaTelaPrincipal.Status.val()) {
                        html += `<div class="col" style="margin:0;padding:0;margin-top:5px;">
                        <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0;margin-bottom:5px;font-size:11px;color:#b8b8b8;font-weight:bold;">STATUS</p>
                        <p class="fw-400 fs-5" style="margin:0;padding:0;">${arg.Data.Status || '-'}</p>
                    </div>`
                    }
                    if (_pesquisaTelaPrincipal.Posicao.val()) {
                        html += `<div class="row" style="display:flex;align-items:flex-center;margin:0;padding:0;margin-top:5px;">
                        <div style="width:60px;min-width:60px;display:flex;align-items:center;justify-content:center;margin-left: -10px;margin-top:15px;">
                            ${icone || ''}
                        </div>
                        <div style="flex:1;padding:0;">
                            <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0 0 5px 0;font-size:11px;color:#b8b8b8;">POSIÇÃO</p>
                            <p class="fw-400 fs-5" style="margin:0;padding:0;">${arg.Data.Descricao || '-'}</p>
                        </div>
                    </div>`
                    }

                    html += `</div>`;

                    popup.setContent(html);
                    popup.update();

                    // Centraliza o mapa e ajusta o zoom após abrir o popup
                    let latLng = new L.LatLng(arg.Data.Latitude, arg.Data.Longitude);
                    if (_novoMapa)
                        _novoMapa.panTo(latLng);

                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null, false);
        // ... já foram carregados, apenas apresenta
    } else {
        infoWindow.setContent(marker.content);
    }
}
function clickMarkerVeiculoGenerico(popup, mapInstance) {
    if (!popup || popup.getContent().substring(0, 4) !== '<!--') return;

    var codigoVeiculo = popup.getContent().substring(4, popup.getContent().indexOf('-->'));
    executarReST("PosicaoFrota/ObterDadosVeiculo", { CodigoVeiculo: codigoVeiculo }, function (arg) {
        if (!arg.Success || arg.Data === false) {
            exibirMensagem(arg.Success ? tipoMensagem.atencao : tipoMensagem.falha, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            return;
        }

        var icone = TrackingIconRastreador(arg.Data.Rastreador);
        var html = `<div id="InfoWindowVeiculo" style="margin:0;padding:0;">`;

        if (_pesquisaTelaPrincipal.Placa.val()) {
            html += `<div class="col" style="margin:0;padding:0;display:flex;align-items:center;">
                        <p class="fs-5" style="margin:0;padding:0;font-weight:bold;">${arg.Data.PlacaVeiculo || '-'}</p>
                        <span style="color:#35a5d7;margin-left:10px;font-size:9px;">Historico de Posições</span>
                     </div>`;
        }

        if (_pesquisaTelaPrincipal.PercentualViagem.val()) {
            html += `<div class="col" style="margin:0;padding:0;margin-top:10px;">
                        <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0;margin-bottom:10px;font-size:11px;color:#b8b8b8;font-weight:bold;">% VIAGEM</p>
                     </div>
                     <div class="progress" style="margin:0;padding:0;margin-top:5px;max-width:100%">
                        <div class="fill" style="width:${arg.Data.PercentualViagem || 0}%;background-color:#35a5d7;margin:0;padding:0;"></div>
                     </div>`;
        }

        if (_pesquisaTelaPrincipal.Carga.val() && _pesquisaTelaPrincipal.PrevisaoEntregaAtualizada.val()) {
            html += `<div class="row" style="display:flex;flex-wrap:nowrap;margin:0;margin-top:5px;padding:0;">
                        <div class="col" style="flex:0 0 27%;max-width:27%;margin:0;padding:0;">
                            <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0;margin-bottom:5px;margin-top:5px;font-size:11px;color:#b8b8b8;font-weight:bold;">CARGA ATUAL</p>
                            <p class="fw-400 fs-5" style="cursor:pointer;margin:0;padding:0;color:#35a5d7" onclick="ClickCarga(${(arg.Data.CargaVinculada)})">${(arg.Data.UltimaCarga && _pesquisaTelaPrincipal.Carga.val()) ? arg.Data.UltimaCarga : '-'}</p>
                        </div>`;
        }

        if (_pesquisaTelaPrincipal.Carga.val() && !(_pesquisaTelaPrincipal.PrevisaoEntregaAtualizada.val())) {
            html += `<div class="row" style="margin:0;margin-top:5px;padding:0;">
                        <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0;margin-bottom:5px;margin-top:5px;font-size:11px;color:#b8b8b8;font-weight:bold;">CARGA ATUAL</p>
                        <p class="fw-400 fs-5" style="cursor:pointer;margin:0;padding:0;color:#35a5d7" onclick="ClickCarga(${(arg.Data.CargaVinculada)})">${(arg.Data.UltimaCarga && _pesquisaTelaPrincipal.Carga.val()) ? arg.Data.UltimaCarga : '-'}</p>
                     </div>`;
        }

        if (_pesquisaTelaPrincipal.PrevisaoEntregaAtualizada.val()) {
            html += `<div class="col" style="flex:0 0 73%;max-width:73%;margin:0;padding:0;margin-top:5px;">
                        <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0;margin-bottom:5px;font-size:11px;color:#b8b8b8;font-weight:bold;">PREVISAO ENTREGA ATUALIZADA (ETA)</p>
                        <p class="fw-400 fs-5" style="margin:0;padding:0;">
                            ${arg.Data.PrevisaoEntregaAtualizada === '01/01/0001 00:00' || !arg.Data.PrevisaoEntregaAtualizada ? '-' : arg.Data.PrevisaoEntregaAtualizada}
                        </p>
                     </div>
                     </div>`;
        }

        if (_pesquisaTelaPrincipal.EnderecoDaEntrega.val()) {
            html += `<div class="col" style="margin:0;padding:0;margin-top:5px;">
                        <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0;margin-bottom:5px;font-size:11px;color:#b8b8b8;font-weight:bold;">Endereço Da Entrega</p>
                        <p class="fw-400 fs-5" style="margin:0;padding:0;">${arg.Data.EnderecoDaEntrega || '-'}</p>
                     </div>`;
        }
        if (_pesquisaTelaPrincipal.TempoStatus.val()) {
            html += `<div class="col" style="margin:0;padding:0;margin-top:5px;">
                        <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0;margin-bottom:5px;font-size:11px;color:#b8b8b8;font-weight:bold;">Tempo no Status</p>
                        <p class="fw-400 fs-5" style="margin:0;padding:0;">${arg.Data.TempoStatusDescricao || '-'}</p>
                     </div>`;
        }
        if (_pesquisaTelaPrincipal.RazaoSocialTransportador.val()) {
            html += `<div class="col" style="margin:0;padding:0;margin-top:5px;">
                        <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0;margin-bottom:5px;font-size:11px;color:#b8b8b8;font-weight:bold;">Razão social do Transportador da carga</p>
                        <p class="fw-400 fs-5" style="margin:0;padding:0;">${arg.Data.Transportador || '-'}</p>
                     </div>`;
        }
        if (_pesquisaTelaPrincipal.Status.val()) {
            html += `<div class="col" style="margin:0;padding:0;margin-top:5px;">
                        <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0;margin-bottom:5px;font-size:11px;color:#b8b8b8;font-weight:bold;">STATUS</p>
                        <p class="fw-400 fs-5" style="margin:0;padding:0;">${arg.Data.Status || '-'}</p>
                     </div>`;
        }
        if (_pesquisaTelaPrincipal.Posicao.val()) {
            html += `<div class="row" style="display:flex;align-items:flex-center;margin:0;padding:0;margin-top:5px;">
                        <div style="width:60px;min-width:60px;display:flex;align-items:center;justify-content:center;margin-left:-10px;margin-top:15px;">${icone || ''}</div>
                        <div style="flex:1;padding:0;">
                            <p class="info-title fw-bold text-uppercase mb-0" style="margin:0;padding:0 0 5px 0;font-size:11px;color:#b8b8b8;">POSIÇÃO</p>
                            <p class="fw-400 fs-5" style="margin:0;padding:0;">${arg.Data.Descricao || '-'}</p>
                        </div>
                     </div>`;
        }

        html += `</div>`;
        popup.setContent(html);
        popup.update();

        if (mapInstance && arg.Data.Latitude && arg.Data.Longitude) {
            mapInstance.panTo(new L.LatLng(arg.Data.Latitude, arg.Data.Longitude));
        }
    }, null, false);
}

function ClickCarga(carga) {
    if (carga != undefined) {
        sessionStorage.setItem('codigoCarga', carga);
        window.open("#Cargas/Carga", '_blank');
    }
}

var _raiosFilial = [];
function AdicionarRaioFilial(self, latitude, longitude, raioKm) {
    if (!validLatLng(latitude, longitude)) return;
    var raioMetros = raioKm * 1000;
    var cor = "#54f011";
    var circle = L.circle([latitude, longitude], {
        color: cor,
        fillColor: cor,
        fillOpacity: 0.1,
        radius: raioMetros,
        opacity: 1
    });

    _raiosFilial.push(circle); // <- Guardamos o círculo para limpar depois
    circle.addTo(self._mapa);
}
function LimparRaiosFilialDoMapa(mapa) {
    for (var i = 0; i < _raiosFilial.length; i++) {
        mapa.removeLayer(_raiosFilial[i]);
    }
    _raiosFilial = [];
}
function validLatLng(lat, lng) {
    return lat >= -90 && lat <= 90 && lng >= -180 && lng <= 180;
}
function pinMarkersVeiculo(data, mapaTipo) {
    if (!_pesquisaModoTv.VisualizarFrotas.val()) return;
    if (!data || !data.length) return;

    var mapDraw = new MapaDraw();
    var mapRef = mapaTipo.map;

    for (var i = 0; i < data.length; i++) {
        var v = data[i];
        var iconUrl;

        if (v.ListaTipoModeloVeicular && v.ListaTipoModeloVeicular.includes(EnumTipoModeloVeicularCarga.Reboque)) {
            iconUrl = mapDraw.icons.pinReboque(false);
        } else if (v.ListaSituacaoVeiculo && v.ListaSituacaoVeiculo.includes(EnumSituacaoVeiculo.EmViagem)) {
            iconUrl = mapDraw.icons.pinTracaoDeslocamento(35, 35, v.Cor, false);
        } else {
            iconUrl = mapDraw.icons.pinTracao(35, 35, v.Cor, false);
        }

        var Icone = L.Icon.extend({ options: { iconUrl: iconUrl } });
        var conteudo = (_pesquisaTelaPrincipal.Placa.val() !== false)
            ? '<!--' + v.CodigoVeiculo + '--><div id="InfoWindowVeiculo"><div class="placa">' + (v.PlacaVeiculo || '-') + '</div>Carregando...</div>'
            : '<!--' + v.CodigoVeiculo + '--><div id="InfoWindowVeiculo"><div class="placa"></div>Carregando...</div>';

        var m = L.marker([v.Latitude, v.Longitude], { icon: new Icone() });
        m.options.codigoVeiculo = v.CodigoVeiculo;
        m.bindPopup(conteudo, { offset: [12, 0] });
        m.on('click', function (e) { clickMarkerVeiculoGenerico(e.target.getPopup(), mapRef); });

        var wifi = v.Rastreador ? mapDraw.icons.wifiOn() : mapDraw.icons.wifiOff();
        m.bindTooltip('<img src="' + wifi + '" alt="icon" style="width:20px;height:20px;">', { direction: 'top' }).openTooltip();

        mapaTipo.markers.addLayer(m);
    }
}
function pinFiliais(data, mapaTipo) {
    if (!mapaTipo || !mapaTipo.map) return;

    var mapRef = mapaTipo.map;
    var mapDraw = new MapaDraw();

    if (mapaTipo.markersFilial) mapaTipo.markersFilial.clearLayers();
    LimparRaiosFilialDoMapa(mapRef);

    for (var i = 0; i < _areasFiliais.length; i++) {
        mapRef.removeLayer(_areasFiliais[i]);
    }
    _areasFiliais = [];

    for (var i = 0; i < _subAreasFiliais.length; i++) {
        mapRef.removeLayer(_subAreasFiliais[i]);
    }
    _subAreasFiliais = [];

    if (!_pesquisaPosicaoFrota.ExibirFiliaisEBases.val()) return;

    if (!data || !data.length) return;

    var IconeFilial = L.Icon.extend({
        options: { iconUrl: mapDraw.icons.PinMapLocal(false), iconSize: [32, 32], iconAnchor: [16, 32] }
    });

    for (var i = 0; i < data.length; i++) {
        var filial = data[i];
        var latitude = filial.Latitude;
        var longitude = filial.Longitude;

        if (latitude && longitude) {
            var marker = L.marker([latitude, longitude], { icon: new IconeFilial() });

            if (_pesquisaPosicaoFrota.VeiculosDentroDoRaioFilial.val() && _pesquisaPosicaoFrota.RaioFilial.val()) {
                AdicionarRaioFilial({ _mapa: mapRef, _circles: [] }, latitude, longitude, _pesquisaPosicaoFrota.RaioFilial.val());
            }

            if (filial.AreaCliente) {
                try {
                    var areaJson = JSON.parse(filial.AreaCliente);
                    if (areaJson[0].type === "polygon") {
                        var paths = areaJson[0].paths.map(function (coord) { return [coord.lat, coord.lng]; });
                        var areaLayer = L.polygon(paths, {
                            color: areaJson[0].strokeColor || '#3388ff',
                            fill: false,
                            stroke: true,
                            weight: 3
                        }).addTo(mapRef);
                        _areasFiliais.push(areaLayer);
                    }
                } catch (e) { }
            }

            if (filial.Raio) {
                var raioLayer = L.circle([latitude, longitude], {
                    radius: filial.Raio,
                    color: '#3388ff',
                    fill: false,
                    stroke: true,
                    weight: 3
                }).addTo(mapRef);
                _areasFiliais.push(raioLayer);
            }

            if (filial.SubAreas) {
                desenharSubAreas(filial.SubAreas, mapaTipo);
            }

            marker.bindPopup('Filial: ' + filial.DescricaoFilial);
            mapaTipo.markersFilial.addLayer(marker);
        }
    }
}

function pinAreasAlvos(data, mapaTipo) {
    if (!_pesquisaModoTv.VisualizarDestinos.val()) return;

    var mapRef = mapaTipo ? mapaTipo.map : _novoMapa;
    var markers = mapaTipo ? mapaTipo.markers : _markers;

    if (!mapRef || !markers) return;
    if (!data) return;

    var mapDraw = new MapaDraw();

    for (var i = 0; i < data.length; i++) {
        try {
            var alvo = data[i];
            if (!validLatLng(alvo.Latitude, alvo.Longitude)) continue;

            markers.addLayer(
                L.marker([alvo.Latitude, alvo.Longitude], {
                    icon: L.icon({ iconUrl: mapDraw.icons.Cliente(), iconSize: [40, 40], iconAnchor: [20, 40], tooltipAnchor: [0, -40] })
                }).bindTooltip(alvo.Descricao, { direction: 'top', offset: [-15, -15] })
            );

            var cor = (alvo.Cor != null && alvo.Cor !== '') ? alvo.Cor : '#FF0000';

            if (alvo.TipoArea == 'Raio') {
                L.circle([alvo.Latitude, alvo.Longitude], { color: cor, fillColor: cor, fillOpacity: 0.5, radius: alvo.Raio, stroke: false }).addTo(mapRef);
                continue;
            }

            if (alvo.Area && alvo.Area.length > 0) {
                var path;
                try { path = JSON.parse(alvo.Area); } catch (e) { continue; }

                for (var j = 0; j < path.length; j++) {
                    try {
                        switch (path[j].type) {
                            case google.maps.drawing.OverlayType.CIRCLE:
                                L.circle(path[j].center, { color: cor, fillColor: cor, fillOpacity: 0.5, radius: path[j].radius, stroke: false }).addTo(mapRef);
                                break;
                            case google.maps.drawing.OverlayType.RECTANGLE:
                                if (path[j].bounds) {
                                    var b = path[j].bounds;
                                    L.polygon([[b.south, b.west], [b.south, b.east], [b.north, b.east], [b.north, b.west]], { color: cor, fillColor: cor, fillOpacity: 0.5, stroke: false }).addTo(mapRef);
                                }
                                break;
                            case google.maps.drawing.OverlayType.POLYGON:
                                if (path[j].paths) L.polygon(path[j].paths, { color: cor, fillColor: cor, fillOpacity: 0.5, stroke: false }).addTo(mapRef);
                                break;
                        }
                    } catch (e) { continue; }
                }
            }
        } catch (e) { continue; }
    }
}
function pinAreasLocais(data, mapaTipo) {
    var mapRef = mapaTipo ? mapaTipo.map : _novoMapa;

    if (!data) data = [];

    for (var i = 0; i < data.length; i++) {
        var area = data[i];

        if (area.Area && area.Area.length > 0) {
            var path;
            try {
                path = JSON.parse(area.Area);
            } catch (e) {
                continue;
            }
            for (var j = 0; j < path.length; j++) {
                try {
                    switch (path[j].type) {
                        case google.maps.drawing.OverlayType.CIRCLE:
                            L.circle(path[j].center, {
                                color: area.Cor,
                                fillColor: area.Cor,
                                fillOpacity: 0.7,
                                radius: path[j].radius,
                                stroke: false
                            }).addTo(mapRef);
                            break;

                        case google.maps.drawing.OverlayType.RECTANGLE:
                            if (path[j].bounds) {
                                var b = path[j].bounds;
                                L.polygon([[b.south, b.west], [b.south, b.east], [b.north, b.east], [b.north, b.west]],
                                    { color: area.Cor, stroke: false, fillOpacity: 0.7 }
                                ).addTo(mapRef);
                            }
                            break;

                        case google.maps.drawing.OverlayType.POLYGON:
                            L.polygon(
                                path[j].paths,
                                { color: area.Cor, stroke: false, fillOpacity: 0.7 }
                            ).addTo(mapRef);
                            break;
                    }
                } catch (e) {
                    continue;
                }
            }
        }
    }
}

/*
 * Declaração das Funções Associadas a Eventos
 */

/*
 * Demais funções
 */

function actionLegendas() {
    executarReST("PosicaoFrota/Legendas", null, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                if (arg.Data.Grupos != undefined) {
                    for (var i = 0; i < arg.Data.Grupos.length; i++) {
                        InserirLegenda(".legenda-veiculos ul", arg.Data.Grupos[i].Descricao, arg.Data.Grupos[i].Cor);
                    }
                }
                if (arg.Data.Categorias != undefined) {
                    for (var i = 0; i < arg.Data.Categorias.length; i++) {
                        InserirLegenda(".legenda-categorias ul", arg.Data.Categorias[i].Descricao, arg.Data.Categorias[i].Cor, arg.Data.Categorias[i].Cor);
                    }
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null, true);
}
function InserirLegenda(selector, text, total, backgroundColor, borderColor, className) {
    if (backgroundColor) {
        if (!borderColor) borderColor = 'none';
        if (!className) className = '';
        let mapDraw = new MapaDraw();

        $(selector).append('<li class="' + className + '" style="display: flex;padding: 8px; gap: 2px; cursor: context-menu; gap: 8px;"><div>' + mapDraw.icons.pinTracao(35, 35, backgroundColor, true) + '</div><span class="descricao-legenda" data-toggle="tooltip" data-placement="top" title="' + text + '">' + text + '</span></li>');
    } else {
        $(selector).append('<li>' + text + '</li>');
    }
}
function buscaGrupoStatusViagem(callback) {
    if (!_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramento || !_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem) {
        executarReST("MonitoramentoGrupoStatusViagem/BuscarTodos", null, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    var selected = [];
                    for (var i = 0; i < arg.Data.GrupoStatusViagem.length; i++) {
                        if (arg.Data.GrupoStatusViagem[i].selected == 'selected') {
                            selected.push(arg.Data.GrupoStatusViagem[i].value);
                        }
                    }
                    _pesquisaPosicaoFrota.GrupoStatusViagem.options(arg.Data.GrupoStatusViagem);
                    _pesquisaPosicaoFrota.GrupoStatusViagem.val(selected);

                    $("#" + _pesquisaPosicaoFrota.GrupoStatusViagem.id).selectpicker('refresh');

                    callback();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else {
        callback();
    }
}
function buscaGrupoTipoOperacao(callback) {
    executarReST("GrupoTipoOperacao/BuscarTodos", null, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _pesquisaPosicaoFrota.GrupoTipoOperacao.options(arg.Data.GrupoTipoOperacao);

                $("#" + _pesquisaPosicaoFrota.GrupoTipoOperacao.id).selectpicker('refresh');

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
function abrirBuscaFiltrosManual(e) {
    var buscaFiltros = new BuscarModeloFiltroPesquisa(e.ModeloFiltrosPesquisa, function (retorno) {

        if (retorno.Codigo !== 0) {
            e.ModeloFiltrosPesquisa.codEntity(retorno.Codigo);
            e.ModeloFiltrosPesquisa.val(retorno.ModeloDescricao);

            PreencherJsonFiltroPesquisa(_pesquisaPosicaoFrota, retorno.Dados);
        }
    },
        EnumCodigoFiltroPesquisa.PosicaoFrota);

    buscaFiltros.AbrirBusca();
}
function controlarExibicoesMapaEGrid(viewMode, contarQuantidadeCarrossel) {
    var mode = (typeof viewMode === 'string') ? viewMode : (viewMode ? 'map' : 'grid');

    $('#mapa-container').toggle(mode === 'map');
    $('#grid-container').toggle(mode === 'grid');
    $('#mapa-grid-container').toggle(mode === 'mix');

    if (mode === 'map' || mode === 'grid' || mode === 'mix') {
        $('#carousel-filters').show();
        $('#legenda-totais-container, #legenda-totais-container-mt').hide();
    } else {
        $('#carousel-filters').hide();
    }

    _gridPosicaoFrota.CarregarGrid(function () {
        $("#posicao-frota-container").show();
        $("#sem-dados-container").hide();

        if (mode === 'mix') {
            actionObterDadosMapaTabela(!!contarQuantidadeCarrossel);
        } else if (mode === 'map') {
            actionObterDadosMapa(!!contarQuantidadeCarrossel);
        }

        setTimeout(function () {
            var mapaAtual = (mode === 'mix') ? _novoMapaMT : _novoMapa;
            if (mapaAtual && mapaAtual.invalidateSize) mapaAtual.invalidateSize();
        }, 250);
    });
}


function salvarFiltrosModoTV(telaPrincipal) {
    if (telaPrincipal) {
        _pesquisaModoTv.VisualizarOrigens.val(_pesquisaTelaPrincipal.VisualizarOrigens.val());
        _pesquisaModoTv.VisualizarDestinos.val(_pesquisaTelaPrincipal.VisualizarDestinos.val());
        _pesquisaModoTv.VisualizarFrotas.val(_pesquisaTelaPrincipal.VisualizarFrotas.val());
        _pesquisaModoTv.MostrarVeiculosAgrupadosNoMapa.val(_pesquisaTelaPrincipal.MostrarVeiculosAgrupadosNoMapa.val());
    } else {
        _pesquisaTelaPrincipal.VisualizarOrigens.val(_pesquisaModoTv.VisualizarOrigens.val());
        _pesquisaTelaPrincipal.VisualizarDestinos.val(_pesquisaModoTv.VisualizarDestinos.val());
        _pesquisaTelaPrincipal.VisualizarFrotas.val(_pesquisaModoTv.VisualizarFrotas.val());
        _pesquisaTelaPrincipal.MostrarVeiculosAgrupadosNoMapa.val(_pesquisaModoTv.MostrarVeiculosAgrupadosNoMapa.val());
    }

    var preferencias = {
        Placa: _pesquisaTelaPrincipal.Placa.val(),
        Carga: _pesquisaTelaPrincipal.Carga.val(),
        PercentualViagem: _pesquisaTelaPrincipal.PercentualViagem.val(),
        PrevisaoEntregaAtualizada: _pesquisaTelaPrincipal.PrevisaoEntregaAtualizada.val(),
        EnderecoDaEntrega: _pesquisaTelaPrincipal.EnderecoDaEntrega.val(),
        Status: _pesquisaTelaPrincipal.Status.val(),
        Posicao: _pesquisaTelaPrincipal.Posicao.val(),
        TempoStatus: _pesquisaTelaPrincipal.TempoStatus.val(),
        RazaoSocialTransportador: _pesquisaTelaPrincipal.RazaoSocialTransportador.val(),
        Legendas: _pesquisaModoTv.Legendas.val(),
        VisualizarOrigens: _pesquisaModoTv.VisualizarOrigens.val(),
        VisualizarDestinos: _pesquisaModoTv.VisualizarDestinos.val(),
        VisualizarFrotas: _pesquisaModoTv.VisualizarFrotas.val(),
        FrequenciaAtualizacao: _pesquisaModoTv.FrequenciaAtualizacao()
    };

    localStorage.setItem('preferenciasMapa', JSON.stringify(preferencias));
    var isMapaTabela = $('#mapa-grid-container').is(':visible');
    if (isMapaTabela) {
        actionObterDadosMapaTabela(false);
    } else {
        actionObterDadosMapa(false);
    }
    aplicarVisibilidadeLegendas();
}

function carregarPreferenciasMapa() {
    preferenciasSalvas = localStorage.getItem('preferenciasMapa');
    if (preferenciasSalvas) {
        var preferencias = JSON.parse(preferenciasSalvas);
        _pesquisaModoTv.VisualizarOrigens.val(preferencias.VisualizarOrigens);
        _pesquisaModoTv.VisualizarDestinos.val(preferencias.VisualizarDestinos);
        _pesquisaModoTv.VisualizarFrotas.val(preferencias.VisualizarFrotas);
        _pesquisaModoTv.Legendas.val(preferencias.Legendas);
        _pesquisaModoTv.FrequenciaAtualizacao(preferencias.FrequenciaAtualizacao);
        _pesquisaTelaPrincipal.Placa.val(preferencias.Placa);
        _pesquisaTelaPrincipal.Carga.val(preferencias.Carga);
        _pesquisaTelaPrincipal.PercentualViagem.val(preferencias.PercentualViagem);
        _pesquisaTelaPrincipal.PrevisaoEntregaAtualizada.val(preferencias.PrevisaoEntregaAtualizada);
        _pesquisaTelaPrincipal.EnderecoDaEntrega.val(preferencias.EnderecoDaEntrega);
        _pesquisaTelaPrincipal.Status.val(preferencias.Status);
        _pesquisaTelaPrincipal.Posicao.val(preferencias.Posicao);
        _pesquisaTelaPrincipal.TempoStatus.val(preferencias.TempoStatus);
        _pesquisaTelaPrincipal.RazaoSocialTransportador.val(preferencias.RazaoSocialTransportador);
    }
}

function voltaAoPadrao() {
    localStorage.removeItem('preferenciasMapa');
    _pesquisaModoTv.Legendas.val(true);
    _pesquisaModoTv.VisualizarOrigens.val(true);
    _pesquisaModoTv.VisualizarDestinos.val(true);
    _pesquisaModoTv.VisualizarFrotas.val(true);
    _pesquisaTelaPrincipal.VisualizarOrigens.val(true);
    _pesquisaTelaPrincipal.VisualizarDestinos.val(true);
    _pesquisaTelaPrincipal.VisualizarFrotas.val(true);
    _pesquisaTelaPrincipal.Placa.val(true);
    _pesquisaTelaPrincipal.Carga.val(true);
    _pesquisaTelaPrincipal.PercentualViagem.val(true);
    _pesquisaTelaPrincipal.PrevisaoEntregaAtualizada.val(true);
    _pesquisaTelaPrincipal.EnderecoDaEntrega.val(true);
    _pesquisaTelaPrincipal.Status.val(true);
    _pesquisaTelaPrincipal.Posicao.val(true);
    _pesquisaTelaPrincipal.TempoStatus.val(true);
    _pesquisaTelaPrincipal.RazaoSocialTransportador.val(true);
    _pesquisaModoTv.FrequenciaAtualizacao('Não atualizar');

    actionObterDadosMapa(false);
}
function ObterDadosFrotaCarrossel(dados) {
    if (!dados) return;

    var listaLegenda = [];
    var keys = Object.keys(dados);
    for (var i = 0; i < keys.length; i++) {
        var k = keys[i];
        listaLegenda.push({
            Codigo: dados[k].Codigo,
            Descricao: dados[k].Descricao,
            Cor: dados[k].Cor,
            Quantidade: dados[k].Total
        });
    }

    var isFull = isFullscreen();
    var seletorAlvo = isFull ? 'view-selection-items-fullscreen' : 'view-selection-items';
    $('#' + seletorAlvo).empty();

    for (var j = 0; j < listaLegenda.length; j++) {
        InserirItensFrotaCarrossel(seletorAlvo, listaLegenda[j].Codigo, listaLegenda[j].Descricao, listaLegenda[j].Cor, listaLegenda[j].Quantidade, (j + 1) < listaLegenda.length);
    }

    aplicarVisibilidadeLegendas();
}
function InserirItensFrotaCarrossel(idSelector, id, descricao, cor, quantidade, pintarBorda) {
    var borderStyle = pintarBorda ? 'border-right: 3px solid #CDCDCD;' : '';
    var html = '<div class="view-selection" style="' + borderStyle + '" onclick="FiltrarOpcoesFrotaCarroselClick(' + id + ')" data-toggle="tooltip" data-placement="top" title="' + descricao + '">' +
        '<div id="' + id + '" class="view-select-button">' +
        '<div class="icon-number" style="color:' + cor + '">' + quantidade + '</div>' +
        '<span>' + descricao + '</span>' +
        '</div></div>';
    $('#' + idSelector).append(html);
}

function InserirLegendaReboque(selector, text) {
    $(selector).html("");

    let mapDraw = new MapaDraw();
    $(selector).append('<li style="display: flex;padding: 8px; cursor: context-menu; gap: 8px;">' + mapDraw.icons.pinReboque(true) + '<span class="descricao-legenda" data-toggle="tooltip" data-placement="top" title="' + text + '">' + text + '</span></li>');
}

function InserirLegendaLocais(selector, text, clienteComVeiculosEmAlvo, ClientesAlvosEstrategicos) {
    $(selector).html("");

    let mapDraw = new MapaDraw();
    $(selector).append('<li style="display: flex;padding: 8px; cursor: context-menu; gap: 8px;">' + mapDraw.icons.PinMapLocal(true) + '<span class="descricao-legenda" data-toggle="tooltip" data-placement="top" title="' + text + '">' + text + '</span></li>');

    if (clienteComVeiculosEmAlvo || ClientesAlvosEstrategicos) {
        text = "Cliente"
        $(selector).append(
            '<li style="display: flex; padding: 8px; cursor: context-menu; gap: 8px;">' +
            '<img src="' + mapDraw.icons.Cliente(true) + '">' +
            '<span class="descricao-legenda" data-toggle="tooltip" data-placement="top" title="' + text + '">' + text + '</span>' +
            '</li>'
        );
    }
}

function FiltrarOpcoesFrotaCarroselClick(codigoStatus) {
    if ($('#' + codigoStatus).hasClass("active")) {
        LimparCampo(_pesquisaPosicaoFrota.GrupoStatusViagem);
        LimparCampo(_pesquisaPosicaoFrota.GrupoTipoOperacao);

        $(".view-select-button.active").removeClass('active');
        controlarExibicoesMapaEGrid(getMode(), true);
        return;
    }

    $(".view-select-button.active").removeClass('active');
    $('#' + codigoStatus).addClass('active');

    if (codigoStatus == 0) codigoStatus = -1;

    if (_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramento)
        _pesquisaPosicaoFrota.GrupoTipoOperacao.val([codigoStatus]);
    else
        _pesquisaPosicaoFrota.GrupoStatusViagem.val([codigoStatus]);

    controlarExibicoesMapaEGrid(getMode(), false);
}

function ObterQuantidadeInformacoesVeiculoCarrossel(data) {
    //LimparCampo(_pesquisaPosicaoFrota.GrupoStatusViagem);
    //LimparCampo(_pesquisaPosicaoFrota.GrupoTipoOperacao);

    var listaVeiculosDisponiveis = [];

    if (data) {
        listaVeiculosDisponiveis = Object.groupBy(data, (x) => x.SituacaoVeiculo);

        if (listaVeiculosDisponiveis['Disponível']) {
            var quantidade = listaVeiculosDisponiveis['Disponível'].length;

            _cabecalhoPosicaoFrota.QuantidadeVeiculosDisponiveis.val(quantidade);
            $('#quantidade-veiculos-disponiveis').text(quantidade);
        } else {
            _cabecalhoPosicaoFrota.QuantidadeVeiculosDisponiveis.val(0);
            $('#quantidade-veiculos-disponiveis').text(0);
        }

    }
}

function ObterOnlineOffline(data) {
    const listaFarolEspelhamentoOnline = data.filter(x => x.Rastreador);
    const listaFarolEspelhamentoOffline = data.filter(x => !x.Rastreador);

    _cabecalhoPosicaoFrota.QuantidadeOnline.val(listaFarolEspelhamentoOnline.length);
    _cabecalhoPosicaoFrota.QuantidadeOffline.val(listaFarolEspelhamentoOffline.length);
}
function PintarOpcaoAtivaOnlineOffline(data) {
    $(".view-select-button.active").removeClass('active');
    $('#view-' + data).addClass('active');
}

function desenharCarrosselFiltro(seletor, descricao, id, cor, quantidade) {
    if (!cor)
        cor = "#3EA7DB";

    $('#' + seletor).append(
        '<div class="view-selection" id="view-select-filters" onclick="FiltrarOpcoesFiltroCarroselClick(' + id + ')" data-toggle="tooltip" data-placement="top" title="' + descricao + '">' +
        '  <div id="' + id + '" class="view-select-button">' +
        '     <div class="icon-number" style="color:' + cor + '">' + quantidade + '</div>' +
        '     <div style="width: 84px;overflow: hidden;text-overflow: ellipsis;white-space: nowrap; text-align: center;">' + descricao + '</div>' +
        '   </div>' +
        '</div>'
    );
}

function showTooltip(event, text) {
    const tooltip = document.getElementById('tooltip');
    tooltip.innerText = text;
    tooltip.style.display = 'block';
    tooltip.style.left = event.clientX + 'px';
    tooltip.style.top = event.clientY + 'px';
}

function hideTooltip() {
    const tooltip = document.getElementById('tooltip');
    tooltip.style.display = 'none';
}

function desenharSubAreas(data, mapaTipo) {
    var mapRef = (mapaTipo && mapaTipo.map) ? mapaTipo.map : _novoMapa;
    if (!data || !data.length || !mapRef) return;

    for (var i = 0; i < data.length; i++) {
        if (!data[i] || !data[i].Area) continue;

        var areaJson;
        try { areaJson = JSON.parse(data[i].Area); } catch (e) { continue; }
        if (!areaJson || !areaJson.length || !areaJson[0]) continue;

        var a0 = areaJson[0];
        var corPolygon = data[i].Cor || a0.strokeColor || a0.fillColor || "#FF0000";

        if ((a0.type === "polygon" || a0.type === google.maps.drawing.OverlayType.POLYGON) && a0.paths) {
            var paths = a0.paths.map(function (coord) { return [coord.lat, coord.lng]; });
            var subAreaLayer = L.polygon(paths, { color: a0.strokeColor || corPolygon, fillColor: a0.fillColor || corPolygon, fillOpacity: (typeof a0.fillOpacity === 'number') ? a0.fillOpacity : 0.5, weight: a0.strokeWeight || 2, stroke: true }).addTo(mapRef);
            _subAreasFiliais.push(subAreaLayer);
        } else if (a0.type === google.maps.drawing.OverlayType.RECTANGLE && a0.bounds) {
            var b = a0.bounds;
            var subAreaLayer = L.polygon([[b.south, b.west], [b.south, b.east], [b.north, b.east], [b.north, b.west]], { color: a0.strokeColor || corPolygon, fillColor: a0.fillColor || corPolygon, fillOpacity: (typeof a0.fillOpacity === 'number') ? a0.fillOpacity : 0.5, weight: a0.strokeWeight || 2, stroke: true }).addTo(mapRef);
            _subAreasFiliais.push(subAreaLayer);
        }
    }
}
function tryParseJson(str) {
    if (!str) return null;
    if (typeof str !== 'string') return str;
    try {
        return JSON.parse(str);
    } catch (e) {
        return null;
    }
}

var _vmSidebarMT = null;

function findMarkerByVeiculoId(id, group) {
    if (!group || !group.getLayers) return null;
    var layers = group.getLayers();
    for (var i = 0; i < layers.length; i++) {
        var l = layers[i];
        if (l && l.options && l.options.codigoVeiculo === id) return l;
    }
    return null;
}

function loadSidebarMapaTabela() {
    if (!window.ko) return;

    const el = document.getElementById('lista-frota-container-mt');
    if (!el || el.getAttribute('data-ko-bound') === '1') return;

    function sidebarFrotaMapaTabela() {
        const self = this;
        self.query = ko.observable('');
        self.sortMode = ko.observable('placa');
        self.items = ko.observableArray([]);

        self.orderAsc = ko.observable(false);
        self.toggleOrder = function () { self.orderAsc(!self.orderAsc()); };
        self.ordered = ko.pureComputed(function () {
            const base = self.filtered().slice();
            base.forEach(function (v, i) { if (typeof v.idx !== 'number') v.idx = i; });
            return base.sort(function (a, b) {
                return self.orderAsc() ? (a.idx - b.idx) : (b.idx - a.idx);
            });
        });

        self.filtered = ko.pureComputed(function () {
            const q = (self.query() || '').trim().toLowerCase();
            let arr = self.items();
            if (q) arr = arr.filter(v =>
                (v.placa || '').toLowerCase().includes(q) ||
                (v.resumo || '').toLowerCase().includes(q)
            );

            if (self.sortMode() === 'status') {
                arr = arr.slice().sort((a, b) => {
                    if (a.online === b.online) return (a.placa || '').localeCompare(b.placa || '');
                    return a.online ? -1 : 1;
                });
            } else {
                arr = arr.slice().sort((a, b) => (a.placa || '').localeCompare(b.placa || ''));
            }
            return arr;
        });

        self.verNoMapa = function (v) {
            const map = window._novoMapaMT;
            const group = window._markersMT;
            $('.pf-card--compact').removeClass('is-focused');
            $(event.currentTarget).closest('.pf-card--compact').addClass('is-focused');
            if (!map || !v) return;

            const marker = findMarkerByVeiculoId(v.id, group);

            const abrirPopupAposCentralizar = function (marker, ll) {
                map.off('moveend.vernomapa');

                map.once('moveend.vernomapa', function () {
                    setTimeout(function () {
                        if (marker && marker.openPopup) {
                            marker.openPopup();
                            const p = marker.getPopup();
                            if (p) clickMarkerVeiculoGenerico(p, map);
                        } else if (ll) {
                            const html = `<!--${v.id}--><div id="InfoWindowVeiculo"><div class="placa">${v.placa || ''}</div>Carregando...</div>`;
                            const p = L.popup({ offset: [12, 0] }).setLatLng(ll).setContent(html).openOn(map);
                            clickMarkerVeiculoGenerico(p, map);
                        }
                    }, 100);
                });
            };

            if (marker) {
                const ll = marker.getLatLng();

                if (group && typeof group.zoomToShowLayer === 'function') {
                    group.zoomToShowLayer(marker, function () {
                        setTimeout(function () {
                            abrirPopupAposCentralizar(marker, ll);
                            map.setView(ll, 16, { animate: true, duration: 0.5 });
                        }, 300);
                    });
                } else {
                    abrirPopupAposCentralizar(marker, ll);
                    map.setView(ll, 16, { animate: true, duration: 0.5 });
                }
            } else if (v.lat && v.lng) {
                const ll = L.latLng(v.lat, v.lng);
                abrirPopupAposCentralizar(null, ll);
                map.setView(ll, 16, { animate: true, duration: 0.5 });
            }
        };
    }

    _vmSidebarMT = new sidebarFrotaMapaTabela();
    ko.applyBindings(_vmSidebarMT, el);
    el.setAttribute('data-ko-bound', '1');

    const wrap = document.getElementById('pf-wrap-mt');
    const handle = wrap ? wrap.querySelector('.pf-handle') : null;
    const icon = handle ? handle.querySelector('i') : null;

    if (!wrap || !handle) return;

    // Remove evento anterior e adiciona novo
    handle.onclick = null;
    handle.addEventListener('click', function () {
        const isCurrentlyClosed = wrap.classList.contains('is-closed');

        if (isCurrentlyClosed) {
            // Abre a sidebar
            wrap.classList.remove('is-closed');
            if (icon) {
                icon.classList.remove('fa-chevron-right');
                icon.classList.add('fa-chevron-left');
            }
            setTimeout(function () {
                if (window._novoMapaMT && _novoMapaMT.invalidateSize)
                    _novoMapaMT.invalidateSize();
            }, 260);
        } else {
            // Fecha a sidebar
            wrap.classList.add('is-closed');
            if (icon) {
                icon.classList.remove('fa-chevron-left');
                icon.classList.add('fa-chevron-right');
            }
        }
    });
}
function updateSidebarFrotaFromVeiculos(veiculos) {
    if (!_vmSidebarMT) return;

    var mapDraw = new MapaDraw();

    const itens = Array.isArray(veiculos) ? veiculos.map(function (v, i) {
        return {
            id: v.CodigoVeiculo,
            placa: v.PlacaVeiculo || '-',
            resumo: (v.Status || v.SituacaoVeiculo || '').toString(),
            online: !!v.Rastreador,
            lat: v.Latitude,
            lng: v.Longitude,
            idx: i,
            wifi: v.Rastreador ? mapDraw.icons.wifiOn() : mapDraw.icons.wifiOff()
        };
    }) : [];

    _vmSidebarMT.items(itens);
}