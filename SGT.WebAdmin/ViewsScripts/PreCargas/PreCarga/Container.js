/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Cargas/Carga/DadosCarga/Carga.js" />
/// <reference path="../../Configuracao/Sistema/OperadorLogistica.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/ConfiguracaoProgramacaoCarga.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPreCarga.js" />
/// <reference path="../../Enumeradores/EnumFiltroPreCarga.js" />
/// <reference path="DadosParaTransporte.js" />
/// <reference path="PreCarga.js" />
/// <reference path="PreCargaManual.js" />
/// <reference path="PreCargasAtualizadas.js" />
/// <reference path="TransportadoresOfertados.js" />

// #region Objetos Globais do Arquivo

var _containerPreCarga;
var _gridPreCarga;
var _gridPreCargaRetornoVincularFilaCarregamento;
var _legendaPreCarga;
var _pesquisaPreCarga;
var _resumoPreCarga;
var _supervisor = false;
var _configuracaoPreCarga;

// #endregion Objetos Globais do Arquivo

// #region Classes

var ContainerPreCarga = function () {
    this.CancelarTodas = PropertyEntity({ type: types.event, eventClick: cancelarMultiplasPreCargasClick, text: Localization.Resources.Cargas.Carga.CancelarPrePlanejamento, visible: ko.observable(false) });
    this.SelecionarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), text: Localization.Resources.Cargas.Carga.MarcarTodas, visible: ko.observable(true) });
    this.VincularFilaCarregamento = PropertyEntity({ type: types.event, eventClick: vincularFilaCarregamentoMultiplasPreCargasClick, text: Localization.Resources.Cargas.Carga.VincularVeiculosFila, visible: ko.observable(false) });
}

var LegendaPreCarga = function () {
    var descricaoCarga = _configuracaoPreCarga.UtilizarProgramacaoCarga ? "Pré Carga" : "Carga";

    this.AguardandoAceite = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.AgAceiteTransportador), visible: ko.observable(_configuracaoPreCarga.UtilizarProgramacaoCarga) });
    this.AguardandoDadosTransporte = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.AgDadosTransportador), visible: ko.observable(_configuracaoPreCarga.UtilizarProgramacaoCarga) });
    this.AguardandoGeracaoCarga = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.Ag + descricaoCarga), visible: ko.observable(true) });
    this.Cancelada = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.Cancelado), visible: ko.observable(true) });
    this.CargaGerada = PropertyEntity({ text: ko.observable(descricaoCarga + Localization.Resources.Cargas.Carga.Vinculada), visible: ko.observable(true) });
    this.Nova = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.Novo), visible: ko.observable(_configuracaoPreCarga.UtilizarProgramacaoCarga) });
    this.ProblemaVincularCarga = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.ProblemaVincularCarga), visible: ko.observable(!_configuracaoPreCarga.UtilizarProgramacaoCarga) });
};

