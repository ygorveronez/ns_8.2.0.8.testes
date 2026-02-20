/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
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
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _simulacao;

function Simulacao() {
    this.Distancia = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Distancia.getFieldDescription(), val: ko.observable("") });
    this.ValorFrete = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.ValorFrete.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.ValorPorPeso = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.ValorPorKg.getFieldDescription(), val: ko.observable("") });
    this.PercentualSobValorMercadoria = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.PercentualSobValorMercadoria, val: ko.observable("") });
    this.ValorMercadoria = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.ValorMercadoria.getFieldDescription(), val: ko.observable("") });
    this.PesoFrete = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.PesoTotal.getFieldDescription(), val: ko.observable("") });
    this.PesoLiquidoFrete = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.PesoLiquidoTotal.getFieldDescription(), val: ko.observable("") });
    this.RetornoCalculo = PropertyEntity({ val: ko.observable(""), sucesso: ko.observable(false) });
}


//*******EVENTOS*******
function loadSimulacao() {
    _simulacao = new Simulacao();

    CarregarHTMLSimulacao();
}

function simularFreteClick() {
    var codigo = _carregamento.Carregamento.codEntity();
    if (codigo == 0) {
        atualizarCarregamentoClick(simularFreteClick);
    } else {
        var data = {
            Carregamento: JSON.stringify(RetornarObjetoPesquisa(_carregamento)),
            Transporte: JSON.stringify(RetornarObjetoPesquisa(_carregamentoTransporte)),
        };
        executarReST("MontagemCarga/SimularCalculoFrete", data, function (arg) {
            if (arg.Success) {
                if (arg.Data === false) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                } else {                    
                    PreencherObjetoKnout(_simulacao, { Data: arg.Data.Simulacao });
                    _simulacao.ValorFrete.visible(true);
                    _simulacao.RetornoCalculo.sucesso(arg.Data.RetornoSucesso);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

//*******METODOS*******
function CarregarHTMLSimulacao() {
    var html = $("#simulacao-template").html();

    $(".simulacao-container").each(function () {
        var $this = $(this);
        var id = guid();

        $this.html(html).attr("id", id);
        KoBindings(_simulacao, id);
    });
}

function LimparSimulacaoFrete() {
    _simulacao.ValorFrete.visible(false);
    _simulacao.RetornoCalculo.sucesso(false);
    _simulacao.RetornoCalculo.val("");
}