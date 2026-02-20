
var _controleAutomatico = true;

var PesquisaIndicadorQuantidadeCarga = function () {
    var dataAtual = moment().format("DD/MM/YYYY");


    this.DataInicial = PropertyEntity({ text: "*Data Inicial:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.DataFinal = PropertyEntity({ text: "*Data Final:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.DataInicial.dateRangeLimit = this.DataFinalCarregamento;
    this.DataFinal.dateRangeInit = this.DataInicialCarregamento;
    this.IndicadorQuantidade = PropertyEntity({});
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.CentrosCarregamento = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, callback: null, url: "CentroCarregamento/ObterTodos", params: { Tipo: 0, Ativo: _statusPesquisa.Todos, OpcaoSemGrupo: false }, text: "Grupo de Pessoas Diferente de: ", options: ko.observable(new Array()) });
}

var _indicadoresQuantidadesCarga, _graficoQuantidades;

var _indiceCentro = 0;

var _timeOutCentros;

function loadIndicadores() {
    LimparTimeOut();
    _indicadoresQuantidadesCarga = new PesquisaIndicadorQuantidadeCarga();
    KoBindings(_indicadoresQuantidadesCarga, "knockoutIndicadoresQuantidadeCarga", false);
    loadSlides();
    LoadChamados();
    InvocarMetodoSequencia();

    $(window).one('hashchange', function () {
        LimparTimeOut();
    });
}

function retornoCentrosCarregamento() {
    carregarCentrosCarregamento(-1);
}

function carregarCentrosCarregamento(indice) {
    ExibirSlide('divConteudoGrafico', 'knockoutIndicadoresQuantidadeCarga', 'divGeralGraficoProdutosExpedidos')
    
    _ControlarManualmenteProgresse = _controleAutomatico;
    if (indice > -1) {
        var filial = _indicadoresQuantidadesCarga.CentrosCarregamento.options()[indice];
        _indicadoresQuantidadesCarga.CentroCarregamento.val(filial.text);
        _indicadoresQuantidadesCarga.CentroCarregamento.codEntity(filial.value);
    } else {

        _indicadoresQuantidadesCarga.CentroCarregamento.val("");
        _indicadoresQuantidadesCarga.CentroCarregamento.codEntity(0);
    }
    carregarIndicadorQuantidadeCargas(function () {
        _indiceCentro = indice;
        _timeOutCentros = setTimeout(function () {
            if (indice == _indicadoresQuantidadesCarga.CentrosCarregamento.options().length - 1) {
                obterProximoIndice();
                InvocarMetodoSequencia();
            } else {
                if (!_pause) {
                    indice++;
                }
                carregarCentrosCarregamento(indice);
            }
        }, _tempoParaTroca);
    });
}

var _graficoIndiceAtraso;

