/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carga.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoCarga.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="OrigemDestino.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

/**
 * Contem as notas fiscais dos pedidos resultantes da busca
 * */
var NOTAS_FISCAIS = ko.observableArray([]);

var _AreaNotaFiscal;
var _gridNotaFiscalMontagemCarga;

var AreaNotaFiscal = function () {
    this.SelecionarTodos = PropertyEntity({ type: types.event, eventClick: selecionarTodasNotasFiscaisClick, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.SelecionarTodos.getFieldDescription()), val: ko.observable(false), visible: true });
    this.CarregandoNotasFiscais = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.TotalNotasFiscais = PropertyEntity({ val: ko.observable(""), def: "", text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TotalNotasFiscais.getFieldDescription()) });
    this.PesoTotal = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PesoTotal.getFieldDescription()), visible: true });
    this.PesoSaldoRestante = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });
    this.PesoLiquidoTotal = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PesoLiquidoTotal.getFieldDescription()), visible: true });
    this.VolumeTotal = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.VolumeTotal.getFieldDescription()), visible: true });
    this.SaldoVolumesRestante = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TotalNotasFiscaisSelecionadas = PropertyEntity({ val: ko.observable(0), def: 0, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TotalNotasFiscaisSelecionadas.getFieldDescription()) });
    this.PesoTotalSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PesoTotalSelecionados.getFieldDescription()), visible: true });
    this.PesoSaldoRestanteSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });
    this.PesoLiquidoTotalSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PesoLiquidoTotalSelecionados.getFieldDescription()), visible: true });
    this.VolumeTotalSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.VolumeTotalSelecionados.getFieldDescription()), visible: true });

    //Visualização Nota Fiscal Grid
    this.TabelaNotaFiscalVisivel = PropertyEntity({ visible: ko.observable(false) });
    this.GridNotaFiscal = PropertyEntity({ type: types.local });
}

//*******EVENTOS*******

function loadAreaNotaFiscal() {
    _AreaNotaFiscal = new AreaNotaFiscal();
    KoBindings(_AreaNotaFiscal, "knoutAreaNotaFiscal");

    _AreaNotaFiscal.TabelaNotaFiscalVisivel.visible(_CONFIGURACAO_TMS.ExibirListagemNotasFiscais && _AreaPedido.TabelaPedidosVisivel.visible());

    controleApresentacaoGridNotaFiscal();
}

function controleApresentacaoGridNotaFiscal() {
    if (_AreaNotaFiscal.TabelaNotaFiscalVisivel.visible()) {
        $("#liAreaNotaFiscal").show();

        inicializarTabelaNotaFiscalMontagemCarga();
    }
}

