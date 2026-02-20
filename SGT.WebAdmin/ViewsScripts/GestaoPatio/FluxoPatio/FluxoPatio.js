/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/AreaVeiculo.js" />
/// <reference path="../../Consultas/AreaVeiculoPosicao.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/TipoCarregamento.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEtapaFluxoGestaoPatio.js" />
/// <reference path="../../Enumeradores/EnumTipoAreaVeiculo.js" />
/// <reference path="../../Enumeradores/EnumTipoFluxoGestaoPatio.js" />
/// <reference path="../../Enumeradores/EnumLimiteCarregamentosCentroCarregamento.js" />
/// <reference path="../../Enumeradores/EnumOrdenacao.js" />
/// <reference path="CancelamentoFluxoPatio.js" />
/// <reference path="EtapasFluxoGestao.js" />
/// <reference path="MontaEtapa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaFluxoPatio;
var _fluxoAtual;
var _knoutsFluxosGestaoPatio = {};
var _configuracaoGestaoPatio = {};
var _itensPorPagina = 10;
var _audioElementFluxoPatio;
var _timerIntervalFluxoPatio = null;
var _quantidadeFluxosEmitemAlertaSom = 0;
var _resumoCapacidadeDocas;

var PesquisaFluxoPatio = function () {
    var tipoPadrao = _CONFIGURACAO_TMS.GerarFluxoPatioDestino ? EnumTipoFluxoGestaoPatio.Todos : EnumTipoFluxoGestaoPatio.Origem;

    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.GestaoPatio.FluxoPatio.Filial.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: ko.observable(Localization.Resources.GestaoPatio.FluxoPatio.DataInicial.getFieldDescription()), getType: typesKnockout.dateTime, def: Global.DataAtual(), val: ko.observable(Global.DataAtual()) });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.DataFinal.getFieldDescription(), getType: typesKnockout.dateTime });
    this.EtapaFluxoGestaoPatio = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: Localization.Resources.GestaoPatio.FluxoPatio.Etapa.getFieldDescription(), options: ko.observableArray([]) });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: ko.observable(Localization.Resources.GestaoPatio.FluxoPatio.NumeroCarga.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroNotaFiscal = PropertyEntity({ text: ko.observable(Localization.Resources.GestaoPatio.FluxoPatio.NumeroNotaFiscal.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Pedido = PropertyEntity({ text: ko.observable(Localization.Resources.GestaoPatio.FluxoPatio.Pedido.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.PreCarga = PropertyEntity({ text: ko.observable(Localization.Resources.GestaoPatio.FluxoPatio.PreCarga.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Placa = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Placa.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Ordenacao = PropertyEntity({ val: ko.observable(EnumOrdenacao.Decrescente), options: EnumOrdenacao.ObterOpcoes(), text: Localization.Resources.Gerais.Geral.Ordenacao.getFieldDescription(), def: EnumOrdenacao.Decrescente });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoEtapaFluxoGestaoPatio.Todas), options: EnumSituacaoEtapaFluxoGestaoPatio.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: EnumSituacaoEtapaFluxoGestaoPatio.Todas });
    this.Tipo = PropertyEntity({ val: ko.observable(tipoPadrao), options: EnumTipoFluxoGestaoPatio.obterOpcoesPesquisa(), text: Localization.Resources.GestaoPatio.FluxoPatio.Tipo.getFieldDescription(), def: tipoPadrao, visible: _CONFIGURACAO_TMS.GerarFluxoPatioDestino });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.GestaoPatio.FluxoPatio.TipoDeOperacao.getFieldDescription(), idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.GestaoPatio.FluxoPatio.Destinatario.getFieldDescription(), idBtnSearch: guid() });
    this.Remetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.GestaoPatio.FluxoPatio.Remetente.getFieldDescription(), idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.GestaoPatio.FluxoPatio.TipoDeCarga.getFieldDescription(), idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.GestaoPatio.FluxoPatio.Motorista.getFieldDescription(), idBtnSearch: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.GestaoPatio.FluxoPatio.ModeloVeicularDaCarga.getFieldDescription(), idBtnSearch: guid() });
    this.AreaVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.GestaoPatio.FluxoPatio.AreaVeiculo.getFieldDescription(), idBtnSearch: guid() });
    this.LocalCarregamento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.GestaoPatio.FluxoPatio.LocalDeCarregamento.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.NumeroNfProdutor = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: Localization.Resources.GestaoPatio.FluxoPatio.NumeroNotaFiscalProdutor.getFieldDescription(), visible: ko.observable(false) });
    this.TipoCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.GestaoPatio.FluxoPatio.TipoCarregamento.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.GestaoPatio.FluxoPatio.Transportador.getFieldDescription(), idBtnSearch: guid() });
    this.DataInicialChegadaVeiculo = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.DataInicialChegadaVeiculo.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataFinalChegadaVeiculo = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.DataFinalChegadaVeiculo.getFieldDescription(), getType: typesKnockout.dateTime });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.DataInicialChegadaVeiculo.dateRangeLimit = this.DataFinalChegadaVeiculo;
    this.DataFinalChegadaVeiculo.dateRangeInit = this.DataInicialChegadaVeiculo;

    this.Tipo.val.subscribe(recarregarSituacoesFiliais);

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            pesquisarFluxoCarga(1, false);
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            }
            else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            }
            else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ConfiguracaoAlerta = PropertyEntity({
        eventClick: function () {
            abrirConfiguracaoAlertaClick();
        }, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ConfiguracaoAlerta, idGrid: guid(), visible: ko.observable(false)
    });
};