function carregarIndicadorQuantidadeCargas(callback) {

    Salvar(_indicadoresQuantidadesCarga, "Indicador/ObterQuantidadesGerais", function (arg) {
        _ControlarManualmenteProgresse = false;
        _controleAutomatico = true;
        if (arg.Success) {
            if (arg.Data !== false) {
                var quantidades = arg.Data.Quantidades;
                html = "";
                gerarGrafico(quantidades);
                gerarGraficoProdutoExpedido(arg.Data.ProdutosExpedidos);
                var total = 0;
                for (var i = 0; i < quantidades.length; i++) {
                    var situacao = quantidades[i];
                    if (situacao.DescricaoTipo == "Total de Cargas") {
                        total = situacao.Quantidade;
                    }
                }
                for (var i = 0; i < quantidades.length; i++) {
                    var situacao = quantidades[i];
                    var percentual = (situacao.Quantidade * 100) / total;
                    if (situacao.DescricaoTipo != "Total de Cargas" && situacao.DescricaoTipo != "Ag. Confirmação do Transportador" && situacao.DescricaoTipo != "Sem Valor de Frete") {
                        html += '<div class="col-xs-12 col-sm-2 col-md-2 col-lg-2">';
                        html += '<div class="easy-pie-chart" style="color: ' + situacao.Cor + '" data-percent="' + parseInt(percentual) + '" data-pie-size="50">';
                        html += '<span class="percent percent-sign">' + parseInt(percentual) + '</span>';
                        html += '</div>';
                        html += '<span class="easy-pie-title">' + situacao.DescricaoTipo + '</span>';
                        html += '<ul class="smaller-stat hidden-sm pull-right">';
                        html += '<li>';
                        html += '<span class="label" style="background: ' + situacao.Cor + '"><i ></i>' + situacao.Quantidade + '</span>';
                        html += '</li>';
                        html += '</ul>';
                        html += '</div>';
                    }
                }
                $("#" + _indicadoresQuantidadesCarga.IndicadorQuantidade.id).html(html);

                $('.easy-pie-chart').each(function () {
                    var $this = $(this),
                        barColor = $this.css('color') || $this.data('pie-color'),
                        trackColor = $this.data('pie-track-color') || 'rgba(0,0,0,0.04)',
                        size = parseInt($this.data('pie-size')) || 25;
                    $this.easyPieChart({
                        barColor: barColor,
                        trackColor: trackColor,
                        scaleColor: false,
                        lineCap: 'butt',
                        lineWidth: parseInt(size / 8.5),
                        animate: 1500,
                        rotate: -90,
                        size: size,
                        onStep: function (from, to, percent) {
                            $(this.el).find('.percent').text(Math.round(percent));
                        }
                    });
                    $this = null;
                });
                callback();
            } else {
                callback();
            }
        } else {
            callback();
        }
    });
}



function gerarGraficoProdutoExpedido(produtosExpedidos) {

    setTimeout(function () {

        $("#divGraficoProdutosExpedidos").html("");

        var graficosProduto = new Array();
        for (var i = 0; i < produtosExpedidos.length; i++) {
            var produto = produtosExpedidos[i];

            var quantidadesExpedidas = [
                { Quantidade: produto.QuantidadeAExpedir, Descricao: "A expedir", Cor: "#e29e23" },
                { Quantidade: produto.QuantidadeExpedida, Descricao: "Expedida", Cor: "#739e73" },
                { Quantidade: produto.QuantidadeTotal, Descricao: "Total", Cor: "#292929" }
            ];

            var id = "a" + guid();
            var html = '<div class="col-xs-12 col-sm-6 col-md-6 col-lg-3" style="margin-bottom:10px; min-height:290px;"><div class="well no-padding"  id="' + id + '"  style="padding:5px"></div></div>';

            $("#divGraficoProdutosExpedidos").append(html);

            var options = {
                type: ChartType.Bar,
                idContainer: id,
                properties: {
                    y: 'Quantidade',
                    yText: 'Quantidade',
                    yType: ChartPropertyType.decimal,
                    x: 'Descricao',
                    xType: ChartPropertyType.string,
                    color: 'Cor'
                },
                title: produto.Produto,
                data: quantidadesExpedidas
            };
            graficosProduto.push(new Chart(options));
        }

        for (var i = 0; i < graficosProduto.length; i++) {
            graficosProduto[i].init();
        }

    }, 150);
}


function gerarGrafico(quantidades) {
    var options = {
        type: ChartType.BarHorizontal,
        idContainer: "divGraficoIndicadores",
        properties: {
            x: 'Quantidade',
            xType: ChartPropertyType.int,
            y: 'DescricaoTipo',
            yType: ChartPropertyType.string,
            color: 'Cor',
            order: 'Ordem'
        },
        margin: {
            top: 40,
            right: 30,
            left: 200,
            bottom: -10
        },
        title: _indicadoresQuantidadesCarga.CentroCarregamento.val() != "" ? _indicadoresQuantidadesCarga.CentroCarregamento.val() : "TODAS AS FILIAIS",
        fileName: "Gráfico de Quantidades de Cargas",
        knockoutParams: _indicadoresQuantidadesCarga,
        data: quantidades
    };

    _graficoQuantidades = new Chart(options);

    _graficoQuantidades.init();
}