function inicializarTabelaNotaFiscalMontagemCarga() {

    const header = [
        { data: "Codigo", visible: false },
        { data: "CodigoPedido", visible: false },
        { data: "NumeroNota", title: Localization.Resources.Cargas.MontagemCarga.NumeroNota, width: "5%", widthDefault: "5%", visible: true },
        { data: "Serie", title: Localization.Resources.Cargas.MontagemCarga.Serie, width: "3%", widthDefault: "3%", visible: true },
        { data: "ChaveNota", title: Localization.Resources.Cargas.MontagemCarga.ChaveNFe, width: "10%", widthDefault: "10%", visible: true },
        { data: "DataNota", title: Localization.Resources.Cargas.MontagemCarga.DataNota, width: "5%", widthDefault: "5%", visible: true },
        { data: "Remetente", title: Localization.Resources.Cargas.MontagemCarga.Remetente, width: "10%", widthDefault: "10%", visible: true },
        { data: "Destinatario", title: Localization.Resources.Cargas.MontagemCarga.Destinatario, width: "10%", widthDefault: "10%", visible: true },
        { data: "Origem", title: Localization.Resources.Cargas.MontagemCarga.Origem, width: "5%", widthDefault: "5%", visible: true },
        { data: "Destino", title: Localization.Resources.Cargas.MontagemCarga.Destino, width: "5%", widthDefault: "5%", visible: true },
        { data: "DiasPendentes", title: Localization.Resources.Cargas.MontagemCarga.DiasPendentes, width: "5%", widthDefault: "5%", visible: true },

        { data: "Filial", title: Localization.Resources.Cargas.MontagemCarga.Filial, width: "5%", widthDefault: "5%", visible: true },
        { data: "Expedidor", title: Localization.Resources.Cargas.MontagemCarga.Expedidor, width: "5%", widthDefault: "5%", visible: true },
        { data: "Recebedor", title: Localization.Resources.Cargas.MontagemCarga.Recebedor, width: "5%", widthDefault: "5%", visible: true },
        { data: "Peso", title: Localization.Resources.Cargas.MontagemCarga.Peso, width: "5%", widthDefault: "5%", visible: true },
        { data: "PesoLiquido", title: Localization.Resources.Cargas.MontagemCarga.PesoLiquidoTotal, width: "5%", widthDefault: "5%", visible: true },
        { data: "Volumes", title: Localization.Resources.Cargas.MontagemCarga.QuantidadeVolumes, width: "5%", widthDefault: "5%", visible: true },
        { data: "Cubagem", title: Localization.Resources.Cargas.MontagemCarga.Cubagem, width: "5%", widthDefault: "5%", visible: true },
        { data: "NumeroPedidoEmbarcador", title: Localization.Resources.Cargas.MontagemCarga.NumeroPedidoEmbarcador, width: "5%", widthDefault: "5%", visible: true },
        { data: "GrupoPessoa", title: Localization.Resources.Cargas.MontagemCarga.GrupoPessoa, width: "5%", widthDefault: "5%", visible: true },
    ];

    const configExportacao = {
        url: "MontagemCargaNotaFiscal/ExportarPesquisa",
        btnText: "Exportar Excel",
        funcaoObterParametros: function () {
            return { CodigoPedidos: JSON.stringify(ObterCodigoPedidos()) };
        }
    }

    const menuOpcoes = null;

    const configRowsSelect = { permiteSelecao: true, marcarTodos: false, permiteSelecionarTodos: false };

    _gridNotaFiscalMontagemCarga = new BasicDataTable(_AreaNotaFiscal.GridNotaFiscal.id, header, menuOpcoes, { column: 1, dir: "asc" }, configRowsSelect, 25, null, null, null, null, null, null, configExportacao, null, null, true, tableNotaFiscalMontagemCargaRegistroSelecionadoChange, selecionarTodasNotasFiscaisClick, "Cargas/MontagemCarga", "grid-montagem-carga-nota-fiscal");
    _gridNotaFiscalMontagemCarga.SetPermitirEdicaoColunas(true);
    _gridNotaFiscalMontagemCarga.SetSalvarPreferenciasGrid(true);
    _gridNotaFiscalMontagemCarga.SetScrollHorizontal(true);
    _gridNotaFiscalMontagemCarga.SetHabilitarScrollHorizontal(true, 150);
}

function carregarTabelaNotaFiscalMontagemCarga() {
    if (_gridNotaFiscalMontagemCarga) 
        _gridNotaFiscalMontagemCarga.CarregarGrid(NOTAS_FISCAIS());
}

function PesquisarNotasFiscais() {
    if (!_AreaNotaFiscal.TabelaNotaFiscalVisivel.visible())
        return;

    NOTAS_FISCAIS.removeAll();

    var data = RetornarObjetoPesquisa(_pesquisaMontegemCarga);
    data.CodigoPedidos = JSON.stringify(ObterCodigoPedidos());

    _AreaNotaFiscal.CarregandoNotasFiscais.val(true);

    executarReST("MontagemCargaNotaFiscal/BuscarNotasFiscais", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data !== false) {
                const data = retorno.Data;

                _AreaNotaFiscal.TotalNotasFiscais.val(data.QuantidadeRegistros);

                for (var i = 0; i < data.Registros.length; i++) {

                    var notaFiscal = data.Registros[i];

                    NOTAS_FISCAIS.push(notaFiscal);
                }

                carregarTabelaNotaFiscalMontagemCarga();

                preencherTotalizadoresNotaFiscal(data.Totalizadores);
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);

        _AreaNotaFiscal.CarregandoNotasFiscais.val(false);
    });
}

function tableNotaFiscalMontagemCargaRegistroSelecionadoChange(registro, selecionado) {
    var pedido = { Codigo: registro.CodigoPedido };

    tablePedidosMontagemCargaRegistroSelecionadoChange(pedido, selecionado);

    setarPedidosSelecionadosMontagemCarga();
}

