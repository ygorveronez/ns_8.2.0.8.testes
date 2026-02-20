/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />
/// <reference path="../../Enumeradores/EnumPrioridadeMontagemCarregamentoPedidoProduto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _formatterCurrency = new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
});

var _HTMLDetalheCarregamento;
var _AreaCarregamento;
var _Carregamentos;
var _detalheCarregamento;
var _percentualCarregamentoAutomatico;
var _percentualCargaEmLote;

var _selecaoCarregamentos;
var _gridSelecaoCarregamentos;

var _parametrosMontagemCarregamentoPedidoProduto;
var _parametrosMontagemCarregamentoPrioridades;

var _restricoesClientesCarregamento;
var _gridRestricoesClientesCarregamento;

var _cancelarGerarNovamenteCarregamentosAutomaticos;

var PercentualCarregamentoAutomatico = function () {
    this.Descricao = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.PercentualProcessado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var PercentualCargaEmLote = function () {
    this.PercentualProcessado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var TipoMontagemCarregamentoPedidoProduto = function () {
    this.Tipo = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDeSelecaoDosProdutosDosPedidos.getRequiredFieldDescription(), val: ko.observable(EnumTipoMontagemCarregamentoPedidoProduto.Ambos), options: EnumTipoMontagemCarregamentoPedidoProduto.obterOpcoes(), def: EnumTipoMontagemCarregamentoPedidoProduto.Ambos });
    this.Prioridade = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PrioridadeCarregamentoFilaPedidoProdutos.getRequiredFieldDescription(), val: ko.observable(EnumPrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaLinhaSeparacaoPedido), options: EnumPrioridadeMontagemCarregamentoPedidoProduto.obterOpcoes(), def: EnumPrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaLinhaSeparacaoPedido });
    this.TipoStatusEstoque = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoStatusEstoque.getRequiredFieldDescription(), val: ko.observable(EnumTipoStatusEstoqueMontagemCarregamentoPedidoProduto.Ambos), options: EnumTipoStatusEstoqueMontagemCarregamentoPedidoProduto.obterOpcoes(), def: EnumTipoStatusEstoqueMontagemCarregamentoPedidoProduto.Ambos });
    this.GerarCarregamentos = PropertyEntity({ eventClick: gerarCarregamentosTipoClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.GerarCarregamentos) });
}

var TipoMontagemCarregamentoPrioridades = function () {
    this.Prioridade = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PrioridadeCarregamentoFilaPedido.getRequiredFieldDescription(), val: ko.observable(EnumPrioridadeMontagemCarregamentoPedido.PrevisaoEntregaCanalEntrega), options: EnumPrioridadeMontagemCarregamentoPedido.obterOpcoes(), def: EnumPrioridadeMontagemCarregamentoPedido.PrevisaoEntregaCanalEntrega });
    this.GerarCarregamentos = PropertyEntity({ eventClick: gerarCarregamentosTipoPrioridadesClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.GerarCarregamentos) });
}

var RestricoesClientesCarregamento = function () {
    this.GridRestricoesClientesCarregamento = PropertyEntity({ type: types.local });
}

var _pesquisaResumoCarregamentos;
var _resumoCarregamentos;
//Grid resumo por montagem pedido produto.

var _gridResumoCarregamentos;
//Grids demais tipos de viagens.
var _gridFrota;
var _gridResumo;
var _gridResumoValorFrete;

var FiltroResumoCarregamentos = function () {
    this.Reload = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.SessaoRoteirizador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var ResumoCarregamentos = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarResumoCarregamentosClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.AtualizarResumo, visible: ko.observable(true), enable: ko.observable(true) });
    //Grid para montagem pedido produto ASSAI
    this.Grid = PropertyEntity({ type: types.local, visible: ko.observable(true) });

    //Grid para demais tipos de roteriização exceto Simulador frete TELHANORTE
    this.GridFrota = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.GridResumo = PropertyEntity({ type: types.local, visible: ko.observable(false) });

    //Grid resumo por valor de frete.. SAINTGOBAIN
    this.GridResumoValorFrete = PropertyEntity({ type: types.local, visible: ko.observable(false) });
}

var SelecaoCarregamentos = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.GerarCargas = PropertyEntity({ eventClick: gerarCargasSelecaoCarregamentosClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.GerarCargas, visible: ko.observable(true), enable: ko.observable(true) });
}

var _knoutsCarregamentos = new Array();
var _carregamentoVisivelMapa = new Array();

var AreaCarregamento = function () {
    this.Carregamentos = PropertyEntity();
    this.CarregandoCarregamentos = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Total = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, eventChange: CarregamentosPesquisaScroll });
    this.TotalCarregamentos = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.MontagemCargaMapa.Carregamentos.getFieldDescription() });
    this.TotalKm = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.MontagemCargaMapa.KMTotal.getFieldDescription() });
    this.VolumeKm = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.MontagemCargaMapa.VolumeBarraKM.getFieldDescription() });
    this.ExibirTodosCarregamentoMapa = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: false, eventChange: exibirCarregamentosMapaChange });
    this.DesenharPolilinhaRotaApartirOrigem = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: false, eventChange: desenharPolilinhaRotaApartirOrigemChange });
    this.ExibirPolilinhaRotaQuandoRoteirizado = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: false, eventChange: desenharPolilinhaRotaQuandoRoteirizadoChange });
    this.CentralizarCarregamentoMapaAoSelecionar = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.FiltoCarregamentos = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Filtrar, title: Localization.Resources.Cargas.MontagemCargaMapa.FiltroDeCarregamentos, eventClick: filtroCarregamentosClick, type: types.event });
    this.OtimizarTodos = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Otimizar, title: Localization.Resources.Cargas.MontagemCargaMapa.OtimizarTodosOsCarregamentos, eventClick: otimizarTodosCarregamentosClick, type: types.event, visible: ko.observable(false) });
    this.GerarCarregamentoAutomatico = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Carregamentos, title: Localization.Resources.Cargas.MontagemCargaMapa.GerarCarregamentosAutomaticos, eventClick: gerarCarregamentosClick, type: types.event, visible: ko.observable(true) });
    this.CancelarTodosCarregamentos = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Cancelar, title: Localization.Resources.Cargas.MontagemCargaMapa.CancelarTodosOsCarregamentos, eventClick: cancelarTodosCarregamentoClick, type: types.event, visible: ko.observable(false) });
    this.CancelarGerarTodosCarregamentos = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CancelarGerarCarregamentos, title: Localization.Resources.Cargas.MontagemCargaMapa.CancelarGerarTodosOsCarregamentos, eventClick: cancelarGerarCarregamentosClick, type: types.event, visible: ko.observable(false) });
    this.GerarCargaEmLote = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Cargas, title: Localization.Resources.Cargas.MontagemCargaMapa.GerarCargasDosCarregamentos, eventClick: gerarCargaEmLoteClick, type: types.event, visible: ko.observable(false) });

    this.TotalCarregamentos.val.subscribe(function (newValue) {
        if (_AreaCarregamento != null) {
            _AreaCarregamento.CancelarGerarTodosCarregamentos.visible(newValue > 0);
        }
    });
};

var DetalheCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.Carregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDoCarregamento.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true) });
    this.CarregamentoRedespacho = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Cargas.MontagemCargaMapa.Redespacho });
    this.GerandoCargaBackground = PropertyEntity({ val: ko.observable(false), text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.GerandoCarga), cssClass: ko.observable("ribbon-tms ribbon-tms-green") });
    this.NumeroCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroCarregamento.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.Filial = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Filial.getFieldDescription(), val: ko.observable("") });
    this.QuantidadeEntregras = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeEntregas.getFieldDescription(), val: ko.observable("") });
    this.Rotas = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Rotas.getFieldDescription(), val: ko.observable("") });
    this.Transportador = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Transportador.getFieldDescription(), val: ko.observable("") });
    this.ModeloVeicularCarga = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ModeloVeicular.getFieldDescription(), val: ko.observable("") });

    this.Distancia = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Distancia.getFieldDescription(), val: ko.observable("") });
    this.TempoDeViagemEmMinutos = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Tempo.getFieldDescription(), val: ko.observable("") });
    this.QtdeEntregasPedidos = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.EBarraP.getFieldDescription(), val: ko.observable("") });
    this.AlertaLimiteEntregas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), type: types.local, cssClass: ko.observable("alerta-limite-entregas blink"), tooltip: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.LimiteDeEntregasDoCentroDeCarregamentoExcedido) });
    this.PossuiProdutoPalletFechado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), type: types.local, cssClass: ko.observable("alerta-produto-pallet-fechado blink"), tooltip: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoComProdutoPalletFechado) });

    this.Peso = PropertyEntity({ id: guid(), text: Localization.Resources.Cargas.MontagemCargaMapa.Peso.getFieldDescription(), val: ko.observable("") });
    this.Pallets = PropertyEntity({ id: guid(), text: Localization.Resources.Cargas.MontagemCargaMapa.Pallets.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.Cubagem = PropertyEntity({ id: guid(), text: Localization.Resources.Cargas.MontagemCargaMapa.Cubagem.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });

    this.CapacidadePeso = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CapacidadePallets = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CapacidadeCubagem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.OcupacaoPeso = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.OcupacaoPeso.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.OcupacaoPallets = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.OcupacaoPallets.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.OcupacaoCubagem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.OcupacaoCubagem.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });

    //Canal de entrega dos pedidos
    this.TiposDePedidos = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array() });

    //Balanças
    this.Balancas = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array() });
    this.PracasPedagio = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array() });
    this.RestricoesClientes = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), eventClick: detalharRestricoesClientesClick });

    this.ProgressPeso = PropertyEntity({ id: guid() });
    this.ProgressPallets = PropertyEntity({ id: guid() });
    this.ProgressCubagem = PropertyEntity({ id: guid() });

    this.DataProgramada = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DataProgramada.getFieldDescription(), val: ko.observable("") });

    //Cores 
    this.CategoriasPessoas = PropertyEntity({ type: types.local, val: ko.observableArray([]), def: [], visible: ko.observable(false) });

    this.ValorFreteSimulado = PropertyEntity({ id: guid(), text: Localization.Resources.Cargas.MontagemCargaMapa.ValorFrete.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });

    // Apresenta quando sessão d erotierização por Simulação de Frete...
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoOperacao.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.ValorFreteMontagem = PropertyEntity({ id: guid(), text: Localization.Resources.Cargas.MontagemCargaMapa.ValorFrete.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });

    //Check-box para exibir/ocultar carregamento do mapa...change: exibirCarregamentoMapaChange, 
    this.ExibirCarregamentoMapa = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: false, text: "", eventChange: exibirCarregamentoMapaChange });

    this.InfoCarregamento = PropertyEntity({ eventClick: detalharCarregamentoClick, eventRightClick: menuCarregamentoRightClick, type: types.event, cssClass: ko.observable("card card-carga no-padding padding-5") });
};

function formatPercentOcupacao(value, info) {
    var tmp = Globalize.format(value, "n4");
    tmp = tmp + '%';
    if (info != null && info != undefined)
        tmp = info + ' : ' + tmp;
    return tmp;
}

function formatPercentOcupacaoProgress(value) {
    var tmp = parseInt(value) + '%';
    return tmp;
}

function formatTitlePracasPedagio(pracas) {
    if (pracas == undefined || pracas == null) return '';

    var result = '';
    var valor = 0;
    for (var i = 0; i < pracas.length; i++) {
        if (result.length > 0) result += ', ';
        result += pracas[i].Descricao;
        valor += pracas[i].Valor;
    }
    if (result.length > 0)
        return result + ' = ' + _formatterCurrency.format(valor);
    else
        return result;
}