var ResumoCapacidadeDocas = function () {
    this.ExibirResumo = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ListaDadosResumo = ko.observableArray();

    this.ExibirDadosResumo = PropertyEntity({
        eventClick: function (e) {
            e.ExibirDadosResumo.visibleFade(!e.ExibirDadosResumo.visibleFade());
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(true)
    });
};

function LoadEtapas() {
    loadTravamentoChave(false, _configuracaoGestaoPatio, OnLoadTravamentoChave);
    loadCheckList(false, _configuracaoGestaoPatio, OnLoadDocaCheckList, EnumEtapaChecklist.Checklist);
    loadCheckList(false, _configuracaoGestaoPatio, OnLoadDocaCheckList, EnumEtapaChecklist.AvaliacaoDescarga);
    loadDocaCarregamento(false, _configuracaoGestaoPatio, OnLoadDocaCarregamento);
    LoadGuaritaFluxoPatio();
    LoadExpedicaoFluxoPatio();
    LoadPosicao();
    LoadPesagemFluxoPatio();
    LoadDocumentosPesagem();
    LoadDeslocamentoPatio();
    LoadChegadaSaidaLojaLoja();
    LoadFimViagem();
    loadInicioHigienizacaoFluxoPatio();
    loadFimHigienizacaoFluxoPatio();
    loadInicioCarregamentoFluxoPatio();
    loadFimCarregamentoFluxoPatio();
    loadSolicitacaoVeiculoFluxoPatio();
    loadDadosTransporte();
    loadCancelamentoFluxoPatio();
    loadObservacoesEtapas();
    loadInicioDescarregamentoFluxoPatio();
    loadFimDescarregamentoFluxoPatio();
    loadDocumentoFiscalFluxoPatio();
    loadDocumentosTransporteFluxoPatio();
    loadMontagemCargaPatio();
    loadSeparacaoMercadoria();
    loadFaturamentoFluxoPatio();
    loadDetalhePedido();
    loadFluxoPatioEnvioNotificacaoSMS();
    loadInformarEquipamentoFluxoPatio();
    loadEnviarNotificacaoApp(EnumTipoNotificacaoMotoristaSMS.GestaoDePatioEnvioManual);

    ControleCamposPesquisa();

    if (_configuracaoGestaoPatio.ObrigatorioInformarDataInicial) {
        _pesquisaFluxoPatio.DataInicial.text(Localization.Resources.GestaoPatio.FluxoPatio.DataInicial.getRequiredFieldDescription());
        _pesquisaFluxoPatio.DataInicial.val("");
        _pesquisaFluxoPatio.DataInicial.def = "";
        _pesquisaFluxoPatio.DataInicial.required = true;
    }
}

function ControleCamposPesquisa() {
    if (_configuracaoGestaoPatio.HabilitarPreCarga) {
        _pesquisaFluxoPatio.CodigoCargaEmbarcador.text(Localization.Resources.GestaoPatio.FluxoPatio.CargaRomaneio.getFieldDescription());
        _pesquisaFluxoPatio.PreCarga.text(Localization.Resources.GestaoPatio.FluxoPatio.PreCargaRota.getFieldDescription());
        _pesquisaFluxoPatio.Pedido.text(Localization.Resources.GestaoPatio.FluxoPatio.PedidoMissao.getFieldDescription());
    }
    else
        _pesquisaFluxoPatio.PreCarga.visible(false);
    obterConfiguracoesTipoCarregamento()

    _pesquisaFluxoPatio.LocalCarregamento.visible(_configuracaoGestaoPatio.InformarDocaCarregamentoUtilizarLocalCarregamento);
}

function loadFluxoPatio() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {
            carregarHTMLModaisFluxoPatio(function () {
                loadEtapasFluxoGestao().then(function () {
                    CarregarConfiguracoesGestaoPatio(LoadEtapas);
                    _pesquisaFluxoPatio = new PesquisaFluxoPatio();
                    KoBindings(_pesquisaFluxoPatio, "knoutPesquisaFluxoPatio", false, _pesquisaFluxoPatio.Pesquisar.id);

                    _resumoCapacidadeDocas = new ResumoCapacidadeDocas();
                    KoBindings(_resumoCapacidadeDocas, "knockoutResumoCapadidadeDocas");

                    _pesquisaFluxoPatio.Placa.get$().mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

                    new BuscarFilial(_pesquisaFluxoPatio.Filial, obterConfiguracaoSequenciaGestaoPatio);
                    new BuscarTiposOperacao(_pesquisaFluxoPatio.TipoOperacao);
                    new BuscarClientes(_pesquisaFluxoPatio.Destinatario);
                    new BuscarClientes(_pesquisaFluxoPatio.Remetente);
                    new BuscarTiposdeCarga(_pesquisaFluxoPatio.TipoCarga);
                    new BuscarMotoristas(_pesquisaFluxoPatio.Motorista);
                    new BuscarModelosVeicularesCarga(_pesquisaFluxoPatio.ModeloVeicularCarga);
                    new BuscarAreaVeiculo(_pesquisaFluxoPatio.AreaVeiculo);
                    new BuscarAreaVeiculoPosicao(_pesquisaFluxoPatio.LocalCarregamento, null, null, null, null, EnumTipoAreaVeiculo.Doca);
                    new BuscarTipoCarregamento(_pesquisaFluxoPatio.TipoCarregamento);
                    new BuscarTransportadores(_pesquisaFluxoPatio.Transportador);


                    _pesquisaFluxoPatio.Filial.multiplesEntities.subscribe(recarregarSituacoesFiliais);
                    _pesquisaFluxoPatio.Filial.multiplesEntities.subscribe(VisibilidadeBotoesPorFilial);

                    BuscarFilialPadrao();

                    _audioElementFluxoPatio = document.createElement('audio');
                    _audioElementFluxoPatio.setAttribute('src', 'sound/voice_on.mp3');
                    _audioElementFluxoPatio.addEventListener("load", function () {
                        _audioElementFluxoPatio.play();
                    }, true);

                    loadFluxoPatioSignalR();
                });
            });
        });
    });
}