var PesquisaPreCarga = function () {
    var dataInicial = _configuracaoPreCarga.UtilizarProgramacaoCarga ? Global.DataAtual() : "";
    var dataFinal = _configuracaoPreCarga.UtilizarProgramacaoCarga ? moment().add(_configuracaoPreCarga.DiasFiltrarDataProgramada, 'days').format("DD/MM/YYYY") : "";
    var situacoesPreSelecionadas = !_configuracaoPreCarga.UtilizarProgramacaoCarga ? [] : [
        EnumSituacaoPreCarga.AguardandoAceite,
        EnumSituacaoPreCarga.AguardandoDadosTransporte,
        EnumSituacaoPreCarga.AguardandoGeracaoCarga,
        EnumSituacaoPreCarga.CargaGerada,
    ];

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Filial.getFieldDescription(), idBtnSearch: guid() });
    this.ConfiguracaoProgramacaoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.ConfiguracaoPrePlanejamento, idBtnSearch: guid(), visible: _configuracaoPreCarga.UtilizarProgramacaoCarga });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloVeicularCarga, idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoCarga.getFieldDescription(), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoOperacao.getFieldDescription(), idBtnSearch: guid() });
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataPrePlanejamentoDe.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(dataInicial), visible: _configuracaoPreCarga.UtilizarProgramacaoCarga });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataPrePlanejamentoAte.getFieldDescription(), dateRangeInit: this.DataInicial, getType: typesKnockout.date, val: ko.observable(dataFinal), visible: _configuracaoPreCarga.UtilizarProgramacaoCarga });
    this.DataViagemInicial = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataInicial, getType: typesKnockout.date, visible: !_configuracaoPreCarga.UtilizarProgramacaoCarga });
    this.DataViagemFinal = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataFinal, dateRangeInit: this.DataViagemInicial, getType: typesKnockout.date, visible: !_configuracaoPreCarga.UtilizarProgramacaoCarga });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable(situacoesPreSelecionadas), def: situacoesPreSelecionadas, options: EnumSituacaoPreCarga.obterOpcoes(), visible: _configuracaoPreCarga.UtilizarProgramacaoCarga });
    this.Status = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), val: ko.observable(EnumFiltroPreCarga.Todos), def: EnumFiltroPreCarga.Todos, options: EnumFiltroPreCarga.obterOpcoesPesquisa(), visible: !_configuracaoPreCarga.UtilizarProgramacaoCarga });
    this.PreCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroPrePlanejamento, getType: typesKnockout.string });
    this.Carga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroCarga.getFieldDescription(), getType: typesKnockout.string, visible: !_configuracaoPreCarga.UtilizarProgramacaoCarga });
    this.Pedido = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroPedido.getFieldDescription(), getType: typesKnockout.string, visible: !_configuracaoPreCarga.UtilizarProgramacaoCarga });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription(), idBtnSearch: guid(), visible: !_configuracaoPreCarga.UtilizarProgramacaoCarga });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), idBtnSearch: guid(), visible: !_configuracaoPreCarga.UtilizarProgramacaoCarga });
    this.CidadesDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Destinos.getFieldDescription(), idBtnSearch: guid() });
    this.RotasFrete = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Rota.getFieldDescription(), idBtnSearch: guid() });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    this.DataViagemInicial.dateRangeLimit = this.DataViagemFinal;
    this.DataViagemFinal.dateRangeInit = this.DataViagemInicial;

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Cargas.Carga.Importar,
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        UrlImportacao: "PreCarga/Importar",
        UrlConfiguracao: "PreCarga/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O013_PreCargas,
        CallbackRegistrosAlterados: callbackExibirRegistraoQueSeraoAlterados,
        CallbackImportacao: function () {
            if (_pesquisaPreCarga.Filial.codEntity() > 0)
                recarregarPreCargas();
        },
        visible: !_configuracaoPreCarga.UtilizarProgramacaoCarga
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
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false)
    });

    this.ImportarPrePlanejamento = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Cargas.Carga.ImportarPrePlanejamento,
        visible: ko.observable(false),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "PreCarga/ImportarPrePlanejamento",
        UrlConfiguracao: "PreCarga/ConfiguracaoImportacaoPrePlanejamento",
        CodigoControleImportacao: EnumCodigoControleImportacao.O013_PreCargas,
        CallbackImportacao: function () {
            //_gridCarga.CarregarGrid();
        }
    });

    this.AdicionarPreCargaManual = PropertyEntity({ eventClick: exibirModalAdicionarPreCargaManualClick, type: types.event, text: Localization.Resources.Cargas.Carga.AdicionarPrePlanejamento, idGrid: guid(), visible: ko.observable(false) });
    this.Pesquisar = PropertyEntity({ eventClick: carregarPreCargas, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
    this.ExibirFiltros = PropertyEntity({ eventClick: function (e) { e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade()); }, type: types.event, text: Localization.Resources.Cargas.Carga.FiltroDePesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
}

var ResumoPreCarga = function () {
    this.ExibirResumo = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(_configuracaoPreCarga.UtilizarProgramacaoCarga) });

    this.AguardandoAceite = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.AgAceiteTransportador), visible: ko.observable(true) });
    this.AguardandoDadosTransporte = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.AgDadosTransportador), visible: ko.observable(true) });
    this.AguardandoGeracaoCarga = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.AgPreCarga), visible: ko.observable(true) });
    this.Cancelada = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.Cancelado), visible: ko.observable(true) });
    this.CargaGerada = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.PreCargaVinculada), visible: ko.observable(true) });
    this.Nova = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.Novo), visible: ko.observable(true) });

    this.ExibirDados = PropertyEntity({ eventClick: function (e) { e.ExibirDados.visibleFade(!e.ExibirDados.visibleFade()); }, type: types.event, idFade: guid(), visibleFade: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadPreCarga() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {
            carregarConfiguracaoPreCarga(function (configuracaoPreCarga) {
                _configuracaoPreCarga = configuracaoPreCarga.Configuracoes;

                _pesquisaPreCarga = new PesquisaPreCarga();
                KoBindings(_pesquisaPreCarga, "knockoutPesquisaPreCarga", false, _pesquisaPreCarga.Pesquisar.id);

                _legendaPreCarga = new LegendaPreCarga();
                KoBindings(_legendaPreCarga, "knockoutLegendaPreCarga");

                _resumoPreCarga = new ResumoPreCarga();
                KoBindings(_resumoPreCarga, "knockoutResumoPreCarga");

                _containerPreCarga = new ContainerPreCarga();
                KoBindings(_containerPreCarga, "knockoutContainerPreCarga");

                new BuscarFilial(_pesquisaPreCarga.Filial);
                new BuscarConfiguracaoProgramacaoCarga(_pesquisaPreCarga.ConfiguracaoProgramacaoCarga);
                new BuscarModelosVeicularesCarga(_pesquisaPreCarga.ModeloVeicularCarga);
                new BuscarTiposdeCarga(_pesquisaPreCarga.TipoCarga);
                new BuscarTiposOperacao(_pesquisaPreCarga.TipoOperacao);
                new BuscarClientes(_pesquisaPreCarga.Remetente);
                new BuscarClientes(_pesquisaPreCarga.Destinatario);
                new BuscarLocalidades(_pesquisaPreCarga.CidadesDestino);
                new BuscarRotasFrete(_pesquisaPreCarga.RotasFrete);

                loadDadosParaTransporte();
                loadPreCargaAtualizada();
                loadPreCargaManual();
                loadPreCargaDados();
                loadPreCargaTransportadoresOfertados();
                loadGridPreCarga();
                loadGridPreCargaRetornoVincularFilaCarregamento();

                _supervisor = VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.PreCarga_Supervisor, _PermissoesPersonalizadasPreCarga);

                carregarUsuarioLogado();
            });
        });
    });
}

