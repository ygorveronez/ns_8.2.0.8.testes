/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Empresa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _dreGerencial;

var DREGerencial = function () {
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, def: Global.PrimeiraDataDoMesAnterior(), val: ko.observable(Global.PrimeiraDataDoMesAnterior()), text: "*Mês Anterior:", required: ko.observable(true) });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, def: Global.UltimaDataDoMesAtual(), val: ko.observable(Global.UltimaDataDoMesAtual()), text: "*Mês Atual:", required: ko.observable(true) });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Gerar = PropertyEntity({ eventClick: GerarClick, type: types.event, text: "Gerar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadDREGerencial() {
    _dreGerencial = new DREGerencial();
    KoBindings(_dreGerencial, "knockoutDREGerencial");
}

function GerarClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_dreGerencial);

    if (valido) {
        var data = {
            DataInicial: _dreGerencial.DataInicial.val(),
            DataFinal: _dreGerencial.DataFinal.val()
        };
        executarReST("DREGerencial/BaixarRelatorio", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    BuscarProcessamentosPendentes();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}