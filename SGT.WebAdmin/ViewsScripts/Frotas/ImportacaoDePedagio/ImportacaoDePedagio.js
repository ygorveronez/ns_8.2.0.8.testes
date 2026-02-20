/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumModalidadePessoa.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _importacaoDePedagio;

var ImportacaoDePedagio = function () {
    //this.Arquivo = PropertyEntity({ text: "Arquivo:", required: false, getType: typesKnockout.dynamic });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Planilha Sem Parar:", val: ko.observable(""), visible: ko.observable(true) });
    this.Enviar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });

    //this.Importar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadImportacaoDePedagio() {
    _importacaoDePedagio = new ImportacaoDePedagio();
    KoBindings(_importacaoDePedagio, "knockoutImportacaoDePedagio");

    HeaderAuditoria("Pedagio");
}

function importarClick(e, sender) {
    var file = document.getElementById(_importacaoDePedagio.Arquivo.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);

    enviarArquivo("ImportacaoDePedagio/ImportarPedagios?callback=?", { Codigo: 0 }, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.Sucesso)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.Mensagem);
                else
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Data.Mensagem, 90000);

                _importacaoDePedagio.Arquivo.val("");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}
//var data = {
//    file: _importacaoDePedagio.Arquivo.val()
//};

//if (data.file != "") {
//    executarReST("ImportacaoDePedagio/ImportarPedagios", data, function (arg) {
//        if (arg.Success) {

//        } else {
//            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
//        }
//    });
//} else
//    exibirMensagem(tipoMensagem.atencao, "Atenção", "Por favor selecione o arquivo do pedágio SEM PARAR!");
//}

//*******MÉTODOS*******

function limparCamposImportacaoDePedagio() {
    LimparCampos(_importacaoDePedagio);
}