function loadGridPreCarga() {
    var totalRegistrosPorPagina = 10;
    var limiteRegistros = 50;

    var configExportacao = {
        url: "PreCarga/ExportarPesquisa",
        titulo: "Pré Planejamentos"
    };

    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _containerPreCarga.SelecionarTodas,
        callbackNaoSelecionado: controlarExibirMultiplasOpcoes,
        callbackSelecionado: controlarExibirMultiplasOpcoes,
        callbackSelecionarTodos: controlarExibirMultiplasOpcoes,
        somenteLeitura: false,
        classeSelecao: "item-selecionado"
    };

    var opcaoCancelar = { descricao: Localization.Resources.Gerais.Geral.Cancelar, id: guid(), evento: "onclick", metodo: cancelarPreCargaClick, tamanho: "10", icone: "", visibilidade: isPermitirCancelarPreCarga };
    var opcaoDadosParaTransporte = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), evento: "onclick", metodo: exibirModalPreCargaDadosParaTransporteClick, tamanho: "10", icone: "" };
    var opcaoDetalhesCarga = { descricao: Localization.Resources.Cargas.Carga.DetalhesDaCarga, id: guid(), evento: "onclick", metodo: exibirModalDetalhesCargaClick, tamanho: "10", icone: "", visibilidade: isPermitirVisualizarDetalhesCarga };
    var opcaoExibirTransportadoresOfertados = { descricao: Localization.Resources.Cargas.Carga.TransportadoresOfertados, id: guid(), evento: "onclick", metodo: exibirModalPreCargaTransportadoresOfertadosClick, tamanho: "10", icone: "", visibilidade: isPermitirVisualizarTransportadoresOfertados };
    var opcaoObservacao = { descricao: Localization.Resources.Gerais.Geral.Observacao, id: guid(), evento: "onclick", metodo: exibirModalObservacaoClick, tamanho: "10", icone: "" };
    var opcaoAlterarDataPlanejamento = { descricao: Localization.Resources.Cargas.Carga.AlterarDataPlanejamento, id: guid(), evento: "onclick", metodo: exibirModalAlterarDataPlanejamento, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [opcaoCancelar, opcaoDadosParaTransporte, opcaoDetalhesCarga, opcaoExibirTransportadoresOfertados, opcaoObservacao, opcaoAlterarDataPlanejamento], tamanho: 10 };

    _gridPreCarga = new GridView("grid-pre-carga", "PreCarga/Pesquisa", _pesquisaPreCarga, menuOpcoes, null, totalRegistrosPorPagina, null, null, null, multiplaEscolha, limiteRegistros, null, configExportacao);
    _gridPreCarga.SetPermitirEdicaoColunas(true);
    _gridPreCarga.SetSalvarPreferenciasGrid(true);
}

