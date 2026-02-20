/// <reference path="Integracao.js" />
/// <reference path="Aprovadores.js" />
/// <reference path="EtapaAjuste.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../../Areas/Relatorios/ViewsScripts/Relatorios/Global/Relatorio.js" />
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAjusteTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoCampoValorTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoParametroBaseTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoComponenteFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoRegistroAjusteTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../Consultas/ContratoTransporteFrete.js" />
/// <reference path="../../Consultas/MotivoReajuste.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridAjusteTabelaFrete;
var _pesquisaAjusteTabelaFrete;
var _gridPreviewAjusteTabelaFrete;
var _ajusteTabelaFrete;
var _CRUDRelatorio;

var _relatorioConsultaTabelaFrete;

var PesquisaAjusteTabelaFrete = function () {
    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tabela de Frete:", idBtnSearch: guid(), required: true, issue: 78 });
    this.DataVigencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Vigência:", idBtnSearch: guid(), issue: 82, enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação:", options: EnumSituacaoAjusteTabelaFrete.obterOpcoesPesquisaAjusteTabelaFrete(), val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.ExibirSomenteRegistrosComReajuste = PropertyEntity({ text: "Exibir somente registros com valor de reajuste", val: ko.observable(false), def: false, getType: typesKnockout.bool })

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAjusteTabelaFrete.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var AjusteTabelaFrete = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(EnumSituacaoAjusteTabelaFrete.Pendente), def: EnumSituacaoAjusteTabelaFrete.Pendente });

    this.TipoPagamento = PropertyEntity({ val: ko.observable(""), options: EnumTipoPagamento.obterOpcoesPesquisa(), def: "", text: "Tipo de Pagamento: ", issue: 120, enable: ko.observable(true) });
    this.TabelaComCargaRealizada = PropertyEntity({ text: "Apenas tabelas com cargas realizadas?", val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true) });
    this.SomenteRegistrosComValores = PropertyEntity({ text: "Exibir apenas registros com valor", val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true) });
    this.AjustarPedagiosComSemParar = PropertyEntity({ text: "Ajustar pedágios com a Sem Parar", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });

    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tabela de Frete:", idBtnSearch: guid(), required: true, issue: 78, enable: ko.observable(true) });
    this.MotivoReajuste = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motivo do Reajuste:", issue: 797, idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.LocalidadeOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid(), issue: 16, enable: ko.observable(true) });

    this.LocalidadeDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid(), issue: 16, enable: ko.observable(true) });

    this.Empresa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.EmpresaExclusiva = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Transportador:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.ContratoTransporteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Contrato do Transportador:", idBtnSearch: guid(), eventChange: contratoTransporteFreteBlur, visible: ko.observable(false), enable: ko.observable(true) });
    this.DataInicialContrato = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataFinalContrato = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.RegiaoDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Região de Destino:", idBtnSearch: guid(), issue: 110, enable: ko.observable(true), visible: ko.observable(true) });
    this.RotaFreteDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Rota de Destino:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.RotaFreteOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Rota de Origem:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });

    this.UtilizarBuscaNasLocalidadesPorEstadoOrigem = PropertyEntity({ getType: typesKnockout.bool, def: false, val: ko.observable(false), text: "Buscar também nas localidades de origem com o(s) Estado(s) informados?", visible: ko.observable(true), enable: ko.observable(true) });
    this.UtilizarBuscaNasLocalidadesPorEstadoDestino = PropertyEntity({ getType: typesKnockout.bool, def: false, val: ko.observable(false), text: "Buscar também nas localidades de destino com o(s) Estado(s) informados?", visible: ko.observable(true), enable: ko.observable(true) });

    this.EstadoDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Estado de Destino:", idBtnSearch: guid(), issue: 12, enable: ko.observable(true) });
    this.EstadoOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Estado de Origem:", idBtnSearch: guid(), issue: 12, enable: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid(), issue: 53, enable: ko.observable(true) });
    this.ModeloReboque = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo de Reboque:", idBtnSearch: guid(), issue: 44, enable: ko.observable(true) });
    this.ModeloTracao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo de Tração:", idBtnSearch: guid(), issue: 44, enable: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), issue: 55, enable: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), issue: 55, enable: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), issue: 55, enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), issue: 121, enable: ko.observable(true) });

    this.RegiaoOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Região Origem:", idBtnSearch: guid(), issue: 82, enable: ko.observable(true) });
    this.CanalEntrega = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Canal de Entrega:", idBtnSearch: guid(), issue: 82, enable: ko.observable(true) });
    this.CanalVenda = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Canal de Venda:", idBtnSearch: guid(), issue: 82, enable: ko.observable(true) });

    this.DataVigencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Vigência de Origem:", idBtnSearch: guid(), issue: 82, enable: ko.observable(true), required: true });
    this.TipoRegistro = PropertyEntity({ text: "Tipo:", options: EnumTipoRegistroAjusteTabelaFrete.obterOpcoesPesquisa(), val: ko.observable(EnumTipoRegistroAjusteTabelaFrete.Todos), def: EnumTipoRegistroAjusteTabelaFrete.Todos, visible: ko.observable(false) });

    this.DataVigenciaAjuste = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Nova Vigência:", idBtnSearch: guid(), issue: 82, enable: ko.observable(true), visible: !_CONFIGURACAO_TMS.ObrigarVigenciaNoAjusteFrete });
    this.NovaVigenciaIndefinida = PropertyEntity({ type: types.map, getType: typesKnockout.date, text: "*Nova Vigência:", visible: _CONFIGURACAO_TMS.ObrigarVigenciaNoAjusteFrete });
    this.ItensAjuste = ko.observableArray();

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.Preview = PropertyEntity({
        eventClick: function (e) {
            GerarPreviewClick(e, this);
        }, type: types.event, text: "Consultar Tabelas de Frete", idGrid: guid(), visible: ko.observable(true)
    });

    this.Mensagem = PropertyEntity({ type: types.local, warning: ko.observable(true), danger: ko.observable(true), success: ko.observable(true), visible: ko.observable(false) });

    this.LimparFiltros = PropertyEntity({ eventClick: LimparFiltrosClick, type: types.event, text: "Limpar Filtros", idGrid: guid(), visible: ko.observable(true), icon: "fal fa-recycle" });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.AplicarAjustes = PropertyEntity({ eventClick: AplicarAjustesClick, type: types.event, text: "Aplicar Ajustes", idFade: guid(), icon: "fal fa-chevron-down", visible: ko.observable(false), enable: ko.observable(true) });

    this.RetornarValoresOriginais = PropertyEntity({ eventClick: RetornarValoresOriginaisClick, type: types.event, text: "Retornar aos Valores Originais", idFade: guid(), icon: "fal fa-undo", visible: ko.observable(false) });

    this.ConfiguracaoRelatorio = PropertyEntity({
        eventClick: function (e) {
            var valido = ValidarCamposObrigatorios(_ajusteTabelaFrete);

            if (valido) {
                if (e.ConfiguracaoRelatorio.visibleFade() == true) {
                    e.ConfiguracaoRelatorio.visibleFade(false);
                    e.ConfiguracaoRelatorio.icon("fal fa-plus");
                } else {
                    e.ConfiguracaoRelatorio.visibleFade(true);
                    e.ConfiguracaoRelatorio.icon("fal fa-minus");
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Verifique os campos obrigatórios!");
            }
        }, type: types.event, text: "Configuração da Consulta", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.TabelaFrete.codEntity.subscribe(function (novoValor) {
        TrocarTabelaFrete(novoValor);
    });

    this.TabelaFrete.val.subscribe(function (novoValor) {
        if (novoValor == null || novoValor.trim() == "")
            _ajusteTabelaFrete.TabelaFrete.codEntity(0);
    });

    this.DataVigencia.val.subscribe(function (novoValor) {
        if (novoValor == null || novoValor.trim() == "")
            _ajusteTabelaFrete.DataVigencia.codEntity(0);
    });
}

var ItemAjuste = function () {
    $this = this;

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Tipo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.TipoComponente = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Descricao = PropertyEntity({});
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable(""), def: "", maxlength: ko.observable(7), title: ko.observable(""), configDecimal: { precision: 4, allowZero: false, allowNegative: false } });
    this.TipoOperacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(EnumTipoCampoValorTabelaFrete.AumentoPercentual), def: EnumTipoCampoValorTabelaFrete.AumentoPercentual, icon: ko.observable(""), title: ko.observable("") });
    this.Aumenta = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: true, title: ko.observable("Aumento de Valor"), icon: ko.observable("fal fa-plus") });
    this.Arredondar = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: "Arredondar o resultado", title: "Arredonda o valor para o inteiro mais próximo (ex: 9,50 será 10,00)." });

    this.AumentoValor = PropertyEntity({
        eventClick: function (e) {
            e.Aumenta.val(true);
            e.Aumenta.title("Aumento de Valor");
            e.Aumenta.icon("fal fa-plus");
        }, type: types.event, text: "Aumento de Valor", icon: "fal fa-plus", idGrid: guid(), visible: ko.observable(true)
    });

    this.ReducaoValor = PropertyEntity({
        eventClick: function (e) {
            e.Aumenta.val(false);
            e.Aumenta.title("Redução de Valor");
            e.Aumenta.icon("fal fa-minus");
        }, type: types.event, text: "Redução de Valor", icon: "fal fa-minus", idGrid: guid(), visible: ko.observable(true)
    });

    this.ValorFixo = PropertyEntity({
        eventClick: function (e) {
            TipoOperacaoChange(e, EnumTipoCampoValorTabelaFrete.ValorFixo);
        }, type: types.event, title: "Substitui o valor original pelo valor informado", text: "Valor Fixo", icon: "fal fa-dollar-sign", idGrid: guid(), visible: ko.observable(true)
    });

    this.ValorParcial = PropertyEntity({
        eventClick: function (e) {
            TipoOperacaoChange(e, EnumTipoCampoValorTabelaFrete.AumentoValor);
        }, type: types.event, title: "Adiciona/remove do valor original o valor informado", text: "Valor Parcial", icon: "fal fa-exchange-alt", idGrid: guid(), visible: ko.observable(true)
    });

    this.Percentual = PropertyEntity({
        eventClick: function (e) {
            TipoOperacaoChange(e, EnumTipoCampoValorTabelaFrete.AumentoPercentual);
        }, type: types.event, title: "Aumenta/reduz o valor original de acordo com o percentual informado", text: "Percentual", icon: "fal fa-percent", idGrid: guid(), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.AbrirAjuste = PropertyEntity({ eventClick: CriarPreviewClick, type: types.event, text: "Abrir Ajuste", icon: "fal fa-arrow-right", visible: ko.observable(true) });
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF", icon: "fal fa-file-pdf", visible: ko.observable(false) });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", icon: "fal fa-file-excel", visible: ko.observable(false) });
    this.FinalizarAjustes = PropertyEntity({ eventClick: FinalizarAjustesClick, type: types.event, text: "Finalizar o Ajuste", icon: "fal fa-chevron-down", visible: ko.observable(false) });
    this.CancelarAjustes = PropertyEntity({ eventClick: CancelarAjustesClick, type: types.event, text: "Cancelar o Ajuste", icon: "fal fa-times", visible: ko.observable(false) });
    this.NovoAjuste = PropertyEntity({ eventClick: LimparCamposAjusteTabelaFrete, type: types.event, text: "Limpar / Novo Ajuste", icon: "fal fa-recycle" });
    this.GerarSimulacao = PropertyEntity({ eventClick: AbrirTelaGeracaoSimulacao, type: types.event, text: "Gerar Simulação", icon: "fal fa-tasks", visible: ko.observable(false) });
    this.ReprocessarAjuste = PropertyEntity({ eventClick: ReprocessarAjusteClick, type: types.event, text: "Reprocessar Ajuste", icon: "fal fa-tasks", visible: ko.observable(false) });
    this.ReprocessarCriacao = PropertyEntity({ eventClick: ReprocessarCriacaoClick, type: types.event, text: "Reprocessar Criação", icon: "fal fa-tasks", visible: ko.observable(false) });
    this.ReprocessarAjusteValores = PropertyEntity({ eventClick: ReprocessarAjusteValoresClick, type: types.event, text: "Reprocessar Aplicação de Ajustes", icon: "fal fa-tasks", visible: ko.observable(false) });
    this.BuscarValoresSemParar = PropertyEntity({ eventClick: BuscarValoresSemPararClick, type: types.event, text: "Buscar Valores na Sem Parar", icon: "fal fa-dollar-sign", visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(false),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "AjusteTabelaFrete/Importar",
        UrlConfiguracao: "AjusteTabelaFrete/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O003_ReajusteFrete,
        ParametrosRequisicao: function () { var paramentros = { Codigo: _ajusteTabelaFrete.Codigo.val() }; return paramentros; },
        CallbackImportacao: function () {
            _gridPreviewAjusteTabelaFrete.CarregarGrid();
        }
    });
}