function oupacaoTipoPedidos(tipo) {
    var cod_carregamento = parseInt(tipo.carregamento);
    var canal_entrega = parseInt(tipo.value);
    var index = obterIndiceKnoutCarregamento(cod_carregamento);
    if (index >= 0) {
        //Progress total da carga
        if (canal_entrega == 0) {
            var idx = obterIndiceCarregamentoCodigo(cod_carregamento);
            var carregamento = _Carregamentos[idx];
            ajustarPesosCapacidades(carregamento.Carregamento);
        } else {
            var data = { Carregamento: cod_carregamento, CanalEntrega: canal_entrega };
            executarReST("MontagemCarga/OcupacaoCarregamento", data, function (arg) {
                if (arg.Success) {
                    if (arg.Data != null) {
                        var info = arg.Data;
                        _knoutsCarregamentos[index].Peso.val(n2(info.TotalPesoCanalEntrega));
                        _knoutsCarregamentos[index].Pallets.val(n2(info.TotalPalletsCanalEntrega));
                        _knoutsCarregamentos[index].Cubagem.val(n2(info.TotalCubagemCanalEntrega));
                        _knoutsCarregamentos[index].CapacidadePeso.val(n2(info.TotalPeso));
                        _knoutsCarregamentos[index].CapacidadePallets.val(n2(info.TotalPallets));
                        _knoutsCarregamentos[index].CapacidadeCubagem.val(n2(info.TotalCubagem));
                        _knoutsCarregamentos[index].OcupacaoPeso.val(n2((info.TotalPesoCanalEntrega * 100) / info.TotalPeso));
                        _knoutsCarregamentos[index].OcupacaoCubagem.val(n2((info.TotalCubagemCanalEntrega * 100) / info.TotalCubagem));
                        _knoutsCarregamentos[index].OcupacaoPallets.val(n2((info.TotalPalletsCanalEntrega * 100) / info.TotalPallets));
                        _knoutsCarregamentos[index].OcupacaoPesoPallet.val(n2((info.TotalPesoPalletCanalEntrega * 100) / info.TotalPesoComPallet));
                        //Aki, vamos atualizar o peso total do carregamento.
                        if (_sessaoRoteirizador.MontagemCarregamentoPedidoProduto.val()) {

                        }
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        }
    }
}

function n2(value) {
    return Globalize.format(value, "n2");
}

function loadDetalhesCarregamento(callback) {

    _pesquisaResumoCarregamentos = new FiltroResumoCarregamentos();

    _AreaCarregamento = new AreaCarregamento();
    KoBindings(_AreaCarregamento, "knoutAreaCarregamento");

    _detalheCarregamento = new DetalheCarregamento();

    _percentualCarregamentoAutomatico = new PercentualCarregamentoAutomatico();
    KoBindings(_percentualCarregamentoAutomatico, "knockoutPercentualCarregamentoAutomatico");

    _percentualCargaEmLote = new PercentualCargaEmLote();
    KoBindings(_percentualCargaEmLote, "knockoutPercentualCargaEmLote");

    _resumoCarregamentos = new ResumoCarregamentos();
    KoBindings(_resumoCarregamentos, "knoutListaResumoCarregamentos");

    _parametrosMontagemCarregamentoPedidoProduto = new TipoMontagemCarregamentoPedidoProduto();
    KoBindings(_parametrosMontagemCarregamentoPedidoProduto, "knockoutTipoMontagemCarregamentoPedidoProduto");

    _parametrosMontagemCarregamentoPrioridades = new TipoMontagemCarregamentoPrioridades();
    KoBindings(_parametrosMontagemCarregamentoPrioridades, "modalTipoMontagemCarregamentoPrioridades");

    LoadSignalRCarregamentoAutomatico();
    LoadSignalRCargaEmLote();

    loadGridResumoCarregamentos();

    loadSelecaoCarregamentos();

    $.get("Content/Static/MontagemCargaMapa/DetalheCarregamento.html?dyn=" + guid(), function (data) {
        _HTMLDetalheCarregamento = data;
        if (callback != null)
            callback();
    });

    _restricoesClientesCarregamento = new RestricoesClientesCarregamento();
    KoBindings(_restricoesClientesCarregamento, "knockoutRestricoesClientesCarregamento");

    var header = [
        { data: "Codigo", title: Localization.Resources.Cargas.MontagemCargaMapa.Codigo, width: "15%" },
        { data: "Descricao", title: Localization.Resources.Cargas.MontagemCargaMapa.Descricao, width: "85%" },
        { data: "CorVisualizacao", visible: false },
        { data: "DT_FontColor", visible: false }
    ];

    _gridRestricoesClientesCarregamento = new BasicDataTable(_restricoesClientesCarregamento.GridRestricoesClientesCarregamento.id, header, null, { column: 2, dir: "asc" }, null, 25);
}

function loadSelecaoCarregamentos() {

    _selecaoCarregamentos = new SelecaoCarregamentos();
    KoBindings(_selecaoCarregamentos, "knoutSelecaoCarregamentos");

    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: Localization.Resources.Cargas.MontagemCargaMapa.NumeroCarregamento, width: "20%" },
        { data: "ModeloVeicular", title: Localization.Resources.Cargas.MontagemCargaMapa.Modelo, width: "20%" },
        { data: "Peso", title: Localization.Resources.Cargas.MontagemCargaMapa.Peso, width: "20%" },
        { data: "Pallet", title: Localization.Resources.Cargas.MontagemCargaMapa.Pallet, width: "20%" },
        { data: "Cubagem", title: Localization.Resources.Cargas.MontagemCargaMapa.Cubagem, width: "20%" }
    ];

    var configRowsSelect = { permiteSelecao: true, marcarTodos: false, permiteSelecionarTodos: true };

    _gridSelecaoCarregamentos = new BasicDataTable(_selecaoCarregamentos.Grid.id, header, null, null, configRowsSelect);
    _gridSelecaoCarregamentos.CarregarGrid([]);
}

function atualizarResumoCarregamentosClick() {
    _pesquisaResumoCarregamentos.Reload.val(true);
    reloadGridResumoCarregamentos();
}

function loadGridResumoCarregamentos() {

    const configuracoesExportacao = { url: "MontagemCarga/ExportarListaResumoCarregamentos", titulo: Localization.Resources.Cargas.MontagemCargaMapa.ResumoCarregamentos };
    _gridResumoCarregamentos = new GridViewExportacao(_resumoCarregamentos.Grid.id, "MontagemCarga/ListaResumoCarregamentos", _pesquisaResumoCarregamentos, null, configuracoesExportacao, null, 100, null, 100);
    _gridResumoCarregamentos.CarregarGrid();

    const header = [
        { data: "CodigoModeloVeicular", visible: false },
        { data: "DescricaoModeloVeicular", title: Localization.Resources.Cargas.MontagemCargaMapa.Modelo, width: "40%" },
        { data: "QuantidadeUtilizar", title: Localization.Resources.Cargas.MontagemCargaMapa.Disponibilidade, width: "20%" },
        { data: "QuantidadeUtilizado", title: Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeVeiculosUtilizado, width: "20%" },
        { data: "QuantidadeDisponivel", title: Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeVeiculosDisponivel, width: "20%" }
    ];

    _gridFrota = new BasicDataTable(_resumoCarregamentos.GridFrota.id, header, null, null, null, 10);
    _gridFrota.CarregarGrid([]);

    const headerResumo = [
        { data: "NumeroCarregamento", title: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDoCarregamento, width: "10%", widthDefault: "10%", visible: true },
        { data: "DescricaoModeloVeicular", title: Localization.Resources.Cargas.MontagemCargaMapa.Modelo, width: "20%", widthDefault: "20%", visible: true },
        { data: "NomeTransportadora", title: Localization.Resources.Cargas.MontagemCargaMapa.Transportador, width: "20%", widthDefault: "20%", visible: true },
        { data: "KM", title: "KM", width: "5%", widthDefault: "5%", visible: false },
        { data: "ValorMercadoria", title: "Valor Mercadoria", width: "5%", widthDefault: "5%", visible: false },
        { data: "Cubagem", title: "Cubagem", width: "5%", widthDefault: "5%", visible: false },
        { data: "PesoCarregamento", title: Localization.Resources.Cargas.MontagemCargaMapa.Peso, width: "10%", widthDefault: "10%", visible: true },
        { data: "TaxaOcupacaoPeso", title: Localization.Resources.Cargas.MontagemCargaMapa.Ocupacao, width: "5%", widthDefault: "5%", visible: true },
        { data: "QtdeEntregas", title: Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeEntregas, width: "5%", widthDefault: "5%", visible: true },
        { data: "RotaDeEntrega", title: Localization.Resources.Cargas.MontagemCargaMapa.Rota, width: "15%", widthDefault: "15%", visible: true }
    ];

    _gridResumo = new BasicDataTable(_resumoCarregamentos.GridResumo.id, headerResumo, null, null, null, 20, null, null, null, null, null, null, null, null, null, null, null, null, "MontagemCarga/ExportarListaResumoCarregamentos", "grid-resumo");
    _gridResumo.SetPermitirEdicaoColunas(true);
    _gridResumo.SetSalvarPreferenciasGrid(true);
    _gridResumo.CarregarGrid([]);

    const headerResumoValorFrete = [
        { data: "NumeroCarregamento", title: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDoCarregamento, width: "10%" },
        { data: "DescricaoModeloVeicular", title: Localization.Resources.Cargas.MontagemCargaMapa.Modelo, width: "30%" },
        { data: "PesoCarregamento", title: Localization.Resources.Cargas.MontagemCargaMapa.Peso, width: "10%" },
        { data: "ValorPedido", title: Localization.Resources.Cargas.MontagemCargaMapa.ValorPedido, width: "15%" },
        { data: "ValorFrete", title: Localization.Resources.Cargas.MontagemCargaMapa.ValorFrete, width: "15%" },
        { data: "QtdeEntregas", title: Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeEntregas, width: "10%" },
        { data: "PercentualFrete", title: Localization.Resources.Cargas.MontagemCargaMapa.PercentualFrete, width: "10%" }
    ];

    _gridResumoValorFrete = new BasicDataTable(_resumoCarregamentos.GridResumoValorFrete.id, headerResumoValorFrete, null, null, null, 20);
    _gridResumoValorFrete.CarregarGrid([]);
}

function reloadGridResumoCarregamentos() {
    var visible = $("#modal-carregamentos").is(':visible');
    if (visible) {
        exibirPedidosCarregamentos();
    }
    _pesquisaResumoCarregamentos.SessaoRoteirizador.val(_sessaoRoteirizador.Codigo.val());
    if (_pesquisaResumoCarregamentos.Reload.val()) {
        if (_sessaoRoteirizador.MontagemCarregamentoPedidoProduto.val()) {
            _gridResumoCarregamentos.CarregarGrid(function () {
                _pesquisaResumoCarregamentos.Reload.val(false);
            });
        } else if (_sessaoRoteirizador.TipoMontagemCarregamentoVRP.val() != EnumTipoMontagemCarregamentoVrp.SimuladorFrete) {
            //Buscar os JSON do resumo da sessão.
            var data = {
                SessaoRoteirizador: _sessaoRoteirizador.Codigo.val()
            };
            executarReST("MontagemCarga/ListasResumosSessaoRoteirizador", data, function (arg) {
                if (arg.Success) {
                    _resumoCarregamentos.Grid.visible(false);
                    if (arg.Data.tipoResumo == EnumTipoResumoCarregamento.ModeloCargas) {
                        _resumoCarregamentos.GridFrota.visible(true);
                        _resumoCarregamentos.GridResumo.visible(true);
                        _resumoCarregamentos.GridResumoValorFrete.visible(false);
                        _gridFrota.CarregarGrid(arg.Data.frota);
                        _gridResumo.CarregarGrid(arg.Data.cargas);
                        _gridResumoValorFrete.CarregarGrid([]);
                        _pesquisaResumoCarregamentos.Reload.val(false);
                    } else {
                        _resumoCarregamentos.GridFrota.visible(false);
                        _resumoCarregamentos.GridResumo.visible(false);
                        _resumoCarregamentos.GridResumoValorFrete.visible(true);
                        _gridFrota.CarregarGrid([]);
                        _gridResumo.CarregarGrid([]);
                        _gridResumoValorFrete.CarregarGrid(arg.Data.valorFrete);
                        _pesquisaResumoCarregamentos.Reload.val(false);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        }
    }
}


function detalharCarregamentoClick(e, o) {
    if (_menu_carga) _menu_carga.close();
    var pesquisar = !e.GerandoCargaBackground.val();
    if (_carregamento) {
        if (_carregamento.Carregamento.codEntity() == e.Codigo.val()) {
            pesquisar = false;
        }
    }

    if (pesquisar) {

        if (_carregamento.AlteracoesPendentes.val() == true) {
            exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.ExistemAlteracoesNoCarregamentoNaoSalvasDesejaContinuar, function () {
                descartarMarkerCarregamentoNaoSalvo();
                //ocultarModalOutroCarregamento();
                retornoCarregamento({ Codigo: e.Codigo.val() });
            });
        } else {
            //ocultarModalOutroCarregamento();
            retornoCarregamento({ Codigo: e.Codigo.val() });
        }

    } else if (e.GerandoCargaBackground.val() == false) {
        e.InfoCarregamento.cssClass("card card-carga-selecionada no-padding padding-5");
    }
}

function detalharRestricoesClientesClick(e, o) {
    _gridRestricoesClientesCarregamento.CarregarGrid(e.RestricoesClientes.val());
    Global.abrirModal('divModalRestricoesClientesCarregamento');
}

function cancelarRestricoesClientesCarregamentoClick() {
    Global.fecharModal("divModalRestricoesClientesCarregamento");
}

function menuCarregamentoRightClick(e, o) {
    if (_menu_carga) _menu_carga.close();
    //Carregamento selecionado.
    if (_carregamento != null && _carregamento.Carregamento.codEntity() == e.Codigo.val() && e.GerandoCargaBackground.val() == false && !sessaoRoteirizadorFinalizada()) {
        _menu_carga = null;
        menuContextCarregamento(e.Codigo.val(), 0, 0, false, null);
        if (_menu_carga) {
            _menu_carga.$menu[0].style.top = o.clientY + 'px';
            _menu_carga.$menu[0].style.left = o.clientX + 'px';
            _menu_carga.open();
        }
        e.InfoCarregamento.cssClass("card card-carga-selecionada no-padding padding-5");
    }
}

function exibirCarregamentosMapaChange() {
    if (_Carregamentos) {
        var value = this.ExibirTodosCarregamentoMapa.val();
        for (var i in _Carregamentos) {
            showHideCarregamento(_Carregamentos[i].Codigo, value);
        }
    }
}

function desenharPolilinhaRotaApartirOrigemChange() {
    if (_Carregamentos) {
        drawPolylineCarregamentos();
    }
}

function desenharPolilinhaRotaQuandoRoteirizadoChange() {
    if (_Carregamentos) {
        var marcado = false;
        if (_AreaCarregamento != null) {
            marcado = _AreaCarregamento.ExibirPolilinhaRotaQuandoRoteirizado.val();
        }
        if (!marcado) {
            disposeDirection();
        } else {
            var codigo = 0;
            if (_carregamento)
                codigo = _carregamento.Carregamento.codEntity();
            if (codigo > 0) {
                var index = obterIndiceCarregamentoCodigo(codigo);
                if (index >= 0) {
                    drawPolylineDirection(_Carregamentos[index].Roteirizacao.PolilinhaRota);
                }
            }
        }
    }
}

function exibirCarregamentoMapaChange(e) {
    drawPolylineCarregamentos();
    var codigo = e.Codigo.val();
    var exibir = e.ExibirCarregamentoMapa.val();
    showHideCarregamento(codigo, exibir);
}

function showHideCarregamento(codigo, show) {
    _carregamentoVisivelMapa[codigo] = show;
    var index = obterIndiceKnoutCarregamento(codigo);
    if (index >= 0) {
        _knoutsCarregamentos[index].ExibirCarregamentoMapa.val(show);
    }
    if (_polylineCargas) {
        if (_polylineCargas[codigo]) {
            _polylineCargas[codigo].setVisible(show);
        }
    }
    var markers = obterMarkersSelecionados();
    if (markers != null) {
        for (var i in markers) {
            if (markers[i].codigo_carregamento == codigo)
                markers[i].marker.setVisible(show);
        }
    }
    if (_arrayMarker) {
        for (var i in _arrayMarker) {
            if (_arrayMarker[i].codigo_carregamento == codigo)
                _arrayMarker[i].marker.setVisible(show);
        }
    }
}

function descartarMarkerCarregamentoNaoSalvo() {
    // Pegar todos os PEDIDOS_SELECIONADOS que não estão no _carregamento.Pedidos.val()
    if (_carregamento) {

        var index = obterIndiceCarregamentoCodigo(_carregamento.Carregamento.codEntity());

        if (index >= 0) {

            var filtered = _Carregamentos[index].Roteirizacao.Pedidos.filter(
                function (e) {
                    return this.indexOf(e) < 0;
                },
                _carregamento.Pedidos.val().map(function (a) { return a.Codigo; })
            );

            if (filtered) {

                var codigosCarregamentos = [];

                for (var i = 0; i < filtered.length; i++) {
                    var marker = getMarkerFromPedido(filtered[i]);
                    if (marker != null) {
                        if (marker.codigo_carregamento != 0) {
                            if (!(marker.codigo_carregamento in codigosCarregamentos)) {
                                codigosCarregamentos.push(marker.codigo_carregamento);
                            }
                        }
                        marker.codigo_carregamento = 0;
                    }
                    // Remover do array Roteirização...
                    _Carregamentos[index].Roteirizacao.Pedidos = _Carregamentos[index].Roteirizacao.Pedidos.filter(function (item) {
                        return item !== filtered[i]
                    });
                }

                if (codigosCarregamentos.length > 0) {
                    drawPolylineCarregamentos();
                }
            }
        }
    }
}

function detalharCarregamentoClickPolyline(codigo) {
    if (_carregamento.AlteracoesPendentes.val() == true) {
        exibirConfirmacao(Localization.Resources.Gerais.Geral.SucessoConfirmacao, Localization.Resources.Cargas.MontagemCargaMapa.ExistemAlteracoesNoCarregamentoNaoSalvasDesejaContinuar, function () {
            descartarMarkerCarregamentoNaoSalvo();
            resetColorPolylines();
            retornoCarregamento({ Codigo: codigo });
        });
    } else {
        resetColorPolylines();
        retornoCarregamento({ Codigo: codigo });
    }
}

function calcWidthBodyCarregamentosScroll() {
    var width = '100%';
    if (_AreaCarregamento) {
        width = (_AreaCarregamento.Total.val() * 390) + 'px';
    }
    return width;
}

function validaOpcoesCarregamentosSessaoFinalizada() {
    if (_sessaoRoteirizador != null) {
        if (_sessaoRoteirizador.SituacaoSessaoRoteirizador.val() == 2) { // Finalizada
            _AreaCarregamento.GerarCargaEmLote.visible(false);
            _AreaCarregamento.CancelarTodosCarregamentos.visible(false);
            _AreaCarregamento.GerarCarregamentoAutomatico.visible(false);
        } else {
            _AreaCarregamento.GerarCarregamentoAutomatico.visible(true);
        }
    }
}

function buscarCarregamentos() {
    if (_carregamento.TipoMontagemCarga.val() == EnumTipoMontagemCarga.NovaCarga) {

        var data = RetornarObjetoPesquisa(_pesquisaMontegemCarga);

        data.Inicio = _AreaCarregamento.Inicio.val();
        data.Limite = 500;
        data.SituacaoCarregamento = JSON.stringify(EnumSituacaoCarregamento.obterSituacoesEmMontagem());
        data.TipoMontagemCarga = EnumTipoMontagemCarga.Todos;

        if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.AgruparCargas)
            data.TipoMontagemCarga = _CONFIGURACAO_TMS.TipoMontagemCargaPadrao;

        if (_sessaoRoteirizador != null) {
            data.SituacaoSessaoRoteirizador = _sessaoRoteirizador.SituacaoSessaoRoteirizador.val();
        }

        _AreaCarregamento.CarregandoCarregamentos.val(true);
        executarReST("MontagemCarga/BuscarCarregamentos", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    var retorno = arg.Data;
                    _Carregamentos = retorno;
                    if (_Carregamentos.length > 0) {
                        _AreaCarregamento.GerarCargaEmLote.visible(true);
                        _AreaCarregamento.CancelarTodosCarregamentos.visible(true);
                    } else {
                        _AreaCarregamento.GerarCargaEmLote.visible(false);
                        _AreaCarregamento.CancelarTodosCarregamentos.visible(false);
                    }

                    validaOpcoesCarregamentosSessaoFinalizada();

                    _AreaCarregamento.Total.val(arg.QuantidadeRegistros);
                    _AreaCarregamento.TotalCarregamentos.val(arg.QuantidadeRegistros);
                    _AreaCarregamento.Inicio.val(_AreaCarregamento.Inicio.val() + data.Limite);
                    for (var i = 0; i < retorno.length; i++) {
                        var carregamento = retorno[i];
                        gerarKnoutDetalheCarregamento(carregamento);
                    }
                    AtualizarKMTotalCarregamentos();
                    drawPolylineCarregamentos();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
            _AreaCarregamento.CarregandoCarregamentos.val(false);
        });
    }
}

function AtualizarKMTotalCarregamentos() {
    var km = 0;
    var volume = 0;
    if (_Carregamentos != null) {
        for (var i = 0; i < _Carregamentos.length; i++) {
            km += _Carregamentos[i].Roteirizacao.DistanciaKM;
            volume += _Carregamentos[i].Peso;
        }
    }
    if (_AreaCarregamento != null) {
        _AreaCarregamento.TotalKm.val(Globalize.format(km, "n2") + " km");
        _AreaCarregamento.VolumeKm.val('(' + Globalize.format(volume, "n2") + ') - ' + Globalize.format(volume / (km > 0 ? km : 1), "n2"));
    }
}

function gerarKnoutDetalheCarregamento(carregamento) {
    var index = obterIndiceKnoutCarregamento(carregamento.Codigo);

    if (index < 0) {
        var knoutDetalheCarregamento = new DetalheCarregamento();
        var html = _HTMLDetalheCarregamento.replace(/#detalheCarregamento/g, knoutDetalheCarregamento.InfoCarregamento.id);
        $("#" + _AreaCarregamento.Carregamentos.id).append(html);
        KoBindings(knoutDetalheCarregamento, knoutDetalheCarregamento.InfoCarregamento.id);
        var dataKnout = { Data: carregamento };
        PreencherObjetoKnout(knoutDetalheCarregamento, dataKnout);

        //Ajustando os tipos de pedidos do carregamento
        var tipos = [
            { active: 'nav-link', icon: 'fal fa-lg fa-truck', tipo: 'TOTAL', value: 0, carregamento: carregamento.Codigo }
        ];
        if (carregamento.CanaisDeEntrega != null) {
            for (var i = 0; i < carregamento.CanaisDeEntrega.length; i++) {
                tipos.push({ active: 'nav-link', icon: 'fal fa-lg fa-list', tipo: carregamento.CanaisDeEntrega[i].Descricao, value: carregamento.CanaisDeEntrega[i].Codigo, carregamento: carregamento.Codigo });
            }
        }

        knoutDetalheCarregamento.TiposDePedidos.val(tipos);
        knoutDetalheCarregamento.ValorFreteMontagem.visible(_sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.SimuladorFrete || _sessaoRoteirizador.SimuladorFreteCriterioSelecaoTransportador.val() != EnumSimuladorFreteCriterioSelecaoTransportador.Nenhum);

        knoutDetalheCarregamento.Peso.val(carregamento.PesoSaldoRestante);
        knoutDetalheCarregamento.ModeloVeicularCarga.val(carregamento.Carregamento.Carregamento.ModeloVeicularCarga.Descricao);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
            knoutDetalheCarregamento.NumeroCarregamento.visible(true);
            if (carregamento.CarregamentoPrioritario)
                knoutDetalheCarregamento.InfoCarregamento.cssClass("card card-carga-prioritaria no-padding padding-5");
        }

        if (carregamento.Carregamento.Carregamento.Situacao == EnumSituacaoCarregamento.AguardandoAprovacaoSolicitacao)
            knoutDetalheCarregamento.InfoCarregamento.cssClass("card card-carga-aguardando-aprovacao-solicitacao no-padding padding-5");
        else if (carregamento.Carregamento.Carregamento.Situacao == EnumSituacaoCarregamento.SolicitacaoReprovada)
            knoutDetalheCarregamento.InfoCarregamento.cssClass("card card-carga-solicitacao-reprovada no-padding padding-5");

        //Frete
        knoutDetalheCarregamento.ValorFreteSimulado.visible(carregamento.Frete.Sucesso);
        knoutDetalheCarregamento.ValorFreteSimulado.val(_formatterCurrency.format(carregamento.Frete.ValorFrete));
        knoutDetalheCarregamento.PossuiProdutoPalletFechado.val(carregamento.Carregamento.PossuiProdutoPalletFechado);
        //Peso e informacoes de carregamento
        var peso = 0;
        var pesoPallet = 0;
        var cubagem = 0;
        var pallets = 0;
        var destinos = [];
        var qtde_iddemanda = 0;
        for (var i = 0; i < carregamento.Carregamento.Carregamento.Pedidos.length; i++) {
            var pedido = carregamento.Carregamento.Carregamento.Pedidos[i];
            peso += pedido.PesoPedidoCarregamento;
            pesoPallet += pedido.PesoPalletPedidoCarregamento;
            cubagem += pedido.CubagemPedidoCarregamento;
            pallets += pedido.PalletPedidoCarregamento;

            var destino = pedido.EnderecoDestino.Destinatario;
            var latLng = pedido.EnderecoDestino.Latitude + pedido.EnderecoDestino.Longitude;
            if (pedido.EnderecoRecebedor != null) {
                destino = pedido.EnderecoRecebedor.Destinatario;
                latLng = pedido.EnderecoRecebedor.Latitude + pedido.EnderecoRecebedor.Longitude;
            }

            var chave = destino + latLng;
            if ($.inArray(chave, destinos) < 0) {
                destinos.push(chave);
            }

            if (pedido.QuantidadeIdDemanda != undefined) {
                if (!isNaN(pedido.QuantidadeIdDemanda))
                    qtde_iddemanda += pedido.QuantidadeIdDemanda;
            }
        }
        if (carregamento.Carregamento.MontagemCarregamentoPedidoProduto) {

        }
        var visivel = _carregamentoVisivelMapa[carregamento.Codigo];
        if (visivel != undefined && visivel != null)
            knoutDetalheCarregamento.ExibirCarregamentoMapa.val(visivel);
        else
            _carregamentoVisivelMapa[carregamento.Codigo] = true;

        knoutDetalheCarregamento.Balancas.val(carregamento.Roteirizacao.Balancas);
        knoutDetalheCarregamento.PracasPedagio.val(carregamento.Roteirizacao.PracasPedagio);
        if (_CONFIGURACAO_TMS.MontagemCarga.ExibirAlertaRestricaoEntregaClienteCardCarregamento) {
            knoutDetalheCarregamento.RestricoesClientes.val(carregamento.Carregamento.RestricoesClientesCarregamento);
        } else {
            knoutDetalheCarregamento.RestricoesClientes.val([]);
        }

        ajustarQuantidadesKnoutCarregamento(knoutDetalheCarregamento, carregamento.Carregamento.Carregamento.ModeloVeicularCarga, carregamento.Carregamento.Transporte.TipoDeCarga, peso, cubagem, pallets, destinos.length, carregamento.Roteirizacao.Pedidos.length, qtde_iddemanda, false, carregamento.Roteirizacao);
        _knoutsCarregamentos.push(knoutDetalheCarregamento);

        validaProdutosCarregamento(carregamento.Carregamento);
    }
}

function CarregamentosPesquisaScroll(e, sender) {
    var elem = sender.target;
    if (_AreaCarregamento.Inicio.val() < _AreaCarregamento.Total.val() &&
        elem.scrollTop >= (elem.scrollHeight - elem.offsetHeight)) {
        buscarCarregamentos();
    }
}

function obterIndiceCarregamento(carregamento) {
    if (!NavegadorIEInferiorVersao12()) {
        return _carregamento.Carregamentos.val().findIndex(function (item) { return item.Codigo == carregamento.Codigo });
    } else {
        for (var i = 0; i < _carregamento.Carregamentos.val().length; i++) {
            if (carregamento.Codigo == _carregamento.Carregamentos.val()[i].Codigo)
                return i;
        }
        return -1;
    }
}

function obterIndiceCarregamentoCodigo(codigo) {
    if (!_Carregamentos)
        return -1;

    if (!NavegadorIEInferiorVersao12()) {
        return _Carregamentos.findIndex(function (item) { return item.Codigo == codigo });
    } else {
        for (var i = 0; i < _Carregamentos.length; i++) {
            if (codigo == _Carregamentos[i].Codigo)
                return i;
        }
        return -1;
    }
}

function desmarcarKnoutsCarregamentos() {
    if (_knoutsCarregamentos) {
        for (var i = 0; i < _knoutsCarregamentos.length; i++) {
            var knoutCarregamento = _knoutsCarregamentos[i];
            knoutCarregamento.InfoCarregamento.cssClass("card card-carga no-padding padding-5");
        }
    }
}

function obterIndiceKnoutCarregamento(codigo) {
    if (!_knoutsCarregamentos)
        return -1;

    if (!NavegadorIEInferiorVersao12()) {
        return _knoutsCarregamentos.findIndex(function (item) { return item.Codigo.val() == codigo });
    } else {
        for (var i = 0; i < _knoutsCarregamentos.length; i++) {
            if (codigo == _knoutsCarregamentos[i].Codigo.val())
                return i;
        }
        return -1;
    }
}

function pedidosSemLatLng() {
    var cont = 0;
    for (var i = 0; i < _AreaPedido.Pedidos.val().length; i++) {
        if (_AreaPedido.Pedidos.val()[i].SemLatLng) {
            cont++;
        }
    }
    return cont;
}

function obterListaPedidos() {
    var listaPedidos = [];
    for (var i = 0; i < _AreaPedido.Pedidos.val().length; i++) {
        listaPedidos.push(_AreaPedido.Pedidos.val()[i].Codigo)
    }
    return JSON.stringify(listaPedidos);
}

function filtroCarregamentosClick() {
    Global.abrirModal("divModalFiltroPesquisaCarregamento");
}

function otimizarTodosCarregamentosClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.RealmenteDesejaOtimizarTodosOsCarregamentos, function () {
        otimizarTodosCarregamentos();
    });
}

function cancelarGerarCarregamentosClick() {
    gerarCarregamentosAutomaticos(true);
}

function gerarCarregamentosClick() {
    gerarCarregamentosAutomaticos(false);
}

function gerarCarregamentosAutomaticos(cancelarGerar) {
    var semGeo = pedidosSemLatLng();
    var incompletosBipagem = pedidosIncompletosBipagem();

    if (incompletosBipagem > 0) {
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Cargas.MontagemCargaMapa.ExistemPedidosQueAindaNaoEstaoCompletosNoCarregamentoPorFavorVerifique.format(incompletosBipagem));
        return
    }

    if (semGeo == 0) {

        var mensagem = Localization.Resources.Cargas.MontagemCargaMapa.RealmenteDesejaGerarOsCarregamentosAutomatico;
        if (cancelarGerar) {
            mensagem = Localization.Resources.Cargas.MontagemCargaMapa.RealmenteDesejaCancelarGerarOsCarregamentosAutomatico;
        }

        _cancelarGerarNovamenteCarregamentosAutomaticos = cancelarGerar;

        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, mensagem, function () {
            if (_sessaoRoteirizador.MontagemCarregamentoPedidoProduto.val() == false && _sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() != EnumTipoMontagemCarregamentoVrp.Prioridades) {
                gerarCarregamentosConfirm(0);// AMBOS
            } else if (_sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.Prioridades) {
                Global.abrirModal('modalTipoMontagemCarregamentoPrioridades');
                $("#modalTipoMontagemCarregamentoPrioridades").one('hidden.bs.modal', function () {
                    LimparCampos(_parametrosMontagemCarregamentoPrioridades);
                });
            } else {
                Global.abrirModal('modalTipoMontagemCarregamentoPedidoProduto');
                $("#modalTipoMontagemCarregamentoPedidoProduto").one('hidden.bs.modal', function () {
                    LimparCampos(_parametrosMontagemCarregamentoPedidoProduto);
                });
            }
        });
    } else {
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Cargas.MontagemCargaMapa.ExistemPedidosSemLatitudeLongitudePorFavorVerifique.format(semGeo));
    }
}

