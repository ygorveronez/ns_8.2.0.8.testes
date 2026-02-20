/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../js/Global/Rest.js" />
/// <reference path="../../js/Global/Mensagem.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/bootstrap/bootstrap.js" />
/// <reference path="../../js/libs/jquery.blockui.js" />
/// <reference path="../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _simulacaoFrete;

var SimulacaoFrete = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, required: true, text: "Data Inicial:" });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, required: true, text: "Data Final:" });

    this.DataFinal.dateRangeInit = this.DataInicial;
    this.DataInicial.dateRangeLimit = this.DataFinal;

    this.GerarSimulacao = PropertyEntity({ eventClick: GerarSimulacaoClick, type: types.event, text: "Gerar Simulação", idGrid: guid(), visible: ko.observable(true) });
}

//*******EVENTOS*******

function LoadSimulacaoFrete() {
    _simulacaoFrete = new SimulacaoFrete();
    KoBindings(_simulacaoFrete, "divModalSimulacao");
}

function GerarSimulacaoClick() {
    _simulacaoFrete.Codigo.val(_ajusteTabelaFrete.Codigo.val());
    Salvar(_simulacaoFrete, "AjusteTabelaFrete/GerarSimulacao", function (r) {
        if (r.Success) {
            if (r.Data) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Simulação solicitada com sucesso. Aguarde enquanto a simulação é gerada.");
                Global.fecharModal("divModalSimulacao");
                LimparCampos(_simulacaoFrete);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

//*******METODOS*******

function AbrirTelaGeracaoSimulacao() {
    LimparCampos(_simulacaoFrete);
    Global.abrirModal('divModalSimulacao');
}