//*******EVENTOS*******

function LoadAjusteTabelaFrete() {
    _pesquisaAjusteTabelaFrete = new PesquisaAjusteTabelaFrete();
    KoBindings(_pesquisaAjusteTabelaFrete, "knockoutPesquisaAjusteTabelaFrete", false, _pesquisaAjusteTabelaFrete.Pesquisar.id);

    _ajusteTabelaFrete = new AjusteTabelaFrete();
    KoBindings(_ajusteTabelaFrete, "knockoutAjusteTabelaFrete", false);

    _CRUDRelatorio = new CRUDRelatorio();
    KoBindings(_CRUDRelatorio, "knockoutCRUDAjusteTabelaFrete", false);

    HeaderAuditoria("AjusteTabelaFrete", _ajusteTabelaFrete);

    LoadSimulacaoFrete();
    loadAutorizadores();
    loadEtapaAjuste();
    loadIntegracao();

    new BuscarTabelasDeFrete(_pesquisaAjusteTabelaFrete.TabelaFrete, null, EnumTipoTabelaFrete.tabelaCliente);
    new BuscarClientes(_ajusteTabelaFrete.Remetente);
    new BuscarClientes(_ajusteTabelaFrete.Destinatario);
    new BuscarClientes(_ajusteTabelaFrete.Tomador);
    new BuscarLocalidades(_ajusteTabelaFrete.LocalidadeDestino);
    new BuscarLocalidades(_ajusteTabelaFrete.LocalidadeOrigem);
    new BuscarModelosVeicularesCarga(_ajusteTabelaFrete.ModeloReboque, null, null, null, [EnumTipoModeloVeicularCarga.Geral, EnumTipoModeloVeicularCarga.Reboque]);
    new BuscarModelosVeicularesCarga(_ajusteTabelaFrete.ModeloTracao, null, null, null, [EnumTipoModeloVeicularCarga.Tracao]);
    new BuscarRegioes(_ajusteTabelaFrete.RegiaoDestino);
    new BuscarRegioes(_ajusteTabelaFrete.RegiaoOrigem);
    new BuscarCanaisEntrega(_ajusteTabelaFrete.CanalEntrega)
    BuscarCanaisVenda(_ajusteTabelaFrete.CanalVenda)
    new BuscarRotasFrete(_ajusteTabelaFrete.RotaFreteDestino);
    new BuscarRotasFrete(_ajusteTabelaFrete.RotaFreteOrigem);
    new BuscarTransportadores(_ajusteTabelaFrete.Empresa);
    new BuscarTransportadores(_ajusteTabelaFrete.EmpresaExclusiva);
    new BuscarMotivoReajuste(_ajusteTabelaFrete.MotivoReajuste);
    new BuscarTiposdeCarga(_ajusteTabelaFrete.TipoCarga);
    new BuscarTiposOperacao(_ajusteTabelaFrete.TipoOperacao);
    new BuscarTabelasDeFrete(_ajusteTabelaFrete.TabelaFrete, null, EnumTipoTabelaFrete.tabelaCliente);
    new BuscarEstados(_ajusteTabelaFrete.EstadoDestino);
    new BuscarEstados(_ajusteTabelaFrete.EstadoOrigem);
    new BuscarVigenciasTabelaFrete(_ajusteTabelaFrete.DataVigenciaAjuste, _ajusteTabelaFrete.TabelaFrete, function (data) { RetornoConsultaVigencia(_ajusteTabelaFrete.DataVigenciaAjuste, data); }, true, _ajusteTabelaFrete.EmpresaExclusiva, _ajusteTabelaFrete.DataInicialContrato, _ajusteTabelaFrete.DataFinalContrato);
    new BuscarVigenciasTabelaFrete(_ajusteTabelaFrete.DataVigencia, _ajusteTabelaFrete.TabelaFrete, function (data) { RetornoConsultaVigencia(_ajusteTabelaFrete.DataVigencia, data); }, false, null, null, null, null, null, _ajusteTabelaFrete.ContratoTransporteFrete);
    new BuscarVigenciasTabelaFrete(_pesquisaAjusteTabelaFrete.DataVigencia, _pesquisaAjusteTabelaFrete.TabelaFrete, function (data) { RetornoConsultaVigencia(_pesquisaAjusteTabelaFrete.DataVigencia, data); });
    new BuscarContratosTransporteFrete(_ajusteTabelaFrete.ContratoTransporteFrete, retornoConsultaContratoTransporteFrete, null, null, _ajusteTabelaFrete.TabelaFrete);

    if (!_FormularioSomenteLeitura) {
        $('#' + _ajusteTabelaFrete.Preview.idGrid).on('click', 'tbody td', function (e) {
            e.stopPropagation();
            editCell(this);
        });
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _ajusteTabelaFrete.RotaFreteDestino.visible(true);
        _ajusteTabelaFrete.RotaFreteOrigem.visible(true);
        _ajusteTabelaFrete.Empresa.visible(!_CONFIGURACAO_TMS.ObrigatorioInformarTransportadorAjusteTabelaFrete && !_CONFIGURACAO_TMS.ObrigatorioInformarContratoTransporteFreteAjusteTabelaFrete);
        _ajusteTabelaFrete.EmpresaExclusiva.visible(_CONFIGURACAO_TMS.ObrigatorioInformarTransportadorAjusteTabelaFrete);
        _ajusteTabelaFrete.EmpresaExclusiva.required = _CONFIGURACAO_TMS.ObrigatorioInformarTransportadorAjusteTabelaFrete;
        _ajusteTabelaFrete.ContratoTransporteFrete.visible(_CONFIGURACAO_TMS.ObrigatorioInformarContratoTransporteFreteAjusteTabelaFrete);
        _ajusteTabelaFrete.ContratoTransporteFrete.required = _CONFIGURACAO_TMS.ObrigatorioInformarContratoTransporteFreteAjusteTabelaFrete;
    }

    SignalRAjusteTabelaAtualizadoEvent = AtualizarAjuste;
    SignalRAjusteTabelaAplicadoEvent = AjusteAplicado;

    BuscarAjustesTabelaFrete();
}