function carregarHTMLModaisFluxoPatio(callback) {
    $.get("Content/Static/GestaoPatio/FluxoPatioModais.html?dyn=" + guid(), function (data) {
        $("#modaisFluxoPatio").html(data);

        callback();
    });
}

function BuscarFluxoPatioPorCodigo(fluxoPatio, requisicaoOculta) {
    var data = { Codigo: fluxoPatio.Codigo.val() };
    var exibirRequisicao = !requisicaoOculta;

    executarReST("FluxoPatio/BuscarPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                if (window._viewTabelada)
                    PreencherObjetoKnout(fluxoPatio, arg);
                else {
                    PreencherFluxoPatio(fluxoPatio, arg.Data);
                    iniciarTimerSonoro();
                }

                if (fluxoPatio.SituacaoEtapaFluxoGestaoPatio === EnumSituacaoEtapaFluxoGestaoPatio.Cancelado)
                    pesquisarFluxoCarga(0, false);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null, exibirRequisicao);
}

function atualizarFluxoPatio() {
    if (_fluxoAtual != null) {
        BuscarFluxoPatioPorCodigo(_fluxoAtual);
    }
}

function BuscarFilialPadrao() {
    executarReST("DadosPadrao/ObterFilial", {}, function (r) {
        if (r.Success && r.Data) {
            var data = r.Data;

            _pesquisaFluxoPatio.Filial.multiplesEntities([]);
            _pesquisaFluxoPatio.Filial.multiplesEntities([{ Codigo: data.Codigo, Descricao: data.Descricao }]);

            pesquisarFluxoCarga(0, false);
        }
    });
}

