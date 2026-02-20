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
/// <reference path="../../Consultas/Empresa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _consultaValores;

var ConsultaValores = function () {

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Até: ", dateRangeInit: this.DataInicial, getType: typesKnockout.date, enable: ko.observable(true) });

    this.CTesEmitidos = PropertyEntity({ text: "(+) CT-es Emitidos: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.CTesCancelados = PropertyEntity({ text: "(-) CT-es Cancelados: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.CTesAnulados = PropertyEntity({ text: "(-) CT-es Anulados: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.CTesFaturados = PropertyEntity({ text: "(-) CT-es Faturados: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PosicaoDoDia = PropertyEntity({ text: "Posição do dia final: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.Consultar = PropertyEntity({ eventClick: ConsultarClick, type: types.event, text: "Consultar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadConsultaValores() {
    _consultaValores = new ConsultaValores();
    KoBindings(_consultaValores, "knockoutGeracaoConsultaValores");

    LoadValoresContasAReceber();
    LoadValoresPosicaoContasAReceber();
}


function ConsultarClick(e, sender) {
    var data = {
        DataInicial: _consultaValores.DataInicial.val(),
        DataFinal: _consultaValores.DataFinal.val()
    };
    executarReST("ConsultaValores/BuscarDados", data, function (arg) {
        if (arg.Success) {
            //var retorno = arg.Data;
            PreencherObjetoKnout(_consultaValores, arg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function limparCamposConsultaValores() {
    LimparCampos(_consultaValores);
}