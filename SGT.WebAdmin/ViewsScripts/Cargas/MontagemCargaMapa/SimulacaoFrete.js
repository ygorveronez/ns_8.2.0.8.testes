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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="../../Enumeradores/EnumTipoMontagemCarregamentoVrp.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _simulacaoFreteMapa;
var _simulacoesFreteMapa;
var _detalhesFrete;
var _gridSimulacoesFretesBlocos;

function Simulacao() {
    this.Distancia = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Distancia.getFieldDescription(), val: ko.observable("") });
    this.ValorFrete = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ValorFrete.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.ValorPorPeso = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ValorPorQuilo.getFieldDescription(), val: ko.observable("") });
    this.PercentualSobValorMercadoria = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PercentualSobRelacaoDaMercadoria.getFieldDescription(), val: ko.observable("") });
    this.ValorMercadoria = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ValorTotalDaMercadoria.getFieldDescription(), val: ko.observable("") });
    this.PesoFrete = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PesoTotal.getFieldDescription(), val: ko.observable("") });
    this.PesoLiquidoFrete = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PesoLiquidoTotal.getFieldDescription(), val: ko.observable("") });
    this.RetornoCalculo = PropertyEntity({ val: ko.observable(""), sucesso: ko.observable(false) });
    this.DetalheFrete = PropertyEntity({ eventClick: detalheFrete, type: types.event, text: "Detalhe Frete", visible: ko.observable(true), enable: ko.observable(true) });
}

var DetalhesFrete = function() {
    this.DetalhesFrete = ko.observableArray([]);;
}

var DetalheFreteModel = function (detalheFrete) {
    this.Formula = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Formula.getFieldDescription(), val: detalheFrete.Formula });
    this.CodigoTabela = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CodigoTabela.getFieldDescription(), val: detalheFrete.CodigoTabela });
    this.Origem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Origem.getFieldDescription(), val: detalheFrete.Origem });
    this.Destino = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Destino.getFieldDescription(), val: detalheFrete.Destino });
    this.DescricaoComponente = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ElementoCusto.getFieldDescription(), val: detalheFrete.DescricaoComponente }); //
    this.ValorCalculado = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ValorCalculado.getFieldDescription(), val: detalheFrete.ValorCalculado });
    this.Valor = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TaxaElemento.getFieldDescription(), val: detalheFrete.Valor });//
    this.ValoresFormula = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.AplicacaoFormula.getFieldDescription(), val: detalheFrete.ValoresFormula });//
}