function loadGridPreCargaRetornoVincularFilaCarregamento() {
    var quantidadePorPagina = 10;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var header = [
        { data: "NumeroPreCarga", title: Localization.Resources.Cargas.Carga.PrePlanejamento, width: "20%", className: 'text-align-center', orderable: true },
        { data: "MensagemRetorno", title: Localization.Resources.Cargas.Carga.MensagemRetorno, width: "80%", orderable: false }
    ];

    _gridPreCargaRetornoVincularFilaCarregamento = new BasicDataTable("grid-pre-carga-retorno-vincular-fila-carregamento", header, null, ordenacao, null, quantidadePorPagina);
    _gridPreCargaRetornoVincularFilaCarregamento.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function cancelarMultiplasPreCargasClick() {
    exibirModalPreCargaCancelamentoMassivo();
}

function cancelarPreCargaClick(registroSelecionado) {
    exibirModalPreCargaCancelamento(registroSelecionado.Codigo);
}

function exibirModalAdicionarPreCargaManualClick() {
    var filial;

    if (_pesquisaPreCarga.Filial.codEntity() > 0)
        filial = { Codigo: _pesquisaPreCarga.Filial.codEntity(), Descricao: _pesquisaPreCarga.Filial.val() };

    exibirModalAdicionarPreCargaManual(filial);
}

function exibirModalDetalhesCargaClick(registroSelecionado) {
    executarReST("Carga/BuscarCargaPorCodigo", { Carga: registroSelecionado.CodigoCarga }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                $("#fdsCarga").html('<button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true" style="position: absolute; z-index: 9999; right: 18px; top: 6px;"><i class="fal fa-times"></i></button>');
                _cargaAtual = GerarTagHTMLDaCarga("fdsCarga", retorno.Data, false);
                $("#fdsCarga .container-carga").addClass("mb-0");

                if (retorno.Data.DadosTransporte.TipoCarga.Codigo <= 0 || retorno.Data.DadosTransporte.ModeloVeicularCarga.Codigo <= 0)
                    $("#" + _cargaAtual.EtapaInicioEmbarcador.idTab).click();
                else if (retorno.Data.SituacaoCarga == EnumSituacaoCargaJanelaCarregamento.SemValorFrete)
                    $("#" + _cargaAtual.EtapaFreteEmbarcador.idTab).click();
                else if (
                    retorno.Data.SituacaoCarga == EnumSituacaoCargaJanelaCarregamento.SemTransportador ||
                    retorno.Data.SituacaoCarga == EnumSituacaoCargaJanelaCarregamento.AgAceiteTransportador ||
                    retorno.Data.SituacaoCarga == EnumSituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador ||
                    retorno.Data.SituacaoCarga == EnumSituacaoCargaJanelaCarregamento.ProntaParaCarregamento
                )
                    $("#" + _cargaAtual.EtapaDadosTransportador.idTab).click();
                else
                    $("#" + _cargaAtual.EtapaInicioEmbarcador.idTab).click();

                Global.abrirModal('divModalDetalhesCarga');
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function exibirModalPreCargaDadosParaTransporteClick(registroSelecionado) {
    exibirModalDadosParaTransporte(registroSelecionado.Codigo);
}

function exibirModalPreCargaTransportadoresOfertadosClick(registroSelecionado) {
    visualizarPreCargaTransportadoresOfertados(registroSelecionado.Codigo);
}

function vincularFilaCarregamentoMultiplasPreCargasClick() {
    var dadosPreCargasSelecionadas = obterDadosPreCargasSelecionadas();

    if ((dadosPreCargasSelecionadas.registrosSelecionados.length == 0) && !dadosPreCargasSelecionadas.selecionarTodos)
        return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Cargas.Carga.NenhumPrePlanejamentoSelecionado);

    var dados = RetornarObjetoPesquisa(_pesquisaPreCarga);

    dados.SelecionarTodos = dadosPreCargasSelecionadas.selecionarTodos;
    dados.ItensSelecionados = JSON.stringify(dadosPreCargasSelecionadas.registrosSelecionados);
    dados.ItensNaoSelecionados = JSON.stringify(dadosPreCargasSelecionadas.registrosNaoSelecionados);

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.VoceTemCertezaDesejaVincularVeiculosFilaTodosPrePlanejamentosSelelcionados, function () {
        executarReST("PreCarga/VincularFilaCarregamentoMassivo", dados, function (retorno) {
            if (retorno.Success) {
                _gridPreCargaRetornoVincularFilaCarregamento.CarregarGrid(retorno.Data);

                Global.abrirModal('divModalRetornoPreCargasVincularFilaCarregamento');

                recarregarPreCargas();
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, null);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function recarregarPreCargas() {
    _gridPreCarga.AtualizarRegistrosSelecionados([]);
    _gridPreCarga.CarregarGrid();
    _containerPreCarga.SelecionarTodas.val(false);

    controlarExibirMultiplasOpcoes();
    recarregarTotalizadoresPorSituacaoPreCarga();
}

function obterDadosPreCargasSelecionadas() {
    return {
        registrosNaoSelecionados: obterCodigosPreCargasNaoSelecionadas(),
        registrosSelecionados: obterCodigosPreCargasSelecionadas(),
        selecionarTodos: _containerPreCarga.SelecionarTodas.val()
    }
}

// #endregion Funções Públicas

// #region Funções Privadas

function carregarConfiguracaoPreCarga(callback) {
    executarReST("PreCarga/ObterConfiguracao", undefined, function (retorno) {
        if (retorno.Success)
            callback(retorno.Data);
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function carregarPreCargas() {
    $("#container-pre-cargas").show();

    _pesquisaPreCarga.ExibirFiltros.visibleFade(false);
    _pesquisaPreCarga.AdicionarPreCargaManual.visible(_configuracaoPreCarga.PermiteAdicionarPreCargaManual);
    _pesquisaPreCarga.ImportarPrePlanejamento.visible(_configuracaoPreCarga.PermiteAdicionarPreCargaManual && _configuracaoPreCarga.UtilizarProgramacaoCarga);

    recarregarPreCargas();
}

function carregarUsuarioLogado() {
    executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data !== false && retorno.Data != null)
                _pesquisaPreCarga.Filial.val(retorno.Data.Filial.Descricao).codEntity(retorno.Data.Filial.Codigo);
        }
    });
}

function controlarExibirMultiplasOpcoes() {
    var existemRegistrosSelecionados = _gridPreCarga.ObterMultiplosSelecionados().length > 0;
    var selecionarTodos = _containerPreCarga.SelecionarTodas.val()

    _containerPreCarga.CancelarTodas.visible(existemRegistrosSelecionados || selecionarTodos);
    _containerPreCarga.VincularFilaCarregamento.visible((existemRegistrosSelecionados || selecionarTodos) && (_CONFIGURACAO_TMS.UtilizarFilaCarregamento || _configuracaoPreCarga.UtilizarProgramacaoCarga));
}

function isPermitirCancelarPreCarga(registroSelecionado) {
    return (registroSelecionado.PermitirCancelar || (registroSelecionado.PermitirCancelarViaSupervisor && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.PreCarga_Supervisor, _PermissoesPersonalizadasPreCarga)));
}

function isPermitirVisualizarDetalhesCarga(registroSelecionado) {
    return registroSelecionado.CodigoCarga > 0;
}

function isPermitirVisualizarTransportadoresOfertados(registroSelecionado) {
    return registroSelecionado.ExibirTransportadoresOfertados;
}

function obterCodigosPreCargasNaoSelecionadas() {
    var preCargasNãoSelecionadas = _gridPreCarga.ObterMultiplosNaoSelecionados().slice();
    var codigosPreCargasNaoSelecionadas = [];

    preCargasNãoSelecionadas.forEach(function (preCarga) {
        codigosPreCargasNaoSelecionadas.push(preCarga.Codigo);
    });

    return codigosPreCargasNaoSelecionadas;
}

function obterCodigosPreCargasSelecionadas() {
    var preCargasSelecionadas = _gridPreCarga.ObterMultiplosSelecionados().slice();
    var codigosPreCargasSelecionadas = [];

    preCargasSelecionadas.forEach(function (preCarga) {
        codigosPreCargasSelecionadas.push(preCarga.Codigo);
    });

    return codigosPreCargasSelecionadas;
}

function recarregarTotalizadoresPorSituacaoPreCarga() {
    executarReST("PreCarga/ObterTotalizadoresPorSituacao", RetornarObjetoPesquisa(_pesquisaPreCarga), function (retorno) {
        if (retorno.Success)
            PreencherObjetoKnout(_resumoPreCarga, retorno);
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            LimparCampos(_resumoPreCarga);
        }
    });
}

// #endregion Funções Privadas