function fecharModalTipoMontagemCarregamentoPedidoProduto() {
    Global.fecharModal("modalTipoMontagemCarregamentoPedidoProduto");
}

function fecharModalTipoMontagemCarregamentoPrioridades() {
    Global.fecharModal("modalTipoMontagemCarregamentoPrioridades");
}

function gerarCarregamentosTipoClick() {
    fecharModalTipoMontagemCarregamentoPedidoProduto();
    gerarCarregamentosConfirm(_parametrosMontagemCarregamentoPedidoProduto.Tipo.val(), _parametrosMontagemCarregamentoPedidoProduto.Prioridade.val(), 0, _parametrosMontagemCarregamentoPedidoProduto.TipoStatusEstoque.val());
}

function gerarCarregamentosTipoPrioridadesClick() {
    fecharModalTipoMontagemCarregamentoPrioridades();
    gerarCarregamentosConfirm(0, 0, _parametrosMontagemCarregamentoPrioridades.Prioridade.val());
}

function gerarCarregamentosConfirm(tipo, prioridade, filaPedidoPrioridade, tipoEstoqueProduto) {

    var codigosCarregamentosCancelar = new Array();
    if (_Carregamentos && _cancelarGerarNovamenteCarregamentosAutomaticos) {
        for (var i = 0; i < _Carregamentos.length; i++) {
            codigosCarregamentosCancelar.push(_Carregamentos[i].Codigo);
        }
    }

    var data = { pedidos: obterListaPedidos() };
    data.SessaoRoteirizador = _pesquisaMontegemCarga.SessaoRoteirizador.codEntity();
    data.TipoMontagemCarregamentoPedidoProduto = tipo;
    data.PrioridadeMontagemCarregamentoPedidoProduto = prioridade;
    data.PrioridadeMontagemCarregamentoPedido = filaPedidoPrioridade;
    data.TipoStatusEstoque = tipoEstoqueProduto;
    data.CancelarGerar = _cancelarGerarNovamenteCarregamentosAutomaticos;
    data.CodigosCarregamentosCancelar = JSON.stringify(codigosCarregamentosCancelar);
    _pesquisaResumoCarregamentos.Reload.val(true);
    executarReST("MontagemCarga/GerarCarregamentoAutomatico", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                LimparCamposPercentualCarregamentoAutomatico();
                Global.abrirModal("knockoutPercentualCarregamentoAutomatico");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function cancelarTodosCarregamentoClick() {
    var codigos = new Array();
    if (_Carregamentos) {
        for (var i = 0; i < _Carregamentos.length; i++) {
            codigos.push(_Carregamentos[i].Codigo);
        }
    }
    if (codigos.length > 0) {
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.RealmenteDesejaCancelarTodosOsCarregamentos, function () {
            _pesquisaResumoCarregamentos.Reload.val(true);
            executarReST("MontagemCarga/CancelarCarregamentos", { Codigos: JSON.stringify(codigos) }, function (arg) {
                if (arg.Success) {
                    if (arg.Data !== false) {

                        for (var i = 0; i < codigos.length; i++) {
                            var codigo = codigos[i];
                            var index = obterIndiceCarregamentoCodigo(codigo);
                            if (index >= 0) {
                                _Carregamentos.splice(index, 1);
                            }
                            disposeCarregamentoMapa(codigo);
                        }

                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.CarregamentosCanceladosComSucesso);
                        limparDadosCarregamento();
                        BuscarDadosMontagemCarga(2);
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        });
    }
}

function AddZeros(valor, tamanho) {
    var length = tamanho - valor.toString().length + 1;
    return Array(length).join('0') + valor;
};

function pedidosIncompletosBipagem() {
    var cont = 0;
    for (var i = 0; i < _AreaPedido.Pedidos.val().length; i++) {
        if (parseInt(_AreaPedido.Pedidos.val()[i].QuantidadeBipagemTotal) > 0) {
            if ((parseInt(_AreaPedido.Pedidos.val()[i].QuantidadeBipagemTotal) - parseInt(_AreaPedido.Pedidos.val()[i].QuantidadeBipada)) > 0) {
                cont++;
            }
        }
    }

    return cont;
}

function obterListaCarregamento() {
    var listaCarregamento = [];

    for (var i = 0; i < _knoutsCarregamentos.length; i++) {
        listaCarregamento.push(_knoutsCarregamentos[i].Codigo.val())
    }

    return JSON.stringify(listaCarregamento);
}

function gerarCargaEmLoteClick() {
    var gerarTodas = false;
    if (gerarTodas) {

        var data = { codigosCarregamento: obterListaCarregamento() };
        data.SessaoRoteirizador = _sessaoRoteirizador.Codigo.val();
        gerarCargasEmLote(data);

    } else {

        modalSelecaoCarregamentos(true);

    }
}

function modalSelecaoCarregamentos(show) {
    var listaCarregamento = new Array();
    if (show) {
        for (var i = 0; i < _knoutsCarregamentos.length; i++) {

            var codigo = _knoutsCarregamentos[i].Codigo.val();
            var idx = obterIndiceCarregamentoCodigo(codigo);
            var carregamento = _Carregamentos[idx].Carregamento;
            totalizadoresPedidos = totalizadoresPedidosCarregamento(carregamento.Carregamento.Pedidos);

            var peso = totalizadoresPedidos.peso;
            var cubagem = totalizadoresPedidos.cubagem;
            var pallets = totalizadoresPedidos.pallets;
            var capacidadePeso = Globalize.parseFloat(carregamento.Carregamento.ModeloVeicularCarga.CapacidadePesoTransporte);
            var capacidadeCubagem = Globalize.parseFloat(carregamento.Carregamento.ModeloVeicularCarga.Cubagem);
            var capacidadePallet = Globalize.parseFloat(carregamento.Carregamento.ModeloVeicularCarga.NumeroPaletes);

            listaCarregamento.push({
                Codigo: codigo,
                Numero: _knoutsCarregamentos[i].NumeroCarregamento.val(),
                ModeloVeicular: _knoutsCarregamentos[i].ModeloVeicularCarga.val(),
                Peso: peso.toFixed(2) + ' | ' + parseInt(peso / capacidadePeso * 100) + "%",
                Pallet: pallets.toFixed(2) + ' | ' + parseInt(pallets / capacidadePallet * 100) + "%",
                Cubagem: cubagem.toFixed(2) + ' | ' + parseInt(cubagem / capacidadeCubagem * 100) + "%",
            });
        }
        _gridSelecaoCarregamentos.CarregarGrid(listaCarregamento);
    }
    $("#modalSelecaoCarregamentos").modal((show ? "show" : "hide"));
}

function gerarCargasSelecaoCarregamentosClick() {
    var codigos = _gridSelecaoCarregamentos.ListaSelecionados().map(function (carregamento) {
        return carregamento.Codigo;
    });

    if (codigos.length == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.MontagemCargaMapa.SelecionePeloMenosUmCarregamentoParaGerarCarga);
        return;
    }

    var data = { codigosCarregamento: JSON.stringify(codigos) };
    data.SessaoRoteirizador = _sessaoRoteirizador.Codigo.val();
    modalSelecaoCarregamentos(false);
    gerarCargasEmLote(data);

}

function gerarCargasEmLote(data) {
    executarReST("MontagemCarga/GerarCargaEmLote", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                LimparCamposPercentualCargaEmLote();
                Global.abrirModal('knockoutPercentualCargaEmLote');

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 15000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function SetarCarregamentoAutomaticoFinalizado(dados) {
    Global.fecharModal("knockoutPercentualCarregamentoAutomatico");
    if (dados.erro !== "")
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, dados.erro);
    else {
        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.CarregamentosGeradosComSucesso);
    }
    // MaiorCapacidadeVeicular e QtdePedidosPesoMaior
    if (dados.QtdePedidosPesoMaior > 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Alerta, dados.QtdePedidosPesoMaior + " " + Localization.Resources.Cargas.MontagemCargaMapa.PedidosTemPesoMaiorQueCapacidadeMaximaDoVeiculoDeveSerGeradoManualmente);
    }
    BuscarDadosMontagemCarga(2);
}

function AtualizaPedidosCapacidadeMaior() {

}

function SetarPercentualCarregamentoAutomatico(percentual, descricao) {
    var strPercentual = parseInt(percentual) + "%";
    _percentualCarregamentoAutomatico.PercentualProcessado.val(strPercentual);
    $("#" + _percentualCarregamentoAutomatico.PercentualProcessado.id).css("width", strPercentual);
    if (descricao != undefined) {
        _percentualCarregamentoAutomatico.Descricao.val(descricao);
    } else {
        _percentualCarregamentoAutomatico.Descricao.val(Localization.Resources.Cargas.MontagemCargaMapa.GerandoCarregamentos);
    }
}

function LimparCamposPercentualCarregamentoAutomatico() {
    SetarPercentualCarregamentoAutomatico(0);
    LimparCampos(_percentualCarregamentoAutomatico);
}

function SetarCargaEmLoteFinalizado(dados) {
    Global.fecharModal("knockoutPercentualCargaEmLote");
    if (dados.erro !== "")
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, dados.erro);
    else {
        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.CargasGeradasComSucesso);
    }
    BuscarDadosMontagemCarga(2);
}

function SetarPercentualCargaEmLote(percentual) {
    var strPercentual = parseInt(percentual) + "%";
    _percentualCargaEmLote.PercentualProcessado.val(strPercentual);
    $("#" + _percentualCargaEmLote.PercentualProcessado.id).css("width", strPercentual);
}

function LimparCamposPercentualCargaEmLote() {
    SetarPercentualCargaEmLote(0);
    LimparCampos(_percentualCargaEmLote);
}

function atualizaRibbonKnoutCarregamentoCargaBackgroundFinalizado(codigoCarregamento, sucesso) {

    var index = obterIndiceKnoutCarregamento(codigoCarregamento);
    if (index >= 0) {
        var cssClass = 'ribbon-tms ribbon-tms-green';
        var texto = Localization.Resources.Cargas.MontagemCargaMapa.CargaGerada;
        if (!sucesso) {
            cssClass = 'ribbon-tms ribbon-tms-red';
            texto = Localization.Resources.Cargas.MontagemCargaMapa.ErroAoGerar;
        }
        _knoutsCarregamentos[index].GerandoCargaBackground.cssClass(cssClass);
        _knoutsCarregamentos[index].GerandoCargaBackground.text(texto);
    }
}