function selecionarNotaFiscalPorPedido(pedido, selecionado) {
    if (!_AreaNotaFiscal.TabelaNotaFiscalVisivel.visible())
        return;

    let notasMesmoPedido = NOTAS_FISCAIS().filter(nota => nota.CodigoPedido === pedido.Codigo);

    let notasSelecionadas = _gridNotaFiscalMontagemCarga.ListaSelecionados();

    if (selecionado)
        notasSelecionadas.push(...notasMesmoPedido.filter(nota => !notasSelecionadas.includes(nota)));
    else
        notasSelecionadas = notasSelecionadas.filter(nota => !notasMesmoPedido.includes(nota));

    _gridNotaFiscalMontagemCarga.SetarSelecionados(notasSelecionadas);

    atualizarTotalizadoresNotaFiscal();
}

function selecionarTodasNotasFiscaisClick() {

    if (NOTAS_FISCAIS().length <= 0)
        return;

    var selecionar = !_AreaNotaFiscal.SelecionarTodos.val();

    var codigosPedidos = ObterCodigoPedidoNotasFiscais();

    for (var i = 0; i < codigosPedidos.length; i++) {
        var codigoPedido = codigosPedidos[i];
        var pedido = { Codigo: codigoPedido };

        tablePedidosMontagemCargaRegistroSelecionadoChange(pedido, selecionar);
    }

    setarPedidosSelecionadosMontagemCarga();    

    _AreaNotaFiscal.SelecionarTodos.val(selecionar);

    atualizarTotalizadoresNotaFiscal();
}

function selecionarTodasNotasFiscais(selecionar) {
    if (!_AreaNotaFiscal.TabelaNotaFiscalVisivel.visible())
        return;

    if (selecionar)
        _gridNotaFiscalMontagemCarga.SetarSelecionados(NOTAS_FISCAIS());
    else
        _gridNotaFiscalMontagemCarga.SetarSelecionados([]);

    atualizarTotalizadoresNotaFiscal();
}

function ObterCodigoPedidoNotasFiscais() {

    var codigoPedidos = NOTAS_FISCAIS().map(function (nota) {
        return nota.CodigoPedido;
    })

    var codigosDistintos = [...new Set(codigoPedidos)];

    return codigosDistintos;
}

function preencherTotalizadoresNotaFiscal(totalizadores) {
    _AreaNotaFiscal.PesoTotal.val(Globalize.format(totalizadores.PesoTotal, "n3"));
    _AreaNotaFiscal.PesoLiquidoTotal.val(Globalize.format(totalizadores.PesoLiquidoTotal, "n3"));
    _AreaNotaFiscal.PesoSaldoRestante.val(Globalize.format(totalizadores.PesoSaldoRestante, "n3"));
    _AreaNotaFiscal.VolumeTotal.val(totalizadores.VolumeTotal);
    _AreaNotaFiscal.SaldoVolumesRestante.val(totalizadores.SaldoVolumesRestante);
}

function atualizarTotalizadoresNotaFiscal() {
    var peso = 0;
    var pesoLiquido = 0;
    var pesoRestante = 0;
    var volume = 0;

    var notasFiscais = _gridNotaFiscalMontagemCarga.ListaSelecionados();

    for (var i in notasFiscais) {
        var notaFiscal = notasFiscais[i];

        peso += notaFiscal.Peso;
        pesoLiquido += notaFiscal.PesoLiquido;
        pesoRestante += notaFiscal.Peso;
        volume += notaFiscal.Volumes;
    }

    // Atualizando totais notas fiscais selecionados area notas fiscais.
    _AreaNotaFiscal.TotalNotasFiscaisSelecionadas.val(notasFiscais.length);
    _AreaNotaFiscal.PesoTotalSelecionados.val(Globalize.format(peso, "n3"));
    _AreaNotaFiscal.PesoLiquidoTotalSelecionados.val(Globalize.format(pesoLiquido, "n3"));
    _AreaNotaFiscal.PesoSaldoRestanteSelecionados.val(Globalize.format(pesoRestante, "n3"));
    _AreaNotaFiscal.VolumeTotalSelecionados.val(volume);
}