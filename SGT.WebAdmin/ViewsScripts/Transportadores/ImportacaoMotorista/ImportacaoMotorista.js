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
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _importacao;

var Importacao = function () {
    this.TransportadorOrigem = PropertyEntity({ text: "*Transportador Origem:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });
    this.TransportadorDestino = PropertyEntity({ text: "*Transportador Destino:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });

    this.Importar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar" });
}


//*******EVENTOS*******

function loadImportacaoMotorista() {
    _importacao = new Importacao();
    KoBindings(_importacao, "knockoutRelatorio");

    new BuscarTransportadores(_importacao.TransportadorOrigem);
    new BuscarTransportadores(_importacao.TransportadorDestino);
}

function importarClick() {
    if (ValidarCamposObrigatorios(_importacao)) {
        exibirConfirmacao("Atenção!", "Deseja realmente importar os motoristas do transportador " + _importacao.TransportadorOrigem.val() + " para o transportador " + _importacao.TransportadorDestino.val() + "?", function () {
            Salvar(_importacao, "ImportacaoMotorista/Importar", function (r) {
                if (r.Success) {
                    LimparCampos(_importacao);
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Importação realizada com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
                }
            });
        });
    }
}