var SimulacoesFretes = function () {
    this.PesquisarSimulacoesFretesCarregamentos = PropertyEntity({
        eventClick: function (e) {
            pesquisarSimulacoesFretesCarregamentosClick();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: true
    });
    this.AgruparSimulacoesFretesCarregamentos = PropertyEntity({
        eventClick: function (e) {
            agruparSimulacoesFretesCarregamentosClick();
        }, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.AgruparSimulacoesSelecionadas, idGrid: guid(), visible: ko.observable(true)
    });
    this.AgruparVencedoresCarregamentos = PropertyEntity({
        eventClick: function (e) {
            agruparVencedoresCarregamentosClick();
        }, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.AgruparVencedoresSelecionadas, idGrid: guid(), visible: ko.observable(true)
    });
    this.CancelarSimulacaoFreteCarregamentos = PropertyEntity({
        eventClick: function (e) {
            cancelarSimuladorFreteCarregamentosClick();
        }, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.NaoFaturarSelecionadas, idGrid: guid(), visible: ko.observable(true)
    });

    this.Grid = PropertyEntity({ type: types.local });
}

//*******EVENTOS*******
function loadSimulacao() {
    _simulacaoFreteMapa = new Simulacao();
    _detalhesFrete = new DetalhesFrete();

    _simulacoesFreteMapa = new SimulacoesFretes();
    KoBindings(_simulacoesFreteMapa, "knoutSimulacaoFretesBlocos");
    KoBindings(_detalhesFrete, "knoutDetalhesFrete");
    loadGridSimulacaoFretesBlocos();

    CarregarHTMLSimulacao();

    simuladorFreteHabilitaBotoesSomente();
}

function simularFreteClick(showNotification) {
    let data = {
        Carregamento: JSON.stringify(RetornarObjetoPesquisa(_carregamento)),
        Transporte: JSON.stringify(RetornarObjetoPesquisa(_carregamentoTransporte)),
        //Pedidos: JSON.stringify(ObterCodigoPedidosSelecionados())
    };
    executarReST("MontagemCarga/SimularCalculoFrete", data, function (arg) {
        if (arg.Success) {
            if (arg.Data === false) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            } else {
                PreencherObjetoKnout(_simulacaoFreteMapa, { Data: arg.Data.Simulacao });
                preencherDetalheFrete(arg.Data);
                _simulacaoFreteMapa.ValorFrete.visible(true);
                _simulacaoFreteMapa.RetornoCalculo.sucesso(arg.Data.Simulacao.RetornoSucesso);
                //Atualizar o Knout do carregamento com o valor do frete simulado
                if (arg.Data.RetornoSucesso) {
                    let index = obterIndiceKnoutCarregamento(_carregamento.Carregamento.codEntity());
                    if (index >= 0) {
                        _knoutsCarregamentos[index].ValorFreteSimulado.visible(true);
                        _knoutsCarregamentos[index].ValorFreteSimulado.val("R$ " + arg.Data.Simulacao.ValorFrete);
                    }
                } else if (showNotification === true) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Data.Simulacao.RetornoCalculo);
                }
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

//*******METODOS*******
function CarregarHTMLSimulacao() {
    let html = $("#simulacao-template").html();

    $(".simulacao-container").each(function () {
        let $this = $(this);
        let id = guid();

        $this.html(html).attr("id", id);
        KoBindings(_simulacaoFreteMapa, id);
    });
}

function LimparSimulacaoFrete() {
    _simulacaoFreteMapa.ValorFrete.visible(false);
    _simulacaoFreteMapa.RetornoCalculo.sucesso(false);
    _simulacaoFreteMapa.RetornoCalculo.val("");
}

function loadGridSimulacaoFretesBlocos() {

    let header = [
        { data: "Codigo", visible: false },
        { data: "CargaGerada", visible: false },
        { data: "Cliente", title: Localization.Resources.Cargas.MontagemCargaMapa.Cliente, width: "12%", widthDefault: "12%", visible: true, filter: true, name: "Cliente" },
        { data: "Transportador", title: Localization.Resources.Cargas.MontagemCargaMapa.Transportador, width: "20%", widthDefault: "20%", visible: true, filter: true },
        { data: "TipoDeCarga", title: Localization.Resources.Cargas.MontagemCargaMapa.TipoDeCarga, width: "10%", widthDefault: "10%", visible: true, },
        { data: "NumeroCarregamento", title: Localization.Resources.Cargas.MontagemCargaMapa.NumeroCarregamento, width: "10%", widthDefault: "10%", visible: true, },
        { data: "DataCarregamento", title: Localization.Resources.Cargas.MontagemCargaMapa.DataCarregamento, width: "10%", widthDefault: "10%", visible: true, filter: true },
        { data: "GrossSales", title: Localization.Resources.Cargas.MontagemCargaMapa.GrossSales, width: "7%", widthDefault: "7%", visible: true },
        { data: "ValorMinimoCargaCliente", visible: false },
        { data: "Destino", title: Localization.Resources.Cargas.MontagemCargaMapa.Destino, width: "7%", widthDefault: "7%", visible: true, },
        { data: "Estado", title: Localization.Resources.Cargas.MontagemCargaMapa.Estado, width: "1%", widthDefault: "1%", visible: true, filter: true },
        { data: "Regiao", title: Localization.Resources.Cargas.MontagemCargaMapa.Regiao, width: "1%", widthDefault: "1%", visible: true, filter: true },
        { data: "ExigeIsca", title: Localization.Resources.Cargas.MontagemCargaMapa.Isca, width: "1%", widthDefault: "11%", visible: true },
        { data: "ModeloVeicular", title: Localization.Resources.Cargas.MontagemCargaMapa.Modelo, width: "1%", widthDefault: "1%", visible: true },
        { data: "TipoOperacao", title: Localization.Resources.Cargas.MontagemCargaMapa.TipoOperacao, width: "1%", widthDefault: "1%", visible: true, filter: true },
        { data: "PesoTotal", title: Localization.Resources.Cargas.MontagemCargaMapa.PesoTotal, width: "1%", sum: true, sumSelected: true, widthDefault: "1%", visible: true },
        { data: "MetroCubicoTotal", title: Localization.Resources.Cargas.MontagemCargaMapa.MetroCubico, width: "1%", sum: true, sumSelected: true, widthDefault: "1%", visible: true },
        { data: "VolumesTotal", title: Localization.Resources.Cargas.MontagemCargaMapa.Volumes, width: "1%", sum: true, sumSelected: true, widthDefault: "1%", visible: true },
        { data: "Quantidade", title: Localization.Resources.Cargas.MontagemCargaMapa.Quantidade, width: "1%", widthDefault: "1%", visible: true },
        { data: "Ranking", title: Localization.Resources.Cargas.MontagemCargaMapa.Ranking, width: "1%", widthDefault: "1%", visible: true },
        { data: "ValorTotal", title: Localization.Resources.Cargas.MontagemCargaMapa.ValorUnitario, width: "1%", sum: true, sumSelected: true, currency: true, widthDefault: "1%", visible: true },
        { data: "ValorTotalSimulacao", title: Localization.Resources.Cargas.MontagemCargaMapa.ValorTotal, width: "1%", sum: true, sumSelected: true, currency: true, visible: false, widthDefault: "1%" },
        { data: "Situacao", title: Localization.Resources.Cargas.MontagemCargaMapa.Situacao, width: "1%", callbackToolTip: retornoCallbackToolTip, widthDefault: "1%", visible: true },
        { data: "Expedicao", title: Localization.Resources.Cargas.MontagemCargaMapa.Expedicao, width: "1%", filter: true, widthDefault: "1%", visible: true },
        { data: "Vencedor", title: Localization.Resources.Cargas.MontagemCargaMapa.Vencedor, width: "1%", filter: true, widthDefault: "1%", visible: true },
        { data: "Limite", title: Localization.Resources.Cargas.MontagemCargaMapa.Limite, width: "1%", filter: true, widthDefault: "1%", visible: true },
        { data: "LeadTime", title: Localization.Resources.Cargas.MontagemCargaMapa.LeadTime, width: "1%", widthDefault: "1%", visible: true },
        { data: "Observacao", visible: false }
    ];

    let setarVencedor = {
        descricao: Localization.Resources.Cargas.MontagemCargaMapa.DefinirVencedor, id: guid(), evento: "onclick", metodo: function (data) {
            DefinirVencedorFreteClick(data)
        }, tamanho: "7", icone: ""
    };

    let menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [setarVencedor] };

    let configRowsSelect = { permiteSelecao: true, marcarTodos: false, permiteSelecionarTodos: true };

    _gridSimulacoesFretesBlocos = new BasicDataTable(_simulacoesFreteMapa.Grid.id, header, menuOpcoes, null, configRowsSelect, 50);

    _gridSimulacoesFretesBlocos.SetPermitirEdicaoColunas(true);
    _gridSimulacoesFretesBlocos.SetSalvarPreferenciasGrid(true);

    pesquisarSimulacoesFretesCarregamentosClick();
}

function simuladorFreteHabilitaBotoesSomente() {
    let visible = (_sessaoRoteirizador.Codigo.val() > 0 && _sessaoRoteirizador.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.SimuladorFrete);
    _simulacoesFreteMapa.AgruparSimulacoesFretesCarregamentos.visible(visible);
    _simulacoesFreteMapa.AgruparVencedoresCarregamentos.visible(visible);
    _simulacoesFreteMapa.CancelarSimulacaoFreteCarregamentos.visible(visible);
}

function DefinirVencedorFreteClick(sender) {
    if (sender.CargaGerada) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.MontagemCargaMapa.CargaDoCarregamentoSelecionadoJaFoiGerada);
        return;
    }

    let data = { Codigo: sender.Codigo };
    executarReST("SessaoRoteirizador/InformarVencedorSimulacaoFrete", data, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
            pesquisarSimulacoesFretesCarregamentosClick();
            BuscarDadosMontagemCarga(2);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function reloadGridSimulacaoFretesBlocos() {
    let visible = $("#modal-carregamentos").is(':visible');
    if (visible) {
        exibirPedidosCarregamentos();
    }
    if (_gridSimulacoesFretesBlocos.BuscarRegistros().length == 0) {
        pesquisarSimulacoesFretesCarregamentosClick();
    }
}

function retornoCallbackToolTip(registro) {
    let valor = '';
    if (registro.GrossSales < registro.ValorMinimoCargaCliente)
        valor = Localization.Resources.Cargas.MontagemCargaMapa.ValorMinimoDeCargaDoClienteNaoAtingido.format(registro.ValorMinimoCargaCliente, registro.Cliente);

    if (registro.GrossSales > registro.ValorLimiteNaCargaTipoOperacao && registro.ValorLimiteNaCargaTipoOperacao > 0)
        valor = Localization.Resources.Cargas.MontagemCargaMapa.ValorMaximoDaCargaPorTipoOperacaoExcedido.format(registro.ValorLimiteNaCargaTipoOperacao, registro.TipoOperacao);

    return valor + (registro.Observacao ? registro.Observacao : registro.Situacao);
}

function pesquisarSimulacoesFretesCarregamentosClick() {
    if (_sessaoRoteirizador.Codigo.val() > 0 && (_sessaoRoteirizador.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.SimuladorFrete || _sessaoRoteirizador.SimuladorFreteCriterioSelecaoTransportador.val() != EnumSimuladorFreteCriterioSelecaoTransportador.Nenhum)) {

        let visible = $("#modal-carregamentos").is(':visible');
        if (visible) {
            exibirPedidosCarregamentos();
        }

        let data = {
            Codigo: _sessaoRoteirizador.Codigo.val()
        };
        executarReST("SessaoRoteirizador/ObterSimulacoesFrete", data, function (arg) {
            if (arg.Success) {
                if (arg.Data.Registros.length > 0) {
                    _gridSimulacoesFretesBlocos.CarregarGrid(arg.Data.Registros);
                } else {
                    limparSimuladorFreteGrid();
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function agruparSimulacoesFretesCarregamentosClick() {

    let selecionados = _gridSimulacoesFretesBlocos.ListaSelecionados();
    let perdedoresSelecionados = selecionados.filter(function (item) { return item.Vencedor != 'SIM'; });
    if (perdedoresSelecionados.length > 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.MontagemCargaMapa.SomenteRegistrosDeVencedoresPodemSerAgrupados);
        return;
    }

    let data = { Codigo: _sessaoRoteirizador.Codigo.val(), Codigos: JSON.stringify(selecionados.map(function (item) { return item.Codigo; })) };
    executarReST("SessaoRoteirizador/AgruparCarregamentosSimuladorFrete", data, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
            pesquisarSimulacoesFretesCarregamentosClick();
            BuscarDadosMontagemCarga(2);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function agruparVencedoresCarregamentosClick() {
    let data = {
        Codigo: _sessaoRoteirizador.Codigo.val()
    };
    exibirConfirmacao("Confirmação", "Realmente deseja agrupar os vencedores de cada cliente ?", function () {
        executarReST("SessaoRoteirizador/AgruparVencedoresSimuladorFrete", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
                pesquisarSimulacoesFretesCarregamentosClick();
                BuscarDadosMontagemCarga(2);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function cancelarSimuladorFreteCarregamentosClick() {
    let selecionados = _gridSimulacoesFretesBlocos.ListaSelecionados();
    let data = { Codigo: _sessaoRoteirizador.Codigo.val(), Codigos: JSON.stringify(selecionados.map(function (item) { return item.Codigo; })) };
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar as simulações e carregamentos dos registros selecionados ?", function () {
        executarReST("SessaoRoteirizador/CancelarSimuladorFrete", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
                pesquisarSimulacoesFretesCarregamentosClick();
                BuscarDadosMontagemCarga(2);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function limparSimuladorFreteGrid() {
    if (_gridSimulacoesFretesBlocos)
        _gridSimulacoesFretesBlocos.CarregarGrid([]);
}

function detalheFrete(e) {
    Global.abrirModal("divDetalhesFrete");
}

function preencherDetalheFrete(detalheFrete) {
    PreencherObjetoKnout(_detalhesFrete, { Data: detalheFrete });
    carregarDetalhesFrete(detalheFrete.DetalhesFrete);
}

function carregarDetalhesFrete(detalhesCarga) {
    for (var i = 0; i < detalhesCarga.length; i++) {
        _detalhesFrete.DetalhesFrete.push(new DetalheFreteModel(detalhesCarga[i]));
    }
}