function TipoOperacaoChange(e, valor) {
    var title = "";
    var icon = "";
    var precision = 4;

    switch (valor) {
        case EnumTipoCampoValorTabelaFrete.ValorFixo:
            title = "Substitui o valor original pelo valor informado";
            icon = "fal fa-dollar";
            e.Valor.maxlength(10);
            break;
        case EnumTipoCampoValorTabelaFrete.AumentoValor:
            title = "Adiciona/remove do valor original o valor informado";
            icon = "fal fa-exchange";
            e.Valor.maxlength(10);
            break;
        case EnumTipoCampoValorTabelaFrete.AumentoPercentual:
            title = "Aumenta/reduz o valor original de acordo com o percentual informado";
            icon = "fal fa-percent";

            e.Valor.maxlength(7);

            break;
        default:
            title = "";
            icon = "";
            break;
    }

    e.Valor.val("");
    e.Valor.get$().maskMoney("destroy");
    e.Valor.get$().maskMoney(ConfigDecimal({ precision: precision }));

    e.Valor.title(title);
    e.TipoOperacao.title(title);
    e.TipoOperacao.icon(icon);
    e.TipoOperacao.val(valor);
}

function CriarPreviewClick() {
    Salvar(_ajusteTabelaFrete, "AjusteTabelaFrete/AbrirAjuste", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Ajuste aberto com sucesso.");
                CarregarAjusteTabelaFrete(r.Data.Codigo);
                _gridAjusteTabelaFrete.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AplicarAjustesClick() {
    var itensAjuste = new Array();
    for (var i = 0; i < _ajusteTabelaFrete.ItensAjuste().length; i++) {
        var itemAjuste = _ajusteTabelaFrete.ItensAjuste()[i];

        itensAjuste.push({
            Codigo: itemAjuste.Codigo.val(),
            Tipo: itemAjuste.Tipo.val(),
            Valor: (itemAjuste.Valor.val() == "" ? 0 : Globalize.parseFloat(itemAjuste.Valor.val())),
            TipoOperacao: itemAjuste.TipoOperacao.val(),
            Aumenta: itemAjuste.Aumenta.val(),
            Arredondar: itemAjuste.Arredondar.val()
        });
    }

    var dados = {
        Codigo: _ajusteTabelaFrete.Codigo.val(),
        Vigencia: _ajusteTabelaFrete.DataVigenciaAjuste.val() == "" ? 0 : _ajusteTabelaFrete.DataVigenciaAjuste.codEntity(),
        NovaVigenciaIndefinida: _ajusteTabelaFrete.NovaVigenciaIndefinida.val(),
        ItensAjuste: JSON.stringify(itensAjuste)
    };

    //if (_CONFIGURACAO_TMS.ObrigarVigenciaNoAjusteFrete && dados.NovaVigenciaIndefinida == "")
    //    return exibirMensagem(tipoMensagem.aviso, "Nova Vigência", "É obrigatório informar uma nova vigência.");

    executarReST("AjusteTabelaFrete/AplicarAjustes", dados, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Ajustes aplicados com sucesso!");
                _gridPreviewAjusteTabelaFrete.CarregarGrid();
                ExibirMensagemProcessamento(EnumSituacaoAjusteTabelaFrete.EmAjuste);
                SituacaoEmOperacao(true);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function RetornarValoresOriginaisClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente retornar os ajustes aos valores originais?", function () {
        executarReST("AjusteTabelaFrete/ResetarAjustes", { Codigo: _ajusteTabelaFrete.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Valores resetados com sucesso.");
                    _gridPreviewAjusteTabelaFrete.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function ReprocessarAjusteClick() {
    executarReST("AjusteTabelaFrete/ReprocessarAjuste", { Codigo: _ajusteTabelaFrete.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado para Reprocessamento.");
                LimparCamposAjusteTabelaFrete();
                _gridAjusteTabelaFrete.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function ReprocessarCriacaoClick() {
    executarReST("AjusteTabelaFrete/ReprocessarCriacao", { Codigo: _ajusteTabelaFrete.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação Enviada");
                LimparCamposAjusteTabelaFrete();
                _gridAjusteTabelaFrete.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function ReprocessarAjusteValoresClick() {
    executarReST("AjusteTabelaFrete/ReprocessarAjusteValores", { Codigo: _ajusteTabelaFrete.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação Enviada");
                LimparCamposAjusteTabelaFrete();
                _gridAjusteTabelaFrete.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function BuscarValoresSemPararClick() {
    executarReST("AjusteTabelaFrete/BuscarValoresSemParar", { Codigo: _ajusteTabelaFrete.Codigo.val() }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", r.Msg);
            _gridPreviewAjusteTabelaFrete.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function FinalizarAjustesClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente finalizar o ajuste e aplicar às tabelas de frete? <b>Este processo é irreversível.</b>", function () {
        executarReST("AjusteTabelaFrete/FinalizarAjuste", { Codigo: _ajusteTabelaFrete.Codigo.val(), NovaVigenciaIndefinida: _ajusteTabelaFrete.NovaVigenciaIndefinida.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ajuste finalizado com sucesso.");
                    LimparCamposAjusteTabelaFrete();
                    _gridAjusteTabelaFrete.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function CancelarAjustesClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente cancelar o ajuste? <b>Este processo é irreversível.</b>", function () {
        executarReST("AjusteTabelaFrete/CancelarAjuste", { Codigo: _ajusteTabelaFrete.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ajuste cancelado com sucesso.");
                    LimparCamposAjusteTabelaFrete();
                    _gridAjusteTabelaFrete.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function TrocarTabelaFrete(codigoTabelaFrete) {
    $("#divConteudoRelatorio").html("");

    if (_gridPreviewAjusteTabelaFrete != null) {
        _gridPreviewAjusteTabelaFrete.Destroy();
        _gridPreviewAjusteTabelaFrete = null;
        $("#" + _ajusteTabelaFrete.Preview.idGrid).html("");
    }

    if (codigoTabelaFrete > 0) {
        VerificarAjustarPedagiosComSemParar(codigoTabelaFrete);

        _gridPreviewAjusteTabelaFrete = new GridView(_ajusteTabelaFrete.Preview.idGrid, "AjusteTabelaFrete/Pesquisa", _ajusteTabelaFrete);
        _gridPreviewAjusteTabelaFrete.SetPermitirEdicaoColunas(true);
        _gridPreviewAjusteTabelaFrete.SetQuantidadeLinhasPorPagina(15);


        _relatorioConsultaTabelaFrete = new RelatorioGlobal("AjusteTabelaFrete/BuscarDadosRelatorio", _gridPreviewAjusteTabelaFrete, function () {
            _relatorioConsultaTabelaFrete.loadRelatorio(function () {
                //if (_ajusteTabelaFrete.DataVigencia.codEntity() > 0)
                _gridPreviewAjusteTabelaFrete.CarregarGrid();

                $("#divConteudoRelatorio").show();

                _relatorioConsultaTabelaFrete.obterKnoutRelatorio().Report.visibleFade(true);
                _relatorioConsultaTabelaFrete.obterKnoutRelatorio().Report.visible(false);
                _relatorioConsultaTabelaFrete.obterKnoutRelatorio().ExibirSumarios.visible(false);
            });

        }, { TabelaFrete: _ajusteTabelaFrete.TabelaFrete.codEntity() }, null, _ajusteTabelaFrete, false);
    }
    else {
        $("#" + _ajusteTabelaFrete.Preview.idGrid).html("");
        _ajusteTabelaFrete.ConfiguracaoRelatorio.visibleFade(false);
        _ajusteTabelaFrete.ConfiguracaoRelatorio.icon("fal fa-plus");
        _ajusteTabelaFrete.AjustarPedagiosComSemParar.val(false);
        _ajusteTabelaFrete.AjustarPedagiosComSemParar.visible(false);
    }

    limparDadosContratoTransporteFrete();
    _ajusteTabelaFrete.DataVigencia.val("");
    _ajusteTabelaFrete.DataVigencia.codEntity(0);
}

function RetornoConsultaVigencia(knout, data) {
    knout.val("De " + data.DataInicial + (data.DataFinal != "" ? " até " + data.DataFinal : ""));
    knout.entityDescription(knout.val());
    knout.codEntity(data.Codigo);
}

function retornoConsultaContratoTransporteFrete(registroSelecionado) {
    _ajusteTabelaFrete.ContratoTransporteFrete.codEntity(registroSelecionado.Codigo);
    _ajusteTabelaFrete.ContratoTransporteFrete.val(registroSelecionado.Descricao);
    _ajusteTabelaFrete.ContratoTransporteFrete.entityDescription(registroSelecionado.Descricao);
    _ajusteTabelaFrete.EmpresaExclusiva.codEntity(registroSelecionado.CodigoTransportador);
    _ajusteTabelaFrete.EmpresaExclusiva.val(registroSelecionado.Transportador);
    _ajusteTabelaFrete.EmpresaExclusiva.entityDescription(registroSelecionado.Transportador);
    _ajusteTabelaFrete.DataInicialContrato.val(registroSelecionado.DataInicio);
    _ajusteTabelaFrete.DataFinalContrato.val(registroSelecionado.DataFim);

    _ajusteTabelaFrete.DataVigencia.val("");
}

function contratoTransporteFreteBlur() {
    if (_ajusteTabelaFrete.ContratoTransporteFrete.val() == "") {
        LimparCampoEntity(_ajusteTabelaFrete.EmpresaExclusiva);
        _ajusteTabelaFrete.DataInicialContrato.val("");
        _ajusteTabelaFrete.DataFinalContrato.val("");
    }
}

function limparDadosContratoTransporteFrete() {
    if (!_ajusteTabelaFrete.ContratoTransporteFrete.visible())
        return;

    LimparCampoEntity(_ajusteTabelaFrete.ContratoTransporteFrete);
    LimparCampoEntity(_ajusteTabelaFrete.EmpresaExclusiva);
    _ajusteTabelaFrete.DataInicialContrato.val("");
    _ajusteTabelaFrete.DataFinalContrato.val("");
}

function GerarPreviewClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_ajusteTabelaFrete);

    if (valido) {
        _gridPreviewAjusteTabelaFrete.CarregarGrid();

        if (_gridPreviewAjusteTabelaFrete.NumeroRegistros() === 0)
            exibirMensagem(tipoMensagem.aviso, "Nenhuma Tabela de Frete encontrada", "Verifique as Tabelas de Frete Cliente disponíveis para os filtros informados");
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Verifique os campos obrigatórios!");
}

function GerarRelatorioPDFClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_ajusteTabelaFrete);

    if (valido)
        _relatorioConsultaTabelaFrete.gerarRelatorio("AjusteTabelaFrete/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
    else
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Verifique os campos obrigatórios!");
}

function GerarRelatorioExcelClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_ajusteTabelaFrete);

    if (valido)
        _relatorioConsultaTabelaFrete.gerarRelatorio("AjusteTabelaFrete/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
    else
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Verifique os campos obrigatórios!");
}

function LimparFiltrosClick() {
    LimparCampos(_ajusteTabelaFrete);
}

function LimparCamposAjusteTabelaFrete() {
    LimparCampos(_ajusteTabelaFrete);
    _CRUDRelatorio.AbrirAjuste.visible(true);
    _CRUDRelatorio.GerarRelatorioPDF.visible(false);
    _CRUDRelatorio.GerarRelatorioExcel.visible(false);
    _CRUDRelatorio.FinalizarAjustes.visible(false);
    _CRUDRelatorio.GerarSimulacao.visible(false);
    _CRUDRelatorio.Importar.visible(false);
    _CRUDRelatorio.CancelarAjustes.visible(false);
    _CRUDRelatorio.ReprocessarAjuste.visible(false);
    _CRUDRelatorio.ReprocessarCriacao.visible(false);
    _CRUDRelatorio.ReprocessarAjusteValores.visible(false);
    _CRUDRelatorio.BuscarValoresSemParar.visible(false);
    _ajusteTabelaFrete.AplicarAjustes.visible(false);
    _ajusteTabelaFrete.AplicarAjustes.enable(true);
    _ajusteTabelaFrete.LimparFiltros.visible(true);

    _ajusteTabelaFrete.TabelaFrete.enable(true);
    _ajusteTabelaFrete.DataVigencia.enable(true);
    _ajusteTabelaFrete.MotivoReajuste.enable(true);
    _ajusteTabelaFrete.LocalidadeOrigem.enable(true);
    _ajusteTabelaFrete.LocalidadeDestino.enable(true);
    _ajusteTabelaFrete.RegiaoDestino.enable(true);
    _ajusteTabelaFrete.CanalEntrega.enable(true);
    _ajusteTabelaFrete.CanalVenda.enable(true);
    _ajusteTabelaFrete.RegiaoOrigem.enable(true);
    _ajusteTabelaFrete.EstadoDestino.enable(true);
    _ajusteTabelaFrete.EstadoOrigem.enable(true);
    _ajusteTabelaFrete.TipoCarga.enable(true);
    _ajusteTabelaFrete.ModeloReboque.enable(true);
    _ajusteTabelaFrete.ModeloTracao.enable(true);
    _ajusteTabelaFrete.Remetente.enable(true);
    _ajusteTabelaFrete.Destinatario.enable(true);
    _ajusteTabelaFrete.Tomador.enable(true);
    _ajusteTabelaFrete.TipoOperacao.enable(true);
    _ajusteTabelaFrete.RotaFreteDestino.enable(true);
    _ajusteTabelaFrete.RotaFreteOrigem.enable(true);
    _ajusteTabelaFrete.Empresa.enable(true);
    _ajusteTabelaFrete.EmpresaExclusiva.enable(true);
    _ajusteTabelaFrete.ContratoTransporteFrete.enable(true);
    _ajusteTabelaFrete.TabelaComCargaRealizada.enable(true);
    _ajusteTabelaFrete.SomenteRegistrosComValores.enable(true);
    _ajusteTabelaFrete.TipoPagamento.enable(true);
    _ajusteTabelaFrete.TipoRegistro.visible(false);
    _ajusteTabelaFrete.UtilizarBuscaNasLocalidadesPorEstadoDestino.enable(true);
    _ajusteTabelaFrete.UtilizarBuscaNasLocalidadesPorEstadoOrigem.enable(true);
    _ajusteTabelaFrete.AjustarPedagiosComSemParar.visible(false);
    _ajusteTabelaFrete.AjustarPedagiosComSemParar.enable(true);
    _ajusteTabelaFrete.Mensagem.visible(false);

    // Aprovadores
    _detalhesAjuste.MensagemEtapaSemRegra.visible(false);

    setarEtapaInicioAjuste();
}

function LimparItensAjusteTabelaFrete() {
    _ajusteTabelaFrete.DataVigenciaAjuste.codEntity(0);
    _ajusteTabelaFrete.DataVigenciaAjuste.val("");

    for (var i = 0; i < _ajusteTabelaFrete.ItensAjuste().length; i++) {
        var itemAjuste = _ajusteTabelaFrete.ItensAjuste()[i];

        itemAjuste.Valor.val("");
        itemAjuste.Aumenta.val(true);
        itemAjuste.Aumenta.title("Aumento de Valor");
        itemAjuste.Aumenta.icon("fal fa-plus");

        TipoOperacaoChange(itemAjuste, EnumTipoCampoValorTabelaFrete.AumentoPercentual);
    }
}

function BuscarAjustesTabelaFrete() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarAjusteTabelaFrete, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    var configExportacao = {
        url: "AjusteTabelaFrete/ExportarPesquisa",
        titulo: "Ajustes de Tabela Frete"
    };

    _gridAjusteTabelaFrete = new GridViewExportacao(_pesquisaAjusteTabelaFrete.Pesquisar.idGrid, "AjusteTabelaFrete/PesquisaAjuste", _pesquisaAjusteTabelaFrete, menuOpcoes, configExportacao, { column: 1, dir: orderDir.desc });
    _gridAjusteTabelaFrete.CarregarGrid();
}

function EditarAjusteTabelaFrete(ajusteTabelaFreteGrid) {
    LimparCamposAjusteTabelaFrete();

    CarregarAjusteTabelaFrete(ajusteTabelaFreteGrid.Codigo);

    _ajusteTabelaFrete.BuscaAvancada.visibleFade(false);
    _ajusteTabelaFrete.BuscaAvancada.icon("fal fa-plus");
    _pesquisaAjusteTabelaFrete.ExibirFiltros.visibleFade(false);
}

function AjusteAplicado(data) {
    if (_ajusteTabelaFrete.Codigo.val() == data.Codigo) {
        _gridPreviewAjusteTabelaFrete.CarregarGrid();
        SituacaoEmOperacao(false);
        ExibirGirdTabelas();
    }
}

function AtualizarAjuste(data) {
    if (_ajusteTabelaFrete.Codigo.val() == data.Codigo) {
        _gridPreviewAjusteTabelaFrete.CarregarGrid();
        CarregarAjusteTabelaFrete(data.Codigo);
    }
}

function CarregarAjusteTabelaFrete(codigoAjusteTabelaFrete) {
    _ajusteTabelaFrete.Codigo.val(codigoAjusteTabelaFrete);
    BuscarPorCodigo(_ajusteTabelaFrete, "AjusteTabelaFrete/BuscarPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {
                var situacao = r.Data.Situacao;

                // Etapas do ajuste
                setarEtapaAjuste();

                // Detalhes do ajuste
                if (r.Data.Detalhes != null) PreencherObjetoKnout(_detalhesAjuste, { Data: r.Data.Detalhes });
                _detalhesAjuste.DetalhesAjuste.visible(r.Data.Detalhes != null);

                // Aprovadores
                _gridAutorizacoes.CarregarGrid();

                // Integração
                MensagemIntegracao();

                _ajusteTabelaFrete.TabelaFrete.enable(false);
                _ajusteTabelaFrete.DataVigencia.enable(false);
                _ajusteTabelaFrete.MotivoReajuste.enable(false);
                _ajusteTabelaFrete.LocalidadeOrigem.enable(false);
                _ajusteTabelaFrete.LocalidadeDestino.enable(false);
                _ajusteTabelaFrete.RegiaoDestino.enable(false);
                _ajusteTabelaFrete.CanalEntrega.enable(false);
                _ajusteTabelaFrete.CanalVenda.enable(false);
                _ajusteTabelaFrete.RegiaoOrigem.enable(false);
                _ajusteTabelaFrete.EstadoDestino.enable(false);
                _ajusteTabelaFrete.EstadoOrigem.enable(false);
                _ajusteTabelaFrete.TipoCarga.enable(false);
                _ajusteTabelaFrete.ModeloReboque.enable(false);
                _ajusteTabelaFrete.RotaFreteDestino.enable(false);
                _ajusteTabelaFrete.RotaFreteOrigem.enable(false);
                _ajusteTabelaFrete.Empresa.enable(false);
                _ajusteTabelaFrete.EmpresaExclusiva.enable(false);
                _ajusteTabelaFrete.ContratoTransporteFrete.enable(false);
                _ajusteTabelaFrete.ModeloTracao.enable(false);
                _ajusteTabelaFrete.Remetente.enable(false);
                _ajusteTabelaFrete.Destinatario.enable(false);
                _ajusteTabelaFrete.Tomador.enable(false);
                _ajusteTabelaFrete.TipoOperacao.enable(false);
                _ajusteTabelaFrete.TabelaComCargaRealizada.enable(false);
                _ajusteTabelaFrete.SomenteRegistrosComValores.enable(false);
                _ajusteTabelaFrete.TipoPagamento.enable(false);
                _ajusteTabelaFrete.LimparFiltros.visible(false);
                _ajusteTabelaFrete.UtilizarBuscaNasLocalidadesPorEstadoDestino.enable(false);
                _ajusteTabelaFrete.UtilizarBuscaNasLocalidadesPorEstadoOrigem.enable(false);
                _ajusteTabelaFrete.TipoRegistro.visible(true);
                _ajusteTabelaFrete.AjustarPedagiosComSemParar.enable(false);

                _CRUDRelatorio.AbrirAjuste.visible(false);
                _CRUDRelatorio.GerarRelatorioExcel.visible(true);
                _CRUDRelatorio.GerarRelatorioPDF.visible(true);
                _CRUDRelatorio.ReprocessarAjuste.visible(false);
                _CRUDRelatorio.ReprocessarCriacao.visible(false);
                _CRUDRelatorio.ReprocessarAjusteValores.visible(false);

                _ajusteTabelaFrete.ItensAjuste.removeAll();

                if (situacao == EnumSituacaoAjusteTabelaFrete.EmAjuste || situacao == EnumSituacaoAjusteTabelaFrete.EmCriacao || situacao == EnumSituacaoAjusteTabelaFrete.EmProcessamento)
                    ExibirMensagemProcessamento(situacao);
                else if (situacao == EnumSituacaoAjusteTabelaFrete.ProblemaAjuste || situacao == EnumSituacaoAjusteTabelaFrete.ProblemaCriacao || situacao == EnumSituacaoAjusteTabelaFrete.ProblemaProcessamento)
                    ExibirMensagemProblema(situacao);
                else
                    ExibirGirdTabelas();

                if (situacao == EnumSituacaoAjusteTabelaFrete.Pendente) {
                    _ajusteTabelaFrete.AplicarAjustes.visible(true);
                    _CRUDRelatorio.FinalizarAjustes.visible(true);
                    //_CRUDRelatorio.GerarSimulacao.visible(true);
                    _CRUDRelatorio.Importar.visible(true);
                    _CRUDRelatorio.CancelarAjustes.visible(true);

                    for (var i = 0; i < r.Data.ItensAjuste.length; i++) {
                        var item = r.Data.ItensAjuste[i];

                        var itemAjuste = new ItemAjuste();
                        itemAjuste.Codigo.val(item.Codigo);
                        itemAjuste.Descricao.val(item.Descricao + ":");
                        itemAjuste.Tipo.val(item.Tipo);

                        TipoOperacaoChange(itemAjuste, EnumTipoCampoValorTabelaFrete.AumentoPercentual);

                        if (item.Tipo == EnumTipoParametroAjusteTabelaFrete.ComponenteFrete) {
                            itemAjuste.TipoComponente.val(item.TipoComponente);
                            if (item.TipoComponente == EnumTipoComponenteFrete.ADVALOREM) {
                                itemAjuste.ValorFixo.visible(false);
                                itemAjuste.ValorParcial.visible(false);
                            }
                        }

                        _ajusteTabelaFrete.ItensAjuste.push(itemAjuste);

                        $("#" + itemAjuste.Valor.id).maskMoney(ConfigDecimal({ precision: 4 }));
                    }

                    if (_ajusteTabelaFrete.AjustarPedagiosComSemParar.val()) _CRUDRelatorio.BuscarValoresSemParar.visible(true);
                }
                else if (situacao == EnumSituacaoAjusteTabelaFrete.EmCriacao) {
                    _CRUDRelatorio.GerarRelatorioExcel.visible(false);
                    _CRUDRelatorio.GerarRelatorioPDF.visible(false);
                }
                else if (situacao == EnumSituacaoAjusteTabelaFrete.ProblemaCriacao) {
                    _CRUDRelatorio.GerarRelatorioExcel.visible(false);
                    _CRUDRelatorio.GerarRelatorioPDF.visible(false);
                    _CRUDRelatorio.ReprocessarCriacao.visible(true);
                }
                else if (situacao == EnumSituacaoAjusteTabelaFrete.ProblemaProcessamento) {
                    _CRUDRelatorio.ReprocessarAjuste.visible(true);
                }
                else if (situacao == EnumSituacaoAjusteTabelaFrete.ProblemaAjuste) {
                    _CRUDRelatorio.ReprocessarAjusteValores.visible(true);
                }
                else if (situacao == EnumSituacaoAjusteTabelaFrete.Finalizado) {
                    if (_CONFIGURACAO_TMS.ObrigarVigenciaNoAjusteFrete) {
                        _ajusteTabelaFrete.AplicarAjustes.visible(true);
                        _ajusteTabelaFrete.AplicarAjustes.enable(false);
                    }
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

var codigoItemEdicao = null;
var valorItemEdicao = null;
var tipoValorItemEdicao = null;
var htmlItemEdicao = null;

function editCell(cell) {
    var $span = $(cell).find("span").first();
    var codigoItemEdicao = $span.data("codigoItem");
    var tipoValorItemEdicao = $span.data("tipoValor");
    var infoEdicao = $span.data("info");
    var precision = _CONFIGURACAO_TMS.TabelaFretePrecisaoDinheiroDois ? 2 : 4;

    if (codigoItemEdicao == null || tipoValorItemEdicao == null)
        return;

    if (_ajusteTabelaFrete.Codigo.val() == 0)
        return;

    if (_ajusteTabelaFrete.Situacao.val() != EnumSituacaoAjusteTabelaFrete.Pendente)
        return;

    var idTxt = guid();

    htmlItemEdicao = cell.innerHTML;
    valorItemEdicao = Globalize.parseFloat(cell.innerHTML.split("</span>")[1].trim());
    cell.innerHTML = '<input id="' + idTxt + '" type="text" value="' + Globalize.format(valorItemEdicao, "n" + precision) + '" style="width: 100%; height: 100%;" />';

    $("#" + idTxt).maskMoney(ConfigDecimal({ precision: precision }));

    switch (tipoValorItemEdicao) {
        case EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal:

            $("#" + idTxt).attr("maxlength", "6");

            break;
        case EnumTipoCampoValorTabelaFrete.AumentoPercentual:

            $("#" + idTxt).attr("maxlength", "7");

            break;
        case EnumTipoCampoValorTabelaFrete.ValorFixo:
        case EnumTipoCampoValorTabelaFrete.AumentoValor:

            $("#" + idTxt).attr("maxlength", "12");

            break;
        default:
            break;
    }

    var urlAlteracao = "AjusteTabelaFrete/SalvarValorItem";
    if (infoEdicao != "")
        urlAlteracao = "AjusteTabelaFrete/SalvarValorFixo";

    $("#" + idTxt).focus();
    $("#" + idTxt).focusout(function () {
        var valor = Globalize.parseFloat($("#" + idTxt).val());

        var dados = {
            CodigoItem: codigoItemEdicao,
            ValorItem: $("#" + idTxt).val(),
            Info: infoEdicao,
            Ajuste: _ajusteTabelaFrete.Codigo.val()
        };

        if (isNaN(valor) || valor == valorItemEdicao) {
            $(this).closest("td").html(htmlItemEdicao);
            htmlItemEdicao = null;
            valorItemEdicao = null;
            tipoValorItemEdicao = null;
            codigoItemEdicao = null;
        }
        else {
            exibirConfirmacao("Atenção!", "A consulta será recarregada, pois esta alteração pode afetar mais de uma linha na exibição. Deseja continuar?", function () {
                executarReST(urlAlteracao, dados, function (retorno) {
                    if (retorno.Success && retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso!", "Valores atualizados com sucesso!");
                        _gridPreviewAjusteTabelaFrete.CarregarGrid();
                    }
                    else {
                        if (retorno.Success)
                            exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                        else
                            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);

                        $("#" + idTxt).closest("td").html(htmlItemEdicao);
                        htmlItemEdicao = null;
                        valorItemEdicao = null;
                        tipoValorItemEdicao = null;
                        codigoItemEdicao = null;
                    }
                });
            },
                function () {
                    $("#" + idTxt).closest("td").html(htmlItemEdicao);
                    htmlItemEdicao = null;
                    valorItemEdicao = null;
                    tipoValorItemEdicao = null;
                    codigoItemEdicao = null;
                });
        }
    });
}

function SituacaoEmOperacao(ativo) {
    _ajusteTabelaFrete.AplicarAjustes.visible(!ativo);
    _ajusteTabelaFrete.RetornarValoresOriginais.visible(!ativo);
}

function ExibirMensagemProcessamento(situacao) {
    var msg = "";
    var warning = false;
    var danger = false;
    var success = false;

    if (situacao == EnumSituacaoAjusteTabelaFrete.EmAjuste) {
        msg = "O sistema está aplicando os ajuste. Aguarde a finalização.";
        warning = true;
    } else if (situacao == EnumSituacaoAjusteTabelaFrete.EmCriacao) {
        msg = "O sistema está criando o ajuste. Aguarde a finalização.";
        success = true;
    } else if (situacao == EnumSituacaoAjusteTabelaFrete.EmProcessamento) {
        msg = "O sistema está finalizando as alterações.";
        warning = true;
    }

    _ajusteTabelaFrete.Mensagem
        .warning(warning)
        .danger(danger)
        .success(success)
        .val(msg)
        .visible(true)
    _ajusteTabelaFrete.Preview.visible(false);
}

function ExibirMensagemProblema(situacao) {
    var msg = "";
    var warning = false;
    var danger = false;
    var success = false;

    if (situacao == EnumSituacaoAjusteTabelaFrete.ProblemaAjuste) {
        msg = "Ocorreu uma falha ao aplicar os ajustes.";
        danger = true;
    } else if (situacao == EnumSituacaoAjusteTabelaFrete.ProblemaCriacao) {
        msg = "Ocorreu uma falha ao criar o Ajuste. Verifique as Tabelas de Frete Cliente disponíveis para os filtros informados";
        danger = true;
    } else if (situacao == EnumSituacaoAjusteTabelaFrete.ProblemaProcessamento) {
        msg = "Ocorreu uma falha ao finalizar o ajuste.";
        danger = true;
    }

    _ajusteTabelaFrete.Mensagem
        .warning(warning)
        .danger(danger)
        .success(success)
        .val(msg)
        .visible(true)
    _ajusteTabelaFrete.Preview.visible(false);
}

function ExibirGirdTabelas() {
    _ajusteTabelaFrete.Preview.visible(true);
}

function VerificarAjustarPedagiosComSemParar(codigo) {
    _ajusteTabelaFrete.AjustarPedagiosComSemParar.val(false);
    executarReST("AjusteTabelaFrete/VerificarAjustarPedagiosComSemParar", { Codigo: codigo }, function (r) {
        if (r.Success) {
            _ajusteTabelaFrete.AjustarPedagiosComSemParar.visible(r.Data);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}