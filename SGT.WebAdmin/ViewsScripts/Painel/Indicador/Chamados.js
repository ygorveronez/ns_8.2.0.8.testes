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

var _dadosChamadosObtidos = null;
var _graficoDadosGerais;
var _gridChamadoUltimosChamados;
var _graficoRelacaoPorMotivo;
var _graficosRelacaoPorUsuario = [];
var _graficoRelacaoPorOcorrencia;
var _gridChamadoRelacaoPorOcorrencia;

//*******EVENTOS*******
function LoadChamados() {
    CarregarGridsChamados();
}




//*******MÉTODOS*******
function ObterDadosChamados(submetodo, callback) {
    _controleAutomatico = true;
    if (submetodo == "DadosTotais" || _dadosChamadosObtidos == null) {
        executarReST("Indicador/ObterDadosChamados", null, function (arg) {
            if (arg.Success && arg.Data) {
                _dadosChamadosObtidos = arg.Data;
                ProcessaDadosChamados(submetodo);
                callback();
            }
            //callback();
        });
    } else {
        ProcessaDadosChamados(submetodo);
        callback();
    }
} 

function ProcessaDadosChamados(submetodo) {
    switch (submetodo) {
        case "DadosTotais":
            ExibirSlide("div-chamados-dados-totais");
            break;
        case "ChamadosAtrasados":
            ExibirSlide("div-chamados-chamados-atrasados");
            break;
        case "ValoresPorMotivo":
            ExibirSlide("div-chamados-valores-por-motivo");
            break;
        case "RelacaoPorUsuario":
            ExibirSlide("div-chamados-relacao-por-usuario");
            break;
        case "RelacaoPorOcorrencia":
            ExibirSlide("div-chamados-relacao-por-ocorrencia");
            break;
        default:
            ExibirSlide();
    }

    ChamadosDadosGerais();
    
    _gridChamadoUltimosChamados.CarregarGrid(_dadosChamadosObtidos.UltimosChamados);

    ChamadosRelacaoPorMotivo();

    ChamadosRelacaoPorUsuario();

    //_gridChamadoRelacaoPorOcorrencia.CarregarGrid(_dadosChamadosObtidos.RelacaoPorOcorrencia);
    ChamadosRelacaoPorOcorrencia();
}


//*******DADOS GERAIS*******
function ChamadosDadosGerais() {
    $(".titulo-ultimos-chamados").text(TituloUltimosChamados(_dadosChamadosObtidos.TempoRetroativo));

    var options = {
        type: ChartType.Bar,
        idContainer: "divGraficoChamadosDadosGerais",
        properties: {
            y: 'Quantidade',
            yText: 'Descricao',
            x: 'Descricao',
            xType: ChartPropertyType.int,
            color: "Color"
        },
        margin: {
            top: 70,
            right: 50,
            left: 80,
            bottom: 50
        },
        width: 800,
        height: 600,
        title: "Gráfico de Chamados",
        yTitle: "Quantidade",
        xTitle: "Descrição",
        fileName: "Gráfico de Chamados",
        breadcumbTitle: "Dados Gerais",
        data: [
            {
                Descricao: "Total Abertos (" + _dadosChamadosObtidos.TotalAbertos + ")",
                Quantidade: _dadosChamadosObtidos.TotalAbertos,
                Color: "#85de7b",
            },
            {
                Descricao: "Encerrados no mês (" + _dadosChamadosObtidos.TotalEncerrados + ")",
                Quantidade: _dadosChamadosObtidos.TotalEncerrados,
                Color: "#3d613c",
            },
            {
                Descricao: "Total (" + (_dadosChamadosObtidos.TotalAbertos + _dadosChamadosObtidos.TotalEncerrados) + ")",
                Quantidade: _dadosChamadosObtidos.TotalAbertos + _dadosChamadosObtidos.TotalEncerrados,
                Color: "#967bde",
            }
        ]
    };

    _graficoDadosGerais = new Chart(options);
    _graficoDadosGerais.init();
}

function TituloUltimosChamados(horas) {
    var plural = horas > 1 ? "s" : "";
    var texto = "Abertos a mais de " + horas + "h" + plural;
    
    return texto;
}



//*******CHAMADOS ATRASADOS*******
function CarregarGridsChamados() {
    //Ultimos Chamados
    var headerUltimosChamados  = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número", width: "7%", className: 'text-align-right', orderable: false },
        { data: "Empresa", title: "Transportador", width: "30%", orderable: false },
        { data: "Carga", title: "Carga", width: "13%", orderable: false },
        { data: "Cliente", title: "Cliente", width: "40%", orderable: false },
        { data: "MotivoChamado", title: "Motivo", width: "22%", orderable: false },
        { data: "Responsavel", title: "Responsável", width: "15%", orderable: false },
        { data: "TempoAtraso", title: "Tempo Atraso", width: "14%", className: 'text-align-center', orderable: false }
    ];

    _gridChamadoUltimosChamados = new BasicDataTable("gridUltimosChamados", headerUltimosChamados, null, { column: 0, dir: orderDir.asc }, null, 10);
    _gridChamadoUltimosChamados.CarregarGrid([]);

    //Relacao Por Ocorrencia
    //var headerRelacaoPorOcorrencia = [
    //    { data: "Motivo", title: "Motivo", width: "60%" },
    //    { data: "Aberto", title: "Aberto", width: "20%", className: 'text-align-right' },
    //    { data: "Finalizado", title: "Finalizado", width: "20%", className: 'text-align-right' },
    //];

    //_gridChamadoRelacaoPorOcorrencia = new BasicDataTable("gridRelacaoPorOcorrencia", headerRelacaoPorOcorrencia, null, { column: 0, dir: orderDir.desc }, null, 10);
    //_gridChamadoRelacaoPorOcorrencia.CarregarGrid([]);
}