function pesquisarFluxoCarga(page, paginou) {
    if (!ValidarCamposObrigatorios(_pesquisaFluxoPatio)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var data = RetornarObjetoPesquisa(_pesquisaFluxoPatio);

    data.inicio = _itensPorPagina * (page - 1);
    data.limite = _itensPorPagina;

    limparTimerSonoro();

    executarReST("FluxoPatio/ObterFluxoPatio", data, function (arg) {
        if (arg.Success) {
            $("#wid-id-4").show();

            if (arg.Data !== false) {
                _knoutsFluxosGestaoPatio = {};
                $("#gestaoPatio").html("");

                for (var i = 0; i < arg.Data.length; i++) {
                    var fluxoCarregamento = arg.Data[i];

                    if (fluxoCarregamento.Etapas.length > 0) {
                        var resultado = 100 / fluxoCarregamento.Etapas.length;
                        var etapaFluxoGestaoPatio = new EtapaFluxoGestaoPatio();

                        etapaFluxoGestaoPatio.TamanhoEtapa.val(resultado.toString().replace(",", ".") + "%");
                        PreencherFluxoPatio(etapaFluxoGestaoPatio, fluxoCarregamento);

                        if (_configuracaoGestaoPatio.ExibirDetalhesIdentificacaoFluxo) {
                            etapaFluxoGestaoPatio.Placas.visible(false);
                            etapaFluxoGestaoPatio.Remetente.visible(Boolean(etapaFluxoGestaoPatio.Remetente.val()));
                            etapaFluxoGestaoPatio.DataCarga.visible(true);
                            etapaFluxoGestaoPatio.Doca.visible(etapaFluxoGestaoPatio.Doca.val());
                        }
                        else if (arg.Data[i].LimiteCarregamentos == EnumLimiteCarregamentosCentroCarregamento.QuantidadeDocas)
                            etapaFluxoGestaoPatio.DataCarga.visible(true);

                        etapaFluxoGestaoPatio.Transportador.visible(!_configuracaoGestaoPatio.OcultarTransportador);
                        etapaFluxoGestaoPatio.TipoOperacao.visible(_configuracaoGestaoPatio.IdentificacaoFluxoExibirTipoOperacao);
                        etapaFluxoGestaoPatio.Destinatario.visible(_configuracaoGestaoPatio.IdentificacaoFluxoExibirOrigemXDestinos);
                        etapaFluxoGestaoPatio.AntecipacaoICMS.visible(etapaFluxoGestaoPatio.AntecipacaoICMS.val());
                        etapaFluxoGestaoPatio.ModeloVeicularCargaVeiculo.visible(Boolean(etapaFluxoGestaoPatio.ModeloVeicularCargaVeiculo.val()) && _configuracaoGestaoPatio.IdentificacaoFluxoExibirModeloVeicularCargaVeiculo);
                        etapaFluxoGestaoPatio.AreaVeiculo.visible(Boolean(etapaFluxoGestaoPatio.AreaVeiculo.val()));
                        etapaFluxoGestaoPatio.Equipamento.visible(Boolean(etapaFluxoGestaoPatio.Equipamento.val()));

                        var idFluxo = fluxoCarregamento.Codigo + "_FluxoGestaoPatio"
                        $("#gestaoPatio").append(_HTMLEtapasFluxoGestaoPatio.replace(/#idFluxoGestaoPatio/g, idFluxo));
                        KoBindings(etapaFluxoGestaoPatio, idFluxo);
                        _knoutsFluxosGestaoPatio[fluxoCarregamento.Codigo] = etapaFluxoGestaoPatio;
                    }
                }

                $('[data-toggle="tooltip"]').tooltip({
                    container: 'body'
                });

                if (!paginou)
                    ComponentePaginacao(arg.QuantidadeRegistros);

                iniciarTimerSonoro();
                buscarCapacidadeCarregamentoPorDocas();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}

function ComponentePaginacao(totalRegistros) {
    var clicouNoPaginar = false;
    if (totalRegistros > 0) {
        $("#divPaginationGestaoPatio").html('<ul style="float:right" id="paginacaoFluxoPatio" class="pagination"></ul>');
        var paginas = Math.ceil((totalRegistros / _itensPorPagina));
        $('#paginacaoFluxoPatio').twbsPagination({
            first: Localization.Resources.Gerais.Geral.Primeiro,
            prev: Localization.Resources.Gerais.Geral.Anterior,
            next: Localization.Resources.Gerais.Geral.Proximo,
            last: Localization.Resources.Gerais.Geral.Ultimo,
            totalPages: paginas,
            visiblePages: 5,
            onPageClick: function (event, page) {
                if (clicouNoPaginar)
                    pesquisarFluxoCarga(page, true);
                clicouNoPaginar = true;
            }
        });
    }
    else
        $("#divPaginationGestaoPatio").html('<span>' + Localization.Resources.Gerais.Geral.NenhumRegistroEncontrado + '</span>');
}

function PreencherFluxoPatio(etapaFluxoGestaoPatio, fluxoCarregamento) {
    var knoutEtapas = [];

    fluxoCarregamento.Etapas.forEach(function (etapa, i) {
        var objetoEtapa = MontaEtapa(fluxoCarregamento, etapa, etapaFluxoGestaoPatio);

        if (objetoEtapa == null)
            return console.error(Localization.Resources.GestaoPatio.FluxoPatio.NaoExisteImplementacaoParaEtapaFluxoGestaoPatio.format(etapa.EtapaFluxoGestaoPatio));

        objetoEtapa.cssClass = ko.observable("");
        objetoEtapa.tempoExcedente = etapa.TempoExcedente;

        setarSituacaoEtapa(fluxoCarregamento, etapa, objetoEtapa, i);

        knoutEtapas.push(objetoEtapa);

        if (etapa.AlertaSonoro)
            _quantidadeFluxosEmitemAlertaSom = _quantidadeFluxosEmitemAlertaSom + 1;
    });

    etapaFluxoGestaoPatio.KnoutEtapas.val(knoutEtapas);
    PreencherObjetoKnout(etapaFluxoGestaoPatio, { Data: fluxoCarregamento });

    etapaFluxoGestaoPatio.MensagensAlerta.val(fluxoCarregamento.MensagensAlerta);
    etapaFluxoGestaoPatio.MensagemAlertaComBloqueio.val(isPossuiMensagemAlertaFluxoPatioComBloqueio(fluxoCarregamento.MensagensAlerta));

    if (fluxoCarregamento.SeparacaoMercadoriaConfirmada)
        etapaFluxoGestaoPatio.PercentualSeparacaoMercadoria.cssClass("progress-bar bg-color-teal");
}

function setarSituacaoEtapa(fluxoCarregamento, etapa, objetoEtapa, etapaFluxo) {
    if (fluxoCarregamento.EtapaAtual > etapaFluxo) {
        if ((etapa.EtapaFluxoGestaoPatio == EnumEtapaFluxoGestaoPatio.Faturamento) && !fluxoCarregamento.FaturamentoFinalizado)
            SetarEtapaFluxoAprovadaComPendencia(objetoEtapa);
        else
            SetarEtapaFluxoAprovada(objetoEtapa);
    }
    else if (fluxoCarregamento.EtapaAtual == etapaFluxo) {
        if (!fluxoCarregamento.PossuiCarga && !EnumEtapaFluxoGestaoPatio.isEtapaHabilitadaPreCarga(etapa.EtapaFluxoGestaoPatio))
            SetarEtapaFluxoDesabilitada(objetoEtapa);
        else if (fluxoCarregamento.SituacaoEtapaFluxoGestaoPatio == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando) {
            if (etapa.AlertaVisual)
                SetarEtapaFluxoAguardandoAlertaVisual(objetoEtapa);
            else
                SetarEtapaFluxoAguardando(objetoEtapa);
        }
        else if (fluxoCarregamento.SituacaoEtapaFluxoGestaoPatio == EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado || fluxoCarregamento.SituacaoEtapaFluxoGestaoPatio == EnumSituacaoEtapaFluxoGestaoPatio.Cancelado)
            SetarEtapaFluxoProblema(objetoEtapa);
        else
            SetarEtapaFluxoAprovada(objetoEtapa);
    }
    else if (objetoEtapa.etapaLiberada)
        SetarEtapaFluxoAguardando(objetoEtapa);
    else if (etapa.PossibilidadePreencherEtapaBloqueada) {
        SetarEtapaFluxoDesbloqueada(objetoEtapa);
    }
    else {
        SetarEtapaFluxoDesabilitada(objetoEtapa);

        if (objetoEtapa.permanenciaMaximaExcedida)
            SetarEtapaFluxoPermanenciaMaximaExcedida(objetoEtapa);
    }
}

function CarregarConfiguracoesGestaoPatio(cb) {
    executarReST("FluxoPatio/ConfiguracoesGestaoPatio", {}, function (arg) {
        if (arg.Success && arg.Data !== false) {
            _configuracaoGestaoPatio = arg.Data;
            cb();
        }
    });
}

function recarregarSituacoesFiliais() {
    var filiais = recursiveMultiplesEntities(_pesquisaFluxoPatio.Filial);

    if (filiais.length == 0) {
        _pesquisaFluxoPatio.EtapaFluxoGestaoPatio.options([]);
        SetarSelectMultiple(_pesquisaFluxoPatio.EtapaFluxoGestaoPatio);
        return;
    }

    executarReST("FluxoPatio/ObterMultiplasEtapasDisponiveis", { Filiais: JSON.stringify(filiais), Tipo: _pesquisaFluxoPatio.Tipo.val() }, function (retorno) {
        if (retorno.Success && retorno.Data) {
            var etapasFluxoPatio = [];

            for (var i = 0; i < retorno.Data.length; i++) {
                etapasFluxoPatio.push({
                    text: retorno.Data[i].Descricao,
                    value: retorno.Data[i].Enumerador
                });
            }

            _pesquisaFluxoPatio.EtapaFluxoGestaoPatio.options(etapasFluxoPatio);
            SetarSelectMultiple(_pesquisaFluxoPatio.EtapaFluxoGestaoPatio);
        }
    });
}

function VisibilidadeBotoesPorFilial(codigos) {
    if (codigos.length == 1 && _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador)
        _pesquisaFluxoPatio.ConfiguracaoAlerta.visible(true);
    else
        _pesquisaFluxoPatio.ConfiguracaoAlerta.visible(false);
}

function ObterDetalhesCargaFluxoClick(e, sender) {
    if (_configuracaoGestaoPatio.OcultarFluxoCarga || (e.Carga.val() == 0))
        return;

    ObterDetalhesCargaFluxo(e.Carga.val());
}

function ObterDetalhesCargaFluxo(codigo) {
    var data = { Carga: codigo };
    executarReST("Carga/BuscarCargaPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                obterTiposIntegracaoCargaTransportador();
                $("#fdsCarga").html('<button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true" style="position: absolute; z-index: 9999; right: 18px; top: 6px;"><i class="fal fa-times"></i></button>');
                var knoutCarga = GerarTagHTMLDaCarga("fdsCarga", arg.Data, null, false);
                $("#fdsCarga .container-carga").addClass("mb-0");
                _cargaAtual = knoutCarga;
                Global.abrirModal('divModalDetalhesCarga');
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function edicaoEtapaFluxoPatioBloqueada() {
    return _fluxoAtual.CargaCancelada.val() || ((_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiEmbarcador) && (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiTMS));
}

function ocultarBotoesEtapa(knoutEtapa) {
    for (var i in knoutEtapa) {
        var propriedadeEtapa = knoutEtapa[i];

        if ((propriedadeEtapa.type == types.event) && (propriedadeEtapa.visible instanceof Function))
            propriedadeEtapa.visible(false);
    }
}

function imprimirComprovanteCargaInformada(codigo) {
    executarDownload("FluxoPatio/ComprovanteCargaInformada", { Codigo: codigo });
}

function isPermitirImprimirComprovanteCargaInformada() {
    return (
        _CONFIGURACAO_TMS.UtilizarFilaCarregamento &&
        _CONFIGURACAO_TMS.PermitirAdicionarCargaFluxoPatio &&
        _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe &&
        _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Terceiros
    );
}

function iniciarTimerSonoro() {
    if (_quantidadeFluxosEmitemAlertaSom > 0 && _timerIntervalFluxoPatio == null) {
        _audioElementFluxoPatio.pause();
        _audioElementFluxoPatio.play();
        _timerIntervalFluxoPatio = setInterval(function () { _audioElementFluxoPatio.play(); }, 15000);//a cada 15 segundos emite som novamente
    }
}

function limparTimerSonoro() {
    _quantidadeFluxosEmitemAlertaSom = 0;
    if (_timerIntervalFluxoPatio != null) {
        clearInterval(_timerIntervalFluxoPatio);
        _timerIntervalFluxoPatio = null;
    }
}

function obterConfiguracaoSequenciaGestaoPatio(data) {
    _pesquisaFluxoPatio.NumeroNfProdutor.visible(false);
    _pesquisaFluxoPatio.NumeroNfProdutor.val(0);

    executarReST("Filial/ObterConfiguracaoSequenciaGestaoPatio", { CodigoFilial: data.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaFluxoPatio.NumeroNfProdutor.visible(retorno.Data.PermitirInformacoesProdutor);
                _guaritaFluxoPatio.ImprimirTicket.visible(retorno.Data.ExibirImprimirTicketBalanca);
            }
        }
    });
}

function obterConfiguracoesTipoCarregamento() {
    _pesquisaFluxoPatio.TipoCarregamento.visible(false);
    _pesquisaFluxoPatio.TipoCarregamento.val("");

    executarReST("TipoCarregamento/ObterConfiguracoesTipoCarregamento", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaFluxoPatio.TipoCarregamento.visible(retorno.Data);
            }
        }
    });
}

function exibirMenuFluxoPatioClick(e, sender) {
    _fluxoAtual = e;

    var exibirOpcaoDetalhesCarga = !_configuracaoGestaoPatio.OcultarFluxoCarga && (_fluxoAtual.Carga.val() != 0) && (_CONFIGURACAO_TMS.UsuarioAdministrador || !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_OcultarDetalhesCarga, _PermissoesPersonalizadasFluxoPatio));
    var exibirOpcaoDetalhesCancelamento = (_fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Cancelado) && _configuracaoGestaoPatio.PermiteCancelarFluxoPatioAtual && (_fluxoAtual.Carga.val() != 0);
    var exibirOpcaoCancelar = _configuracaoGestaoPatio.PermiteCancelarFluxoPatioAtual && (_fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando) && (_fluxoAtual.Carga.val() != 0) && !_fluxoAtual.CargaCancelada.val() && (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteCancelarFluxo, _PermissoesPersonalizadasFluxoPatio));
    var exibirOpcaoObservacoesEtapas = _configuracaoGestaoPatio.HabilitarObservacaoEtapa;
    var exibirOpcaoEnviarNotificacao = _fluxoAtual.EnvioNotificacaoSMS;
    var exibirOpcaoEnviarNotificacaoSuperApp = _fluxoAtual.PermiteEnviarNotificacaoSuperApp && _fluxoAtual.PermiteEnviarNotificacaoSuperApp.val();

    var exibirOpcaoInformarEquipamentoFluxoPatio = _fluxoAtual.InformarEquipamentoFluxoPatio;

    var totalOpcoesAtivas = 0;

    if (exibirOpcaoDetalhesCarga)
        totalOpcoesAtivas++;

    if (exibirOpcaoDetalhesCancelamento)
        totalOpcoesAtivas++;

    if (exibirOpcaoCancelar)
        totalOpcoesAtivas++;

    if (exibirOpcaoObservacoesEtapas)
        totalOpcoesAtivas++;

    if (exibirOpcaoEnviarNotificacao)
        totalOpcoesAtivas++;

    if (exibirOpcaoInformarEquipamentoFluxoPatio)
        totalOpcoesAtivas++;

    if (exibirOpcaoEnviarNotificacaoSuperApp)
        totalOpcoesAtivas++;

    if (totalOpcoesAtivas == 0)
        return;

    if (totalOpcoesAtivas == 1) {
        if (exibirOpcaoDetalhesCarga)
            ObterDetalhesCargaFluxo(_fluxoAtual.Carga.val());
        else if (exibirOpcaoDetalhesCancelamento)
            exibirDetalhesCancelamentoFluxoPatio(_fluxoAtual.Codigo.val());
        else if (exibirOpcaoCancelar)
            exibirCancelamentoFluxoPatio(_fluxoAtual.Codigo.val());
        else if (exibirOpcaoObservacoesEtapas)
            exibirObservacoesEtapasFluxoPatio(_fluxoAtual.Codigo.val());
        else if (exibirOpcaoEnviarNotificacao)
            exibirModalFluxoPatioEnvioNotificacaoSMS(_fluxoAtual.codigo.val());
        else if (exibirOpcaoInformarEquipamentoFluxoPatio )
            exibirModalInformarEquipamentoFluxoPatio(_fluxoAtual.codigo.val());
        else if (exibirOpcaoEnviarNotificacaoSuperApp)
            visualizarEnviarNotificacaoSuperAppFluxoPatioClick();

        return;
    }

    if (exibirOpcaoDetalhesCarga)
        $("#limenuDetalhesCarga").show();
    else
        $("#limenuDetalhesCarga").hide();

    if (exibirOpcaoDetalhesCancelamento)
        $("#limenuVisualizarDetalhesCancelamento").show();
    else
        $("#limenuVisualizarDetalhesCancelamento").hide();

    if (exibirOpcaoCancelar)
        $("#limenuCancelarFluxo").show();
    else
        $("#limenuCancelarFluxo").hide();

    if (exibirOpcaoObservacoesEtapas)
        $("#limenuVisualizarObservacoesEtapas").show();
    else
        $("#limenuVisualizarObservacoesEtapas").hide();

    if (exibirOpcaoEnviarNotificacao)
        $("#limenuEnviarNotificacaoSMS").show();
    else
        $("#limenuEnviarNotificacaoSMS").hide();

    if (exibirOpcaoInformarEquipamentoFluxoPatio)
        $("#limenuInformarEquipamentoFluxoPatio").show();
    else
        $("#limenuInformarEquipamentoFluxoPatio").hide();

    if (exibirOpcaoEnviarNotificacaoSuperApp)
        $("#limenuEnviarNotificacaoSuperApp").show();
    else
        $("#limenuEnviarNotificacaoSuperApp").hide();

    var menu = document.querySelectorAll("#ulMenuFluxoPatio");
    var margemEsquerdaContainerPrincipal = 0;//$("#main").css("margin-left").replace("px", "");
    var margemTopoContainerPrincipal = 0;//$("#main").css("margin-top").replace("px", "");

    menu[0].style.display = 'block';
    //menu[0].style.left = sender.clientX - margemEsquerdaContainerPrincipal - 15 + $(document).scrollLeft() + 'px';
    //menu[0].style.top = sender.clientY - margemTopoContainerPrincipal - 60 + $(document).scrollTop() + 'px';

    menu[0].style.left = sender.clientX - 250 + $(document).scrollLeft() + 'px';
    menu[0].style.top = sender.clientY - 60 + $(document).scrollTop() + 'px';

    $('#ulMenuFluxoPatio').one("mouseleave", function () {
        $(this).hide();
    });
}

function visualizarCancelamentoFluxoPatioClick() {
    exibirCancelamentoFluxoPatio(_fluxoAtual.Codigo.val());
}

function visualizarDetalhesCargaClick() {
    ObterDetalhesCargaFluxo(_fluxoAtual.Carga.val());
}

function visualizarDetalhesCancelamentoClick() {
    exibirDetalhesCancelamentoFluxoPatio(_fluxoAtual.Codigo.val());
}

function visualizarObservacoesEtapasClick() {
    exibirObservacoesEtapasFluxoPatio(_fluxoAtual.Codigo.val());
}

function visualizarEnvioSMSMotoristaClick() {
    exibirModalFluxoPatioEnvioNotificacaoSMS(_fluxoAtual.Codigo.val());
}
function visualizarInformarEquipamentoFluxoPatioClick() {
    exibirModalInformarEquipamentoFluxoPatio(_fluxoAtual.Codigo.val());
}
function visualizarEnviarNotificacaoSuperAppFluxoPatioClick() {
    let listaCargasMotoristas = [{
        Codigo: _fluxoAtual.Codigo.val(),
        Carga: _fluxoAtual.Carga.val(),
        CargaEmbarcador: _fluxoAtual.NumeroCarga.val(),
        Tracao: _fluxoAtual.Placas.val(),
        CPFMotoristas: _fluxoAtual.CPFMotoristas.val(),
        Motorista: _fluxoAtual.NomeMotoristas.val(),
        Transportador: _fluxoAtual.Transportador.val()
    }];
    exibirModalEnviarNotificacaoApp(listaCargasMotoristas);
}

function buscarCapacidadeCarregamentoPorDocas() {
    var dados = RetornarObjetoPesquisa(_pesquisaFluxoPatio);

    executarReST("FluxoPatio/ObterCapacidadePorDoca", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data === false) {
                _resumoCapacidadeDocas.ExibirResumo.val(false);
                _resumoCapacidadeDocas.ListaDadosResumo([]);
                return;
            }

            _resumoCapacidadeDocas.ExibirResumo.val(true);
            _resumoCapacidadeDocas.ListaDadosResumo(retorno.Data.CapacidadePorTipoOperacao);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            _resumoCapacidadeDocas.ExibirResumo.val(false);
            _resumoCapacidadeDocas.ListaDadosResumo([]);
        }
    });
}

function downloadRomaneioTotalizadorPDF(e) {
    executarDownload("FluxoPatio/DownloadRomaneioTotalizador", { Carga: e.CodigoCarga.val() });
}

function downloadRomaneioDetalhadoPDF(e) {
    executarDownload("FluxoPatio/DownloadRomaneioDetalhado", { Carga: e.CodigoCarga.val() });
}
function downloadRomaneioDetalhadoResumidoPDF(e) {
    executarDownload("FluxoPatio/DownloadRomaneioDetalhadoResumido", { Carga: e.CodigoCarga.val() });
}