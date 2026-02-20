/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoFluxoGestaoPatio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaFluxoHorario;
var _gridFluxoHorario;
var _CRUDRelatorio;
var _relatorioFluxoHorario;
var _situacoesFluxo = [
];

var PesquisaFluxoHorario = function () {
    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date, required: true });
    this.DataFim = PropertyEntity({ text: "Data Fim: ", getType: typesKnockout.date, required: true });

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.EtapaFluxoGestaoPatio = PropertyEntity({ val: ko.observable(EnumEtapaFluxoGestaoPatio.Todas), options: ko.observableArray(_situacoesFluxo), text: "Situação: ", def: EnumEtapaFluxoGestaoPatio.Todas });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.Preview = PropertyEntity({
        eventClick: function (e) {
            if(ValidaDatas())
                _gridFluxoHorario.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}


//*******EVENTOS*******
function loadRelatorioFluxoHorario() {
    _pesquisaFluxoHorario = new PesquisaFluxoHorario();
    KoBindings(_pesquisaFluxoHorario, "knockoutPesquisaFluxoHorario", false, _pesquisaFluxoHorario.Preview.id);

    _CRUDRelatorio = new CRUDRelatorio();
    KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaFluxoHorario", false);

    BuscarFluxoHorario();
    new BuscarFilial(_pesquisaFluxoHorario.Filial);

    _relatorioFluxoHorario = new RelatorioGlobal("Relatorios/FluxoHorario/BuscarDadosRelatorio", null, function () {
        RecarregarSituacoes();

        $("#divConteudoRelatorio").show();
    }, null, null, _pesquisaFluxoHorario);
    
}

function BuscarFluxoHorario() {
    _gridFluxoHorario = new GridView(_pesquisaFluxoHorario.Preview.idGrid, "Relatorios/FluxoHorario/Pesquisa", _pesquisaFluxoHorario, null, null, 24);
}



//*******MÉTODOS******* 
function RecarregarSituacoes() {
    executarReST("FluxoPatio/ObterEtapasDisponiveis", { Tipo: EnumTipoFluxoGestaoPatio.Origem }, function (arg) {
        if (arg.Success && arg.Data !== false) {
            var formatacaoOption = arg.Data.map(function (sit) {
                return {
                    text: sit.Descricao,
                    value: sit.Enumerador
                }
            });
            var situacoesFilial = _situacoesFluxo.concat(formatacaoOption);

            _pesquisaFluxoHorario.EtapaFluxoGestaoPatio.options(situacoesFilial);
        }
    });
}

function ValidaDatas() {
    if (!ValidarCamposObrigatorios(_pesquisaFluxoHorario)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "É obrigatório informar as datas.");
        return false;
    }

    var inicio = parseDate(_pesquisaFluxoHorario.DataInicio.val());
    var fim = parseDate(_pesquisaFluxoHorario.DataFim.val());

    if (inicio > fim) {
        exibirMensagem(tipoMensagem.atencao, "Datas Inválidas", "A data inicial deve ser maior que a data final.");
        return false;
    }

    if (inicio.getMonth() == fim.getMonth()) {
        var totalDias = DiferencaDias(inicio, fim);
        if (totalDias > 30) {
            exibirMensagem(tipoMensagem.atencao, "Datas Inválidas", "A diferença das datas não pode ser maior que 30 dias.");
            return false;
        }
    } else if (fim.getDate() >= inicio.getDate()) {
        exibirMensagem(tipoMensagem.atencao, "Datas Inválidas", "A data final deve ser anterior ao dia " + inicio.getDate() + ".");
        return false;
    }

    return true;
}

function parseDate(str) {
    var expandido = str.split('/');
    return new Date(parseInt(expandido[2]), parseInt(expandido[1]) - 1, parseInt(expandido[0]));
}

function DiferencaDias(dataInicio, dataFim) {
    return Math.round((dataFim - dataInicio) / (1000 * 60 * 60 * 24));
}

function GerarRelatorioPDFClick(e, sender) {
    if (ValidaDatas())
        _relatorioFluxoHorario.gerarRelatorio("Relatorios/FluxoHorario/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    if (ValidaDatas())
        _relatorioFluxoHorario.gerarRelatorio("Relatorios/FluxoHorario/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}