//*******RELACAO POR MOTIVO*******
function ChamadosRelacaoPorMotivo() {
    var options = {
        type: ChartType.Bar,
        idContainer: "divGraficoChamadosValoresPorMotivo",
        properties: {
            y: 'Valor',
            yText: 'Motivo',
            x: 'Motivo',
            xType: ChartPropertyType.decimal,
            color: "Color"
        },
        margin: {
            top: 70,
            right: 50,
            left: 80,
            bottom: 50
        },
        width: 1600,
        height: 600,
        title: "Valores por Motivo",
        yTitle: "Valor",
        xTitle: "Motivo",
        fileName: "Valores por Motivo",
        breadcumbTitle: "Valores por Motivo",
        data: _dadosChamadosObtidos.RelacaoPorMotivo
    };

    _graficoRelacaoPorMotivo = new Chart(options);
    _graficoRelacaoPorMotivo.init();
}



//*******RELACAO POR USUARIO*******
function ChamadosRelacaoPorUsuario() {
    var nColunas = 3;
    var $container = $("#relacao-por-usuario-container");
    var width = ($container.width() / nColunas) - 30;

    for (var id in _graficosRelacaoPorUsuario) {
        _graficosRelacaoPorUsuario[id].destroy();
    }

    $container.empty();

    _dadosChamadosObtidos.RelacaoPorUsuario.forEach(function (usuario) {
        var _id = "divGraficoChamadosValoresPorUsuario-" + usuario.Codigo;
        var $div = $("<div></div>", {
            class: "col col-sm-" + (12 / nColunas), id: _id
        });

        $container.append($div);

        var data = [];

        if (usuario.Abertos > 0)
            data.push({ label: "Aberto", value: usuario.Abertos, color: "#1cb25b" });

        if (usuario.Finalizados > 0)
            data.push({ label: "Finalizados", value: usuario.Finalizados, color: "#cc8118" });

        var options = {
            type: ChartType.Pie,
            idContainer: _id,
            margin: {
                top: 70,
                right: 50,
                left: 80,
                bottom: 50
            },
            title: usuario.Responsavel,
            data: data,
            width: width,
            height: width * 0.8,
            pieLabels: {
                mainLabel: {
                    fontSize: 25
                }
            }
        };

        _graficosRelacaoPorUsuario[usuario.Codigo] = new Chart(options);
        _graficosRelacaoPorUsuario[usuario.Codigo].init();
    });

    if (_dadosChamadosObtidos.RelacaoPorUsuario.length == 0) {
        var $div = $("<div></div>", {
            class: "col col-sm-12 text-align-center h4"
        }).text("Nenhum registro encontrado");

        $container.append($div);
    }
}



//*******RELACAO POR OCORRENCIA*******
function ChamadosRelacaoPorOcorrencia() {
    //var options = {
    //    type: ChartType.GroupBarHorizontal,
    //    idContainer: "divGraficoChamadosValoresPorOcorrencia",
    //    properties: {
    //        y: 'Motivo',
    //        yText: 'Motivo',
    //        x: [{ prop: 'Aberto', color: "#85de7b", text: "Aberto" }, { prop: 'Finalizado', color: "#b94747", text: "Finalizado" }],
    //        xType: ChartPropertyType.decimal,
    //    },
    //    margin: {
    //        top: 70,
    //        right: 50,
    //        left: 80,
    //        bottom: 50
    //    },
    //    width: 1000,
    //    height: 600,
    //    title: "Valores por Motivo Ocorrência",
    //    yTitle: "",
    //    xTitle: "Valor",
    //    fileName: "Valores por Motivo Ocorrência",
    //    breadcumbTitle: "Valores por Motivo Ocorrência",
    //    data: _dadosChamadosObtidos.RelacaoPorOcorrencia
    //};
    var options = {
        type: ChartType.BarHorizontal,
        idContainer: "divGraficoChamadosValoresPorOcorrencia",
        properties: {
            x: 'Total',
            xType: ChartPropertyType.decimal,
            xText: 'Motivo',
            y: 'Motivo',
            yType: ChartPropertyType.string,
            color: "Color"
        },
        margin: {
            top: 70,
            right: 50,
            left: 80,
            bottom: 50
        },
        width: 1000,
        height: 600,
        title: "Valores por Motivo Ocorrência",
        yTitle: "",
        xTitle: "Valor",
        fileName: "Valores por Motivo Ocorrência",
        breadcumbTitle: "Valores por Motivo Ocorrência",
        data: _dadosChamadosObtidos.RelacaoPorOcorrencia
    };

    _graficoRelacaoPorOcorrencia = new Chart(options);
    _graficoRelacaoPorOcorrencia.init();
}